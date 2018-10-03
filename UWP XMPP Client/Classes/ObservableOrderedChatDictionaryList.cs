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
                SORTED_LIST[index].index = index;
                SortItem(index);
                OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Replace, oldChat, value, index));
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
                ChatTemplate item = DICTIONARY[chat.id];
                int i = item.chat.lastActive.CompareTo(chat.lastActive);
                item.chat = chat;
                if (i != 0)
                {
                    SortItem(SORTED_LIST.IndexOf(item));
                }
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
            foreach (var item in SORTED_LIST)
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
            OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, value, index));
            return index;
        }

        public void Clear()
        {
            DICTIONARY.Clear();
            foreach (var item in SORTED_LIST)
            {
                item.PropertyChanged -= Item_PropertyChanged;
            }
            SORTED_LIST.Clear();
            OnCollectionReset();
        }

        public bool Contains(object value)
        {
            return SORTED_LIST.Contains(value);
        }

        public int IndexOf(object value)
        {
            ChatTemplate item = (ChatTemplate)value;
            if (item != null && DICTIONARY.ContainsKey(item.chat.id))
            {
                return DICTIONARY[item.chat.id].index;
            }
            return -1;
        }

        public void Insert(int index, object value)
        {
            ChatTemplate item = (ChatTemplate)value;
            if (!DICTIONARY.ContainsKey(item.chat.id))
            {
                SORTED_LIST.Insert(index, item);
                updateIndexes(index);
                DICTIONARY.Add(item.chat.id, item);
                OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, item, index));
            }
        }

        public void Remove(object value)
        {
            RemoveId(((ChatTemplate)value).chat.id);
        }

        public bool RemoveId(string id)
        {
            if (DICTIONARY.ContainsKey(id))
            {
                ChatTemplate item = DICTIONARY[id];
                InternalRemoveAt(SORTED_LIST.IndexOf(item), item);
                return true;
            }
            return false;
        }

        public void RemoveAt(int index)
        {
            InternalRemoveAt(index, SORTED_LIST[index]);
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
                int index = AddSortedToList(item);
                DICTIONARY.Add(item.chat.id, item);
                return index;
            }
            return IndexOf(item);
        }

        private int AddSortedToList(ChatTemplate item)
        {
            for (int i = 0; i < SORTED_LIST.Count; i++)
            {
                if (SORTED_LIST[i].chat.lastActive.CompareTo(item.chat.lastActive) <= 0)
                {
                    item.index = i;
                    SORTED_LIST.Insert(i, item);
                    updateIndexes(i);
                    return i;
                }
            }
            item.index = SORTED_LIST.Count;
            SORTED_LIST.Add(item);
            return item.index;
        }

        private void updateIndexes(int startIndex)
        {
            for (int i = startIndex; i < SORTED_LIST.Count; i++)
            {
                SORTED_LIST[i].index = i;
            }
        }

        private void InternalRemoveAt(int index, ChatTemplate item)
        {
            item.PropertyChanged -= Item_PropertyChanged;
            SORTED_LIST.RemoveAt(index);
            updateIndexes(index);
            DICTIONARY.Remove(item.chat.id);
            OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, item, index));
        }

        private void SortItem(int index)
        {
            if (index > 0 && index < (SORTED_LIST.Count - 1))
            {
                int leftComp = SORTED_LIST[index - 1].chat.lastActive.CompareTo(SORTED_LIST[index].chat.lastActive);
                int rightComp = SORTED_LIST[index + 1].chat.lastActive.CompareTo(SORTED_LIST[index].chat.lastActive);

                if (leftComp < 0)
                {
                    Exchange(index, index - 1);
                    SortItem(index - 1);
                }
                else if (rightComp > 0)
                {
                    Exchange(index, index + 1);
                    SortItem(index + 1);
                }
            }
        }

        private void Exchange(int index1, int index2)
        {
            ChatTemplate tmp = SORTED_LIST[index1];
            SORTED_LIST[index1] = SORTED_LIST[index2];
            SORTED_LIST[index2] = tmp;

            SORTED_LIST[index2].index = index2;
            SORTED_LIST[index1].index = index1;

            OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Move, SORTED_LIST[index1], index1, index2));
            OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Move, SORTED_LIST[index2], index2, index1));
        }

        #endregion

        #region --Misc Methods (Protected)--


        #endregion
        //--------------------------------------------------------Events:---------------------------------------------------------------------\\
        #region --Events--
        private void Item_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            PropertyChanged?.Invoke(this, e);
            SortItem(((ChatTemplate)sender).index);
        }

        #endregion
    }
}
