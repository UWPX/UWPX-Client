using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;
using Logging;
using Manager.Classes;
using Manager.Classes.Chat;
using Manager.Classes.Toast;
using Microsoft.Toolkit.Uwp.UI.Helpers;
using Storage.Classes;
using Storage.Classes.Contexts;
using Storage.Classes.Models.Chat;
using UWPX_UI_Context.Classes.DataTemplates.Controls;
using UWPX_UI_Context.Classes.DataTemplates.Dialogs;
using UWPX_UI_Context.Classes.Events;
using Windows.UI.Xaml;
using XMPP_API.Classes;
using XMPP_API.Classes.Network.XML.Messages;
using XMPP_API.Classes.Network.XML.Messages.XEP_0048;

namespace UWPX_UI_Context.Classes.DataContext.Controls
{
    public sealed partial class ChatMasterControlContext
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        public readonly ChatMasterControlDataTemplate MODEL;
        private readonly ThemeListener THEME_LISTENER = new ThemeListener();

        public delegate void OnErrorEventHandler(ChatMasterControlContext sender, OnErrorEventArgs args);

        public event OnErrorEventHandler OnError;

        private MessageResponseHelper<IQMessage> updateBookmarksHelper;

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        public ChatMasterControlContext(ResourceDictionary resources)
        {
            MODEL = new ChatMasterControlDataTemplate(resources);
            THEME_LISTENER.ThemeChanged += THEME_LISTENER_ThemeChanged;
            LoadSettings();
        }

        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--
        private async Task SetChatInRosterAsync(ChatDataTemplate chat, bool inRoster)
        {
            if (chat.Chat.inRoster != inRoster)
            {
                chat.Chat.inRoster = inRoster;
                UpdateView(chat);
                chat.Chat.Update();
            }

            if (inRoster)
            {
                await chat.Client.xmppClient.GENERAL_COMMAND_HELPER.addToRosterAsync(chat.Chat.bareJid);
            }
            else
            {
                await chat.Client.xmppClient.GENERAL_COMMAND_HELPER.removeFromRosterAsync(chat.Chat.bareJid);
            }
        }

        private void SetChatBookmarked(ChatDataTemplate chat, bool bookmarked)
        {
            if (chat.Chat.inRoster != bookmarked)
            {
                chat.Chat.inRoster = bookmarked;
                UpdateView(chat);
                chat.Chat.Update();
            }
            UpdateBookmarks(chat.Client.xmppClient);
        }

        public void SetChatMuted(ChatDataTemplate chat, bool muted)
        {
            if (chat.Chat.muted != muted)
            {
                chat.Chat.muted = muted;
                UpdateView(chat);
                chat.Chat.Update();
            }
        }

        #endregion
        //--------------------------------------------------------Misc Methods:---------------------------------------------------------------\\
        #region --Misc Methods (Public)--
        public void UpdateView(DependencyPropertyChangedEventArgs args)
        {
            ChatDataTemplate newChat = null;
            if (args.OldValue is ChatDataTemplate oldChat)
            {
                if (!(oldChat.Chat is null))
                {
                    oldChat.Chat.PropertyChanged -= OnChatPropertyChanged;
                }
                if (!(oldChat.Chat.muc is null))
                {
                    oldChat.Chat.muc.PropertyChanged -= OnMucPropertyChanged;
                }
            }

            if (args.NewValue is ChatDataTemplate)
            {
                newChat = args.NewValue as ChatDataTemplate;
                if (!(newChat.Chat is null))
                {
                    newChat.Chat.PropertyChanged += OnChatPropertyChanged;
                }
                if (!(newChat.Chat.muc is null))
                {
                    newChat.Chat.muc.PropertyChanged += OnMucPropertyChanged;
                }
            }

            UpdateView(newChat);
        }

        public async Task ToggleChatInRosterAsync(ChatDataTemplate chat)
        {
            await SetChatInRosterAsync(chat, !chat.Chat.inRoster);
        }

        public void ToggleChatBookmarked(ChatDataTemplate chat)
        {
            SetChatBookmarked(chat, !chat.Chat.inRoster);
        }

        public async Task DeleteChatAsync(DeleteChatConfirmDialogDataTemplate confirmDialogModel, ChatDataTemplate chat)
        {
            if (confirmDialogModel.Confirmed)
            {
                await Task.Run(async () =>
                {
                    if (chat.Chat.chatType == ChatType.MUC)
                    {
                        SetChatBookmarked(chat, false);
                    }
                    else
                    {
                        if (confirmDialogModel.RemoveFromRoster)
                        {
                            await SetChatInRosterAsync(chat, false);
                        }
                    }

                    DataCache.INSTANCE.DeleteChat(chat.Chat, confirmDialogModel.KeepChatMessages, confirmDialogModel.RemoveFromRoster);
                });
            }
        }

        public async Task LeaveMucAsync(ChatDataTemplate chatTemplate)
        {
            await MucHandler.INSTANCE.LeaveRoomAsync(chatTemplate.Client.xmppClient, chatTemplate.Chat.muc);
        }

        public async Task EnterMucAsync(ChatDataTemplate chatTemplate)
        {
            await MucHandler.INSTANCE.EnterMucAsync(chatTemplate.Client.xmppClient, chatTemplate.Chat.muc);
        }

