using Storage.Classes.Models.Account;
using UWPX_UI_Context.Classes.DataTemplates.Dialogs;

namespace UWPX_UI_Context.Classes.DataContext.Dialogs
{
    public sealed class CertificateRequirementsDialogContext
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        public readonly CertificateRequirementsDialogDataTemplate MODEL;

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        public CertificateRequirementsDialogContext(AccountModel account)
        {
            MODEL = new CertificateRequirementsDialogDataTemplate(account);
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
