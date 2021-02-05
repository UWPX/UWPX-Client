using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using Storage.Classes.Models.Chat;

namespace Manager.Classes.Chat
{
    public class ObservableChatDictionaryList: ICollection<ChatDataTemplate>, INotifyCollectionChanged, INotifyPropertyChanged, IDisposable, IList<ChatDataTemplate>
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        /// <summary>
        /// Dictionary mapping <see cref="ChatModel.id"/> to a <see cref="ChatDataTemplate"/>.
        /// </summary>
        private readonly Dictionary<int, ChatDataTemplate> DICTIONARY;
        private readonly List<ChatDataTemplate> LIST;

        public int Count => LIST.Count;
        public bool IsSynchronized => false;
        public object SyncRoot => false;
        public bool IsFixedSize => false;
        public bool IsReadOnly => false;
        public ChatDataTemplate this[int index]
        {
            get => LIST[index];
            set
            {
                ChatDataTemplate oldChat = LIST[index];
                oldChat.PropertyChanged -= Item_PropertyChanged;
                LIST[index] = (ChatDataTemplate)value;
                LIST[index].PropertyChanged += Item_PropertyChanged;
                LIST[index].Index = index;
                OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Replace, oldChat, value, index));
            }
        }

        public event NotifyCollectionChangedEventHandler CollectionChanged;
        public event PropertyChangedEventHandler PropertyChanged;

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        public ObservableChatDictionaryList()
        {
            DICTIONARY = new Dictionary<int, ChatDataTemplate>();
            LIST = new List<ChatDataTemplate>();
        }

        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--
        IEnumerator<ChatDataTemplate> IEnumerable<ChatDataTemplate>.GetEnumerator()
        {
            return LIST.GetEnumerator();
        }

        public IEnumerator GetEnumerator()
        {
            return LIST.GetEnumerator();
        }

        #endregion
        //--------------------------------------------------------Misc Methods:---------------------------------------------------------------\\
        #region --Misc Methods (Public)--
        /// <summary>
        /// Based on: https://stackoverflow.com/questions/670577/observablecollection-doesnt-support-addrange-method-so-i-get-notified-for-each/45364074#45364074
        /// </summary>
        /// <param name="collection">The collection of items that should get added.</param>
        /// <param name="callCollectionReset">Should we call "collection changed" only once at the end?</param>
        public void AddRange(IEnumerable<ChatDataTemplate> collection, bool callCollectionReset)
        {
            if (collection is null)
            {
                throw new ArgumentNullException(nameof(collection));
            }

            if (collection.Count() <= 0)
            {
                return;
            }

            foreach (ChatDataTemplate i in collection)
            {
                int index = InternalAdd(i);
                if (!callCollectionReset)
                {
                    OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, i, index));
                }
            }
            if (callCollectionReset)
            {
                OnCollectionReset();
            }
        }

        public void CopyTo(ChatDataTemplate[] array, int arrayIndex)
        {
            foreach (ChatDataTemplate item in LIST)
            {
                array.SetValue(item, arrayIndex);
            }
        }

        public void Dispose()
        {
            foreach (ChatDataTemplate item in LIST)
            {
                item.PropertyChanged -= Item_PropertyChanged;
            }
        }

        void ICollection<ChatDataTemplate>.Add(ChatDataTemplate item)
        {
            Add(item);
        }

        public void Add(ChatDataTemplate item)
        {
            int index = InternalAdd(item);
            OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, item, index));
        }

        public void Clear()
        {
            DICTIONARY.Clear();
            foreach (ChatDataTemplate item in LIST)
            {
                item.PropertyChanged -= Item_PropertyChanged;
            }
            LIST.Clear();
            OnCollectionReset();
        }

        public bool Contains(ChatDataTemplate value)
        {
            return LIST.Contains(value);
        }

        public bool Contains(int id)
        {
            lock (SyncRoot)
            {
                return DICTIONARY.ContainsKey(id);
            }
        }

        public int IndexOf(ChatDataTemplate value)
        {
            ChatDataTemplate item = (ChatDataTemplate)value;
            if (!(item is null) && DICTIONARY.ContainsKey(item.Chat.id))
            {
                return DICTIONARY[item.Chat.id].Index;
            }
            return -1;
        }

        public void Insert(int index, ChatDataTemplate value)
        {
            if (!DICTIONARY.ContainsKey(value.Chat.id))
            {
                LIST.Insert(index, value);
                UpdateIndexes(index);
                DICTIONARY.Add(value.Chat.id, value);
                OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, value, index));
            }
        }

        bool ICollection<ChatDataTemplate>.Remove(ChatDataTemplate item)
        {
            return Remove(item);
        }

        public bool Remove(ChatDataTemplate item)
        {
            return RemoveId(item.Chat.id);
        }

        public bool RemoveId(int id)
        {
            if (DICTIONARY.ContainsKey(id))
            {
                ChatDataTemplate item = DICTIONARY[id];
                InternalRemoveAt(LIST.IndexOf(item), item);
                return true;
            }
            return false;
        }

        public void RemoveAt(int index)
        {
            InternalRemoveAt(index, LIST[index]);
        }

        #endregion

        #region --Misc Methods (Private)--
        private void OnCollectionReset()
        {
            OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        }

        private void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
        {
            CollectionChanged?.Invoke(this, e);
        }

        private int InternalAdd(ChatDataTemplate item)
        {
            if (!DICTIONARY.ContainsKey(item.Chat.id))
            {
                item.PropertyChanged += Item_PropertyChanged;
                item.Index = LIST.Count;
                LIST.Add(item);
                DICTIONARY.Add(item.Chat.id, item);
                return item.Index;
            }
            return IndexOf(item);
        }

        private void UpdateIndexes(int startIndex)
        {
            for (int i = startIndex; i < LIST.Count; i++)
            {
                LIST[i].Index = i;
            }
        }

        private void InternalRemoveAt(int index, ChatDataTemplate item)
        {
            item.PropertyChanged -= Item_PropertyChanged;
            LIST.RemoveAt(index);
            UpdateIndexes(index);
            DICTIONARY.Remove(item.Chat.id);
            OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, item, index));
        }

        #endregion

        #region --Misc Methods (Protected)--


        #endregion
        //--------------------------------------------------------Events:---------------------------------------------------------------------\\
        #region --Events--
        private void Item_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            PropertyChanged?.Invoke(this, e);
        }

        #endregion
    }
}