        public async Task SendPresenceProbeAsync(ChatDataTemplate chatTemplate)
        {
            await chatTemplate.Client.xmppClient.GENERAL_COMMAND_HELPER.sendPresenceProbeAsync(chatTemplate.Client.dbAccount.fullJid.FullJid(), chatTemplate.Chat.bareJid);
        }

        public async Task RequestPresenceSubscriptionAsync(ChatDataTemplate chatTemplate)
        {
            await chatTemplate.Client.xmppClient.GENERAL_COMMAND_HELPER.requestPresenceSubscriptionAsync(chatTemplate.Chat.bareJid);
        }

        public async Task CancelPresenceSubscriptionAsync(ChatDataTemplate chatTemplate)
        {
            await chatTemplate.Client.xmppClient.GENERAL_COMMAND_HELPER.unsubscribeFromPresenceAsync(chatTemplate.Chat.bareJid);
            await Task.Run(() =>
            {
                switch (chatTemplate.Chat.subscription)
                {
                    case "to":
                        chatTemplate.Chat.subscription = "none";
                        break;

                    case "both":
                        chatTemplate.Chat.subscription = "from";
                        break;
                }
                chatTemplate.Chat.Update();
            });
        }

        public void MarkAsRead(ChatDataTemplate chat)
        {
            Task.Run(() =>
            {
                DataCache.INSTANCE.MarkAllChatMessagesAsRead(chat.Chat.id);
                ToastHelper.UpdateBadgeNumber();
            });
        }

        public async Task RejectPresenceSubscriptionAsync(ChatDataTemplate chatTemplate)
        {
            await chatTemplate.Client.xmppClient.GENERAL_COMMAND_HELPER.answerPresenceSubscriptionRequestAsync(chatTemplate.Chat.bareJid, false);
            switch (chatTemplate.Chat.subscription)
            {
                case "from":
                    chatTemplate.Chat.subscription = "none";
                    break;

                case "both":
                    chatTemplate.Chat.subscription = "to";
                    break;
            }
            chatTemplate.Chat.Update();
        }

        public async Task AnswerPresenceSubscriptionRequestAsync(ChatDataTemplate chatTemplate, bool accepted)
        {
            await chatTemplate.Client.xmppClient.GENERAL_COMMAND_HELPER.answerPresenceSubscriptionRequestAsync(chatTemplate.Chat.bareJid, accepted);
            chatTemplate.Chat.subscription = accepted ? "to" : "none";
            chatTemplate.Chat.Update();
        }

        #endregion

        #region --Misc Methods (Private)--
        private void UpdateView(ChatDataTemplate chatTemplate)
        {
            if (!(chatTemplate is null))
            {
                MODEL.UpdateViewChat(chatTemplate.Chat);
                MODEL.UpdateLastAction(chatTemplate.LastMsg);
                if (chatTemplate.Chat.chatType == ChatType.MUC)
                {
                    MODEL.UpdateViewMuc(chatTemplate.Chat?.muc);
                }
            }
        }

        private void UpdateBookmarks(XMPPClient client)
        {
            List<ConferenceItem> conferences;
            using (MainDbContext ctx = new MainDbContext())
            {
                conferences = ctx.GetXEP0048ConferenceItemsForAccount(client.getXMPPAccount().getBareJid());
            }
            if (updateBookmarksHelper != null)
            {
                updateBookmarksHelper.Dispose();
            }
            updateBookmarksHelper = client.PUB_SUB_COMMAND_HELPER.setBookmars_xep_0048(conferences, OnUpdateBookmarksMessage, OnUpdateBookmarksTimeout);
        }

        private void InvokeOnError(string titel, string message)
        {
            Logger.Error(titel + ": " + message);
            OnError?.Invoke(this, new OnErrorEventArgs(titel, message));
        }

        private void LoadSettings()
        {
            int show = Settings.GetSettingInt(SettingsConsts.CHAT_SHOW_ACCOUNT_COLOR);
            switch (show)
            {
                case 1:
                    MODEL.ShowAccountColor = true;
                    break;

                case 2:
                    MODEL.ShowAccountColor = false;
                    break;

                default:
                    MODEL.ShowAccountColor = ConnectionHandler.INSTANCE.GetClients().Count > 1;
                    break;
            }

            Settings.SettingChanged += Settings_SettingChanged;
        }

        #endregion

        #region --Misc Methods (Protected)--


        #endregion
        //--------------------------------------------------------Events:---------------------------------------------------------------------\\
        #region --Events--
        private void OnChatPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (sender is ChatModel chat)
            {
                MODEL.UpdateViewChat(chat);
            }
        }

        private void OnMucPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (sender is MucInfoModel muc)
            {
                MODEL.UpdateViewMuc(muc);
            }
        }

        private void THEME_LISTENER_ThemeChanged(ThemeListener sender)
        {
            MODEL.OnThemeChanged();
        }

        private void Settings_SettingChanged(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case SettingsConsts.CHAT_SHOW_ACCOUNT_COLOR:
                    LoadSettings();
                    break;
            }
        }

        #endregion
    }
}
