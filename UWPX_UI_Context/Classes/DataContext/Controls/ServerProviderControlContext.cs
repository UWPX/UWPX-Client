using System;
using UWPX_UI_Context.Classes.DataTemplates.Controls;
using XMPP_API.Classes;

namespace UWPX_UI_Context.Classes.DataContext.Controls
{
    public class ServerProviderControlContext
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        public readonly ServerProviderControlDataTemplate MODEL = new ServerProviderControlDataTemplate();

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--


        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--


        #endregion
        //--------------------------------------------------------Misc Methods:---------------------------------------------------------------\\
        #region --Misc Methods (Public)--
        public void UpdateView(Provider provider)
        {
            if (provider is not null)
            {
                MODEL.CompanyText = provider.company ? "Yes" : "No";
                MODEL.FreeText = provider.free ? "Yes" : "No";
                MODEL.HostedText = provider.professionalHosting ? "Yes" : "No";
                MODEL.PasswordResetText = provider.passwordReset ? "Yes" : "No";
                MODEL.LegalNoticeUrl = string.IsNullOrEmpty(provider.legalNotice) ? "" : provider.legalNotice;
                MODEL.OnlinceSiceText = provider.onlineSince == DateTime.MinValue || provider.onlineSince == DateTime.MaxValue ? "-" : provider.onlineSince.ToLongDateString();
                MODEL.MamStorageTimeText = provider.mamStorageTime <= 0 ? "-" : $"{provider.mamStorageTime} days";
                MODEL.UploadStorageTime = provider.fileUploadStorageTime <= 0 ? "-" : $"{provider.fileUploadStorageTime} days";
                MODEL.MaxUploadSizeText = provider.maxFileUploadSize <= 0 ? "-" : $"{provider.maxFileUploadSize} MB";
            }
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
