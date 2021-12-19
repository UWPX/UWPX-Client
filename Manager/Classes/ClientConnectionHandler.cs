using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Logging;
using Manager.Classes.Chat;
using Manager.Classes.Toast;
using Shared.Classes.Threading;
using Storage.Classes;
using Storage.Classes.Contexts;
using Storage.Classes.Events;
using Storage.Classes.Models.Account;
using Storage.Classes.Models.Chat;
using XMPP_API.Classes;
using XMPP_API.Classes.Events;
using XMPP_API.Classes.Network;
using XMPP_API.Classes.Network.Events;
using XMPP_API.Classes.Network.XML;
using XMPP_API.Classes.Network.XML.Messages;
using XMPP_API.Classes.Network.XML.Messages.XEP_0045;
using XMPP_API.Classes.Network.XML.Messages.XEP_0048;
using XMPP_API.Classes.Network.XML.Messages.XEP_0184;
using XMPP_API.Classes.Network.XML.Messages.XEP_0249;
using XMPP_API.Classes.Network.XML.Messages.XEP_0384;
using XMPP_API.Classes.Network.XML.Messages.XEP_0384.Session;

namespace Manager.Classes
{
    public class ClientConnectionHandler: IAsyncDisposable
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        public readonly Client client;
        private readonly PostClientConnectedHandler postClientConnectedHandler;

        public event ClientConnectedHandler ClientConnected;
        public delegate void ClientConnectedHandler(ClientConnectionHandler handler, ClientConnectedEventArgs args);

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        public ClientConnectionHandler(AccountModel dbAccount)
        {
            client = new Client(dbAccount);
            postClientConnectedHandler = new PostClientConnectedHandler(this);

            // Ensure no event gets bound multiple times:
            UnsubscribeFromEvents();
            SubscribeToEvents();
        }

        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--
        /// <summary>
        /// Updates the <see cref="XMPPAccount"/> for current <see cref="XMPPClient"/>.
        /// </summary>
        public void SetAccount(XMPPAccount account)
        {
            client.xmppClient.setAccount(account);
        }

        #endregion
        //--------------------------------------------------------Misc Methods:---------------------------------------------------------------\\
        #region --Misc Methods (Public)--
        public ValueTask DisposeAsync()
        {
            UnsubscribeFromEvents();
            return DisconnectAsync();
        }

        /// <summary>
        /// Connects the XMPP client in case it is not already connected or connecting.
        /// </summary>
        public void Connect()
        {
            switch (client.xmppClient.getConnetionState())
            {
                case ConnectionState.DISCONNECTED:
                case ConnectionState.DISCONNECTING:
                case ConnectionState.ERROR:
                    Task.Run(() => ConnectAsync()).ContinueWith((Task prev) =>
                    {
                        if (prev.Exception != null)
                        {
                            Logger.Error("Error during client.connect() - ClientConnectionHandler!", prev.Exception);
                        }
                    });
                    break;

                default:
                    break;
            }
        }

        /// <summary>
        /// Connects the client.
        /// </summary>
        public Task ConnectAsync()
        {
            return client.xmppClient.connectAsync();
        }

        /// <summary>
        /// Disconnects the client.
        /// </summary>
        public Task DisconnectAsync()
        {
            return client.xmppClient.disconnectAsync();
        }

        /// <summary>
        /// Reconnects the client.
        /// </summary>
        public Task ReconnectAsync()
        {
            return client.xmppClient.reconnectAsync();
        }

        private async Task<bool> DecryptOmemoEncryptedMessageAsync(OmemoEncryptedMessage msg, bool trustedKeysOnly)
        {
            try
            {
                await client.xmppClient.connection.omemoHelper.decryptOmemoEncryptedMessageAsync(msg, trustedKeysOnly);
                return true;
            }
            catch (Exception e)
            {
                Logger.Error("Failed to decrypt " + nameof(OmemoEncryptedMessage) + " for '" + client.dbAccount.bareJid + "' with: ", e);
                return false;
            }
        }

