using System.Collections.Generic;
using UWP_XMPP_Client.Classes;
using UWP_XMPP_Client.Classes.Events;
using UWP_XMPP_Client.DataTemplates;
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

        private CustomObservableCollection<MultiSelectTemplate> items;
        private List<object> selectedItems;

        public delegate void SelectionChangedMultiEventHandler(MultiSelectControl sender, SelectionChangedMultiEventArgs args);
        public event SelectionChangedMultiEventHandler SelectionChanged;

        private static object selectedItemsLock = new object();

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
            this.items = new CustomObservableCollection<MultiSelectTemplate>();
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
            lock (selectedItemsLock)
            {
                selectedItems.Clear();
                selectedItems.AddRange(list);
            }
        }

        public void setItems(List<object> list)
        {
            items.Clear();
            List<MultiSelectTemplate> templateList = new List<MultiSelectTemplate>();
            for (int i = 0; i < list.Count; i++)
            {
                templateList.Add(new MultiSelectTemplate()
                {
                    isSelected = false,
                    item = list[i]
                });
            }
            items.AddRange(templateList);
        }

        #endregion
        //--------------------------------------------------------Misc Methods:---------------------------------------------------------------\\
        #region --Misc Methods (Public)--


        #endregion

        #region --Misc Methods (Private)--
        private void updateSelectedItems()
        {
            lock (selectedItemsLock)
            {
                selectedItems.Clear();
                for (int i = 0; i < items.Count; i++)
                {
                    if (items[i].isSelected)
                    {
                        selectedItems.Add(items[i].item);
                    }
                }
            }
            SelectionChanged?.Invoke(this, new SelectionChangedMultiEventArgs(selectedItems));
        }

        private void showSelectedItems()
        {
            lock (selectedItemsLock)
            {
                for (int i = 0; i < selectedItems.Count; i++)
                {
                    for (int e = 0; e < items.Count; e++)
                    {
                        if (Equals(selectedItems[i], items[e].item))
                        {
                            items[e].isSelected = true;
                        }
                    }
                }
            }
        }

        #endregion

        #region --Misc Methods (Protected)--


        #endregion
        //--------------------------------------------------------Events:---------------------------------------------------------------------\\
        #region --Events--
        private void items_listv_DataContextChanged(FrameworkElement sender, DataContextChangedEventArgs args)
        {
            showSelectedItems();
        }

        private void CheckBox_CheckedChanged(object sender, RoutedEventArgs e)
        {
            updateSelectedItems();
        }

        #endregion
    }
}
