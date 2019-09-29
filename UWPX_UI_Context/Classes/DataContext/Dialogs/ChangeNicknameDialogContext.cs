using System.Collections.Generic;
using System.Threading.Tasks;
using Data_Manager2.Classes.DBManager;
using Data_Manager2.Classes.DBTables;
using Logging;
using UWPX_UI_Context.Classes.DataTemplates.Dialogs;
using XMPP_API.Classes;
using XMPP_API.Classes.Network.XML.Messages;
using XMPP_API.Classes.Network.XML.Messages.Helper;
using XMPP_API.Classes.Network.XML.Messages.XEP_0045;
using XMPP_API.Classes.Network.XML.Messages.XEP_0048;

namespace UWPX_UI_Context.Classes.DataContext.Dialogs
{
    public class ChangeNicknameDialogContext
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        public readonly ChangeNicknameDialogDataTemplate MODEL = new ChangeNicknameDialogDataTemplate();

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--


        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--


        #endregion
        //--------------------------------------------------------Misc Methods:---------------------------------------------------------------\\
        #region --Misc Methods (Public)--
        public void UpdateView(ChatTable chat, MUCChatInfoTable mucInfo)
        {
            MODEL.MucName = string.IsNullOrEmpty(mucInfo.name) ? chat.chatJabberId : mucInfo.name;
            MODEL.Nickname = mucInfo.nickname;
        }

        /// <summary>
        /// Updates the nickname in the local DB.
        /// Also updates bookmarks if any exist.
        /// </summary>
        /// <returns>Returns true on success.</returns>
        public async Task<bool> SaveAsync(ChatTable chat, MUCChatInfoTable mucInfo, XMPPClient client)
        {
            MODEL.IsSaving = true;
            bool success = await Task.Run(async () =>
            {
                mucInfo.nickname = MODEL.Nickname;

                // Try to update the nickname at the chat room:
                bool result = await UpdateNicknameAsync(chat, mucInfo, client);
                if (!result)
                {
                    return false;
                }

                // Update the DB entry since the server could respond with a different nickname:
                MUCDBManager.INSTANCE.setMUCChatInfo(mucInfo, false, true);

                // Update bookmarks:
                return chat.inRoster ? await UpdateBookmarksAsync(chat, client) : true;
            });
            MODEL.IsSaving = false;
            MODEL.Error = !success;
            if (success)
            {
                Logger.Info("Updating nickname to \"" + MODEL.Nickname + "\" in chat \"" + MODEL.MucName + "\" was successful!");
            }
            else
            {
                Logger.Info("Updating nickname to \"" + MODEL.Nickname + "\" in chat \"" + MODEL.MucName + "\" failed!");
            }
            return success;
        }

        #endregion

        #region --Misc Methods (Private)--
        private async Task<bool> UpdateBookmarksAsync(ChatTable chat, XMPPClient client)
        {
            List<ConferenceItem> conferences = MUCDBManager.INSTANCE.getXEP0048ConferenceItemsForAccount(client.getXMPPAccount().getBareJid());
            MessageResponseHelperResult<IQMessage> result = await client.PUB_SUB_COMMAND_HELPER.setBookmars_xep_0048Async(conferences);
            if (string.Equals(result.RESULT.TYPE, IQMessage.RESULT))
            {
                return true;
            }
            if (result.RESULT is IQErrorMessage errorMessage)
            {
                Logger.Warn("Failed to update XEP-0048 Bookmarks: " + errorMessage.ERROR_OBJ.ToString());
            }
            else
            {
                Logger.Warn("Failed to update XEP-0048 Bookmarks: " + result.RESULT.TYPE);
            }
            return false;
        }

        private async Task<bool> UpdateNicknameAsync(ChatTable chat, MUCChatInfoTable mucInfo, XMPPClient client)
        {
            MessageResponseHelperResult<MUCMemberPresenceMessage> result = await client.MUC_COMMAND_HELPER.changeNicknameAsync(chat.chatJabberId, mucInfo.nickname);
            if (!string.IsNullOrEmpty(result.RESULT.ERROR_TYPE))
            {
                Logger.Warn("Failed to change nickname for room \"" + chat.chatJabberId + "\" to \"" + mucInfo.nickname + "\" with: " + result.RESULT.ERROR_MESSAGE);
                return false;
            }
            // Nickname has been updated successfully:
            else if (result.RESULT.STATUS_CODES.Contains(MUCPresenceStatusCode.MEMBER_NICK_CHANGED))
            {
                return true;
            }
            // The room has change the nickname:
            else if (result.RESULT.STATUS_CODES.Contains(MUCPresenceStatusCode.ROOM_NICK_CHANGED))
            {
                if (!Utils.isFullJid(result.RESULT.JID))
                {
                    Logger.Error("Expected a full JID as a MUC nickname changed message with status code 210, but received: " + result.RESULT.JID);
                    return false;
                }
                mucInfo.nickname = Utils.getJidResourcePart(result.RESULT.JID);
                return true;
            }
            // Should not happen:
            Logger.Error("Unknown MUC member presence status code combination received. This should not happen!");
            return false;
        }

        #endregion

        #region --Misc Methods (Protected)--


        #endregion
        //--------------------------------------------------------Events:---------------------------------------------------------------------\\
        #region --Events--


        #endregion
    }
}
