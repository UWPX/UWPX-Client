using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using DnsClient;
using DnsClient.Protocol;
using Logging;
using Microsoft.Toolkit.Uwp.Connectivity;
using Windows.Networking;
using Windows.Networking.Sockets;
using Windows.Security.Cryptography.Certificates;
using Windows.Storage.Streams;
using XMPP_API.Classes.Network.Events;

namespace XMPP_API.Classes.Network.TCP
{
    public class TcpConnection: AbstractConnection, IDisposable
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        private const string LOGGER_TAG = "[TcpConnection]: ";

        /// <summary>
        /// How many characters should get read at once max.
        /// </summary>
        private const int BUFFER_SIZE = 4096;
        /// <summary>
        /// The timeout in ms for TCP connections.
        /// </summary>
        private const int CONNECTION_TIMEOUT_MS = 3000;
        /// <summary>
        /// The timeout for upgrading to a TLS connection in ms.
        /// </summary>
        private const int TLS_UPGRADE_TIMEOUT_MS = 5000;
        /// <summary>
        /// The timeout for sending data.
        /// </summary>
        private const int SEND_TIMEOUT_MS = 1000;

        /// <summary>
        /// The maximum amount of attempts to connect until we fail.
        /// </summary>
        private const int MAX_CONNECTION_ATTEMPTS = 3;
        /// <summary>
        /// The time in ms between connection attempts.
        /// </summary>
        private const int CONNECTION_ATTEMPT_DELAY = 1000;

        private StreamSocket socket;
        private HostName hostName;

        private DataReader dataReader;
        private DataWriter dataWriter;

        /// <summary>
        /// Disables the TCP connection timeout defined in <see cref="CONNECTION_TIMEOUT_MS"/>.
        /// Default = false.
        /// </summary>
        public bool disableConnectionTimeout;
        /// <summary>
        /// Disables the TLS upgrade timeout defined in <see cref="TLS_UPGRADE_TIMEOUT_MS"/>.
        /// Default = false.
        /// </summary>
        public bool disableTlsUpgradeTimeout;

        private ConnectionError lastConnectionError;

        /// <summary>
        /// Enforces the connection timeout <see cref="disableConnectionTimeout"/>.
        /// </summary>
        private CancellationTokenSource connectTimeoutCTS;
        private CancellationTokenSource readCTS;
        private CancellationTokenSource sendCTS;
        private CancellationTokenSource tlsUpgradeCTS;
        private CancellationTokenSource connectCTS;
        private readonly SemaphoreSlim CONNECT_DISCONNECT_SEMA = new SemaphoreSlim(1, 1);
        private static readonly SemaphoreSlim WRITE_SEMA = new SemaphoreSlim(1, 1);
        private bool disposed;

        public delegate void NewDataReceivedEventHandler(TcpConnection sender, NewDataReceivedEventArgs args);
        public event NewDataReceivedEventHandler NewDataReceived;

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        public TcpConnection(XMPPAccount account) : base(account) { }

        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--
        public StreamSocketInformation GetSocketInfo()
        {
            return socket?.Information;
        }

        #endregion
        //--------------------------------------------------------Misc Methods:---------------------------------------------------------------\\
        #region --Misc Methods (Public)--
        public async Task ConnectAsync()
        {
            if (disposed)
            {
                Logger.Debug(LOGGER_TAG + "Canceling ConnectAsync since we are disposed.");
                return;
            }

            await CONNECT_DISCONNECT_SEMA.WaitAsync();
            if (state != ConnectionState.CONNECTED || state != ConnectionState.CONNECTING)
            {
                SetState(ConnectionState.CONNECTING);
                connectCTS = new CancellationTokenSource();
                try
                {
                    bool result = await ConnectInternalAsync().AsAsyncOperation().AsTask(connectCTS.Token);
                    if (result)
                    {
                        StartReaderTask();
                    }
                }
                catch (TaskCanceledException)
                {
                    Logger.Debug(LOGGER_TAG + "ConnectAsync got canceled.");
                    SetState(ConnectionState.DISCONNECTED);
                    return;
                }
            }
            CONNECT_DISCONNECT_SEMA.Release();
        }

