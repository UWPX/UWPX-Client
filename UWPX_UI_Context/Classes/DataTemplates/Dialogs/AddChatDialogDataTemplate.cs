using System.Threading.Tasks;
using Manager.Classes;
using Manager.Classes.Chat;
using Shared.Classes;
using Storage.Classes.Models.Chat;

namespace UWPX_UI_Context.Classes.DataTemplates.Dialogs
{
    public sealed class AddChatDialogDataTemplate: AbstractDataTemplate
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        private bool _Confirmed;
        public bool Confirmed
        {
            get => _Confirmed;
            set => SetProperty(ref _Confirmed, value);
        }

        private bool _AddToRoster;
        public bool AddToRoster
        {
            get => _AddToRoster;
            set => SetProperty(ref _AddToRoster, value);
        }

        private bool _ChatExists;
        public bool ChatExists
        {
            get => _ChatExists;
            set => SetChatExistsProperty(value);
        }

        private bool _IsBareJidValid;
        public bool IsBareJidValid
        {
            get => _IsBareJidValid;
            set => SetIsBareJidValidProperty(value);
        }

        private bool _IsInputValid;
        public bool IsInputValid
        {
            get => _IsInputValid;
            set => SetProperty(ref _IsInputValid, value);
        }

        private bool _SubscribeToPresence;
        public bool SubscribeToPresence
        {
            get => _SubscribeToPresence;
            set => SetProperty(ref _SubscribeToPresence, value);
        }

        private string _ChatBareJid;
        public string ChatBareJid
        {
            get => _ChatBareJid;
            set => SetChatBareJidProperty(value);
        }

        private bool _IsInRoster;
        public bool IsInRoster
        {
            get => _IsInRoster;
            set => SetProperty(ref _IsInRoster, value);
        }

        private bool _IsSubscribedToPresence;
        public bool IsSubscribedToPresence
        {
            get => _IsSubscribedToPresence;
            set => SetProperty(ref _IsSubscribedToPresence, value);
        }

        private Client _Client;
        public Client Client
        {
            get => _Client;
            set => SetProperty(ref _Client, value);
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
        private void SetChatBareJidProperty(string value)
        {
            // Make sure we only allow lower case JIDs:
            value = value?.ToLowerInvariant();

            if (SetProperty(ref _ChatBareJid, value, nameof(ChatBareJid)))
            {
                if (Client is null || string.IsNullOrEmpty(value))
                {
                    ChatExists = false;
                }
                else
                {
                    Task.Run(() =>
                    {
                        ChatModel chat;
                        using (SemaLock semaLock = DataCache.INSTANCE.NewChatSemaLock())
                        {
                            chat = DataCache.INSTANCE.GetChat(Client.dbAccount.bareJid, value, semaLock);
                        }
                        if (chat is null)
                        {
                            ChatExists = false;
                            IsInRoster = false;
                            IsSubscribedToPresence = false;
                        }
                        else
                        {
                            IsInRoster = chat.inRoster;
                            IsSubscribedToPresence = chat.subscriptionRequested || string.Equals(chat.subscription, "to") || string.Equals(chat.subscription, "both");
                            ChatExists = chat.isChatActive;
                        }
                    });
                }
            }
        }

        private void SetChatExistsProperty(bool value)
        {
            if (SetProperty(ref _ChatExists, value, nameof(ChatExists)))
            {
                UpdateIsInputValid();
            }
        }

        private void SetIsBareJidValidProperty(bool value)
        {
            if (SetProperty(ref _IsBareJidValid, value, nameof(IsBareJidValid)))
            {
                UpdateIsInputValid();
            }
        }

        #endregion
        //--------------------------------------------------------Misc Methods:---------------------------------------------------------------\\
        #region --Misc Methods (Public)--
        public void OnChatSelected(ChatDataTemplate chat)
        {
            ChatBareJid = chat.Chat.bareJid;
        }

        #endregion

        #region --Misc Methods (Private)--
        private void UpdateIsInputValid()
        {
            IsInputValid = IsBareJidValid && !ChatExists;
        }

        #endregion

        #region --Misc Methods (Protected)--


        #endregion
        //--------------------------------------------------------Events:---------------------------------------------------------------------\\
        #region --Events--


        #endregion
    }
}
