using System.Threading.Tasks;
using Data_Manager2.Classes.DBManager;
using Data_Manager2.Classes.DBTables;
using Shared.Classes;
using UWPX_UI_Context.Classes.DataTemplates.Controls;
using Windows.UI.Xaml;
using XMPP_API.Classes;

namespace UWPX_UI_Context.Classes.DataContext.Controls
{
    public class ContactInfoControlContext
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        public readonly ContactInfoControlDataTemplate MODEL = new ContactInfoControlDataTemplate();

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--


        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--


        #endregion
        //--------------------------------------------------------Misc Methods:---------------------------------------------------------------\\
        #region --Misc Methods (Public)--
        public void UpdateView(DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue is ChatTable chat)
            {
                MODEL.UpdateView(chat);
            }
            else if (e.NewValue is XMPPClient client)
            {
                MODEL.UpdateView(client);
            }
        }

        public async Task SendPresenceProbeAsync(XMPPClient client, ChatTable chat)
        {
            if (client is null || chat is null)
            {
                return;
            }

            await client.GENERAL_COMMAND_HELPER.sendPresenceProbeAsync(client.getXMPPAccount().getFullJid(), chat.chatJabberId);
        }

        public async Task RequestPresenceSubscriptionAsync(XMPPClient client, ChatTable chat)
        {
            if (client is null || chat is null)
            {
                return;
            }

            await client.GENERAL_COMMAND_HELPER.requestPresenceSubscriptionAsync(chat.chatJabberId);
        }

        public async Task CancelPresenceSubscriptionAsync(XMPPClient client, ChatTable chat)
        {
            if (client is null || chat is null)
            {
                return;
            }

            await client.GENERAL_COMMAND_HELPER.unsubscribeFromPresenceAsync(chat.chatJabberId).ConfAwaitFalse();
            await Task.Run(() =>
            {
                switch (chat.subscription)
                {
                    case "to":
                        chat.subscription = "none";
                        break;

                    case "both":
                        chat.subscription = "from";
                        break;
                }
                ChatDBManager.INSTANCE.setChat(chat, false, true);
            }).ConfAwaitFalse();
        }

        public async Task RejectPresenceSubscriptionAsync(XMPPClient client, ChatTable chat)
        {
            if (client is null || chat is null)
            {
                return;
            }

            await client.GENERAL_COMMAND_HELPER.answerPresenceSubscriptionRequestAsync(chat.chatJabberId, false).ConfAwaitFalse();
            await Task.Run(() =>
            {
                switch (chat.subscription)
                {
                    case "from":
                        chat.subscription = "none";
                        break;

                    case "both":
                        chat.subscription = "to";
                        break;
                }
                ChatDBManager.INSTANCE.setChat(chat, false, true);
            }).ConfAwaitFalse();
        }

        public async Task SwitchChatInRosterAsync(XMPPClient client, ChatTable chat)
        {
            if (client is null || chat is null)
            {
                return;
            }

            await SetChatInRosterAsync(client, chat, !chat.inRoster).ConfAwaitFalse();
        }

        private async Task SetChatInRosterAsync(XMPPClient client, ChatTable chat, bool inRoster)
        {
            if (chat.inRoster != inRoster)
            {
                await Task.Run(() =>
                {
                    chat.inRoster = inRoster;
                    MODEL.UpdateView(chat);
                    ChatDBManager.INSTANCE.setChat(chat, false, true);
                }).ConfAwaitFalse();
            }

            if (inRoster)
            {
                await client.GENERAL_COMMAND_HELPER.addToRosterAsync(chat.chatJabberId).ConfAwaitFalse();
            }
            else
            {
                await client.GENERAL_COMMAND_HELPER.removeFromRosterAsync(chat.chatJabberId).ConfAwaitFalse();
            }
        }

        public async Task ToggleChatMutedAsync(ChatTable chat)
        {
            if (chat is null)
            {
                return;
            }

            await Task.Run(() =>
            {
                chat.muted = !chat.muted;
                MODEL.UpdateView(chat);
                ChatDBManager.INSTANCE.setChat(chat, false, true);
            }).ConfAwaitFalse();
        }

        #endregion

        #region --Misc Methods (Private)--


        #endregion

        #region --Misc Methods (Protected)--


        #endregion
        //--------------------------------------------------------Events:---------------------------------------------------------------------\\
        #region --Events--


        #endregion
    }
}
