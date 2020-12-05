﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using Data_Manager2.Classes.DBManager;
using Data_Manager2.Classes.DBTables;
using Logging;
using XMPP_API.Classes;
using XMPP_API.Classes.Network;
using XMPP_API.Classes.Network.Events;
using XMPP_API.Classes.Network.XML.Messages;
using XMPP_API.Classes.Network.XML.Messages.Helper;
using XMPP_API.Classes.Network.XML.Messages.XEP_0313;
using XMPP_API.Classes.Network.XML.Messages.XEP_0384;

namespace Data_Manager2.Classes
{
    internal enum SetupState
    {
        NOT_STARTED,
        SENDING_OUTSTANDING_MESSAGES,
        REQUESTING_BOOKMARKS,
        REQUESTING_ROSTER,
        REQUESTING_MAM,
        INITIALISING_OMEMO_KEYS,
        DONE,
        CANCELED
    }


    public class PostClientConnectedHandler: IDisposable
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        private readonly XMPPClient client;

        private SetupState state = SetupState.NOT_STARTED;

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        public PostClientConnectedHandler(XMPPClient client)
        {
            this.client = client;
            this.client.ConnectionStateChanged += OnConnectionStateChanged;
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
                    Logger.Info("Starting post client connected process for: " + client.getXMPPAccount().getBareJid());
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
                    state = SetupState.DONE;
                    Continue();
                    break;

                case SetupState.DONE:
                    Logger.Info("Post client connected process done for: " + client.getXMPPAccount().getBareJid());
                    break;

                case SetupState.CANCELED:
                    Logger.Info("Post client connected process canceled for: " + client.getXMPPAccount().getBareJid());
                    break;

                default:
                    Debug.Assert(false); // Should not happen
                    break;
            }
        }

        private async Task RequestBookmarksAsync()
        {
            state = SetupState.REQUESTING_BOOKMARKS;
            await client.PUB_SUB_COMMAND_HELPER.requestBookmars_xep_0048Async();
            Continue();
        }

        private async Task RequestRosterAsync()
        {
            state = SetupState.REQUESTING_ROSTER;
            await client.GENERAL_COMMAND_HELPER.sendRequestRosterMessageAsync();
            Continue();
        }

        private async Task RequestMamAsync()
        {
            state = SetupState.REQUESTING_MAM;
            MamRequestTable mamRequest = MamDBManager.INSTANCE.getLastRequest(client.getXMPPAccount().getBareJid());
            QueryFilter filter = new QueryFilter();
            if (!(mamRequest is null))
            {
                filter.AfterId(mamRequest.lastMsgId);
            }

            MessageResponseHelperResult<MamResult> result = await client.GENERAL_COMMAND_HELPER.requestMamAsync(filter);
            if (result.STATE == MessageResponseHelperResultState.SUCCESS)
            {
                mamRequest = new MamRequestTable
                {
                    accountId = client.getXMPPAccount().getBareJid(),
                    lastUpdate = DateTime.Now
                };
                if (result.RESULT.COUNT > 0)
                {
                    mamRequest.lastMsgId = result.RESULT.LAST;
                }
                MamDBManager.INSTANCE.setLastRequest(mamRequest);
            }
            Continue();
        }

        private async Task InitOmemoAsync()
        {
            state = SetupState.INITIALISING_OMEMO_KEYS;
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
            IList<ChatMessageTable> toSend = ChatDBManager.INSTANCE.getChatMessages(client.getXMPPAccount().getBareJid(), MessageState.SENDING);
            Logger.Info("Sending " + toSend.Count + " outstanding chat messages for: " + client.getXMPPAccount().getBareJid());
            await SendOutsandingChatMessagesAsync(toSend);
            Logger.Info("Finished sending outstanding chat messages for: " + client.getXMPPAccount().getBareJid());

            IList<ChatMessageTable> toEncrypt = ChatDBManager.INSTANCE.getChatMessages(client.getXMPPAccount().getBareJid(), MessageState.TO_ENCRYPT);
            Logger.Info("Sending " + toSend.Count + " outstanding OMEMO chat messages for: " + client.getXMPPAccount().getBareJid());
            await SendOutsandingChatMessagesAsync(toEncrypt);
            Logger.Info("Finished sending outstanding OMEMO chat messages for: " + client.getXMPPAccount().getBareJid());
            Continue();
        }

        /// <summary>
        /// Sends all chat messages passed to it.
        /// </summary>
        /// <param name="messages">A list of chat messages to send. They SHOULD be sorted by their chatId for optimal performance.</param>
        private async Task SendOutsandingChatMessagesAsync(IList<ChatMessageTable> messages)
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
                    await client.SendAsync(msg);
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
            switch (args.newState)
            {
                case ConnectionState.CONNECTED:
                    Start();
                    break;

                case ConnectionState.DISCONNECTED:
                case ConnectionState.ERROR:
                    Cancel();
                    break;
            }
        }

        #endregion
    }
}
