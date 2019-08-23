using Data_Manager2.Classes.DBManager;
using Data_Manager2.Classes.DBTables;
using Shared.Classes;
using Shared.Classes.Collections;
using XMPP_API.Classes;
using XMPP_API.Classes.Network.XML.Messages.XEP_0384;

namespace UWPX_UI_Context.Classes.DataTemplates.Controls.Chat
{
    public class ContactOmemoControlDataTemplate: AbstractDataTemplate
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        private bool _Loading;
        public bool Loading
        {
            get => _Loading;
            set => SetProperty(ref _Loading, value);
        }

        private ChatTable _Chat;
        public ChatTable Chat
        {
            get => _Chat;
            set => SetChatProperty(value);
        }

        private XMPPClient _Client;
        public XMPPClient Client
        {
            get => _Client;
            set => SetProperty(ref _Client, value);
        }

        private bool _TrustedOnly;
        public bool TrustedOnly
        {
            get => _TrustedOnly;
            set => SetTrustedOnlyPropery(value);
        }

        private bool _NoFingerprintsFound;
        public bool NoFingerprintsFound
        {
            get => _NoFingerprintsFound;
            set => SetProperty(ref _NoFingerprintsFound, value);
        }

        public readonly CustomObservableCollection<OmemoFingerprint> FINGERPRINTS = new CustomObservableCollection<OmemoFingerprint>(true);

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--


        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--


        #endregion
        //--------------------------------------------------------Misc Methods:---------------------------------------------------------------\\
        #region --Misc Methods (Public)--


        #endregion

        #region --Misc Methods (Private)--
        private void SetTrustedOnlyPropery(bool value)
        {
            if (SetProperty(ref _TrustedOnly, value, nameof(TrustedOnly)) && !(Chat is null) && Chat.omemoTrustedKeysOnly != value)
            {
                Chat.omemoTrustedKeysOnly = value;
                ChatDBManager.INSTANCE.setChatTableValue(nameof(Chat.id), Chat.id, nameof(Chat.omemoTrustedKeysOnly), Chat.omemoTrustedKeysOnly);
            }
        }

        private void SetChatProperty(ChatTable value)
        {
            if (SetProperty(ref _Chat, value, nameof(Chat)))
            {
                TrustedOnly = !(Chat is null) && Chat.omemoTrustedKeysOnly;
            }
        }

        #endregion

        #region --Misc Methods (Protected)--


        #endregion
        //--------------------------------------------------------Events:---------------------------------------------------------------------\\
        #region --Events--


        #endregion
    }
}
