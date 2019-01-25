using Data_Manager2.Classes;
using Data_Manager2.Classes.DBManager;
using Shared.Classes;
using System.Threading.Tasks;
using XMPP_API.Classes;
using XMPP_API.Classes.Network;

namespace UWPX_UI_Context.Classes.DataTemplates
{
    public sealed class AccountDataTemplate : AbstractDataTemplate
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        private XMPPAccount _Account;
        public XMPPAccount Account
        {
            get { return _Account; }
            set { SetAccount(value); }
        }
        private bool _Enabled;
        public bool Enabled
        {
            get { return _Enabled; }
            set { SetEnabled(value); }
        }

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--


        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--
        private void SetAccount(XMPPAccount value)
        {
            if (!(Account is null))
            {
                Account.PropertyChanged -= Account_PropertyChanged;
            }
            SetProperty(ref _Account, value, nameof(Account));
            Enabled = !value.disabled;
            if (!(Account is null))
            {
                Account.PropertyChanged += Account_PropertyChanged;
            }
        }

        private void SetEnabled(bool value)
        {
            if (SetProperty(ref _Enabled, value, nameof(Enabled)))
            {
                OnEnabledChanged(value);
            }
        }

        #endregion
        //--------------------------------------------------------Misc Methods:---------------------------------------------------------------\\
        #region --Misc Methods (Public)--


        #endregion

        #region --Misc Methods (Private)--
        private void OnEnabledChanged(bool value)
        {
            if (Account is null)
            {
                return;
            }

            if (!value != Account.disabled)
            {
                Account.disabled = !value;
                AccountDBManager.INSTANCE.setAccountDisabled(Account);
            }

            foreach (XMPPClient client in ConnectionHandler.INSTANCE.getClients())
            {
                if (string.Equals(client.getXMPPAccount().getIdAndDomain(), Account.getIdAndDomain()))
                {
                    Task.Run(async () =>
                    {
                        if (value && !client.isConnected())
                        {
                            await client.disconnectAsync();
                        }
                        else if (value && !client.isConnected())
                        {
                            client.connect();
                        }
                    });
                    break;
                }
            }
        }

        #endregion

        #region --Misc Methods (Protected)--


        #endregion
        //--------------------------------------------------------Events:---------------------------------------------------------------------\\
        #region --Events--
        private void Account_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (sender is XMPPAccount account)
            {
                if (string.Equals(e.PropertyName, nameof(account.disabled)))
                {
                    Enabled = !account.disabled;
                }
            }
        }

        #endregion
    }
}
