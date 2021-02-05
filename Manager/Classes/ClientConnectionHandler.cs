using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Logging;
using Manager.Classes.Toast;
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
        public async ValueTask DisposeAsync()
        {
            UnsubscribeFromEvents();
            await DisconnectAsync();
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
        public async Task ConnectAsync()
        {
            await client.xmppClient.connectAsync();
        }

        /// <summary>
        /// Disconnects the client.
        /// </summary>
        public async Task DisconnectAsync()
        {
            await client.xmppClient.disconnectAsync();
        }

        /// <summary>
        /// Reconnects the client.
        /// </summary>
        public async Task ReconnectAsync()
        {
            await client.xmppClient.reconnectAsync();
        }

        private async Task<bool> DecryptOmemoEncryptedMessageAsync(OmemoEncryptedMessage msg)
        {
            try
            {
                await client.xmppClient.connection.omemoHelper.decryptOmemoEncryptedMessageAsync(msg);
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


            // Check if device id is valid and if, decrypt the OMEMO messages:
            if (msg is OmemoEncryptedMessage omemoMessage)
            {
                OmemoHelper helper = client.xmppClient.getOmemoHelper();
                if (helper is null)
                {
                    OnOmemoSessionBuildError(client.xmppClient, new OmemoSessionBuildErrorEventArgs(from, OmemoSessionBuildError.KEY_ERROR, new List<OmemoEncryptedMessage> { omemoMessage }));
                    Logger.Error("Failed to decrypt OMEMO message - OmemoHelper is null");
                    return;
                }
                else if (!await DecryptOmemoEncryptedMessageAsync(omemoMessage) || omemoMessage.IS_PURE_KEY_EXCHANGE_MESSAGE)
                {
                    return;
                }
            }

            ChatModel chat;
            using (MainDbContext ctx = new MainDbContext())
            {
                chat = ctx.Chats.Where(c => string.Equals(c.accountBareJid, to) && string.Equals(c.bareJid, from)).FirstOrDefault();
            }
            bool chatChanged = false;

            // Spam detection:
            if (Settings.GetSettingBoolean(SettingsConsts.SPAM_DETECTION_ENABLED))
            {
                if (Settings.GetSettingBoolean(SettingsConsts.SPAM_DETECTION_FOR_ALL_CHAT_MESSAGES) || chat is null)
                {
                    if (SpamHelper.INSTANCE.IsSpam(msg.MESSAGE))
                    {
                        Logger.Warn("Received spam message from " + from);
                        return;
                    }
                }
            }

            bool newChat = chat is null;
            if (newChat)
            {
                chatChanged = true;
                chat = new ChatModel(from, client.dbAccount)
                {
                    lastActive = msg.delay,
                    chatType = string.Equals(msg.TYPE, MessageMessage.TYPE_GROUPCHAT) ? ChatType.MUC : ChatType.CHAT
                };
            }

            // Mark chat as active:
            chat.isChatActive = true;

            ChatMessageModel message;
            using (MainDbContext ctx = new MainDbContext())
            {
                message = ctx.ChatMessages.Where(m => string.Equals(m.stableId, msg.ID) && string.Equals(m.fromBareJid, from) && string.Equals(m.chat.accountBareJid, client.dbAccount.bareJid)).FirstOrDefault();
            }
            // Filter messages that already exist:
            // ToDo: Allow MUC messages being edited and detect it
            if (!(message is null))
            {
                Logger.Debug("Duplicate message received from '" + from + "'. Dropping it...");
                return;
            }
            message = new ChatMessageModel(msg, chat);

            // Handle MUC invite messages:
            if (msg is DirectMUCInvitationMessage inviteMessage)
            {
                if (!newChat)
                {
                    Logger.Info("Ignoring received MUC direct invitation form '" + from + "' since we already joined this MUC (" + inviteMessage.ROOM_JID + ").");
                    return;
                }
                using (MainDbContext ctx = new MainDbContext())
                {
                    if (chatChanged)
                    {
                        if (newChat)
                        {
                            ctx.Add(chat);
                            newChat = false;
                        }
                        else
                        {
                            ctx.Update(chat);
                        }
                        chatChanged = false;
                    }
                    // Ensure we add the message to the DB before we add the invite since the invite has the message as a foreign key:
                    ctx.Add(message);
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
                using (MainDbContext ctx = new MainDbContext())
                {
                    ctx.Update(chat);
                    chatChanged = false;
                }
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

                                    // Update badge notification count:
                                    ToastHelper.UpdateBadgeNumber();
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

        #endregion

        #region --Misc Methods (Private)--
        private void SetOmemoChatMessagesSendFailed(IEnumerable<OmemoEncryptedMessage> messages, ChatModel chat)
        {
            foreach (OmemoEncryptedMessage msg in messages)
            {
                using (MainDbContext ctx = new MainDbContext())
                {
                    foreach (ChatMessageModel msgModel in ctx.ChatMessages.Where(m => string.Equals(m.stableId, msg.ID) && string.Equals(m.chat.accountBareJid, client.dbAccount.bareJid)))
                    {
                        msgModel.state = MessageState.ENCRYPT_FAILED;
                        ctx.Update(msgModel);
                    }
                }
            }
        }

        /// <summary>
        /// Called once a client enters the 'Disconnected' or 'Error' state.
        /// </summary>
        private void OnDisconnectedOrError()
        {
            using (MainDbContext ctx = new MainDbContext())
            {
                foreach (ChatModel chat in ctx.Chats.Where(c => string.Equals(c.accountBareJid, client.dbAccount.bareJid)))
                {
                    chat.presence = Presence.Unavailable;
                    ctx.Update(chat);
                }
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
            client.xmppClient.ConnectionStateChanged += OnConnectionStateChanged;
            client.xmppClient.MessageSend += OnMessageSend;
            client.xmppClient.NewBookmarksResultMessage += OnNewBookmarksResultMessage;
            client.xmppClient.NewDeliveryReceipt += OnNewDeliveryReceipt;
            client.xmppClient.OmemoSessionBuildError += OnOmemoSessionBuildError;
        }

        #endregion

        #region --Misc Methods (Protected)--


        #endregion
        //--------------------------------------------------------Events:---------------------------------------------------------------------\\
        #region --Events--
        private void OnOmemoSessionBuildError(XMPPClient xmppClient, OmemoSessionBuildErrorEventArgs args)
        {
            Task.Run(() =>
            {
                ChatModel chat;
                using (MainDbContext ctx = new MainDbContext())
                {
                    chat = ctx.Chats.Where(c => string.Equals(c.accountBareJid, xmppClient.getXMPPAccount().getBareJid()) && string.Equals(c.bareJid, args.CHAT_JID)).FirstOrDefault();
                    if (!(chat is null))
                    {
                        // Add an error chat message:
                        ChatMessageModel msg = new ChatMessageModel()
                        {
                            chat = chat,
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
                        ctx.Add(msg);

                        // Set chat messages to encrypted failed:
                        SetOmemoChatMessagesSendFailed(args.MESSAGES, chat);
                    }
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
            using (MainDbContext ctx = new MainDbContext())
            {
                chat = ctx.Chats.Where(c => string.Equals(c.accountBareJid, client.dbAccount.bareJid) && string.Equals(c.bareJid, from)).FirstOrDefault();
            }
            switch (args.PRESENCE_MESSAGE.TYPE)
            {
                case "subscribe":
                case "unsubscribed":
                    if (chat is null)
                    {
                        chat = new ChatModel(from, client.dbAccount);
                    }
                    chat.subscription = args.PRESENCE_MESSAGE.TYPE;
                    break;

                // Answer presence probes:
                case "probe":
                    await AnswerPresenceProbeAsync(from, client.dbAccount.bareJid, chat, args.PRESENCE_MESSAGE);
                    return;

                default:
                    break;
            }

            if (chat != null)
            {
                chat.status = args.PRESENCE_MESSAGE.STATUS;
                chat.presence = args.PRESENCE_MESSAGE.PRESENCE;
                using (MainDbContext ctx = new MainDbContext())
                {
                    ctx.Add(chat);
                }
            }
            else
            {
                Logger.Warn("Received a presence message for an unknown chat from: " + from + ", to: " + client.dbAccount.bareJid);
            }
        }

        private void OnNewRoosterMessage(IMessageSender sender, NewValidMessageEventArgs args)
        {
            if (args.MESSAGE is RosterResultMessage msg && sender is XMPPClient xmppClient)
            {
                string type = msg.TYPE;
                if (!string.Equals(type, IQMessage.SET))
                {
                    // No roster result or set => return
                    return;
                }
                using (MainDbContext ctx = new MainDbContext())
                {
                    IEnumerable<ChatModel> chats = ctx.Chats.Where(c => string.Equals(c.accountBareJid, client.dbAccount.bareJid));
                    foreach (ChatModel chat in chats)
                    {
                        chat.inRoster = false;
                    }

                    foreach (RosterItem item in msg.ITEMS)
                    {
                        ChatModel chat = chats.Where(c => string.Equals(c.bareJid, item.JID)).FirstOrDefault();
                        if (!(chat is null))
                        {
                            chat.inRoster = !string.Equals(item.SUBSCRIPTION, "remove");
                        }
                        else if (!string.Equals(item.SUBSCRIPTION, "remove"))
                        {
                            chat = new ChatModel(item.JID, client.dbAccount)
                            {
                                inRoster = true,
                                chatType = ChatType.CHAT
                            };
                            ctx.Add(chat);
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
                        ctx.Update(chat);
                    }
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
            using (MainDbContext ctx = new MainDbContext())
            {
                ChatMessageModel msg = ctx.ChatMessages.Where(m => string.Equals(m.stableId, args.CHAT_MESSAGE_ID) && string.Equals(m.chat.accountBareJid, client.dbAccount.bareJid)).FirstOrDefault();
                Debug.Assert(!(msg is null));
                msg.state = MessageState.SEND;
                ctx.Update(msg);
            }
        }

        private void OnNewBookmarksResultMessage(XMPPClient xmppClient, NewBookmarksResultMessageEventArgs args)
        {
            foreach (ConferenceItem ci in args.BOOKMARKS_MESSAGE.STORAGE.CONFERENCES)
            {
                ChatModel chat;
                MucInfoModel info;
                bool newMuc = false;
                using (MainDbContext ctx = new MainDbContext())
                {
                    chat = ctx.Chats.Where(c => string.Equals(c.accountBareJid, client.dbAccount.bareJid) && string.Equals(c.bareJid, ci.jid)).FirstOrDefault();

                    if (chat is null)
                    {
                        chat = new ChatModel(ci.jid, client.dbAccount);
                    }
                    chat.chatType = ChatType.MUC;
                    chat.inRoster = true;
                    chat.presence = Presence.Unavailable;
                    chat.isChatActive = true;

                    if (newMuc)
                    {
                        ctx.Add(chat);
                        info = new MucInfoModel()
                        {
                            chat = chat,
                            subject = null,
                            autoEnterRoom = ci.autoJoin,
                            name = ci.name,
                            nickname = ci.nick,
                            password = ci.password,
                            state = MucState.DISCONNECTED
                        };
                        ctx.Add(info);
                    }
                    else
                    {
                        ctx.Update(chat);
                        info = ctx.MucInfos.Where(m => m.chat.id == chat.id).FirstOrDefault();
                        info.autoEnterRoom = ci.autoJoin;
                        info.name = ci.name;
                        info.nickname = ci.nick;
                        info.password = ci.password;
                        ctx.Update(info);
                    }
                }
                // Enter MUC manually if the MUC is new for this client:
                if (newMuc && info.autoEnterRoom && !Settings.GetSettingBoolean(SettingsConsts.DISABLE_AUTO_JOIN_MUC))
                {
                    Task.Run(() => MucHandler.INSTANCE.EnterMucAsync(xmppClient, info));
                }
            }
        }

        private void OnNewDeliveryReceipt(XMPPClient xmppClient, NewDeliveryReceiptEventArgs args)
        {
            using (MainDbContext ctx = new MainDbContext())
            {
                string from = Utils.getBareJidFromFullJid(args.MSG.getFrom());
                ChatMessageModel msg = ctx.ChatMessages.Where(m => string.Equals(m.stableId, args.MSG.RECEIPT_ID) && string.Equals(m.fromBareJid, from) && string.Equals(m.chat.accountBareJid, client.dbAccount.bareJid)).FirstOrDefault();
                Debug.Assert(!(msg is null));
                msg.state = MessageState.SEND;
                ctx.Update(msg);
            }
        }

        #endregion
    }
}
