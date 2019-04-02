using Data_Manager2.Classes.DBManager;
using Data_Manager2.Classes.DBTables;
using Data_Manager2.Classes.Toast;
using Logging;
using System.Threading.Tasks;
using UWPX_UI_Context.Classes.DataTemplates.Dialogs;
using UWPX_UI_Context.Classes.DataTemplates.Pages;
using XMPP_API.Classes;

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
        private async Task AddChatAsync(XMPPClient client, string bareJid, bool addToRoster, bool requestSubscription)
        {
            if (client is null || string.IsNullOrEmpty(bareJid))
            {
                Logger.Error("Unable to add chat! client ?= " + (client is null) + " bareJid ?=" + (bareJid is null));
                return;
            }

            await Task.Run(async () =>
            {
                if (addToRoster)
                {
                    await client.GENERAL_COMMAND_HELPER.addToRosterAsync(bareJid);
                }
                if (requestSubscription)
                {
                    await client.GENERAL_COMMAND_HELPER.requestPresenceSubscriptionAsync(bareJid);
                }
                ChatDBManager.INSTANCE.setChat(new ChatTable(bareJid, client.getXMPPAccount().getBareJid())
                {
                    inRoster = addToRoster,
                    subscription = requestSubscription ? "pending" : null
                }, false, true);
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
