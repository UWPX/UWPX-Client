using Data_Manager2.Classes.DBTables;
using UWPX_UI_Context.Classes.DataContext.Dialogs;
using Windows.UI.Xaml.Controls;
using XMPP_API.Classes;

namespace UWPX_UI.Dialogs
{
    public sealed partial class ChangeNicknameDialog: ContentDialog
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        public readonly ChangeNicknameDialogContext VIEW_MODEL = new ChangeNicknameDialogContext();
        private readonly ChatTable CHAT;
        private readonly MUCChatInfoTable MUC_INFO;
        private readonly XMPPClient CLIENT;

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        public ChangeNicknameDialog(ChatTable chat, MUCChatInfoTable mucInfo, XMPPClient client)
        {
            CHAT = chat;
            MUC_INFO = mucInfo;
            CLIENT = client;
            VIEW_MODEL.UpdateView(chat, mucInfo);
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
        private void ContentDialog_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            Hide();
        }

        private async void ContentDialog_SecondaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            args.Cancel = await VIEW_MODEL.SaveAsync(CHAT, MUC_INFO, CLIENT);
        }

        #endregion
    }
}
