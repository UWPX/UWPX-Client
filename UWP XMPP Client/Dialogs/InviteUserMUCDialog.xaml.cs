using System;
using Data_Manager2.Classes.DBManager;
using Data_Manager2.Classes.DBTables;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Reflection;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using XMPP_API.Classes;

namespace UWP_XMPP_Client.Dialogs
{
    public sealed partial class InviteUserMUCDialog : ContentDialog
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        public string UserJid
        {
            get { return (string)GetValue(UserJidProperty); }
            set { SetValue(UserJidProperty, value); }
        }
        public static readonly DependencyProperty UserJidProperty = DependencyProperty.Register("UserJid", typeof(string), typeof(InviteUserMUCDialog), null);

        public string Reason
        {
            get { return (string)GetValue(ReasonProperty); }
            set { SetValue(ReasonProperty, value); }
        }
        public static readonly DependencyProperty ReasonProperty = DependencyProperty.Register("Reason", typeof(string), typeof(InviteUserMUCDialog), null);

        private TextBox tbx;
        public bool canceled { get; private set; }
        private readonly ObservableCollection<string> SUGGESTIONS;
        private readonly string USER_ACCOUNT_ID;
        private readonly List<string> MEMBER_LIST;

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        /// <summary>
        /// Basic Constructor
        /// </summary>
        /// <history>
        /// 02/03/2018 Created [Fabian Sauter]
        /// </history>
        public InviteUserMUCDialog(XMPPClient client, List<string> memberList)
        {
            this.USER_ACCOUNT_ID = client.getXMPPAccount().getBareJid();
            this.MEMBER_LIST = memberList;
            this.canceled = true;
            this.SUGGESTIONS = new ObservableCollection<string>();
            this.InitializeComponent();
            this.Reason = "Hi, I'd like to invite you to a MUC chat room.";
        }

        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--


        #endregion
        //--------------------------------------------------------Misc Methods:---------------------------------------------------------------\\
        #region --Misc Methods (Public)--


        #endregion

        #region --Misc Methods (Private)--
        /// <summary>
        /// Updates the list of suggestions for the auto suggest box.
        /// </summary>
        /// <param name="text">The filter text.</param>
        private void updateSuggestedUsers(string text)
        {
            Task.Run(async () =>
            {
                List<ChatTable> list = ChatDBManager.INSTANCE.findUsers(text);
                list.RemoveAll((chat) =>
                {
                    return Equals(chat.chatJabberId, USER_ACCOUNT_ID) || MEMBER_LIST.Contains(chat.chatJabberId);
                });

                await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
                {
                    SUGGESTIONS.Clear();
                    foreach (ChatTable c in list)
                    {
                        SUGGESTIONS.Add(c.chatJabberId);
                    }
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
            Hide();
        }

        private void ContentDialog_SecondaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            canceled = false;
            Hide();
        }

        private void user_asbox_KeyDown(object sender, KeyRoutedEventArgs e)
        {
            if (e.Key == Windows.System.VirtualKey.Space)
            {
                e.Handled = true;
            }
        }

        private void user_asbox_KeyUp(object sender, KeyRoutedEventArgs e)
        {
            if (e.Key == Windows.System.VirtualKey.Escape)
            {
                user_asbox.IsSuggestionListOpen = false;
                Focus(FocusState.Keyboard);
            }
        }

        private void ContentDialog_Loaded(object sender, RoutedEventArgs e)
        {
            // Get TextBox from AutoSuggestBox:
            // Source: https://github.com/Koopakiller/Samples/blob/master/UWP%20AutoSuggestBox%20set%20SelectionStart/UWP%20AutoSuggestBox%20set%20SelectionStart/MainPage.xaml.cs
            // Get the type of the control:
            var type = typeof(AutoSuggestBox);

            // Get Method from Type:
            var method = type.GetMethod("GetTemplateChild", BindingFlags.Instance | BindingFlags.NonPublic);

            // Call method of the object "Asb" and pass parameter "TextBox" (name of the control to obtain):
            tbx = (TextBox)method.Invoke(user_asbox, new object[] { "TextBox" });

            tbx.SelectionChanged += Tbx_SelectionChanged;
        }

        private void Tbx_SelectionChanged(object sender, RoutedEventArgs e)
        {
            int selectionLengt = tbx.SelectionLength;
            int selectionStart = tbx.SelectionStart;
            tbx.Text = tbx.Text.ToLowerInvariant();
            tbx.SelectionStart = selectionStart;
            tbx.SelectionLength = selectionLengt;

            memberInvite_stckp.Visibility = Visibility.Collapsed;
            selfInvite_stckp.Visibility = Visibility.Collapsed;
            invalidJid_stckp.Visibility = Visibility.Collapsed;
            validJid_stckp.Visibility = Visibility.Collapsed;
            IsSecondaryButtonEnabled = false;

            // Check if valid JID:
            if (!Utils.isBareJid(tbx.Text))
            {
                invalidJid_stckp.Visibility = Visibility.Visible;
                return;
            }
            validJid_stckp.Visibility = Visibility.Visible;

            // Check for self invite:
            if (Equals(tbx.Text, USER_ACCOUNT_ID))
            {
                selfInvite_stckp.Visibility = Visibility.Visible;
                return;
            }

            // Check for member invite:
            if (MEMBER_LIST.Contains(tbx.Text))
            {
                memberInvite_stckp.Visibility = Visibility.Visible;
                return;
            }

            IsSecondaryButtonEnabled = true;
        }

        private void user_asbox_TextChanged(AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs args)
        {
            if (args.Reason == AutoSuggestionBoxTextChangeReason.UserInput)
            {
                updateSuggestedUsers(sender.Text);
            }
        }

        #endregion
    }
}
