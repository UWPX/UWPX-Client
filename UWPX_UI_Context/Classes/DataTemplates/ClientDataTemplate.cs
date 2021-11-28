using Manager.Classes;
using Shared.Classes;
using XMPP_API.Classes;
using XMPP_API.Classes.Network;
using XMPP_API.Classes.Network.Events;

namespace UWPX_UI_Context.Classes.DataTemplates
{
    public sealed class ClientDataTemplate: AbstractDataTemplate
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        private Client _Client;
        public Client Client
        {
            get => _Client;
            set => SetClientProperty(value);
        }

        private ConnectionState _ConnectionState;
        public ConnectionState ConnectionState
        {
            get => _ConnectionState;
            set => SetProperty(ref _ConnectionState, value);
        }

        private XMPPAccount _Account;
        public XMPPAccount Account
        {
            get => _Account;
            set => SetProperty(ref _Account, value);
        }

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        public ClientDataTemplate(Client client)
        {
            Client = client;
        }

        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--
        private void SetClientProperty(Client value)
        {
            Client oldValue = Client;
            if (SetProperty(ref _Client, value, nameof(Client)))
            {
                if (oldValue is not null)
                {
                    oldValue.xmppClient.ConnectionStateChanged -= Client_ConnectionStateChanged;
                }

                if (value is not null)
                {
                    value.xmppClient.ConnectionStateChanged += Client_ConnectionStateChanged;
                    Account = value.xmppClient.getXMPPAccount();
                    ConnectionState = value.xmppClient.getConnetionState();
                }
                else
                {
                    Account = null;
                    ConnectionState = ConnectionState.DISCONNECTED;
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
        private void Client_ConnectionStateChanged(XMPPClient client, ConnectionStateChangedEventArgs args)
        {
            ConnectionState = client.getConnetionState();
        }

        #endregion
    }
}
