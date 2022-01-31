using System;
using System.Linq;
using System.Threading.Tasks;
using Logging;
using Windows.ApplicationModel.Background;
using Windows.Foundation.Metadata;

namespace UWPX_UI_Context.Classes
{
    public static class BackgroundTaskHelper
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        public const string TOAST_BACKGROUND_TASK_NAME = "ToastBackgroundTask";
        public const string PUSH_BACKGROUND_TASK_NAME = "PushBackgroundTask";
        public const string PUSH_CHANNEL_BACKGROUND_TASK_NAME = "PushChannelBackgroundTask";

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--


        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--


        #endregion
        //--------------------------------------------------------Misc Methods:---------------------------------------------------------------\\
        #region --Misc Methods (Public)--
        public static async Task RegisterBackgroundTasksAsync()
        {
            await RegisterToastBackgroundTaskAsync();
            await RegisterPushBackgroundTaskAsync();
            await RegisterPushChannelBackgroundTaskAsync();
        }

        #endregion

        #region --Misc Methods (Private)--
        private static bool CheckBackgroundAccessStatus(BackgroundAccessStatus status)
        {
            return status == BackgroundAccessStatus.AlwaysAllowed || status == BackgroundAccessStatus.AllowedSubjectToSystemPolicy;
        }

        private async static Task RegisterToastBackgroundTaskAsync()
        {
            if (!ApiInformation.IsTypePresent("Windows.ApplicationModel.Background.ToastNotificationActionTrigger"))
            {
                Logger.Warn("Failed to register toast notification background task. API not present.");
                return;
            }

            // If background task is already registered, do nothing:
            if (BackgroundTaskRegistration.AllTasks.Any(i => i.Value.Name.Equals(TOAST_BACKGROUND_TASK_NAME)))
            {
                Logger.Info(TOAST_BACKGROUND_TASK_NAME + " background task already registered.");
                return;
            }

            // Otherwise request access:
            BackgroundAccessStatus status = await BackgroundExecutionManager.RequestAccessAsync();
            if(!CheckBackgroundAccessStatus(status))
            {
                Logger.Info(TOAST_BACKGROUND_TASK_NAME + $" failed to register, since it's not allowed ({status}).");
                return;
            }

            // Create the background task:
            BackgroundTaskBuilder builder = new BackgroundTaskBuilder
            {
                Name = TOAST_BACKGROUND_TASK_NAME
            };

            // Assign the toast action trigger:
            builder.SetTrigger(new ToastNotificationActionTrigger());

            // And register the task:
            builder.Register();

            Logger.Info("Registered " + TOAST_BACKGROUND_TASK_NAME + " background task.");
        }

        private async static Task RegisterPushBackgroundTaskAsync()
        {
            // If background task is already registered, do nothing:
            if (BackgroundTaskRegistration.AllTasks.Any(i => i.Value.Name.Equals(PUSH_BACKGROUND_TASK_NAME)))
            {
                Logger.Info(PUSH_BACKGROUND_TASK_NAME + " background task already registered.");
                return;
            }

            // Otherwise request access:
            BackgroundAccessStatus status = await BackgroundExecutionManager.RequestAccessAsync();
            if (!CheckBackgroundAccessStatus(status))
            {
                Logger.Info(PUSH_BACKGROUND_TASK_NAME + $" failed to register, since it's not allowed ({status}).");
                return;
            }

            // Create the background task:
            BackgroundTaskBuilder builder = new BackgroundTaskBuilder
            {
                Name = PUSH_BACKGROUND_TASK_NAME,
                IsNetworkRequested = true
            };

            // Assign the push notification trigger:
            builder.SetTrigger(new PushNotificationTrigger());
            builder.TaskEntryPoint = "Push_BackgroundTask.Classes.PushBackgroundTask";

            // And register the task:
            builder.Register().Completed += OnBackgroundTaskRegistrationCompleted;

            Logger.Info("Registered " + PUSH_BACKGROUND_TASK_NAME + " background task.");
        }

        private async static Task RegisterPushChannelBackgroundTaskAsync()
        {
            // If background task is already registered, do nothing:
            if (BackgroundTaskRegistration.AllTasks.Any(i => i.Value.Name.Equals(PUSH_CHANNEL_BACKGROUND_TASK_NAME)))
            {
                Logger.Info(PUSH_CHANNEL_BACKGROUND_TASK_NAME + " background task already registered.");
                return;
            }

            // Otherwise request access:
            BackgroundAccessStatus status = await BackgroundExecutionManager.RequestAccessAsync();
            if (!CheckBackgroundAccessStatus(status))
            {
                Logger.Info(PUSH_CHANNEL_BACKGROUND_TASK_NAME + $" failed to register, since it's not allowed ({status}).");
                return;
            }

            // Create the background task:
            BackgroundTaskBuilder builder = new BackgroundTaskBuilder
            {
                Name = PUSH_CHANNEL_BACKGROUND_TASK_NAME,
                IsNetworkRequested = true
            };

            // Assign the timer trigger to every day once:
            builder.SetTrigger(new TimeTrigger(60*24, false));
            builder.AddCondition(new SystemCondition(SystemConditionType.InternetAvailable));
            builder.TaskEntryPoint = "PushChannel_BackgroundTask.Classes.PushChannelBackgroundTask";

            // And register the task:
            builder.Register().Completed += OnBackgroundTaskRegistrationCompleted;

            Logger.Info("Registered " + PUSH_CHANNEL_BACKGROUND_TASK_NAME + " background task.");
        }

        #endregion

        #region --Misc Methods (Protected)--


        #endregion
        //--------------------------------------------------------Events:---------------------------------------------------------------------\\
        #region --Events--
        private static void OnBackgroundTaskRegistrationCompleted(BackgroundTaskRegistration sender, BackgroundTaskCompletedEventArgs args)
        {
            sender.Completed -= OnBackgroundTaskRegistrationCompleted;
            Logger.Info("Registered " + sender.Name + " background task.");
        }

        #endregion
    }
}
