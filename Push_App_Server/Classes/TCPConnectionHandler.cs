using Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Windows.Networking;
using Windows.Networking.Sockets;
using Windows.Security.Cryptography.Certificates;

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
        /// <summary>
        /// Gets detailed certificate information.
        /// Source: https://github.com/Microsoft/Windows-universal-samples/blob/master/Samples/StreamSocket/cs/Scenario5_Certificates.xaml.cs
        /// </summary>
        /// <param name="serverCert">The server certificate.</param>
        /// <param name="intermediateCertificates">The server certificate chain.</param>
        /// <returns>A string containing certificate details.</returns>
        public string getCertificateInformation(Certificate serverCert, IReadOnlyList<Certificate> intermediateCertificates)
        {
            StringBuilder stringBuilder = new StringBuilder();

            stringBuilder.AppendLine("\tFriendly Name: " + serverCert.FriendlyName);
            stringBuilder.AppendLine("\tSubject: " + serverCert.Subject);
            stringBuilder.AppendLine("\tIssuer: " + serverCert.Issuer);
            stringBuilder.AppendLine("\tValidity: " + serverCert.ValidFrom + " - " + serverCert.ValidTo);

            // Enumerate the entire certificate chain.
            if (intermediateCertificates.Count > 0)
            {
                stringBuilder.AppendLine("\tCertificate chain: ");
                foreach (var cert in intermediateCertificates)
                {
                    stringBuilder.AppendLine("\t\tIntermediate Certificate Subject: " + cert.Subject);
                }
            }
            else
            {
                stringBuilder.AppendLine("\tNo certificates within the intermediate chain.");
            }

            return stringBuilder.ToString();
        }

        public string getCertificateInformation()
        {
            return getCertificateInformation(tcpSocket.Information.ServerCertificate, tcpSocket.Information.ServerIntermediateCertificates);
        }

        #endregion
        //--------------------------------------------------------Misc Methods:---------------------------------------------------------------\\
        #region --Misc Methods (Public)--
        public async Task connectAsync()
        {
            // Setup socket:
            tcpSocket = new StreamSocket();
            serverHost = new HostName(Consts.PUSH_SERVER_ADDRESS);

            // Connect:
            await tcpSocket.ConnectAsync(serverHost, Consts.PORT.ToString(), SocketProtectionLevel.Tls12);

            // Setup writer:
            writer = new StreamWriter(tcpSocket.OutputStream.AsStreamForWrite());

            // Setup Reader:
            reader = new StreamReader(tcpSocket.InputStream.AsStreamForRead());
            return;
        }

        public async Task disconnectAsync()
        {
            if (cTS != null)
            {
                cTS.Cancel();
                await Task.Delay(TimeSpan.FromSeconds(1));
            }
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
        public void cleanupConnection()
        {
            tcpSocket = null;
            writer = null;
            reader = null;
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
