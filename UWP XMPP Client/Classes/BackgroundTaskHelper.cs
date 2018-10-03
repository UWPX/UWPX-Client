using Logging;
using System;
using System.Linq;
using System.Threading.Tasks;
using Windows.ApplicationModel.Background;

namespace UWP_XMPP_Client.Classes
{
    static class BackgroundTaskHelper
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        public const string TOAST_BACKGROUND_TASK_NAME = "ToastBackgroundTask";

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--


        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--


        #endregion
        //--------------------------------------------------------Misc Methods:---------------------------------------------------------------\\
        #region --Misc Methods (Public)--
        public async static Task registerToastBackgroundTaskAsync()
        {
            // If background task is already registered, do nothing
            if (BackgroundTaskRegistration.AllTasks.Any(i => i.Value.Name.Equals(TOAST_BACKGROUND_TASK_NAME)))
            {
                Logger.Info(TOAST_BACKGROUND_TASK_NAME + " background task already registered.");
                return;
            }

            // Otherwise request access
            BackgroundAccessStatus status = await BackgroundExecutionManager.RequestAccessAsync();

            // Create the background task
            BackgroundTaskBuilder builder = new BackgroundTaskBuilder
            {
                Name = TOAST_BACKGROUND_TASK_NAME
            };

            // Assign the toast action trigger
            builder.SetTrigger(new ToastNotificationActionTrigger());

            // And register the task
            builder.Register();

            Logger.Info("Registered " + TOAST_BACKGROUND_TASK_NAME + " background task.");
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
