using System.Threading.Tasks;
using Manager.Classes;
using Shared.Classes;
using XMPP_API.Classes;
using XMPP_API.Classes.Network;
using XMPP_API.Classes.Network.Events;

namespace UWPX_UI_Context.Classes.DataTemplates
{
    public sealed class AccountDataTemplate: AbstractDataTemplate
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        private Client _Client;
        public Client Client
        {
            get => _Client;
            set => SetClient(value);
        }
        private bool _Enabled;
        public bool Enabled
        {
            get => _Enabled;
            set => SetEnabled(value);
        }
        private string _ErrorText;
        public string ErrorText
        {
            get => _ErrorText;
            set => SetProperty(ref _ErrorText, value);
        }
        private ConnectionState _State;
        public ConnectionState State
        {
            get => _State;
            set => SetProperty(ref _State, value);
        }
        private ConnectionError _ConnectionError;
        public ConnectionError ConnectionError
        {
            get => _ConnectionError;
            set => SetProperty(ref _ConnectionError, value);
        }

        public object lastConnectionError { get; private set; }

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--


        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--
        private void SetClient(Client value)
        {
            if (!(Client is null))
            {
                Client.xmppClient.ConnectionStateChanged -= ConnectionStateChanged;
            }

            if (SetProperty(ref _Client, value, nameof(Client)))
            {
                State = value.xmppClient.getConnetionState();
                Enabled = !value.dbAccount.disabled;
                ProcessLastConnectionError(value.xmppClient.getLastConnectionError());
            }

            if (!(Client is null))
            {
                Client.xmppClient.ConnectionStateChanged += ConnectionStateChanged;
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
            if (Client is null || !value == Client.dbAccount.disabled)
            {
                return;
            }

            Task.Run(async () =>
            {
                Client.dbAccount.disabled = !value;
                Client.dbAccount.Save();

                if (value && !Client.xmppClient.isConnected())
                {
                    _ = Client.xmppClient.connectAsync();
                }
                else if (value && Client.xmppClient.isConnected())
                {
                    await Client.xmppClient.disconnectAsync();
                }
            });
        }

        private void ProcessLastConnectionError(object lastConnectionError)
        {
            this.lastConnectionError = lastConnectionError;
            if (lastConnectionError is ConnectionError connectionError)
            {
                ConnectionError = connectionError;
                switch (connectionError.ERROR_CODE)
                {
                    case ConnectionErrorCode.UNKNOWN:
                        ErrorText = connectionError.ERROR_MESSAGE ?? "";
                        break;

                    case ConnectionErrorCode.SOCKET_ERROR:
                        ErrorText = connectionError.SOCKET_ERROR.ToString();
                        break;

                    default:
                        ErrorText = connectionError.ERROR_CODE.ToString();
                        break;
                }
            }
            else
            {
                ErrorText = "";
            }
        }

        #endregion

        #region --Misc Methods (Protected)--


        #endregion
        //--------------------------------------------------------Events:---------------------------------------------------------------------\\
        #region --Events--
        private void ConnectionStateChanged(XMPPClient client, ConnectionStateChangedEventArgs args)
        {
            State = args.newState;

            if (args.newState == ConnectionState.ERROR)
            {
                ProcessLastConnectionError(client.getLastConnectionError());
            }
            else if (args.newState == ConnectionState.CONNECTED)
            {
                ProcessLastConnectionError(null);
            }
        }

        #endregion
    }
}
