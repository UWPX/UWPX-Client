using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Shared.Classes;
using Storage.Classes;

namespace UWPX_UI_Context.Classes.DataTemplates.Dialogs
{
    public sealed class InitialStartDialogDataTemplate: AbstractDataTemplate
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        private bool _ShowOnStartup;
        public bool ShowOnStartup
        {
            get => _ShowOnStartup;
            set => SetBoolInvertedProperty(ref _ShowOnStartup, value, SettingsConsts.HIDE_WELCOME_DIALOG);
        }

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

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--


        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--
        public void SetShowOnStartupProperty(bool value)
        {
            if (SetProperty(ref _ShowOnStartup, value, nameof(ShowOnStartup)))
            {
                Settings.SetSetting(SettingsConsts.HIDE_WELCOME_DIALOG, !value);
            }
        }

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
                Task.Run(async () => await AppCenterHelper.SetCrashesEnabledAsync(value));
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


        #endregion

        #region --Misc Methods (Protected)--


        #endregion
        //--------------------------------------------------------Events:---------------------------------------------------------------------\\
        #region --Events--


        #endregion
    }
}
