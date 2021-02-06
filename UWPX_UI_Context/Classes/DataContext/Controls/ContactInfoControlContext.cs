using System.Threading.Tasks;
using Manager.Classes.Chat;
using Shared.Classes;
using UWPX_UI_Context.Classes.DataTemplates.Controls;
using Windows.UI.Xaml;

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
            if (e.NewValue is ChatDataTemplate chat)
            {
                MODEL.UpdateView(chat.Chat);
                MODEL.UpdateView(chat.Client);
            }
        }

        public async Task SendPresenceProbeAsync(ChatDataTemplate chat)
        {
            if (chat is null)
            {
                return;
            }

            await chat.Client.xmppClient.GENERAL_COMMAND_HELPER.sendPresenceProbeAsync(chat.Client.xmppClient.getXMPPAccount().getFullJid(), chat.Chat.bareJid);
        }

        public async Task RequestPresenceSubscriptionAsync(ChatDataTemplate chat)
        {
            if (chat is null)
            {
                return;
            }

            await chat.Client.xmppClient.GENERAL_COMMAND_HELPER.requestPresenceSubscriptionAsync(chat.Chat.bareJid);
        }

        public async Task CancelPresenceSubscriptionAsync(ChatDataTemplate chat)
        {
            if (chat is null)
            {
                return;
            }

            await chat.Client.xmppClient.GENERAL_COMMAND_HELPER.unsubscribeFromPresenceAsync(chat.Chat.bareJid).ConfAwaitFalse();
            await Task.Run(() =>
            {
                switch (chat.Chat.subscription)
                {
                    case "to":
                        chat.Chat.subscription = "none";
                        break;

                    case "both":
                        chat.Chat.subscription = "from";
                        break;
                }
                chat.Chat.Save();
            }).ConfAwaitFalse();
        }

        public async Task RejectPresenceSubscriptionAsync(ChatDataTemplate chat)
        {
            if (chat is null)
            {
                return;
            }

            await chat.Client.xmppClient.GENERAL_COMMAND_HELPER.answerPresenceSubscriptionRequestAsync(chat.Chat.bareJid, false).ConfAwaitFalse();
            await Task.Run(() =>
            {
                switch (chat.Chat.subscription)
                {
                    case "from":
                        chat.Chat.subscription = "none";
                        break;

                    case "both":
                        chat.Chat.subscription = "to";
                        break;
                }
                chat.Chat.Save();
            }).ConfAwaitFalse();
        }

        public async Task SwitchChatInRosterAsync(ChatDataTemplate chat)
        {
            if (chat is null)
            {
                return;
            }

            await SetChatInRosterAsync(chat, !chat.Chat.inRoster).ConfAwaitFalse();
        }

        private async Task SetChatInRosterAsync(ChatDataTemplate chat, bool inRoster)
        {
            if (chat.Chat.inRoster != inRoster)
            {
                await Task.Run(() =>
                {
                    chat.Chat.inRoster = inRoster;
                    MODEL.UpdateView(chat.Chat);
                    chat.Chat.Save();
                }).ConfAwaitFalse();
            }

            if (inRoster)
            {
                await chat.Client.xmppClient.GENERAL_COMMAND_HELPER.addToRosterAsync(chat.Chat.bareJid).ConfAwaitFalse();
            }
            else
            {
                await chat.Client.xmppClient.GENERAL_COMMAND_HELPER.removeFromRosterAsync(chat.Chat.bareJid).ConfAwaitFalse();
            }
        }

        public async Task ToggleChatMutedAsync(ChatDataTemplate chat)
        {
            if (chat is null)
            {
                return;
            }

            await Task.Run(() =>
            {
                chat.Chat.muted = !chat.Chat.muted;
                MODEL.UpdateView(chat.Chat);
                chat.Chat.Save();
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
