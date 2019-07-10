using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Data_Manager2.Classes;
using Data_Manager2.Classes.DBManager;
using Data_Manager2.Classes.DBTables;
using Data_Manager2.Classes.Toast;
using Logging;
using UWPX_UI_Context.Classes.DataTemplates.Dialogs;
using UWPX_UI_Context.Classes.DataTemplates.Pages;
using XMPP_API.Classes;
using XMPP_API.Classes.Network.XML.Messages;
using XMPP_API.Classes.Network.XML.Messages.Helper;
using XMPP_API.Classes.Network.XML.Messages.XEP_0048;

namespace UWPX_UI_Context.Classes.DataContext.Pages
{
    public sealed class ChatsPageContext
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        public readonly ChatPageDataTemplate MODEL = new ChatPageDataTemplate();

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--


        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--


        #endregion
        //--------------------------------------------------------Misc Methods:---------------------------------------------------------------\\
        #region --Misc Methods (Public)--
        public async Task OnAddChatAsync(AddChatDialogDataTemplate dataTemplate)
        {
            if (dataTemplate.Confirmed)
            {
                await AddChatAsync(dataTemplate.Client, dataTemplate.ChatBareJid, dataTemplate.AddToRoster, dataTemplate.SubscribeToPresence);
            }
        }

        public async Task OnAddMucAsync(AddMucDialogDataTemplate dataTemplate)
        {
            if (dataTemplate.Confirmed)
            {
                await AddMucAsync(dataTemplate.Client, dataTemplate.RoomBareJid, dataTemplate.Nickname, dataTemplate.Password, dataTemplate.Bookmark, dataTemplate.AutoJoin);
            }
        }

        public void OnNavigatedTo(object parameter)
        {
            // Subscribe to toast events:
            ToastHelper.OnChatMessageToast -= ToastHelper_OnChatMessageToast;
            ToastHelper.OnChatMessageToast += ToastHelper_OnChatMessageToast;

            MODEL.EvaluateOnNavigatedToArgs(parameter);
        }

        public void OnNavigatedFrom()
        {
            // Subscribe to toast events:
            ToastHelper.OnChatMessageToast -= ToastHelper_OnChatMessageToast;
        }

        #endregion

        #region --Misc Methods (Private)--
        private async Task AddMucAsync(XMPPClient client, string roomBareJid, string nickname, string password, bool bookmark, bool autoJoin)
        {
            ChatTable muc = new ChatTable(roomBareJid, client.getXMPPAccount().getBareJid())
            {
                chatType = ChatType.MUC,
                inRoster = bookmark,
                subscription = "none",
                isChatActive = true
            };
            ChatDBManager.INSTANCE.setChat(muc, false, true);

            MUCChatInfoTable info = new MUCChatInfoTable()
            {
                chatId = muc.id,
                subject = null,
                state = MUCState.DISCONNECTED,
                name = null,
                password = string.IsNullOrEmpty(password) ? null : password,
                nickname = nickname,
                autoEnterRoom = autoJoin,
            };
            MUCDBManager.INSTANCE.setMUCChatInfo(info, false, true);

            if (info.autoEnterRoom)
            {
                Task t = MUCHandler.INSTANCE.enterMUCAsync(client, muc, info);
            }

            if (bookmark)
            {
                List<ConferenceItem> conferenceItems = MUCDBManager.INSTANCE.getXEP0048ConferenceItemsForAccount(client.getXMPPAccount().getBareJid());
                MessageResponseHelperResult<IQMessage> result = await client.PUB_SUB_COMMAND_HELPER.setBookmars_xep_0048Async(conferenceItems);
                if (result.STATE == MessageResponseHelperResultState.SUCCESS)
                {
                    if (result.RESULT is IQErrorMessage errMsg)
                    {
                        Logger.Warn("Failed to set bookmarks: " + errMsg.ToString());
                    }
                }
                else
                {
                    Logger.Warn("Failed to set bookmarks: " + result.STATE);
                }
            }
        }

        /// <summary>
        /// Adds and starts a new chat.
        /// </summary>
        /// <param name="client">Which account/client owns this chat?</param>
        /// <param name="bareJid">The bare JID of the opponent.</param>
        /// <param name="addToRoster">Should the chat get added to the users roster?</param>
        /// <param name="requestSubscription">Request a presence subscription?</param>
        private async Task AddChatAsync(XMPPClient client, string bareJid, bool addToRoster, bool requestSubscription)
        {
            if (client is null || string.IsNullOrEmpty(bareJid))
            {
                Logger.Error("Unable to add chat! client ?= " + (client is null) + " bareJid ?=" + (bareJid is null));
                return;
            }

            await Task.Run(async () =>
            {
                ChatTable chat = ChatDBManager.INSTANCE.getChat(ChatTable.generateId(bareJid, client.getXMPPAccount().getBareJid()));

                if (chat is null)
                {
                    chat = new ChatTable(bareJid, client.getXMPPAccount().getBareJid());
                }

                // Set chat active:
                chat.isChatActive = true;
                chat.lastActive = DateTime.Now;

                // Add to roster:
                if (addToRoster && !chat.inRoster)
                {
                    await client.GENERAL_COMMAND_HELPER.addToRosterAsync(bareJid);
                    chat.inRoster = true;
                }

                // Request presence subscription:
                if (requestSubscription && (string.Equals(chat.subscription, "none") || string.Equals(chat.subscription, "from")) && string.IsNullOrEmpty(chat.subscription))
                {
                    await client.GENERAL_COMMAND_HELPER.requestPresenceSubscriptionAsync(bareJid);
                    chat.subscription = "pending";
                }
                ChatDBManager.INSTANCE.setChat(chat, false, true);
            });

        }

        #endregion

        #region --Misc Methods (Protected)--


        #endregion
        //--------------------------------------------------------Events:---------------------------------------------------------------------\\
        #region --Events--
        private void ToastHelper_OnChatMessageToast(OnChatMessageToastEventArgs args)
        {
            if (!args.Cancel && args.toasterTypeOverride == ChatMessageToasterType.FULL && UiUtils.IsWindowActivated)
            {
                args.toasterTypeOverride = ChatMessageToasterType.REDUCED;
            }
        }

        #endregion
    }
}
