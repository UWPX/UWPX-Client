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
            AccountManager.INSTANCE.AccountChanged += INSTANCE_AccountChanged;
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
            Parallel.ForEach(clients, async (c) =>
            {
                if (!c.getXMPPAccount().disabled)
                {
                    switch (c.getConnetionState())
                    {
                        case ConnectionState.DISCONNECTED:
                        case ConnectionState.DISCONNECTING:
                        case ConnectionState.ERROR:
                            await Task.Factory.StartNew(async () =>
                            {
                                await c.connectAsync();
                            }, TaskCreationOptions.None).ContinueWith((Task prev) =>
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
                ChatManager.INSTANCE.resetPresence(c.getXMPPAccount().getIdAndDomain());
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
                await c.disconnectAsync();
                ChatManager.INSTANCE.resetPresence(c.getXMPPAccount().getIdAndDomain());
                await Task.Delay(200);
                if (!c.getXMPPAccount().disabled)
                {
                    await c.connectAsync();
                }
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
            foreach (XMPPAccount acc in AccountManager.INSTANCE.loadAllAccounts())
            {
                loadAccount(acc);
            }
        }

        private XMPPClient loadAccount(XMPPAccount acc)
        {
            XMPPClient c = new XMPPClient(acc);
            c.NewChatMessage += C_NewChatMessage;
            c.NewRoosterMessage += C_NewRoosterMessage;
            c.NewPresence += C_NewPresence;
            c.ConnectionStateChanged += C_ConnectionStateChanged; // Requesting roster if connected
            clients.Add(c);
            return c;
        }

        #endregion

        #region --Misc Methods (Protected)--


        #endregion
        //--------------------------------------------------------Events:---------------------------------------------------------------------\\
        #region --Events--
        private async void C_ConnectionStateChanged(XMPPClient client, XMPP_API.Classes.Network.Events.ConnectionStateChangedEventArgs args)
        {
            switch (args.newState)
            {
                case ConnectionState.CONNECTED:
                    await client.requestRoosterAsync();
                    ClientConnected?.Invoke(this, new ClientConnectedEventArgs(client));
                    break;
                case ConnectionState.ERROR:
                case ConnectionState.DISCONNECTED:
                    ChatManager.INSTANCE.resetPresence(client.getXMPPAccount().getIdAndDomain());
                    break;
            }
        }

        private void C_NewPresence(XMPPClient client, XMPP_API.Classes.Events.NewPresenceMessageEventArgs args)
        {
            string from = Utils.removeResourceFromJabberid(args.getFrom());

            // If received a presence message from your own account return
            if (string.Equals(from, client.getXMPPAccount().getIdAndDomain()))
            {
                return;
            }

            string to = client.getXMPPAccount().getIdAndDomain();
            string id = ChatTable.generateId(from, to);
            ChatTable chat = ChatManager.INSTANCE.getChat(id);
            switch (args.getPresenceType())
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
                    chat.subscription = args.getPresenceType();
                    break;
            }

            if (chat != null)
            {
                chat.id = id;
                chat.chatJabberId = from;
                chat.userAccountId = to;
                chat.status = args.getStatus();
                switch (args.getPresence())
                {
                    case Presence.NotDefined:
                        break;
                    default:
                        chat.presence = args.getPresence();
                        break;
                }
                ChatManager.INSTANCE.setChat(chat, false, true);
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
                    ChatManager.INSTANCE.setAllNotInRoster(client.getXMPPAccount().getIdAndDomain());
                }
                else if (type == null || !string.Equals(type, IQMessage.SET))
                {
                    // No roster result or set => return
                    return;
                }
                foreach (RosterItem item in msg.getItems())
                {
                    string from = item.getJabberId();
                    string id = ChatTable.generateId(from, to);
                    ChatTable chat = ChatManager.INSTANCE.getChat(id);
                    if (chat != null)
                    {
                        chat.id = id;
                        chat.chatJabberId = from;
                        chat.userAccountId = to;
                        chat.subscription = item.getSubscription();
                        chat.inRoster = !item.getSubscription().Equals("remove");
                        chat.ask = item.getAsk();
                    }
                    else if (!string.Equals(item.getSubscription(), "remove"))
                    {
                        chat = new ChatTable()
                        {
                            id = id,
                            chatJabberId = from,
                            userAccountId = to,
                            subscription = item.getSubscription(),
                            lastActive = DateTime.Now,
                            muted = false,
                            inRoster = true,
                            ask = item.getAsk()
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
                            chat.presence = Presence.Unavailable;
                            break;
                        default:
                            break;
                    }

                    ChatManager.INSTANCE.setChat(chat, false, true);
                }
            }
        }

        private void C_NewChatMessage(XMPPClient client, XMPP_API.Classes.Network.Events.NewChatMessageEventArgs args)
        {
            MessageMessage msg = args.getMessage();
            string to = Utils.removeResourceFromJabberid(msg.getTo());
            string from = Utils.removeResourceFromJabberid(msg.getFrom());
            string id = ChatTable.generateId(from, to);
            ChatTable chat = ChatManager.INSTANCE.getChat(id);
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
                    subscription = null
                };
                ChatManager.INSTANCE.setChat(chat, false, true);
            }

            // ToDo re-implement show toast message
            /*Task.Factory.StartNew(() =>
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

            ChatMessageTable message = new ChatMessageTable(msg, chat);
            message.state = Data_Manager.Classes.MessageState.UNREAD;
            ChatManager.INSTANCE.setChatMessageEntry(message, true);
        }

        private async void INSTANCE_AccountChanged(AccountManager handler, AccountChangedEventArgs args)
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

        #endregion
    }
}
