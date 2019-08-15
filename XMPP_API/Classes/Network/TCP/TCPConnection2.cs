using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using DnsClient;
using DnsClient.Protocol;
using Logging;
using Windows.Networking;
using Windows.Networking.Sockets;
using Windows.Security.Cryptography.Certificates;
using Windows.Storage.Streams;
using XMPP_API.Classes.Network.Events;

namespace XMPP_API.Classes.Network.TCP
{
    public class TCPConnection2: AbstractConnection2
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
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

        private const int MAX_CONNECTION_TRIES = 3;

        private StreamSocket socket;
        private HostName hostName;

        private DataReader dataReader;
        private DataWriter dataWriter;
        private static readonly SemaphoreSlim WRITE_SEMA = new SemaphoreSlim(1, 1);

        /// <summary>
        /// Used to cancel connectAsync().
        /// </summary>
        private CancellationTokenSource connectingCTS;
        private CancellationTokenSource tlsUpgradeCTS;
        private CancellationTokenSource sendCTS;
        /// <summary>
        /// Used to cancel all read operations.
        /// </summary>
        private CancellationTokenSource readingCTS;
        private ConnectionError lastConnectionError;

        public delegate void NewDataReceivedEventHandler(TCPConnection2 connection, NewDataReceivedEventArgs args);

        public event NewDataReceivedEventHandler NewDataReceived;

        /// <summary>
        /// Disables the TCP connection timeout defined in <see cref="CONNECTION_TIMEOUT_MS"/>.
        /// Default = false.
        /// </summary>
        public bool disableTcpTimeout;
        /// <summary>
        /// Disables the TLS upgrade timeout defined in <see cref="TLS_UPGRADE_TIMEOUT_MS"/>.
        /// Default = false.
        /// </summary>
        public bool disableTlsTimeout;

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        /// <summary>
        /// Basic Constructor
        /// </summary>
        /// <history>
        /// 05/05/2018 Created [Fabian Sauter]
        /// </history>
        public TCPConnection2(XMPPAccount account) : base(account)
        {
            lastConnectionError = null;
            connectingCTS = null;
            dataReader = null;
            dataWriter = null;
            socket = null;
            readingCTS = null;
            disableTcpTimeout = false;
            disableTlsTimeout = false;
        }

        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--
        public StreamSocketInformation getSocketInfo()
        {
            return socket?.Information;
        }

        #endregion
        //--------------------------------------------------------Misc Methods:---------------------------------------------------------------\\
        #region --Misc Methods (Public)--
        public async Task connectAsync()
        {
            switch (state)
            {
                case ConnectionState.DISCONNECTED:
                case ConnectionState.ERROR:
                    setState(ConnectionState.CONNECTING);
                    for (int i = 1; i <= MAX_CONNECTION_TRIES; i++)
                    {
                        try
                        {
                            // Cancel connecting if for example disconnectAsync() got called:
                            if (state != ConnectionState.CONNECTING)
                            {
                                return;
                            }

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
                            if (disableTcpTimeout)
                            {
                                connectingCTS = new CancellationTokenSource();
                            }
                            else
                            {
                                connectingCTS = new CancellationTokenSource(CONNECTION_TIMEOUT_MS);
                            }

                            await socket.ConnectAsync(hostName, account.port.ToString(), SocketProtectionLevel.PlainSocket).AsTask(connectingCTS.Token);

                            // Setup stream reader and writer:
                            dataWriter = new DataWriter(socket.OutputStream);
                            dataReader = new DataReader(socket.InputStream) { InputStreamOptions = InputStreamOptions.Partial };

                            // Update account connection info:
                            ConnectionInformation connectionInfo = account.CONNECTION_INFO;
                            connectionInfo.socketInfo = socket?.Information;

                            // Connection successfully established:
                            if (state == ConnectionState.CONNECTING)
                            {
                                setState(ConnectionState.CONNECTED);
                            }
                            return;
                        }
                        catch (TaskCanceledException e)
                        {
                            Logger.Error("[TCPConnection2]: " + i + " try to connect to " + account.serverAddress + " failed:", e);
                            lastConnectionError = new ConnectionError(ConnectionErrorCode.CONNECT_TIMEOUT, e.Message);
                        }
                        catch (Exception e)
                        {
                            onConnectionError(e, i);
                        }
                    }
                    setState(ConnectionState.ERROR, lastConnectionError);
                    break;

                default:
                    break;
            }
        }

