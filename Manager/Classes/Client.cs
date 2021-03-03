﻿using Shared.Classes;
using Storage.Classes;
using Storage.Classes.Models.Account;
using XMPP_API.Classes;
using XMPP_API.Classes.Network;

namespace Manager.Classes
{
    public class Client: AbstractDataTemplate
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        public XMPPClient xmppClient
        {
            get => _xmppClient;
            set => SetProperty(ref _xmppClient, value);
        }
        private XMPPClient _xmppClient;

        public AccountModel dbAccount
        {
            get => _dbAccount;
            set => SetProperty(ref _dbAccount, value);
        }
        private AccountModel _dbAccount;

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
        /// <summary>
        /// Loads one specific XMPPAccount and subscribes to all its events.
        /// </summary>
        /// <param name="account">The account which should get loaded.</param>
        /// <returns>Returns a new XMPPClient instance.</returns>
        private XMPPClient LoadXmppClient(AccountModel account)
        {
            XMPPAccount xmppAccount = account.ToXMPPAccount();
            Vault.LoadPassword(xmppAccount);
            XMPPClient client = new XMPPClient(xmppAccount);
            client.connection.TCP_CONNECTION.disableConnectionTimeout = Settings.GetSettingBoolean(SettingsConsts.DEBUG_DISABLE_TCP_TIMEOUT);
            client.connection.TCP_CONNECTION.disableTlsUpgradeTimeout = Settings.GetSettingBoolean(SettingsConsts.DEBUG_DISABLE_TLS_TIMEOUT);

            // Enable OMEMO:
            EnableOmemo(account, client);
            return client;
        }

        /// <summary>
        /// Enables OMEMO by setting all OMEMO keys.
        /// </summary>
        private void EnableOmemo(AccountModel account, XMPPClient client)
        {
            XMPPAccount xmppAccount = client.getXMPPAccount();
            xmppAccount.omemoDeviceId = account.omemoInfo.deviceId;
            xmppAccount.omemoDeviceLabel = account.omemoInfo.deviceLabel;
            xmppAccount.omemoIdentityKey = account.omemoInfo.identityKey;
            xmppAccount.omemoSignedPreKey = account.omemoInfo.signedPreKey;
            xmppAccount.OMEMO_PRE_KEYS.Clear();
            xmppAccount.OMEMO_PRE_KEYS.AddRange(account.omemoInfo.preKeys);
            xmppAccount.omemoBundleInfoAnnounced = account.omemoInfo.bundleInfoAnnounced;
            client.enableOmemo(new OmemoStorage(account));
        }

        #endregion

        #region --Misc Methods (Protected)--


        #endregion
        //--------------------------------------------------------Events:---------------------------------------------------------------------\\
        #region --Events--


        #endregion
    }
}