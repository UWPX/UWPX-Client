﻿using System.Collections.Generic;
using System.Threading.Tasks;
using Manager.Classes;
using Manager.Classes.Chat;
using Shared.Classes;
using Storage.Classes.Models.Chat;
using UWPX_UI_Context.Classes.DataTemplates.Controls.Chat.MUC;
using Windows.UI.Xaml;
using XMPP_API.Classes;
using XMPP_API.Classes.Network.XML.Messages;
using XMPP_API.Classes.Network.XML.Messages.XEP_0048;

namespace UWPX_UI_Context.Classes.DataContext.Controls.Chat.MUC
{
    public class MucInfoControlContext
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        public readonly MucInfoControlDataTemplate MODEL = new MucInfoControlDataTemplate();

        private MessageResponseHelper<IQMessage> updateBookmarksHelper;
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
            if (e.OldValue is ChatDataTemplate oldChat)
            {
                oldChat.PropertyChanged -= Chat_PropertyChanged;
            }

            if (e.NewValue is ChatDataTemplate newChat)
            {
                UpdateView(newChat.Chat);
                UpdateView(newChat.MucInfo);
                newChat.PropertyChanged += Chat_PropertyChanged;
            }
        }

        public async Task ToggleChatMutedAsync(ChatModel chat)
        {
            if (chat is null)
            {
                return;
            }

            await Task.Run(() =>
            {
                chat.muted = !chat.muted;
                UpdateView(chat);
                ChatDBManager.INSTANCE.setChat(chat, false, true);
            }).ConfAwaitFalse();
        }

        public async Task ToggleMucAutoJoinAsync(MucInfoModel mucInfo)
        {
            if (mucInfo is null)
            {
                return;
            }

            await Task.Run(() =>
            {
                mucInfo.autoEnterRoom = !mucInfo.autoEnterRoom;
                UpdateView(mucInfo);
                MUCDBManager.INSTANCE.setMUCChatInfo(mucInfo, false, true);
            }).ConfAwaitFalse();
        }

        public void ToggleChatBookmarked(ChatModel chat, XMPPClient client)
        {
            SetChatBookmarked(chat, client, !chat.inRoster);
        }

        public async Task LeaveMucAsync(ChatModel chat, MucInfoModel mucInfo, XMPPClient client)
        {
            await MucHandler.INSTANCE.leaveRoomAsync(client, chat, mucInfo);
        }

        public async Task EnterMucAsync(ChatModel chat, MucInfoModel mucInfo, XMPPClient client)
        {
            await MucHandler.INSTANCE.enterMucAsync(client, chat, mucInfo);
        }

        #endregion

        #region --Misc Methods (Private)--
        private void UpdateView(ChatModel chat)
        {
            MODEL.BookmarkText = chat.inRoster ? "Remove bookmark" : "Bookmark";
            MODEL.ChatBareJid = chat.bareJid;
            MODEL.MuteGlyph = chat.muted ? "\uE74F" : "\uE767";
            MODEL.MuteTooltip = chat.muted ? "Unmute" : "Mute";
            if (string.IsNullOrEmpty(MODEL.MucName))
            {
                MODEL.MucName = chat.bareJid;
                MODEL.DifferentMucName = !string.Equals(MODEL.ChatBareJid, MODEL.MucName);
            }
        }

        private void UpdateView(MucInfoModel mucInfo)
        {
            MODEL.MucSubject = mucInfo.subject;
            MODEL.MucState = mucInfo.state;
            MODEL.EnterMucAvailable = mucInfo.state != MucState.ENTERD && mucInfo.state != MucState.ENTERING;
            MODEL.AutoJoin = mucInfo.autoEnterRoom;
            MODEL.Nickname = mucInfo.nickname;
            if (!string.IsNullOrEmpty(mucInfo.name))
            {
                MODEL.MucName = mucInfo.name;
                MODEL.DifferentMucName = !string.Equals(MODEL.ChatBareJid, MODEL.MucName);
            }
        }

        private void SetChatBookmarked(ChatModel chat, XMPPClient client, bool bookmarked)
        {
            if (chat.inRoster != bookmarked)
            {
                chat.inRoster = bookmarked;
                UpdateView(chat);
                ChatDBManager.INSTANCE.setChat(chat, false, true);
            }
            UpdateBookmarks(client);
        }

        private void UpdateBookmarks(XMPPClient client)
        {
            List<ConferenceItem> conferences = MUCDBManager.INSTANCE.getXEP0048ConferenceItemsForAccount(client.getXMPPAccount().getBareJid());
            if (updateBookmarksHelper != null)
            {
                updateBookmarksHelper.Dispose();
            }
            // TODO: Register callbacks for once an error occurred and show a notification to the user
            updateBookmarksHelper = client.PUB_SUB_COMMAND_HELPER.setBookmars_xep_0048(conferences, null, null);
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
                switch (e.PropertyName)
                {
                    case nameof(ChatDataTemplate.MucInfo):
                        UpdateView(chat.Chat.muc);
                        break;

                    case nameof(ChatDataTemplate.Chat):
                        UpdateView(chat.Chat);
                        break;

                    default:
                        break;
                }
            }
        }

        #endregion
    }
}
