using Data_Manager2.Classes.DBTables;
using UWPX_UI_Context.Classes.DataContext.Dialogs;
using Windows.UI.Xaml.Controls;

namespace UWPX_UI.Dialogs
{
    public sealed partial class DeleteChatConfirmDialog: ContentDialog
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        public readonly DeleteChatConfirmDialogContext VIEW_MODEL;

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        public DeleteChatConfirmDialog(ChatTable chat)
        {
            VIEW_MODEL = new DeleteChatConfirmDialogContext(chat);
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
        private void yes_btn_Click(Controls.IconButtonControl sender, Windows.UI.Xaml.RoutedEventArgs args)
        {
            VIEW_MODEL.Confirm();
            Hide();
        }

        private void no_btn_Click(Controls.IconButtonControl sender, Windows.UI.Xaml.RoutedEventArgs args)
        {
            VIEW_MODEL.Cancel();
            Hide();
        }

        #endregion
    }
}
