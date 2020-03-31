using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Logging;
using Windows.Networking;
using Windows.Networking.Sockets;
using Windows.Storage.Streams;
using XMPP_API.Classes.Network.TCP;

namespace Push.Classes
{
    public class TcpConnection: IDisposable
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        private const string LOGGER_TAG = "[Push]: ";

        /// <summary>
        /// How many characters should get read at once max.
        /// </summary>
        private const int BUFFER_SIZE = 4096;
        /// <summary>
        /// The timeout in ms for TCP connections.
        /// </summary>
        private const int CONNECTION_TIMEOUT_MS = 3000;
        /// <summary>
        /// The timeout for sending data.
        /// </summary>
        private const int SEND_TIMEOUT_MS = 1000;
        /// <summary>
        /// The timeout for sending data.
        /// </summary>
        private const int READ_TIMEOUT_MS = 3000;

        /// <summary>
        /// The maximum amount of attempts to connect until we fail.
        /// </summary>
        private const int MAX_CONNECTION_ATTEMPTS = 3;
        /// <summary>
        /// The time in ms between connection attempts.
        /// </summary>
        private const int CONNECTION_ATTEMPT_DELAY = 1000;

        private StreamSocket socket;
        private readonly HostName HOSTNAME;
        private readonly ushort PORT;

        private DataReader dataReader;
        private DataWriter dataWriter;

        /// <summary>
        /// Enforces the connection timeout <see cref="disableConnectionTimeout"/>.
        /// </summary>
        private CancellationTokenSource connectTimeoutCTS;
        private CancellationTokenSource readCTS;
        private CancellationTokenSource sendCTS;

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        public TcpConnection(string hostname, ushort port)
        {
            HOSTNAME = new HostName(hostname);
            PORT = port;
        }

        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--


        #endregion
        //--------------------------------------------------------Misc Methods:---------------------------------------------------------------\\
        #region --Misc Methods (Public)--
        public async Task ConnectAsync()
        {
            Exception lastException = null;
            for (int i = 1; i <= MAX_CONNECTION_ATTEMPTS; i++)
            {
                try
                {
                    // Setup socket:
                    socket = new StreamSocket();
                    socket.Control.KeepAlive = true;
                    socket.Control.QualityOfService = SocketQualityOfService.LowLatency;

                    // Connect with timeout:
                    connectTimeoutCTS = new CancellationTokenSource(CONNECTION_TIMEOUT_MS);

                    // Start the connection process:
                    await socket.ConnectAsync(HOSTNAME, PORT.ToString(), SocketProtectionLevel.PlainSocket).AsTask(connectTimeoutCTS.Token);

                    // Setup stream reader and writer:
                    dataWriter = new DataWriter(socket.OutputStream);
                    dataReader = new DataReader(socket.InputStream) { InputStreamOptions = InputStreamOptions.Partial };
                    return;
                }
                catch (TaskCanceledException e)
                {
                    Logger.Error(LOGGER_TAG + i + " try to connect to " + HOSTNAME.ToString() + " failed:", e);
                    lastException = e;
                }
                catch (Exception e)
                {
                    lastException = e;
                }

                if (i < MAX_CONNECTION_ATTEMPTS)
                {
                    // Wait between connection attempts:
                    Logger.Info(LOGGER_TAG + "Starting delay between connection attempts...");
                    await Task.Delay(CONNECTION_ATTEMPT_DELAY);
                }
            }
            throw lastException;
        }

        public void Disconnect()
        {
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

        public async Task SendAsync(string msg)
        {
            // Sometimes dataWriter is blocking for an infinite time, so give it a timeout:
            sendCTS = new CancellationTokenSource(SEND_TIMEOUT_MS);
            sendCTS.CancelAfter(SEND_TIMEOUT_MS);

            Exception ex = await Task.Run(async () =>
            {
                try
                {
                    // Append a NULL-Byte to inform the server, this is everything:
                    msg += '\0';

                    uint l = dataWriter.WriteString(msg);

                    // Check if all bytes got actually written to the TCP buffer:
                    if (l < msg.Length)
                    {
                        return new Exception("Send only " + l + " of " + msg.Length + " bytes.");
                    }

                    await dataWriter.StoreAsync();
                    await dataWriter.FlushAsync();
                }
                catch (Exception e) { return e; }
                return null;
            }, sendCTS.Token).ConfigureAwait(false);

            if (!(ex is null))
            {
                throw ex;
            }
        }

        public async Task<TcpReadResult> ReadAsync()
        {
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

            // Sometimes dataReader is blocking for an infinite time, so give it a timeout:
            readCTS = new CancellationTokenSource(READ_TIMEOUT_MS);

            // If there is still data left to read, continue until a timeout occurs or a close got requested:
            while (!(readCTS is null) && !readCTS.IsCancellationRequested && readCount >= BUFFER_SIZE)
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

        public void Dispose()
        {
            Disconnect();
        }

        #endregion

        #region --Misc Methods (Private)--


        #endregion

        #region --Misc Methods (Protected)--


        #endregion
        //--------------------------------------------------------Events:---------------------------------------------------------------------\\
        #region --Events--


        #endregion
    }
}
