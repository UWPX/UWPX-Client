using Shared.Classes;

namespace UWPX_UI_Context.Classes.DataTemplates.Dialogs
{
    public sealed class WhatsNewDialogDataTemplate: AbstractDataTemplate
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        private bool _ShowOnStartup;
        public bool ShowOnStartup
        {
            get => _ShowOnStartup;
            set => SetShowOnStartupProperty(value);
        }
        private bool _ToDonatePageNavigated;
        public bool ToDonatePageNavigated
        {
            get => _ToDonatePageNavigated;
            set => SetProperty(ref _ToDonatePageNavigated, value);
        }

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        public WhatsNewDialogDataTemplate()
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
                Settings.SetSetting(SettingsConsts.HIDE_WHATS_NEW_DIALOG, !value);
            }
        }

        #endregion
        //--------------------------------------------------------Misc Methods:---------------------------------------------------------------\\
        #region --Misc Methods (Public)--
        public void LoadSettings()
        {
            ShowOnStartup = !Settings.GetSettingBoolean(SettingsConsts.HIDE_WHATS_NEW_DIALOG);
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
