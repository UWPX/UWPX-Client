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
            internal set { SetProperty(ref _Account, value); }
        }

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        public AddAccountPageDataTemplate()
        {
            BareJidText = "aaa@aaa.de";
        }

        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--
        private void SetBareJidText(string value)
        {
            if (SetProperty(ref _BareJidText, value, nameof(BareJidText)))
            {
                IsValidBareJid = Utils.isBareJid(value);

                // Update domain and local part if needed:
                if (IsValidBareJid && !(Account is null))
                {
                    Account.user.domainPart = Utils.getJidDomainPart(value);
                    Account.user.localPart = Utils.getJidLocalPart(value);
                    Account.serverAddress = Account.user.domainPart;
                }
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
