using Shared.Classes;
using XMPP_API.Classes;
using XMPP_API.Classes.Network;

namespace UWPX_UI_Context.Classes.DataTemplates
{
    public sealed class XMPPClientDataTemplate: AbstractDataTemplate
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        private XMPPClient _Client;
        public XMPPClient Client
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
        public XMPPClientDataTemplate(XMPPClient client)
        {
            Client = client;
        }

        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--
        private void SetClientProperty(XMPPClient value)
        {
            XMPPClient oldValue = Client;
            if (SetProperty(ref _Client, value, nameof(Client)))
            {
                if (!(oldValue is null))
                {
                    oldValue.ConnectionStateChanged -= Client_ConnectionStateChanged;
                }

                if (!(value is null))
                {
                    value.ConnectionStateChanged += Client_ConnectionStateChanged;
                    Account = value.getXMPPAccount();
                    ConnectionState = value.getConnetionState();
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
        private void Client_ConnectionStateChanged(XMPPClient client, XMPP_API.Classes.Network.Events.ConnectionStateChangedEventArgs args)
        {
            ConnectionState = client.getConnetionState();
        }

        #endregion
    }
}
