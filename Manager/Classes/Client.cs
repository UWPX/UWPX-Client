using System.ComponentModel;
using Logging;
using Manager.Classes.Push;
using Shared.Classes;
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

        public readonly ClientPushManager PUSH_MANAGER;

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        public Client(AccountModel dbAccount)
        {
            this.dbAccount = dbAccount;
            xmppClient = LoadXmppClient(dbAccount);
            PUSH_MANAGER = new ClientPushManager(this);
        }

        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--


        #endregion
        //--------------------------------------------------------Misc Methods:---------------------------------------------------------------\\
        #region --Misc Methods (Public)--
        public void UpdatePush(string node, string secret, string pushServerBareJid)
        {
            // Already up to date?
            if (string.Equals(node, dbAccount.push.node) && string.Equals(secret, dbAccount.push.secret) && string.Equals(pushServerBareJid, dbAccount.push.bareJid))
            {
                Logger.Info($"No need to update push information for '{dbAccount.bareJid}'. Already up to date.");
            }
            else
            {
                // Update the DB:
                dbAccount.push.node = node;
                dbAccount.push.secret = secret;
                dbAccount.push.state = PushState.ENABLING;
                dbAccount.push.bareJid = pushServerBareJid;
                dbAccount.push.Update();

                // Ensure we update the push configuration on the XMPP server if needed:
                PUSH_MANAGER.TryUpdateClientPush();
            }
        }

        public void DisablePush()
        {
            // Already disabled?
            if (dbAccount.push.state != PushState.DISABLED && dbAccount.push.state != PushState.DISABLING)
            {
                // Update the DB:
                dbAccount.push.state = PushState.DISABLING;
                dbAccount.push.Update();

                // Ensure we update the push configuration on the XMPP server if needed:
                PUSH_MANAGER.TryUpdateClientPush();
            }
        }

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
            xmppAccount.PropertyChanged += OnXmppAccountPropertyChanged;
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
        private void OnXmppAccountPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (sender is XMPPAccount account)
            {
                switch (e.PropertyName)
                {
                    case nameof(XMPPAccount.omemoDeviceId) when account.omemoDeviceId != dbAccount.omemoInfo.deviceId:
                        dbAccount.omemoInfo.deviceId = account.omemoDeviceId;
                        dbAccount.Update();
                        break;

                    case nameof(XMPPAccount.omemoBundleInfoAnnounced) when account.omemoBundleInfoAnnounced != dbAccount.omemoInfo.bundleInfoAnnounced:
                        dbAccount.omemoInfo.bundleInfoAnnounced = account.omemoBundleInfoAnnounced;
                        dbAccount.Update();
                        break;

                    case nameof(XMPPAccount.omemoDeviceLabel) when !string.Equals(account.omemoDeviceLabel, dbAccount.omemoInfo.deviceLabel):
                        dbAccount.omemoInfo.deviceLabel = account.omemoDeviceLabel;
                        dbAccount.Update();
                        break;
                }
            }
        }

        #endregion
    }
}