        public async Task HandleNewChatMessageAsync(MessageMessage msg)
        {
            // Handel MUC room subject messages:
            if (msg is MUCRoomSubjectMessage)
            {
                MucHandler.INSTANCE.OnMUCRoomSubjectMessage(msg as MUCRoomSubjectMessage);
                return;
            }

            string from = Utils.getBareJidFromFullJid(msg.getFrom());
            string to = Utils.getBareJidFromFullJid(msg.getTo());
            string chatBareJid = string.Equals(from, client.dbAccount.bareJid) ? to : from;

            SemaLock semaLock = DataCache.INSTANCE.NewChatSemaLock();
            ChatModel chat = DataCache.INSTANCE.GetChat(client.dbAccount.bareJid, chatBareJid, semaLock);
            bool newChat = chat is null;
            bool chatChanged = false;

            // Spam detection:
            if (Settings.GetSettingBoolean(SettingsConsts.SPAM_DETECTION_ENABLED))
            {
                if (Settings.GetSettingBoolean(SettingsConsts.SPAM_DETECTION_FOR_ALL_CHAT_MESSAGES) || newChat)
                {
                    if (SpamHelper.INSTANCE.IsSpam(msg.MESSAGE))
                    {
                        Logger.Warn("Received spam message from " + chatBareJid);
                        return;
                    }
                }
            }

            // Detect invalid chat messages:
            if (!string.Equals(msg.TYPE, MessageMessage.TYPE_CHAT) && !string.Equals(msg.TYPE, MessageMessage.TYPE_ERROR) && !string.Equals(msg.TYPE, MessageMessage.TYPE_GROUPCHAT))
            {
                Logger.Warn($"Received an unknown message type ('{msg.TYPE}') form '{chatBareJid}'. Dropping it.");
                return;
            }

            // Add the new chat to the DB since it's expected to be there by for example our OMEMO encryption:
            if (newChat)
            {
                chat = new ChatModel(chatBareJid, client.dbAccount)
                {
                    lastActive = msg.delay,
                    chatType = string.Equals(msg.TYPE, MessageMessage.TYPE_GROUPCHAT) ? ChatType.MUC : ChatType.CHAT,
                    isChatActive = false // Mark chat as inactive until we can be sure, it's a valid message
                };
                DataCache.INSTANCE.AddChatUnsafe(chat, client);
            }
            else
            {
                // Mark chat as active:
                chat.isChatActive = true;
                chatChanged = true;
            }
            semaLock.Dispose();

            // Check if device id is valid and if, decrypt the OMEMO messages:
            if (msg is OmemoEncryptedMessage omemoMessage)
            {
                OmemoHelper helper = client.xmppClient.getOmemoHelper();
                if (helper is null)
                {
                    OnOmemoSessionBuildError(client.xmppClient, new OmemoSessionBuildErrorEventArgs(chatBareJid, OmemoSessionBuildError.KEY_ERROR, new List<OmemoEncryptedMessage> { omemoMessage }));
                    Logger.Error("Failed to decrypt OMEMO message - OmemoHelper is null");
                    return;
                }
                else if (!await DecryptOmemoEncryptedMessageAsync(omemoMessage, !newChat && chat.omemoInfo.trustedKeysOnly))
                {
                    if (newChat)
                    {
                        // We failed to decrypt, so this chat could be spam. Delete it again...
                        DataCache.INSTANCE.DeleteChat(chat, false, false);
                        Logger.Debug($"Deleting chat '{chat.bareJid}' again, since decrypting the initial OMEMO message failed.");
                    }
                    return;
                }
                else if (omemoMessage.IS_PURE_KEY_EXCHANGE_MESSAGE)
                {
                    return;
                }
            }

            // Valid new chat, so we can change it to active now:
            chat.isChatActive = true;
            chatChanged = true;

            ChatMessageModel message = null;
            if (!newChat)
            {
                message = DataCache.INSTANCE.GetChatMessage(chat.id, msg.ID);
            }

            // Filter messages that already exist:
            // ToDo: Allow MUC messages being edited and detect it
            if (!(message is null))
            {
                Logger.Debug("Duplicate message received from '" + chatBareJid + "'. Dropping it...");
                return;
            }
            message = new ChatMessageModel(msg, chat);
            await DataCache.INSTANCE.AddChatMessageAsync(message, chat);
            if (message.isImage && !Settings.GetSettingBoolean(SettingsConsts.DISABLE_IMAGE_AUTO_DOWNLOAD))
            {
                await ConnectionHandler.INSTANCE.IMAGE_DOWNLOAD_HANDLER.StartDownloadAsync(message.image);
            }

            // Handle MUC invite messages:
            if (msg is DirectMUCInvitationMessage inviteMessage)
            {
                if (!newChat)
                {
                    Logger.Info("Ignoring received MUC direct invitation form '" + chatBareJid + "' since we already joined this MUC (" + inviteMessage.ROOM_JID + ").");
                    return;
                }
                // Ensure we add the message to the DB before we add the invite since the invite has the message as a foreign key:
                using (MainDbContext ctx = new MainDbContext())
                {
                    ctx.Add(new MucDirectInvitationModel(inviteMessage, message));
                }
            }

            bool isMUCMessage = string.Equals(MessageMessage.TYPE_GROUPCHAT, message.type);

            if (chat.lastActive.CompareTo(msg.delay) < 0)
            {
                chatChanged = true;
                chat.lastActive = msg.delay;
            }

            // Send XEP-0184 (Message Delivery Receipts) reply:
            if (msg.RECIPT_REQUESTED && !Settings.GetSettingBoolean(SettingsConsts.DONT_SEND_CHAT_MESSAGE_RECEIVED_MARKERS))
            {
                await Task.Run(async () =>
                {
                    DeliveryReceiptMessage receiptMessage = new DeliveryReceiptMessage(client.dbAccount.fullJid.FullJid(), from, msg.ID);
                    await client.xmppClient.SendAsync(receiptMessage);
                });
            }

            if (chatChanged)
            {
                chat.Update();
                chatChanged = false;
            }

            // Show toast:
            if (!chat.muted)
            {
                await Task.Run(() =>
                {
                    try
                    {
                        switch (msg.TYPE)
                        {
                            case MessageMessage.TYPE_GROUPCHAT:
                            case MessageMessage.TYPE_CHAT:
                                if (!message.isCC)
                                {
                                    // Create toast:
                                    if (message.isImage)
                                    {
                                        ToastHelper.ShowChatTextImageToast(message, chat);
                                    }
                                    else
                                    {
                                        ToastHelper.ShowChatTextToast(message, chat);
                                    }
                                }
                                break;

                            default:
                                break;
                        }
                    }
                    catch (Exception e)
                    {
                        Logger.Error("Failed to send toast notification!", e);
                    }
                });
            }
        }

