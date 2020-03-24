using System.Collections.Generic;
using Shared.Classes.Collections;
using UWPX_UI.Classes.Events;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace UWPX_UI.Controls
{
    public sealed partial class MultiSelectControl: UserControl
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        public string Header
        {
            get => (string)GetValue(HeaderProperty);
            set => SetValue(HeaderProperty, value);
        }
        public static readonly DependencyProperty HeaderProperty = DependencyProperty.Register(nameof(Header), typeof(string), typeof(MultiSelectControl), null);

        public CustomObservableCollection<object> ItemSource
        {
            get => (CustomObservableCollection<object>)GetValue(ItemSourceProperty);
            set => SetValue(ItemSourceProperty, value);
        }
        public static readonly DependencyProperty ItemSourceProperty = DependencyProperty.Register(nameof(ItemSource), typeof(CustomObservableCollection<object>), typeof(MultiSelectControl), new PropertyMetadata(new CustomObservableCollection<object>(true)));

        private readonly List<object> SELECTED_ITEMS = new List<object>();

        public delegate void SelectionChangedMultiEventHandler(MultiSelectControl sender, MultiSelectChangedEventArgs args);
        public event SelectionChangedMultiEventHandler SelectionChanged;

        private bool ignoreSelectionChanged = false;

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        public MultiSelectControl()
        {
            InitializeComponent();
        }

        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--
        public void SetSelectedItems(List<object> selectedItems)
        {
            SELECTED_ITEMS.Clear();
            SELECTED_ITEMS.AddRange(selectedItems);
            ShowSelectedItems();
        }

        public List<object> GetSelectedItems()
        {
            return SELECTED_ITEMS;
        }

        #endregion
        //--------------------------------------------------------Misc Methods:---------------------------------------------------------------\\
        #region --Misc Methods (Public)--


        #endregion

        #region --Misc Methods (Private)--
        private void ShowSelectedItems()
        {
            ignoreSelectionChanged = true;
            items_listv.SelectedItems.Clear();
            foreach (object item in SELECTED_ITEMS)
            {
                items_listv.SelectedItems.Add(item);
            }
            ignoreSelectionChanged = false;
        }

        #endregion

        #region --Misc Methods (Protected)--


        #endregion
        //--------------------------------------------------------Events:---------------------------------------------------------------------\\
        #region --Events--
        private void Items_listv_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ignoreSelectionChanged)
            {
                return;
            }

            // Remove items:
            foreach (object item in e.RemovedItems)
            {
                SELECTED_ITEMS.Remove(item);
            }

            // Add items:
            foreach (object item in e.AddedItems)
            {
                SELECTED_ITEMS.Add(item);
            }

            // Trigger the event:
            SelectionChanged?.Invoke(this, new MultiSelectChangedEventArgs(SELECTED_ITEMS));
        }

        #endregion
    }
}
