using Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Windows.ApplicationModel.Background;
using Windows.Networking;
using Windows.Networking.Sockets;
using Windows.Security.Cryptography.Certificates;
using Windows.Storage.Streams;

namespace XMPP_API.Classes.Network.TCP
{
    class TCPConnectionHandler : AbstractConnectionHandler
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        private StreamSocket tcpSocket;
        private HostName serverHost;
        private StreamWriter writer;
        private StreamReader reader;
        private BackgroundTaskRegistration socketBackgroundTask;

        private Task listenerTask;
        private CancellationTokenSource cTS;

        private readonly int BUFFER_SIZE = 4096;

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        /// <summary>
        /// Basic Constructor
        /// </summary>
        /// <history>
        /// 17/08/2017 Created [Fabian Sauter]
        /// </history>
        public TCPConnectionHandler(XMPPAccount sCC) : base(sCC)
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

        public async override Task connectToServerAsync()
        {
            switch (getState())
            {
                case ConnectionState.ERROR:
                case ConnectionState.DISCONNECTED:
                    setState(ConnectionState.CONNECTING);
                    for (int i = 1; i < 4; i++)
                    {
                        try
                        {
                            // Setup socket:
                            tcpSocket = new StreamSocket();
                            serverHost = new HostName(ACCOUNT.serverAddress);

                            // Enable transfer ownership:
                            socketBackgroundTask = MyBackgroundTaskHelper.getSocketTask();
                            if (socketBackgroundTask != null)
                            {
                                // https://docs.microsoft.com/de-de/windows/uwp/networking/network-communications-in-the-background#socket-broker-and-the-socketactivitytrigger
                                tcpSocket.EnableTransferOwnership(socketBackgroundTask.TaskId, SocketActivityConnectedStandbyAction.Wake);
                            }

                            // Connect:
                            await tcpSocket.ConnectAsync(serverHost, ACCOUNT.port.ToString());

                            // Setup writer:
                            writer = new StreamWriter(tcpSocket.OutputStream.AsStreamForWrite());

                            // Setup Reader:
                            reader = new StreamReader(tcpSocket.InputStream.AsStreamForRead());
                            setState(ConnectionState.CONNECTED);
                            return;
                        }
                        catch (Exception e)
                        {
                            Logger.Error(i + " try to connect to: " + ACCOUNT.serverAddress, e);
                            cleanupConnection();
                        }
                    }
                    setState(ConnectionState.ERROR);
                    break;
                default:
                    Logger.Warn("Unable to connect to server. Socket state = " + getState());
                    break;
            }
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
        public async Task transferOwnershipAsync()
        {
            if (socketBackgroundTask != null)
            {
                try
                {
                    try
                    {
                        await tcpSocket.CancelIOAsync();
                    }
                    catch (Exception)
                    {
                    }
                    DataWriter dataWriter = new DataWriter();
                    dataWriter.WriteString(ACCOUNT.getIdAndDomain());
                    tcpSocket.TransferOwnership(ACCOUNT.getIdAndDomain(), new SocketActivityContext(dataWriter.DetachBuffer()));
                    Logger.Info("Transfered socket ownership for socket:" + ACCOUNT.getIdAndDomain());
                }
                catch (Exception)
                {
                    Logger.Error("An error occurred during transferOwnershipAsync with socket: " + ACCOUNT.getIdAndDomain());
                }
            }
        }

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