        /// <summary>
        /// Updates a <see cref="Client"/> with the given <paramref name="account"/>.
        /// Handles reconnecting the account in case <see cref="AccountModel.enabled"/> is true.
        /// </summary>
        /// <param name="account">The new <see cref="AccountModel"/>.</param>
        public async Task UpdateAccountAsync(AccountModel account)
        {
            // Ensure we disconnect the old client before we update it:
            await DisconnectAsync();

            // Update it:
            client.dbAccount = account;
            XMPPAccount xmppAccount = account.ToXMPPAccount();
            Vault.LoadPassword(xmppAccount);
            SetAccount(xmppAccount);

            // Connect:
            if (client.dbAccount.enabled)
            {
                await ConnectAsync();
            }
        }

        #endregion

        #region --Misc Methods (Private)--
        private void SetOmemoChatMessagesSendFailed(IEnumerable<OmemoEncryptedMessage> messages, ChatModel chat)
        {
            foreach (OmemoEncryptedMessage omemoMsg in messages)
            {
                ChatMessageModel msg = DataCache.INSTANCE.GetChatMessage(chat.id, omemoMsg.ID);
                if (!(msg is null))
                {
                    msg.state = MessageState.ENCRYPT_FAILED;
                    msg.Update();
                }
            }
        }

