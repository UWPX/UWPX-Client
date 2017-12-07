using Logging;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Windows.Networking;
using Windows.Networking.Sockets;
using XMPP_API.Classes.Network.Events;

namespace XMPP_API.Classes.Network.TCP
{
    class TCPConnection : AbstractConnection
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        /// <summary>
        /// How much characters should get read at once max.
        /// </summary>
        private static readonly int BUFFER_SIZE = 4096;
        /// <summary>
        /// The timeout in ms for TCP connections.
        /// </summary>
        private static readonly int CONNECTION_TIMEOUT = 3000;

        private StreamSocket socket;
        private HostName hostName;
        private StreamWriter writer;
        private StreamReader reader;

        /// <summary>
        /// Used to cancel connectAsync().
        /// </summary>
        private CancellationTokenSource connectingCTS;

        /// <summary>
        /// Used to cancel all read operations.
        /// </summary>
        private CancellationTokenSource readingCTS;

        public delegate void NewDataReceivedEventHandler(AbstractConnection handler, NewDataReceivedEventArgs args);

        public event NewDataReceivedEventHandler NewDataReceived;

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        /// <summary>
        /// Basic Constructor
        /// </summary>
        /// <history>
        /// 07/12/2017 Created [Fabian Sauter]
        /// </history>
        public TCPConnection(XMPPAccount account) : base(account)
        {
        }

        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--


        #endregion
        //--------------------------------------------------------Misc Methods:---------------------------------------------------------------\\
        #region --Misc Methods (Public)--
        public override async Task connectAsync()
        {
            switch (state)
            {
                case ConnectionState.DISCONNECTED:
                case ConnectionState.ERROR:
                    string lastExceptionMessage;
                    setState(ConnectionState.CONNECTING);
                    lastExceptionMessage = "";

                    // Try to connect three times:
                    for (int i = 1; i < 4; i++)
                    {
                        // Cancel connecting if for example disconnectAsync() got called:
                        if (state != ConnectionState.CONNECTING)
                        {
                            return;
                        }

                        try
                        {
                            // Setup socket:
                            socket = new StreamSocket();
                            socket.Control.KeepAlive = true;
                            hostName = new HostName(account.serverAddress);

                            // Connect with timeout:
                            connectingCTS = new CancellationTokenSource();
                            Task t = socket.ConnectAsync(hostName, account.port.ToString()).AsTask();
                            int waitResult = Task.WaitAny(t, Task.Delay(CONNECTION_TIMEOUT, connectingCTS.Token));
                            connectingCTS = null;
                            if (waitResult != 0)
                            {
                                throw new TimeoutException("ConnectAsync timed out! CONNECTION_TIMEOUT = " + CONNECTION_TIMEOUT);
                            }

                            // Setup writer:
                            writer = new StreamWriter(socket.OutputStream.AsStreamForWrite());

                            // Setup Reader:
                            reader = new StreamReader(socket.InputStream.AsStreamForRead());

                            // Connection successfully established:
                            setState(ConnectionState.CONNECTED);
                            return;
                        }
                        catch (TimeoutException e)
                        {
                            Logger.Error("[TCPConnection]: " + i + " try to connect to " + account.serverAddress + "- TimeoutException - " + e.Message);
                            lastExceptionMessage = SocketErrorStatus.ConnectionTimedOut.ToString();
                            await cleanupAsync();
                        }
                        catch (Exception e)
                        {
                            SocketErrorStatus socketErrorStatus = SocketError.GetStatus(e.GetBaseException().HResult);
                            if (socketErrorStatus == SocketErrorStatus.Unknown)
                            {
                                Logger.Error("[TCPConnection]: " + i + " try to connect to " + account.serverAddress + "- Unknown Exception - ", e);
                            }
                            else
                            {
                                Logger.Error("[TCPConnection]: " + i + " try to connect to " + account.serverAddress + "- Exception - " + socketErrorStatus.ToString());
                            }
                            lastExceptionMessage = socketErrorStatus.ToString();
                            await cleanupAsync();
                            throw;
                        }
                    }
                    setState(ConnectionState.ERROR, lastExceptionMessage);
                    break;
                default:
                    // Throw an exception, because the server is already trying to connect:
                    throw new InvalidOperationException("[TCPConnection]: Unable to connect! state != Error or Disconnected! state = " + state);
            }
        }

        public async override Task disconnectAsync()
        {
            switch (state)
            {
                case ConnectionState.CONNECTING:
                case ConnectionState.CONNECTED:
                    setState(ConnectionState.DISCONNECTING);
                    // Stop all connectAsync():
                    if (connectingCTS != null)
                    {
                        connectingCTS.Cancel();
                    }

                    // Cleanup:
                    await cleanupAsync();
                    setState(ConnectionState.DISCONNECTED);
                    break;
            }
        }

        /// <summary>
        /// Upgrades the connection to TLS 1.2.
        /// </summary>
        public async Task upgradeToTLSAsync()
        {
            if (state != ConnectionState.CONNECTED)
            {
                throw new InvalidOperationException("[TCPConnection]: Unable to upgrade to TLS! ConnectionState != Connected! state = " + state);
            }

            await socket.UpgradeToSslAsync(SocketProtectionLevel.Tls12, hostName);
        }

