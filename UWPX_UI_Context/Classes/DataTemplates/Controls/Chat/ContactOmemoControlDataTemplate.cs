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
            set => SetProperty(ref _Chat, value);
        }

        private XMPPClient _Client;
        public XMPPClient Client
        {
            get => _Client;
            set => SetProperty(ref _Client, value);
        }

        public readonly CustomObservableCollection<OmemoFingerprint> FINGERPRINTS = new CustomObservableCollection<OmemoFingerprint>(true);

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        public ContactOmemoControlDataTemplate()
        {
            Loading = true;
        }

        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--


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
