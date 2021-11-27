using System;
using System.Collections.Generic;
using Manager.Classes.Chat;
using UWPX_UI_Context.Classes.DataContext.Dialogs;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace UWPX_UI.Dialogs
{
    public sealed partial class ResetOmemoSessionsDialog: ContentDialog
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        public readonly ResetOmemoSessionsDialogContext VIEW_MODEL = new ResetOmemoSessionsDialogContext();

        private ChatDataTemplate chat;

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        public ResetOmemoSessionsDialog(ChatDataTemplate chat)
        {
            this.chat = chat;
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
        private async void OnResetClicked(Controls.IconProgressButtonControl sender, RoutedEventArgs args)
        {
            await VIEW_MODEL.ResetAsync(chat);
            Hide();
        }

        private void OnCancelClicked(Controls.IconButtonControl sender, RoutedEventArgs args)
        {
            Hide();
        }

        #endregion
    }
}
