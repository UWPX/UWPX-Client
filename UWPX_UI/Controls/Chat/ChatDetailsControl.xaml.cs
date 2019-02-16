using UWPX_UI_Context.Classes;
using UWPX_UI_Context.Classes.DataContext.Controls;
using UWPX_UI_Context.Classes.DataTemplates;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace UWPX_UI.Controls.Chat
{
    public sealed partial class ChatDetailsControl : UserControl
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        public ChatDataTemplate Chat
        {
            get { return (ChatDataTemplate)GetValue(ChatProperty); }
            set { SetValue(ChatProperty, value); }
        }
        public static readonly DependencyProperty ChatProperty = DependencyProperty.Register(nameof(ChatDataTemplate), typeof(ChatDataTemplate), typeof(ChatDetailsControl), new PropertyMetadata(null, ChatPropertyChanged));

        public readonly ChatDetailsControlContext VIEW_MODEL = new ChatDetailsControlContext();

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        public ChatDetailsControl()
        {
            this.InitializeComponent();
        }

        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--


        #endregion
        //--------------------------------------------------------Misc Methods:---------------------------------------------------------------\\
        #region --Misc Methods (Public)--
        public void UpdateView(DependencyPropertyChangedEventArgs args)
        {
            VIEW_MODEL.UpdateView(args);
        }

        #endregion

        #region --Misc Methods (Private)--


        #endregion

        #region --Misc Methods (Protected)--


        #endregion
        //--------------------------------------------------------Events:---------------------------------------------------------------------\\
        #region --Events--
        private static void ChatPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is ChatDetailsControl detailsControl)
            {
                detailsControl.UpdateView(e);
            }
        }

        private void HeaderInfo_grid_RightTapped(object sender, Windows.UI.Xaml.Input.RightTappedRoutedEventArgs e)
        {
            if (sender is FrameworkElement element)
            {
                headerInfo_grid.ContextFlyout.ShowAt(element);
            }
        }

        private void CopyNameText_mfi_Click(object sender, RoutedEventArgs e)
        {
            UiUtils.SetClipboardText(VIEW_MODEL.MODEL.NameText);
        }

        private void CopyChatStatus_mfi_Click(object sender, RoutedEventArgs e)
        {
            UiUtils.SetClipboardText(VIEW_MODEL.MODEL.StatusText);
        }

        private void CopyAccountText_mfi_Click(object sender, RoutedEventArgs e)
        {
            UiUtils.SetClipboardText(VIEW_MODEL.MODEL.AccountText);
        }

        private void Info_mfo_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Join_mfo_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Leave_mfo_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Test_mfo_Click(object sender, RoutedEventArgs e)
        {

        }

        private void ClipImgLib_btn_Click(object sender, RoutedEventArgs e)
        {

        }

        private void ClipImgCam_btn_Click(object sender, RoutedEventArgs e)
        {

        }

        private void ClipDraw_btn_Click(object sender, RoutedEventArgs e)
        {

        }

        private void ClipFile_btn_Click(object sender, RoutedEventArgs e)
        {

        }

        private async void Send_btn_Click(object sender, RoutedEventArgs e)
        {
            await VIEW_MODEL.SendChatMessageAsync(Chat);
        }

        private async void Message_tbx_KeyUp(object sender, Windows.UI.Xaml.Input.KeyRoutedEventArgs e)
        {
            await VIEW_MODEL.OnChatMessageKeyDown(e, Chat);
        }

        private async void ReadOnOmemo_link_Click(object sender, RoutedEventArgs e)
        {
            await VIEW_MODEL.OnReadOnOmemoClickedAsync();
        }

        #endregion
    }
}