        /// <summary>
        /// Called once a client enters the 'Disconnected' or 'Error' state.
        /// </summary>
        private void OnDisconnectedOrError()
        {
            IEnumerable<ChatModel> chats;
            using (SemaLock semaLock = DataCache.INSTANCE.NewChatSemaLock())
            {
                chats = DataCache.INSTANCE.GetChats(client.dbAccount.bareJid, semaLock);
            }
            foreach (ChatModel chat in chats)
            {
                chat.presence = Presence.Unavailable;
            }
            using (MainDbContext ctx = new MainDbContext())
            {
                ctx.UpdateRange(chats);
            }
            MucHandler.INSTANCE.OnClientDisconnected(client.xmppClient);
        }

        /// <summary>
        /// Called once a client enters the 'Disconnecting' state.
        /// </summary>
        private void OnDisconneting()
        {
            MucHandler.INSTANCE.OnClientDisconnecting(client.xmppClient);
        }

        /// <summary>
        /// Called once a client enters the 'Connected' state.
        /// </summary>
        private void OnConnected()
        {
            MucHandler.INSTANCE.OnClientConnected(client.xmppClient);
            ClientConnected?.Invoke(this, new ClientConnectedEventArgs(client));
        }

        private async Task AnswerPresenceProbeAsync(string from, string to, ChatModel chat, PresenceMessage msg)
        {
            PresenceMessage answer;
            if (chat is null)
            {
                Logger.Warn("Received a presence probe message for an unknown chat from: " + from + ", to: " + to);
                return;
            }
            else
            {
                switch (chat.subscription)
                {
                    case "both":
                    case "from":
                        answer = new PresenceMessage(client.dbAccount.fullJid.FullJid(), from, client.dbAccount.presence, client.dbAccount.status, client.dbAccount.presencePriorety);
                        Logger.Debug("Answered presence probe from: " + from);
                        break;

                    case "none" when chat.inRoster:
                    case "to" when chat.inRoster:
                        answer = new PresenceErrorMessage(client.dbAccount.fullJid.FullJid(), from, PresenceErrorType.FORBIDDEN);
                        Logger.Warn("Received a presence probe but chat has no subscription: " + from + ", to: " + to + " subscription: " + chat.subscription);
                        break;

                    default:
                        answer = new PresenceErrorMessage(client.dbAccount.fullJid.FullJid(), from, PresenceErrorType.NOT_AUTHORIZED);
                        Logger.Warn("Received a presence probe but chat has no subscription: " + from + ", to: " + to + " subscription: " + chat.subscription);
                        break;
                }
            }
            await client.xmppClient.SendAsync(answer);
        }

        /// <summary>
        /// Unsubscribes from all <see cref="XMPPClient"/> events.
        /// </summary>
        private void UnsubscribeFromEvents()
        {
            client.xmppClient.NewChatMessage -= OnNewChatMessage;
            client.xmppClient.NewRoosterMessage -= OnNewRoosterMessage;
            client.xmppClient.NewPresence -= OnNewPresence;
            client.xmppClient.NewChatState -= OnNewChatState;
            client.xmppClient.ConnectionStateChanged -= OnConnectionStateChanged;
            client.xmppClient.MessageSend -= OnMessageSend;
            client.xmppClient.NewBookmarksResultMessage -= OnNewBookmarksResultMessage;
            client.xmppClient.NewDeliveryReceipt -= OnNewDeliveryReceipt;
            client.xmppClient.OmemoSessionBuildError -= OnOmemoSessionBuildError;
        }

