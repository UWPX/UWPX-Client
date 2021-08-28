using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Logging;
using Microsoft.AppCenter.Analytics;
using Microsoft.AppCenter.Crashes;
#if !DEBUG
using Microsoft.AppCenter;
using Storage.Classes;
#endif

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
        /// Sets up App Center crash and analytics support.
        /// </summary>
        public static void SetupAppCenter()
        {
            try
            {
#if !DEBUG
                AppCenter.Start(APP_CENTER_SECRET, typeof(Crashes));
                if (Settings.GetSettingBoolean(SettingsConsts.DISABLE_CRASH_REPORTING))
                {
                    Crashes.Instance.InstanceEnabled = false;
                    Logger.Info("AppCenter crash reporting is disabled.");
                }

                AppCenter.Start(APP_CENTER_SECRET, typeof(Analytics));
                if (Settings.GetSettingBoolean(SettingsConsts.DISABLE_ANALYTICS))
                {
                    Analytics.SetEnabledAsync(false);
                    Logger.Info("AppCenter analytics are disabled.");
                }
#endif
            }
            catch (Exception e)
            {
                Logger.Error("Failed to start APPCenter!", e);
                throw e;
            }
            Logger.Info("App Center crash reporting registered.");
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
