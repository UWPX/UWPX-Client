using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Logging;
using Manager.Classes.Chat;
using Shared.Classes.Image;
using Shared.Classes.Threading;
using Storage.Classes;
using Storage.Classes.Contexts;
using Storage.Classes.Models.Account;
using Storage.Classes.Models.Chat;
using XMPP_API.Classes;
using XMPP_API.Classes.Network;
using XMPP_API.Classes.Network.Events;
using XMPP_API.Classes.Network.XML.Messages;
using XMPP_API.Classes.Network.XML.Messages.Helper;
using XMPP_API.Classes.Network.XML.Messages.XEP_0059;
using XMPP_API.Classes.Network.XML.Messages.XEP_0084;
using XMPP_API.Classes.Network.XML.Messages.XEP_0313;
using XMPP_API.Classes.Network.XML.Messages.XEP_0384;

namespace Manager.Classes
{
    internal enum SetupState
    {
        NOT_STARTED,
        DISCO,
        SENDING_OUTSTANDING_MESSAGES,
        REQUESTING_BOOKMARKS,
        REQUESTING_ROSTER,
        REQUESTING_MAM,
        INITIALISING_OMEMO_KEYS,
        UPDATING_AVATAR,
        DONE,
        CANCELED
    }


    public class PostClientConnectedHandler: IDisposable
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        private readonly ClientConnectionHandler ccHandler;

        private SetupState state = SetupState.NOT_STARTED;

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        public PostClientConnectedHandler(ClientConnectionHandler ccHandler)
        {
            this.ccHandler = ccHandler;
            this.ccHandler.client.xmppClient.ConnectionStateChanged += OnConnectionStateChanged;
        }

        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--


        #endregion
        //--------------------------------------------------------Misc Methods:---------------------------------------------------------------\\
        #region --Misc Methods (Public)--
        public void Dispose()
        {
            state = SetupState.CANCELED;
        }

        #endregion

        #region --Misc Methods (Private)--
        private void Continue()
        {
            switch (state)
            {
                case SetupState.NOT_STARTED:
                    Logger.Info("Starting post client connected process for: " + ccHandler.client.dbAccount.bareJid);
                    _ = DiscoAsync();
                    break;

                case SetupState.DISCO:
                    _ = SendOutsandingChatMessagesAsync();
                    break;

                case SetupState.SENDING_OUTSTANDING_MESSAGES:
                    _ = RequestRosterAsync();
                    break;

                case SetupState.REQUESTING_ROSTER:
                    _ = RequestBookmarksAsync();
                    break;

                case SetupState.REQUESTING_BOOKMARKS:
                    _ = InitOmemoAsync();
                    break;

                case SetupState.INITIALISING_OMEMO_KEYS:
                    _ = RequestMamAsync();
                    break;

                case SetupState.REQUESTING_MAM:
                    state = SetupState.UPDATING_AVATAR;
                    _ = UpdateAvatarAsync();
                    break;

                case SetupState.UPDATING_AVATAR:
                    state = SetupState.DONE;
                    Continue();
                    break;

                case SetupState.DONE:
                    Logger.Info("Post client connected process done for: " + ccHandler.client.dbAccount.bareJid);
                    return;

                case SetupState.CANCELED:
                    Logger.Info("Post client connected process canceled for: " + ccHandler.client.dbAccount.bareJid);
                    break;

                default:
                    Debug.Assert(false); // Should not happen
                    break;
            }
            Logger.Debug("PostClientConnectedHandler for " + ccHandler.client.dbAccount.bareJid + " now in state: " + state.ToString());
        }

        private async Task DiscoAsync()
        {
            state = SetupState.DISCO;
            await ccHandler.client.xmppClient.connection.DISCO_HELPER.DiscoAsync();
            Continue();
        }

        private async Task RequestBookmarksAsync()
        {
            state = SetupState.REQUESTING_BOOKMARKS;
            await ccHandler.client.xmppClient.PUB_SUB_COMMAND_HELPER.requestBookmars_xep_0048Async();
            Continue();
        }

