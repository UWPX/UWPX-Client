using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Windows.Networking;
using Windows.Networking.Sockets;
using Windows.Security.Cryptography.Certificates;

namespace XMPP_API.Classes.Network.TCP
{
    class TCPConnectionHandler : AbstractConnectionHandler
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        private StreamSocket tcpSocket;
        private StreamWriter writer;
        private StreamReader reader;

        private readonly int BUFFER_SIZE = 4096;
        private Task listenerTask;
        private CancellationTokenSource cTS;
        private HostName serverHost;

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Construktoren--
        /// <summary>
        /// Basic Constructor
        /// </summary>
        /// <history>
        /// 17/08/2017 Created [Fabian Sauter]
        /// </history>
        public TCPConnectionHandler(ServerConnectionConfiguration sCC) : base(sCC)
        {
        }

        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--
        /// <summary>
        /// Gets detailed certificate information.
        /// Source: https://github.com/Microsoft/Windows-universal-samples/blob/master/Samples/StreamSocket/cs/Scenario5_Certificates.xaml.cs
        /// </summary>
        /// <param name="serverCert">The server certificate</param>
        /// <param name="intermediateCertificates">The server certificate chain</param>
        /// <returns>A string containing certificate details</returns>
        private string getCertificateInformation(Certificate serverCert, IReadOnlyList<Certificate> intermediateCertificates)
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

        #endregion
        //--------------------------------------------------------Misc Methods:---------------------------------------------------------------\\
        #region --Misc Methods (Public)--
        public async Task sendMessageToServerAsync(string msg)
        {
            if (getState() != ConnectionState.CONNECTED)
            {
                await connectToServerAsync();
            }
            if (getState() != ConnectionState.CONNECTED)
            {
                throw new Exception("Unable to connect to the given server - sendMessageToServerAsync!");
            }
            else
            {
                await writer.WriteLineAsync(msg);
                await writer.FlushAsync();
                Debug.WriteLine("Send:" + msg);
            }
        }

        public async Task<string> readMessageFromServerAsync()
        {
            if (getState() != ConnectionState.CONNECTED)
            {
                await connectToServerAsync();
            }
            if (getState() != ConnectionState.CONNECTED)
            {
                return null;
            }
            char[] buffer = new char[BUFFER_SIZE];
            cTS = new CancellationTokenSource();
            reader.ReadAsync(buffer, 0, BUFFER_SIZE).Wait(cTS.Token);
            string result = new string(buffer);

            return result.Substring(0, result.IndexOf("\0"));
        }

        public async Task upgradeToTLS()
        {
            await tcpSocket.UpgradeToSslAsync(SocketProtectionLevel.Tls12, serverHost);
        }

        public override async Task connectToServerAsync()
        {
            if (getState() == ConnectionState.CONNECTED)
            {
                return;
            }
            setState(ConnectionState.CONNECTING);
            for (int i = 1; i < 4; i++)
            {
                try
                {
                    // Setup socket:
                    tcpSocket = new StreamSocket();
                    serverHost = new HostName(SCC.serverAddress);

                    await tcpSocket.ConnectAsync(serverHost, SCC.port.ToString());

                    // Setup writer:
                    writer = new StreamWriter(tcpSocket.OutputStream.AsStreamForWrite());

                    // Setup Reader:
                    reader = new StreamReader(tcpSocket.InputStream.AsStreamForRead());

                    setState(ConnectionState.CONNECTED);
                    return;
                }
                catch (Exception e)
                {
                    Debug.WriteLine("Try " + i + ": " + e.Message + "\n" + e.StackTrace);
                    Debug.WriteLine(e.StackTrace);
                    cleanupConnection();
                }
            }
            setState(ConnectionState.ERROR);
        }

        public override async Task disconnectFromServerAsync()
        {
            setState(ConnectionState.DISCONNECTING);
            try
            {
                await tcpSocket.CancelIOAsync();
            }
            catch (Exception)
            {
            }
            if (listenerTask != null)
            {
                cTS.Cancel();
                listenerTask.Wait();
            }
        }

        public void connectAndHoldConnection()
        {
            listenerTask = Task.Factory.StartNew(async () =>
            {
                await connectToServerAsync();
                while (getState() == ConnectionState.CONNECTED)
                {
                    try
                    {
                        string data = await readMessageFromServerAsync();
                        if (data != null && data != "")
                        {
                            onConnectionNewData(data);
                        }
                    }
                    catch (OperationCanceledException e)
                    {

                    }
                    catch (Exception e)
                    {
                        Debug.WriteLine(e.Message);
                    }
                }
                cleanupConnection();
                setState(ConnectionState.DISCONNECTED);
            });
        }

        #endregion

        #region --Misc Methods (Private)--


        #endregion

        #region --Misc Methods (Protected)--
        protected override void cleanupConnection()
        {
            tcpSocket?.Dispose();
            tcpSocket = null;
            writer?.Dispose();
            writer = null;
            reader?.Dispose();
            reader = null;
        }

        #endregion
        //--------------------------------------------------------Events:---------------------------------------------------------------------\\
        #region --Events--


        #endregion
    }
}
