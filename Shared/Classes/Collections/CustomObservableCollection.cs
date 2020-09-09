using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;

namespace Shared.Classes.Collections
{
    public interface IObservableList<T>: IList<T>, IList, INotifyCollectionChanged, INotifyPropertyChanged { }
    public class CustomObservableCollection<T>: IObservableList<T>
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        private readonly List<T> LIST = new List<T>();
        private bool deferNotifyCollectionChanged = false;

        private const string COUNT_NAME = nameof(List<T>.Count);
        private const string INDEXER_NAME = "Item[]";
        public readonly bool INVOKE_IN_UI_THREAD;

        public int Count => LIST.Count;
        public bool IsReadOnly => false;

        public bool IsFixedSize => IsReadOnly;

        public bool IsSynchronized => false;

        public object SyncRoot => throw new NotImplementedException();

        object IList.this[int index]
        {
            get => this[index];
            set
            {
                try
                {
                    this[index] = (T)value;
                }
                catch (InvalidCastException)
                {
                    throw new ArgumentException("Unable to add the given value.", nameof(value));
                }
            }
        }

        public event NotifyCollectionChangedEventHandler CollectionChanged;
        public event PropertyChangedEventHandler PropertyChanged;

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        public CustomObservableCollection(bool invokeInUiThread) : base()
        {
            INVOKE_IN_UI_THREAD = invokeInUiThread;
        }

        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--
        public T this[int index]
        {
            get => LIST[index];
            set => SetItem(index, value);
        }

        public IEnumerator<T> GetEnumerator()
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
        /// <summary>
        /// Adds the elements of the specified collection to the end of the ObservableCollection.
        /// </summary>
        /// <param name="collection">A collection of items to add.</param>
        public void AddRange(IEnumerable<T> collection)
        {
            deferNotifyCollectionChanged = true;
            foreach (T item in collection)
            {
                Add(item);
            }
            deferNotifyCollectionChanged = false;
            NotifyProperties();
            // Broken. Throws an invalid index exception
            //OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, collection is IList ? collection : collection.ToList()));
            OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset)); // Workaround
        }

        /// <summary>
        /// Adds the elements of the specified collection at the specific index of the ObservableCollection.
        /// </summary>
        /// <param name="index">The index all items should be added to.</param>
        /// <param name="collection">A collection of items to add.</param>
        public void InsertRange(int index, IEnumerable<T> collection)
        {
            deferNotifyCollectionChanged = true;
            foreach (T item in collection)
            {
                Insert(index, item);
            }
            deferNotifyCollectionChanged = false;
            NotifyProperties();
            // Broken. Throws an invalid index exception
            // OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, collection is IList ? collection : collection.ToList()));
            OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset)); // Workaround
        }

        public void Insert(int index, T item)
        {
            if (item is INotifyPropertyChanged i)
            {
                i.PropertyChanged += CustomObservableCollection_PropertyChanged;
            }
            LIST.Insert(index, item);
            OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, item, index));
        }

        public void RemoveAt(int index)
        {
            if (this[index] is INotifyPropertyChanged i)
            {
                i.PropertyChanged -= CustomObservableCollection_PropertyChanged;
            }
            T oldItem = this[index];
            LIST.RemoveAt(index);
            OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, oldItem, index));
        }

        public void Add(T item)
        {
            if (item is INotifyPropertyChanged i)
            {
                i.PropertyChanged += CustomObservableCollection_PropertyChanged;
            }
            LIST.Add(item);
            OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, item, Count - 1));
        }

        public void Clear()
        {
            foreach (T item in this)
            {
                if (item is INotifyPropertyChanged i)
                {
                    i.PropertyChanged -= CustomObservableCollection_PropertyChanged;
                }
            }
            LIST.Clear();
            NotifyProperties();
            OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset)); // Perhaps replace with Remove
        }

        public bool Remove(T item)
        {
            if (item is INotifyPropertyChanged i)
            {
                i.PropertyChanged -= CustomObservableCollection_PropertyChanged;
            }
            if (LIST.Remove(item))
            {
                OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, item));
                return true;
            }
            return false;
        }

        public int IndexOf(T item)
        {
            return LIST.IndexOf(item);
        }

        public bool Contains(T item)
        {
            return LIST.Contains(item);
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            LIST.CopyTo(array, arrayIndex);
        }

        int IList.Add(object value)
        {
            try
            {
                Add((T)value);
            }
            catch (InvalidCastException)
            {
                throw new ArgumentException("Unable to add the given value.", nameof(value));
            }

            return LIST.Count - 1;
        }

        bool IList.Contains(object value)
        {
            return value is T item && Contains(item);
        }

        int IList.IndexOf(object value)
        {
            return value is T item ? IndexOf(item) : -1;
        }

        void IList.Insert(int index, object value)
        {
            try
            {
                Insert(index, (T)value);
            }
            catch (InvalidCastException)
            {
                throw new ArgumentException("Unable to insert the given value.", nameof(value));
            }
        }

        void IList.Remove(object value)
        {
            try
            {
                Remove((T)value);
            }
            catch (InvalidCastException)
            {
                throw new ArgumentException("Unable to remove the given value.", nameof(value));
            }
        }

        public void CopyTo(Array array, int index)
        {
            if (array is T[] tArray)
            {
                LIST.CopyTo(tArray, index);
            }
            else
            {
                throw new ArgumentException("Unable to copy to array.", nameof(array));
            }
        }

        #endregion

        #region --Misc Methods (Private)--
        private void NotifyProperties(bool count = true)
        {
            if (count)
            {
                OnPropertyChanged(new PropertyChangedEventArgs(COUNT_NAME));
            }
            OnPropertyChanged(new PropertyChangedEventArgs(INDEXER_NAME));
        }

        #endregion

        #region --Misc Methods (Protected)--
        protected void SetItem(int index, T item)
        {
            if (this[index] is INotifyPropertyChanged i)
            {
                i.PropertyChanged -= CustomObservableCollection_PropertyChanged;
            }
            if (item is INotifyPropertyChanged iNew)
            {
                iNew.PropertyChanged += CustomObservableCollection_PropertyChanged;
            }
            T oldItem = LIST[index];
            LIST[index] = item;
            OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Replace, oldItem, item, index));
        }

        protected async void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
        {
            if (deferNotifyCollectionChanged)
            {
                return;
            }

            if (INVOKE_IN_UI_THREAD)
            {
                await SharedUtils.CallDispatcherAsync(() => CollectionChanged?.Invoke(this, e));
            }
            else
            {
                CollectionChanged?.Invoke(this, e);
            }
        }

        protected async void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            if (INVOKE_IN_UI_THREAD)
            {
                await SharedUtils.CallDispatcherAsync(() => PropertyChanged?.Invoke(this, e));
            }
            else
            {
                PropertyChanged?.Invoke(this, e);
            }
        }

        #endregion
        //--------------------------------------------------------Events:---------------------------------------------------------------------\\
        #region --Events--
        private void CustomObservableCollection_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            OnPropertyChanged(e);
        }

        #endregion
    }
}
