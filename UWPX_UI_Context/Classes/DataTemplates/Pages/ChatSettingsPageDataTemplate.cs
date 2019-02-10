using Data_Manager2.Classes;
using Shared.Classes;
using System.Runtime.CompilerServices;

namespace UWPX_UI_Context.Classes.DataTemplates.Pages
{
    public class ChatSettingsPageDataTemplate : AbstractDataTemplate
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        private bool _EnterToSend;
        public bool EnterToSend
        {
            get { return _EnterToSend; }
            set { SetBoolProperty(ref _EnterToSend, value, SettingsConsts.ENTER_TO_SEND_MESSAGES); }
        }
        private bool _SendChatState;
        public bool SendChatState
        {
            get { return _SendChatState; }
            set { SetBoolInversedProperty(ref _SendChatState, value, SettingsConsts.DONT_SEND_CHAT_STATE); }
        }
        private bool _SendReceivedMarkers;
        public bool SendReceivedMarkers
        {
            get { return _SendReceivedMarkers; }
            set { SetBoolInversedProperty(ref _SendReceivedMarkers, value, SettingsConsts.DONT_SEND_CHAT_MESSAGE_RECEIVED_MARKERS); }
        }
        private bool _AdvancedChatMessageProcessing;
        public bool AdvancedChatMessageProcessing
        {
            get { return _AdvancedChatMessageProcessing; }
            set { SetBoolInversedProperty(ref _AdvancedChatMessageProcessing, value, SettingsConsts.DISABLE_ADVANCED_CHAT_MESSAGE_PROCESSING); }
        }
        private bool _AutoJoinMucs;
        public bool AutoJoinMucs
        {
            get { return _AutoJoinMucs; }
            set { SetBoolInversedProperty(ref _AutoJoinMucs, value, SettingsConsts.DISABLE_AUTO_JOIN_MUC); }
        }
        private bool _ImageAutoDownload;
        public bool ImageAutoDownload
        {
            get { return _ImageAutoDownload; }
            set { SetBoolInversedProperty(ref _ImageAutoDownload, value, SettingsConsts.DISABLE_IMAGE_AUTO_DOWNLOAD); }
        }
        private bool _StoreImagesInLibrary;
        public bool StoreImagesInLibrary
        {
            get { return _StoreImagesInLibrary; }
            set { SetBoolInversedProperty(ref _StoreImagesInLibrary, value, SettingsConsts.DISABLE_DOWNLOAD_IMAGES_TO_LIBARY); }
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
            EnterToSend = Settings.getSettingBoolean(SettingsConsts.ENTER_TO_SEND_MESSAGES);
            SendChatState = !Settings.getSettingBoolean(SettingsConsts.DONT_SEND_CHAT_STATE);
            SendReceivedMarkers = !Settings.getSettingBoolean(SettingsConsts.DONT_SEND_CHAT_MESSAGE_RECEIVED_MARKERS);
            AdvancedChatMessageProcessing = !Settings.getSettingBoolean(SettingsConsts.DISABLE_ADVANCED_CHAT_MESSAGE_PROCESSING);

            // MUC:
            AutoJoinMucs = !Settings.getSettingBoolean(SettingsConsts.DISABLE_AUTO_JOIN_MUC);

            // Media:
            ImageAutoDownload = !Settings.getSettingBoolean(SettingsConsts.DISABLE_IMAGE_AUTO_DOWNLOAD);
            StoreImagesInLibrary = !Settings.getSettingBoolean(SettingsConsts.DISABLE_DOWNLOAD_IMAGES_TO_LIBARY);
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

        #region --Misc Methods (Protected)--


        #endregion
        //--------------------------------------------------------Events:---------------------------------------------------------------------\\
        #region --Events--


        #endregion
    }
}
