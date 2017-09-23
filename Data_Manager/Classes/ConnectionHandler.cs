using Data_Manager.Classes.DBEntries;
using Data_Manager.Classes.Managers;
using Logging;
using System;
using System.Collections;
using System.Collections.Generic;
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
                if(c.getSeverConnectionConfiguration().Equals(account))
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
            if(xMPPClients != null)
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

                tasks.Add(Task.Factory.StartNew(async () => {
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
                if (!client.getSeverConnectionConfiguration().disabled)
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
            ChatEntry chat = ChatManager.INSTANCE.getChatEntry(args.getFrom(), client.getSeverConnectionConfiguration().getIdAndDomain());
            if (args.getPresenceType() != null && args.getPresenceType().Equals("subscribe"))
            {
                if(chat == null)
                {
                    chat = new ChatEntry()
                    {
                        id = args.getFrom(),
                        userAccountId = client.getSeverConnectionConfiguration().getIdAndDomain(),
                        inRoster = false,
                        muted = false,
                        lastActive = DateTime.Now,
                    };
                }
            }
            if(chat != null)
            {
                chat.presence = args.getPresence();
                chat.status = args.getStatus();
                chat.userAccountId = client.getSeverConnectionConfiguration().getIdAndDomain();
                if (args.getPresenceType() != null)
                {
                    chat.subscription = args.getPresenceType();
                }
                ChatManager.INSTANCE.setChatEntry(chat, true);
            }
        }

        private void Client_NewRoosterMessage(XMPPClient client, XMPP_API.Classes.Network.Events.NewPresenceEventArgs args)
        {
            if(args.getMessage() is RosterMessage)
            {
                XMPPAccount account = client.getSeverConnectionConfiguration();
                RosterMessage msg = args.getMessage() as RosterMessage;
                string type = msg.getMessageType();
                if(type != null && type.Equals(IQMessage.RESULT))
                {
                    ChatManager.INSTANCE.setAllNotInRoster(client.getSeverConnectionConfiguration().getIdAndDomain());
                }
                foreach (RosterItem item in msg.getItems())
                {
                    ChatEntry chat = ChatManager.INSTANCE.getChatEntry(item.getJabberId(), account.getIdAndDomain());
                    if(chat != null)
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
                            id = item.getJabberId(),
                            userAccountId = account.getIdAndDomain(),
                            name = item.getName(),
                            subscription = item.getSubscription(),
                            lastActive = DateTime.Now,
                            muted = false,
                            inRoster = true,
                            ask = item.getAsk()
                        };
                    }
                    ChatManager.INSTANCE.setChatEntry(chat, true);
                }
            }
        }

        private async void Client_ConnectionStateChanged(XMPPClient client, ConnectionState state)
        {
            if(state == ConnectionState.CONNECTED)
            {
                await client.requestRoosterAsync();
            }
        }

        private void Client_NewChatMessage(XMPPClient client, XMPP_API.Classes.Network.Events.NewChatMessageEventArgs args)
        {
            MessageMessage msg = args.getMessage();
            string pureJabberId = Utils.removeResourceFromJabberid(msg.getFrom());
            ChatEntry chat = ChatManager.INSTANCE.getChatEntry(pureJabberId, client.getSeverConnectionConfiguration().getIdAndDomain());
            if(chat == null)
            {
                chat = new ChatEntry(pureJabberId, Utils.removeResourceFromJabberid(msg.getTo()));
                ChatManager.INSTANCE.setChatEntry(chat, true);
            }
            ChatMessageEntry entry = new ChatMessageEntry(msg, chat);
            ChatManager.INSTANCE.setChatMessageEntry(entry);
        }

        #endregion
    }
}