        private async Task RequestRosterAsync()
        {
            state = SetupState.REQUESTING_ROSTER;
            await ccHandler.client.xmppClient.GENERAL_COMMAND_HELPER.sendRequestRosterMessageAsync();
            Continue();
        }

        private void HandleMamMessages(MamResult result)
        {
            Task.Run(async () =>
            {
                foreach (QueryArchiveResultMessage message in result.RESULTS)
                {
                    foreach (AbstractMessage abstractMessage in message.CONTENT)
                    {
                        if (abstractMessage is MessageMessage msg)
                        {
                            await ccHandler.HandleNewChatMessageAsync(msg);
                        }
                        else
                        {
                            Logger.Warn("MAM contained message of type: " + abstractMessage.GetType());
                        }
                    }
                }
            });
        }

        private async Task RequestMamAsync()
        {
            state = SetupState.REQUESTING_MAM;

            // Skip MAM retrieval in case it has been disabled in the settings:
            if (Settings.GetSettingBoolean(SettingsConsts.DISABLE_MAM))
            {
                Logger.Info("MAM request disabled. Skipping.");
                Continue();
            }

            if (!ccHandler.client.xmppClient.connection.DISCO_HELPER.HasFeature(Consts.XML_XEP_0313_NAMESPACE, ccHandler.client.dbAccount.bareJid))
            {
                Logger.Info("No need to request MAM for " + ccHandler.client.dbAccount.bareJid + " - not supported.");
                Continue();
                return;
            }

            bool extendedMamSupport = ccHandler.client.xmppClient.connection.DISCO_HELPER.HasFeature(Consts.XML_XEP_0313_EXTENDED_NAMESPACE, ccHandler.client.dbAccount.bareJid);
            Logger.Info("Extended MAM support for " + ccHandler.client.dbAccount.bareJid + ": " + extendedMamSupport);

            // Check for how many days into the past we should request the MAM:
            int maxMamBacklogDays = Settings.GetSettingInt(SettingsConsts.MAM_REQUEST_DAYS);
            DateTime maxMamBackglogDate;
            if (maxMamBacklogDays < 0)
            {
                maxMamBackglogDate = DateTime.MinValue;
            }
            else
            {
                maxMamBackglogDate = DateTime.Now.AddDays(-maxMamBacklogDays);
            }

            // Get the oldest message we have received:
            DateTime lastMsgDate = ccHandler.client.dbAccount.mamRequest.lastMsgDate;

            if (lastMsgDate != DateTime.MaxValue && lastMsgDate <= maxMamBackglogDate)
            {
                Logger.Info($"No need to request MAM since we already requested the maximum backlog of {maxMamBacklogDays} days.");
                return;
            }

            // Request only 30 messages at the time:
            Set rsm = new Set
            {
                max = 30
            };
            QueryFilter filter = new QueryFilter();
            if (extendedMamSupport)
            {
                // Only extended MAM supports setting the 'after-id' property.
                // Reference: https://xmpp.org/extensions/xep-0313.html#support
                if (!(ccHandler.client.dbAccount.mamRequest.lastMsgId is null))
                {
                    filter.AfterId(ccHandler.client.dbAccount.mamRequest.lastMsgId);
                }
            }
            else
            {
                // Fallback for servers not supporting 'urn:xmpp:mam:2#extended'.
                if (lastMsgDate != DateTime.MaxValue)
                {
                    filter.End(lastMsgDate);
                }
            }
            if (maxMamBackglogDate != DateTime.MinValue)
            {
                filter.Start(maxMamBackglogDate);
            }

            // Request MAM:
            bool done = false;
            int iteration = 1;
            while (!done)
            {
                MessageResponseHelperResult<MamResult> result = await RequestMamWithRetry(filter, rsm, 2);
                if (result.STATE == MessageResponseHelperResultState.SUCCESS)
                {
                    if (result.RESULT.RESULTS.Count > 0)
                    {
                        HandleMamMessages(result.RESULT);

                        // Update the MAM request entry:
                        lastMsgDate = GetLastMessageDate(result.RESULT);
                        if (ccHandler.client.dbAccount.mamRequest.lastMsgDate > lastMsgDate)
                        {
                            ccHandler.client.dbAccount.mamRequest.lastMsgId = result.RESULT.FIRST;
                            ccHandler.client.dbAccount.mamRequest.lastMsgDate = lastMsgDate;
                        }

                        // Request the next page:
                        rsm.after = result.RESULT.LAST;

                        Logger.Info("MAM request for " + ccHandler.client.dbAccount.bareJid + " received " + result.RESULT.RESULTS.Count + " messages in iteration " + iteration + '.');
                        Logger.Debug("First: " + result.RESULT.RESULTS[result.RESULT.RESULTS.Count - 1].QUERY_ID + " Last: " + result.RESULT.RESULTS[0].QUERY_ID);
                    }
                    if (result.RESULT.COMPLETE || result.RESULT.RESULTS.Count <= 0)
                    {
                        done = true;
                        Logger.Info("MAM requested for " + ccHandler.client.dbAccount.bareJid);
                    }
                    ++iteration;
                }
                else if (state == SetupState.REQUESTING_MAM)
                {
                    done = true;
                    Logger.Warn("Failed to request MAM archive for " + ccHandler.client.dbAccount.bareJid + " with: " + result.STATE);
                }
                else
                {
                    Logger.Error($"Failed to request MAM in iteration {iteration} for '{ccHandler.client.dbAccount.bareJid}' with {result.STATE}.");
                    Continue();
                    return;
                }
            }

            ccHandler.client.dbAccount.mamRequest.lastUpdate = DateTime.Now;
            ccHandler.client.dbAccount.mamRequest.Update();
            Continue();
        }

