using System;
using System.Collections.Generic;
using System.Threading.Tasks;
#if !DEBUG
using Data_Manager2.Classes;
#endif
using Logging;
using Microsoft.AppCenter;
using Microsoft.AppCenter.Analytics;
using Microsoft.AppCenter.Crashes;

namespace UWPX_UI_Context.Classes
{
    public static class AppCenterHelper
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
#if !DEBUG
        private const string APP_CENTER_SECRET = "523e7039-f6cb-4bf1-9000-53277ed97c53";
#endif

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
        public static void SetupAppCenter(EventHandler<Microsoft.AppCenter.Push.PushNotificationReceivedEventArgs> appCenterPushCallback)
        {
            try
            {
#if !DEBUG
                AppCenter.Start(APP_CENTER_SECRET, typeof(Crashes));
                if (Settings.getSettingBoolean(SettingsConsts.DISABLE_CRASH_REPORTING))
                {
                    Crashes.Instance.InstanceEnabled = false;
                    Logger.Info("AppCenter crash reporting is disabled.");
                }

                AppCenter.Start(APP_CENTER_SECRET, typeof(Analytics));
                if (Settings.getSettingBoolean(SettingsConsts.DISABLE_ANALYTICS))
                {
                    Analytics.SetEnabledAsync(false);
                    Logger.Info("AppCenter analytics are disabled.");
                }
                // Only enable push for debug builds:
                AppCenter.Start(APP_CENTER_SECRET, typeof(Microsoft.AppCenter.Push.Push));
#endif

                if (!AppCenter.Configured)
                {
                    Microsoft.AppCenter.Push.Push.PushNotificationReceived -= appCenterPushCallback;
                    Microsoft.AppCenter.Push.Push.PushNotificationReceived += appCenterPushCallback;
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
