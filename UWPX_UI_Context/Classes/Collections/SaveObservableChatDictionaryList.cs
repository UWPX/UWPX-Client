using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using Shared.Classes;
using Storage.Classes.Models.Chat;
using UWPX_UI_Context.Classes.DataTemplates;

namespace UWPX_UI_Context.Classes.Collections
{
    public class SaveObservableChatDictionaryList: ICollection, INotifyCollectionChanged, INotifyPropertyChanged, IDisposable, IList
    {
        public object this[int index]
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


        #endregion
        //--------------------------------------------------------Misc Methods:---------------------------------------------------------------\\
        #region --Misc Methods (Public)--
        public bool UpdateChat(ChatModel chat)
        {
            lock (SyncRoot)
            {
                return CHATS.UpdateChat(chat);
            }
        }

        public bool UpdateMUCInfo(MucInfoModel mucInfo)
        {
            lock (SyncRoot)
            {
                return CHATS.UpdateMUCInfo(mucInfo);
            }
        }

        public int Add(object value)
        {
            lock (SyncRoot)
            {
                return CHATS.Add(value);
            }
        }

        public void AddRange(IList<ChatDataTemplate> list, bool callCollectionReset)
        {
            lock (SyncRoot)
            {
                CHATS.AddRange(list, callCollectionReset);
            }
        }

        public void Clear()
        {
            lock (SyncRoot)
            {
                CHATS.Clear();
            }
        }

        public bool Contains(object value)
        {
            lock (SyncRoot)
            {
                return CHATS.Contains(value);
            }
        }

        public void CopyTo(Array array, int index)
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

        public IEnumerator GetEnumerator()
        {
            lock (SyncRoot)
            {
                return CHATS.GetEnumerator();
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
                SharedUtils.CallDispatcherAsync(() => PropertyChanged?.Invoke(sender, e)).Wait();
            }
        }

        private void OnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            lock (SyncRoot)
            {
                SharedUtils.CallDispatcherAsync(() => CollectionChanged?.Invoke(sender, e)).Wait();
            }
        }

        #endregion
    }
}
