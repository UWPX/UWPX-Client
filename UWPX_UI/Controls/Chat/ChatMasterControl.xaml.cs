using Data_Manager2.Classes;
using Microsoft.Toolkit.Uwp.UI.Controls;
using UWPX_UI_Context.Classes.DataContext;
using UWPX_UI_Context.Classes.DataTemplates;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace UWPX_UI.Controls.Chat
{
    public sealed partial class ChatMasterControl : UserControl
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        public ChatDataTemplate Chat
        {
            get { return (ChatDataTemplate)GetValue(ChatProperty); }
            set { SetValue(ChatProperty, value); }
        }
        public static readonly DependencyProperty ChatProperty = DependencyProperty.Register(nameof(ChatDataTemplate), typeof(ChatDataTemplate), typeof(ChatMasterControl), new PropertyMetadata(null, ChatPropertyChanged));

        private readonly ChatMasterControlContext VIEW_MODEL;

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        public ChatMasterControl()
        {
            this.InitializeComponent();
            this.VIEW_MODEL = new ChatMasterControlContext(Resources);
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
        #region --Presence--
        private void RequestPresenceSubscription_mfo_Click(object sender, RoutedEventArgs e)
        {

        }

        private void CancelPresenceSubscription_mfo_Click(object sender, RoutedEventArgs e)
        {

        }

        private void RejectPresenceSubscription_mfo_Click(object sender, RoutedEventArgs e)
        {

        }

        private void ProbePresence_mfo_Click(object sender, RoutedEventArgs e)
        {

        }

        #endregion
        private void UserControl_RightTapped(object sender, Windows.UI.Xaml.Input.RightTappedRoutedEventArgs e)
        {
            if (Chat is null || Chat.Chat is null)
            {
                return;
            }
            switch (Chat.Chat.chatType)
            {
                case ChatType.CHAT:
                    chat_mfo.ShowAt(this, e.GetPosition(this));
                    break;
                case ChatType.MUC:
                    muc_mfo.ShowAt(this, e.GetPosition(this));
                    break;
                default:
                    break;
            }
        }

        private void Mute_tmfo_Click(object sender, RoutedEventArgs e)
        {

        }

        private void RemoveFromRoster_mfo_Click(object sender, RoutedEventArgs e)
        {

        }

        private void DeleteChat_mfo_Click(object sender, RoutedEventArgs e)
        {

        }

        private void ShowInfo_mfo_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Join_mfo_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Leave_mfo_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Bookmark_tmfo_Click(object sender, RoutedEventArgs e)
        {

        }

        private void MuteMUC_tmfo_Click(object sender, RoutedEventArgs e)
        {

        }

        private void DeleteMUC_mfo_Click(object sender, RoutedEventArgs e)
        {

        }

        private void AccountActionAccept_btn_Click(object sender, RoutedEventArgs e)
        {

        }

        private void AccountActionRefuse_btn_Click(object sender, RoutedEventArgs e)
        {

        }

#pragma warning disable CS0618 // Type or member is obsolete
        private void SlideListItem_sli_SwipeStatusChanged(SlidableListItem sender, SwipeStatusChangedEventArgs args)
#pragma warning restore CS0618 // Type or member is obsolete
        {

        }

        private void Muc_mfo_Opening(object sender, object e)
        {

        }

        private void ShowProfile_mfo_Click(object sender, RoutedEventArgs e)
        {

        }

        private static void ChatPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is ChatMasterControl masterControl)
            {
                masterControl.UpdateView(e);
            }
        }

        #endregion
    }
}
