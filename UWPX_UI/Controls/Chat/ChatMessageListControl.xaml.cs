﻿using System.Collections.Specialized;
using System.Threading.Tasks;
using Manager.Classes.Chat;
using Microsoft.Toolkit.Uwp.UI;
using UWPX_UI_Context.Classes.DataContext.Controls;
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
        private bool loaded = false;

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
        private void TryIncrementalLoading()
        {
            if (scrollViewer.VerticalOffset <= 10)
            {
                if (!scrolledToTheTop && !VIEW_MODEL.MODEL.CHAT_MESSAGES.IsLoading)
                {
                    scrolledToTheTop = true;
                    if (!IsDummy)
                    {
                        _ = LoadMoreMessagesAsync();
                    }
                }
            }
            else
            {
                scrolledToTheTop = false;
            }
        }

        private void TryMarkAsRead()
        {
            // Distance from the bottom:
            double offsetFromBottom = scrollViewer.ScrollableHeight - scrollViewer.VerticalOffset;
            if (offsetFromBottom <= 10)
            {
                if (!IsDummy)
                {
                    Chat?.MarkAllAsRead();
                }
            }
        }

        /// <summary>
        /// Loads more chat messages in case there are more available and until the view port is full of messages.
        /// </summary>
        private async Task LoadMoreMessagesAsync()
        {
            if (!IsDummy && loaded && VIEW_MODEL.MODEL.CHAT_MESSAGES.HasMoreMessages)
            {
                if (scrolledToTheTop)
                {
                    await VIEW_MODEL.MODEL.CHAT_MESSAGES.LoadMoreMessagesAsync();
                }

                while (VIEW_MODEL.MODEL.CHAT_MESSAGES.HasMoreMessages && (scrollViewer.DesiredSize.Height < scrollViewer.ViewportHeight))
                {
                    await VIEW_MODEL.MODEL.CHAT_MESSAGES.LoadMoreMessagesAsync();
                }
            }
        }
        #endregion

        #region --Misc Methods (Protected)--


        #endregion
        //--------------------------------------------------------Events:---------------------------------------------------------------------\\
        #region --Events--
        private static async void ChatPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            if (d is ChatMessageListControl control)
            {
                control.scrolledToTheTop = true;
                await control.LoadMoreMessagesAsync();
                if (!(control.scrollViewer is null))
                {
                    control.scrollViewer.ChangeView(null, control.scrollViewer.ScrollableHeight, null, true);
                }
            }
        }

        private static void OnIsDummyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is ChatMessageListControl control && e.NewValue is bool b)
            {
                control.VIEW_MODEL.MODEL.IsDummy = b;
            }
        }

        private void OnScrollViewerViewChanged(object sender, ScrollViewerViewChangedEventArgs e)
        {
            TryIncrementalLoading();
            TryMarkAsRead();
            if (e.IsIntermediate)
            {
                UpdateBehavior();
            }
        }

        private async void MainListView_Loaded(object sender, RoutedEventArgs e)
        {
            loaded = true;
            itemsStackPanel = mainListView.FindDescendant<ItemsStackPanel>();
            scrollViewer = mainListView.FindDescendant<ScrollViewer>();
            if (scrollViewer is null)
            {
                return;
            }
            scrollViewer.ViewChanged += OnScrollViewerViewChanged;
            VIEW_MODEL.MODEL.CHAT_MESSAGES.CollectionChanged += OnChatMessagesCollectionChanged;
            UpdateBehavior();
            await LoadMoreMessagesAsync();
            scrollViewer.ChangeView(null, scrollViewer.ScrollableHeight, null, true);
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

        private void mainListView_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            _ = LoadMoreMessagesAsync();
        }
        #endregion
    }
}
