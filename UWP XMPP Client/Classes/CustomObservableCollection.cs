using System.Collections.ObjectModel;
using System.ComponentModel;

namespace UWP_XMPP_Client.Classes
{
    public class CustomObservableCollection<T> : ObservableCollection<T> where T : INotifyPropertyChanged

    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--


        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        public CustomObservableCollection()
        {

        }

        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--


        #endregion
        //--------------------------------------------------------Misc Methods:---------------------------------------------------------------\\
        #region --Misc Methods (Public)--
        protected override void RemoveItem(int index)
        {
            (Items[index] as INotifyPropertyChanged).PropertyChanged -= CustomObservableCollection_PropertyChanged;
            base.RemoveItem(index);
        }

        protected override void InsertItem(int index, T item)
        {
            (item as INotifyPropertyChanged).PropertyChanged += CustomObservableCollection_PropertyChanged;
            base.InsertItem(index, item);
        }

        protected override void SetItem(int index, T item)
        {
            (Items[index] as INotifyPropertyChanged).PropertyChanged -= CustomObservableCollection_PropertyChanged;
            (item as INotifyPropertyChanged).PropertyChanged += CustomObservableCollection_PropertyChanged;
            base.SetItem(index, item);
        }

        #endregion

        #region --Misc Methods (Private)--


        #endregion

        #region --Misc Methods (Protected)--


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
