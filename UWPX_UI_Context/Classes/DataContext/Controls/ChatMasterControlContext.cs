using System.Collections.Generic;
using System.Threading.Tasks;
using Data_Manager2.Classes;
using Data_Manager2.Classes.DBManager;
using Data_Manager2.Classes.DBManager.Omemo;
using Data_Manager2.Classes.Toast;
using Logging;
using Microsoft.Toolkit.Uwp.UI.Helpers;
using UWPX_UI_Context.Classes.DataTemplates;
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
                ChatDBManager.INSTANCE.setChat(chat.Chat, false, true);
            }

            if (inRoster)
            {
                await chat.Client.GENERAL_COMMAND_HELPER.addToRosterAsync(chat.Chat.chatJabberId);
            }
            else
            {
                await chat.Client.GENERAL_COMMAND_HELPER.removeFromRosterAsync(chat.Chat.chatJabberId);
            }
        }

        private void SetChatBookmarked(ChatDataTemplate chat, bool bookmarked)
        {
            if (chat.Chat.inRoster != bookmarked)
            {
                chat.Chat.inRoster = bookmarked;
                UpdateView(chat);
                ChatDBManager.INSTANCE.setChat(chat.Chat, false, true);
            }
            UpdateBookmarks(chat.Client);
        }

        public async Task SetChatMutedAsync(ChatDataTemplate chat, bool muted)
        {
            if (chat.Chat.muted != muted)
            {
                await Task.Run(() =>
                {
                    chat.Chat.muted = muted;
                    UpdateView(chat);
                    ChatDBManager.INSTANCE.setChat(chat.Chat, false, true);
                });
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
                oldChat.PropertyChanged -= Chat_PropertyChanged;
                oldChat.ChatMessageChanged -= Chat_ChatMessageChanged;
                oldChat.NewChatMessage -= Chat_NewChatMessage;
            }

            if (args.NewValue is ChatDataTemplate)
            {
                newChat = args.NewValue as ChatDataTemplate;
                newChat.PropertyChanged += Chat_PropertyChanged;
                newChat.ChatMessageChanged += Chat_ChatMessageChanged;
                newChat.NewChatMessage += Chat_NewChatMessage;
            }

            UpdateView(newChat);
        }

        public async Task SwitchChatInRosterAsync(ChatDataTemplate chat)
        {
            await SetChatInRosterAsync(chat, !chat.Chat.inRoster);
        }

        public void SwitchChatBookmarked(ChatDataTemplate chat)
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

                        MUCDBManager.INSTANCE.setMUCChatInfo(chat.MucInfo, true, false);
                        Logger.Info("Deleted MUC info for: " + chat.Chat.id);
                    }
                    else
                    {
                        if (confirmDialogModel.RemoveFromRoster)
                        {
                            await SetChatInRosterAsync(chat, false);
                        }
                    }

                    if (!confirmDialogModel.KeepChatMessages)
                    {
                        await ChatDBManager.INSTANCE.deleteAllChatMessagesForChatAsync(chat.Chat.id);
                        Logger.Info("Deleted chat messages for: " + chat.Chat.id);
                    }

                    if (chat.Chat.chatType == ChatType.MUC || confirmDialogModel.RemoveFromRoster)
                    {
                        ChatDBManager.INSTANCE.setChat(chat.Chat, true, true);
                        Logger.Info("Deleted chat: " + chat.Chat.id);
                    }
                    else
                    {
                        chat.Chat.isChatActive = false;
                        ChatDBManager.INSTANCE.setChat(chat.Chat, false, true);
                        Logger.Info("Marked chat as not active: " + chat.Chat.id);
                    }

                    // Delete all fingerprints:
                    OmemoSignalKeyDBManager.INSTANCE.deletePreKey()
                });
            }
        }

        public async Task LeaveMucAsync(ChatDataTemplate chatTemplate)
        {
            await MUCHandler.INSTANCE.leaveRoomAsync(chatTemplate.Client, chatTemplate.Chat, chatTemplate.MucInfo);
        }

        public async Task EnterMucAsync(ChatDataTemplate chatTemplate)
        {
            await MUCHandler.INSTANCE.enterMUCAsync(chatTemplate.Client, chatTemplate.Chat, chatTemplate.MucInfo);
        }

        public async Task SendPresenceProbeAsync(ChatDataTemplate chatTemplate)
        {
            await chatTemplate.Client.GENERAL_COMMAND_HELPER.sendPresenceProbeAsync(chatTemplate.Client.getXMPPAccount().getFullJid(), chatTemplate.Chat.chatJabberId);
        }

        public async Task RequestPresenceSubscriptionAsync(ChatDataTemplate chatTemplate)
        {
            await chatTemplate.Client.GENERAL_COMMAND_HELPER.requestPresenceSubscriptionAsync(chatTemplate.Chat.chatJabberId);
        }

        public async Task CancelPresenceSubscriptionAsync(ChatDataTemplate chatTemplate)
        {
            await chatTemplate.Client.GENERAL_COMMAND_HELPER.unsubscribeFromPresenceAsync(chatTemplate.Chat.chatJabberId);
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
                ChatDBManager.INSTANCE.setChat(chatTemplate.Chat, false, true);
            });
        }

        public void MarkAsRead(ChatDataTemplate chat)
        {
            Task.Run(() =>
            {
                ChatDBManager.INSTANCE.markAllMessagesAsRead(chat.Chat.id);
                ToastHelper.UpdateBadgeNumber();
            });
        }

        public async Task RejectPresenceSubscriptionAsync(ChatDataTemplate chatTemplate)
        {
            await chatTemplate.Client.GENERAL_COMMAND_HELPER.answerPresenceSubscriptionRequestAsync(chatTemplate.Chat.chatJabberId, false);
            await Task.Run(() =>
            {
                switch (chatTemplate.Chat.subscription)
                {
                    case "from":
                        chatTemplate.Chat.subscription = "none";
                        break;

                    case "both":
                        chatTemplate.Chat.subscription = "to";
                        break;
                }
                ChatDBManager.INSTANCE.setChat(chatTemplate.Chat, false, true);
            });
        }

        public async Task AnswerPresenceSubscriptionRequestAsync(ChatDataTemplate chatTemplate, bool accepted)
        {
            await chatTemplate.Client.GENERAL_COMMAND_HELPER.answerPresenceSubscriptionRequestAsync(chatTemplate.Chat.chatJabberId, accepted);
            await Task.Run(() =>
            {
                chatTemplate.Chat.subscription = accepted ? "to" : "none";
                ChatDBManager.INSTANCE.setChat(chatTemplate.Chat, false, true);
            });
        }

        #endregion

        #region --Misc Methods (Private)--
        private void UpdateView(ChatDataTemplate chatTemplate)
        {
            if (chatTemplate is null)
            {

            }
            else
            {
                MODEL.UpdateViewClient(chatTemplate.Client);
                MODEL.UpdateViewChat(chatTemplate.Chat);
                MODEL.UpdateViewMuc(chatTemplate.Chat, chatTemplate.MucInfo);
            }
        }

        private void UpdateBookmarks(XMPPClient client)
        {
            List<ConferenceItem> conferences = MUCDBManager.INSTANCE.getXEP0048ConferenceItemsForAccount(client.getXMPPAccount().getBareJid());
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
            int show = Settings.getSettingInt(SettingsConsts.CHAT_SHOW_ACCOUNT_COLOR);
            switch (show)
            {
                case 1:
                    MODEL.ShowAccountColor = true;
                    break;

                case 2:
                    MODEL.ShowAccountColor = false;
                    break;

                default:
                    MODEL.ShowAccountColor = ConnectionHandler.INSTANCE.getClients().Count > 1;
                    break;
            }

            Settings.SettingChanged += Settings_SettingChanged;
        }

        #endregion

        #region --Misc Methods (Protected)--


        #endregion
        //--------------------------------------------------------Events:---------------------------------------------------------------------\\
        #region --Events--
        private void Chat_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (sender is ChatDataTemplate chat)
            {
                UpdateView(chat);
            }
        }

        private void THEME_LISTENER_ThemeChanged(ThemeListener sender)
        {
            MODEL.OnThemeChanged();
        }

        private void Chat_NewChatMessage(ChatDataTemplate chat, Data_Manager2.Classes.Events.NewChatMessageEventArgs args)
        {
            MODEL.UpdateLastAction(chat.Chat);
            MODEL.UpdateUnreadCount(chat.Chat);
        }

        private void Chat_ChatMessageChanged(ChatDataTemplate chat, Data_Manager2.Classes.Events.ChatMessageChangedEventArgs args)
        {
            MODEL.UpdateLastAction(chat.Chat);
            MODEL.UpdateUnreadCount(chat.Chat);
        }

        private void Settings_SettingChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
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
