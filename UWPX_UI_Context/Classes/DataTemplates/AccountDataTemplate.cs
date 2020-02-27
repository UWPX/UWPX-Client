using System.Threading.Tasks;
using Data_Manager2.Classes.DBManager;
using Shared.Classes;
using XMPP_API.Classes;
using XMPP_API.Classes.Network;

namespace UWPX_UI_Context.Classes.DataTemplates
{
    public sealed class AccountDataTemplate: AbstractDataTemplate
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        private XMPPClient _Client;
        public XMPPClient Client
        {
            get => _Client;
            set => SetClient(value);
        }
        private XMPPAccount _Account;
        public XMPPAccount Account
        {
            get => _Account;
            private set => SetAccount(value);
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

        private void SetClient(XMPPClient value)
        {
            if (!(Client is null))
            {
                Client.ConnectionStateChanged -= Client_ConnectionStateChanged;
            }

            if (SetProperty(ref _Client, value, nameof(Client)))
            {
                Account = value?.getXMPPAccount();
                State = value.getConnetionState();
                ProcessLastConnectionError(value.getLastConnectionError());
            }

            if (!(Client is null))
            {
                Client.ConnectionStateChanged += Client_ConnectionStateChanged;
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
            if (Account is null || Client is null || !value == Account.disabled)
            {
                return;
            }

            Task.Run(async () =>
            {
                Account.disabled = !value;
                AccountDBManager.INSTANCE.setAccountDisabled(Account);

                if (value && !Client.isConnected())
                {
                    Client.connectAsync();
                }
                else if (value && Client.isConnected())
                {
                    await Client.disconnectAsync();
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

        private void Client_ConnectionStateChanged(XMPPClient client, XMPP_API.Classes.Network.Events.ConnectionStateChangedEventArgs args)
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
