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
    class ObservableChatDictionaryList : ICollection, INotifyCollectionChanged, INotifyPropertyChanged, IDisposable, IList
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        private readonly Dictionary<string, ChatTemplate> DICTIONARY;
        private readonly List<ChatTemplate> LIST;

        public int Count => LIST.Count;
        public bool IsSynchronized => false;
        public object SyncRoot => false;
        public bool IsFixedSize => false;
        public bool IsReadOnly => false;
        object IList.this[int index]
        {
            get => LIST[index];
            set
            {
                ChatTemplate oldChat = LIST[index];
                oldChat.PropertyChanged -= Item_PropertyChanged;
                LIST[index] = (ChatTemplate)value;
                LIST[index].PropertyChanged += Item_PropertyChanged;
                LIST[index].index = index;
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
        public ObservableChatDictionaryList()
        {
            this.DICTIONARY = new Dictionary<string, ChatTemplate>();
            this.LIST = new List<ChatTemplate>();
        }

        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--
        public IEnumerator GetEnumerator()
        {
            return LIST.GetEnumerator();
        }

        #endregion
        //--------------------------------------------------------Misc Methods:---------------------------------------------------------------\\
        #region --Misc Methods (Public)--
        public bool UpdateChat(ChatTable chat)
        {
            if (DICTIONARY.ContainsKey(chat.id))
            {
                DICTIONARY[chat.id].chat = chat;
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
            foreach (var item in LIST)
            {
                array.SetValue(item, index);
            }
        }

        public void Dispose()
        {
            foreach (var item in LIST)
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
            foreach (var item in LIST)
            {
                item.PropertyChanged -= Item_PropertyChanged;
            }
            LIST.Clear();
            OnCollectionReset();
        }

        public bool Contains(object value)
        {
            return LIST.Contains(value);
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
                LIST.Insert(index, item);
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

        private int InternalAdd(ChatTemplate item)
        {
            if (!DICTIONARY.ContainsKey(item.chat.id))
            {
                item.PropertyChanged += Item_PropertyChanged;
                item.index = LIST.Count;
                LIST.Add(item);
                DICTIONARY.Add(item.chat.id, item);
                return item.index;
            }
            return IndexOf(item);
        }

        private void updateIndexes(int startIndex)
        {
            for (int i = startIndex; i < LIST.Count; i++)
            {
                LIST[i].index = i;
            }
        }

        private void InternalRemoveAt(int index, ChatTemplate item)
        {
            item.PropertyChanged -= Item_PropertyChanged;
            LIST.RemoveAt(index);
            updateIndexes(index);
            DICTIONARY.Remove(item.chat.id);
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
