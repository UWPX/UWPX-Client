using Shared.Classes;
using XMPP_API.Classes;
using XMPP_API.Classes.Network;

namespace UWPX_UI_Context.Classes.DataTemplates.Pages
{
    public sealed class AddAccountPageDataTemplate: AbstractDataTemplate
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        private string _BareJidText;
        public string BareJidText
        {
            get => _BareJidText;
            set => SetProperty(ref _BareJidText, value);
        }
        private bool _IsValidBareJid;
        public bool IsValidBareJid
        {
            get => _IsValidBareJid;
            set => SetProperty(ref _IsValidBareJid, value);
        }
        private XMPPAccount _Account;
        public XMPPAccount Account
        {
            get => _Account;
            internal set => SetAccountProperty(value);
        }

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--


        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--
        private void SetAccountProperty(XMPPAccount value)
        {
            if (SetProperty(ref _Account, value, nameof(Account)))
            {
                BareJidText = Account.getBareJid();
            }
        }

        private void SetIsValidBareJidProperty(bool value)
        {
            if (SetProperty(ref _IsValidBareJid, value, nameof(IsValidBareJid)) && value && !(Account is null))
            {
                // Update domain and local part if needed:
                string domainPart = Utils.getJidDomainPart(BareJidText);
                if (string.Equals(Account.serverAddress, Account.user.domainPart))
                {
                    Account.serverAddress = domainPart;
                }
                Account.user.domainPart = domainPart;
                Account.user.localPart = Utils.getJidLocalPart(BareJidText);
            }
        }

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


        #endregion
    }
}
