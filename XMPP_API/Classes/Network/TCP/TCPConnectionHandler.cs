using Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
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
        private SocketErrorStatus socketErrorStatus;

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

        public SocketErrorStatus getSocketErrorStatus()
        {
            return socketErrorStatus;
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
                if (Consts.ENABLE_DEBUG_OUTPUT)
                {
                    Logger.Info("Send to (" + ACCOUNT.serverAddress + "):" + msg);
                }
            }
        }

        public async Task<string> readMessageFromServerAsync()
        {
            if (getState() != ConnectionState.CONNECTED)
            {
                return null;
            }
            string result = "";
            while (true)
            {
                char[] buffer = new char[BUFFER_SIZE + 1];
                Task<int> t = reader.ReadAsync(buffer, 0, BUFFER_SIZE);
                cTS = new CancellationTokenSource();
                t.Wait(cTS.Token);
                string data = new string(buffer).Substring(0, buffer.Length - 1);
                int index = data.IndexOf("\0");
                if(index < 0)
                {
                    index = data.Length;
                }
                data = data.Substring(0, index);
                if (t.Result < BUFFER_SIZE)
                {
                    return result + data;
                }
                result += data;
            }
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
                            socketErrorStatus = SocketError.GetStatus(e.GetBaseException().HResult);
                            if(socketErrorStatus == SocketErrorStatus.Unknown)
                            {
                                Logger.Error(i + " try to connect to " + ACCOUNT.serverAddress, e);
                            }
                            else
                            {
                                Logger.Error(i + " try to connect to " + ACCOUNT.serverAddress + "; Exception: " + socketErrorStatus.ToString());
                            }
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
                int errorCount = 0;
                while (getState() == ConnectionState.CONNECTED)
                {
                    try
                    {
                        string data = await readMessageFromServerAsync();
                        if (data != null && data != "")
                        {
                            onConnectionNewData(data);
                        }
                        errorCount = 0;
                    }
                    catch (OperationCanceledException e)
                    {
                        errorCount++;
                    }
                    catch (COMException e)
                    {
                        Logger.Error("Server closed connection - TCPConnectionHandler", e);
                    }
                    catch (Exception e)
                    {
                        SocketErrorStatus state = socketErrorStatus = SocketError.GetStatus(e.GetBaseException().HResult);
                        if(state == SocketErrorStatus.Unknown)
                        {
                            Logger.Error("Error during reading a message from the server - TCPConnectionHandler", e);
                        }
                        else
                        {
                            Logger.Error("Error during reading a message from the server - TCPConnectionHandler " + state.ToString());
                            break;
                        }
                        errorCount++;
                    }
                    if(errorCount > 5)
                    {
                        break;
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
            if(tcpSocket != null)
            {
                try
                {
                    tcpSocket.CancelIOAsync().AsTask().Wait();
                }
                catch (Exception)
                {
                }
                tcpSocket?.Dispose();
            }
            if(writer != null)
            {
                writer.Flush();
                writer.Dispose();
            }
            if(reader != null)
            {
                reader.Dispose();
            }
            GC.Collect();
        }

        #endregion
        //--------------------------------------------------------Events:---------------------------------------------------------------------\\
        #region --Events--


        #endregion
    }
}
