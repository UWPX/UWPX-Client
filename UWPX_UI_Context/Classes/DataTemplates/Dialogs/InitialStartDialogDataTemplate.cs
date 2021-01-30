using Shared.Classes;

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
            set => SetShowOnStartupProperty(value);
        }

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        public InitialStartDialogDataTemplate()
        {
            LoadSettings();
        }

        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--
        public void SetShowOnStartupProperty(bool value)
        {
            if (SetProperty(ref _ShowOnStartup, value, nameof(ShowOnStartup)))
            {
                Settings.SetSetting(SettingsConsts.HIDE_INITIAL_START_DIALOG_ALPHA, !value);
            }
        }

        #endregion
        //--------------------------------------------------------Misc Methods:---------------------------------------------------------------\\
        #region --Misc Methods (Public)--
        public void LoadSettings()
        {
            ShowOnStartup = !Settings.getSettingBoolean(SettingsConsts.HIDE_INITIAL_START_DIALOG_ALPHA);
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
