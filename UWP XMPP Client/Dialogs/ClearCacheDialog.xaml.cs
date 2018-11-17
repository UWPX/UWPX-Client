using Data_Manager2.Classes;
using Data_Manager2.Classes.DBTables;
using Data_Manager2.Classes.DBTables.Omemo;
using System;
using System.Threading.Tasks;
using Thread_Save_Components.Classes.SQLite;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace UWP_XMPP_Client.Dialogs
{
    public sealed partial class ClearCacheDialog : ContentDialog
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--


        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        /// <summary>
        /// Basic Constructor
        /// </summary>
        /// <history>
        /// 20/01/2018 Created [Fabian Sauter]
        /// </history>
        public ClearCacheDialog()
        {
            this.InitializeComponent();
            setSelectedNodes();
        }

        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--
        private void setSelectedNodes()
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
        private bool isChecked(Microsoft.UI.Xaml.Controls.TreeViewNode node)
        {
            return false;
        }

        private void clearCache()
        {
            clear_btn.IsEnabled = false;
            close_btn.IsEnabled = false;
            tree_tv.IsEnabled = false;
            clear_prgr.Visibility = Visibility.Visible;

            bool chatMessages = isChecked(chatMessages_tvn);
            bool chat = isChecked(chats_tvn);
            bool images = isChecked(images_tvn);

            bool discoFeatures = isChecked(discoFeatures_tvn);
            bool discoIdentities = isChecked(discoIdentities_tvn);
            bool discoItems = isChecked(discoItems_tvn);

            bool mucChatInfo = isChecked(mucChatInfo_tvn);
            bool mucMembers = isChecked(mucMembers_tvn);
            bool mucDirectInvites = isChecked(mucDirectInvites_tvn);

            bool accounts = isChecked(account_tvn);
            bool passwordVault = isChecked(passwordVault_tvn);
            bool ignoredCertificateErrors = isChecked(ignoredCertificateErrors_tvn);
            bool connectionOptions = isChecked(connectionOptions_tvn);

            bool omemoDeviceListSubscriptions = isChecked(omemoDeviceListSubscriptions_tvn);
            bool omemoDevices = isChecked(omemoDevices_tvn);
            bool omemoIdentityKeys = isChecked(omemoIdentityKeys_tvn);
            bool omemoPreKeys = isChecked(omemoPreKeys_tvn);
            bool omemoSignedPreKeys = isChecked(omemoSignedPreKeys_tvn);
            bool omemoSessions = isChecked(omemoSessions_tvn);

            bool reloadClients = isChecked(reloadClients_tvn);

            Task.Run(async () =>
            {
                // General:
                if (chatMessages)
                {
                    AbstractDBManager.dB.RecreateTable<ChatMessageTable>();
                }
                if (chat)
                {
                    AbstractDBManager.dB.RecreateTable<ChatTable>();
                }
                if (images)
                {
                    AbstractDBManager.dB.RecreateTable<ImageTable>();
                }

                // Disco:
                if (discoFeatures)
                {
                    AbstractDBManager.dB.RecreateTable<DiscoFeatureTable>();
                }
                if (discoIdentities)
                {
                    AbstractDBManager.dB.RecreateTable<DiscoIdentityTable>();
                }
                if (discoItems)
                {
                    AbstractDBManager.dB.RecreateTable<DiscoItemTable>();
                }

                // MUC:
                if (mucChatInfo)
                {
                    AbstractDBManager.dB.RecreateTable<MUCChatInfoTable>();
                }
                if (mucMembers)
                {
                    AbstractDBManager.dB.RecreateTable<MUCOccupantTable>();
                }
                if (mucDirectInvites)
                {
                    AbstractDBManager.dB.RecreateTable<MUCDirectInvitationTable>();
                }

                // Accounts:
                if (accounts)
                {
                    AbstractDBManager.dB.RecreateTable<AccountTable>();
                }
                if (passwordVault)
                {
                    Vault.deleteAllVaults();
                }
                if (ignoredCertificateErrors)
                {
                    AbstractDBManager.dB.RecreateTable<IgnoredCertificateErrorTable>();
                }
                if (connectionOptions)
                {
                    AbstractDBManager.dB.RecreateTable<ConnectionOptionsTable>();
                }

                // OMEMO:
                if (omemoDeviceListSubscriptions)
                {
                    AbstractDBManager.dB.RecreateTable<OmemoDeviceListSubscriptionTable>();
                }
                if (omemoDevices)
                {
                    AbstractDBManager.dB.RecreateTable<OmemoDeviceTable>();
                }
                if (omemoIdentityKeys)
                {
                    AbstractDBManager.dB.RecreateTable<OmemoIdentityKeyTable>();
                }
                if (omemoPreKeys)
                {
                    AbstractDBManager.dB.RecreateTable<OmemoPreKeyTable>();
                }
                if (omemoSignedPreKeys)
                {
                    AbstractDBManager.dB.RecreateTable<OmemoSignedPreKeyTable>();
                }
                if (omemoSessions)
                {
                    AbstractDBManager.dB.RecreateTable<OmemoSessionStoreTable>();
                }

                // Clients:
                if (reloadClients)
                {
                    ConnectionHandler.INSTANCE.reloadClients();
                }

                await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
                {
                    clear_btn.IsEnabled = true;
                    close_btn.IsEnabled = true;
                    tree_tv.IsEnabled = true;
                    clear_prgr.Visibility = Visibility.Collapsed;

                    // Show non found in app notification:
                    done_notification.Show("Done cleaning cache!", 0);
                });
            });
        }

        #endregion

        #region --Misc Methods (Protected)--


        #endregion
        //--------------------------------------------------------Events:---------------------------------------------------------------------\\
        #region --Events--
        private void clear_btn_Click(object sender, RoutedEventArgs e)
        {
            done_notification.Dismiss();
            clearCache();
        }

        private void close_btn_Click(object sender, RoutedEventArgs e)
        {
            done_notification.Dismiss();
            Hide();
        }

        #endregion
    }
}
