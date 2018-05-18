using Logging;
using System;
using System.Threading;
using System.Threading.Tasks;
using Windows.Networking;
using Windows.Networking.Sockets;
using Windows.Security.Cryptography.Certificates;
using Windows.Storage.Streams;
using XMPP_API.Classes.Network.Events;

namespace XMPP_API.Classes.Network.TCP
{
    public class TCPConnection2 : AbstractConnection2
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
        private const int CONNECTION_TIMEOUT = 3000;
        /// <summary>
        /// The timeout for upgrading to a TLS connection in ms.
        /// </summary>
        private const int TLS_UPGRADE_TIMEOUT = 5000;

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
        /// <summary>
        /// Used to cancel all read operations.
        /// </summary>
        private CancellationTokenSource readingCTS;
        private ConnectionError lastConnectionError;

        public delegate void NewDataReceivedEventHandler(TCPConnection2 connection, NewDataReceivedEventArgs args);

        public event NewDataReceivedEventHandler NewDataReceived;

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
            this.lastConnectionError = null;
            this.connectingCTS = null;
            this.dataReader = null;
            this.dataWriter = null;
            this.socket = null;
            this.readingCTS = null;
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
                            hostName = new HostName(account.serverAddress);

                            // Add all ignored certificate errors:
                            foreach (ChainValidationResult item in account.connectionConfiguration.IGNORED_CERTIFICATE_ERRORS)
                            {
                                socket.Control.IgnorableServerCertificateErrors.Add(item);
                            }

                            // Connect with timeout:
                            connectingCTS = new CancellationTokenSource(CONNECTION_TIMEOUT);
                            await socket.ConnectAsync(hostName, account.port.ToString()).AsTask(connectingCTS.Token);

                            // Setup stream reader and writer:
                            dataWriter = new DataWriter(socket.OutputStream);
                            dataReader = new DataReader(socket.InputStream) { InputStreamOptions = InputStreamOptions.Partial };

                            // Update account connection info:
                            ConnectionInformation connectionInfo = account.CONNECTION_INFO;
                            connectionInfo.socketInfo = socket.Information;

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

            setState(ConnectionState.DISCONNECTING);
            connectingCTS?.Cancel();
            readingCTS?.Cancel();
            tlsUpgradeCTS?.Cancel();
            socket?.Dispose();
            socket = null;
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
            try
            {
                tlsUpgradeCTS = new CancellationTokenSource(TLS_UPGRADE_TIMEOUT);
                await socket.UpgradeToSslAsync(SocketProtectionLevel.Tls12, hostName).AsTask(tlsUpgradeCTS.Token);
            }
            catch (Exception e)
            {
                SocketErrorStatus socketErrorStatus = SocketError.GetStatus(e.GetBaseException().HResult);
                lastConnectionError = new ConnectionError(socketErrorStatus, "TLS upgrade failed!");
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
                    WRITE_SEMA.Wait();
                    dataWriter.WriteString(s);
                    await dataWriter.StoreAsync();
                    await dataWriter.FlushAsync();

                    Logger.Debug("[TCPConnection2]: Send to (" + account.serverAddress + "):" + s);
                    return true;
                }
                catch (Exception e)
                {
                    if (Logger.logLevel >= LogLevel.DEBUG)
                    {
                        Logger.Error("[TCPConnection2]: failed to send: " + s, e);
                    }
                    else
                    {
                        Logger.Error("[TCPConnection2]: failed to send message!", e);
                    }
                }
                finally
                {
                    WRITE_SEMA.Release();
                }
            }
            return false;
        }

        public async Task<TCPReadResult> readAsync()
        {
            string data = "";
            if (state != ConnectionState.CONNECTED)
            {
                return new TCPReadResult(false, null);
            }

            uint readCount = 0;

            // Read the first batch:
            readCount = await dataReader.LoadAsync(BUFFER_SIZE);
            if (readCount <= 0 || dataReader == null)
            {
                return new TCPReadResult(false, null);
            }

            while (dataReader.UnconsumedBufferLength > 0)
            {
                data += dataReader.ReadString(dataReader.UnconsumedBufferLength);
            }

            // If there is still data left to read, continue until a timeout occurs or a close got requested:
            while (!readingCTS.IsCancellationRequested && state == ConnectionState.CONNECTED && readCount >= BUFFER_SIZE)
            {
                try
                {
                    Task<uint> t = dataReader.LoadAsync(BUFFER_SIZE).AsTask(readingCTS.Token);
                    t.Wait(100);
                    readCount = t.Result;

                    while (dataReader.UnconsumedBufferLength > 0)
                    {
                        data += dataReader.ReadString(dataReader.UnconsumedBufferLength);
                    }
                }
                catch (OperationCanceledException) { }
            }

            return new TCPReadResult(true, data);
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
                            if (!readResult.SUCCESS)
                            {
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
                            }
                            else
                            {
                                lastReadingFailedCount = 0;
                                errorCount = 0;
                                Logger.Debug("[TCPConnection2]: Received from (" + account.serverAddress + "):" + readResult.DATA);

                                // Trigger the NewDataReceived event:
                                NewDataReceived?.Invoke(this, new NewDataReceivedEventArgs(readResult.DATA));
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
