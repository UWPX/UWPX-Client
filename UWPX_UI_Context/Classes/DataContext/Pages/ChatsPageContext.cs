using System;
using System.Threading.Tasks;
using Logging;
using Manager.Classes;
using Manager.Classes.Chat;
using Manager.Classes.Toast;
using Storage.Classes.Models.Chat;
using UWPX_UI_Context.Classes.DataTemplates.Dialogs;
using UWPX_UI_Context.Classes.DataTemplates.Pages;

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

        /// <summary>
        /// Adds and starts a new chat.
        /// </summary>
        /// <param name="client">Which account/client owns this chat?</param>
        /// <param name="bareJid">The bare JID of the opponent.</param>
        /// <param name="addToRoster">Should the chat get added to the users roster?</param>
        /// <param name="requestSubscription">Request a presence subscription?</param>
        private async Task AddChatAsync(Client client, string bareJid, bool addToRoster, bool requestSubscription)
        {
            if (client is null || string.IsNullOrEmpty(bareJid))
            {
                Logger.Error("Unable to add chat! client ?= " + (client is null) + " bareJid ?=" + (bareJid is null));
                return;
            }

            await Task.Run(async () =>
            {
                ChatModel chat = DataCache.INSTANCE.GetChat(client.dbAccount.bareJid, bareJid);

                if (chat is null)
                {
                    chat = new ChatModel(bareJid, client.dbAccount);
                }

                // Set chat active:
                chat.isChatActive = true;
                chat.lastActive = DateTime.Now;

                // Add to roster:
                if (addToRoster && !chat.inRoster)
                {
                    await client.xmppClient.GENERAL_COMMAND_HELPER.addToRosterAsync(bareJid);
                    chat.inRoster = true;
                }

                // Request presence subscription:
                if (requestSubscription && (string.Equals(chat.subscription, "none") || string.Equals(chat.subscription, "from")) && string.IsNullOrEmpty(chat.subscription))
                {
                    await client.xmppClient.GENERAL_COMMAND_HELPER.requestPresenceSubscriptionAsync(bareJid);
                    chat.subscription = "pending";
                }
                DataCache.INSTANCE.AddChat(chat, client);
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