        public async Task sendAsync(string msg)
        {
            if (state != ConnectionState.CONNECTED)
            {
                throw new InvalidOperationException("[TCPConnection]: Unable to send message! ConnectionState != Connected! state = " + state);
            }
            await writer.WriteLineAsync(msg);
            await writer.FlushAsync();
            if (Consts.ENABLE_DEBUG_OUTPUT)
            {
                Logger.Info("Send to (" + account.serverAddress + "):" + msg);
            }
        }

        /// <summary>
        /// Starts a new task that reads all incoming messages.
        /// </summary>
        public void startReaderTask()
        {
            if (state != ConnectionState.CONNECTED)
            {
                throw new InvalidOperationException("[TCPConnection]: Unable to start reader task! ConnectionState != Connected! state = " + state);
            }

            Task.Factory.StartNew(() =>
            {
                string data = null;
                int errorCount = 0;
                int countNullOrEmptyStringRead = 0;
                string lastErrorMessage = null;
                DateTime lastNullOrEmptyStringRead = DateTime.MinValue;
                while (true)
                {
                    try
                    {
                        data = readNextString();
                        if (string.IsNullOrEmpty(data))
                        {
                            if (countNullOrEmptyStringRead++ <= 0)
                            {
                                lastNullOrEmptyStringRead = DateTime.Now;
                            }

                            // Read 5 empty or null strings in an interval lower than 1 second:
                            if (countNullOrEmptyStringRead > 5 && (DateTime.Now.Subtract(lastNullOrEmptyStringRead).TotalSeconds < 1))
                            {
                                lastErrorMessage = "Loop detected!";
                                errorCount = 3;
                            }
                        }
                        else
                        {
                            // Trigger the NewDataReceived event:
                            NewDataReceived?.Invoke(this, new NewDataReceivedEventArgs(data));
                            errorCount = 0;
                            countNullOrEmptyStringRead = 0;
                            if (Consts.ENABLE_DEBUG_OUTPUT)
                            {
                                Logger.Info("Received from (" + account.serverAddress + "):" + data);
                            }
                        }
                    }
                    catch (OperationCanceledException e)
                    {
                        lastErrorMessage = e.Message;
                        errorCount++;
                    }
                    catch (Exception e)
                    {
                        SocketErrorStatus status = SocketError.GetStatus(e.GetBaseException().HResult);
                        switch (status)
                        {
                            // Some kind of connection lost:
                            case SocketErrorStatus.ConnectionTimedOut:
                            case SocketErrorStatus.ConnectionRefused:
                            case SocketErrorStatus.NetworkDroppedConnectionOnReset:
                            case SocketErrorStatus.SoftwareCausedConnectionAbort:
                            case SocketErrorStatus.ConnectionResetByPeer:
                                lastErrorMessage = status.ToString();
                                errorCount = 3;
                                break;
                            // Some unknown error occurred:
                            case SocketErrorStatus.Unknown:
                                lastErrorMessage = e.Message;
                                errorCount++;
                                break;
                            default:
                                lastErrorMessage = status.ToString();
                                errorCount++;
                                break;
                        }
                    }

                    if (errorCount >= 3)
                    {
                        setState(ConnectionState.ERROR, lastErrorMessage);
                        return;
                    }
                }
            });
        }

        #endregion

        #region --Misc Methods (Private)--


        #endregion

        #region --Misc Methods (Protected)--
        protected async override Task cleanupAsync()
        {
            if (socket != null)
            {
                try
                {
                    if (writer != null)
                    {
                        // Flush the writer to ensure everything got send:
                        await writer.FlushAsync();
                    }
                }
                catch (Exception)
                {
                }
                try
                {
                    await socket.CancelIOAsync();
                }
                catch (Exception)
                {
                }
                socket?.Dispose();
                writer?.Dispose();
                reader?.Dispose();
                socket = null;
                writer = null;
                reader = null;
                GC.Collect();
            }
        }

        protected string readNextString()
        {
            string result = "";
            while (true)
            {
                char[] buffer = new char[BUFFER_SIZE + 1];
                readingCTS = new CancellationTokenSource();

                // Read next BUFFER_SIZE chars:
                Task t = reader.ReadAsync(buffer, 0, BUFFER_SIZE);

                try
                {
                    t.Wait(readingCTS.Token);
                }
                catch (AggregateException)
                {
                    break;
                }

                // Cut the read string and remove everything starting from the first "\0":
                string data = new string(buffer).Substring(0, buffer.Length - 1);
                int index = data.IndexOf("\0");
                if (index < 0)
                {
                    index = data.Length;
                }
                data = data.Substring(0, index);

                // Is data left to read?
                if (reader.Peek() < 0)
                {
                    // No? then break:
                    result += data;
                    break;
                }
                result += data;
            }
            return result;
        }

        #endregion
        //--------------------------------------------------------Events:---------------------------------------------------------------------\\
        #region --Events--


        #endregion

    }
}
