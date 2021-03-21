using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using Manager.Classes.Chat;
using Shared.Classes.Collections;
using Storage.Classes.Models.Chat;

namespace UWPX_UI_Context.Classes.Collections
{
    public class AdvancedChatsCollection: CustomObservableCollection<ChatDataTemplate>
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        private readonly SaveObservableChatDictionaryList CHATS;

        public Predicate<ChatDataTemplate> filter;

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        public AdvancedChatsCollection(SaveObservableChatDictionaryList chats) : base(true)
        {
            CHATS = chats;
            CHATS.CollectionChanged += OnChatsCollectionChanged;
            CHATS.PropertyChanged += OnChatsPropertyChanged;

            foreach (ChatDataTemplate chat in CHATS)
            {
                InsertSorted(chat);
            }
        }

        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--


        #endregion
        //--------------------------------------------------------Misc Methods:---------------------------------------------------------------\\
        #region --Misc Methods (Public)--
        public void Filter()
        {
            List<ChatDataTemplate> removed = new List<ChatDataTemplate>();
            foreach (ChatDataTemplate chat in this)
            {
                if (!(filter is null) && (!chat.Chat.isChatActive || !filter(chat)))
                {
                    removed.Add(chat);
                }
            }

            List<ChatDataTemplate> added = new List<ChatDataTemplate>();
            foreach (ChatDataTemplate chat in CHATS)
            {
                if (!Contains(chat) && chat.Chat.isChatActive && (filter is null || filter(chat)))
                {
                    added.Add(chat);
                }
            }

            foreach (ChatDataTemplate chat in removed)
            {
                Remove(chat);
            }

            foreach (ChatDataTemplate chat in added)
            {
                InsertSorted(chat);
            }
        }

        public void Sort()
        {

        }

        #endregion

        #region --Misc Methods (Private)--
        private int InsertSorted(ChatDataTemplate chat)
        {
            if (Count <= 0)
            {
                Add(chat);
                return 0;
            }
            if (this[Count - 1].CompareTo(chat) <= 0)
            {
                Add(chat);
                return Count - 1;
            }
            if (this[0].CompareTo(chat) >= 0)
            {
                Insert(0, chat);
                return 0;
            }
            int index = this.ToList().BinarySearch(chat);
            if (index < 0)
            {
                index = ~index;
            }

            Insert(index, chat);
            return index;
        }

        private void ReorderChat(ChatDataTemplate chat)
        {
            int oldIndex = IndexOf(chat);
            if (oldIndex < 0)
            {
                return;
            }

            deferNotifyCollectionChanged = true;
            RemoveAt(oldIndex);
            int newIndex = InsertSorted(chat);
            deferNotifyCollectionChanged = false;
            if (oldIndex != newIndex)
            {
                OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Move, chat, newIndex, oldIndex));
            }
        }

        #endregion

        #region --Misc Methods (Protected)--


        #endregion
        //--------------------------------------------------------Events:---------------------------------------------------------------------\\
        #region --Events--
        private void OnChatsPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (sender is ChatDataTemplate chat)
            {
                if (string.Equals(e.PropertyName, nameof(ChatDataTemplate.Chat) + '.' + nameof(ChatModel.lastActive)))
                {
                    ReorderChat(chat);
                }
                else
                {
                    int index = IndexOf(chat);
                    bool included = chat.Chat.isChatActive && (filter is null || filter(chat));
                    if (index >= 0)
                    {
                        if (!included)
                        {
                            RemoveAt(index);
                        }
                    }
                    else
                    {
                        if (included)
                        {
                            InsertSorted(chat);
                        }
                    }
                }
            }
        }

        private void OnChatsCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    foreach (ChatDataTemplate chat in e.NewItems)
                    {
                        if (chat.Chat.isChatActive && filter(chat))
                        {
                            InsertSorted(chat);
                        }
                    }
                    break;

                case NotifyCollectionChangedAction.Move:
                    throw new NotImplementedException();

                case NotifyCollectionChangedAction.Remove:
                    foreach (ChatDataTemplate chat in e.OldItems)
                    {
                        if (filter(chat) || !chat.Chat.isChatActive)
                        {
                            Remove(chat);
                        }
                    }
                    break;

                case NotifyCollectionChangedAction.Replace:
                    throw new NotImplementedException();

                case NotifyCollectionChangedAction.Reset:
                    Filter();
                    Sort();
                    break;
            }
        }

        #endregion
    }
}
