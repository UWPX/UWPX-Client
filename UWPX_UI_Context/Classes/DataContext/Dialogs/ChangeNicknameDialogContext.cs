using System.Collections.Generic;
using System.Threading.Tasks;
using Logging;
using Manager.Classes.Chat;
using Storage.Classes.Contexts;
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
        public void UpdateView(ChatDataTemplate chat)
        {
            MODEL.MucName = string.IsNullOrEmpty(chat.Chat.muc.name) ? chat.Chat.bareJid : chat.Chat.muc.name;
            MODEL.Nickname = chat.Chat.muc.nickname;
        }

        /// <summary>
        /// Updates the nickname in the local DB.
        /// Also updates bookmarks if any exist.
        /// </summary>
        /// <returns>Returns true on success.</returns>
        public async Task<bool> SaveAsync(ChatDataTemplate chat)
        {
            MODEL.IsSaving = true;
            bool success = await Task.Run(async () =>
            {
                chat.Chat.muc.nickname = MODEL.Nickname;

                // Try to update the nickname at the chat room:
                bool result = await UpdateNicknameAsync(chat);
                if (!result)
                {
                    return false;
                }

                // Update the DB entry since the server could respond with a different nickname:
                chat.Chat.muc.Update();

                // Update bookmarks:
                return chat.Chat.inRoster ? await chat.Client.PublishBookmarksAsync() : true;
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
        private async Task<bool> UpdateNicknameAsync(ChatDataTemplate chat)
        {
            MessageResponseHelperResult<MUCMemberPresenceMessage> result = await chat.Client.xmppClient.MUC_COMMAND_HELPER.changeNicknameAsync(chat.Chat.bareJid, chat.Chat.muc.nickname);
            if (!string.IsNullOrEmpty(result.RESULT.ERROR_TYPE))
            {
                Logger.Warn("Failed to change nickname for room \"" + chat.Chat.bareJid + "\" to \"" + chat.Chat.muc.nickname + "\" with: " + result.RESULT.ERROR_MESSAGE);
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
                chat.Chat.muc.nickname = Utils.getJidResourcePart(result.RESULT.JID);
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
