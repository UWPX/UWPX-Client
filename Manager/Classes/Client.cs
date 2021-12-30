using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Threading.Tasks;
using Logging;
using Manager.Classes.Push;
using Shared.Classes;
using Storage.Classes;
using Storage.Classes.Contexts;
using Storage.Classes.Models.Account;
using XMPP_API.Classes;
using XMPP_API.Classes.Network;
using XMPP_API.Classes.Network.XML.Messages;
using XMPP_API.Classes.Network.XML.Messages.Helper;
using XMPP_API.Classes.Network.XML.Messages.XEP_0048;
using XMPP_API.Classes.Network.XML.Messages.XEP_0060;
using XMPP_API.Classes.Network.XML.Messages.XEP_0084;

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

        /// <summary>
        /// Publishes the current XEP-0048 bookmarks to the server.
        /// </summary>
        /// <returns>True on success.</returns>
        public async Task<bool> PublishBookmarksAsync()
        {
            List<ConferenceItem> conferences;
            using (MainDbContext ctx = new MainDbContext())
            {
                conferences = ctx.GetXEP0048ConferenceItemsForAccount(xmppClient.getXMPPAccount().getBareJid());
            }
            MessageResponseHelperResult<IQMessage> result = await xmppClient.PUB_SUB_COMMAND_HELPER.setBookmars_xep_0048Async(conferences);
            if (string.Equals(result.RESULT.TYPE, IQMessage.RESULT))
            {
                return true;
            }
            if (result.RESULT is IQErrorMessage errorMessage)
            {
                Logger.Warn($"Failed to update XEP-0048 Bookmarks: {errorMessage.ERROR_OBJ}");
            }
            else
            {
                Logger.Warn($"Failed to update XEP-0048 Bookmarks: {result.RESULT.TYPE}");
            }
            return false;
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

        private async Task<ImageModel> RequestAvatarAsync(string avatarHash, string type, string bareJid, string logBareJid)
        {
            MessageResponseHelperResult<IQMessage> result = await xmppClient.PUB_SUB_COMMAND_HELPER.requestAvatarAsync(bareJid, avatarHash);
            if (result.STATE == MessageResponseHelperResultState.SUCCESS)
            {
                if (result.RESULT is AvatarResponseMessage avatar)
                {
                    if (string.IsNullOrEmpty(avatar.AVATAR_BASE_64))
                    {
                        Logger.Error($"Failed to convert the avatar with hash '{avatarHash}' for '{logBareJid}' to an image. Data node is empty.");
                        return null;
                    }

                    try
                    {
                        return new ImageModel
                        {
                            hash = avatar.HASH,
                            data = Convert.FromBase64String(avatar.AVATAR_BASE_64),
                            type = type
                        };

                    }
                    catch (Exception e)
                    {
                        Logger.Error($"Failed to convert the avatar with hash '{avatarHash}' for '{logBareJid}' to an image.", e);
                    }
                }
                else if (result.RESULT is IQErrorMessage errorMsg)
                {
                    Logger.Error($"Failed to request the avatar with hash '{avatarHash}' for '{logBareJid}' with: {errorMsg.ERROR_OBJ}");
                }
            }
            else
            {
                Logger.Error($"Failed to request the avatar with hash '{avatarHash}' for '{logBareJid}' with: {result.STATE}");
            }
            return null;
        }

        private async Task CheckAvatarSubscriptionAsync(ContactInfoModel contactInfo, string bareJid, string logBareJid)
        {
            if (!contactInfo.ShouldCheckAvatarSubscription())
            {
                Logger.Debug($"No need to check subscription state for avatar metadata node for '{logBareJid}'.");
                return;
            }

            MessageResponseHelperResult<IQMessage> result = await xmppClient.PUB_SUB_COMMAND_HELPER.requestAvatarMetadataSubscriptionAsync(bareJid);
            if (result.STATE == MessageResponseHelperResultState.SUCCESS)
            {
                if (result.RESULT is IQErrorMessage errorMsg)
                {
                    Logger.Error($"Failed to request avatar metadata subscription for '{logBareJid}' with: {errorMsg.ERROR_OBJ}");
                    contactInfo.avatarSubscriptionState = AvatarMetadataSubscriptionState.UNKNOWN;
                    contactInfo.Update();
                }
                else if (result.RESULT is PubSubSubscriptionMessage subMsg)
                {
                    contactInfo.lastAvatarUpdate = DateTime.Now;
                    contactInfo.avatarSubscriptionState = subMsg.SUBSCRIPTION == PubSubSubscriptionState.SUBSCRIBED ? AvatarMetadataSubscriptionState.SUBSCRIBED : AvatarMetadataSubscriptionState.UNKNOWN;
                    contactInfo.Update();
                    Logger.Info($"Subscribed to avatar metadata node for '{logBareJid}'.");
                }
                else
                {
                    // Should not happen:
                    Debug.Assert(false);
                }
            }
            else
            {
                Logger.Error($"Failed to request avatar metadata subscription for '{logBareJid}' with: {result.STATE}");
            }
        }

        public async Task<bool> UpdateAvatarAsync(ContactInfoModel contactInfo, AvatarMetadata metadata, string bareJid, string logBareJid)
        {
            if (!(contactInfo.avatar is null) && string.Equals(contactInfo.avatar.hash, metadata.HASH))
            {
                Logger.Info("No need to update the avatar. It has the same hash.");
                return true;
            }

            ImageModel avatar = await RequestAvatarAsync(metadata.HASH, metadata.INFOS[0].TYPE, bareJid, logBareJid);
            // Did the avatar actually change:
            if (contactInfo.avatar != avatar && (avatar is null || !avatar.Equals(contactInfo.avatar)))
            {
                ImageModel oldAvatar = contactInfo.avatar;
                using (MainDbContext ctx = new MainDbContext())
                {
                    if (!(avatar is null))
                    {
                        ctx.Add(avatar);
                    }
                    contactInfo.avatar = avatar;
                    contactInfo.lastAvatarUpdate = DateTime.Now;
                    ctx.Update(contactInfo);
                    ctx.SaveChanges(); // To prevent a FOREIGEN KEY constraint exception this order is required

                    if (!(oldAvatar is null))
                    {

                        ctx.SaveChanges();
                        ctx.Remove(oldAvatar);
                    }
                }
                Logger.Info($"Updated avatar for '{logBareJid}'.");
                return true;
            }
            return false;
        }

        public async Task CheckForAvatarUpdatesAsync(ContactInfoModel contactInfo, string bareJid, string logBareJid)
        {
            MessageResponseHelperResult<IQMessage> result = await xmppClient.PUB_SUB_COMMAND_HELPER.requestAvatarMetadataAsync(bareJid);
            if (result.STATE == MessageResponseHelperResultState.SUCCESS)
            {
                if (result.RESULT is AvatarMetadataResponseMessage metadataResponseMsg)
                {
                    AvatarMetadata metadata = metadataResponseMsg.METADATA;
                    if (contactInfo.avatar is null)
                    {
                        if (metadata.HASH is null)
                        {
                            Logger.Info($"No need to update (null) avatar for '{logBareJid}'.");
                        }
                        else if (metadata.INFOS.Count <= 0)
                        {
                            Logger.Warn($"Received avatar without valid metadata from '{logBareJid}'.");
                        }
                        else
                        {
                            // New avatar:
                            if (await UpdateAvatarAsync(contactInfo, metadata, bareJid, logBareJid))
                            {
                                await CheckAvatarSubscriptionAsync(contactInfo, bareJid, logBareJid);
                            }
                        }
                    }
                    else
                    {
                        // Avatar got removed:
                        if (metadata.HASH is null)
                        {
                            using (MainDbContext ctx = new MainDbContext())
                            {
                                contactInfo.avatar = null;
                                contactInfo.lastAvatarUpdate = DateTime.Now;
                                ctx.Update(contactInfo);
                                ctx.SaveChanges(); // To prevent a FOREIGEN KEY constraint exception this order is required
                                ctx.Remove(contactInfo.avatar);
                            }
                        }
                        else if (metadata.INFOS.Count <= 0)
                        {
                            Logger.Warn($"Received avatar without valid metadata from '{logBareJid}'.");
                        }
                        else if (string.Equals(metadata.HASH, contactInfo.avatar.hash))
                        {

                            Logger.Info($"No need to update (same hash) avatar for '{logBareJid}'.");
                        }
                        // Avatar got updated:
                        else if (await UpdateAvatarAsync(contactInfo, metadata, bareJid, logBareJid))
                        {
                            await CheckAvatarSubscriptionAsync(contactInfo, bareJid, logBareJid);
                        }

                    }
                }
            }
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
