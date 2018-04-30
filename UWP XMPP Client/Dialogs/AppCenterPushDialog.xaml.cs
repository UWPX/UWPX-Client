using System;
using UWP_XMPP_Client.Classes;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace UWP_XMPP_Client.Dialogs
{
    public sealed partial class AppCenterPushDialog : ContentDialog
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        public string MarkdownText
        {
            get { return (string)GetValue(MarkdownTextProperty); }
            set { SetValue(MarkdownTextProperty, value); }
        }
        public static readonly DependencyProperty MarkdownTextProperty = DependencyProperty.Register("MarkdownText", typeof(string), typeof(AppCenterPushDialog), null);

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        /// <summary>
        /// Basic Constructor
        /// </summary>
        /// <history>
        /// 30/04/2018 Created [Fabian Sauter]
        /// </history>
        public AppCenterPushDialog(string title, string markdownText)
        {
            this.Title = title;
            this.MarkdownText = markdownText;
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


        #endregion

        #region --Misc Methods (Protected)--


        #endregion
        //--------------------------------------------------------Events:---------------------------------------------------------------------\\
        #region --Events--
        private void ContentDialog_SecondaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
        }

        private async void MarkdownTextBlock_LinkClicked(object sender, Microsoft.Toolkit.Uwp.UI.Controls.LinkClickedEventArgs e)
        {
            await UiUtils.launchBrowserAsync(new Uri(e.Link));
        }

        #endregion
    }
}
