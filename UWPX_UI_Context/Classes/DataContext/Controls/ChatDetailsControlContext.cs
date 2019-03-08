using Data_Manager2.Classes;
using Data_Manager2.Classes.DBManager;
using Data_Manager2.Classes.DBTables;
using Logging;
using System;
using System.Threading.Tasks;
using UWPX_UI_Context.Classes.DataTemplates;
using UWPX_UI_Context.Classes.DataTemplates.Controls;
using Windows.System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Input;
using XMPP_API.Classes.Network.XML.Messages;
using XMPP_API.Classes.Network.XML.Messages.XEP_0384;

namespace UWPX_UI_Context.Classes.DataContext.Controls
{
    public sealed partial class ChatDetailsControlContext
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        public readonly ChatDetailsControlDataTemplate MODEL = new ChatDetailsControlDataTemplate();

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
            if (args.NewValue == args.OldValue)
            {
                return;
            }

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

        public async Task SendChatMessageAsync(ChatDataTemplate chat)
        {
            if (!string.IsNullOrWhiteSpace(MODEL.MessageText))
            {
                // Remove tailing and leading whitespaces, tabs and newlines:
                string trimedMsg = MODEL.MessageText.Trim(UiUtils.TRIM_CHARS);

                // Send message:
                if (MODEL.isDummy)
                {
                    SendDummyMessage(chat.Chat, trimedMsg);
                }
                else if (MODEL.OmemoEnabled)
                {
                    await SendChatMessageAsync(trimedMsg, chat, true);
                    Logger.Debug("Send encrypted: " + trimedMsg);
                }
                else
                {
                    await SendChatMessageAsync(trimedMsg, chat, false);
                    Logger.Debug("Send unencrypted: " + trimedMsg);
                }
                MODEL.MessageText = string.Empty;
            }
        }

        private async Task SendChatMessageAsync(string message, ChatDataTemplate chat, bool encrypt)
        {
            MessageMessage toSendMsg;

            string from = chat.Client.getXMPPAccount().getIdDomainAndResource();
            string to = chat.Chat.chatJabberId;
            string chatType = chat.Chat.chatType == ChatType.CHAT ? MessageMessage.TYPE_CHAT : MessageMessage.TYPE_GROUPCHAT;
            bool reciptRequested = true;

            if (encrypt)
            {
                if (chat.Chat.chatType == ChatType.CHAT)
                {
                    toSendMsg = new OmemoMessageMessage(from, to, message, chatType, reciptRequested);
                }
                else
                {
                    // ToDo: Add MUC OMEMO support
                    throw new NotImplementedException("Sending encrypted messages for MUC is not supported right now!");
                }
            }
            else
            {
                if (chat.Chat.chatType == ChatType.CHAT)
                {
                    toSendMsg = new MessageMessage(from, to, message, chatType, reciptRequested);
                }
                else
                {
                    toSendMsg = new MessageMessage(from, to, message, chatType, chat.MucInfo.nickname, reciptRequested);
                }
            }

            // Create a copy for the DB:
            ChatMessageTable toSendMsgDB = new ChatMessageTable(toSendMsg, chat.Chat)
            {
                state = toSendMsg is OmemoMessageMessage ? MessageState.TO_ENCRYPT : MessageState.SENDING
            };

            // Set the chat message id for later identification:
            toSendMsg.chatMessageId = toSendMsgDB.id;

            // Update chat last active:
            chat.Chat.lastActive = DateTime.Now;

            // Update DB:
            await Task.Run(() =>
            {
                ChatDBManager.INSTANCE.setChatMessage(toSendMsgDB, true, false);
                ChatDBManager.INSTANCE.setChat(chat.Chat, false, true);
            });

            // Send the message:
            if (toSendMsg is OmemoMessageMessage toSendOmemoMsg)
            {
                chat.Client.sendOmemoMessage(toSendOmemoMsg, chat.Chat.chatJabberId, chat.Client.getXMPPAccount().getIdAndDomain());
            }
            else
            {
                await chat.Client.sendAsync(toSendMsg).ConfigureAwait(false);
            }
        }

        public async Task OnChatMessageKeyDown(KeyRoutedEventArgs args, ChatDataTemplate chat)
        {
            if (args.Key == VirtualKey.Enter)
            {
                if (!string.IsNullOrWhiteSpace(MODEL.MessageText))
                {
                    if (!UiUtils.IsVirtualKeyDown(VirtualKey.Shift) && Settings.getSettingBoolean(SettingsConsts.ENTER_TO_SEND_MESSAGES))
                    {
                        await SendChatMessageAsync(chat);
                        args.Handled = true;
                    }
                }
            }
        }

        public async Task OnReadOnOmemoClickedAsync()
        {
            await UiUtils.LaunchUriAsync(new Uri("https://conversations.im/omemo/"));
        }

        public async Task LeaveMucAsync(ChatDataTemplate chatTemplate)
        {
            if (!MODEL.isDummy)
            {
                await MUCHandler.INSTANCE.leaveRoomAsync(chatTemplate.Client, chatTemplate.Chat, chatTemplate.MucInfo);
            }
        }

        public async Task EnterMucAsync(ChatDataTemplate chatTemplate)
        {
            if (!MODEL.isDummy)
            {
                await MUCHandler.INSTANCE.enterMUCAsync(chatTemplate.Client, chatTemplate.Chat, chatTemplate.MucInfo);
            }
        }

        #endregion

        #region --Misc Methods (Private)--
        private void UpdateView(ChatDataTemplate chatTemplate)
        {
            if (!(chatTemplate is null))
            {
                MODEL.UpdateViewClient(chatTemplate.Client);
                if (chatTemplate.Chat.chatType == ChatType.MUC)
                {
                    MODEL.UpdateViewChat(chatTemplate.Chat, chatTemplate.MucInfo);
                }
                else
                {
                    MODEL.UpdateViewChat(chatTemplate.Chat, null);
                }
                MODEL.UpdateViewMuc(chatTemplate.Chat, chatTemplate.MucInfo);
            }
            MODEL.LoadSettings();
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

        private async void Chat_NewChatMessage(ChatDataTemplate chat, Data_Manager2.Classes.Events.NewChatMessageEventArgs args)
        {
            if (!MODEL.isDummy)
            {
                await MODEL.OnNewChatMessageAsync(args.MESSAGE, chat.Chat, chat.MucInfo);
                if (args.MESSAGE.state == MessageState.UNREAD)
                {
                    await Task.Run(() =>
                    {
                        ChatDBManager.INSTANCE.markMessageAsRead(args.MESSAGE);
                    });
                }
            }
        }

        private async void Chat_ChatMessageChanged(ChatDataTemplate chat, Data_Manager2.Classes.Events.ChatMessageChangedEventArgs args)
        {
            if (!MODEL.isDummy)
            {
                await MODEL.OnChatMessageChangedAsync(args.MESSAGE);
            }
        }

        #endregion
    }
}
