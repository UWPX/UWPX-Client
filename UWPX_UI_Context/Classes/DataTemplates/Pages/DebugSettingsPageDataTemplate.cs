
using System.Runtime.CompilerServices;
using Data_Manager2.Classes;
using Data_Manager2.Classes.DBManager;
using Shared.Classes;

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

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        public DebugSettingsPageDataTemplate()
        {
            LoadSettings();
        }

        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--
        private bool SetSpamRegexProperty(string value)
        {
            if (SetProperty(ref _SpamRegex, value, nameof(SpamRegex)))
            {
                Settings.setSetting(SettingsConsts.SPAM_REGEX, value);
                SpamDBManager.INSTANCE.updateSpamRegex(value);
                return true;
            }
            return false;
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
        public void ResetSpamRegex()
        {
            SpamRegex = SpamDBManager.DEFAULT_SPAM_REGEX;
        }

        #endregion

        #region --Misc Methods (Private)--
        private void LoadSettings()
        {
            // Debug:
            DisableTcpTimeout = Settings.getSettingBoolean(SettingsConsts.DEBUG_DISABLE_TCP_TIMEOUT);
            DisableTlsTimeout = Settings.getSettingBoolean(SettingsConsts.DEBUG_DISABLE_TLS_TIMEOUT);

            // Spam:
            SpamRegex = Settings.getSettingString(SettingsConsts.SPAM_REGEX, SpamDBManager.DEFAULT_SPAM_REGEX);
            SpamDetectionNewChatsOnly = !Settings.getSettingBoolean(SettingsConsts.SPAM_DETECTION_FOR_ALL_CHAT_MESSAGES);
            SpamDetectionEnabled = Settings.getSettingBoolean(SettingsConsts.SPAM_DETECTION_ENABLED);
        }

        #endregion

        #region --Misc Methods (Protected)--


        #endregion
        //--------------------------------------------------------Events:---------------------------------------------------------------------\\
        #region --Events--


        #endregion
    }
}
