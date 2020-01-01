using UWPX_UI_Context.Classes.DataContext.Dialogs;
using UWPX_UI_Context.Classes.DataTemplates;
using Windows.UI.Xaml.Controls;

namespace UWPX_UI.Dialogs
{
    public sealed partial class ChangeNicknameDialog: ContentDialog
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        public readonly ChangeNicknameDialogContext VIEW_MODEL = new ChangeNicknameDialogContext();
        private readonly ChatDataTemplate CHAT;

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        public ChangeNicknameDialog(ChatDataTemplate chat)
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
        private void cancel_btn_Click(Controls.IconButtonControl sender, Windows.UI.Xaml.RoutedEventArgs args)
        {
            Hide();
        }

        private async void save_btn_Click(Controls.IconProgressButtonControl sender, Windows.UI.Xaml.RoutedEventArgs args)
        {
            if (await VIEW_MODEL.SaveAsync(CHAT))
            {
                Hide();
            }
        }

        #endregion
    }
}
