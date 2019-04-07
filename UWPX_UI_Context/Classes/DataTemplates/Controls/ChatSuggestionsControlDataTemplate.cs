using Data_Manager2.Classes;
using Data_Manager2.Classes.DBManager;
using Data_Manager2.Classes.DBTables;
using Shared.Classes;
using Shared.Classes.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UWPX_UI_Context.Classes.Collections.Toolkit;
using Windows.Foundation.Collections;
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

        private ChatDataTemplate _SelectedItem;
        public ChatDataTemplate SelectedItem
        {
            get { return _SelectedItem; }
            set { SetProperty(ref _SelectedItem, value); }
        }

        private bool _HasSuggestions;
        public bool HasSuggestions
        {
            get { return _HasSuggestions; }
            set { SetProperty(ref _HasSuggestions, value); }
        }

        private bool _HasFilteredSuggestions;
        public bool HasFilteredSuggestions
        {
            get { return _HasFilteredSuggestions; }
            set { SetProperty(ref _HasFilteredSuggestions, value); }
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
            SUGGESTIONS.CollectionChanged += SUGGESTIONS_CollectionChanged;
            SUGGESTIONS_ACV.VectorChanged += SUGGESTIONS_ACV_VectorChanged;
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
                UpdateSelectedItme();
            }
        }

        #endregion

        #region --Misc Methods (Private)--
        private void UpdateSelectedItme()
        {
            IEnumerable<object> result = SUGGESTIONS_ACV.Where((x) => x is ChatDataTemplate chat && string.Equals(chat.Chat.chatJabberId, filterText));
            SelectedItem = result.Count() > 0 ? result.First() as ChatDataTemplate : null;
        }

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
        private void SUGGESTIONS_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            HasSuggestions = SUGGESTIONS.Count > 0;
        }

        private void SUGGESTIONS_ACV_VectorChanged(IObservableVector<object> sender, IVectorChangedEventArgs args)
        {
            HasFilteredSuggestions = sender.Count > 0;
        }

        #endregion
    }
}
