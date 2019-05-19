using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Toolkit.Collections;

namespace Shared.Classes.Collections
{
    /// <summary>
    /// Based on: https://stackoverflow.com/questions/670577/observablecollection-doesnt-support-addrange-method-so-i-get-notified-for-each/45364074#45364074
    /// </summary>
    public class CustomObservableCollection<T>: ObservableCollection<T>, IIncrementalSource<T>
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        private const string COUNT_NAME = nameof(Count);
        private const string INDEXER_NAME = "Item[]";
        public readonly bool INVOKE_IN_UI_THREAD;
        public Func<int, int, CancellationToken, Task<IEnumerable<T>>> GetPagedItemsFuncAsync;

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
        public async Task<IEnumerable<T>> GetPagedItemsAsync(int pageIndex, int pageSize, CancellationToken cancellationToken = default)
        {
            return await GetPagedItemsFuncAsync(pageIndex, pageSize, cancellationToken);
        }

        #endregion
        //--------------------------------------------------------Misc Methods:---------------------------------------------------------------\\
        #region --Misc Methods (Public)--
        /// <summary>
        /// Adds the elements of the specified collection to the end of the ObservableCollection(Of T).
        /// </summary>
        public void AddRange(IEnumerable<T> collection)
        {
            if (collection is null)
            {
                throw new ArgumentNullException(nameof(collection));
            }

            if (collection is ICollection<T> list)
            {
                if (list.Count == 0)
                {
                    return;
                }
            }
            else if (!collection.Any())
            {
                return;
            }
            else
            {
                list = new List<T>(collection);
            }

            CheckReentrancy();

            int startIndex = Count;
            foreach (T i in collection)
            {
                Items.Add(i);
            }

            NotifyProperties();
            OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, list is IList ? list : list.ToList(), startIndex));
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

        private void OnCollectionReset()
        {
            OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        }

        #endregion

        #region --Misc Methods (Protected)--
        protected override void RemoveItem(int index)
        {
            if (Items[index] is INotifyPropertyChanged i)
            {
                i.PropertyChanged -= CustomObservableCollection_PropertyChanged;
            }
            base.RemoveItem(index);
        }

        protected override void InsertItem(int index, T item)
        {
            if (item is INotifyPropertyChanged i)
            {
                i.PropertyChanged += CustomObservableCollection_PropertyChanged;
            }
            base.InsertItem(index, item);
        }

        protected override void SetItem(int index, T item)
        {
            if (Items[index] is INotifyPropertyChanged i)
            {
                i.PropertyChanged -= CustomObservableCollection_PropertyChanged;
            }
            if (item is INotifyPropertyChanged iNew)
            {
                iNew.PropertyChanged += CustomObservableCollection_PropertyChanged;
            }
            (item as INotifyPropertyChanged).PropertyChanged += CustomObservableCollection_PropertyChanged;
            base.SetItem(index, item);
        }

        protected async override void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
        {
            if (INVOKE_IN_UI_THREAD)
            {
                await SharedUtils.CallDispatcherAsync(() => base.OnCollectionChanged(e));
            }
            else
            {
                base.OnCollectionChanged(e);
            }
        }

        protected async override void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            if (INVOKE_IN_UI_THREAD)
            {
                await SharedUtils.CallDispatcherAsync(() => base.OnPropertyChanged(e));
            }
            else
            {
                base.OnPropertyChanged(e);
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
