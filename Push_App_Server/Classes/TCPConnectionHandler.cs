using Logging;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Windows.Networking;
using Windows.Networking.Sockets;

namespace Push_App_Server.Classes
{
    public class TCPConnectionHandler
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        private StreamSocket tcpSocket;
        private HostName serverHost;
        private StreamWriter writer;
        private StreamReader reader;

        private CancellationTokenSource cTS;

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        /// <summary>
        /// Basic Constructor
        /// </summary>
        /// <history>
        /// 18/11/2017 Created [Fabian Sauter]
        /// </history>
        public TCPConnectionHandler()
        {

        }
        
        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--


        #endregion
        //--------------------------------------------------------Misc Methods:---------------------------------------------------------------\\
        #region --Misc Methods (Public)--
        public async Task connectAsync()
        {
            for (int i = 1; i < 4; i++)
            {
                try
                {
                    // Setup socket:
                    tcpSocket = new StreamSocket();
                    serverHost = new HostName(Consts.PUSH_SERVER_ADDRESS);

                    // Connect:
                    await tcpSocket.ConnectAsync(serverHost, Consts.PORT.ToString());

                    // Setup writer:
                    writer = new StreamWriter(tcpSocket.OutputStream.AsStreamForWrite());

                    // Setup Reader:
                    reader = new StreamReader(tcpSocket.InputStream.AsStreamForRead());
                    return;
                }
                catch (Exception e)
                {
                    Logger.Error(i + " try to connect to: " + Consts.PUSH_SERVER_ADDRESS, e);
                    cleanupConnection();
                }
            }
        }

        public async Task disconnectAsync()
        {
            if (cTS != null)
            {
                cTS.Cancel();
            }
            await Task.Delay(TimeSpan.FromSeconds(1));
            cleanupConnection();
        }

        public async Task sendMessageToServerAsync(string msg)
        {
            await writer.WriteLineAsync(msg);
            await writer.FlushAsync();
            Logger.Info("Send to (" + Consts.PUSH_SERVER_ADDRESS + "):" + msg);
        }

        public string readMessageFromServer()
        {
            string result = "";
            while (true)
            {
                char[] buffer = new char[Consts.BUFFER_SIZE + 1];
                Task<int> t = reader.ReadAsync(buffer, 0, Consts.BUFFER_SIZE);
                cTS = new CancellationTokenSource();
                t.Wait(cTS.Token);
                string data = new string(buffer).Substring(0, buffer.Length - 1);
                int index = data.IndexOf("\0");
                if (index < 0)
                {
                    index = data.Length;
                }
                data = data.Substring(0, index);
                if (t.Result < Consts.BUFFER_SIZE)
                {
                    return result + data;
                }
                result += data;
            }
        }

        #endregion

        #region --Misc Methods (Private)--
        private void cleanupConnection()
        {
            tcpSocket?.Dispose();
            tcpSocket = null;
            writer?.Dispose();
            writer = null;
            reader?.Dispose();
            reader = null;
            cTS?.Dispose();
            cTS = null;
            GC.Collect();
        }

        #endregion

        #region --Misc Methods (Protected)--


        #endregion
        //--------------------------------------------------------Events:---------------------------------------------------------------------\\
        #region --Events--


        #endregion
    }
}
