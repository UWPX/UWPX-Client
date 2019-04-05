using Shared.Classes;
using XMPP_API.Classes;

namespace UWPX_UI_Context.Classes.DataTemplates.Dialogs
{
    public sealed class AddChatDialogDataTemplate : AbstractDataTemplate
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        private bool _Confirmed;
        public bool Confirmed
        {
            get { return _Confirmed; }
            set { SetProperty(ref _Confirmed, value); }
        }

        private bool _AddToRoster;
        public bool AddToRoster
        {
            get { return _AddToRoster; }
            set { SetProperty(ref _AddToRoster, value); }
        }

        private bool _SubscribeToPresence;
        public bool SubscribeToPresence
        {
            get { return _SubscribeToPresence; }
            set { SetProperty(ref _SubscribeToPresence, value); }
        }

        private string _ChatBareJid;
        public string ChatBareJid
        {
            get { return _ChatBareJid; }
            set { SetProperty(ref _ChatBareJid, value); }
        }

        private XMPPClient _Client;
        public XMPPClient Client
        {
            get { return _Client; }
            set { SetProperty(ref _Client, value); }
        }

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        public AddChatDialogDataTemplate()
        {
            AddToRoster = true;
            SubscribeToPresence = true;
        }

        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--


        #endregion
        //--------------------------------------------------------Misc Methods:---------------------------------------------------------------\\
        #region --Misc Methods (Public)--
        public void OnChatSelected(ChatDataTemplate chat)
        {
            ChatBareJid = chat.Chat.chatJabberId;
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
