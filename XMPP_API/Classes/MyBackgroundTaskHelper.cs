using Logging;
using Microsoft.Toolkit.Uwp.Helpers;
using System;
using System.Threading.Tasks;
using Windows.ApplicationModel.Background;

namespace XMPP_API.Classes
{
    public static class MyBackgroundTaskHelper
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        private const string BACKGROUND_TASK_NAME = "UwpXmppSocketBackgroundTask";
        private const string BACKGROUND_TASK_ENTRY_POINT = "BackgroundSocket.Classes.BackgroundSocketHandler";
        private static BackgroundTaskRegistration socketTask;

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--


        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--
        public static BackgroundTaskRegistration getSocketTask()
        {
            return socketTask;
        }

        #endregion
        //--------------------------------------------------------Misc Methods:---------------------------------------------------------------\\
        #region --Misc Methods (Public)--


        #endregion

        #region --Misc Methods (Private)--
        /// <summary>
        /// Registers the socket background task if necessary.
        /// </summary>
        public async static Task registerBackgroundTask()
        {
            if (BackgroundTaskHelper.IsBackgroundTaskRegistered(BACKGROUND_TASK_NAME))
            {
                removeBackgroundTask();
            }
            await BackgroundExecutionManager.RequestAccessAsync();
            socketTask = BackgroundTaskHelper.Register(BACKGROUND_TASK_NAME, BACKGROUND_TASK_ENTRY_POINT, new SocketActivityTrigger(), false, true);

            Logger.Info("Registered the " + BACKGROUND_TASK_NAME + " background task.");
        }

        /// <summary>
        /// Removes the socket background task if necessary.
        /// </summary>
        public static void removeBackgroundTask()
        {
            BackgroundTaskHelper.Unregister(BACKGROUND_TASK_NAME);
            Logger.Info("Unregistered the " + BACKGROUND_TASK_NAME + " background task.");
        }

        #endregion

        #region --Misc Methods (Protected)--


        #endregion
        //--------------------------------------------------------Events:---------------------------------------------------------------------\\
        #region --Events--


        #endregion
    }
}
