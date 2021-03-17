using System.ComponentModel;
using System.Threading.Tasks;
using Manager.Classes.Chat;
using Shared.Classes;
using Storage.Classes.Models.Chat;
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
        public void UpdateView(DependencyPropertyChangedEventArgs args)
        {
            if (args.OldValue is ChatDataTemplate oldChat)
            {
                if (!(oldChat.Chat is null))
                {
                    oldChat.Chat.PropertyChanged -= OnChatPropertyChanged;
                }
            }

            if (args.NewValue is ChatDataTemplate newChat)
            {
                if (!(newChat.Chat is null))
                {
                    newChat.Chat.PropertyChanged += OnChatPropertyChanged;
                }
                MODEL.UpdateView(newChat.Chat);
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
                chat.Chat.Update();
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
                chat.Chat.Update();
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
                    chat.Chat.Update();
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
                chat.Chat.Update();
            }).ConfAwaitFalse();
        }

        public void SaveCustomChatName(string chatName, ChatModel chat)
        {
            if (string.IsNullOrEmpty(chatName) || string.Equals(chatName, chat.bareJid))
            {
                chat.customName = null;
            }
            else
            {
                chat.customName = chatName;
            }
            chat.Update();
        }

        #endregion

        #region --Misc Methods (Private)--


        #endregion

        #region --Misc Methods (Protected)--


        #endregion
        //--------------------------------------------------------Events:---------------------------------------------------------------------\\
        #region --Events--
        private void OnChatPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (sender is ChatModel chat)
            {
                MODEL.UpdateView(chat);
            }
        }

        #endregion
    }
}
