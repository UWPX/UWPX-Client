using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Data_Manager2.Classes;
using Logging;
using Microsoft.AppCenter.Analytics;
using Microsoft.AppCenter.Crashes;
using Microsoft.AppCenter.Push;

namespace UWPX_UI_Context.Classes
{
    public static class AppCenterHelper
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        private const string APP_CENTER_SECRET = "523e7039-f6cb-4bf1-9000-53277ed97c53";

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--


        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--
        public static async Task SetCrashesEnabledAsync(bool enabled)
        {
            Crashes.Instance.InstanceEnabled = enabled;

            if (enabled)
            {
                Logger.Info("AppCenter crash reporting enabled.");
            }
            else
            {
                Logger.Info("AppCenter crash reporting disabled.");
            }
            if (await Analytics.IsEnabledAsync())
            {
                Analytics.TrackEvent("crash_reporting", new Dictionary<string, string> { { "disabled", (!enabled).ToString() } });
            }
        }

        public static async Task SetAnalyticsEnabledAsync(bool enabled)
        {
            if (enabled)
            {
                await Analytics.SetEnabledAsync(true);
                Analytics.TrackEvent("analytics", new Dictionary<string, string> { { "disabled", (!enabled).ToString() } });
                Logger.Info("AppCenter analytics enabled.");
            }
            else
            {
                Analytics.TrackEvent("analytics", new Dictionary<string, string> { { "disabled", (!enabled).ToString() } });
                await Analytics.SetEnabledAsync(false);
                Logger.Info("AppCenter analytics disabled.");
            }
        }

        #endregion
        //--------------------------------------------------------Misc Methods:---------------------------------------------------------------\\
        #region --Misc Methods (Public)--
        /// <summary>
        /// Sets up App Center crash, analytics and push support.
        /// </summary>
        public static void SetupAppCenter(EventHandler<PushNotificationReceivedEventArgs> appCenterPushCallback)
        {
            try
            {
#if DEBUG
                Microsoft.AppCenter.AppCenter.Start(APP_CENTER_SECRET, typeof(Crashes));
                if (Settings.getSettingBoolean(SettingsConsts.DISABLE_CRASH_REPORTING))
                {
                    Crashes.Instance.InstanceEnabled = false;
                    Logger.Info("AppCenter crash reporting is disabled.");
                }

                Microsoft.AppCenter.AppCenter.Start(APP_CENTER_SECRET, typeof(Analytics));
                if (Settings.getSettingBoolean(SettingsConsts.DISABLE_ANALYTICS))
                {
                    Analytics.SetEnabledAsync(false);
                    Logger.Info("AppCenter analytics are disabled.");
                }
                // Only enable push for debug builds:
                Microsoft.AppCenter.AppCenter.Start(APP_CENTER_SECRET, typeof(Push));
#endif

                if (!Microsoft.AppCenter.AppCenter.Configured)
                {
                    Push.PushNotificationReceived -= appCenterPushCallback;
                    Push.PushNotificationReceived += appCenterPushCallback;
                }
            }
            catch (Exception e)
            {
                Logger.Error("Failed to start APPCenter!", e);
                throw e;
            }
            Logger.Info("App Center crash reporting registered.");
            Logger.Info("App Center push registered.");
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
