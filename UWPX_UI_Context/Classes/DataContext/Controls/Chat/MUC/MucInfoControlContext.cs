﻿using System.ComponentModel;
using System.Threading.Tasks;
using Manager.Classes;
using Manager.Classes.Chat;
using Storage.Classes.Models.Chat;
using UWPX_UI_Context.Classes.DataTemplates.Controls.Chat.MUC;
using Windows.UI.Xaml;

namespace UWPX_UI_Context.Classes.DataContext.Controls.Chat.MUC
{
    public class MucInfoControlContext
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        public readonly MucInfoControlDataTemplate MODEL = new MucInfoControlDataTemplate();

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
                if (oldChat.Chat is not null)
                {
                    oldChat.Chat.PropertyChanged -= OnChatPropertyChanged;
                }
                if (oldChat.Chat.muc is not null)
                {
                    oldChat.Chat.muc.PropertyChanged -= OnMucPropertyChanged;
                }
            }

            ChatDataTemplate newChat = null;
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
            }

            UpdateView(newChat?.Chat);
            UpdateView(newChat?.Chat?.muc);
        }

        public void ToggleChatMuted(ChatDataTemplate chat)
        {
            if (chat is null)
            {
                return;
            }
            chat.Chat.muted = !chat.Chat.muted;
            UpdateView(chat.Chat);
            chat.Chat.Update();
        }

        public void ToggleMucAutoJoin(ChatDataTemplate chat)
        {
            if (chat is null)
            {
                return;
            }

            chat.Chat.muc.autoEnterRoom = !chat.Chat.muc.autoEnterRoom;
            UpdateView(chat.Chat.muc);
            chat.Chat.muc.Update();
        }

        public Task ToggleChatBookmarkedAsync(ChatDataTemplate chat)
        {
            return SetChatBookmarkedAsync(chat, !chat.Chat.inRoster);
        }

        public Task LeaveMucAsync(ChatDataTemplate chat)
        {
            return MucHandler.INSTANCE.LeaveRoomAsync(chat.Client.xmppClient, chat.Chat.muc);
        }

        public Task EnterMucAsync(ChatDataTemplate chat)
        {
            return MucHandler.INSTANCE.EnterMucAsync(chat.Client.xmppClient, chat.Chat.muc);
        }

        #endregion

        #region --Misc Methods (Private)--
        private void UpdateView(ChatModel chat)
        {
            MODEL.BookmarkText = chat.inRoster ? "Remove bookmark" : "Bookmark";
            MODEL.MuteGlyph = chat.muted ? "\uE74F" : "\uE767";
            MODEL.MuteTooltip = chat.muted ? "Unmute" : "Mute";
            if (string.IsNullOrEmpty(MODEL.MucName))
            {
                MODEL.MucName = chat.bareJid;
                MODEL.DifferentMucName = !string.Equals(chat.bareJid, MODEL.MucName);
            }
        }

        private void UpdateView(MucInfoModel mucInfo)
        {
            MODEL.EnterMucAvailable = mucInfo.state != MucState.ENTERD && mucInfo.state != MucState.ENTERING;
            if (!string.IsNullOrEmpty(mucInfo.name))
            {
                MODEL.MucName = mucInfo.name;
                MODEL.DifferentMucName = !string.Equals(mucInfo.chat.bareJid, MODEL.MucName);
            }
        }

        private Task SetChatBookmarkedAsync(ChatDataTemplate chat, bool bookmarked)
        {
            if (chat.Chat.inRoster != bookmarked)
            {
                chat.Chat.inRoster = bookmarked;
                UpdateView(chat.Chat);
                chat.Chat.Update();
            }
            return chat.Client.PublishBookmarksAsync();
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
                UpdateView(chat);
            }
        }

        private void OnMucPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (sender is MucInfoModel muc)
            {
                UpdateView(muc);
            }
        }

        #endregion
    }
}
