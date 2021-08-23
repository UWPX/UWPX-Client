using System.Runtime.CompilerServices;
using Push.Classes;
using Shared.Classes;
using Storage.Classes;

namespace UWPX_UI_Context.Classes.DataTemplates.Pages
{
    public sealed class AccountSettingsPageDataTemplate: AbstractDataTemplate
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        private bool _PushEnabled;
        public bool PushEnabled
        {
            get => _PushEnabled;
            set => SetPushEnabledProperty(value);
        }

        private string _ChannelUri;
        public string ChannelUri
        {
            get => _ChannelUri;
            set => SetProperty(ref _ChannelUri, value);
        }

        private string _ChannelCreatedDate;
        public string ChannelCreatedDate
        {
            get => _ChannelCreatedDate;
            set => SetProperty(ref _ChannelCreatedDate, value);
        }

        private string _PushManagerState;
        public string PushManagerState
        {
            get => _PushManagerState;
            set => SetProperty(ref _PushManagerState, value);
        }

        private bool _DebugSettingsEnabled;
        public bool DebugSettingsEnabled
        {
            get => _DebugSettingsEnabled;
            set => SetProperty(ref _DebugSettingsEnabled, value);
        }

        private bool _PushInitialized;
        public bool PushInitialized
        {
            get => _PushInitialized;
            set => SetProperty(ref _PushInitialized, value);
        }

        private bool _PushError;
        public bool PushError
        {
            get => _PushError;
            set => SetProperty(ref _PushError, value);
        }

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--


        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--
        private bool SetBoolProperty(ref bool storage, bool value, string settingsToken, [CallerMemberName] string propertyName = null)
        {
            if (SetProperty(ref storage, value, propertyName) && Settings.GetSettingBoolean(settingsToken) != value)
            {
                Settings.SetSetting(settingsToken, value);
                return true;
            }
            return false;
        }

        private bool SetPushEnabledProperty(bool value)
        {
            if (SetBoolProperty(ref _PushEnabled, value, SettingsConsts.PUSH_ENABLED, nameof(PushEnabled)))
            {
                PushManager.INSTANCE.Init();
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