        /// <summary>
        /// Subscribes to all <see cref="XMPPClient"/> events.
        /// </summary>
        private void SubscribeToEvents()
        {
            client.xmppClient.NewChatMessage += OnNewChatMessage;
            client.xmppClient.NewRoosterMessage += OnNewRoosterMessage;
            client.xmppClient.NewPresence += OnNewPresence;
            client.xmppClient.NewChatState += OnNewChatState;
            client.xmppClient.ConnectionStateChanged += OnConnectionStateChanged;
            client.xmppClient.MessageSend += OnMessageSend;
            client.xmppClient.NewBookmarksResultMessage += OnNewBookmarksResultMessage;
            client.xmppClient.NewDeliveryReceipt += OnNewDeliveryReceipt;
            client.xmppClient.OmemoSessionBuildError += OnOmemoSessionBuildError;
            ;
        }

        #endregion

        #region --Misc Methods (Protected)--


        #endregion
        //--------------------------------------------------------Events:---------------------------------------------------------------------\\
        #region --Events--
        private void OnOmemoSessionBuildError(XMPPClient xmppClient, OmemoSessionBuildErrorEventArgs args)
        {
            Task.Run(async () =>
            {
                ChatModel chat;
                using (SemaLock semaLock = DataCache.INSTANCE.NewChatSemaLock())
                {
                    chat = DataCache.INSTANCE.GetChat(xmppClient.getXMPPAccount().getBareJid(), args.CHAT_JID, semaLock);
                }
                if (!(chat is null))
                {
                    // Add an error chat message:
                    ChatMessageModel msg = new ChatMessageModel()
                    {
                        chatId = chat.id,
                        date = DateTime.Now,
                        fromBareJid = args.CHAT_JID,
                        isCC = false,
                        isDummyMessage = false,
                        isEncrypted = false,
                        isFavorite = false,
                        isImage = false,
                        message = "Failed to encrypt and send " + args.MESSAGES.Count + " OMEMO message(s) with:\n" + args.ERROR,
                        stableId = AbstractMessage.getRandomId(),
                        state = MessageState.UNREAD,
                        type = MessageMessage.TYPE_ERROR
                    };
                    await DataCache.INSTANCE.AddChatMessageAsync(msg, chat);

                    // Set chat messages to encrypted failed:
                    SetOmemoChatMessagesSendFailed(args.MESSAGES, chat);
                }
            });
        }

        private async void OnNewChatMessage(XMPPClient xmppClient, NewChatMessageEventArgs args)
        {
            await HandleNewChatMessageAsync(args.getMessage());
        }

        private async void OnNewPresence(XMPPClient xmppClient, NewPresenceMessageEventArgs args)
        {
            string from = Utils.getBareJidFromFullJid(args.PRESENCE_MESSAGE.getFrom());

            // If received a presence message from your own account, ignore it:
            if (string.Equals(from, client.dbAccount.bareJid))
            {
                return;
            }

            ChatModel chat;
            using (SemaLock semaLock = DataCache.INSTANCE.NewChatSemaLock())
            {
                chat = DataCache.INSTANCE.GetChat(xmppClient.getXMPPAccount().getBareJid(), from, semaLock);
            }
            if (chat is null)
            {
                Logger.Warn("Received a presence message for an unknown chat from: " + from + ", to: " + client.dbAccount.bareJid);
                return;
            }

            // Answer presence probes:
            // TODO: answer those only for chats we are subscribed to.
            if (string.Equals(args.PRESENCE_MESSAGE.TYPE, "probe"))
            {
                await AnswerPresenceProbeAsync(from, client.dbAccount.bareJid, chat, args.PRESENCE_MESSAGE);
                return;
            }

            if (chat.chatType == ChatType.CHAT)
            {
                if (string.Equals(args.PRESENCE_MESSAGE.TYPE, "subscribe"))
                {
                    chat.subscription = args.PRESENCE_MESSAGE.TYPE;
                }

                if (!string.Equals(args.PRESENCE_MESSAGE.TYPE, "unsubscribe") && !string.Equals(args.PRESENCE_MESSAGE.TYPE, "subscribe"))
                {
                    switch (chat.subscription)
                    {
                        case "from":
                        case "none":
                        case "pending":
                        case null:
                            chat.presence = Presence.Unavailable;
                            break;

                        default:
                            chat.status = args.PRESENCE_MESSAGE.STATUS;
                            chat.presence = args.PRESENCE_MESSAGE.PRESENCE;
                            break;
                    }
                }

                chat.Update();
            }
        }

