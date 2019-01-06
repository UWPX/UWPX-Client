using Data_Manager2.Classes;
using System.Runtime.CompilerServices;

namespace UWPX_UI_Context.Classes.DataTemplates
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
        private bool _DontSendChatState;
        public bool DontSendChatState
        {
            get { return _DontSendChatState; }
            set { SetBoolProperty(ref _DontSendChatState, value, SettingsConsts.DONT_SEND_CHAT_STATE); }
        }
        private bool _DontSendReceivedMarkers;
        public bool DontSendReceivedMarkers
        {
            get { return _DontSendReceivedMarkers; }
            set { SetBoolProperty(ref _DontSendReceivedMarkers, value, SettingsConsts.DONT_SEND_CHAT_MESSAGE_RECEIVED_MARKERS); }
        }
        private bool _DisableAdvancedChatMessageProcessing;
        public bool DisableAdvancedChatMessageProcessing
        {
            get { return _DisableAdvancedChatMessageProcessing; }
            set { SetBoolProperty(ref _DisableAdvancedChatMessageProcessing, value, SettingsConsts.DISABLE_ADVANCED_CHAT_MESSAGE_PROCESSING); }
        }
        private bool _DisableAutoJoinMucs;
        public bool DisableAutoJoinMucs
        {
            get { return _DisableAutoJoinMucs; }
            set { SetBoolProperty(ref _DisableAutoJoinMucs, value, SettingsConsts.DISABLE_AUTO_JOIN_MUC); }
        }
        private bool _DisableImageAutoDownload;
        public bool DisableImageAutoDownload
        {
            get { return _DisableImageAutoDownload; }
            set { SetBoolProperty(ref _DisableImageAutoDownload, value, SettingsConsts.DISABLE_IMAGE_AUTO_DOWNLOAD); }
        }
        private bool _DisableStoreImagesInLibrary;
        public bool DisableStoreImagesInLibrary
        {
            get { return _DisableStoreImagesInLibrary; }
            set { SetBoolProperty(ref _DisableStoreImagesInLibrary, value, SettingsConsts.DISABLE_DOWNLOAD_IMAGES_TO_LIBARY); }
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

        }

        private bool SetBoolProperty(ref bool storage, bool value, string setting, [CallerMemberName] string propertyName = null)
        {
            if (SetProperty(ref storage, value, propertyName))
            {
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
