using UWPX_UI_Context.Classes.DataContext.Dialogs;
using Windows.UI.Xaml.Controls;

namespace UWPX_UI.Dialogs
{
    public sealed partial class ClearCacheDialog : ContentDialog
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        public readonly ClearCacheDialogContext VIEW_MODEL = new ClearCacheDialogContext();

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        public ClearCacheDialog()
        {
            this.InitializeComponent();
        }

        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--
        private void SetSelectedNodes()
        {
            tree_tv.SelectedNodes.Add(general_tvn);
            tree_tv.SelectedNodes.Add(disco_tvn);
            tree_tv.SelectedNodes.Add(muc_tvn);
            tree_tv.SelectedNodes.Add(clients_tvn);
        }

        #endregion
        //--------------------------------------------------------Misc Methods:---------------------------------------------------------------\\
        #region --Misc Methods (Public)--


        #endregion

        #region --Misc Methods (Private)--
        private void UpdateSelected()
        {
            // General:
            VIEW_MODEL.MODEL.ChatMessages = tree_tv.SelectedNodes.Contains(chatMessages_tvn);
            VIEW_MODEL.MODEL.Chats = tree_tv.SelectedNodes.Contains(chats_tvn);
            VIEW_MODEL.MODEL.Images = tree_tv.SelectedNodes.Contains(images_tvn);

            // Disco:
            VIEW_MODEL.MODEL.DiscoFeatures = tree_tv.SelectedNodes.Contains(discoFeatures_tvn);
            VIEW_MODEL.MODEL.DiscoFeatures = tree_tv.SelectedNodes.Contains(discoIdentities_tvn);
            VIEW_MODEL.MODEL.DiscoItems = tree_tv.SelectedNodes.Contains(discoItems_tvn);

            // MUC:
            VIEW_MODEL.MODEL.MucChatInfo = tree_tv.SelectedNodes.Contains(mucChatInfo_tvn);
            VIEW_MODEL.MODEL.MucOccupants = tree_tv.SelectedNodes.Contains(mucMembers_tvn);
            VIEW_MODEL.MODEL.MucDirectInvites = tree_tv.SelectedNodes.Contains(mucDirectInvites_tvn);

            // Accounts:
            VIEW_MODEL.MODEL.Accounts = tree_tv.SelectedNodes.Contains(accounts_tvn);
            VIEW_MODEL.MODEL.PasswordVault = tree_tv.SelectedNodes.Contains(passwordVault_tvn);
            VIEW_MODEL.MODEL.IgnoredCertErrors = tree_tv.SelectedNodes.Contains(ignoredCertificateErrors_tvn);
            VIEW_MODEL.MODEL.ConnectionOptions = tree_tv.SelectedNodes.Contains(connectionOptions_tvn);

            // OMEMO:
            VIEW_MODEL.MODEL.OmemoDeviceListSubscriptions = tree_tv.SelectedNodes.Contains(omemoDeviceListSubscriptions_tvn);
            VIEW_MODEL.MODEL.OmemoDevices = tree_tv.SelectedNodes.Contains(omemoDevices_tvn);
            VIEW_MODEL.MODEL.OmemoIdentityKeys = tree_tv.SelectedNodes.Contains(omemoIdentityKeys_tvn);
            VIEW_MODEL.MODEL.OmemoPreKeys = tree_tv.SelectedNodes.Contains(omemoPreKeys_tvn);
            VIEW_MODEL.MODEL.OmemoSignedPreKeys = tree_tv.SelectedNodes.Contains(omemoSignedPreKeys_tvn);
            VIEW_MODEL.MODEL.OmemoSessions = tree_tv.SelectedNodes.Contains(omemoSessions_tvn);

            // Clients:
            VIEW_MODEL.MODEL.ReloadClients = tree_tv.SelectedNodes.Contains(reloadClients_tvn);
        }

        #endregion

        #region --Misc Methods (Protected)--


        #endregion
        //--------------------------------------------------------Events:---------------------------------------------------------------------\\
        #region --Events--
        private void Cancel_btn_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            Hide();
        }

        private async void Clear_btn_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            UpdateSelected();
            bool result = await VIEW_MODEL.ClearCacheAsync();
            if (result)
            {
                done_notification.Show("Done cleaning cache.", 5000);
            }
            else
            {
                done_notification.Show("Failed to clear cache! View the logs for more information.", 0);
            }
        }

        private void Tree_tv_Loaded(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            SetSelectedNodes();
        }

        #endregion
    }
}