        private DateTime GetLastMessageDate(MamResult result)
        {
            return result.RESULTS.Count > 0 ? result.RESULTS.Last().DELAY : DateTime.Now;
        }

        private async Task<MessageResponseHelperResult<MamResult>> RequestMamWithRetry(QueryFilter filter, Set rsm, int numOfTries)
        {
            MessageResponseHelperResult<MamResult> result = await ccHandler.client.xmppClient.GENERAL_COMMAND_HELPER.requestMamAsync(filter, rsm);
            if (result.STATE == MessageResponseHelperResultState.SUCCESS || numOfTries <= 0 || state != SetupState.REQUESTING_MAM)
            {
                return result;
            }
            Logger.Warn($"MAM request failed for '{ccHandler.client.dbAccount.bareJid}' failed with {result.STATE}. Retrying...");
            return await RequestMamWithRetry(filter, rsm, --numOfTries);
        }

        private async Task InitOmemoAsync()
        {
            state = SetupState.INITIALISING_OMEMO_KEYS;
            OmemoHelper omemoHelper = ccHandler.client.xmppClient.getOmemoHelper();
            if (!(omemoHelper is null))
            {
                await omemoHelper.initAsync();
            }
            else
            {
                Logger.Warn("Failed to initialize OMEMO handler since it is null.");
            }
            Continue();
        }

        private async Task<ImageModel> RequestAvatarAsync(string avatarHash, string type)
        {
            MessageResponseHelperResult<IQMessage> result = await ccHandler.client.xmppClient.PUB_SUB_COMMAND_HELPER.requestAvatarAsync(null, avatarHash);
            if (result.STATE == MessageResponseHelperResultState.SUCCESS)
            {
                if (result.RESULT is AvatarResponseMessage avatar)
                {
                    try
                    {
                        return new ImageModel
                        {
                            hash = avatar.HASH,
                            lastUpdate = DateTime.Now,
                            data = Convert.FromBase64String(avatar.AVATAR_BASE_64),
                            type = type
                        };

                    }
                    catch (Exception e)
                    {
                        Logger.Error($"Failed to requets the avatar with hash '{avatarHash}' for '{ccHandler.client.dbAccount.bareJid}' to an image.", e);
                    }
                }
                else if (result.RESULT is IQErrorMessage errorMsg)
                {
                    Logger.Error($"Failed to requets the avatar with hash '{avatarHash}' for '{ccHandler.client.dbAccount.bareJid}' with: {errorMsg.ERROR_OBJ}");
                }
            }
            else
            {
                Logger.Error($"Failed to requets the avatar with hash '{avatarHash}' for '{ccHandler.client.dbAccount.bareJid}' with: {result.STATE}");
            }
            return null;
        }

