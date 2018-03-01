using Data_Manager.Classes.Events;
using Data_Manager2.Classes.DBManager;
using Data_Manager2.Classes.DBTables;
using Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using XMPP_API.Classes;
using XMPP_API.Classes.Network;
using XMPP_API.Classes.Network.XML;
using XMPP_API.Classes.Network.XML.Messages;
using XMPP_API.Classes.Network.XML.Messages.XEP_0045;
using XMPP_API.Classes.Network.XML.Messages.XEP_0048_1_0;

namespace Data_Manager2.Classes
{
    public class ConnectionHandler
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        public static readonly ConnectionHandler INSTANCE = new ConnectionHandler();
        private static List<XMPPClient> clients;

        public delegate void ClientConnectedHandler(ConnectionHandler handler, ClientConnectedEventArgs args);

        public event ClientConnectedHandler ClientConnected;
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
            clients = null;
            loadAccounts();
            AccountDBManager.INSTANCE.AccountChanged += INSTANCE_AccountChanged;
        }

        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--
        public XMPPClient getClient(string iDAndDomain)
        {
            foreach (XMPPClient c in clients)
            {
                if (c.getXMPPAccount().getIdAndDomain().Equals(iDAndDomain))
                {
                    return c;
                }
            }
            return null;
        }

        public List<XMPPClient> getClients()
        {
            return clients;
        }

        #endregion
        //--------------------------------------------------------Misc Methods:---------------------------------------------------------------\\
        #region --Misc Methods (Public)--
        public void connectAll()
        {
            if (clients == null || clients.Count <= 0)
            {
                return;
            }
            Parallel.ForEach(clients, (c) =>
            {
                if (!c.getXMPPAccount().disabled)
                {
                    switch (c.getConnetionState())
                    {
                        case ConnectionState.DISCONNECTED:
                        case ConnectionState.DISCONNECTING:
                        case ConnectionState.ERROR:
                            Task.Run(async () =>
                            {
                                await c.connectAsync();
                            }).ContinueWith((Task prev) =>
                            {
                                if (prev.Exception != null)
                                {
                                    Logger.Error("Error during connectAll() - ConnectionHandler!", prev.Exception);
                                }
                            });
                            break;
                    }
                }
            });
        }

        public void disconnectAll()
        {
            if (clients == null || clients.Count <= 0)
            {
                return;
            }
            Parallel.ForEach(clients, async (c) =>
            {
                await c.disconnectAsync();
            });
        }

        public void reconnectAll()
        {
            if (clients == null || clients.Count <= 0)
            {
                return;
            }
            Parallel.ForEach(clients, async (c) =>
            {
                await reconnectClientAsync(c);
            });
        }

        #endregion

        #region --Misc Methods (Private)--
        private void loadAccounts()
        {
            if (clients == null)
            {
                clients = new List<XMPPClient>();
            }
            else
            {
                clients.Clear();
            }
            foreach (XMPPAccount acc in AccountDBManager.INSTANCE.loadAllAccounts())
            {
                loadAccount(acc);
            }
        }

        private XMPPClient loadAccount(XMPPAccount acc)
        {
            XMPPClient c = new XMPPClient(acc);

            // Ensure no event gets bound multiple times:
            unloadAccount(c);

            c.NewChatMessage += C_NewChatMessage;
            c.NewRoosterMessage += C_NewRoosterMessage;
            c.NewPresence += C_NewPresence;
            // Requesting roster if connected
            c.ConnectionStateChanged += C_ConnectionStateChanged;
            c.MessageSend += C_MessageSend;
            c.NewBookmarksResultMessage += C_NewBookmarksResultMessage;
            clients.Add(c);
            return c;
        }

        private void unloadAccount(XMPPClient c)
        {
            c.NewChatMessage -= C_NewChatMessage;
            c.NewRoosterMessage -= C_NewRoosterMessage;
            c.NewPresence -= C_NewPresence;
            c.ConnectionStateChanged -= C_ConnectionStateChanged;
            c.MessageSend -= C_MessageSend;
            c.NewBookmarksResultMessage -= C_NewBookmarksResultMessage;
        }

        private void onClientDisconnectedOrError(XMPPClient client)
        {
            ChatDBManager.INSTANCE.resetPresence(client.getXMPPAccount().getIdAndDomain());
            MUCHandler.INSTANCE.onClientDisconnected(client);
        }

        private void onClientDisconneting(XMPPClient client)
        {
            MUCHandler.INSTANCE.onClientDisconnecting(client);
        }

        private void onClientConnected(XMPPClient client)
        {
            Task.Run(async () =>
            {
                await client.requestRoosterAsync();
                await client.requestBookmarksAsync();
                MUCHandler.INSTANCE.onClientConnected(client);
                ClientConnected?.Invoke(this, new ClientConnectedEventArgs(client));
            });
        }

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

        #endregion

        #region --Misc Methods (Protected)--


        #endregion
        //--------------------------------------------------------Events:---------------------------------------------------------------------\\
        #region --Events--
        private void C_ConnectionStateChanged(XMPPClient client, XMPP_API.Classes.Network.Events.ConnectionStateChangedEventArgs args)
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
            }
        }

        private void C_NewPresence(XMPPClient client, XMPP_API.Classes.Events.NewPresenceMessageEventArgs args)
        {
            string from = Utils.getBareJidFromFullJid(args.PRESENCE_MESSAGE.getFrom());

            // If received a presence message from your own account return
            if (string.Equals(from, client.getXMPPAccount().getIdAndDomain()))
            {
                return;
            }

            string to = client.getXMPPAccount().getIdAndDomain();
            string id = ChatTable.generateId(from, to);
            ChatTable chat = ChatDBManager.INSTANCE.getChat(id);
            switch (args.PRESENCE_MESSAGE.TYPE)
            {
                case "subscribe":
                case "unsubscribed":
                    if (chat == null)
                    {
                        chat = new ChatTable()
                        {
                            id = id,
                            chatJabberId = from,
                            userAccountId = to,
                            inRoster = false,
                            muted = false,
                            lastActive = DateTime.Now,
                            ask = null,
                            status = null,
                            chatState = null
                        };
                    }
                    chat.subscription = args.PRESENCE_MESSAGE.TYPE;
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

        private void C_NewRoosterMessage(XMPPClient client, XMPP_API.Classes.Network.Events.NewValidMessageEventArgs args)
        {
            if (args.getMessage() is RosterMessage)
            {
                RosterMessage msg = args.getMessage() as RosterMessage;
                XMPPAccount account = client.getXMPPAccount();
                string to = client.getXMPPAccount().getIdAndDomain();
                string type = msg.getMessageType();

                if (string.Equals(type, IQMessage.RESULT))
                {
                    ChatDBManager.INSTANCE.setAllNotInRoster(client.getXMPPAccount().getIdAndDomain());
                }
                else if (type == null || !string.Equals(type, IQMessage.SET))
                {
                    // No roster result or set => return
                    return;
                }

                foreach (RosterItem item in msg.getItems())
                {
                    string from = item.JID;
                    string id = ChatTable.generateId(from, to);
                    ChatTable chat = ChatDBManager.INSTANCE.getChat(id);
                    if (chat != null)
                    {
                        chat.subscription = item.SUBSCRIPTION;
                        chat.inRoster = !item.SUBSCRIPTION.Equals("remove");
                        chat.ask = item.ASK;
                    }
                    else if (!Equals(item.SUBSCRIPTION, "remove"))
                    {
                        chat = new ChatTable()
                        {
                            id = id,
                            chatJabberId = from,
                            userAccountId = to,
                            subscription = item.SUBSCRIPTION,
                            lastActive = DateTime.Now,
                            muted = false,
                            inRoster = true,
                            ask = item.ASK,
                            chatType = ChatType.CHAT
                        };
                    }
                    else
                    {
                        continue;
                    }

                    switch (chat.subscription)
                    {
                        case "unsubscribe":
                        case "from":
                        case "none":
                        case "pending":
                        case null:
                            chat.presence = Presence.Unavailable;
                            break;
                    }

                    ChatDBManager.INSTANCE.setChat(chat, false, true);
                }
            }
        }

        private void C_NewChatMessage(XMPPClient client, XMPP_API.Classes.Network.Events.NewChatMessageEventArgs args)
        {
            MessageMessage msg = args.getMessage();

            // Handel MUC room subject messages:
            if (msg is MUCRoomSubjectMessage)
            {
                MUCHandler.INSTANCE.onMUCRoomSubjectMessage(msg as MUCRoomSubjectMessage);
                return;
            }

            string to = Utils.getBareJidFromFullJid(msg.getTo());
            string from = Utils.getBareJidFromFullJid(msg.getFrom());
            string id = ChatTable.generateId(from, to);

            ChatTable chat = ChatDBManager.INSTANCE.getChat(id);
            if (chat == null)
            {
                chat = new ChatTable()
                {
                    id = id,
                    chatJabberId = from,
                    userAccountId = to,
                    ask = null,
                    inRoster = false,
                    lastActive = DateTime.Now,
                    muted = false,
                    presence = Presence.Unavailable,
                    status = null,
                    subscription = null,
                    chatType = Equals(msg.getType(), MessageMessage.TYPE_CHAT) ? ChatType.MUC : ChatType.CHAT,
                };
                ChatDBManager.INSTANCE.setChat(chat, false, true);
            }

            // ToDo re-implement show toast message
            /*Task.Run(() =>
            {
                if (!msg.getToasted())
                {
                    switch (msg.getType())
                    {
                        case "chat":
                            ToastHelper.showChatTextToast(msg.getMessage(), msg.getId(), chat);
                            break;
                    }
                    msg.setToasted();
                }
            });*/

            ChatMessageTable message = new ChatMessageTable(msg, chat)
            {
                state = Data_Manager.Classes.MessageState.UNREAD
            };

            // Filter MUC messages that got send
            // and are now returned to the sender as a part of distributing them to everybody:
            if (string.Equals(MessageMessage.TYPE_GROUPCHAT, message.type))
            {
                if (Equals(message.fromUser, client.getXMPPAccount().getIdAndDomain()))
                {
                    return;
                }
            }

            bool msgExists = ChatDBManager.INSTANCE.getChatMessageById(msg.getId() + '_' + chat.id) != null;
            ChatDBManager.INSTANCE.setChatMessage(message, !msgExists, msgExists);
        }

        private async void INSTANCE_AccountChanged(AccountDBManager handler, AccountChangedEventArgs args)
        {
            if (clients != null && clients.Count > 0)
            {
                Parallel.ForEach(clients.ToArray(), async (c) =>
                {
                    if (c.getXMPPAccount().getIdAndDomain().Equals(args.ACCOUNT.getIdAndDomain()))
                    {
                        clients.Remove(c);
                        try
                        {
                            await c.disconnectAsync();
                        }
                        catch
                        {

                        }
                    }
                });
            }
            if (!args.REMOVED)
            {
                XMPPClient c = loadAccount(args.ACCOUNT);
                if (!c.getXMPPAccount().disabled)
                {
                    await c.connectAsync();
                }
            }
        }

        private void C_MessageSend(XMPPClient client, XMPP_API.Classes.Network.Events.MessageSendEventArgs args)
        {
            ChatDBManager.INSTANCE.updateChatMessageState(args.ID, Data_Manager.Classes.MessageState.SEND);
        }

        private void C_NewBookmarksResultMessage(XMPPClient client, XMPP_API.Classes.Network.Events.NewBookmarksResultMessageEventArgs args)
        {
            BookmarksResultMessage msg = args.BOOKMARKS_MESSAGE;

            foreach (ConferenceItem c in args.BOOKMARKS_MESSAGE.CONFERENCE_ITEMS)
            {
                string to = client.getXMPPAccount().getIdAndDomain();
                string from = c.jid;
                string id = ChatTable.generateId(from, to);
                ChatTable chat = ChatDBManager.INSTANCE.getChat(id);
                if (chat == null)
                {
                    chat = new ChatTable()
                    {
                        id = id,
                        chatJabberId = from,
                        userAccountId = to,
                        ask = null,
                        lastActive = DateTime.Now,
                        muted = false,
                        status = null,
                        subscription = null
                    };
                }
                chat.chatType = ChatType.MUC;
                chat.inRoster = true;
                chat.presence = Presence.Unavailable;

                ChatDBManager.INSTANCE.setChat(chat, false, true);
            }
        }

        #endregion
    }
}
