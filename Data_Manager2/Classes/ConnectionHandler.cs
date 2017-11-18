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

        private static List<XMPPClient> clients = null;
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

        private void disconnectAll()
        {
            Parallel.ForEach(clients, async (c) =>
            {
                await c.disconnectAsync();
                ChatManager.INSTANCE.resetPresence(c.getXMPPAccount().getIdAndDomain());
            });
        }

        private void reconnectAll()
        {
            Parallel.ForEach(clients, async (c) =>
            {
                await c.disconnectAsync();
                ChatManager.INSTANCE.resetPresence(c.getXMPPAccount().getIdAndDomain());
                await c.connectAsync();
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

        private void loadAccount(XMPPAccount acc)
        {
            XMPPClient c = new XMPPClient(acc);
            c.NewChatMessage += C_NewChatMessage;
            c.NewRoosterMessage += C_NewRoosterMessage;
            c.NewPresence += C_NewPresence;
            c.ConnectionStateChanged += C_ConnectionStateChanged; // Requesting roster if connected
            clients.Add(c);
        }

        #endregion

        #region --Misc Methods (Protected)--


        #endregion
        //--------------------------------------------------------Events:---------------------------------------------------------------------\\
        #region --Events--
        private async void C_ConnectionStateChanged(XMPPClient client, ConnectionState state)
        {
            if (state == ConnectionState.CONNECTED)
            {
                await client.requestRoosterAsync();
            }
        }

        private void C_NewPresence(XMPPClient client, XMPP_API.Classes.Events.NewPresenceEventArgs args)
        {
            string from = Utils.removeResourceFromJabberid(args.getFrom());
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

        private void C_NewRoosterMessage(XMPPClient client, XMPP_API.Classes.Network.Events.NewPresenceEventArgs args)
        {
            if (args.getMessage() is RosterMessage)
            {
                RosterMessage msg = args.getMessage() as RosterMessage;
                XMPPAccount account = client.getXMPPAccount();
                string to = client.getXMPPAccount().getIdAndDomain();
                string type = msg.getMessageType();

                if (type != null && type.Equals(IQMessage.RESULT))
                {
                    ChatManager.INSTANCE.setAllNotInRoster(client.getXMPPAccount().getIdAndDomain());
                }
                else
                {
                    // No roster result => return
                    return;
                }
                foreach (RosterItem item in msg.getItems())
                {
                    string from = item.getJabberId();
                    ChatTable chat = ChatManager.INSTANCE.getChat(ChatTable.generateId(from, to));
                    if (chat != null)
                    {
                        chat.subscription = item.getSubscription();
                        chat.inRoster = !item.getSubscription().Equals("remove");
                        chat.ask = item.getAsk();
                    }
                    else
                    {
                        chat = new ChatTable()
                        {
                            id = ChatTable.generateId(from, to),
                            chatJabberId = from,
                            userAccountId = to,
                            subscription = item.getSubscription(),
                            lastActive = DateTime.Now,
                            muted = false,
                            inRoster = true,
                            ask = item.getAsk()
                        };
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

        private void INSTANCE_AccountChanged(AccountManager handler, Data_Manager.Classes.Events.AccountChangedEventArgs args)
        {
            if(clients != null)
            {
                Parallel.ForEach(clients, (c) =>
                {
                    if (c.getXMPPAccount().getIdAndDomain().Equals(args.ACCOUNT.getIdAndDomain()))
                    {
                        clients.Remove(c);
                    }
                });
                if (!args.REMOVED)
                {
                    loadAccount(args.ACCOUNT);
                }
            }
        }

        #endregion
    }
}
