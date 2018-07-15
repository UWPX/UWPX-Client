using System;
using UWP_XMPP_Client.Classes;
using UWP_XMPP_Client.Dialogs;
using UWP_XMPP_Client.Pages.SettingsPages;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using XMPP_API.Classes;
using XMPP_API.Classes.Network.XML.Messages;
using XMPP_API.Classes.Network.XML.Messages.XEP_0402;

namespace UWP_XMPP_Client.Pages
{
    public sealed partial class ManageBookmarksPage : Page
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        private MessageResponseHelper<IQMessage> messageResponseHelper;
        private CustomObservableCollection<ConferenceItem> bookmarks;

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        /// <summary>
        /// Basic Constructor
        /// </summary>
        /// <history>
        /// 13/06/2018 Created [Fabian Sauter]
        /// </history>
        public ManageBookmarksPage()
        {
            SystemNavigationManager.GetForCurrentView().BackRequested += BrowseMUCRoomsPage_BackRequested;
            this.messageResponseHelper = null;
            this.bookmarks = new CustomObservableCollection<ConferenceItem>();
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
        private void requestBookmarks(XMPPClient c)
        {
            messageResponseHelper = c.PUB_SUB_COMMAND_HELPER.requestBookmars(onMessage, onTimeout);
        }

        private void onTimeout()
        {
            Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => noneFound_notification.Show("Request failed - timeout!")).AsTask();
        }

        private bool onMessage(AbstractMessage msg)
        {
            if (msg is IQErrorMessage errorMsg)
            {
                Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                {
                    bookmarks.Clear();
                    if (errorMsg.ERROR_OBJ.ERROR_NAME == ErrorName.ITEM_NOT_FOUND)
                    {
                        noneFound_notification.Show("Found 0 bookmarks.", 2000);
                    }
                    else
                    {
                        noneFound_notification.Show("Request failed with:\n" + errorMsg.ERROR_OBJ.ERROR_NAME + " and " + errorMsg.ERROR_OBJ.ERROR_MESSAGE);
                    }
                    reload_abb.IsEnabled = true;
                    loading_grid.Visibility = Visibility.Collapsed;
                    main_grid.Visibility = Visibility.Visible;
                }).AsTask();
                return true;
            }
            else if (msg is BookmarksResultMessage result)
            {
                Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                {
                    bookmarks.Clear();
                    bookmarks.AddRange(result.conferences);
                    if (result.conferences.Count > 1)
                    {
                        noneFound_notification.Show("Found " + result.conferences.Count + " bookmarks.", 2000);
                    }
                    else
                    {
                        noneFound_notification.Show("Found " + result.conferences.Count + " bookmark.", 2000);
                    }
                    reload_abb.IsEnabled = true;
                    loading_grid.Visibility = Visibility.Collapsed;
                    main_grid.Visibility = Visibility.Visible;
                }).AsTask();
                return true;
            }
            return false;
        }

        private void reload(XMPPClient c)
        {
            reload_abb.IsEnabled = false;
            if (c != null && c.isConnected())
            {
                loading_grid.Visibility = Visibility.Visible;
                main_grid.Visibility = Visibility.Collapsed;
                requestBookmarks(c);
            }
        }

        #endregion

        #region --Misc Methods (Protected)--


        #endregion
        //--------------------------------------------------------Events:---------------------------------------------------------------------\\
        #region --Events--
        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            UiUtils.setBackgroundImage(backgroundImage_img);
        }

        private void BrowseMUCRoomsPage_BackRequested(object sender, BackRequestedEventArgs e)
        {
            if (!(Window.Current.Content is Frame rootFrame))
            {
                return;
            }
            if (rootFrame.CanGoBack && e.Handled == false)
            {
                e.Handled = true;
                rootFrame.GoBack();
            }
        }

        private void reload_abb_Click(object sender, RoutedEventArgs e)
        {
            reload(account_asc.getSelectedAccount());
        }

        private void addAccount_hlb_Click(object sender, RoutedEventArgs e)
        {
            (Window.Current.Content as Frame).Navigate(typeof(AccountSettingsPage));
        }

        private void Page_Unloaded(object sender, RoutedEventArgs e)
        {
            messageResponseHelper?.stop();
        }

        private async void add_abb_Click(object sender, RoutedEventArgs e)
        {
            AddBookmarkDialog dialog = new AddBookmarkDialog(account_asc.getSelectedAccount());
            await UiUtils.showDialogAsyncQueue(dialog);
            if (dialog.success)
            {
                noneFound_notification.Show("Bookmark added.", 2000);
            }
        }

        private void account_asc_AccountSelectionChanged(Controls.AccountSelectionControl sender, Classes.Events.AccountSelectionChangedEventArgs args)
        {
            reload(args.CLIENT);
        }

        #endregion
    }
}
