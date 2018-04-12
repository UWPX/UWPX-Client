using System;
using System.Collections.ObjectModel;
using UWP_XMPP_Client.DataTemplates;
using Windows.Security.Cryptography.Certificates;
using Windows.UI.Xaml.Controls;

namespace UWP_XMPP_Client.Dialogs
{
    public sealed partial class ChangeCertificateRequirementsDialog : ContentDialog
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        private ObservableCollection<CertificateRequirementTemplate> certificateRequirements;

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        /// <summary>
        /// Basic Constructor
        /// </summary>
        /// <history>
        /// 12/04/2018 Created [Fabian Sauter]
        /// </history>
		public ChangeCertificateRequirementsDialog()
        {
            this.certificateRequirements = new ObservableCollection<CertificateRequirementTemplate>();
            loadCertificateRequirements();
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
        private void loadCertificateRequirements()
        {
            certificateRequirements.Add(new CertificateRequirementTemplate()
            {
                certificateError = ChainValidationResult.Expired,
                description = "The certificate has not expired.",
                required = true,
                name = "Not expired"
            });
            certificateRequirements.Add(new CertificateRequirementTemplate()
            {
                certificateError = ChainValidationResult.InvalidName,
                description = "The certificate has a valid name.",
                required = true,
                name = "Valid name"
            });
            certificateRequirements.Add(new CertificateRequirementTemplate()
            {
                certificateError = ChainValidationResult.InvalidSignature,
                description = "The certificate has a valid signature.",
                required = true,
                name = "Valid signature"
            });
            certificateRequirements.Add(new CertificateRequirementTemplate()
            {
                certificateError = ChainValidationResult.Revoked,
                description = "The certificate has been revoked.",
                required = true,
                name = "Not revoked"
            });
            certificateRequirements.Add(new CertificateRequirementTemplate()
            {
                certificateError = ChainValidationResult.Untrusted,
                description = "The certificate is untrusted/self signed.",
                required = true,
                name = "Trusted"
            });
            certificateRequirements.Add(new CertificateRequirementTemplate()
            {
                certificateError = ChainValidationResult.WrongUsage,
                description = "The certificate is not intended to get used for this.",
                required = true,
                name = "Right usage"
            });
        }

        #endregion

        #region --Misc Methods (Protected)--


        #endregion
        //--------------------------------------------------------Events:---------------------------------------------------------------------\\
        #region --Events--
        private void ContentDialog_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
        }

        private void ContentDialog_SecondaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
        }

        #endregion
    }
}
