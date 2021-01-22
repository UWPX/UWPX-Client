using Storage.Classes;
using Storage.Classes.Models.Account;
using XMPP_API.Classes;

namespace Manager.Classes.Client
{
    public class Client
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        public readonly XMPPClient xmppClient;
        public readonly AccountModel dbAccount;

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        public Client(AccountModel dbAccount)
        {
            this.dbAccount = dbAccount;
            xmppClient = LoadXmppClient(dbAccount);
        }

        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--


        #endregion
        //--------------------------------------------------------Misc Methods:---------------------------------------------------------------\\
        #region --Misc Methods (Public)--


        #endregion

        #region --Misc Methods (Private)--
        private static XMPPClient LoadXmppClient(AccountModel dbAccount)
        {
            XMPPClient client = new XMPPClient(dbAccount.ToXMPPAccount());
            client.connection.TCP_CONNECTION.disableConnectionTimeout = Settings.GetSettingBoolean(SettingsConsts.DEBUG_DISABLE_TCP_TIMEOUT);
            client.connection.TCP_CONNECTION.disableTlsUpgradeTimeout = Settings.GetSettingBoolean(SettingsConsts.DEBUG_DISABLE_TLS_TIMEOUT);
            return client;
        }

        #endregion

        #region --Misc Methods (Protected)--


        #endregion
        //--------------------------------------------------------Events:---------------------------------------------------------------------\\
        #region --Events--


        #endregion
    }
}