        private void OnNewRoosterMessage(IMessageSender sender, NewValidMessageEventArgs args)
        {
            if (args.MESSAGE is RosterResultMessage msg && sender is XMPPClient xmppClient)
            {
                string type = msg.TYPE;
                if (!string.Equals(type, IQMessage.RESULT) && !string.Equals(type, IQMessage.SET))
                {
                    // No roster result or set => return
                    return;
                }

                SemaLock semaLock = DataCache.INSTANCE.NewChatSemaLock();
                IEnumerable<ChatModel> chats = DataCache.INSTANCE.GetChats(client.dbAccount.bareJid, semaLock);

                // TYPE == SET is being send by the server when the roster changes from outside:
                if (string.Equals(type, IQMessage.RESULT))
                {
                    foreach (ChatModel chat in chats)
                    {
                        chat.inRoster = false;
                    }
                }

                List<ChatModel> addedChats = new List<ChatModel>();
                List<ChatModel> updatedChats = new List<ChatModel>();

                foreach (RosterItem item in msg.ITEMS)
                {
                    ChatModel chat = chats.Where(c => string.Equals(c.bareJid, item.JID)).FirstOrDefault();
                    if (!(chat is null))
                    {
                        chat.inRoster = !string.Equals(item.SUBSCRIPTION, "remove");
                        updatedChats.Add(chat);
                    }
                    else if (!string.Equals(item.SUBSCRIPTION, "remove"))
                    {
                        chat = new ChatModel(item.JID, client.dbAccount)
                        {
                            inRoster = true,
                            chatType = ChatType.CHAT
                        };
                        addedChats.Add(chat);
                    }
                    else
                    {
                        continue;
                    }

                    // Only update the subscription state, if not set to subscribe:
                    if (!string.Equals(chat.subscription, "subscribe"))
                    {
                        chat.subscription = item.SUBSCRIPTION;
                    }
                    chat.subscriptionRequested = string.Equals(item.ASK, "subscribe");

                    switch (chat.subscription)
                    {
                        case "unsubscribe":
                        case "from":
                        case "none":
                        case "pending":
                        case null:
                            chat.presence = Presence.Unavailable;
                            break;

                        default:
                            break;
                    }
                }

                // Add all new chats:
                foreach (ChatModel chat in addedChats)
                {
                    DataCache.INSTANCE.AddChatUnsafe(chat, client);
                }
                semaLock.Dispose();

                // Update all existing chats:
                using (MainDbContext ctx = new MainDbContext())
                {
                    ctx.UpdateRange(updatedChats);
                }
            }
        }

        private void OnConnectionStateChanged(XMPPClient xmppClient, ConnectionStateChangedEventArgs args)
        {
            switch (args.newState)
            {
                case ConnectionState.CONNECTED:
                    OnConnected();
                    break;

                case ConnectionState.DISCONNECTING:
                    OnDisconneting();
                    break;

                case ConnectionState.ERROR:
                case ConnectionState.DISCONNECTED:
                    OnDisconnectedOrError();
                    break;

                default:
                    break;
            }
        }