        public async Task DisconnectAsync()
        {
            CancelConnectionAttempt();
            if (state == ConnectionState.CONNECTED || state == ConnectionState.CONNECTING)
            {
                await CONNECT_DISCONNECT_SEMA.WaitAsync();
                SetState(ConnectionState.DISCONNECTING);
                StopReaderTask();
                Cleanup();
                SetState(ConnectionState.DISCONNECTED);
                CONNECT_DISCONNECT_SEMA.Release();
            }
        }

        public async Task UpgradeToTlsAsync()
        {
            await CONNECT_DISCONNECT_SEMA.WaitAsync();
            if (state != ConnectionState.CONNECTED)
            {
                Logger.Error("Unable to upgrade to TLS since the state is not connected: " + state.ToString());
                CONNECT_DISCONNECT_SEMA.Release();
                return;
            }

            DateTime duration = DateTime.Now;
            try
            {
                tlsUpgradeCTS = disableTlsUpgradeTimeout ? new CancellationTokenSource() : new CancellationTokenSource(TLS_UPGRADE_TIMEOUT_MS);
                await socket.UpgradeToSslAsync(SocketProtectionLevel.Tls12, new HostName(account.user.domainPart)).AsTask(tlsUpgradeCTS.Token);
            }
            catch (Exception e)
            {
                SocketErrorStatus socketErrorStatus = SocketError.GetStatus(e.GetBaseException().HResult);
                lastConnectionError = new ConnectionError(socketErrorStatus, "TLS upgrade failed after " + DateTime.Now.Subtract(duration).TotalMilliseconds + "ms!");
                SetState(ConnectionState.ERROR, lastConnectionError);
                throw e;
            }
            finally
            {
                CONNECT_DISCONNECT_SEMA.Release();
            }
        }

        public async void Dispose()
        {
            await DisconnectAsync();
            disposed = true;
        }

        public async Task<bool> SendAsync(string s)
        {
            if (state == ConnectionState.CONNECTED)
            {
                try
                {
                    await WRITE_SEMA.WaitAsync();

                    // Sometimes dataWriter is blocking for an infinite time, so give it a timeout:
                    sendCTS = new CancellationTokenSource(SEND_TIMEOUT_MS);
                    sendCTS.CancelAfter(SEND_TIMEOUT_MS);

                    bool success = true;
                    await Task.Run(async () =>
                    {
                        try
                        {
                            uint l = dataWriter.WriteString(s);

                            // Check if all bytes got actually written to the TCP buffer:
                            if (l < s.Length)
                            {
                                lastConnectionError = new ConnectionError(ConnectionErrorCode.SENDING_FAILED, "Send only " + l + " of " + s.Length + "bytes");
                                Logger.Error(LOGGER_TAG + "Failed to send - " + lastConnectionError.ERROR_MESSAGE + ": " + s);
                                success = false;
                                return;
                            }

                            await dataWriter.StoreAsync();
                            await dataWriter.FlushAsync();
                        }
                        catch (Exception) { }
                    }, sendCTS.Token).ConfigureAwait(false);

                    WRITE_SEMA.Release();

                    if (success)
                    {
                        if (sendCTS is null)
                        {
                            Logger.Error(LOGGER_TAG + "Failed to send - " + nameof(sendCTS) + " is null : " + s);
                            return false;
                        }
                        else if (sendCTS.IsCancellationRequested)
                        {
                            lastConnectionError = new ConnectionError(ConnectionErrorCode.SENDING_FAILED, "IsCancellationRequested");
                            Logger.Error(LOGGER_TAG + "Failed to send - " + lastConnectionError.ERROR_MESSAGE + ": " + s);
                            return false;
                        }
                        Logger.Debug(LOGGER_TAG + "Send to (" + account.serverAddress + "):" + s);
                        return true;
                    }
                }
                catch (TaskCanceledException e)
                {
                    lastConnectionError = new ConnectionError(ConnectionErrorCode.SENDING_FAILED, "TaskCanceledException");
                    if (Logger.logLevel >= LogLevel.DEBUG)
                    {
                        Logger.Error(LOGGER_TAG + "Failed to send - " + lastConnectionError.ERROR_MESSAGE + ": " + s, e);
                    }
                    else
                    {
                        Logger.Error(LOGGER_TAG + "Failed to send message - " + lastConnectionError.ERROR_MESSAGE + ": " + s);
                    }

                    WRITE_SEMA.Release();
                }
                catch (Exception e)
                {
                    lastConnectionError = new ConnectionError(ConnectionErrorCode.SENDING_FAILED, e.Message);
                    if (Logger.logLevel >= LogLevel.DEBUG)
                    {
                        Logger.Error(LOGGER_TAG + "Failed to send - " + lastConnectionError.ERROR_MESSAGE + ": " + s, e);
                    }
                    else
                    {
                        Logger.Error(LOGGER_TAG + "Failed to send - " + lastConnectionError.ERROR_MESSAGE + ": " + s);
                    }

                    WRITE_SEMA.Release();
                }
            }
            return false;
        }

