using Storage.Classes.Models.Chat;
using UWPX_UI_Context.Classes.DataTemplates.Dialogs;

namespace UWPX_UI_Context.Classes.DataContext.Dialogs
{
    public sealed class DeleteChatConfirmDialogContext
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        public readonly DeleteChatConfirmDialogDataTemplate MODEL;

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        public DeleteChatConfirmDialogContext(ChatModel chat)
        {
            MODEL = new DeleteChatConfirmDialogDataTemplate(chat);
        }

        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--


        #endregion
        //--------------------------------------------------------Misc Methods:---------------------------------------------------------------\\
        #region --Misc Methods (Public)--
        public void Confirm()
        {
            MODEL.Confirmed = true;
        }

        public void Cancel()
        {
            MODEL.Confirmed = false;
        }

        #endregion

        #region --Misc Methods (Private)--


        #endregion

        #region --Misc Methods (Protected)--


        #endregion
        //--------------------------------------------------------Events:---------------------------------------------------------------------\\
        #region --Events--


        #endregion
    }
}
