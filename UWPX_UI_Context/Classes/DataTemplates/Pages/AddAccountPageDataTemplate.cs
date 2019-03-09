using Shared.Classes;
using XMPP_API.Classes;
using XMPP_API.Classes.Network;

namespace UWPX_UI_Context.Classes.DataTemplates.Pages
{
    public sealed class AddAccountPageDataTemplate : AbstractDataTemplate
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        private string _BareJidText;
        public string BareJidText
        {
            get { return _BareJidText; }
            set { SetBareJidText(value); }
        }
        private bool _IsValidBareJid;
        public bool IsValidBareJid
        {
            get { return _IsValidBareJid; }
            set { SetProperty(ref _IsValidBareJid, value); }
        }
        private XMPPAccount _Account;
        public XMPPAccount Account
        {
            get { return _Account; }
            internal set { SetAccountProperty(value); }
        }

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--


        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--
        private void SetBareJidText(string value)
        {
            value = value.ToLowerInvariant();
            if (SetProperty(ref _BareJidText, value, nameof(BareJidText)))
            {
                IsValidBareJid = Utils.isBareJid(value);

                // Update domain and local part if needed:
                if (IsValidBareJid && !(Account is null))
                {
                    if (string.Equals(Account.serverAddress, Account.user.domainPart))
                    {
                        Account.serverAddress = Utils.getJidDomainPart(value);
                    }
                    Account.user.domainPart = Account.serverAddress;
                    Account.user.localPart = Utils.getJidLocalPart(value);
                }
            }
        }

        private void SetAccountProperty(XMPPAccount value)
        {
            if (SetProperty(ref _Account, value, nameof(Account)))
            {
                BareJidText = Account.getBareJid();
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
