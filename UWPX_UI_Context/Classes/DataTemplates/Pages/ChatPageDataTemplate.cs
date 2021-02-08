using System.Linq;
using Manager.Classes.Chat;
using Manager.Classes.Toast;
using Microsoft.Toolkit.Uwp.UI;
using Shared.Classes;

namespace UWPX_UI_Context.Classes.DataTemplates.Pages
{
    public sealed class ChatPageDataTemplate: AbstractDataTemplate
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        public readonly ChatFilterDataTemplate CHAT_FILTER;

        public readonly AdvancedCollectionView CHATS_ACV;

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
            CHATS_ACV = new AdvancedCollectionView(DataCache.INSTANCE.CHATS, true)
            {
                Filter = AcvFilter
            };

            CHATS_ACV.ObserveFilterProperty(nameof(ChatDataTemplate.Chat));
            CHATS_ACV.SortDescriptions.Add(new SortDescription(nameof(ChatDataTemplate.Chat), SortDirection.Descending));
            CHAT_FILTER = new ChatFilterDataTemplate(CHATS_ACV);

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
                SelectedItem = (ChatDataTemplate)CHATS_ACV.First((x) => x is ChatDataTemplate chat && chat?.Chat.id == args.CHAT_ID);
            }
        }

        #endregion

        #region --Misc Methods (Private)--
        private bool AcvFilter(object o)
        {
            return CHAT_FILTER.Filter(o);
        }

        #endregion

        #region --Misc Methods (Protected)--


        #endregion
        //--------------------------------------------------------Events:---------------------------------------------------------------------\\
        #region --Events--


        #endregion
    }
}