        private void OnMessageSend(XMPPClient xmppClient, MessageSendEventArgs args)
        {
            ChatMessageModel msg = DataCache.INSTANCE.GetChatMessage(args.CHAT_MESSAGE_ID);
            Debug.Assert(!(msg is null));
            if (!(msg is null))
            {
                msg.state = MessageState.SEND;
                msg.Update();
            }
        }

        private void OnNewBookmarksResultMessage(XMPPClient xmppClient, NewBookmarksResultMessageEventArgs args)
        {
            foreach (ConferenceItem ci in args.BOOKMARKS_MESSAGE.STORAGE.CONFERENCES)
            {
                SemaLock semaLock = DataCache.INSTANCE.NewChatSemaLock();
                ChatModel chat = DataCache.INSTANCE.GetChat(client.dbAccount.bareJid, ci.jid, semaLock);
                bool newMuc = chat is null;
                if (newMuc)
                {
                    chat = new ChatModel(ci.jid, client.dbAccount)
                    {
                        chatType = ChatType.MUC
                    };
                    chat.muc = new MucInfoModel(chat)
                    {
                        state = MucState.DISCONNECTED
                    };
                }
                else
                {
                    semaLock.Dispose();
                }

                // Update chat:
                chat.inRoster = true;
                chat.presence = Presence.Unavailable;
                chat.isChatActive = true;

                // Update MUC info:
                chat.muc.autoEnterRoom = ci.autoJoin;
                chat.muc.name = ci.name;
                chat.muc.nickname = ci.nick;
                chat.muc.password = ci.password;

                if (newMuc)
                {
                    DataCache.INSTANCE.AddChatUnsafe(chat, client);
                    semaLock.Dispose();
                }
                else
                {
                    chat.Update();
                }

                // Enter MUC manually if the MUC is new for this client:
                if (newMuc && chat.muc.autoEnterRoom && !Settings.GetSettingBoolean(SettingsConsts.DISABLE_AUTO_JOIN_MUC))
                {
                    Task.Run(() => MucHandler.INSTANCE.EnterMucAsync(xmppClient, chat.muc));
                }
            }
        }

        private void OnNewDeliveryReceipt(XMPPClient xmppClient, NewDeliveryReceiptEventArgs args)
        {
            string fromBareJid = Utils.getBareJidFromFullJid(args.MSG.getFrom());
            ChatModel chat;
            using (SemaLock semaLock = DataCache.INSTANCE.NewChatSemaLock())
            {
                chat = DataCache.INSTANCE.GetChat(client.dbAccount.bareJid, fromBareJid, semaLock);
            }
            if (chat is null)
            {
                Logger.Warn($"Delivery receipt for an unknown chat ({fromBareJid}) on account '{client.dbAccount.bareJid}' received.");
                return;
            }

            ChatMessageModel msg = DataCache.INSTANCE.GetChatMessage(chat.id, args.MSG.RECEIPT_ID);
            if (msg is null)
            {
                Logger.Warn($"Delivery receipt for an unknown chat message ({args.MSG.RECEIPT_ID}) on account '{client.dbAccount.bareJid}' received.");
                return;
            }
            msg.state = MessageState.DELIVERED;
            msg.Update();
        }

        private void OnNewChatState(XMPPClient client, NewChatStateEventArgs args)
        {
            string from = Utils.getBareJidFromFullJid(args.FROM);
            ChatModel chat;
            using (SemaLock semaLock = DataCache.INSTANCE.NewChatSemaLock())
            {
                chat = DataCache.INSTANCE.GetChat(client.getXMPPAccount().getBareJid(), from, semaLock);
            }
            if (chat is null)
            {
                Logger.Warn("Received a chat state message for an unknown chat from: " + args.FROM + ", to: " + args.TO);
                return;
            }
            chat.chatState = args.STATE;
            chat.lastChatStateUpdate = DateTime.Now;
        }

        #endregion
    }
}
