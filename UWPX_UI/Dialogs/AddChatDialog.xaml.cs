using Manager.Classes.Chat;
using UWPX_UI.Controls.Chat;
using UWPX_UI_Context.Classes.DataContext.Dialogs;
using Windows.UI.Xaml.Controls;

namespace UWPX_UI.Dialogs
{
    public sealed partial class AddChatDialog: ContentDialog
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        public readonly AddChatDialogContext VIEW_MODEL = new AddChatDialogContext();

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        public AddChatDialog()
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


        #endregion

        #region --Misc Methods (Protected)--


        #endregion
        //--------------------------------------------------------Events:---------------------------------------------------------------------\\
        #region --Events--
        private void add_btn_Click(Controls.IconButtonControl sender, Windows.UI.Xaml.RoutedEventArgs args)
        {
            VIEW_MODEL.OnConfirm();
            Hide();
        }

        private void cancel_btn_Click(Controls.IconButtonControl sender, Windows.UI.Xaml.RoutedEventArgs args)
        {
            VIEW_MODEL.OnCancel();
            Hide();
        }

        private void AccountSelectionControl_AddAccountClick(Controls.AccountSelectionControl sender, System.ComponentModel.CancelEventArgs args)
        {
            VIEW_MODEL.OnCancel();
            Hide();
        }

        private void AccountSelectionControl_AccountSelectionChanged(Controls.AccountSelectionControl sender, Classes.Events.AccountSelectionChangedEventArgs args)
        {
            VIEW_MODEL.MODEL.Client = args.CLIENT;
        }

        private void ChatSuggestionsControl_SelectionChanged(ChatSuggestionsControl sender, SelectionChangedEventArgs args)
        {
            if (args.AddedItems.Count == 1 && args.AddedItems[0] is ChatDataTemplate chat)
            {
                VIEW_MODEL.OnChatSelected(chat);
            }
        }

        #endregion
    }
}
