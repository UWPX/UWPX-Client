using System.ComponentModel;
using System.Threading.Tasks;
using Logging;
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
        public void UpdatePush(string node, string secret, string pushServerBareJid)
        {
            // Already up to date?
            if (!(dbAccount.push is null) && string.Equals(node, dbAccount.push.node) && string.Equals(secret, dbAccount.push.secret) && string.Equals(pushServerBareJid, dbAccount.push.bareJid))
            {
                Logger.Info($"No need to update push information for '{dbAccount.bareJid}'. Already up to date.");
            }
            else
            {
                // Update the DB:
                if (dbAccount.push is null)
                {
                    dbAccount.push = new PushAccountModel();
                }
                dbAccount.push.node = node;
                dbAccount.push.secret = secret;
                dbAccount.push.published = false;
                dbAccount.push.bareJid = pushServerBareJid;
                dbAccount.Update();

                // Update the client:
                XMPPAccount account = xmppClient.getXMPPAccount();
                account.pushEnabled = true;
                account.pushPublished = false;
                account.pushNode = node;
                account.pushNodeSecret = secret;
                account.pushServerBareJid = pushServerBareJid;
            }

            if (Settings.GetSettingBoolean(SettingsConsts.PUSH_ENABLED) && !dbAccount.push.published)
            {
                Task.Run(async () => await xmppClient.tryEnablePushAsync());
            }
        }

        public void DisablePush()
        {
            // Already disabled?
            if (dbAccount.push.enabled)
            {
                // Update the DB:
                dbAccount.push.enabled = false;
                dbAccount.push.published = false;
                dbAccount.Update();

                // Update the client:
                XMPPAccount account = xmppClient.getXMPPAccount();
                account.pushEnabled = false;
                account.pushPublished = false;
            }

            if (!dbAccount.push.published)
            {
                Task.Run(async () => await xmppClient.tryDisablePushAsync());
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

                    case nameof(XMPPAccount.pushPublished) when (account.pushPublished != dbAccount.push.published) && !(dbAccount.push is null):
                        dbAccount.push.published = account.pushPublished;
                        dbAccount.Update();
                        break;
                }
            }
        }

        #endregion
    }
}
