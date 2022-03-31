using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Logging;
using Manager.Classes;
using Shared.Classes;
using Storage.Classes;
using Windows.Storage;

namespace UWPX_UI_Context.Classes.DataTemplates.Pages
{
    public class MiscSettingsPageDataTemplate: AbstractDataTemplate
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        private bool _Analytics;
        public bool Analytics
        {
            get => _Analytics;
            set => SetAnalytics(value);
        }
        private bool _CrashReports;
        public bool CrashReports
        {
            get => _CrashReports;
            set => SetCrashreports(value);
        }
        private bool _AutomaticExtendedCrashReports;
        public bool AutomaticExtendedCrashReports
        {
            get => _AutomaticExtendedCrashReports;
            set => SetBoolProperty(ref _AutomaticExtendedCrashReports, value, SettingsConsts.ALWAYS_REPORT_CRASHES_WITHOUT_ASKING);
        }
        private bool _ShowWelcomeDialogOnStartup;
        public bool ShowWelcomeDialogOnStartup
        {
            get => _ShowWelcomeDialogOnStartup;
            set => SetBoolInvertedProperty(ref _ShowWelcomeDialogOnStartup, value, SettingsConsts.HIDE_WELCOME_DIALOG);
        }
        private bool _ShowWhatsNewDialogOnStartup;
        public bool ShowWhatsNewDialogOnStartup
        {
            get => _ShowWhatsNewDialogOnStartup;
            set => SetBoolInvertedProperty(ref _ShowWhatsNewDialogOnStartup, value, SettingsConsts.HIDE_WHATS_NEW_DIALOG);
        }

        private string _LogFolderPath;
        public string LogFolderPath
        {
            get => _LogFolderPath;
            set => SetProperty(ref _LogFolderPath, value);
        }
        private string _ImageCacheFolderPath;
        public string ImageCacheFolderPath
        {
            get => _ImageCacheFolderPath;
            set => SetProperty(ref _ImageCacheFolderPath, value);
        }
        private bool _IsRunningOnPc;
        public bool IsRunningOnPc
        {
            get => _IsRunningOnPc;
            set => SetProperty(ref _IsRunningOnPc, value);
        }

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        public MiscSettingsPageDataTemplate()
        {
            LoadSettings();
        }

        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--
        private void SetAnalytics(bool value)
        {
            if (SetProperty(ref _Analytics, value, nameof(Analytics)))
            {
                Settings.SetSetting(SettingsConsts.DISABLE_ANALYTICS, !value);
                Task.Run(async () => await AppCenterHelper.SetAnalyticsEnabledAsync(value));
            }
        }

        private void SetCrashreports(bool value)
        {
            if (SetProperty(ref _CrashReports, value, nameof(CrashReports)))
            {
                Settings.SetSetting(SettingsConsts.DISABLE_CRASH_REPORTING, !value);
                AppCenterHelper.SetCrashesEnabledAsync(value);
            }
        }

        private bool SetBoolProperty(ref bool storage, bool value, string settingsToken, [CallerMemberName] string propertyName = null)
        {
            if (SetProperty(ref storage, value, propertyName))
            {
                Settings.SetSetting(settingsToken, value);
                return true;
            }
            return false;
        }

        private bool SetBoolInvertedProperty(ref bool storage, bool value, string settingsToken, [CallerMemberName] string propertyName = null)
        {
            if (SetProperty(ref storage, value, propertyName))
            {
                Settings.SetSetting(settingsToken, !value);
                return true;
            }
            return false;
        }

        #endregion
        //--------------------------------------------------------Misc Methods:---------------------------------------------------------------\\
        #region --Misc Methods (Public)--


        #endregion

        #region --Misc Methods (Private)--
        private void LoadSettings()
        {
            Analytics = !Settings.GetSettingBoolean(SettingsConsts.DISABLE_ANALYTICS);
            CrashReports = !Settings.GetSettingBoolean(SettingsConsts.DISABLE_CRASH_REPORTING);
            AutomaticExtendedCrashReports = Settings.GetSettingBoolean(SettingsConsts.ALWAYS_REPORT_CRASHES_WITHOUT_ASKING);
            ShowWelcomeDialogOnStartup = !Settings.GetSettingBoolean(SettingsConsts.HIDE_WELCOME_DIALOG);
            ShowWhatsNewDialogOnStartup = !Settings.GetSettingBoolean(SettingsConsts.HIDE_WHATS_NEW_DIALOG);
            IsRunningOnPc = DeviceFamilyHelper.IsRunningOnDesktopDevice();

            Task.Run(async () =>
            {
                StorageFolder folder = await ConnectionHandler.INSTANCE.IMAGE_DOWNLOAD_HANDLER.GetCacheFolderAsync();
                ImageCacheFolderPath = folder is null ? "" : folder.Path;

                folder = await Logger.GetLogFolderAsync();
                LogFolderPath = folder is null ? "" : folder.Path;
            });
        }

        #endregion

        #region --Misc Methods (Protected)--


        #endregion
        //--------------------------------------------------------Events:---------------------------------------------------------------------\\
        #region --Events--


        #endregion
    }
}