        public void disconnect()
        {
            if (state == ConnectionState.DISCONNECTED)
            {
                return;
            }

            Logger.Info("[TCPConnection2]: Disconnecting");
            setState(ConnectionState.DISCONNECTING);
            connectingCTS?.Cancel();
            readingCTS?.Cancel();
            tlsUpgradeCTS?.Cancel();
            sendCTS?.Cancel();
            try
            {
                dataReader?.DetachStream();
                dataReader?.Dispose();
                dataReader = null;
                dataWriter?.DetachStream();
                dataWriter?.Dispose();
                dataWriter = null;
            }
            catch (Exception)
            {
            }
            socket?.Dispose();
            socket = null;
            Logger.Info("[TCPConnection2]: Disconnected");
            setState(ConnectionState.DISCONNECTED);
        }

        /// <summary>
        /// Upgrades the connection to TLS 1.2.
        /// Timeout is TLS_UPGRADE_TIMEOUT.
        /// </summary>
        public async Task upgradeToTLSAsync()
        {
            if (state != ConnectionState.CONNECTED)
            {
                throw new InvalidOperationException("[TCPConnection2]: Unable to upgrade to TLS! ConnectionState != Connected! state = " + state);
            }
            DateTime d = DateTime.Now;
            try
            {
                if (disableTlsTimeout)
                {
                    tlsUpgradeCTS = new CancellationTokenSource();
                }
                else
                {
                    tlsUpgradeCTS = new CancellationTokenSource(TLS_UPGRADE_TIMEOUT_MS);
                }
                await socket.UpgradeToSslAsync(SocketProtectionLevel.Tls12, hostName).AsTask(tlsUpgradeCTS.Token);
            }
            catch (Exception e)
            {
                SocketErrorStatus socketErrorStatus = SocketError.GetStatus(e.GetBaseException().HResult);
                lastConnectionError = new ConnectionError(socketErrorStatus, "TLS upgrade failed after " + DateTime.Now.Subtract(d).TotalMilliseconds + "ms!");
                setState(ConnectionState.ERROR, lastConnectionError);
                throw e;
            }
        }