        private async Task UpdateAvatarAsync()
        {
            state = SetupState.UPDATING_AVATAR;
            MessageResponseHelperResult<IQMessage> result = await ccHandler.client.xmppClient.PUB_SUB_COMMAND_HELPER.requestAvatarMetadataAsync(null);
            if (result.STATE == MessageResponseHelperResultState.SUCCESS)
            {
                if (result.RESULT is AvatarMetadataResponseMessage metadataResponseMsg)
                {
                    if (ccHandler.client.dbAccount.contactInfo.avatar is null)
                    {
                        if (metadataResponseMsg.HASH is null)
                        {
                            Logger.Info($"No need to update (null) avatar for '{ccHandler.client.dbAccount.bareJid}'.");
                        }
                        else if (metadataResponseMsg.INFOS.Count <= 0)
                        {
                            Logger.Warn($"Received avatar withount valid metadata from '{ccHandler.client.dbAccount.bareJid}'.");
                        }
                        else {
                            // New avatar:
                            ImageModel avatar = await RequestAvatarAsync(metadataResponseMsg.HASH, metadataResponseMsg.INFOS[0].TYPE);
                            using (MainDbContext ctx = new MainDbContext())
                            {
                                ctx.Add(avatar);
                                ccHandler.client.dbAccount.contactInfo.avatar = avatar;
                                ctx.Update(ccHandler.client.dbAccount.contactInfo);
                            }
                        }
                    }
                    else
                    {
                        // Avatar got removed:
                        if (metadataResponseMsg.HASH is null)
                        {
                            using (MainDbContext ctx = new MainDbContext())
                            {
                                ctx.Remove(ccHandler.client.dbAccount.contactInfo.avatar);
                                ccHandler.client.dbAccount.contactInfo.avatar = null;
                                ctx.Update(ccHandler.client.dbAccount.contactInfo);
                            }
                        }
                        else if (metadataResponseMsg.INFOS.Count <= 0)
                        {
                            Logger.Warn($"Received avatar withount valid metadata from '{ccHandler.client.dbAccount.bareJid}'.");
                        }
                        else if (string.Equals(metadataResponseMsg.HASH, ccHandler.client.dbAccount.contactInfo.avatar.hash))
                        {

                            Logger.Info($"No need to update (same hash) avatar for '{ccHandler.client.dbAccount.bareJid}'.");
                        }
                        // Avatar got updated:
                        else
                        {
                            ImageModel avatar = await RequestAvatarAsync(metadataResponseMsg.HASH, metadataResponseMsg.INFOS[0].TYPE);
                            if (!(avatar is null))
                            {
                                using (MainDbContext ctx = new MainDbContext())
                                {
                                    ctx.Remove(ccHandler.client.dbAccount.contactInfo.avatar);
                                    ctx.Add(avatar);
                                    ccHandler.client.dbAccount.contactInfo.avatar = avatar;
                                    ctx.Update(ccHandler.client.dbAccount.contactInfo);
                                }
                            }
                        }
                    }
                }
            }
            Continue();
        }

        private void Start()
        {
            state = SetupState.NOT_STARTED;
            Continue();
        }

        private void Cancel()
        {
            state = SetupState.CANCELED;
        }

