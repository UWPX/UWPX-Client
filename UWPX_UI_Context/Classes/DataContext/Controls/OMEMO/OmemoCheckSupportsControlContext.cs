using System.Threading.Tasks;
using Shared.Classes;
using Storage.Classes.Models.Chat;
using UWPX_UI_Context.Classes.DataTemplates.Controls.OMEMO;
using XMPP_API.Classes.Network.XML.Messages;
using XMPP_API.Classes.Network.XML.Messages.Helper;
using XMPP_API.Classes.Network.XML.Messages.XEP_0384;

namespace UWPX_UI_Context.Classes.DataContext.Controls.OMEMO
{
    public class OmemoCheckSupportsControlContext
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        public readonly OmemoCheckSupportsControlDataTemplate MODEL = new OmemoCheckSupportsControlDataTemplate();

        private Task<OmemoSupportedStatus> updateTask;

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--


        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--


        #endregion
        //--------------------------------------------------------Misc Methods:---------------------------------------------------------------\\
        #region --Misc Methods (Public)--
        public void Refresh()
        {
            Task.Run(async () => await RefreshAsync());
        }

        #endregion

        #region --Misc Methods (Private)--
        private async Task RefreshAsync()
        {
            MODEL.Status = OmemoSupportedStatus.CHECKING;

            // Wait for the old update to finish first:
            if (!(updateTask is null) && !updateTask.IsCompleted)
            {
                MODEL.Status = await updateTask.ConfAwaitFalse();
            }

            updateTask = Task.Run(async () =>
            {
                if (MODEL.Chat is null || MODEL.Chat.Chat.chatType != ChatType.CHAT)
                {
                    return OmemoSupportedStatus.UNKNOWN;
                }

                if (!MODEL.Chat.Client.xmppClient.isConnected())
                {
                    if (MODEL.Chat.Chat.omemoInfo.devices.Count > 0)
                    {
                        return OmemoSupportedStatus.SUPPORTED;
                    }
                    MODEL.ErrorText = "Failed to check if your contact supports OMEMO.\nAccount not connected.";
                    return OmemoSupportedStatus.ERROR;
                }

                // Current OMEMO:
                MessageResponseHelperResult<IQMessage> result = await MODEL.Chat.Client.xmppClient.OMEMO_COMMAND_HELPER.requestDeviceListAsync(MODEL.Chat.Chat.bareJid);
                if (result.STATE == MessageResponseHelperResultState.SUCCESS)
                {
                    if (result.RESULT is OmemoDeviceListResultMessage deviceListResultMessage)
                    {
                        if (deviceListResultMessage.DEVICES.DEVICES.Count > 0)
                        {
                            return OmemoSupportedStatus.SUPPORTED;
                        }
                        return OmemoSupportedStatus.UNSUPPORTED;
                    }
                }
                else
                {
                    MODEL.ErrorText = "Requesting devices failed.";
                    return OmemoSupportedStatus.ERROR;
                }

                // OMEMO <= 0.3.0:
                result = await MODEL.Chat.Client.xmppClient.PUB_SUB_COMMAND_HELPER.requestNodeAsync(MODEL.Chat.Chat.bareJid, "eu.siacs.conversations.axolotl.devicelist", 1);
                if (result.STATE == MessageResponseHelperResultState.SUCCESS)
                {
                    if (result.RESULT is not IQErrorMessage)
                    {
                        return OmemoSupportedStatus.OLD_VERSION;
                    }
                }
                else
                {
                    MODEL.ErrorText = "Requesting devices failed.";
                    return OmemoSupportedStatus.ERROR;
                }
                return OmemoSupportedStatus.UNSUPPORTED;
            });
            MODEL.Status = await updateTask.ConfAwaitFalse();
            updateTask = null;
        }

        #endregion

        #region --Misc Methods (Protected)--


        #endregion
        //--------------------------------------------------------Events:---------------------------------------------------------------------\\
        #region --Events--


        #endregion
    }
}
