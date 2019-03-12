using Shared.Classes;
using System.Collections.ObjectModel;
using Windows.Security.Cryptography.Certificates;
using XMPP_API.Classes.Network;

namespace UWPX_UI_Context.Classes.DataTemplates.Dialogs
{
    public sealed class CertificateRequirementsDialogDataTemplate : AbstractDataTemplate
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        public readonly ObservableCollection<CertificateRequirementDataTemplate> ITEMS = new ObservableCollection<CertificateRequirementDataTemplate>();
        private readonly XMPPAccount ACCOUNT;

        private bool _Confirmed;
        public bool Confirmed
        {
            get { return _Confirmed; }
            set { SetProperty(ref _Confirmed, value); }
        }

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        public CertificateRequirementsDialogDataTemplate(XMPPAccount account)
        {
            this.ACCOUNT = account;
            LoadRequirements();
        }

        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--


        #endregion
        //--------------------------------------------------------Misc Methods:---------------------------------------------------------------\\
        #region --Misc Methods (Public)--


        #endregion

        #region --Misc Methods (Private)--
        private void LoadRequirements()
        {
            ITEMS.Add(new CertificateRequirementDataTemplate()
            {
                CertError = ChainValidationResult.Expired,
                Description = "The certificate hasn't expired.",
                Required = true,
                Name = "Not expired"
            });
            ITEMS.Add(new CertificateRequirementDataTemplate()
            {
                CertError = ChainValidationResult.InvalidName,
                Description = "The certificate has a valid name.",
                Required = true,
                Name = "Valid Name"
            });
            ITEMS.Add(new CertificateRequirementDataTemplate()
            {
                CertError = ChainValidationResult.Untrusted,
                Description = "The certificate isn't untrusted/self signed.",
                Required = true,
                Name = "Trusted"
            });
            ITEMS.Add(new CertificateRequirementDataTemplate()
            {
                CertError = ChainValidationResult.WrongUsage,
                Description = "The certificate is intended to get used for this.",
                Required = true,
                Name = "Right usage"
            });

            // Load ignored errors:
            foreach (ChainValidationResult ignored in ACCOUNT.connectionConfiguration.IGNORED_CERTIFICATE_ERRORS)
            {
                foreach (CertificateRequirementDataTemplate item in ITEMS)
                {
                    if (item.CertError == ignored)
                    {
                        item.Required = false;
                        break;
                    }
                }
            }
        }

        #endregion

        #region --Misc Methods (Protected)--


        #endregion
        //--------------------------------------------------------Events:---------------------------------------------------------------------\\
        #region --Events--


        #endregion
    }
}
