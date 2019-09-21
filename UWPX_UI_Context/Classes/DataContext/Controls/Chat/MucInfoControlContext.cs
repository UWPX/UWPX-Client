using System.Collections.Generic;
using System.Threading.Tasks;
using Data_Manager2.Classes;
using Data_Manager2.Classes.DBManager;
using Data_Manager2.Classes.DBTables;
using Shared.Classes;
using UWPX_UI_Context.Classes.DataTemplates.Controls.Chat;
using Windows.UI.Xaml;
using XMPP_API.Classes;
using XMPP_API.Classes.Network.XML.Messages;
using XMPP_API.Classes.Network.XML.Messages.XEP_0048;

namespace UWPX_UI_Context.Classes.DataContext.Controls.Chat
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
            if (e.NewValue is ChatTable chat)
            {
                UpdateView(chat);
            }
            else if (e.NewValue is MUCChatInfoTable mucInfo)
            {
                UpdateView(mucInfo);
            }
        }

        public async Task ToggleChatMutedAsync(ChatTable chat)
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

        public async Task ToggleMucAutoJoinAsync(MUCChatInfoTable mucInfo)
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

        public void ToggleChatBookmarked(ChatTable chat, XMPPClient client)
        {
            SetChatBookmarked(chat, client, !chat.inRoster);
        }

        public async Task LeaveMucAsync(ChatTable chat, MUCChatInfoTable mucInfo, XMPPClient client)
        {
            await MUCHandler.INSTANCE.leaveRoomAsync(client, chat, mucInfo);
        }

        public async Task EnterMucAsync(ChatTable chat, MUCChatInfoTable mucInfo, XMPPClient client)
        {
            await MUCHandler.INSTANCE.enterMUCAsync(client, chat, mucInfo);
        }

        #endregion

        #region --Misc Methods (Private)--
        private void UpdateView(ChatTable chat)
        {
            MODEL.BookmarkText = chat.inRoster ? "Remove bookmark" : "Bookmark";
            MODEL.ChatBareJid = chat.chatJabberId;
            MODEL.MuteGlyph = chat.muted ? "\uE74F" : "\uE767";
            MODEL.MuteTooltip = chat.muted ? "Unmute" : "Mute";
            if (string.IsNullOrEmpty(MODEL.MucName))
            {
                MODEL.MucName = chat.chatJabberId;
                MODEL.DifferentMucName = !string.Equals(MODEL.ChatBareJid, MODEL.MucName);
            }
        }

        private void UpdateView(MUCChatInfoTable mucInfo)
        {
            MODEL.MucSubject = mucInfo.subject;
            MODEL.MucState = mucInfo.state;
            MODEL.EnterMucAvailable = mucInfo.state != MUCState.ENTERD && mucInfo.state != MUCState.ENTERING;
            MODEL.AutoJoin = mucInfo.autoEnterRoom;
            if (!string.IsNullOrEmpty(mucInfo.name))
            {
                MODEL.MucName = mucInfo.name;
                MODEL.DifferentMucName = !string.Equals(MODEL.ChatBareJid, MODEL.MucName);
            }
        }

        private void SetChatBookmarked(ChatTable chat, XMPPClient client, bool bookmarked)
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


        #endregion
    }
}
