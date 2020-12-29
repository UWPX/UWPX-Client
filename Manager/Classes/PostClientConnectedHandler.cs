using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using Logging;
using XMPP_API.Classes;
using XMPP_API.Classes.Network;
using XMPP_API.Classes.Network.Events;
using XMPP_API.Classes.Network.XML.Messages;
using XMPP_API.Classes.Network.XML.Messages.Helper;
using XMPP_API.Classes.Network.XML.Messages.XEP_0313;
using XMPP_API.Classes.Network.XML.Messages.XEP_0384;

namespace Manager.Classes
{
    internal enum SetupState
    {
        NOT_STARTED,
        DISCO,
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
        private readonly ClientConnectionHandler ccHandler;

        private SetupState state = SetupState.NOT_STARTED;

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        public PostClientConnectedHandler(ClientConnectionHandler ccHandler)
        {
            this.ccHandler = ccHandler;
            this.ccHandler.client.ConnectionStateChanged += OnConnectionStateChanged;
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
                    Logger.Info("Starting post client connected process for: " + ccHandler.client.getXMPPAccount().getBareJid());
                    _ = DiscoAsync();
                    break;

                case SetupState.DISCO:
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
                    Logger.Info("Post client connected process done for: " + ccHandler.client.getXMPPAccount().getBareJid());
                    return;

                case SetupState.CANCELED:
                    Logger.Info("Post client connected process canceled for: " + ccHandler.client.getXMPPAccount().getBareJid());
                    break;

                default:
                    Debug.Assert(false); // Should not happen
                    break;
            }
            Logger.Debug("PostClientConnectedHandler for " + ccHandler.client.getXMPPAccount().getBareJid() + " now in state: " + state.ToString());
        }

        private async Task DiscoAsync()
        {
            state = SetupState.DISCO;
            await ccHandler.client.connection.DISCO_HELPER.DiscoAsync();
            Continue();
        }

        private async Task RequestBookmarksAsync()
        {
            state = SetupState.REQUESTING_BOOKMARKS;
            await ccHandler.client.PUB_SUB_COMMAND_HELPER.requestBookmars_xep_0048Async();
            Continue();
        }

        private async Task RequestRosterAsync()
        {
            state = SetupState.REQUESTING_ROSTER;
            await ccHandler.client.GENERAL_COMMAND_HELPER.sendRequestRosterMessageAsync();
            Continue();
        }

        private void HandleMamMessages(MamResult result)
        {
            Task.Run(async () =>
            {
                foreach (QueryArchiveResultMessage message in result.RESULTS)
                {
                    await ccHandler.HandleNewChatMessageAsync(message.MESSAGE);
                }
            });
        }

