using Data_Manager2.Classes.DBTables;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using UWPX_UI_Context.Classes.DataTemplates;

namespace UWPX_UI_Context.Classes.Collections
{
    public class ObservableChatDictionaryList : ICollection, INotifyCollectionChanged, INotifyPropertyChanged, IDisposable, IList
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        private readonly Dictionary<string, ChatDataTemplate> DICTIONARY;
        private readonly List<ChatDataTemplate> LIST;

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
            this.DICTIONARY = new Dictionary<string, ChatDataTemplate>();
            this.LIST = new List<ChatDataTemplate>();
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
                DICTIONARY[chat.id].Chat = chat;
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
                ChatDataTemplate node = DICTIONARY[mucInfo.chatId];
                node.MucInfo = mucInfo;
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
        /// <param name="list">The list of items that should get added.</param>
        /// <param name="callCollectionReset">Should we call "collection changed" only once at the end?</param>
        public void AddRange(IList<ChatDataTemplate> list, bool callCollectionReset)
        {
            if (list is null)
            {
                throw new ArgumentNullException(nameof(list));
            }

            if (list.Count <= 0)
            {
                return;
            }

            foreach (var i in list)
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

        public void CopyTo(Array array, int index)
        {
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
            int index = InternalAdd((ChatDataTemplate)value);
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
            ChatDataTemplate item = (ChatDataTemplate)value;
            if (item != null && DICTIONARY.ContainsKey(item.Chat.id))
            {
                return DICTIONARY[item.Chat.id].Index;
            }
            return -1;
        }

        public void Insert(int index, object value)
        {
            ChatDataTemplate item = (ChatDataTemplate)value;
            if (!DICTIONARY.ContainsKey(item.Chat.id))
            {
                LIST.Insert(index, item);
                UpdateIndexes(index);
                DICTIONARY.Add(item.Chat.id, item);
                OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, item, index));
            }
        }

        public void Remove(object value)
        {
            RemoveId(((ChatDataTemplate)value).Chat.id);
        }

        public bool RemoveId(string id)
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
