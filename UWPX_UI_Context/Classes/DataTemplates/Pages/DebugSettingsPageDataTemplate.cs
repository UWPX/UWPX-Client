
using System.Runtime.CompilerServices;
using Manager.Classes;
using Shared.Classes;
using Storage.Classes;

namespace UWPX_UI_Context.Classes.DataTemplates.Pages
{
    public sealed class DebugSettingsPageDataTemplate: AbstractDataTemplate
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        private bool _DisableTcpTimeout;
        public bool DisableTcpTimeout
        {
            get => _DisableTcpTimeout;
            set => SetBoolProperty(ref _DisableTcpTimeout, value, SettingsConsts.DEBUG_DISABLE_TCP_TIMEOUT);
        }
        private bool _DisableTlsTimeout;
        public bool DisableTlsTimeout
        {
            get => _DisableTlsTimeout;
            set => SetBoolProperty(ref _DisableTlsTimeout, value, SettingsConsts.DEBUG_DISABLE_TLS_TIMEOUT);
        }
        private bool _SpamDetectionEnabled;
        public bool SpamDetectionEnabled
        {
            get => _SpamDetectionEnabled;
            set => SetBoolProperty(ref _SpamDetectionEnabled, value, SettingsConsts.SPAM_DETECTION_ENABLED);
        }
        private bool _SpamDetectionNewChatsOnly;
        public bool SpamDetectionNewChatsOnly
        {
            get => _SpamDetectionNewChatsOnly;
            set => SetBoolInversedProperty(ref _SpamDetectionNewChatsOnly, value, SettingsConsts.SPAM_DETECTION_FOR_ALL_CHAT_MESSAGES);
        }
        private string _SpamRegex;
        public string SpamRegex
        {
            get => _SpamRegex;
            set => SetSpamRegexProperty(value);
        }
        private string _DeviceNonce;
        public string DeviceNonce
        {
            get => _DeviceNonce;
            set => SetProperty(ref _DeviceNonce, value);
        }
        private string _DeviceID;
        public string DeviceID
        {
            get => _DeviceID;
            set => SetProperty(ref _DeviceID, value);
        }
        private string _AccountIds;
        public string AccountIds
        {
            get => _AccountIds;
            set => SetProperty(ref _AccountIds, value);
        }

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--


        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--
        private bool SetSpamRegexProperty(string value)
        {
            if (SetProperty(ref _SpamRegex, value, nameof(SpamRegex)))
            {
                Settings.SetSetting(SettingsConsts.SPAM_REGEX, value);
                SpamHelper.INSTANCE.UpdateSpamRegex(value);
                return true;
            }
            return false;
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

        private bool SetBoolInversedProperty(ref bool storage, bool value, string settingsToken, [CallerMemberName] string propertyName = null)
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
