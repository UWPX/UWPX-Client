using Data_Manager2.Classes;
using Data_Manager2.Classes.DBManager;
using Data_Manager2.Classes.DBTables;
using Shared.Classes;
using Shared.Classes.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UWPX_UI_Context.Classes.Collections.Toolkit;
using XMPP_API.Classes;

namespace UWPX_UI_Context.Classes.DataTemplates.Controls
{
    public sealed class ChatSuggestionsControlDataTemplate : AbstractDataTemplate
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        private bool _IsLoading;
        public bool IsLoading
        {
            get { return _IsLoading; }
            set { SetProperty(ref _IsLoading, value); }
        }

        private readonly CustomObservableCollection<ChatDataTemplate> SUGGESTIONS = new CustomObservableCollection<ChatDataTemplate>(true);
        public readonly AdvancedCollectionView SUGGESTIONS_ACV;
        private string filterText = "";

        private Task loadingTask;

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        public ChatSuggestionsControlDataTemplate()
        {
            SUGGESTIONS_ACV = new AdvancedCollectionView(SUGGESTIONS)
            {
                Filter = AcvFilter
            };
        }

        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--


        #endregion
        //--------------------------------------------------------Misc Methods:---------------------------------------------------------------\\
        #region --Misc Methods (Public)--
        public async Task UpdateViewAsync(XMPPClient client)
        {
            await LoadSuggestionsAsync(client);
        }

        public void UpdateView(string filterText)
        {
            if (!string.Equals(filterText, this.filterText))
            {
                this.filterText = filterText;
                SUGGESTIONS_ACV.RefreshFilter();
            }
        }

        #endregion

        #region --Misc Methods (Private)--
        private async Task LoadSuggestionsAsync(XMPPClient client)
        {
            if (!(loadingTask is null))
            {
                await loadingTask;
            }

            IsLoading = true;
            loadingTask = Task.Run(() =>
            {
                if (client is null)
                {
                    SUGGESTIONS.Clear();
                    return;
                }

                List<ChatTable> chats = ChatDBManager.INSTANCE.getNotStartedChatsForClient(client.getXMPPAccount().getBareJid(), ChatType.CHAT);
                SUGGESTIONS.Clear();
                SUGGESTIONS.AddRange(chats.Select((x) =>
                {
                    return new ChatDataTemplate
                    {
                        Chat = x,
                        Client = client
                    };
                }));
                SUGGESTIONS_ACV.RefreshFilter();
            });

            await loadingTask;
            IsLoading = false;
        }

        private bool AcvFilter(object o)
        {
            return o is ChatDataTemplate chat && (string.IsNullOrWhiteSpace(filterText) || chat.Chat.chatJabberId.Contains(filterText));
        }

        #endregion

        #region --Misc Methods (Protected)--


        #endregion
        //--------------------------------------------------------Events:---------------------------------------------------------------------\\
        #region --Events--


        #endregion
    }
}
