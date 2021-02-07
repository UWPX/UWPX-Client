using System.Runtime.CompilerServices;
using Shared.Classes;
using Storage.Classes;

namespace UWPX_UI_Context.Classes.DataTemplates.Pages
{
    public class ChatSettingsPageDataTemplate: AbstractDataTemplate
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        private bool _EnterToSend;
        public bool EnterToSend
        {
            get => _EnterToSend;
            set => SetBoolProperty(ref _EnterToSend, value, SettingsConsts.ENTER_TO_SEND_MESSAGES);
        }
        private bool _SendChatState;
        public bool SendChatState
        {
            get => _SendChatState;
            set => SetBoolInversedProperty(ref _SendChatState, value, SettingsConsts.DONT_SEND_CHAT_STATE);
        }
        private bool _SendReceivedMarkers;
        public bool SendReceivedMarkers
        {
            get => _SendReceivedMarkers;
            set => SetBoolInversedProperty(ref _SendReceivedMarkers, value, SettingsConsts.DONT_SEND_CHAT_MESSAGE_RECEIVED_MARKERS);
        }
        private bool _AdvancedChatMessageProcessing;
        public bool AdvancedChatMessageProcessing
        {
            get => _AdvancedChatMessageProcessing;
            set => SetBoolInversedProperty(ref _AdvancedChatMessageProcessing, value, SettingsConsts.DISABLE_ADVANCED_CHAT_MESSAGE_PROCESSING);
        }
        private bool _AutoJoinMucs;
        public bool AutoJoinMucs
        {
            get => _AutoJoinMucs;
            set => SetBoolInversedProperty(ref _AutoJoinMucs, value, SettingsConsts.DISABLE_AUTO_JOIN_MUC);
        }
        private bool _ImageAutoDownload;
        public bool ImageAutoDownload
        {
            get => _ImageAutoDownload;
            set => SetBoolInversedProperty(ref _ImageAutoDownload, value, SettingsConsts.DISABLE_IMAGE_AUTO_DOWNLOAD);
        }
        private bool _StoreImagesInLibrary;
        public bool StoreImagesInLibrary
        {
            get => _StoreImagesInLibrary;
            set => SetBoolInversedProperty(ref _StoreImagesInLibrary, value, SettingsConsts.DISABLE_DOWNLOAD_IMAGES_TO_LIBARY);
        }
        private bool _EnableOmemoForNewChats;
        public bool EnableOmemoForNewChats
        {
            get => _EnableOmemoForNewChats;
            set => SetBoolProperty(ref _EnableOmemoForNewChats, value, SettingsConsts.ENABLE_OMEMO_BY_DEFAULT_FOR_NEW_CHATS);
        }
        private bool _VibrateForNewChatMessages;
        public bool VibrateForNewChatMessages
        {
            get => _VibrateForNewChatMessages;
            set => SetBoolInversedProperty(ref _VibrateForNewChatMessages, value, SettingsConsts.DISABLE_VIBRATION_FOR_NEW_CHAT_MESSAGES);
        }
        private bool _PlaySoundForNewChatMessages;
        public bool PlaySoundForNewChatMessages
        {
            get => _PlaySoundForNewChatMessages;
            set => SetBoolInversedProperty(ref _PlaySoundForNewChatMessages, value, SettingsConsts.DISABLE_PLAY_SOUND_FOR_NEW_CHAT_MESSAGES);
        }
        private bool _IsEmojiButtonEnabled;
        public bool IsEmojiButtonEnabled
        {
            get => _IsEmojiButtonEnabled;
            set => SetBoolProperty(ref _IsEmojiButtonEnabled, value, SettingsConsts.CHAT_ENABLE_EMOJI_BUTTON);
        }
        private int _ShowAccountColor;
        public int ShowAccountColor
        {
            get => _ShowAccountColor;
            set => SetIntProperty(ref _ShowAccountColor, value, SettingsConsts.CHAT_SHOW_ACCOUNT_COLOR);
        }
        private bool _VibrationSupported;
        public bool VibrationSupported
        {
            get => _VibrationSupported;
            set => SetProperty(ref _VibrationSupported, value);
        }

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        public ChatSettingsPageDataTemplate()
        {
            LoadSettings();
        }

        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--


        #endregion
        //--------------------------------------------------------Misc Methods:---------------------------------------------------------------\\
        #region --Misc Methods (Public)--


        #endregion

        #region --Misc Methods (Private)--
        private void LoadSettings()
        {
            // General:
            EnterToSend = Settings.GetSettingBoolean(SettingsConsts.ENTER_TO_SEND_MESSAGES);
            SendChatState = !Settings.GetSettingBoolean(SettingsConsts.DONT_SEND_CHAT_STATE);
            SendReceivedMarkers = !Settings.GetSettingBoolean(SettingsConsts.DONT_SEND_CHAT_MESSAGE_RECEIVED_MARKERS);
            AdvancedChatMessageProcessing = !Settings.GetSettingBoolean(SettingsConsts.DISABLE_ADVANCED_CHAT_MESSAGE_PROCESSING);
            VibrateForNewChatMessages = !Settings.GetSettingBoolean(SettingsConsts.DISABLE_VIBRATION_FOR_NEW_CHAT_MESSAGES);
            PlaySoundForNewChatMessages = !Settings.GetSettingBoolean(SettingsConsts.DISABLE_PLAY_SOUND_FOR_NEW_CHAT_MESSAGES);
            IsEmojiButtonEnabled = Settings.GetSettingBoolean(SettingsConsts.CHAT_ENABLE_EMOJI_BUTTON);
            ShowAccountColor = Settings.GetSettingInt(SettingsConsts.CHAT_SHOW_ACCOUNT_COLOR, 0);

            // MUC:
            AutoJoinMucs = !Settings.GetSettingBoolean(SettingsConsts.DISABLE_AUTO_JOIN_MUC);

            // Media:
            ImageAutoDownload = !Settings.GetSettingBoolean(SettingsConsts.DISABLE_IMAGE_AUTO_DOWNLOAD);
            StoreImagesInLibrary = !Settings.GetSettingBoolean(SettingsConsts.DISABLE_DOWNLOAD_IMAGES_TO_LIBARY);

            // OMEMO:
            EnableOmemoForNewChats = Settings.GetSettingBoolean(SettingsConsts.ENABLE_OMEMO_BY_DEFAULT_FOR_NEW_CHATS);

            // Misc:
            VibrationSupported = DeviceFamilyHelper.SupportsVibration();
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

        private bool SetIntProperty(ref int storage, int value, string settingsToken, [CallerMemberName] string propertyName = null)
        {
            if (SetProperty(ref storage, value, propertyName))
            {
                Settings.SetSetting(settingsToken, value);
                return true;
            }
            return false;
        }

        #endregion

        #region --Misc Methods (Protected)--


        #endregion
        //--------------------------------------------------------Events:---------------------------------------------------------------------\\
        #region --Events--


        #endregion
    }
}
