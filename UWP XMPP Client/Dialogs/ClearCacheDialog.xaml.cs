using Data_Manager2.Classes;
using Data_Manager2.Classes.DBManager;
using Data_Manager2.Classes.DBTables;
using System;
using System.Threading.Tasks;
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
        }

        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--


        #endregion
        //--------------------------------------------------------Misc Methods:---------------------------------------------------------------\\
        #region --Misc Methods (Public)--


        #endregion

        #region --Misc Methods (Private)--
        private void clearCache()
        {
            IsPrimaryButtonEnabled = false;
            IsSecondaryButtonEnabled = false;
            main_sclv.IsEnabled = false;
            main_prgr.Visibility = Visibility.Visible;

            bool chatMessages = (bool)chatMessages_chbx.IsChecked;
            bool chat = (bool)chats_chbx.IsChecked;
            bool images = (bool)images_chbx.IsChecked;

            bool discoFeatures = (bool)discoFeatures_chbx.IsChecked;
            bool discoIdentities = (bool)discoIdentities_chbx.IsChecked;
            bool discoItems = (bool)discoItems_chbx.IsChecked;

            bool mucChatInfo = (bool)mucChatInfo_chbx.IsChecked;
            bool mucMembers = (bool)mucMembers_chbx.IsChecked;
            bool reconnect = (bool)reconnect_chbx.IsChecked;

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
                    AbstractDBManager.dB.RecreateTable<MUCMemberTable>();
                }

                // Reconnect clients:
                if (reconnect)
                {
                    ConnectionHandler.INSTANCE.reconnectAll();
                }

                await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
                {
                    IsPrimaryButtonEnabled = true;
                    IsSecondaryButtonEnabled = true;
                    main_sclv.IsEnabled = true;
                    main_prgr.Visibility = Visibility.Collapsed;

                    // Show non found in app notification:
                    done_notification.Show("Done clearing cache!", 0);
                });
            });
        }

        #endregion

        #region --Misc Methods (Protected)--


        #endregion
        //--------------------------------------------------------Events:---------------------------------------------------------------------\\
        #region --Events--
        private void ContentDialog_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            args.Cancel = true;
            done_notification.Dismiss();
            clearCache();
        }

        private void ContentDialog_SecondaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            args.Cancel = true;
            done_notification.Dismiss();
            Hide();
        }

        #endregion
    }
}
