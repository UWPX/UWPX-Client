using Storage.Classes;
using UWPX_UI_Context.Classes.DataTemplates.Dialogs;

namespace UWPX_UI_Context.Classes.DataContext.Dialogs
{
    public sealed class InitialStartDialogContext
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        public readonly InitialStartDialogDataTemplate MODEL = new InitialStartDialogDataTemplate();

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        public InitialStartDialogContext()
        {
            LoadSettings();
        }

        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--


        #endregion
        //--------------------------------------------------------Misc Methods:---------------------------------------------------------------\\
        #region --Misc Methods (Public)--
        private void LoadSettings()
        {
            MODEL.ShowOnStartup = !Settings.GetSettingBoolean(SettingsConsts.HIDE_INITIAL_START_DIALOG_ALPHA);
            MODEL.Analytics = !Settings.GetSettingBoolean(SettingsConsts.DISABLE_ANALYTICS);
            MODEL.CrashReports = !Settings.GetSettingBoolean(SettingsConsts.DISABLE_CRASH_REPORTING);
            MODEL.AutomaticExtendedCrashReports = Settings.GetSettingBoolean(SettingsConsts.ALWAYS_REPORT_CRASHES_WITHOUT_ASKING);
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
