using System.Collections.Generic;
using System.Threading.Tasks;
using Logging;
using Manager.Classes;
using Manager.Classes.Chat;
using Storage.Classes.Contexts;
using Storage.Classes.Models.Chat;
using UWPX_UI_Context.Classes.DataTemplates.Dialogs;
using XMPP_API.Classes.Network.XML.Messages;
using XMPP_API.Classes.Network.XML.Messages.Helper;
using XMPP_API.Classes.Network.XML.Messages.XEP_0048;

namespace UWPX_UI_Context.Classes.DataContext.Dialogs
{
    public sealed class AddMucDialogContext
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        public readonly AddMucDialogDataTemplate MODEL = new AddMucDialogDataTemplate();

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--


        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--


        #endregion
        //--------------------------------------------------------Misc Methods:---------------------------------------------------------------\\
        #region --Misc Methods (Public)--
        public async Task AddAsync()
        {
            MODEL.IsAdding = true;
            await AddMucAsync(MODEL.Client, MODEL.RoomBareJid, MODEL.Nickname, MODEL.Password, MODEL.Bookmark, MODEL.AutoJoin);
            MODEL.IsAdding = false;
            MODEL.Confirmed = true;
        }

        public void Clancel()
        {
            MODEL.Confirmed = false;
        }

        public void FromDirectInvite(MucDirectInvitationModel invite)
        {
            MODEL.Password = invite.roomPassword;
            MODEL.RoomBareJid = invite.roomJid;
        }

        #endregion

        #region --Misc Methods (Private)--
        private async Task AddMucAsync(Client client, string roomBareJid, string nickname, string password, bool bookmark, bool autoJoin)
        {
            ChatModel chat = new ChatModel(roomBareJid, client.dbAccount)
            {
                chatType = ChatType.MUC,
                inRoster = bookmark,
                subscription = "none",
                isChatActive = true
            };
            MucInfoModel muc = new MucInfoModel()
            {
                chat = chat,
                subject = null,
                autoEnterRoom = autoJoin,
                name = null,
                nickname = nickname,
                password = string.IsNullOrEmpty(password) ? null : password,
                state = MucState.DISCONNECTED
            };
            chat.muc = muc;
            SemaLock semaLock = DataCache.INSTANCE.NewChatSemaLock();
            DataCache.INSTANCE.AddChatUnsafe(chat, client);
            semaLock.Dispose();

            if (muc.autoEnterRoom)
            {
                await MucHandler.INSTANCE.EnterMucAsync(client.xmppClient, muc);
            }

            if (bookmark)
            {
                List<ConferenceItem> conferenceItems;
                using (MainDbContext ctx = new MainDbContext())
                {
                    conferenceItems = ctx.GetXEP0048ConferenceItemsForAccount(client.dbAccount.bareJid);
                }
                MessageResponseHelperResult<IQMessage> result = await client.xmppClient.PUB_SUB_COMMAND_HELPER.setBookmars_xep_0048Async(conferenceItems);
                if (result.STATE == MessageResponseHelperResultState.SUCCESS)
                {
                    if (result.RESULT is IQErrorMessage errMsg)
                    {
                        Logger.Warn("Failed to set bookmarks: " + errMsg.ToString());
                    }
                }
                else
                {
                    Logger.Warn("Failed to set bookmarks: " + result.STATE);
                }
            }
        }

        #endregion

        #region --Misc Methods (Protected)--


        #endregion
        //--------------------------------------------------------Events:---------------------------------------------------------------------\\
        #region --Events--


        #endregion
    }
}
