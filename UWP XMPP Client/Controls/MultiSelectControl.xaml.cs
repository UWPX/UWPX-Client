using System.Collections.Generic;
using UWP_XMPP_Client.Classes.Events;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace UWP_XMPP_Client.Controls
{
    public sealed partial class MultiSelectControl : UserControl
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        public string header
        {
            get { return (string)GetValue(headerProperty); }
            set { SetValue(headerProperty, value); }
        }
        public static readonly DependencyProperty headerProperty = DependencyProperty.Register("header", typeof(string), typeof(MultiSelectControl), null);

        public int maxContentHeight
        {
            get { return (int)GetValue(maxContentHeightProperty); }
            set { SetValue(maxContentHeightProperty, value); }
        }
        public static readonly DependencyProperty maxContentHeightProperty = DependencyProperty.Register("maxContentHeight", typeof(int), typeof(MultiSelectControl), null);

        public List<object> itemSource
        {
            get { return (List<object>)GetValue(itemSourceProperty); }
            set { SetValue(itemSourceProperty, value); }
        }
        public static readonly DependencyProperty itemSourceProperty = DependencyProperty.Register("itemSource", typeof(List<object>), typeof(MultiSelectControl), null);

        private List<object> selectedItems;

        public delegate void SelectionChangedMultiEventHandler(MultiSelectControl sender, SelectionChangedMultiEventArgs args);
        public event SelectionChangedMultiEventHandler SelectionChanged;

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        /// <summary>
        /// Basic Constructor
        /// </summary>
        /// <history>
        /// 02/03/2018 Created [Fabian Sauter]
        /// </history>
        public MultiSelectControl()
        {
            this.MaxHeight = 200;
            this.selectedItems = new List<object>();
            this.InitializeComponent();
        }

        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--
        public IList<object> getSelectedItems()
        {
            return selectedItems;
        }

        public void setSelectedItems(List<object> list)
        {
            selectedItems.Clear();
            selectedItems.AddRange(list);
        }

        #endregion
        //--------------------------------------------------------Misc Methods:---------------------------------------------------------------\\
        #region --Misc Methods (Public)--


        #endregion

        #region --Misc Methods (Private)--
        private void showSelectedItems()
        {
            items_listv.SelectedItems.Clear();
            for (int i = 0; i < selectedItems.Count; i++)
            {
                items_listv.SelectedItems.Add(selectedItems[i]);
            }
        }

        #endregion

        #region --Misc Methods (Protected)--


        #endregion
        //--------------------------------------------------------Events:---------------------------------------------------------------------\\
        #region --Events--
        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            showSelectedItems();
        }

        private void items_listv_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Remove items:
            for (int i = 0; i < e.RemovedItems.Count; i++)
            {
                selectedItems.Remove(e.RemovedItems[i]);
            }

            // Add items:
            for (int i = 0; i < e.AddedItems.Count; i++)
            {
                if (!selectedItems.Contains(e.AddedItems[i]))
                {
                    selectedItems.Add(e.AddedItems[i]);
                }
            }

            // Trigger the event:
            SelectionChanged?.Invoke(this, new SelectionChangedMultiEventArgs(selectedItems));
        }

        #endregion
    }
}
