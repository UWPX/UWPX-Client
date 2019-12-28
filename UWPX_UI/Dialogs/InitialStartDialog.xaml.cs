using System;
using UWPX_UI_Context.Classes;
using UWPX_UI_Context.Classes.DataContext.Dialogs;
using Windows.UI.Xaml.Controls;

namespace UWPX_UI.Dialogs
{
    public sealed partial class InitialStartDialog: ContentDialog
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        public readonly InitialStartDialogContext VIEW_MODEL = new InitialStartDialogContext();

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        public InitialStartDialog()
        {
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
        private async void Content_mdc_LinkClicked(object sender, Microsoft.Toolkit.Uwp.UI.Controls.LinkClickedEventArgs e)
        {
            await UiUtils.LaunchUriAsync(new Uri(e.Link));
        }

        private void Close_btn_Click(Controls.IconButtonControl sender, Windows.UI.Xaml.RoutedEventArgs args)
        {

        }

        #endregion
    }
}