        public async Task<TcpReadResult> ReadAsync()
        {
            if (state != ConnectionState.CONNECTED)
            {
                return new TcpReadResult(TcpReadState.FAILURE, null);
            }

            StringBuilder data = new StringBuilder();

            // Read the first batch:
            uint readCount = await dataReader.LoadAsync(BUFFER_SIZE);

            // To close a TCP connection, the opponent sends a 0 length message:
            if (readCount <= 0)
            {
                return new TcpReadResult(TcpReadState.END_OF_STREAM, null);
            }
            if (dataReader is null)
            {
                return new TcpReadResult(TcpReadState.FAILURE, null);
            }

            while (dataReader.UnconsumedBufferLength > 0)
            {
                data.Append(dataReader.ReadString(dataReader.UnconsumedBufferLength));
            }

            // If there is still data left to read, continue until a timeout occurs or a close got requested:
            while (!(readCTS is null) && !readCTS.IsCancellationRequested && state == ConnectionState.CONNECTED && readCount >= BUFFER_SIZE)
            {
                try
                {
                    readCount = await dataReader.LoadAsync(BUFFER_SIZE).AsTask(readCTS.Token);

                    while (dataReader.UnconsumedBufferLength > 0)
                    {
                        data.Append(dataReader.ReadString(dataReader.UnconsumedBufferLength));
                    }
                }
                catch (OperationCanceledException) { }
            }
            return new TcpReadResult(TcpReadState.SUCCESS, data.ToString());
        }

        /// <summary>
        /// Performs a DNS SRV lookup for the given host and returns a list of <see cref="SrvRecord"/> objects for the "xmpp-client" service.
        /// </summary>
        /// <param name="host">The host the lookup should be performed for.</param>
        /// <returns>A list of <see cref="SrvRecord"/> objects ordered by their priority or an empty list of an error occurred or no entries were found.</returns>
        public static async Task<List<SrvRecord>> DnsSrvLookupAsync(string host)
        {
            Logger.Info(LOGGER_TAG + "Performing DNS SRV lookup for: " + host);
            LookupClient lookup = new LookupClient();
            try
            {
                IDnsQueryResponse response = await lookup.QueryAsync("_xmpp-client._tcp." + host, QueryType.SRV);
                if (response is DnsQueryResponse dnsResponse)
                {
                    IEnumerable<SrvRecord> records = dnsResponse.AllRecords.Where((record) => record is SrvRecord).Select((record) => record as SrvRecord);
                    List<SrvRecord> list = records.ToList();
                    list.Sort((a, b) => a.Priority.CompareTo(b.Priority));
                    return list;
                }
            }
            catch (Exception e)
            {
                Logger.Error(LOGGER_TAG + "Failed to look up DNS SRV record for: " + host, e);
            }
            return new List<SrvRecord>();
        }

        #endregion

        #region --Misc Methods (Private)--
        private void CancelConnectionAttempt()
        {
            if (!disposed)
            {
                connectCTS?.Cancel();
            }
        }

