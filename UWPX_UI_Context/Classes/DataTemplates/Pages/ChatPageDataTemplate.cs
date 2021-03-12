using System.Linq;
using Manager.Classes.Chat;
using Manager.Classes.Toast;
using Shared.Classes;
using UWPX_UI_Context.Classes.Collections;

namespace UWPX_UI_Context.Classes.DataTemplates.Pages
{
    public sealed class ChatPageDataTemplate: AbstractDataTemplate
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        public readonly ChatFilterDataTemplate CHAT_FILTER;

        public readonly AdvancedChatsCollection CHATS_ACC;

        private ChatDataTemplate _SelectedItem;
        public ChatDataTemplate SelectedItem
        {
            get => _SelectedItem;
            set => SetSelectedItem(value);
        }

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        public ChatPageDataTemplate()
        {
            CHATS_ACC = new AdvancedChatsCollection(DataCache.INSTANCE.CHATS);
            CHATS_ACC.Filter();
            CHATS_ACC.Sort();
            CHAT_FILTER = new ChatFilterDataTemplate(CHATS_ACC);

            SelectedItem = DataCache.INSTANCE.SelectedChat;
        }

        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--
        private void SetSelectedItem(ChatDataTemplate value)
        {
            if (SetProperty(ref _SelectedItem, value, nameof(SelectedItem)))
            {
                DataCache.INSTANCE.SelectedChat = value;
            }
        }

        #endregion
        //--------------------------------------------------------Misc Methods:---------------------------------------------------------------\\
        #region --Misc Methods (Public)--
        public void EvaluateOnNavigatedToArgs(object parameter)
        {
            if (parameter is ChatToastActivation args)
            {
                SelectedItem = CHATS_ACC.First((x) => x is ChatDataTemplate chat && chat?.Chat.id == args.CHAT_ID);
            }
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
