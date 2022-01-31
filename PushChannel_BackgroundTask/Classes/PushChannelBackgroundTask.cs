using System;
using System.Threading.Tasks;
using Logging;
using Push.Classes;
using Storage.Classes;
using Windows.ApplicationModel.AppService;
using Windows.ApplicationModel.Background;
using Windows.Foundation.Collections;

namespace PushChannel_BackgroundTask.Classes
{
    public sealed class PushChannelBackgroundTask: IBackgroundTask
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        private BackgroundTaskDeferral deferral;
        private AppServiceConnection appServiceConnection;

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--


        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--
        private async Task<bool> IsAppRunningAsync()
        {
            AppServiceConnectionStatus result = await appServiceConnection.OpenAsync();
            if (result == AppServiceConnectionStatus.Success)
            {
                ValueSet request = new ValueSet
                {
                    { "request", "is_running" }
                };
                AppServiceResponse response = await appServiceConnection.SendMessageAsync(request);
                if (response.Status == AppServiceResponseStatus.Success)
                {
                    return string.Equals(response.Message["response"] as string, "true");
                }
            }
            return false;
        }

        private async Task<bool> InitPushInForegroundAsync()
        {
            Logger.Info("Initializing push in foreground...");
            AppServiceConnectionStatus result = await appServiceConnection.OpenAsync();
            if (result == AppServiceConnectionStatus.Success)
            {
                ValueSet request = new ValueSet
                {
                    { "request", "init_push" }
                };
                AppServiceResponse response = await appServiceConnection.SendMessageAsync(request);
                if (response.Status == AppServiceResponseStatus.Success)
                {
                    return string.Equals(response.Message["response"] as string, "true");
                }
            }
            return false;
        }

        #endregion
        //--------------------------------------------------------Misc Methods:---------------------------------------------------------------\\
        #region --Misc Methods (Public)--
        public async void Run(IBackgroundTaskInstance taskInstance)
        {
            deferral = taskInstance.GetDeferral();
            Logger.Info("Updating push from background task...");
            if (Settings.GetSettingBoolean(SettingsConsts.PUSH_ENABLED))
            {
                Logger.Info("Updating push from background task.");
                // Init the app service connection:
                appServiceConnection = new AppServiceConnection
                {
                    AppServiceName = "uwpx.status",
                    PackageFamilyName = Windows.ApplicationModel.Package.Current.Id.FamilyName
                };
                if (!await IsAppRunningAsync() || !await InitPushInForegroundAsync())
                {
                    PushManager.INSTANCE.Init();
                    Logger.Info("Updating push from background task done.");
                }
            }
            else
            {
                Logger.Info("Skipping push init from background task - disabled.");
            }
            deferral.Complete();
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