        private void Cleanup()
        {
            connectTimeoutCTS?.Cancel();
            connectTimeoutCTS = null;

            connectCTS?.Cancel();
            connectCTS = null;

            readCTS?.Cancel();
            readCTS = null;

            sendCTS?.Cancel();
            sendCTS = null;

            tlsUpgradeCTS?.Cancel();
            tlsUpgradeCTS = null;

            try
            {
                dataWriter?.DetachStream();
                dataWriter?.Dispose();
                dataWriter = null;
            }
            catch (Exception) { }
            try
            {
                dataReader?.DetachStream();
                dataReader?.Dispose();
                dataReader = null;
            }
            catch (Exception) { }
            socket?.Dispose();
            socket = null;
        }

        private async Task<bool> ConnectInternalAsync()
        {
            for (int i = 1; i <= MAX_CONNECTION_ATTEMPTS; i++)
            {
                // Check if we still should connect:
                if (state != ConnectionState.CONNECTING)
                {
                    Logger.Debug(LOGGER_TAG + "Connection attempt canceled.");
                    return false;
                }

                // Check if the network is up:
                if (!NetworkHelper.Instance.ConnectionInformation.IsInternetAvailable)
                {
                    lastConnectionError = new ConnectionError(ConnectionErrorCode.NO_INTERNET);
                    SetState(ConnectionState.ERROR, lastConnectionError);
                    Logger.Warn(LOGGER_TAG + "Unable to connect to " + account.serverAddress + " - no internet!");
                    return false;
                }

                Logger.Debug(LOGGER_TAG + "Starting a connection attempt.");
                try
                {
                    // Setup socket:
                    socket = new StreamSocket();
                    socket.Control.KeepAlive = true;
                    socket.Control.QualityOfService = SocketQualityOfService.LowLatency;
                    hostName = new HostName(account.serverAddress);

                    // Add all ignored certificate errors:
                    foreach (ChainValidationResult item in account.connectionConfiguration.IGNORED_CERTIFICATE_ERRORS)
                    {
                        socket.Control.IgnorableServerCertificateErrors.Add(item);
                    }

                    // Connect with timeout:
                    connectTimeoutCTS = disableConnectionTimeout ? new CancellationTokenSource() : new CancellationTokenSource(CONNECTION_TIMEOUT_MS);

                    // Start the connection process:
                    await socket.ConnectAsync(hostName, account.port.ToString(), SocketProtectionLevel.PlainSocket).AsTask(connectTimeoutCTS.Token);

                    // Setup stream reader and writer:
                    dataWriter = new DataWriter(socket.OutputStream);
                    dataReader = new DataReader(socket.InputStream) { InputStreamOptions = InputStreamOptions.Partial };

                    // Update account connection info:
                    ConnectionInformation connectionInfo = account.CONNECTION_INFO;
                    connectionInfo.socketInfo = socket?.Information;

                    // Connection successfully established:
                    SetState(ConnectionState.CONNECTED);
                    return true;
                }
                catch (TaskCanceledException e)
                {
                    Logger.Error(LOGGER_TAG + i + " try to connect to " + account.serverAddress + " failed:", e);
                    lastConnectionError = new ConnectionError(ConnectionErrorCode.CONNECT_TIMEOUT, e.Message);
                }
                catch (Exception e)
                {
                    OnConnectionError(e, i);
                }

                if (state == ConnectionState.CONNECTING)
                {
                    if (i < MAX_CONNECTION_ATTEMPTS)
                    {
                        // Wait between connection attempts:
                        Logger.Info(LOGGER_TAG + "Starting delay between connection attempts...");
                        await Task.Delay(CONNECTION_ATTEMPT_DELAY);
                    }
                    else
                    {
                        SetState(ConnectionState.ERROR, lastConnectionError);
                    }
                }
            }
            return false;
        }

        private void StopReaderTask()
        {
            if (readCTS != null && !readCTS.IsCancellationRequested)
            {
                readCTS.Cancel();
            }
        }

