using System.Collections.Specialized;
using System.ComponentModel;
using System.Threading.Tasks;
using Microsoft.Toolkit.Uwp.UI.Extensions;
using UWPX_UI_Context.Classes.DataContext.Controls;
using UWPX_UI_Context.Classes.DataTemplates;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace UWPX_UI.Controls.Chat
{
    public sealed partial class ChatMessageListControl: UserControl
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        public ChatDataTemplate Chat
        {
            get => (ChatDataTemplate)GetValue(ChatProperty);
            set => SetValue(ChatProperty, value);
        }
        public static readonly DependencyProperty ChatProperty = DependencyProperty.Register(nameof(ChatDataTemplate), typeof(ChatDataTemplate), typeof(ChatMessageListControl), new PropertyMetadata(null, ChatPropertyChanged));

        public bool IsDummy
        {
            get => (bool)GetValue(IsDummyProperty);
            set => SetValue(IsDummyProperty, value);
        }
        public static readonly DependencyProperty IsDummyProperty = DependencyProperty.Register(nameof(IsDummy), typeof(bool), typeof(ChatMessageListControl), new PropertyMetadata(false, OnIsDummyChanged));

        public double ScrollHeaderMinSize
        {
            get => (double)GetValue(ScrollHeaderMinSizeProperty);
            set => SetValue(ScrollHeaderMinSizeProperty, value);
        }
        public static readonly DependencyProperty ScrollHeaderMinSizeProperty = DependencyProperty.Register(nameof(ScrollHeaderMinSize), typeof(double), typeof(ChatMessageListControl), new PropertyMetadata(0d));

        private ScrollViewer scrollViewer;
        private ItemsStackPanel itemsStackPanel;

        public readonly ChatMessageListControlContext VIEW_MODEL = new ChatMessageListControlContext();

        /// <summary>
        /// Should be set to true by default to prevent loading to many messages at once.
        /// </summary>
        private bool scrolledToTheTop = true;

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        public ChatMessageListControl()
        {
            InitializeComponent();
        }

        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--


        #endregion
        //--------------------------------------------------------Misc Methods:---------------------------------------------------------------\\
        #region --Misc Methods (Public)--


        #endregion

        #region --Misc Methods (Private)--
        /// <summary>
        /// Updates the view and changes all properties.
        /// </summary>
        private void UpdateView(DependencyPropertyChangedEventArgs args)
        {
            ChatDataTemplate oldChat = args.OldValue is ChatDataTemplate ? args.OldValue as ChatDataTemplate : null;
            ChatDataTemplate newChat = args.NewValue is ChatDataTemplate ? args.NewValue as ChatDataTemplate : null;
            VIEW_MODEL.UpdateView(oldChat, newChat);
        }

        /// <summary>
        /// Returns the last <see cref="UIElement"/> from the <see cref="ListView"/> or null if there is none.
        /// </summary>
        private UIElement GetLastListUiElement()
        {
            if (mainListView.ItemsPanelRoot.Children.Count > 0)
            {
                return mainListView.ItemsPanelRoot.Children[mainListView.ItemsPanelRoot.Children.Count - 1];
            }
            return null;
        }

        /// <summary>
        /// Updates the scrollDown_grid.Visibility and itemsStackPanel.ItemsUpdatingScrollMode.
        /// Based on the VerticalOffset of the <see cref="ScrollViewer"/>.
        /// </summary>
        private void UpdateBehavior()
        {
            UIElement lastElem = GetLastListUiElement();
            if (lastElem is null)
            {
                scrollDown_grid.Visibility = Visibility.Collapsed;
                itemsStackPanel.ItemsUpdatingScrollMode = ItemsUpdatingScrollMode.KeepLastItemInView;
                return;
            }

            if (scrollViewer.VerticalOffset > (scrollViewer.ScrollableHeight - lastElem.ActualSize.Y))
            {
                scrollDown_grid.Visibility = Visibility.Collapsed;
                itemsStackPanel.ItemsUpdatingScrollMode = ItemsUpdatingScrollMode.KeepLastItemInView;
            }
            else
            {
                scrollDown_grid.Visibility = Visibility.Visible;
                itemsStackPanel.ItemsUpdatingScrollMode = ItemsUpdatingScrollMode.KeepScrollOffset;
            }
        }

        /// <summary>
        /// Tries to load more chat messages in case the user has scrolled to the top.
        /// </summary>
        private async Task TryIncrementalLoadingAsync()
        {
            if (scrollViewer.VerticalOffset < mainListViewHeader.ActualHeight + 10)
            {
                if (!scrolledToTheTop)
                {
                    scrolledToTheTop = true;
                    await LoadMoreMessagesAsync();
                }
            }
            else
            {
                scrolledToTheTop = false;
            }
        }

        /// <summary>
        /// Loads more chat messages in case there are more available and until the view port is full of messages.
        /// </summary>
        private async Task LoadMoreMessagesAsync()
        {
            if (VIEW_MODEL.MODEL.HasMoreMessages)
            {
                do
                {
                    await VIEW_MODEL.LoadMoreMessagesAsync();
                } while (VIEW_MODEL.MODEL.HasMoreMessages && scrollViewer.DesiredSize.Height < scrollViewer.ViewportHeight);
            }
        }
        #endregion

        #region --Misc Methods (Protected)--


        #endregion
        //--------------------------------------------------------Events:---------------------------------------------------------------------\\
        #region --Events--
        private static void ChatPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            if (d is ChatMessageListControl control)
            {
                control.UpdateView(args);
            }
        }

        private static void OnIsDummyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is ChatMessageListControl control)
            {
                control.VIEW_MODEL.MODEL.IsDummy = e.NewValue is bool b && b;
            }
        }

        private async void OnScrollViewerViewChanged(object sender, ScrollViewerViewChangedEventArgs e)
        {
            await TryIncrementalLoadingAsync();
            if (e.IsIntermediate)
            {
                UpdateBehavior();
            }
        }

        private async void MainListView_Loaded(object sender, RoutedEventArgs e)
        {
            itemsStackPanel = mainListView.FindDescendant<ItemsStackPanel>();
            scrollViewer = mainListView.FindDescendant<ScrollViewer>();
            scrollViewer.ViewChanged += OnScrollViewerViewChanged;
            VIEW_MODEL.MODEL.CHAT_MESSAGES.CollectionChanged += OnChatMessagesCollectionChanged;
            VIEW_MODEL.MODEL.ChatChanged += OnChatChanged;
            UpdateBehavior();
            await LoadMoreMessagesAsync();
        }

        private void scrollDown_btn_Click(IconButtonControl sender, RoutedEventArgs args)
        {
            if (mainListView.ItemsPanelRoot.Children.Count > 0)
            {
                scrollViewer.ChangeView(null, scrollViewer.ScrollableHeight, null);
            }
        }

        private void OnChatMessagesCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (VIEW_MODEL.MODEL.CHAT_MESSAGES.Count <= 0)
            {
                scrollDown_grid.Visibility = Visibility.Collapsed;
                itemsStackPanel.ItemsUpdatingScrollMode = ItemsUpdatingScrollMode.KeepLastItemInView;
                return;
            }
        }

        private async void OnChatChanged(object sender, PropertyChangedEventArgs e)
        {
            scrolledToTheTop = true;
            VIEW_MODEL.MODEL.CHAT_MESSAGES.Clear();
            await LoadMoreMessagesAsync();
        }
        #endregion
    }
}
