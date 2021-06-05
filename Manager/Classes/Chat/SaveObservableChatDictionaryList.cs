using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;

namespace Manager.Classes.Chat
{
    public class SaveObservableChatDictionaryList: ICollection<ChatDataTemplate>, INotifyCollectionChanged, INotifyPropertyChanged, IDisposable, IList<ChatDataTemplate>, IList
    {
        public ChatDataTemplate this[int index]
        {
            get
            {
                lock (SyncRoot)
                {
                    return CHATS[index];
                }
            }
            set
            {
                lock (SyncRoot)
                {
                    CHATS[index] = value;
                }
            }
        }

        object IList.this[int index]
        {
            get => this[index];
            set => this[index] = (ChatDataTemplate)value;
        }

        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        private readonly ObservableChatDictionaryList CHATS = new ObservableChatDictionaryList();

        public int Count => CHATS.Count;
        public bool IsSynchronized => CHATS.IsSynchronized;
        public object SyncRoot => new object();
        public bool IsFixedSize => CHATS.IsFixedSize;
        public bool IsReadOnly => CHATS.IsReadOnly;

        public event NotifyCollectionChangedEventHandler CollectionChanged;
        public event PropertyChangedEventHandler PropertyChanged;
        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        public SaveObservableChatDictionaryList()
        {
            CHATS.CollectionChanged += OnCollectionChanged;
            CHATS.PropertyChanged += OnPropertyChanged;
        }

        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--
        IEnumerator<ChatDataTemplate> IEnumerable<ChatDataTemplate>.GetEnumerator()
        {
            lock (SyncRoot)
            {
                return (IEnumerator<ChatDataTemplate>)CHATS.GetEnumerator();
            }
        }

        public IEnumerator GetEnumerator()
        {
            lock (SyncRoot)
            {
                return CHATS.GetEnumerator();
            }
        }

        public ChatDataTemplate GetChat(int chatId)
        {
            lock (SyncRoot)
            {
                return CHATS.GetChat(chatId);
            }
        }

        #endregion
        //--------------------------------------------------------Misc Methods:---------------------------------------------------------------\\
        #region --Misc Methods (Public)--
        void ICollection<ChatDataTemplate>.Add(ChatDataTemplate item)
        {
            Add(item);
        }

        bool ICollection<ChatDataTemplate>.Remove(ChatDataTemplate item)
        {
            return Remove(item);
        }

        public void Add(ChatDataTemplate value)
        {
            lock (SyncRoot)
            {
                CHATS.Add(value);
            }
        }

        public void AddRange(IEnumerable<ChatDataTemplate> collection, bool callCollectionReset)
        {
            lock (SyncRoot)
            {
                CHATS.AddRange(collection, callCollectionReset);
            }
        }

        public void Clear()
        {
            lock (SyncRoot)
            {
                CHATS.Clear();
            }
        }

        public bool Contains(ChatDataTemplate value)
        {
            lock (SyncRoot)
            {
                return CHATS.Contains(value);
            }
        }

        public bool Contains(int id)
        {
            lock (SyncRoot)
            {
                return CHATS.Contains(id);
            }
        }

        public void CopyTo(ChatDataTemplate[] array, int index)
        {
            lock (SyncRoot)
            {
                CHATS.CopyTo(array, index);
            }
        }

        public void Dispose()
        {
            lock (SyncRoot)
            {
                CHATS.Dispose();
            }
        }

        public int IndexOf(ChatDataTemplate value)
        {
            lock (SyncRoot)
            {
                return CHATS.IndexOf(value);
            }
        }

        public void Insert(int index, ChatDataTemplate value)
        {
            lock (SyncRoot)
            {
                CHATS.Insert(index, value);
            }
        }

        public bool Remove(ChatDataTemplate value)
        {
            lock (SyncRoot)
            {
                return CHATS.Remove(value);
            }
        }

        public void RemoveAt(int index)
        {
            lock (SyncRoot)
            {
                CHATS.RemoveAt(index);
            }
        }

        public bool RemoveId(int id)
        {
            lock (SyncRoot)
            {
                return CHATS.RemoveId(id);
            }
        }

        public int Add(object value)
        {
            lock (SyncRoot)
            {
                return CHATS.Add(value);
            }
        }

        public bool Contains(object value)
        {
            lock (SyncRoot)
            {
                return CHATS.Contains(value);
            }
        }

        public int IndexOf(object value)
        {
            lock (SyncRoot)
            {
                return CHATS.IndexOf(value);
            }
        }

        public void Insert(int index, object value)
        {
            lock (SyncRoot)
            {
                CHATS.Insert(index, value);
            }
        }

        public void Remove(object value)
        {
            lock (SyncRoot)
            {
                CHATS.Remove(value);
            }
        }

        public void CopyTo(Array array, int index)
        {
            lock (SyncRoot)
            {
                CHATS.CopyTo(array, index);
            }
        }

        public void Sort()
        {
            lock (SyncRoot)
            {
                CHATS.Sort();
            }
        }

        public List<ChatDataTemplate> ToList()
        {
            lock (SyncRoot)
            {
                return CHATS.ToList();
            }
        }

        #endregion

        #region --Misc Methods (Private)--


        #endregion

        #region --Misc Methods (Protected)--


        #endregion
        //--------------------------------------------------------Events:---------------------------------------------------------------------\\
        #region --Events--
        private void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            lock (SyncRoot)
            {
                PropertyChanged?.Invoke(sender, e);
            }
        }

        private void OnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            lock (SyncRoot)
            {
                CollectionChanged?.Invoke(sender, e);
            }
        }

        #endregion
    }
}
