using System;
using System.Threading.Tasks;
using Data_Manager2.Classes;
using Data_Manager2.Classes.DBManager;
using Data_Manager2.Classes.DBTables;
using Data_Manager2.Classes.Toast;
using Logging;
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
            ChatDataTemplate newChat = null;
            if (args.OldValue is ChatDataTemplate oldChat)
            {
                oldChat.PropertyChanged -= Chat_PropertyChanged;
            }

            if (args.NewValue is ChatDataTemplate)
            {
                newChat = args.NewValue as ChatDataTemplate;
                newChat.PropertyChanged += Chat_PropertyChanged;
            }

            UpdateView(newChat);
        }

        public void SendChatMessage(ChatDataTemplate chat)
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
                else
                {
                    Task.Run(async () => await SendChatMessageAsync(chat, trimedMsg));
                    Logger.Debug("Sending message (encrypted=" + MODEL.OmemoEnabled + "): " + trimedMsg);
                }
                MODEL.MessageText = string.Empty;
            }
        }

        private async Task SendChatMessageAsync(ChatDataTemplate chat, string message)
        {
            MessageMessage toSendMsg;

            string from = chat.Client.getXMPPAccount().getFullJid();
            string to = chat.Chat.chatJabberId;
            string chatType = chat.Chat.chatType == ChatType.CHAT ? MessageMessage.TYPE_CHAT : MessageMessage.TYPE_GROUPCHAT;
            bool reciptRequested = true;

            if (chat.Chat.omemoEnabled)
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
                toSendMsg = chat.Chat.chatType == ChatType.CHAT
                    ? new MessageMessage(from, to, message, chatType, reciptRequested)
                    : new MessageMessage(from, to, message, chatType, chat.MucInfo.nickname, reciptRequested);
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
            await Task.Run(async () =>
            {
                await ChatDBManager.INSTANCE.setChatMessageAsync(toSendMsgDB, true, false);
                ChatDBManager.INSTANCE.setChat(chat.Chat, false, true);
            });

            // Send the message:
            if (toSendMsg is OmemoMessageMessage toSendOmemoMsg)
            {
                await chat.Client.sendOmemoMessageAsync(toSendOmemoMsg, chat.Chat.chatJabberId, chat.Client.getXMPPAccount().getBareJid());
            }
            else
            {
                await chat.Client.SendAsync(toSendMsg);
            }
        }

        public void OnEnterKeyDown(KeyRoutedEventArgs args, ChatDataTemplate chat)
        {
            if (Settings.getSettingBoolean(SettingsConsts.ENTER_TO_SEND_MESSAGES))
            {
                if (UiUtils.IsVirtualKeyDown(VirtualKey.Shift))
                {
                    Logger.Info("Enter + Shift down.");
                    return;
                }
                Logger.Info("Enter down.");

                args.Handled = true;
                if (!string.IsNullOrWhiteSpace(MODEL.MessageText))
                {
                    SendChatMessage(chat);
                }
            }
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

        public void MarkAsRead(ChatDataTemplate chat)
        {
            Task.Run(() =>
            {
                ChatDBManager.INSTANCE.markAllMessagesAsRead(chat.Chat.id);
                ToastHelper.UpdateBadgeNumber();
            });
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

        #endregion
    }
}
