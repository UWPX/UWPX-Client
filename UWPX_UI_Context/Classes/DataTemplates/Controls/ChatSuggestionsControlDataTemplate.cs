using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Manager.Classes;
using Manager.Classes.Chat;
using Microsoft.Toolkit.Uwp.UI;
using Shared.Classes;
using Shared.Classes.Collections;
using Shared.Classes.Threading;
using Windows.Foundation.Collections;

namespace UWPX_UI_Context.Classes.DataTemplates.Controls
{
    public sealed class ChatSuggestionsControlDataTemplate: AbstractDataTemplate
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        private bool _IsLoading;
        public bool IsLoading
        {
            get => _IsLoading;
            set => SetProperty(ref _IsLoading, value);
        }

        private ChatDataTemplate _SelectedItem;
        public ChatDataTemplate SelectedItem
        {
            get => _SelectedItem;
            set => SetProperty(ref _SelectedItem, value);
        }

        private bool _HasSuggestions;
        public bool HasSuggestions
        {
            get => _HasSuggestions;
            set => SetProperty(ref _HasSuggestions, value);
        }

        private bool _HasFilteredSuggestions;
        public bool HasFilteredSuggestions
        {
            get => _HasFilteredSuggestions;
            set => SetProperty(ref _HasFilteredSuggestions, value);
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
        public Task UpdateViewAsync(Client client)
        {
            return LoadSuggestionsAsync(client);
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
            IEnumerable<object> result = SUGGESTIONS_ACV.Where((x) => x is ChatDataTemplate chat && string.Equals(chat.Chat.bareJid, filterText));
            SelectedItem = result.Count() > 0 ? result.First() as ChatDataTemplate : null;
        }

        private async Task LoadSuggestionsAsync(Client client)
        {
            if (loadingTask is not null)
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
                List<ChatDataTemplate> chats;
                using (SemaLock semaLock = DataCache.INSTANCE.NewChatSemaLock())
                {
                    semaLock.Wait();
                    chats = DataCache.INSTANCE.CHATS.Where(c => !c.Chat.isChatActive && string.Equals(c.Client.dbAccount.bareJid, client.dbAccount.bareJid)).ToList();
                }
                SUGGESTIONS.Clear();
                SUGGESTIONS.AddRange(chats);
                SUGGESTIONS_ACV.RefreshFilter();
            });

            await loadingTask;
            IsLoading = false;
        }

        private bool AcvFilter(object o)
        {
            return o is ChatDataTemplate chat && (string.IsNullOrWhiteSpace(filterText) || chat.Chat.bareJid.Contains(filterText));
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