        public async Task<bool> sendAsync(string s)
        {
            if (state == ConnectionState.CONNECTED)
            {
                try
                {
                    // Sometimes dataWriter is blocking for an infinite time, so give it a timeout:
                    sendCTS = new CancellationTokenSource(SEND_TIMEOUT_MS);
                    sendCTS.CancelAfter(SEND_TIMEOUT_MS);

                    WRITE_SEMA.Wait();
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
                                Logger.Error("[TCPConnection2]: failed to send - " + lastConnectionError.ERROR_MESSAGE + ": " + s);
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
                        if (sendCTS.IsCancellationRequested)
                        {
                            lastConnectionError = new ConnectionError(ConnectionErrorCode.SENDING_FAILED, "IsCancellationRequested");
                            Logger.Error("[TCPConnection2]: failed to send - " + lastConnectionError.ERROR_MESSAGE + ": " + s);
                            return false;
                        }
                        Logger.Debug("[TCPConnection2]: Send to (" + account.serverAddress + "):" + s);
                        return true;
                    }
                }
                catch (TaskCanceledException e)
                {
                    lastConnectionError = new ConnectionError(ConnectionErrorCode.SENDING_FAILED, "TaskCanceledException");
                    if (Logger.logLevel >= LogLevel.DEBUG)
                    {
                        Logger.Error("[TCPConnection2]: failed to send - " + lastConnectionError.ERROR_MESSAGE + ": " + s, e);
                    }
                    else
                    {
                        Logger.Error("[TCPConnection2]: failed to send message - " + lastConnectionError.ERROR_MESSAGE + ": " + s);
                    }

                    WRITE_SEMA.Release();
                }
                catch (Exception e)
                {
                    lastConnectionError = new ConnectionError(ConnectionErrorCode.SENDING_FAILED, e.Message);
                    if (Logger.logLevel >= LogLevel.DEBUG)
                    {
                        Logger.Error("[TCPConnection2]: failed to send - " + lastConnectionError.ERROR_MESSAGE + ": " + s, e);
                    }
                    else
                    {
                        Logger.Error("[TCPConnection2]: failed to send - " + lastConnectionError.ERROR_MESSAGE + ": " + s);
                    }

                    WRITE_SEMA.Release();
                }
            }
            return false;
        }

        public async Task<TCPReadResult> readAsync()
        {
            if (state != ConnectionState.CONNECTED)
            {
                return new TCPReadResult(TCPReadState.FAILURE, null);
            }

            StringBuilder data = new StringBuilder();
            uint readCount = 0;

            // Read the first batch:
            readCount = await dataReader.LoadAsync(BUFFER_SIZE);

            // To close a TCP connection, the opponent sends a 0 length message:
            if (readCount <= 0)
            {
                return new TCPReadResult(TCPReadState.END_OF_STREAM, null);
            }
            if (dataReader is null)
            {
                return new TCPReadResult(TCPReadState.FAILURE, null);
            }

            while (dataReader.UnconsumedBufferLength > 0)
            {
                data.Append(dataReader.ReadString(dataReader.UnconsumedBufferLength));
            }

            // If there is still data left to read, continue until a timeout occurs or a close got requested:
            while (!readingCTS.IsCancellationRequested && state == ConnectionState.CONNECTED && readCount >= BUFFER_SIZE)
            {
                try
                {
                    readCount = await dataReader.LoadAsync(BUFFER_SIZE).AsTask(readingCTS.Token);

                    while (dataReader.UnconsumedBufferLength > 0)
                    {
                        data.Append(dataReader.ReadString(dataReader.UnconsumedBufferLength));
                    }
                }
                catch (OperationCanceledException)
                {
                }
            }

            return new TCPReadResult(TCPReadState.SUCCESS, data.ToString());
        }

        public void startReaderTask()
        {
            if (state != ConnectionState.CONNECTED)
            {
                throw new InvalidOperationException("[TCPConnection2]: Unable to start reader task! ConnectionState != CONNECTED - state = " + state);
            }

            // Ensure no other reader task is running:
            if (readingCTS != null && !readingCTS.IsCancellationRequested)
            {
                readingCTS.Cancel();
            }

            readingCTS = new CancellationTokenSource();

            try
            {
                Task.Run(async () =>
                {
                    TCPReadResult readResult = null;
                    int lastReadingFailedCount = 0;
                    int errorCount = 0;
                    DateTime lastReadingFailed = DateTime.MinValue;

                    while (state == ConnectionState.CONNECTED && errorCount < 3)
                    {
                        try
                        {
                            readResult = await readAsync();
                            // Check if reading failed:
                            switch (readResult.STATE)
                            {
                                case TCPReadState.SUCCESS:
                                    lastReadingFailedCount = 0;
                                    errorCount = 0;
                                    Logger.Debug("[TCPConnection2]: Received from (" + account.serverAddress + "):" + readResult.DATA);

                                    // Trigger the NewDataReceived event:
                                    NewDataReceived?.Invoke(this, new NewDataReceivedEventArgs(readResult.DATA));
                                    break;

                                case TCPReadState.FAILURE:
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

                                case TCPReadState.END_OF_STREAM:
                                    Logger.Info("Socket closed because received 0-length message from: " + account.serverAddress);
                                    disconnect();
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
                        setState(ConnectionState.ERROR, lastConnectionError);
                    }
                }, readingCTS.Token);
            }
            catch (OperationCanceledException)
            {
                // Reader task got canceled
            }
        }

        /// <summary>
        /// Performs a DNS SRV lookup for the given host and returns a list of <see cref="SrvRecord"/> objects for the "xmpp-client" service.
        /// </summary>
        /// <param name="host">The host the lookup should be performed for.</param>
        /// <returns>A list of <see cref="SrvRecord"/> objects ordered by their priority or an empty list of an error occurred or no entries were found.</returns>
        public static async Task<List<SrvRecord>> dnsSrvLookupAsync(string host)
        {
            Logger.Info("Performing DNS SRV lookup for: " + host);
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
                Logger.Error("[TCPConnection2]: Failed to look up DNS SRV record for: " + host, e);
            }
            return new List<SrvRecord>();
        }

        #endregion

        #region --Misc Methods (Private)--
        private void onConnectionError(Exception e, int connectionTry)
        {
            Logger.Error("[TCPConnection2]: " + connectionTry + " try to connect to " + account?.serverAddress + " failed:", e);
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
