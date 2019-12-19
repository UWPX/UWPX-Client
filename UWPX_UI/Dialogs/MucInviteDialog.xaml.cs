using UWPX_UI_Context.Classes.DataContext.Dialogs;
using UWPX_UI_Context.Classes.DataTemplates;
using Windows.UI.Xaml.Controls;

namespace UWPX_UI.Dialogs
{
    public sealed partial class MucInviteDialog: ContentDialog
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        public readonly MucInviteDialogContext VIEW_MODEL = new MucInviteDialogContext();
        public readonly ChatDataTemplate CHAT;

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        public MucInviteDialog(ChatDataTemplate chat)
        {
            CHAT = chat;
            VIEW_MODEL.UpdateView(chat);
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


        #endregion

        #region --Misc Methods (Protected)--


        #endregion
        //--------------------------------------------------------Events:---------------------------------------------------------------------\\
        #region --Events--
        private async void Invite_btn_Click(Controls.IconProgressButtonControl sender, Windows.UI.Xaml.RoutedEventArgs args)
        {
            await VIEW_MODEL.InviteAsync(CHAT);
            Hide();
        }

        private void Cancel_ibtn_Click(Controls.IconButtonControl sender, Windows.UI.Xaml.RoutedEventArgs args)
        {
            VIEW_MODEL.Cancel();
            Hide();
        }

        #endregion
    }
}
