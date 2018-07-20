using Data_Manager2.Classes.DBTables;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using UWP_XMPP_Client.DataTemplates;

namespace Thread_Save_Components.Classes.Collections
{
    public class ObservableOrderedDictionary : ICollection<ChatTemplate>, INotifyCollectionChanged, INotifyPropertyChanged, IDisposable
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        private const string CountName = nameof(Count);
        private const string IndexerName = "Item[]";

        public int Count => LIST.Count;
        public bool IsReadOnly => false;

        private readonly Dictionary<string, LinkedListNode<ChatTemplate>> DICTIONARY;
        private readonly LinkedList<ChatTemplate> LIST;

        public event NotifyCollectionChangedEventHandler CollectionChanged;
        public event PropertyChangedEventHandler PropertyChanged;

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        /// <summary>
        /// Basic Constructor
        /// </summary>
        /// <history>
        /// 20/07/2018 Created [Fabian Sauter]
        /// </history>
        public ObservableOrderedDictionary()
        {
            this.DICTIONARY = new Dictionary<string, LinkedListNode<ChatTemplate>>();
            this.LIST = new LinkedList<ChatTemplate>();
        }

        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--
        public IEnumerator<ChatTemplate> GetEnumerator()
        {
            return LIST.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return LIST.GetEnumerator();
        }

        #endregion
        //--------------------------------------------------------Misc Methods:---------------------------------------------------------------\\
        #region --Misc Methods (Public)--
        public void Add(ChatTemplate item)
        {
            internalAdd(item);
            OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, item));
            NotifyProperties(true);
        }

        public bool UpdateChat(ChatTable chat)
        {
            if (!DICTIONARY.ContainsKey(chat.id))
            {
                return false;
            }
            else
            {
                LinkedListNode<ChatTemplate> node = DICTIONARY[chat.id];
                LinkedListNode<ChatTemplate> cur = node;
                int i = cur.Value.chat.lastActive.CompareTo(chat.lastActive); // Sorted ascending
                cur.Value.chat = chat;
                return true;
            }
        }

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
                internalAdd(i);
            }

            NotifyProperties();
            OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, list as IList ?? list.ToList()));
        }

        public void Clear()
        {
            DICTIONARY.Clear();
            LIST.Clear();
            OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        }

        public bool Contains(ChatTemplate item)
        {
            return DICTIONARY.ContainsKey(item.chat.id);
        }

        public void CopyTo(ChatTemplate[] array, int arrayIndex)
        {
            LIST.CopyTo(array, arrayIndex);
        }

        public bool Remove(string id)
        {
            if (DICTIONARY.ContainsKey(id))
            {
                ChatTemplate item = DICTIONARY[id].Value;
                item.PropertyChanged -= Item_PropertyChanged;
                LIST.Remove(DICTIONARY[id]);
                DICTIONARY.Remove(id);
                OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, item));
                NotifyProperties(true);
                return true;
            }
            return false;
        }

        public bool Remove(ChatTemplate item)
        {
            return Remove(item.chat.id);
        }

        public void Dispose()
        {
            foreach (var item in LIST)
            {
                item.PropertyChanged -= Item_PropertyChanged;
            }
        }

        #endregion

        #region --Misc Methods (Private)--
        private void internalAdd(ChatTemplate item)
        {
            if (!DICTIONARY.ContainsKey(item.chat.id))
            {
                item.PropertyChanged += Item_PropertyChanged;
                LinkedListNode<ChatTemplate> node = AddSortedToList(item);
                DICTIONARY.Add(item.chat.id, node);
            }
        }

        private void NotifyProperties(bool count = true)
        {
            if (count)
            {
                OnPropertyChanged(new PropertyChangedEventArgs(CountName));
            }
            OnPropertyChanged(new PropertyChangedEventArgs(IndexerName));
        }

        private void OnCollectionReset() => OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));

        private void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
        {
            CollectionChanged?.Invoke(this, e);
        }

        private void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            PropertyChanged?.Invoke(this, e);
        }

        #endregion

        #region --Misc Methods (Protected)--
        protected LinkedListNode<ChatTemplate> AddSortedToList(ChatTemplate item)
        {
            LinkedListNode<ChatTemplate> cur = LIST.First;
            while (cur != null)
            {
                if (cur.Value.chat.lastActive.CompareTo(item.chat.lastActive) >= 0)
                {
                    return LIST.AddBefore(cur, item);
                }
                cur = cur.Next;
            }
            return LIST.AddLast(item);
        }

        #endregion
        //--------------------------------------------------------Events:---------------------------------------------------------------------\\
        #region --Events--
        private void Item_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            OnPropertyChanged(e);
        }

        #endregion
    }
}
