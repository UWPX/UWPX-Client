using System.ComponentModel;
using Shared.Classes;
using Storage.Classes.Models.Account;
using Windows.Networking.Sockets;
using Windows.UI.Xaml;
using XMPP_API.Classes.Network;
using XMPP_API.Classes.Network.XML;

namespace UWPX_UI_Context.Classes.DataTemplates.Controls
{
    public sealed class AccountInfoGeneralControlDataTemplate: AbstractDataTemplate
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        private MessageParserStats _ParserStats;
        public MessageParserStats ParserStats
        {
            get => _ParserStats;
            set => SetProperty(ref _ParserStats, value);
        }
        private bool _TlsConnected;
        public bool TlsConnected
        {
            get => _TlsConnected;
            set => SetProperty(ref _TlsConnected, value);
        }
        private MessageCarbonsState _MsgCarbonsState;
        public MessageCarbonsState MsgCarbonsState
        {
            get => _MsgCarbonsState;
            set => SetProperty(ref _MsgCarbonsState, value);
        }
        private PushState _PushState;
        public PushState PushState
        {
            get => _PushState;
            set => SetProperty(ref _PushState, value);
        }
        private StreamSocketInformation _SocketInfo;
        public StreamSocketInformation SocketInfo
        {
            get => _SocketInfo;
            set => SetProperty(ref _SocketInfo, value);
        }

        private bool _DebugSettingsEnabled;
        public bool DebugSettingsEnabled
        {
            get => _DebugSettingsEnabled;
            set => SetProperty(ref _DebugSettingsEnabled, value);
        }

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--


        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--


        #endregion
        //--------------------------------------------------------Misc Methods:---------------------------------------------------------------\\
        #region --Misc Methods (Public)--
        public void UpdateViewModel(DependencyPropertyChangedEventArgs e)
        {
            if (e.OldValue is AccountDataTemplate accountOld)
            {
                Unsubscribe(accountOld);
            }

            if (e.NewValue is AccountDataTemplate accountNew)
            {
                Subscribe(accountNew);
                ParserStats = accountNew.Client.xmppClient.getMessageParserStats();
                MsgCarbonsState = accountNew.Client.xmppClient.getXMPPAccount().CONNECTION_INFO.msgCarbonsState;
                PushState = accountNew.Client.dbAccount.push.state;
                SocketInfo = accountNew.Client.xmppClient.getXMPPAccount().CONNECTION_INFO.socketInfo;
                TlsConnected = accountNew.Client.xmppClient.getXMPPAccount().CONNECTION_INFO.tlsConnected;
            }
            else
            {
                ParserStats = null;
            }
        }

        #endregion

        #region --Misc Methods (Private)--
        private void Unsubscribe(AccountDataTemplate account)
        {
            account.Client.xmppClient.getXMPPAccount().CONNECTION_INFO.PropertyChanged -= OnConnectionInfoPropertyChanged;
            account.Client.dbAccount.push.PropertyChanged -= OnPushAccountModelPropertyChanged;
        }

        private void Subscribe(AccountDataTemplate account)
        {
            account.Client.xmppClient.getXMPPAccount().CONNECTION_INFO.PropertyChanged += OnConnectionInfoPropertyChanged;
            account.Client.dbAccount.push.PropertyChanged += OnPushAccountModelPropertyChanged;
        }

        #endregion

        #region --Misc Methods (Protected)--


        #endregion
        //--------------------------------------------------------Events:---------------------------------------------------------------------\\
        #region --Events--
        private void OnConnectionInfoPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (sender is ConnectionInformation connectionInfo)
            {
                switch (e.PropertyName)
                {
                    case nameof(connectionInfo.msgCarbonsState):
                        MsgCarbonsState = connectionInfo.msgCarbonsState;
                        break;

                    case nameof(connectionInfo.tlsConnected):
                        TlsConnected = connectionInfo.tlsConnected;
                        break;

                    case nameof(connectionInfo.socketInfo):
                        SocketInfo = connectionInfo.socketInfo;
                        break;
                }
            }
        }

        private void OnPushAccountModelPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (sender is PushAccountModel push && e.PropertyName.Equals(nameof(PushAccountModel.state)))
            {
                PushState = push.state;
            }
        }

        #endregion
    }
}
