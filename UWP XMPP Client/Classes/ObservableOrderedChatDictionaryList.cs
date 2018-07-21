using Data_Manager2.Classes.DBTables;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using UWP_XMPP_Client.DataTemplates;

namespace UWP_XMPP_Client.Classes
{
    class ObservableOrderedChatDictionaryList : ICollection, INotifyCollectionChanged, INotifyPropertyChanged, IDisposable, IList
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        private readonly Dictionary<string, ChatTemplate> DICTIONARY;
        private readonly List<ChatTemplate> SORTED_LIST;

        public int Count => SORTED_LIST.Count;
        public bool IsSynchronized => false;
        public object SyncRoot => false;
        public bool IsFixedSize => false;
        public bool IsReadOnly => false;
        object IList.this[int index]
        {
            get => SORTED_LIST[index];
            set
            {
                ChatTemplate oldChat = SORTED_LIST[index];
                oldChat.PropertyChanged -= Item_PropertyChanged;
                SORTED_LIST[index] = (ChatTemplate)value;
                SORTED_LIST[index].PropertyChanged += Item_PropertyChanged;
                OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Replace, oldChat, value));
            }
        }

        public event NotifyCollectionChangedEventHandler CollectionChanged;
        public event PropertyChangedEventHandler PropertyChanged;

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        /// <summary>
        /// Basic Constructor
        /// </summary>
        /// <history>
        /// 21/07/2018 Created [Fabian Sauter]
        /// </history>
        public ObservableOrderedChatDictionaryList()
        {
            this.DICTIONARY = new Dictionary<string, ChatTemplate>();
            this.SORTED_LIST = new List<ChatTemplate>();
        }

        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--
        public IEnumerator GetEnumerator()
        {
            return SORTED_LIST.GetEnumerator();
        }

        #endregion
        //--------------------------------------------------------Misc Methods:---------------------------------------------------------------\\
        #region --Misc Methods (Public)--
        public bool UpdateChat(ChatTable chat)
        {
            if (DICTIONARY.ContainsKey(chat.id))
            {
                ChatTemplate node = DICTIONARY[chat.id];
                ChatTemplate cur = node;
                int i = cur.chat.lastActive.CompareTo(chat.lastActive); // Sorted ascending
                cur.chat = chat;
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool UpdateMUCInfo(MUCChatInfoTable mucInfo)
        {
            if (DICTIONARY.ContainsKey(mucInfo.chatId))
            {
                ChatTemplate node = DICTIONARY[mucInfo.chatId];
                node.mucInfo = mucInfo;
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Based on: https://stackoverflow.com/questions/670577/observablecollection-doesnt-support-addrange-method-so-i-get-notified-for-each/45364074#45364074
        /// </summary>
        public void AddRange(IEnumerable<ChatTemplate> collection)
        {
            if (collection == null)
            {
                throw new ArgumentNullException(nameof(collection));
            }

            if (collection is ICollection<ChatTemplate> list)
            {
                if (list.Count == 0) return;
            }
            else if (!collection.Any())
            {
                return;
            }
            else
            {
                list = new List<ChatTemplate>(collection);
            }

            foreach (var i in collection)
            {
                InternalAdd(i);
            }
            OnCollectionReset();
        }

        public void CopyTo(Array array, int index)
        {
            int i = index;
            foreach (var item in DICTIONARY)
            {
                array.SetValue(item, index);
            }
        }

        public void Dispose()
        {
            foreach (var item in SORTED_LIST)
            {
                item.PropertyChanged -= Item_PropertyChanged;
            }
        }

        public int Add(object value)
        {
            int index = InternalAdd((ChatTemplate)value);
            OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, value));
            return index;
        }

        public void Clear()
        {
            DICTIONARY.Clear();
            SORTED_LIST.Clear();
            OnCollectionReset();
        }

        public bool Contains(object value)
        {
            return SORTED_LIST.Contains(value);
        }

        public int IndexOf(object value)
        {
            return SORTED_LIST.IndexOf((ChatTemplate)value);
        }

        public void Insert(int index, object value)
        {
            SORTED_LIST.Insert(index, (ChatTemplate)value);
        }

        public void Remove(object value)
        {
            SORTED_LIST.Remove((ChatTemplate)value);
        }

        public void RemoveAt(int index)
        {
            SORTED_LIST.RemoveAt(index);
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

        private int InternalAdd(ChatTemplate item)
        {
            if (!DICTIONARY.ContainsKey(item.chat.id))
            {
                item.PropertyChanged += Item_PropertyChanged;
                int index = InternalAddSortedToList(item);
                DICTIONARY.Add(item.chat.id, item);
                return index;
            }
            return IndexOf(item);
        }

        private int InternalAddSortedToList(ChatTemplate item)
        {
            for (int i = 0; i < SORTED_LIST.Count; i++)
            {
                if (SORTED_LIST[i].chat.lastActive.CompareTo(item.chat.lastActive) >= 0)
                {
                    SORTED_LIST.Insert(i, item);
                    return i;
                }
            }
            SORTED_LIST.Add(item);
            return SORTED_LIST.Count - 1;
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