        private async Task RequestMamAsync()
        {
            state = SetupState.REQUESTING_MAM;

            if (!ccHandler.client.connection.DISCO_HELPER.HasFeature(Consts.XML_XEP_0313_NAMESPACE, ccHandler.client.getXMPPAccount().getBareJid()))
            {
                Logger.Info("No need to request MAM for " + ccHandler.client.getXMPPAccount().getBareJid() + " - not supported.");
                Continue();
                return;
            }

            bool extendedMamSupport = ccHandler.client.connection.DISCO_HELPER.HasFeature(Consts.XML_XEP_0313_EXTENDED_NAMESPACE, ccHandler.client.getXMPPAccount().getBareJid());
            Logger.Info("Extended MAM support for " + ccHandler.client.getXMPPAccount().getBareJid() + ": " + extendedMamSupport);

            MamRequestTable mamRequest = MamDBManager.INSTANCE.getLastRequest(ccHandler.client.getXMPPAccount().getBareJid());
            string lastMsgId = null;
            DateTime lastMsgDate = DateTime.MinValue;
            if (!(mamRequest is null))
            {
                lastMsgId = mamRequest.lastMsgId;
                lastMsgDate = mamRequest.lastUpdate;
            }

            // Request all MAM messages:
            bool done = false;
            int iteration = 1;
            while (!done)
            {
                QueryFilter filter = new QueryFilter();
                if (extendedMamSupport)
                {
                    // Only extended MAM supports setting the 'after-id' property.
                    // Reference: https://xmpp.org/extensions/xep-0313.html#support
                    if (!(lastMsgId is null))
                    {
                        filter.AfterId(lastMsgId);
                    }
                }
                else
                {
                    // Fallback for servers not supporting 'urn:xmpp:mam:2#extended'.
                    if (lastMsgDate != DateTime.MinValue)
                    {
                        filter.Start(lastMsgDate);
                    }
                }

                MessageResponseHelperResult<MamResult> result = await RequestMamWithRetry(filter, 2);
                if (result.STATE == MessageResponseHelperResultState.SUCCESS)
                {
                    mamRequest = new MamRequestTable
                    {
                        accountId = ccHandler.client.getXMPPAccount().getBareJid(),
                        lastUpdate = DateTime.Now
                    };
                    if (result.RESULT.RESULTS.Count > 0)
                    {
                        mamRequest.lastMsgId = result.RESULT.LAST;
                        lastMsgId = result.RESULT.LAST;
                        HandleMamMessages(result.RESULT);
                        Logger.Info("MAM request for " + ccHandler.client.getXMPPAccount().getBareJid() + " received " + result.RESULT.RESULTS.Count + " messages in iteration " + iteration + '.');
                        Logger.Debug("First: " + result.RESULT.RESULTS[result.RESULT.RESULTS.Count - 1].QUERY_ID + " Last: " + result.RESULT.RESULTS[0].QUERY_ID);
                    }
                    if (result.RESULT.COMPLETE || result.RESULT.RESULTS.Count <= 0)
                    {
                        done = true;
                        Logger.Info("MAM request for " + ccHandler.client.getXMPPAccount().getBareJid());
                    }

                    DateTime newDate = GetLastMessageDate(result.RESULT);
                    if (newDate == mamRequest.lastUpdate)
                    {
                        done = true;
                        Logger.Info("MAM request for " + ccHandler.client.getXMPPAccount().getBareJid());
                    }
                    else
                    {
                        lastMsgDate = newDate;
                    }
                    MamDBManager.INSTANCE.setLastRequest(mamRequest);
                }
                else if (state == SetupState.REQUESTING_MAM)
                {
                    done = true;
                    Logger.Warn("Failed to request MAM archive for " + ccHandler.client.getXMPPAccount().getBareJid() + " with: " + result.STATE);
                }
                ++iteration;
            }
            Continue();
        }

        private DateTime GetLastMessageDate(MamResult result)
        {
            return result.RESULTS.Count > 0 ? result.RESULTS[0].MESSAGE.delay : DateTime.Now;
        }

        private async Task<MessageResponseHelperResult<MamResult>> RequestMamWithRetry(QueryFilter filter, int numOfTries)
        {
            MessageResponseHelperResult<MamResult> result = await ccHandler.client.GENERAL_COMMAND_HELPER.requestMamAsync(filter);
            if (result.STATE == MessageResponseHelperResultState.SUCCESS || numOfTries <= 0 || state != SetupState.REQUESTING_MAM)
            {
                return result;
            }
            return await RequestMamWithRetry(filter, --numOfTries);
        }

        private async Task InitOmemoAsync()
        {
            state = SetupState.INITIALISING_OMEMO_KEYS;
            OmemoHelper omemoHelper = ccHandler.client.getOmemoHelper();
            if (!(omemoHelper is null))
            {
                await omemoHelper.initAsync();
            }
            else
            {
                Logger.Warn("Failed to initialize OMEMO handler since it is null.");
            }
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
            IList<ChatMessageTable> toSend = ChatDBManager.INSTANCE.getChatMessages(ccHandler.client.getXMPPAccount().getBareJid(), MessageState.SENDING);
            Logger.Info("Sending " + toSend.Count + " outstanding chat messages for: " + ccHandler.client.getXMPPAccount().getBareJid());
            await SendOutsandingChatMessagesAsync(toSend);
            Logger.Info("Finished sending outstanding chat messages for: " + ccHandler.client.getXMPPAccount().getBareJid());

            IList<ChatMessageTable> toEncrypt = ChatDBManager.INSTANCE.getChatMessages(ccHandler.client.getXMPPAccount().getBareJid(), MessageState.TO_ENCRYPT);
            Logger.Info("Sending " + toSend.Count + " outstanding OMEMO chat messages for: " + ccHandler.client.getXMPPAccount().getBareJid());
            await SendOutsandingChatMessagesAsync(toEncrypt);
            Logger.Info("Finished sending outstanding OMEMO chat messages for: " + ccHandler.client.getXMPPAccount().getBareJid());
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
                MessageMessage msg = msgDb.toXmppMessage(ccHandler.client.getXMPPAccount().getFullJid(), chat);

                if (msg is OmemoMessageMessage omemoMsg)
                {
                    await ccHandler.client.sendOmemoMessageAsync(omemoMsg, chat.chatJabberId, ccHandler.client.getXMPPAccount().getBareJid());
                }
                else
                {
                    await ccHandler.client.SendAsync(msg);
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
