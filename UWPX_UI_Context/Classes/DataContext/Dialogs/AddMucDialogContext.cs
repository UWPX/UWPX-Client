using System.Collections.Generic;
using System.Threading.Tasks;
using Logging;
using Storage.Classes.Models.Chat;
using UWPX_UI_Context.Classes.DataTemplates.Dialogs;
using XMPP_API.Classes;
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
        private async Task AddMucAsync(XMPPClient client, string roomBareJid, string nickname, string password, bool bookmark, bool autoJoin)
        {
            ChatModel muc = new ChatModel(roomBareJid, client.getXMPPAccount().getBareJid())
            {
                chatType = ChatType.MUC,
                inRoster = bookmark,
                subscription = "none",
                isChatActive = true
            };

            MucInfoModel info = new MucInfoModel()
            {
                chatId = muc.id,
                subject = null,
                state = MucState.DISCONNECTED,
                name = null,
                password = string.IsNullOrEmpty(password) ? null : password,
                nickname = nickname,
                autoEnterRoom = autoJoin,
            };

            await Task.Run(() =>
            {
                ChatDBManager.INSTANCE.setChat(muc, false, true);
                MUCDBManager.INSTANCE.setMUCChatInfo(info, false, true);
            });

            if (info.autoEnterRoom)
            {
                await MucHandler.INSTANCE.enterMucAsync(client, muc, info);
            }

            if (bookmark)
            {
                List<ConferenceItem> conferenceItems = MUCDBManager.INSTANCE.getXEP0048ConferenceItemsForAccount(client.getXMPPAccount().getBareJid());
                MessageResponseHelperResult<IQMessage> result = await client.PUB_SUB_COMMAND_HELPER.setBookmars_xep_0048Async(conferenceItems);
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
