using System;
using Data_Manager2.Classes.DBManager;
using Data_Manager2.Classes.DBTables;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Reflection;
using System.Threading.Tasks;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using XMPP_API.Classes;

namespace UWP_XMPP_Client.Dialogs
{
    public sealed partial class InviteUserMUCDialog : ContentDialog
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        public string Text
        {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }
        public static readonly DependencyProperty TextProperty = DependencyProperty.Register("Text", typeof(string), typeof(InviteUserMUCDialog), null);

        private TextBox tbx;
        public bool canceled;
        private ObservableCollection<string> suggestions;

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        /// <summary>
        /// Basic Constructor
        /// </summary>
        /// <history>
        /// 02/03/2018 Created [Fabian Sauter]
        /// </history>
        public InviteUserMUCDialog()
        {
            this.canceled = true;
            this.suggestions = new ObservableCollection<string>();
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
        private void updateSuggestedUsers(string text)
        {
            Task.Run(async () =>
            {
                List<ChatTable> list = ChatDBManager.INSTANCE.findUsers(text);

                await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
                {
                    suggestions.Clear();
                    foreach (ChatTable c in list)
                    {
                        suggestions.Add(c.chatJabberId);
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
            if (e.Key == Windows.System.VirtualKey.Enter)
            {
                e.Handled = true;
                canceled = false;
                Hide();
            }
            else if(e.Key == Windows.System.VirtualKey.Escape)
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
            int selectionStart = tbx.SelectionStart;
            user_asbox.Text = user_asbox.Text.ToLower();
            tbx.SelectionStart = selectionStart;
            tbx.SelectionLength = 0;
            user_asbox.BorderBrush = new SolidColorBrush(Utils.isBareJid(user_asbox.Text) ? Colors.Green : Colors.Red);
        }

        private void user_asbox_TextChanged(AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs args)
        {
            if(args.Reason == AutoSuggestionBoxTextChangeReason.UserInput)
            {
                updateSuggestedUsers(sender.Text);
            }
        }

        #endregion
    }
}
