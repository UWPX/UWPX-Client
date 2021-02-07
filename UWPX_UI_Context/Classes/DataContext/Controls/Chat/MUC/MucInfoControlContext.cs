using System.Collections.Generic;
using System.Threading.Tasks;
using Manager.Classes;
using Manager.Classes.Chat;
using Storage.Classes.Contexts;
using Storage.Classes.Models.Chat;
using UWPX_UI_Context.Classes.DataTemplates.Controls.Chat.MUC;
using Windows.UI.Xaml;
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
                UpdateView(newChat.Chat.muc);
                newChat.PropertyChanged += Chat_PropertyChanged;
            }
        }

        public void ToggleChatMuted(ChatDataTemplate chat)
        {
            if (chat is null)
            {
                return;
            }
            chat.Chat.muted = !chat.Chat.muted;
            UpdateView(chat.Chat);
            chat.Chat.Save();
        }

        public void ToggleMucAutoJoin(ChatDataTemplate chat)
        {
            if (chat is null)
            {
                return;
            }

            chat.Chat.muc.autoEnterRoom = !chat.Chat.muc.autoEnterRoom;
            UpdateView(chat.Chat.muc);
            chat.Chat.muc.Save();
        }

        public void ToggleChatBookmarked(ChatDataTemplate chat)
        {
            SetChatBookmarked(chat, !chat.Chat.inRoster);
        }

        public async Task LeaveMucAsync(ChatDataTemplate chat)
        {
            await MucHandler.INSTANCE.LeaveRoomAsync(chat.Client.xmppClient, chat.Chat.muc);
        }

        public async Task EnterMucAsync(ChatDataTemplate chat)
        {
            await MucHandler.INSTANCE.EnterMucAsync(chat.Client.xmppClient, chat.Chat.muc);
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

        private void SetChatBookmarked(ChatDataTemplate chat, bool bookmarked)
        {
            if (chat.Chat.inRoster != bookmarked)
            {
                chat.Chat.inRoster = bookmarked;
                UpdateView(chat.Chat);
                chat.Chat.Save();
            }
            UpdateBookmarks(chat);
        }

        private void UpdateBookmarks(ChatDataTemplate chat)
        {
            List<ConferenceItem> conferences;
            using (MainDbContext ctx = new MainDbContext())
            {
                conferences = ctx.GetXEP0048ConferenceItemsForAccount(chat.Client.dbAccount.bareJid);
            }
            if (updateBookmarksHelper != null)
            {
                updateBookmarksHelper.Dispose();
            }
            // TODO: Register callbacks for once an error occurred and show a notification to the user
            updateBookmarksHelper = chat.Client.xmppClient.PUB_SUB_COMMAND_HELPER.setBookmars_xep_0048(conferences, null, null);
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
                    case nameof(ChatDataTemplate.Chat):
                        UpdateView(chat.Chat);
                        UpdateView(chat.Chat.muc);
                        break;

                    default:
                        break;
                }
            }
        }

        #endregion
    }
}
