using Data_Manager2.Classes.DBManager;
using Data_Manager2.Classes.DBTables;
using Data_Manager2.Classes.Events;
using Data_Manager2.Classes.Omemo;
using Data_Manager2.Classes.Toast;
using Logging;
using Shared.Classes.Collections;
using Shared.Classes.Network;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Threading;
using System.Threading.Tasks;
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
    public class ConnectionHandler
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        private static readonly SemaphoreSlim CLIENT_SEMA = new SemaphoreSlim(1);
        public static readonly ConnectionHandler INSTANCE = new ConnectionHandler();
        private readonly CustomObservableCollection<XMPPClient> CLIENTS;
        private readonly DownloadHandler DOWNLOAD_HANDLER = new DownloadHandler();
        public readonly ImageDownloadHandler IMAGE_DOWNLOAD_HANDLER;

        public delegate void ClientConnectedHandler(ConnectionHandler handler, ClientConnectedEventArgs args);
        public delegate void ClientsCollectionChangedHandler(ConnectionHandler handler, NotifyCollectionChangedEventArgs args);

        public event ClientConnectedHandler ClientConnected;
        public event ClientsCollectionChangedHandler ClientsCollectionChanged;
        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        /// <summary>
        /// Basic Constructor
        /// </summary>
        /// <history>
        /// 18/11/2017 Created [Fabian Sauter]
        /// </history>
        public ConnectionHandler()
        {
            this.IMAGE_DOWNLOAD_HANDLER = new ImageDownloadHandler(DOWNLOAD_HANDLER);
            Task.Run(async () => await this.IMAGE_DOWNLOAD_HANDLER.ContinueDownloadsAsync());
            this.CLIENTS = new CustomObservableCollection<XMPPClient>(false);
            this.CLIENTS.CollectionChanged += CLIENTS_CollectionChanged;
            loadClients();
            AccountDBManager.INSTANCE.AccountChanged += INSTANCE_AccountChanged;
        }

        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--
        /// <summary>
        /// Returns the XMPPClient that matches the given id and domain.
        /// </summary>
        /// <param name="iDAndDomain">The id and domain of the requested XMPPClient. e.g. 'alice@jabber.org'</param>
        public XMPPClient getClient(string iDAndDomain)
        {
            foreach (XMPPClient c in CLIENTS)
            {
                if (c.getXMPPAccount().getBareJid().Equals(iDAndDomain))
                {
                    return c;
                }
            }
            return null;
        }

        /// <summary>
        /// Returns all available XMPPClients.
        /// </summary>
        public CustomObservableCollection<XMPPClient> getClients()
        {
            return CLIENTS;
        }

        #endregion
        //--------------------------------------------------------Misc Methods:---------------------------------------------------------------\\
        #region --Misc Methods (Public)--
        /// <summary>
        /// Starts connecting all XMPPClients.
        /// </summary>
        public void connectAll()
        {
            Parallel.ForEach(CLIENTS, (c) =>
            {
                if (!c.getXMPPAccount().disabled)
                {
                    switch (c.getConnetionState())
                    {
                        case ConnectionState.DISCONNECTED:
                        case ConnectionState.DISCONNECTING:
                        case ConnectionState.ERROR:
                            Task.Run(() => c.connect()).ContinueWith((Task prev) =>
                            {
                                if (prev.Exception != null)
                                {
                                    Logger.Error("Error during connectAll() - ConnectionHandler!", prev.Exception);
                                }
                            });
                            break;

                        default:
                            break;
                    }
                }
            });
        }

        /// <summary>
        /// Disconnects all XMPPClients in parallel.
        /// </summary>
        public void disconnectAll()
        {
            Parallel.ForEach(CLIENTS, async (c) => await c.disconnectAsync());
        }

        /// <summary>
        /// Disconnects all XMPPClients.
        /// </summary>
        public async Task disconnectAllAsync()
        {
            Task[] tasks = new Task[CLIENTS.Count];
            CLIENT_SEMA.Wait();
            for (int i = 0; i < CLIENTS.Count; i++)
            {
                tasks[i] = CLIENTS[i].disconnectAsync();
            }
            CLIENT_SEMA.Release();

            for (int i = 0; i < tasks.Length; i++)
            {
                await tasks[i].ConfigureAwait(true);
            }
        }

        /// <summary>
        /// Disconnects and removes the given account from the client list.
        /// </summary>
        /// <param name="accountId">The account id of the client you would like to remove.</param>
        public async Task removeAccountAsync(string accountId)
        {
            CLIENT_SEMA.Wait();
            for (int i = 0; i < CLIENTS.Count; i++)
            {
                if (Equals(CLIENTS[i].getXMPPAccount().getBareJid(), accountId))
                {
                    await CLIENTS[i].disconnectAsync();
                    CLIENTS.RemoveAt(i);
                }
            }
            CLIENT_SEMA.Release();
        }

        /// <summary>
        /// Reconnects all XMPPClients.
        /// </summary>
        public void reconnectAll()
        {
            Parallel.ForEach(CLIENTS, async (c) => await reconnectClientAsync(c));
        }

        #endregion

        #region --Misc Methods (Private)--
        /// <summary>
        /// Loads all XMPPClients from the DB and inserts them into the clients list.
        /// </summary>
        private void loadClients()
        {
            CLIENT_SEMA.Wait();
            CLIENTS.Clear();
            foreach (XMPPAccount acc in AccountDBManager.INSTANCE.loadAllAccounts())
            {
                CLIENTS.Add(loadAccount(acc));
            }
            CLIENT_SEMA.Release();
        }

        /// <summary>
        /// Reloads all clients from the DB.
        /// Disconnects all existing clients first.
        /// </summary>
        public void reloadClients()
        {
            disconnectAll();
            loadClients();
            connectAll();
        }

        /// <summary>
        /// Loads one specific XMPPAccount and subscribes to all its events.
        /// </summary>
        /// <param name="acc">The account which should get loaded.</param>
        /// <returns></returns>
        private XMPPClient loadAccount(XMPPAccount acc)
        {
            XMPPClient c = new XMPPClient(acc);

            // Enable OMEMO:
            OmemoStore signalProtocolStore = new OmemoStore(acc);
            // Generate OMEMO keys if necessary:
            if (!acc.omemoKeysGenerated)
            {
                acc.generateOmemoKeys();
                AccountDBManager.INSTANCE.setAccount(acc, false);
            }
            c.enableOmemo(signalProtocolStore);

            // Sometimes the DB fails and only stores less than 100 OMEMO pre keys.
            // So if we detect an issue with that generate a new batch of OMEMO pre keys and announce the new ones.
            if (acc.OMEMO_PRE_KEYS.Count < 100)
            {
                Logger.Warn("Only " + acc.OMEMO_PRE_KEYS.Count + " found. Generating a new set.");
                acc.OMEMO_PRE_KEYS.Clear();
                acc.OMEMO_PRE_KEYS.AddRange(CryptoUtils.generateOmemoPreKeys());
                acc.omemoBundleInfoAnnounced = false;
                AccountDBManager.INSTANCE.setAccount(acc, false);
            }

            // Ensure no event gets bound multiple times:
            unsubscribeFromEvents(c);

            c.NewChatMessage += C_NewChatMessage;
            c.NewRoosterMessage += C_NewRoosterMessage;
            c.NewPresence += C_NewPresence;
            // Requesting roster if connected
            c.ConnectionStateChanged += C_ConnectionStateChanged;
            c.MessageSend += C_MessageSend;
            c.NewBookmarksResultMessage += C_NewBookmarksResultMessage;
            c.NewDeliveryReceipt += C_NewDeliveryReceipt;
            c.getXMPPAccount().PropertyChanged += ConnectionHandler_PropertyChanged;
            c.OmemoSessionBuildError += C_OmemoSessionBuildError;
            return c;
        }

        /// <summary>
        /// Unsubscribes from all events of the given XMPPClient.
        /// </summary>
        private void unsubscribeFromEvents(XMPPClient c)
        {
            c.NewChatMessage -= C_NewChatMessage;
            c.NewRoosterMessage -= C_NewRoosterMessage;
            c.NewPresence -= C_NewPresence;
            c.ConnectionStateChanged -= C_ConnectionStateChanged;
            c.MessageSend -= C_MessageSend;
            c.NewBookmarksResultMessage -= C_NewBookmarksResultMessage;
            c.NewDeliveryReceipt -= C_NewDeliveryReceipt;
            c.getXMPPAccount().PropertyChanged -= ConnectionHandler_PropertyChanged;
            c.OmemoSessionBuildError -= C_OmemoSessionBuildError;
        }

        /// <summary>
        /// Called once a client enters the 'Disconnected' or 'Error' state.
        /// </summary>
        /// <param name="client">The Client which entered the state.</param>
        private void onClientDisconnectedOrError(XMPPClient client)
        {
            ChatDBManager.INSTANCE.resetPresence(client.getXMPPAccount().getBareJid());
            MUCHandler.INSTANCE.onClientDisconnected(client);
        }

        /// <summary>
        /// Called once a client enters the 'Disconnecting' state.
        /// </summary>
        /// <param name="client">The Client which entered the state.</param>
        private void onClientDisconneting(XMPPClient client)
        {
            MUCHandler.INSTANCE.onClientDisconnecting(client);
        }

        /// <summary>
        /// Called once a client enters the 'Connected' state.
        /// </summary>
        /// <param name="client">The Client which entered the state.</param>
        private void onClientConnected(XMPPClient client)
        {
            Task.Run(async () =>
            {
                await client.GENERAL_COMMAND_HELPER.sendRequestRosterMessageAsync();
                client.PUB_SUB_COMMAND_HELPER.requestBookmars_xep_0048(null, null);
                MUCHandler.INSTANCE.onClientConnected(client);
                ClientConnected?.Invoke(this, new ClientConnectedEventArgs(client));
                await sendOutsandingChatMessagesAsync(client);
            });
        }

        private async Task sendOutsandingChatMessagesAsync(XMPPClient client)
        {
            IList<ChatMessageTable> toSend = ChatDBManager.INSTANCE.getChatMessages(client.getXMPPAccount().getBareJid(), MessageState.SENDING);
            Logger.Info("Sending " + toSend.Count + " outstanding chat messages for: " + client.getXMPPAccount().getBareJid());
            await sendOutsandingChatMessagesAsync(client, toSend);
            Logger.Info("Finished sending outstanding chat messages for: " + client.getXMPPAccount().getBareJid());

            IList<ChatMessageTable> toEncrypt = ChatDBManager.INSTANCE.getChatMessages(client.getXMPPAccount().getBareJid(), MessageState.TO_ENCRYPT);
            Logger.Info("Sending " + toSend.Count + " outstanding OMEMO chat messages for: " + client.getXMPPAccount().getBareJid());
            await sendOutsandingChatMessagesAsync(client, toEncrypt);
            Logger.Info("Finished sending outstanding OMEMO chat messages for: " + client.getXMPPAccount().getBareJid());
        }

        /// <summary>
        /// Sends all chat messages passed to it.
        /// </summary>
        /// <param name="client">The XMPPClient that should be used to send the messages.</param>
        /// <param name="messages">A list of chat messages to send. They SHOULD be sorted by their chatId for optimal performance.</param>
        private async Task sendOutsandingChatMessagesAsync(XMPPClient client, IList<ChatMessageTable> messages)
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
                    await client.sendOmemoMessageAsync(omemoMsg, chat.chatJabberId, client.getXMPPAccount().getBareJid());
                }
                else
                {
                    await client.sendAsync(msg);
                }
            }
        }

        /// <summary>
        /// Reconnects a given client.
        /// </summary>
        /// <param name="client">The client, for which a reconnect should get performed.</param>
        /// <returns></returns>
        private async Task reconnectClientAsync(XMPPClient client)
        {
            if (client.getXMPPAccount().disabled)
            {
                // Only disconnect if the client is disabled:
                await client.disconnectAsync();
            }
            else
            {
                await client.reconnectAsync();
            }
        }

        private void setOmemoChatMessagesSendFailed(IList<OmemoMessageMessage> messages, ChatTable chat)
        {
            foreach (OmemoMessageMessage msg in messages)
            {
                string msgId = ChatMessageTable.generateId(msg.ID, chat.id);
                ChatDBManager.INSTANCE.updateChatMessageState(msgId, MessageState.ENCRYPT_FAILED);
            }
        }

        #endregion

        #region --Misc Methods (Protected)--


        #endregion
        //--------------------------------------------------------Events:---------------------------------------------------------------------\\
        #region --Events--
        private void C_ConnectionStateChanged(XMPPClient client, ConnectionStateChangedEventArgs args)
        {
            switch (args.newState)
            {
                case ConnectionState.CONNECTED:
                    onClientConnected(client);
                    break;

                case ConnectionState.DISCONNECTING:
                    onClientDisconneting(client);
                    break;

                case ConnectionState.ERROR:
                case ConnectionState.DISCONNECTED:
                    onClientDisconnectedOrError(client);
                    break;

                default:
                    break;
            }
        }

        private async Task answerPresenceProbeAsync(string from, string to, ChatTable chat, XMPPClient client, PresenceMessage msg)
        {
            XMPPAccount account = client.getXMPPAccount();
            PresenceMessage answer = null;
            if (chat is null)
            {
                answer = new PresenceErrorMessage(account.getFullJid(), from, PresenceErrorType.FORBIDDEN);
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
            await client.sendAsync(answer);
        }

        private async void C_NewPresence(XMPPClient client, XMPP_API.Classes.Events.NewPresenceMessageEventArgs args)
        {
            string from = Utils.getBareJidFromFullJid(args.PRESENCE_MESSAGE.getFrom());

            // If received a presence message from your own account, ignore it:
            if (string.Equals(from, client.getXMPPAccount().getBareJid()))
            {
                return;
            }

            string to = client.getXMPPAccount().getBareJid();
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
                    await answerPresenceProbeAsync(from, to, chat, client, args.PRESENCE_MESSAGE);
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

        private void C_NewRoosterMessage(IMessageSender sender, NewValidMessageEventArgs args)
        {
            if (args.MESSAGE is RosterResultMessage msg && sender is XMPPClient client)
            {
                string to = client.getXMPPAccount().getBareJid();
                string type = msg.TYPE;

                if (string.Equals(type, IQMessage.RESULT))
                {
                    ChatDBManager.INSTANCE.setAllNotInRoster(client.getXMPPAccount().getBareJid());
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

        private async void C_NewChatMessage(XMPPClient client, XMPP_API.Classes.Network.Events.NewChatMessageEventArgs args)
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
            string id;
            if (msg.CC_TYPE == CarbonCopyType.SENT)
            {
                id = ChatTable.generateId(to, from);
            }
            else
            {
                id = ChatTable.generateId(from, to);
            }

            // Check if device id is valid and if, decrypt the OMEMO messages:
            if (msg is OmemoMessageMessage omemoMessage)
            {
                OmemoHelper helper = client.getOmemoHelper();
                if (helper is null)
                {
                    C_OmemoSessionBuildError(client, new OmemoSessionBuildErrorEventArgs(id, OmemoSessionBuildError.KEY_ERROR, new List<OmemoMessageMessage> { omemoMessage }));
                    Logger.Error("Failed to decrypt OMEMO message - OmemoHelper is null");
                    return;
                }
                else if (!client.getXMPPAccount().checkOmemoKeys())
                {
                    C_OmemoSessionBuildError(client, new OmemoSessionBuildErrorEventArgs(id, OmemoSessionBuildError.KEY_ERROR, new List<OmemoMessageMessage> { omemoMessage }));
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
                    await client.sendAsync(receiptMessage);
                });
            }

            ChatDBManager.INSTANCE.setChatMessage(message, !doesMessageExist, doesMessageExist && !isMUCMessage);

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
                                    if (message.isImage)
                                    {
                                        ToastHelper.showChatTextImageToast(message, chat);
                                    }
                                    else
                                    {
                                        ToastHelper.showChatTextToast(message, chat);
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

        private async void INSTANCE_AccountChanged(AccountDBManager handler, AccountChangedEventArgs args)
        {
            CLIENT_SEMA.Wait();
            for (int i = 0; i < CLIENTS.Count; i++)
            {
                if (Equals(CLIENTS[i].getXMPPAccount().getBareJid(), args.ACCOUNT.getBareJid()))
                {
                    // Disconnect first:
                    await CLIENTS[i].disconnectAsync();

                    if (args.REMOVED)
                    {
                        unsubscribeFromEvents(CLIENTS[i]);
                        CLIENTS.RemoveAt(i);
                    }
                    else
                    {
                        CLIENTS[i].setAccount(args.ACCOUNT);
                        if (!CLIENTS[i].getXMPPAccount().disabled)
                        {
                            CLIENTS[i].connect();
                        }
                    }
                    CLIENT_SEMA.Release();
                    return;
                }
            }

            // Account got added:
            if (!args.REMOVED)
            {
                XMPPClient client = loadAccount(args.ACCOUNT);
                if (!client.getXMPPAccount().disabled)
                {
                    client.connect();
                }
                CLIENTS.Add(client);
            }
            CLIENT_SEMA.Release();
        }

        private void C_MessageSend(XMPPClient client, MessageSendEventArgs args)
        {
            ChatDBManager.INSTANCE.updateChatMessageState(args.CHAT_MESSAGE_ID, MessageState.SEND);
        }

        private void C_NewBookmarksResultMessage(XMPPClient client, NewBookmarksResultMessageEventArgs args)
        {
            foreach (ConferenceItem c in args.BOOKMARKS_MESSAGE.STORAGE.CONFERENCES)
            {
                bool newMUC = false;
                string to = client.getXMPPAccount().getBareJid();
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

        private void C_NewDeliveryReceipt(XMPPClient client, NewDeliveryReceiptEventArgs args)
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
                AccountDBManager.INSTANCE.setAccount(account, false);
            }
        }

        private void C_OmemoSessionBuildError(XMPPClient client, XMPP_API.Classes.Events.OmemoSessionBuildErrorEventArgs args)
        {
            Task.Run(() =>
            {
                ChatTable chat = ChatDBManager.INSTANCE.getChat(ChatTable.generateId(args.CHAT_JID, client.getXMPPAccount().getBareJid()));
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
                    ChatDBManager.INSTANCE.setChatMessage(msg, true, false);

                    // Set chat messages to encrypted failed:
                    setOmemoChatMessagesSendFailed(args.MESSAGES, chat);
                }
            });
        }

        private void CLIENTS_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            ClientsCollectionChanged?.Invoke(this, e);
        }
        #endregion
    }
}
