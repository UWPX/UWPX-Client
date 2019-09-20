using Logging;
using System;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Background;
using Windows.Networking.Sockets;
using Windows.Storage.Streams;
using Windows.UI.Notifications;

namespace BackgroundSocket.Classes
{
    public sealed class BackgroundSocketHandler : IBackgroundTask
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        private readonly uint BUFFER_SIZE = 4096;
        private BackgroundTaskDeferral deferral;
        private int count;

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        /// <summary>
        /// Basic Constructor
        /// </summary>
        /// <history>
        /// 20/09/2017 Created [Fabian Sauter]
        /// </history>
        public BackgroundSocketHandler()
        {
            count = 0;
            deferral = null;
        }

        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--


        #endregion
        //--------------------------------------------------------Misc Methods:---------------------------------------------------------------\\
        #region --Misc Methods (Public)--
        public async void Run(IBackgroundTaskInstance taskInstance)
        {
            Logger.Debug("BackgroundSocketHandler Run():" + (++count));
            deferral = taskInstance.GetDeferral();
            if(taskInstance.TriggerDetails is SocketActivityTriggerDetails)
            {
                SocketActivityTriggerDetails socketActivityDetails = taskInstance.TriggerDetails as SocketActivityTriggerDetails;
                SocketActivityInformation socketInfo = socketActivityDetails.SocketInformation;
                StreamSocket socket;
                try
                {
                    showToast(socketInfo.Id + " " + socketInfo.ToString());
                }
                catch (Exception e)
                {
                    showToast(e.Message);
                }

                switch (socketActivityDetails.Reason)
                {
                    case SocketActivityTriggerReason.None:
                        break;
                    // Somebody send something:
                    case SocketActivityTriggerReason.SocketActivity:
                        socket = socketInfo.StreamSocket;
                        await readFromSocket(socket);
                        socket.TransferOwnership(socketActivityDetails.SocketInformation.Id);
                        break;
                    case SocketActivityTriggerReason.ConnectionAccepted:
                        break;
                    case SocketActivityTriggerReason.KeepAliveTimerExpired:
                        socket = socketInfo.StreamSocket;
                        await sendKeepAliveAsync(socket);
                        socket.TransferOwnership(socketActivityDetails.SocketInformation.Id);
                        break;
                    case SocketActivityTriggerReason.SocketClosed:
                        break;
                    default:
                        break;
                }
            }
            deferral.Complete();
        }

        private async Task sendKeepAliveAsync(StreamSocket socket)
        {
            DataWriter writer = new DataWriter(socket.OutputStream);
            writer.WriteBytes(Encoding.UTF8.GetBytes(" "));
            await writer.StoreAsync();
            writer.DetachStream();
            writer.Dispose();
        }

        private async Task readFromSocket(StreamSocket socket)
        {
            DataReader reader = new DataReader(socket.InputStream)
            {
                InputStreamOptions = InputStreamOptions.Partial
            };

            // Read data:
            await reader.LoadAsync(BUFFER_SIZE);
            string data = reader.ReadString(reader.UnconsumedBufferLength);
            reader.DetachStream();

            // Process data:
            // ...
            showToast(data);
        }

        #endregion

        #region --Misc Methods (Private)--
        private void showToast(string text)
        {
            Windows.Data.Xml.Dom.XmlDocument tost = ToastNotificationManager.GetTemplateContent(ToastTemplateType.ToastImageAndText01);
            Windows.Data.Xml.Dom.XmlNodeList childUpdate = tost?.GetElementsByTagName("text");
            if (childUpdate != null) childUpdate[0].InnerText = text;
            ToastNotification titleNotification = new ToastNotification(tost) { Group = "NetUpdate" };
            ToastNotificationManager.CreateToastNotifier().Show(titleNotification);
        }

        #endregion

        #region --Misc Methods (Protected)--


        #endregion
        //--------------------------------------------------------Events:---------------------------------------------------------------------\\
        #region --Events--


        #endregion
    }
}
