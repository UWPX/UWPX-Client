using Data_Manager2.Classes;
using Data_Manager2.Classes.DBManager;
using Logging;
using Microsoft.Toolkit.Uwp.UI.Helpers;
using System.Collections.Generic;
using System.Threading.Tasks;
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
            this.MODEL = new ChatMasterControlDataTemplate(resources);
            this.THEME_LISTENER.ThemeChanged += THEME_LISTENER_ThemeChanged;
        }

        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--
        private async Task SetChatInRosterAsync(ChatDataTemplate Chat, bool inRoster)
        {
            if (Chat.Chat.inRoster != inRoster)
            {
                Chat.Chat.inRoster = inRoster;
                UpdateView(Chat);
                ChatDBManager.INSTANCE.setChat(Chat.Chat, false, true);
            }

            if (inRoster)
            {
                await Chat.Client.GENERAL_COMMAND_HELPER.addToRosterAsync(Chat.Chat.chatJabberId);
            }
            else
            {
                await Chat.Client.GENERAL_COMMAND_HELPER.removeFromRosterAsync(Chat.Chat.chatJabberId);
            }
        }

        private void SetChatBookmarked(ChatDataTemplate Chat, bool bookmarked)
        {
            if (Chat.Chat.inRoster != bookmarked)
            {
                Chat.Chat.inRoster = bookmarked;
                UpdateView(Chat);
                ChatDBManager.INSTANCE.setChat(Chat.Chat, false, true);
            }
            UpdateBookmarks(Chat.Client);
        }

        public async Task SetChatMutedAsync(ChatDataTemplate Chat, bool muted)
        {
            if (Chat.Chat.muted != muted)
            {
                await Task.Run(() =>
                {
                    Chat.Chat.muted = muted;
                    UpdateView(Chat);
                    ChatDBManager.INSTANCE.setChat(Chat.Chat, false, true);
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
                oldChat.PropertyChanged -= OldChat_PropertyChanged;
                oldChat.ChatMessageChanged -= OldChat_ChatMessageChanged;
                oldChat.NewChatMessage -= OldChat_NewChatMessage;
            }

            if (args.NewValue is ChatDataTemplate)
            {
                newChat = args.NewValue as ChatDataTemplate;
                newChat.PropertyChanged += OldChat_PropertyChanged;
                newChat.ChatMessageChanged += OldChat_ChatMessageChanged;
                newChat.NewChatMessage += OldChat_NewChatMessage;
            }

            UpdateView(newChat);
        }

        public async Task SwitchChatInRosterAsync(ChatDataTemplate Chat)
        {
            await SetChatInRosterAsync(Chat, !Chat.Chat.inRoster);
        }

        public void SwitchChatBookmarked(ChatDataTemplate Chat)
        {
            SetChatBookmarked(Chat, !Chat.Chat.inRoster);
        }

        public async Task DeleteChatAsync(DeleteChatConfirmDialogDataTemplate confirmDialogModel, ChatDataTemplate Chat)
        {
            if (confirmDialogModel.Confirmed)
            {
                await Task.Run(async () =>
                {
                    if (Chat.Chat.chatType == ChatType.MUC)
                    {
                        SetChatBookmarked(Chat, false);

                        MUCDBManager.INSTANCE.setMUCChatInfo(Chat.MucInfo, true, false);
                        Logger.Info("Deleted MUC info for: " + Chat.Chat.id);
                    }
                    else
                    {
                        if (confirmDialogModel.RemoveFromRoster)
                        {
                            await SetChatInRosterAsync(Chat, false);
                        }
                    }

                    if (!confirmDialogModel.KeepChatMessages)
                    {
                        await ChatDBManager.INSTANCE.deleteAllChatMessagesForChatAsync(Chat.Chat.id);
                        Logger.Info("Deleted chat messages for: " + Chat.Chat.id);
                    }

                    if (Chat.Chat.chatType == ChatType.MUC || confirmDialogModel.RemoveFromRoster)
                    {
                        ChatDBManager.INSTANCE.setChat(Chat.Chat, true, true);
                        Logger.Info("Deleted chat: " + Chat.Chat.id);
                    }
                    else
                    {
                        Chat.Chat.isChatActive = false;
                        ChatDBManager.INSTANCE.setChat(Chat.Chat, false, true);
                        Logger.Info("Marked chat as not active: " + Chat.Chat.id);
                    }
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
            Task.Run(() => ChatDBManager.INSTANCE.markAllMessagesAsRead(chat.Chat.id));
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

        #endregion

        #region --Misc Methods (Protected)--


        #endregion
        //--------------------------------------------------------Events:---------------------------------------------------------------------\\
        #region --Events--
        private void OldChat_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
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

        private void OldChat_NewChatMessage(ChatDataTemplate chat, Data_Manager2.Classes.Events.NewChatMessageEventArgs args)
        {
            MODEL.UpdateLastAction(chat.Chat);
            MODEL.UpdateUnreadCount(chat.Chat);
        }

        private void OldChat_ChatMessageChanged(ChatDataTemplate chat, Data_Manager2.Classes.Events.ChatMessageChangedEventArgs args)
        {
            MODEL.UpdateLastAction(chat.Chat);
            MODEL.UpdateUnreadCount(chat.Chat);
        }

        #endregion
    }
}