        /// <summary>
        /// Sends all outstanding chat messages for the current client.
        /// </summary>
        private async Task SendOutsandingChatMessagesAsync()
        {
            state = SetupState.SENDING_OUTSTANDING_MESSAGES;
            List<Tuple<ChatMessageModel, string>> toSend;
            using (MainDbContext ctx = new MainDbContext())
            {
                toSend = ctx.ChatMessages.Where(m => m.state == MessageState.SENDING).Include(ctx.GetIncludePaths(typeof(ChatMessageModel))).Join(ctx.Chats, m => m.chatId, c => c.id, (m, c) => new { m, c.bareJid }).Where(t => string.Equals(t.bareJid, ccHandler.client.dbAccount.bareJid)).Select(t => new Tuple<ChatMessageModel, string>(t.m, t.bareJid)).ToList();
            }
            Logger.Info("Sending " + toSend.Count() + " outstanding chat messages for: " + ccHandler.client.dbAccount.bareJid);
            await SendOutsandingChatMessagesAsync(toSend);
            Logger.Info("Finished sending outstanding chat messages for: " + ccHandler.client.dbAccount.bareJid);

            List<Tuple<ChatMessageModel, string>> toEncrypt;
            using (MainDbContext ctx = new MainDbContext())
            {
                toEncrypt = ctx.ChatMessages.Where(m => m.state == MessageState.TO_ENCRYPT).Include(ctx.GetIncludePaths(typeof(ChatMessageModel))).Join(ctx.Chats, m => m.chatId, c => c.id, (m, c) => new { m, c.bareJid }).Where(t => string.Equals(t.bareJid, ccHandler.client.dbAccount.bareJid)).Select(t => new Tuple<ChatMessageModel, string>(t.m, t.bareJid)).ToList();
            }
            Logger.Info("Sending " + toEncrypt.Count() + " outstanding OMEMO chat messages for: " + ccHandler.client.dbAccount.bareJid);
            await SendOutsandingChatMessagesAsync(toEncrypt);
            Logger.Info("Finished sending outstanding OMEMO chat messages for: " + ccHandler.client.dbAccount.bareJid);
            Continue();
        }

        /// <summary>
        /// Sends all chat messages passed to it.
        /// </summary>
        /// <param name="messages">A list of chat messages to send. They SHOULD be sorted by their chatId for optimal performance.</param>
        private async Task SendOutsandingChatMessagesAsync(List<Tuple<ChatMessageModel, string>> messages)
        {
            ChatModel chat = null;
            foreach (Tuple<ChatMessageModel, string> msgDb in messages)
            {
                MessageMessage msg = msgDb.Item1.ToMessageMessage(ccHandler.client.dbAccount.fullJid.FullJid(), msgDb.Item2);

                if (chat is null || chat.id != msgDb.Item1.chatId)
                {
                    using (SemaLock semaLock = DataCache.INSTANCE.NewChatSemaLock())
                    {
                        chat = DataCache.INSTANCE.GetChat(msgDb.Item1.chatId, semaLock);
                    }
                }

                if (chat is null)
                {
                    Logger.Warn($"Failed to send outstanding chat message from {msgDb.Item1.fromBareJid} to '{msgDb.Item2}'. Chat does not exist any more.");
                    continue;
                }

                if (msg is OmemoEncryptedMessage omemoMsg)
                {
                    await ccHandler.client.xmppClient.sendOmemoMessageAsync(omemoMsg, msgDb.Item2, ccHandler.client.dbAccount.bareJid, ccHandler.client.dbAccount.omemoInfo.trustedKeysOnly, chat.omemoInfo.trustedKeysOnly);
                }
                else
                {
                    await ccHandler.client.xmppClient.SendAsync(msg);
                }
            }
        }

        #endregion

        #region --Misc Methods (Protected)--


        #endregion
        //--------------------------------------------------------Events:---------------------------------------------------------------------\\
        #region --Events--
        private void OnConnectionStateChanged(XMPPClient client, ConnectionStateChangedEventArgs args)
        {
            if (args.newState == ConnectionState.CONNECTED)
            {
                Start();
            }
            else
            {
                Cancel();
            }
        }

        #endregion
    }
}