        private void StartReaderTask()
        {
            // Ensure no other reader task is running:
            StopReaderTask();

            readCTS = new CancellationTokenSource();

            try
            {
                Task.Run(async () =>
                {
                    TcpReadResult readResult = null;
                    int lastReadingFailedCount = 0;
                    int errorCount = 0;
                    DateTime lastReadingFailed = DateTime.MinValue;

                    while (state == ConnectionState.CONNECTED && errorCount < 3)
                    {
                        try
                        {
                            readResult = await ReadAsync();
                            // Check if reading failed:
                            switch (readResult.STATE)
                            {
                                case TcpReadState.SUCCESS:
                                    lastReadingFailedCount = 0;
                                    errorCount = 0;
                                    Logger.Debug(LOGGER_TAG + "Received from (" + account.serverAddress + "):" + readResult.DATA);

                                    // Trigger the NewDataReceived event:
                                    NewDataReceived?.Invoke(this, new NewDataReceivedEventArgs(readResult.DATA));
                                    break;

                                case TcpReadState.FAILURE:
                                    if (lastReadingFailedCount++ <= 0)
                                    {
                                        lastReadingFailed = DateTime.Now;
                                    }

                                    // Read 5 empty or null strings in an interval lower than 1 second:
                                    double c = DateTime.Now.Subtract(lastReadingFailed).TotalSeconds;
                                    if (lastReadingFailedCount > 5 && c < 1)
                                    {
                                        lastConnectionError = new ConnectionError(ConnectionErrorCode.READING_LOOP);
                                        errorCount = int.MaxValue;
                                        continue;
                                    }
                                    break;

                                case TcpReadState.END_OF_STREAM:
                                    Logger.Info(LOGGER_TAG + "Socket closed because received 0-length message from: " + account.serverAddress);
                                    await DisconnectAsync();
                                    break;
                            }
                        }
                        catch (OperationCanceledException)
                        {
                            lastConnectionError = new ConnectionError(ConnectionErrorCode.READING_CANCELED);
                            errorCount++;
                        }
                        catch (Exception e)
                        {
                            SocketErrorStatus status = SocketErrorStatus.Unknown;
                            if (e is AggregateException aggregateException && aggregateException.InnerException != null)
                            {
                                status = SocketError.GetStatus(e.InnerException.HResult);
                            }
                            else
                            {
                                Exception baseException = e.GetBaseException();
                                if (baseException != null)
                                {
                                    status = SocketError.GetStatus(e.GetBaseException().HResult);
                                }
                                else
                                {
                                    status = SocketError.GetStatus(e.HResult);
                                }
                            }

                            lastConnectionError = new ConnectionError(status, e.Message);
                            switch (status)
                            {
                                // Some kind of connection lost:
                                case SocketErrorStatus.ConnectionTimedOut:
                                case SocketErrorStatus.ConnectionRefused:
                                case SocketErrorStatus.NetworkDroppedConnectionOnReset:
                                case SocketErrorStatus.SoftwareCausedConnectionAbort:
                                case SocketErrorStatus.ConnectionResetByPeer:
                                    errorCount = int.MaxValue;
                                    break;

                                default:
                                    errorCount++;
                                    break;
                            }
                        }
                    }

                    if (errorCount >= 3)
                    {
                        SetState(ConnectionState.ERROR, lastConnectionError);
                    }
                }, readCTS.Token);
            }
            catch (OperationCanceledException)
            {
                // Reader task got canceled
            }
        }

        private void OnConnectionError(Exception e, int connectionTry)
        {
            Logger.Error(LOGGER_TAG + connectionTry + " try to connect to " + account?.serverAddress + " failed:", e);
            SocketErrorStatus socketErrorStatus = SocketError.GetStatus(e.GetBaseException().HResult);
            lastConnectionError = new ConnectionError(socketErrorStatus, e.Message);
        }

        #endregion

        #region --Misc Methods (Protected)--


        #endregion
        //--------------------------------------------------------Events:---------------------------------------------------------------------\\
        #region --Events--


        #endregion
    }
}
