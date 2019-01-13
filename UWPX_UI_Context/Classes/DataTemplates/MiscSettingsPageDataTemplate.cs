using Data_Manager2.Classes;
using Data_Manager2.Classes.DBManager;
using Logging;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Windows.Storage;

namespace UWPX_UI_Context.Classes.DataTemplates
{
    public class MiscSettingsPageDataTemplate : AbstractDataTemplate
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        private bool _Analytics;
        public bool Analytics
        {
            get { return _Analytics; }
            set { SetAnalytics(value); }
        }
        private bool _Crashreports;
        public bool Crashreports
        {
            get { return _Crashreports; }
            set { SetCrashreports(value); }
        }
        private bool _ShowWelcomeDialogOnStartup;
        public bool ShowWelcomeDialogOnStartup
        {
            get { return _ShowWelcomeDialogOnStartup; }
            set { SetBoolInversedProperty(ref _ShowWelcomeDialogOnStartup, value, SettingsConsts.HIDE_INITIAL_START_DIALOG_ALPHA); }
        }
        private bool _ShowWhatsNewDialogOnStartup;
        public bool ShowWhatsNewDialogOnStartup
        {
            get { return _ShowWhatsNewDialogOnStartup; }
            set { SetBoolInversedProperty(ref _ShowWhatsNewDialogOnStartup, value, SettingsConsts.HIDE_WHATS_NEW_DIALOG); }
        }

        private string _LogFolderPath;
        public string LogFolderPath
        {
            get { return _LogFolderPath; }
            set { SetProperty(ref _LogFolderPath, value); }
        }
        private string _ImageCacheFolderPath;
        public string ImageCacheFolderPath
        {
            get { return _ImageCacheFolderPath; }
            set { SetProperty(ref _ImageCacheFolderPath, value); }
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
                Settings.setSetting(SettingsConsts.DISABLE_ANALYTICS, !value);
                Task.Run(async () => await AppCenterHelper.SetAnalyticsEnabledAsync(value));
            }
        }

        private void SetCrashreports(bool value)
        {
            if (SetProperty(ref _Crashreports, value, nameof(Crashreports)))
            {
                Settings.setSetting(SettingsConsts.DISABLE_CRASH_REPORTING, !value);
                Task.Run(async () => await AppCenterHelper.SetCrashesEnabledAsync(value));
            }
        }

        private bool SetBoolProperty(ref bool storage, bool value, string settingsToken, [CallerMemberName] string propertyName = null)
        {
            if (SetProperty(ref storage, value, propertyName))
            {
                Settings.setSetting(settingsToken, value);
                return true;
            }
            return false;
        }

        private bool SetBoolInversedProperty(ref bool storage, bool value, string settingsToken, [CallerMemberName] string propertyName = null)
        {
            if (SetProperty(ref storage, value, propertyName))
            {
                Settings.setSetting(settingsToken, !value);
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
            Analytics = !Settings.getSettingBoolean(SettingsConsts.DISABLE_ANALYTICS);
            Crashreports = !Settings.getSettingBoolean(SettingsConsts.DISABLE_CRASH_REPORTING);
            ShowWelcomeDialogOnStartup = !Settings.getSettingBoolean(SettingsConsts.HIDE_INITIAL_START_DIALOG_ALPHA);
            ShowWhatsNewDialogOnStartup = !Settings.getSettingBoolean(SettingsConsts.HIDE_WHATS_NEW_DIALOG);

            Task.Run(async () =>
            {
                StorageFolder folder = await ImageDBManager.INSTANCE.getCachedImagesFolderAsync();
                if (!(folder is null))
                {
                    ImageCacheFolderPath = folder.Path;
                }
                else
                {
                    ImageCacheFolderPath = "";
                }

                folder = await Logger.getLogFolderAsync();
                if (!(folder is null))
                {
                    LogFolderPath = folder.Path;
                }
                else
                {
                    LogFolderPath = "";
                }
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
