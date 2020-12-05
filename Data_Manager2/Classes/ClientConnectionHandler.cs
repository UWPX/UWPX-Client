using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Data_Manager2.Classes.DBManager;
using Data_Manager2.Classes.DBTables;
using Data_Manager2.Classes.Events;
using Data_Manager2.Classes.Omemo;
using Data_Manager2.Classes.Toast;
using Logging;
using XMPP_API.Classes;
using XMPP_API.Classes.Crypto;
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
using XMPP_API.Classes.Network.XML.Messages.XEP_0384.Signal.Session;

namespace Data_Manager2.Classes
{
    public class ClientConnectionHandler: IAsyncDisposable
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        public readonly XMPPClient client;

        public event ClientConnectedHandler ClientConnected;
        public delegate void ClientConnectedHandler(ClientConnectionHandler handler, ClientConnectedEventArgs args);

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        public ClientConnectionHandler(XMPPAccount account)
        {
            client = LoadAccount(account);
        }

        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--
        /// <summary>
        /// Returns true in case the account is disabled.
        /// </summary>
        public bool IsDisabled()
        {
            return client.getXMPPAccount().disabled;
        }

        public string GetBareJid()
        {
            return client.getXMPPAccount().getBareJid();
        }

        public void SetAccount(XMPPAccount account)
        {

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
            switch (client.getConnetionState())
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

        public async Task ConnectAsync()
        {
            await client.connectAsync();
        }

        public async Task DisconnectAsync()
        {
            await client.disconnectAsync();
        }

        public async Task ReconnectAsync()
        {
            await client.reconnectAsync();
        }

        #endregion

        #region --Misc Methods (Private)--
        /// <summary>
        /// Loads one specific XMPPAccount and subscribes to all its events.
        /// </summary>
        /// <param name="account">The account which should get loaded.</param>
        /// <returns>Returns a new XMPPClient instance.</returns>
        private XMPPClient LoadAccount(XMPPAccount account)
        {
            XMPPClient client = new XMPPClient(account);
            client.connection.TCP_CONNECTION.disableConnectionTimeout = Settings.getSettingBoolean(SettingsConsts.DEBUG_DISABLE_TCP_TIMEOUT);
            client.connection.TCP_CONNECTION.disableTlsUpgradeTimeout = Settings.getSettingBoolean(SettingsConsts.DEBUG_DISABLE_TLS_TIMEOUT);

            // Enable OMEMO:
            OmemoStore signalProtocolStore = new OmemoStore(account);
            // Generate OMEMO keys if necessary:
            if (!account.omemoKeysGenerated)
            {
                account.generateOmemoKeys();
                AccountDBManager.INSTANCE.setAccount(account, true, false);
            }
            client.enableOmemo(signalProtocolStore);

            // Sometimes the DB fails and only stores less than 100 OMEMO pre keys.
            // So if we detect an issue with that generate a new batch of OMEMO pre keys and announce the new ones.
            if (account.OMEMO_PRE_KEYS.Count < 100)
            {
                Logger.Warn("Only " + account.OMEMO_PRE_KEYS.Count + " found. Generating a new set.");
                account.OMEMO_PRE_KEYS.Clear();
                account.OMEMO_PRE_KEYS.AddRange(CryptoUtils.generateOmemoPreKeys());
                account.omemoBundleInfoAnnounced = false;
                AccountDBManager.INSTANCE.setAccount(account, true, false);
            }

            // Ensure no event gets bound multiple times:
            UnsubscribeFromEvents();
            SubscribeToEvents();
            return client;
        }

        private void SetOmemoChatMessagesSendFailed(IList<OmemoMessageMessage> messages, ChatTable chat)
        {
            foreach (OmemoMessageMessage msg in messages)
            {
                string msgId = ChatMessageTable.generateId(msg.ID, chat.id);
                ChatDBManager.INSTANCE.updateChatMessageState(msgId, MessageState.ENCRYPT_FAILED);
            }
        }

        /// <summary>
        /// Called once a client enters the 'Disconnected' or 'Error' state.
        /// </summary>
        /// <param name="client">The Client which entered the state.</param>
        private void OnDisconnectedOrError(XMPPClient client)
        {
            ChatDBManager.INSTANCE.resetPresence(GetBareJid());
            MUCHandler.INSTANCE.onClientDisconnected(client);
        }

        /// <summary>
        /// Called once a client enters the 'Disconnecting' state.
        /// </summary>
        /// <param name="client">The Client which entered the state.</param>
        private void OnDisconneting(XMPPClient client)
        {
            MUCHandler.INSTANCE.onClientDisconnecting(client);
        }

        /// <summary>
        /// Called once a client enters the 'Connected' state.
        /// </summary>
        /// <param name="client">The Client which entered the state.</param>
        private void OnConnected(XMPPClient client)
        {
            Task.Run(async () =>
            {
                await client.GENERAL_COMMAND_HELPER.sendRequestRosterMessageAsync();
                client.PUB_SUB_COMMAND_HELPER.requestBookmars_xep_0048(null, null);
                MUCHandler.INSTANCE.onClientConnected(client);
                ClientConnected?.Invoke(this, new ClientConnectedEventArgs(client));
                await SendOutsandingChatMessagesAsync(client);
            });
        }

        private async Task SendOutsandingChatMessagesAsync(XMPPClient client)
        {
            IList<ChatMessageTable> toSend = ChatDBManager.INSTANCE.getChatMessages(GetBareJid(), MessageState.SENDING);
            Logger.Info("Sending " + toSend.Count + " outstanding chat messages for: " + GetBareJid());
            await SendOutsandingChatMessagesAsync(client, toSend);
            Logger.Info("Finished sending outstanding chat messages for: " + GetBareJid());

            IList<ChatMessageTable> toEncrypt = ChatDBManager.INSTANCE.getChatMessages(GetBareJid(), MessageState.TO_ENCRYPT);
            Logger.Info("Sending " + toSend.Count + " outstanding OMEMO chat messages for: " + GetBareJid());
            await SendOutsandingChatMessagesAsync(client, toEncrypt);
            Logger.Info("Finished sending outstanding OMEMO chat messages for: " + GetBareJid());
        }

        /// <summary>
        /// Sends all chat messages passed to it.
        /// </summary>
        /// <param name="client">The XMPPClient that should be used to send the messages.</param>
        /// <param name="messages">A list of chat messages to send. They SHOULD be sorted by their chatId for optimal performance.</param>
        private async Task SendOutsandingChatMessagesAsync(XMPPClient client, IList<ChatMessageTable> messages)
        {
            ChatTable chat = null;
            foreach (ChatMessageTable msgDb in messages)
            {
                if (chat is null || !string.Equals(chat.id, msgDb.chatId))
                {
                    chat = ChatDBManager.INSTANCE.getChat(msgDb.chatId);

                    if (chat is null)
                    {
                        Logger.Warn("Unable to send outstanding chat message for: " + msgDb.chatId + " - no such chat.");
                        continue;
                    }
                }
                MessageMessage msg = msgDb.toXmppMessage(client.getXMPPAccount().getFullJid(), chat);

                if (msg is OmemoMessageMessage omemoMsg)
                {
                    await client.sendOmemoMessageAsync(omemoMsg, chat.chatJabberId, GetBareJid());
                }
                else
                {
                    await client.SendAsync(msg);
                }
            }
        }

        private async Task AnswerPresenceProbeAsync(string from, string to, ChatTable chat, XMPPClient client, PresenceMessage msg)
        {
            XMPPAccount account = client.getXMPPAccount();
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
                        answer = new PresenceMessage(account.getBareJid(), from, account.presence, account.status, account.presencePriorety);
                        Logger.Debug("Answered presence probe from: " + from);
                        break;

                    case "none" when chat.inRoster:
                    case "to" when chat.inRoster:
                        answer = new PresenceErrorMessage(account.getFullJid(), from, PresenceErrorType.FORBIDDEN);
                        Logger.Warn("Received a presence probe but chat has no subscription: " + from + ", to: " + to + " subscription: " + chat.subscription);
                        break;

                    default:
                        answer = new PresenceErrorMessage(account.getFullJid(), from, PresenceErrorType.NOT_AUTHORIZED);
                        Logger.Warn("Received a presence probe but chat has no subscription: " + from + ", to: " + to + " subscription: " + chat.subscription);
                        break;
                }
            }
            await client.SendAsync(answer);
        }

        /// <summary>
        /// Unsubscribes from all XMPPClient events.
        /// </summary>
        private void UnsubscribeFromEvents()
        {
            client.NewChatMessage -= OnNewChatMessage;
            client.NewRoosterMessage -= OnNewRoosterMessage;
            client.NewPresence -= OnNewPresence;
            client.ConnectionStateChanged -= OnConnectionStateChanged;
            client.MessageSend -= OnMessageSend;
            client.NewBookmarksResultMessage -= OnNewBookmarksResultMessage;
            client.NewDeliveryReceipt -= OnNewDeliveryReceipt;
            client.getXMPPAccount().PropertyChanged -= ConnectionHandler_PropertyChanged;
            client.OmemoSessionBuildError -= OnOmemoSessionBuildError;
        }

        /// <summary>
        /// Subscribes to all XMPPClient events.
        /// </summary>
        private void SubscribeToEvents()
        {
            client.NewChatMessage += OnNewChatMessage;
            client.NewRoosterMessage += OnNewRoosterMessage;
            client.NewPresence += OnNewPresence;
            client.ConnectionStateChanged += OnConnectionStateChanged;
            client.MessageSend += OnMessageSend;
            client.NewBookmarksResultMessage += OnNewBookmarksResultMessage;
            client.NewDeliveryReceipt += OnNewDeliveryReceipt;
            client.getXMPPAccount().PropertyChanged += ConnectionHandler_PropertyChanged;
            client.OmemoSessionBuildError += OnOmemoSessionBuildError;
        }

        #endregion

        #region --Misc Methods (Protected)--


        #endregion
        //--------------------------------------------------------Events:---------------------------------------------------------------------\\
        #region --Events--
        private void OnOmemoSessionBuildError(XMPPClient client, OmemoSessionBuildErrorEventArgs args)
        {
            Task.Run(async () =>
            {
                ChatTable chat = ChatDBManager.INSTANCE.getChat(ChatTable.generateId(args.CHAT_JID, GetBareJid()));
                if (!(chat is null))
                {
                    // Add an error chat message:
                    ChatMessageTable msg = new ChatMessageTable()
                    {
                        id = ChatMessageTable.generateErrorMessageId(AbstractMessage.getRandomId(), chat.id),
                        chatId = chat.id,
                        date = DateTime.Now,
                        fromUser = args.CHAT_JID,
                        isImage = false,
                        message = "Failed to encrypt and send " + args.MESSAGES.Count + " OMEMO message(s) with:\n" + args.ERROR,
                        state = MessageState.UNREAD,
                        type = MessageMessage.TYPE_ERROR
                    };
                    await ChatDBManager.INSTANCE.setChatMessageAsync(msg, true, false);

                    // Set chat messages to encrypted failed:
                    SetOmemoChatMessagesSendFailed(args.MESSAGES, chat);
                }
            });
        }

        private async void OnNewChatMessage(XMPPClient client, XMPP_API.Classes.Network.Events.NewChatMessageEventArgs args)
        {
            MessageMessage msg = args.getMessage();

            // Handel MUC room subject messages:
            if (msg is MUCRoomSubjectMessage)
            {
                MUCHandler.INSTANCE.onMUCRoomSubjectMessage(msg as MUCRoomSubjectMessage);
                return;
            }

            string from = Utils.getBareJidFromFullJid(msg.getFrom());
            string to = Utils.getBareJidFromFullJid(msg.getTo());
            string id = msg.CC_TYPE == CarbonCopyType.SENT ? ChatTable.generateId(to, from) : ChatTable.generateId(from, to);

            // Check if device id is valid and if, decrypt the OMEMO messages:
            if (msg is OmemoMessageMessage omemoMessage)
            {
                OmemoHelper helper = client.getOmemoHelper();
                if (helper is null)
                {
                    OnOmemoSessionBuildError(client, new OmemoSessionBuildErrorEventArgs(id, OmemoSessionBuildError.KEY_ERROR, new List<OmemoMessageMessage> { omemoMessage }));
                    Logger.Error("Failed to decrypt OMEMO message - OmemoHelper is null");
                    return;
                }
                else if (!client.getXMPPAccount().checkOmemoKeys())
                {
                    OnOmemoSessionBuildError(client, new OmemoSessionBuildErrorEventArgs(id, OmemoSessionBuildError.KEY_ERROR, new List<OmemoMessageMessage> { omemoMessage }));
                    Logger.Error("Failed to decrypt OMEMO message - keys are corrupted");
                    return;
                }
                else if (!await omemoMessage.decryptAsync(client.getOmemoHelper(), client.getXMPPAccount().omemoDeviceId))
                {
                    return;
                }
            }

            ChatTable chat = ChatDBManager.INSTANCE.getChat(id);
            bool chatChanged = false;

            // Spam detection:
            if (Settings.getSettingBoolean(SettingsConsts.SPAM_DETECTION_ENABLED))
            {
                if (Settings.getSettingBoolean(SettingsConsts.SPAM_DETECTION_FOR_ALL_CHAT_MESSAGES) || chat is null)
                {
                    if (SpamDBManager.INSTANCE.isSpam(msg.MESSAGE))
                    {
                        Logger.Warn("Received spam message from " + from);
                        return;
                    }
                }
            }

            if (chat is null)
            {
                chatChanged = true;
                chat = new ChatTable(from, to)
                {
                    lastActive = msg.getDelay(),
                    chatType = string.Equals(msg.TYPE, MessageMessage.TYPE_GROUPCHAT) ? ChatType.MUC : ChatType.CHAT
                };
            }

            // Mark chat as active:
            chat.isChatActive = true;

            ChatMessageTable message = new ChatMessageTable(msg, chat);

            // Handle MUC invite messages:
            if (msg is DirectMUCInvitationMessage)
            {
                DirectMUCInvitationMessage inviteMessage = msg as DirectMUCInvitationMessage;
                bool doesRoomExist = ChatDBManager.INSTANCE.doesMUCExist(ChatTable.generateId(inviteMessage.ROOM_JID, msg.getTo()));
                bool doesOutstandingInviteExist = ChatDBManager.INSTANCE.doesOutstandingMUCInviteExist(id, inviteMessage.ROOM_JID);

                if (doesRoomExist && doesOutstandingInviteExist)
                {
                    return;
                }

                MUCDirectInvitationTable inviteTable = new MUCDirectInvitationTable(inviteMessage, message.id);
                ChatDBManager.INSTANCE.setMUCDirectInvitation(inviteTable);
            }

            bool isMUCMessage = string.Equals(MessageMessage.TYPE_GROUPCHAT, message.type);
            ChatMessageTable existingMessage = ChatDBManager.INSTANCE.getChatMessageById(message.id);
            bool doesMessageExist = existingMessage != null;

            if (isMUCMessage)
            {
                MUCChatInfoTable mucInfo = MUCDBManager.INSTANCE.getMUCInfo(chat.id);
                if (mucInfo != null)
                {
                    if (Equals(message.fromUser, mucInfo.nickname))
                    {
                        // Filter MUC messages that already exist:
                        // ToDo: Allow MUC messages being edited and detect it
                        if (doesMessageExist)
                        {
                            return;
                        }
                        else
                        {
                            message.state = MessageState.SEND;
                        }
                    }
                    else
                    {
                        if (doesMessageExist)
                        {
                            message.state = existingMessage.state;
                        }
                    }
                }
            }

            if (chat.lastActive.CompareTo(msg.getDelay()) < 0)
            {
                chatChanged = true;
                chat.lastActive = msg.getDelay();
            }

            if (chatChanged)
            {
                ChatDBManager.INSTANCE.setChat(chat, false, true);
            }

            // Send XEP-0184 (Message Delivery Receipts) reply:
            if (msg.RECIPT_REQUESTED && id != null && !Settings.getSettingBoolean(SettingsConsts.DONT_SEND_CHAT_MESSAGE_RECEIVED_MARKERS))
            {
                await Task.Run(async () =>
                {
                    DeliveryReceiptMessage receiptMessage = new DeliveryReceiptMessage(client.getXMPPAccount().getFullJid(), from, msg.ID);
                    await client.SendAsync(receiptMessage);
                });
            }

            await ChatDBManager.INSTANCE.setChatMessageAsync(message, !doesMessageExist, doesMessageExist && !isMUCMessage);

            // Show toast:
            if (!doesMessageExist && !chat.muted)
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
                                        ToastHelper.showChatTextImageToast(message, chat);
                                    }
                                    else
                                    {
                                        ToastHelper.showChatTextToast(message, chat);
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

        private async void OnNewPresence(XMPPClient client, NewPresenceMessageEventArgs args)
        {
            string from = Utils.getBareJidFromFullJid(args.PRESENCE_MESSAGE.getFrom());

            // If received a presence message from your own account, ignore it:
            if (string.Equals(from, GetBareJid()))
            {
                return;
            }

            string to = GetBareJid();
            string id = ChatTable.generateId(from, to);
            ChatTable chat = ChatDBManager.INSTANCE.getChat(id);
            switch (args.PRESENCE_MESSAGE.TYPE)
            {
                case "subscribe":
                case "unsubscribed":
                    if (chat is null)
                    {
                        chat = new ChatTable(from, to);
                    }
                    chat.subscription = args.PRESENCE_MESSAGE.TYPE;
                    break;

                // Answer presence probes:
                case "probe":
                    await AnswerPresenceProbeAsync(from, to, chat, client, args.PRESENCE_MESSAGE);
                    return;

                default:
                    break;
            }

            if (chat != null)
            {
                chat.status = args.PRESENCE_MESSAGE.STATUS;
                chat.presence = args.PRESENCE_MESSAGE.PRESENCE;
                ChatDBManager.INSTANCE.setChat(chat, false, true);
            }
            else
            {
                Logger.Warn("Received a presence message for an unknown chat from: " + from + ", to: " + to);
            }
        }

        private void OnNewRoosterMessage(IMessageSender sender, NewValidMessageEventArgs args)
        {
            if (args.MESSAGE is RosterResultMessage msg && sender is XMPPClient client)
            {
                string to = GetBareJid();
                string type = msg.TYPE;

                if (string.Equals(type, IQMessage.RESULT))
                {
                    ChatDBManager.INSTANCE.setAllNotInRoster(GetBareJid());
                }
                else if (!string.Equals(type, IQMessage.SET))
                {
                    // No roster result or set => return
                    return;
                }

                foreach (RosterItem item in msg.ITEMS)
                {
                    string from = item.JID;
                    string id = ChatTable.generateId(from, to);
                    ChatTable chat = ChatDBManager.INSTANCE.getChat(id);
                    if (chat != null)
                    {
                        chat.inRoster = !string.Equals(item.SUBSCRIPTION, "remove");
                    }
                    else if (!string.Equals(item.SUBSCRIPTION, "remove"))
                    {
                        chat = new ChatTable(from, to)
                        {
                            inRoster = true,
                            chatType = ChatType.CHAT
                        };
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

                    ChatDBManager.INSTANCE.setChat(chat, false, true);
                }
            }
        }

        private void OnConnectionStateChanged(XMPPClient client, ConnectionStateChangedEventArgs args)
        {
            switch (args.newState)
            {
                case ConnectionState.CONNECTED:
                    OnConnected(client);
                    break;

                case ConnectionState.DISCONNECTING:
                    OnDisconneting(client);
                    break;

                case ConnectionState.ERROR:
                case ConnectionState.DISCONNECTED:
                    OnDisconnectedOrError(client);
                    break;

                default:
                    break;
            }
        }

        private void OnMessageSend(XMPPClient client, MessageSendEventArgs args)
        {
            ChatDBManager.INSTANCE.updateChatMessageState(args.CHAT_MESSAGE_ID, MessageState.SEND);
        }

        private void OnNewBookmarksResultMessage(XMPPClient client, NewBookmarksResultMessageEventArgs args)
        {
            foreach (ConferenceItem c in args.BOOKMARKS_MESSAGE.STORAGE.CONFERENCES)
            {
                bool newMUC = false;
                string to = GetBareJid();
                string from = c.jid;
                string id = ChatTable.generateId(from, to);

                // Create / update chat:
                ChatTable chat = ChatDBManager.INSTANCE.getChat(id);
                if (chat is null)
                {
                    chat = new ChatTable(from, to);
                    newMUC = true;
                }
                chat.chatType = ChatType.MUC;
                chat.inRoster = true;
                chat.presence = Presence.Unavailable;
                chat.isChatActive = true;

                ChatDBManager.INSTANCE.setChat(chat, false, true);

                // Create / update MUC info:
                MUCChatInfoTable info = MUCDBManager.INSTANCE.getMUCInfo(chat.id);
                if (info is null)
                {
                    info = new MUCChatInfoTable
                    {
                        chatId = chat.id,
                        subject = null
                    };
                    newMUC = true;
                }

                info.autoEnterRoom = c.autoJoin;
                info.name = c.name;
                info.nickname = c.nick;
                info.password = c.password;

                MUCDBManager.INSTANCE.setMUCChatInfo(info, false, true);


                // Enter MUC manually if the MUC is new for this client:
                if (newMUC && info.autoEnterRoom && !Settings.getSettingBoolean(SettingsConsts.DISABLE_AUTO_JOIN_MUC))
                {
                    Task.Run(() => MUCHandler.INSTANCE.enterMUCAsync(client, chat, info));
                }
            }
        }

        private void OnNewDeliveryReceipt(XMPPClient client, NewDeliveryReceiptEventArgs args)
        {
            Task.Run(() =>
            {
                string to = Utils.getBareJidFromFullJid(args.MSG.getTo());
                string from = Utils.getBareJidFromFullJid(args.MSG.getFrom());
                string chatId = ChatTable.generateId(from, to);
                string msgId = ChatMessageTable.generateId(args.MSG.RECEIPT_ID, chatId);
                ChatDBManager.INSTANCE.setMessageAsDeliverd(msgId, true);
            });
        }

        private void ConnectionHandler_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (sender is XMPPAccount account)
            {
                // Check if an OMEMO related account property changed.
                // If so also update the OMEMO keys.
                bool updateOmemoKeys = e.PropertyName.ToLowerInvariant().Contains("omemo");
                AccountDBManager.INSTANCE.setAccount(account, updateOmemoKeys, false);
            }
        }

        #endregion
    }
}
