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

        private bool scrolledToTheTop;

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
        private void UpdateView(DependencyPropertyChangedEventArgs args)
        {
            ChatDataTemplate oldChat = args.OldValue is ChatDataTemplate ? args.OldValue as ChatDataTemplate : null;
            ChatDataTemplate newChat = args.NewValue is ChatDataTemplate ? args.NewValue as ChatDataTemplate : null;
            VIEW_MODEL.UpdateView(oldChat, newChat);
        }

        private UIElement GetLastListUiElement()
        {
            if (mainListView.ItemsPanelRoot.Children.Count > 0)
            {
                return mainListView.ItemsPanelRoot.Children[mainListView.ItemsPanelRoot.Children.Count - 1];
            }
            return null;
        }

        private void UpdateScrollDownBtnVisibility()
        {
            UIElement lastElem = GetLastListUiElement();
            bool lastElemOutOfView = scrollViewer.VerticalOffset > (scrollViewer.ScrollableHeight - lastElem.ActualSize.Y);
            scrollDown_btn.Visibility = (lastElem is null) || lastElemOutOfView ? Visibility.Collapsed : Visibility.Visible;
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

        private async void ScrollViewer_ViewChanged(object sender, ScrollViewerViewChangedEventArgs e)
        {
            if (scrollViewer.VerticalOffset < mainListViewHeader.ActualHeight + 10)
            {
                if (!scrolledToTheTop)
                {
                    scrolledToTheTop = true;
                    if (VIEW_MODEL.MODEL.hasMoreMessages)
                    {
                        itemsStackPanel.ItemsUpdatingScrollMode = ItemsUpdatingScrollMode.KeepItemsInView;
                        await VIEW_MODEL.MODEL.LoadMoreMessagesAsync();
                        itemsStackPanel.ItemsUpdatingScrollMode = ItemsUpdatingScrollMode.KeepLastItemInView;
                    }
                }
            }
            else
            {
                scrolledToTheTop = false;
            }

            if (e.IsIntermediate)
            {
                UpdateScrollDownBtnVisibility();
            }
        }

        private void MainListView_Loaded(object sender, RoutedEventArgs e)
        {
            itemsStackPanel = mainListView.FindDescendant<ItemsStackPanel>();
            scrollViewer = mainListView.FindDescendant<ScrollViewer>();
            scrollViewer.ViewChanged += ScrollViewer_ViewChanged;
            UpdateScrollDownBtnVisibility();
        }

        private void scrollDown_btn_Click(IconButtonControl sender, RoutedEventArgs args)
        {
            if (mainListView.ItemsPanelRoot.Children.Count > 0)
            {
                scrollViewer.ChangeView(null, scrollViewer.ScrollableHeight, null);
            }
        }

        #endregion
    }
}
