using System;
using System.ComponentModel;
using System.Threading.Tasks;
using Logging;
using Manager.Classes;
using Manager.Classes.Chat;
using Storage.Classes;
using Storage.Classes.Models.Account;
using Storage.Classes.Models.Chat;
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
                if (oldChat.Chat is not null)
                {
                    oldChat.Chat.PropertyChanged -= OnChatPropertyChanged;
                }
                if (oldChat.Chat.muc is not null)
                {
                    oldChat.Chat.muc.PropertyChanged -= OnMucPropertyChanged;
                }
                if (oldChat.Client.dbAccount is not null)
                {
                    oldChat.Client.dbAccount.PropertyChanged -= OnAccountPropertyChanged;
                }
                oldChat.CHAT_STATE_HELPER.SetInactive();
            }

            if (args.NewValue is ChatDataTemplate)
            {
                newChat = args.NewValue as ChatDataTemplate;
                if (newChat.Chat is not null)
                {
                    newChat.Chat.PropertyChanged += OnChatPropertyChanged;
                }
                if (newChat.Chat.muc is not null)
                {
                    newChat.Chat.muc.PropertyChanged += OnMucPropertyChanged;
                }
                if (newChat.Client.dbAccount is not null)
                {
                    newChat.Client.dbAccount.PropertyChanged += OnAccountPropertyChanged;
                }
                newChat.CHAT_STATE_HELPER.SetActive();
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
                    Logger.Debug("Sending message (encrypted=" + chat.Chat.omemoInfo.enabled + "): " + trimedMsg);
                    Task.Run(async () => await SendChatMessageAsync(chat, trimedMsg));
                }
                MODEL.MessageText = string.Empty;
            }
        }

        private async Task SendChatMessageAsync(ChatDataTemplate chat, string message)
        {
            MessageMessage toSendMsg;

            string from = chat.Client.dbAccount.fullJid.FullJid();
            string to = chat.Chat.bareJid;
            string chatType = chat.Chat.chatType == ChatType.CHAT ? MessageMessage.TYPE_CHAT : MessageMessage.TYPE_GROUPCHAT;
            bool reciptRequested = true;

            if (chat.Chat.omemoInfo.enabled)
            {
                if (chat.Chat.chatType == ChatType.CHAT)
                {
                    toSendMsg = new OmemoEncryptedMessage(from, to, message, chatType, reciptRequested);
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
                    : new MessageMessage(from, to, message, chatType, chat.Chat.muc.nickname, reciptRequested);
            }

            // Create a copy for the DB:
            ChatMessageModel toSendMsgDB = new ChatMessageModel(toSendMsg, chat.Chat)
            {
                state = toSendMsg is OmemoEncryptedMessage ? MessageState.TO_ENCRYPT : MessageState.SENDING
            };

            // Update chat last active:
            chat.Chat.lastActive = DateTime.Now;

            // Update DB:
            chat.Chat.Update();
            await DataCache.INSTANCE.AddChatMessageAsync(toSendMsgDB, chat.Chat);

            // Set the chat message id for later identification:
            toSendMsg.chatMessageId = toSendMsgDB.id;

            // Send the message:
            if (toSendMsg is OmemoEncryptedMessage toSendOmemoMsg)
            {
                await chat.Client.xmppClient.sendOmemoMessageAsync(toSendOmemoMsg, chat.Chat.bareJid, chat.Client.dbAccount.bareJid, chat.Client.dbAccount.omemoInfo.trustedKeysOnly, chat.Chat.omemoInfo.trustedKeysOnly);
            }
            else
            {
                await chat.Client.xmppClient.SendAsync(toSendMsg);
            }
        }

        public void OnEnterKeyDown(KeyRoutedEventArgs args, ChatDataTemplate chat)
        {
            if (Settings.GetSettingBoolean(SettingsConsts.ENTER_TO_SEND_MESSAGES))
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
                await MucHandler.INSTANCE.LeaveRoomAsync(chatTemplate.Client.xmppClient, chatTemplate.Chat.muc);
            }
        }

        public async Task EnterMucAsync(ChatDataTemplate chatTemplate)
        {
            if (!MODEL.isDummy)
            {
                await MucHandler.INSTANCE.EnterMucAsync(chatTemplate.Client.xmppClient, chatTemplate.Chat.muc);
            }
        }

        public void MarkAsIotDevice(ChatModel chat)
        {
            chat.chatType = ChatType.IOT_DEVICE;
            chat.Update();
        }

        #endregion

        #region --Misc Methods (Private)--
        private void UpdateView(ChatDataTemplate chatTemplate)
        {
            if (chatTemplate is not null)
            {
                MODEL.UpdateView(chatTemplate.Client.dbAccount);
                MODEL.UpdateView(chatTemplate.Chat);
                if (chatTemplate.Chat.chatType == ChatType.MUC)
                {
                    MODEL.UpdateView(chatTemplate.Chat?.muc);
                }
            }
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
                MODEL.UpdateView(chat);
            }
        }

        private void OnMucPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (sender is MucInfoModel muc)
            {
                MODEL.UpdateView(muc);
            }
        }

        private void OnAccountPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (sender is AccountModel account)
            {
                MODEL.UpdateView(account);
            }
        }

        #endregion
    }
}
