using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;

namespace UWP_XMPP_Client.Classes
{
    /// <summary>
    /// Based on: https://stackoverflow.com/questions/670577/observablecollection-doesnt-support-addrange-method-so-i-get-notified-for-each/45364074#45364074
    /// </summary>
    public class CustomObservableCollection<T> : ObservableCollection<T>
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        private const string CountName = nameof(Count);
        private const string IndexerName = "Item[]";

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        /// <summary>
        /// Initializes a new instance of the System.Collections.ObjectModel.ObservableCollection(Of T) class.
        /// </summary>
        public CustomObservableCollection() : base()
        {
        }

        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--
        

        #endregion
        //--------------------------------------------------------Misc Methods:---------------------------------------------------------------\\
        #region --Misc Methods (Public)--
        /// <summary>
        /// Adds the elements of the specified collection to the end of the ObservableCollection(Of T).
        /// </summary>
        public void AddRange(IEnumerable<T> collection)
        {
            if (collection == null)
            {
                throw new ArgumentNullException(nameof(collection));
            }

            if (collection is ICollection<T> list)
            {
                if (list.Count == 0) return;
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
            foreach (var i in collection)
            {
                Items.Add(i);
            }

            NotifyProperties();
            OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, list as IList ?? list.ToList(), startIndex));
        }

        #endregion

        #region --Misc Methods (Private)--
        private void NotifyProperties(bool count = true)
        {
            if (count)
            {
                OnPropertyChanged(new PropertyChangedEventArgs(CountName));
            }
            OnPropertyChanged(new PropertyChangedEventArgs(IndexerName));
        }

        private void OnCollectionReset() => OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));

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
