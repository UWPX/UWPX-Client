using Data_Manager.Classes.DBEntries;
using Data_Manager.Classes.Managers;
using Logging;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using XMPP_API.Classes;
using XMPP_API.Classes.Events;
using XMPP_API.Classes.Network;
using XMPP_API.Classes.Network.XML;
using XMPP_API.Classes.Network.XML.Messages;

namespace Data_Manager.Classes
{
    public class ConnectionHandler
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        public static readonly ConnectionHandler INSTANCE = new ConnectionHandler();

        private ArrayList xMPPClients;
        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        /// <summary>
        /// Basic Constructor
        /// </summary>
        /// <history>
        /// 26/08/2017 Created [Fabian Sauter]
        /// </history>
        public ConnectionHandler()
        {
            loadAllAccounts();
        }

        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--
        public ArrayList getXMPPClients()
        {
            return xMPPClients;
        }

        public XMPPClient getClientForAccount(XMPPAccount account)
        {
            foreach (XMPPClient c in xMPPClients)
            {
                if (c.getXMPPAccount().Equals(account))
                {
                    return c;
                }
            }
            return null;
        }

        #endregion
        //--------------------------------------------------------Misc Methods:---------------------------------------------------------------\\
        #region --Misc Methods (Public)--
        public async Task transferSocketOwnershipAsync()
        {
            if (xMPPClients != null)
            {
                foreach (XMPPClient c in xMPPClients)
                {
                    await c.transferSocketOwnershipAsync();
                }
            }
        }

        public void connect()
        {
            connectToAllAccounts();
        }

        public void reloadAllAccounts()
        {
            Task.Factory.StartNew(() =>
            {
                closeAllAccount();
                loadAllAccounts();
                connectToAllAccounts();
            });
        }

        public void closeAllAccount()
        {
            List<Task> tasks = new List<Task>();
            foreach (XMPPClient client in xMPPClients)
            {

                tasks.Add(Task.Factory.StartNew(async () =>
                {
                    await client.disconnectAsync();
                }));
            }
            Task.WaitAll(tasks.ToArray());
        }

        #endregion

        #region --Misc Methods (Private)--
        private void loadAllAccounts()
        {
            xMPPClients = new ArrayList();
            foreach (XMPPAccount account in UserManager.INSTANCE.getAccounts())
            {
                XMPPClient client = new XMPPClient(account);
                client.NewRoosterMessage += Client_NewRoosterMessage;
                client.ConnectionStateChanged += Client_ConnectionStateChanged;
                client.NewChatMessage += Client_NewChatMessage;
                client.NewPresence += Client_NewPresence;
                xMPPClients.Add(client);
            }

            // Socket background task:
            if (xMPPClients.Count > 0 && !Settings.getSettingBoolean(SettingsConsts.DISABLE_SOCKET_BACKGROUND_TASK))
            {
                MyBackgroundTaskHelper.registerBackgroundTask();
            }
        }

        private void connectToAllAccounts()
        {
            foreach (XMPPClient client in xMPPClients)
            {
                if (!client.getXMPPAccount().disabled)
                {
                    switch (client.getConnetionState())
                    {
                        case ConnectionState.DISCONNECTING:
                        case ConnectionState.DISCONNECTED:
                        case ConnectionState.ERROR:
                            Task.Factory.StartNew(async () =>
                            {
                                await client.connectAsync();
                            }, TaskCreationOptions.None).ContinueWith((Task prev) =>
                            {
                                if (prev.Exception != null)
                                {
                                    Logger.Error("Error during connectToAllAccounts() - ConnectionHandler!", prev.Exception);
                                }
                            });
                        break;
                    }
                }
            }
        }

        #endregion

        #region --Misc Methods (Protected)--


        #endregion
        //--------------------------------------------------------Events:---------------------------------------------------------------------\\
        #region --Events--
        private void Client_NewPresence(XMPPClient client, NewPresenceEventArgs args)
        {
            string from = Utils.removeResourceFromJabberid(args.getFrom());
            string to = client.getXMPPAccount().getIdAndDomain();
            string id = ChatEntry.generateId(from, to);
            ChatEntry chat = ChatManager.INSTANCE.getChatEntry(id);
            switch (args.getPresenceType())
            {
                case "subscribe":
                case "unsubscribed":
                    if(chat == null)
                    {
                        chat = new ChatEntry()
                        {
                            id = id,
                            chatJabberId = from,
                            userAccountId = to,
                            inRoster = false,
                            muted = false,
                            lastActive = DateTime.Now,
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
                ChatManager.INSTANCE.setChatEntry(chat, true);
            }
        }

        private void Client_NewRoosterMessage(XMPPClient client, XMPP_API.Classes.Network.Events.NewPresenceEventArgs args)
        {
            if (args.getMessage() is RosterMessage)
            {
                XMPPAccount account = client.getXMPPAccount();
                RosterMessage msg = args.getMessage() as RosterMessage;
                //Debug.Assert(account.getIdAndDomain().Equals(Utils.removeResourceFromJabberid((args.getMessage() as RosterMessage).getTo()))); // [Assert]
                string type = msg.getMessageType();
                string to = client.getXMPPAccount().getIdAndDomain();
                if (type != null && type.Equals(IQMessage.RESULT))
                {
                    ChatManager.INSTANCE.setAllNotInRoster(client.getXMPPAccount().getIdAndDomain());
                }
                foreach (RosterItem item in msg.getItems())
                {
                    string from = item.getJabberId();
                    ChatEntry chat = ChatManager.INSTANCE.getChatEntry(ChatEntry.generateId(from, to));
                    if (chat != null)
                    {
                        chat.name = item.getName();
                        chat.subscription = item.getSubscription();
                        chat.inRoster = !item.getSubscription().Equals("remove");
                        chat.ask = item.getAsk();
                    }
                    else
                    {
                        chat = new ChatEntry()
                        {
                            id = ChatEntry.generateId(from, account.getIdAndDomain()),
                            chatJabberId = from,
                            userAccountId = account.getIdAndDomain(),
                            name = item.getName(),
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

                    ChatManager.INSTANCE.setChatEntry(chat, true);
                }
            }
        }

        private async void Client_ConnectionStateChanged(XMPPClient client, ConnectionState state)
        {
            if (state == ConnectionState.CONNECTED)
            {
                await client.requestRoosterAsync();
            }
        }

        private void Client_NewChatMessage(XMPPClient client, XMPP_API.Classes.Network.Events.NewChatMessageEventArgs args)
        {
            MessageMessage msg = args.getMessage();
            string to = Utils.removeResourceFromJabberid(msg.getTo());
            string from = Utils.removeResourceFromJabberid(msg.getFrom());
            ChatEntry chat = ChatManager.INSTANCE.getChatEntry(ChatEntry.generateId(from, to));
            if (chat == null)
            {
                chat = new ChatEntry(from, to);
                ChatManager.INSTANCE.setChatEntry(chat, true);
            }

            Task.Factory.StartNew(() =>
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
            });

            ChatMessageEntry entry = new ChatMessageEntry(msg, chat);
            entry.state = MessageState.UNREAD;
            ChatManager.INSTANCE.setChatMessageEntry(entry, true);
        }

        #endregion
    }
}
