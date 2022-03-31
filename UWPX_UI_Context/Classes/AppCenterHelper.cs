using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Logging;
using Microsoft.AppCenter.Analytics;
using Microsoft.AppCenter.Crashes;
using System.Text;
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
        public static void SetCrashesEnabledAsync(bool enabled)
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

        public static string GenerateCrashReport(ErrorReport crashReport)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("--------------------UWPX-Crash-Report--------------------");
            sb.Append("ID: ");
            sb.AppendLine(crashReport.Id);
            sb.Append("Time: ");
            sb.AppendLine(crashReport.AppErrorTime.ToString("dd/MM/yyyy HH:mm:ss"));
            sb.Append("App Build: ");
            sb.AppendLine(crashReport.Device.AppBuild);
            sb.Append("Device: ");
            sb.Append(crashReport.Device.OemName);
            sb.Append(" - ");
            sb.AppendLine(crashReport.Device.Model);
            sb.AppendLine(crashReport.StackTrace);
            sb.AppendLine("---------------------------------------------------------");
            return sb.ToString();
        }

        public static void ReportCrashDetails(string details, string report)
        {
            try
            {
                ErrorAttachmentLog[] attachmentLogs = new ErrorAttachmentLog[]
                {
                    ErrorAttachmentLog.AttachmentWithText(details, "details.txt"),
                    ErrorAttachmentLog.AttachmentWithText(report, "report.txt")
                };
                Crashes.TrackError(new DummyAppCenterException(), new Dictionary<string, string> { { nameof(details), details }, { nameof(report), report } }, attachmentLogs);
                Logger.Info($"Crash reported: {report}\n{details}");
            }
            catch (Exception e)
            {
                Logger.Error("Failed to report crash.", e);
            }
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
