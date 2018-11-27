using System;
using Data_Manager2.Classes.DBManager;
using System.Threading.Tasks;
using UWP_XMPP_Client.DataTemplates;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using XMPP_API.Classes;

namespace UWP_XMPP_Client.Dialogs
{
    public sealed partial class ChangeAccountPresenceDialog : ContentDialog
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
        /// 19/03/2018 Created [Fabian Sauter]
        /// </history>
        public ChangeAccountPresenceDialog()
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
        private void savePresence()
        {
            XMPPClient client = accountSelection_asc.getSelectedAccount();
            if (client is null)
            {
                accountSelection_asc.showErrorMessage("No account selected!");
                return;
            }

            if (!client.isConnected())
            {
                accountSelection_asc.showErrorMessage("Account not connected!");
                return;
            }

            if (presence_cbx.SelectedIndex < 0)
            {
                accountSelection_asc.showErrorMessage("No presence selected!");
                return;
            }

            if (presence_cbx.SelectedItem is PresenceTemplate)
            {
                save_btn.IsEnabled = false;
                save_pgr.Visibility = Visibility.Visible;

                PresenceTemplate templateItem = presence_cbx.SelectedItem as PresenceTemplate;
                string status = string.IsNullOrEmpty(status_tbx.Text) ? null : status_tbx.Text;

                Task.Run(async () =>
                {
                    // Save presence and status:
                    client.getXMPPAccount().presence = templateItem.presence;
                    client.getXMPPAccount().status = status;

                    AccountDBManager.INSTANCE.setAccount(client.getXMPPAccount(), false);

                    // Send the updated presence and status to the server:
                    await client.GENERAL_COMMAND_HELPER.setPreseceAsync(templateItem.presence, status);

                    await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
                    {
                        save_pgr.Visibility = Visibility.Collapsed;
                        save_btn.IsEnabled = true;
                        accountSelection_asc.showInfoMessage("Presence updated!");
                    });
                });
            }
            else
            {
                accountSelection_asc.showErrorMessage("Invalid presence!");
            }
        }

        #endregion

        #region --Misc Methods (Protected)--


        #endregion
        //--------------------------------------------------------Events:---------------------------------------------------------------------\\
        #region --Events--
        private void presence_cbx_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            save_btn.IsEnabled = presence_cbx.SelectedIndex >= 0;
        }

        private void save_btn_Click(object sender, RoutedEventArgs e)
        {
            savePresence();
        }

        private void close_btn_Click(object sender, RoutedEventArgs e)
        {
            Hide();
        }

        private void AccountSelectionControl_AccountSelectionChanged(Controls.AccountSelectionControl sender, Classes.Events.AccountSelectionChangedEventArgs args)
        {
            if (args.CLIENT != null)
            {
                if (!args.CLIENT.isConnected())
                {
                    presence_cbx.IsEnabled = false;
                    save_btn.IsEnabled = false;
                    return;
                }
                presence_cbx.IsEnabled = true;

                Presence accountPresence = args.CLIENT.getXMPPAccount().presence;
                for (int i = 0; i < presence_cbx.Items.Count; i++)
                {
                    if (presence_cbx.Items[i] is PresenceTemplate)
                    {
                        if ((presence_cbx.Items[i] as PresenceTemplate).presence == accountPresence)
                        {
                            presence_cbx.SelectedIndex = i;
                            break;
                        }
                    }
                }
                status_tbx.Text = args.CLIENT.getXMPPAccount().status ?? "";
            }
            else
            {
                presence_cbx.IsEnabled = false;
                save_btn.IsEnabled = false;
            }
        }

        private void AccountSelectionControl_AddAccountClicked(Controls.AccountSelectionControl sender, Classes.Events.AddAccountClickedEventArgs args)
        {
            Hide();
        }

        #endregion
    }
}
