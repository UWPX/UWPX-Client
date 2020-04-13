using Shared.Classes;
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
                ParserStats = accountNew.Client.getMessageParserStats();
                MsgCarbonsState = accountNew.Account.CONNECTION_INFO.msgCarbonsState;
                PushState = accountNew.Account.CONNECTION_INFO.pushState;
                SocketInfo = accountNew.Account.CONNECTION_INFO.socketInfo;
                TlsConnected = accountNew.Account.CONNECTION_INFO.tlsConnected;
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
            account.Account.CONNECTION_INFO.PropertyChanged -= CONNECTION_INFO_PropertyChanged;
        }

        private void Subscribe(AccountDataTemplate account)
        {
            account.Account.CONNECTION_INFO.PropertyChanged += CONNECTION_INFO_PropertyChanged;
        }

        #endregion

        #region --Misc Methods (Protected)--


        #endregion
        //--------------------------------------------------------Events:---------------------------------------------------------------------\\
        #region --Events--
        private void CONNECTION_INFO_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (sender is ConnectionInformation connectionInfo)
            {
                switch (e.PropertyName)
                {
                    case nameof(connectionInfo.msgCarbonsState):
                        MsgCarbonsState = connectionInfo.msgCarbonsState;
                        break;

                    case nameof(connectionInfo.pushState):
                        PushState = connectionInfo.pushState;
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

        #endregion
    }
}
