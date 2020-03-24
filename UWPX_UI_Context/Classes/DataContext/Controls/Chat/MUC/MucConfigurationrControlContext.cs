using System.Threading.Tasks;
using Logging;
using UWPX_UI_Context.Classes.DataTemplates;
using UWPX_UI_Context.Classes.DataTemplates.Controls.Chat.MUC;
using UWPX_UI_Context.Classes.DataTemplates.Controls.IoT;
using Windows.UI.Xaml;
using XMPP_API.Classes.Network.XML.Messages;
using XMPP_API.Classes.Network.XML.Messages.Helper;
using XMPP_API.Classes.Network.XML.Messages.XEP_0004;
using XMPP_API.Classes.Network.XML.Messages.XEP_0045;
using XMPP_API.Classes.Network.XML.Messages.XEP_0045.Configuration;

namespace UWPX_UI_Context.Classes.DataContext.Controls.Chat.MUC
{
    public class MucConfigurationrControlContext
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        public readonly MucConfigurationrControlDataTemplate MODEL = new MucConfigurationrControlDataTemplate();

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
            if (!(e.NewValue is ChatDataTemplate chat) || chat.MucInfo is null || chat.MucInfo.affiliation != MUCAffiliation.OWNER)
            {
                MODEL.IsAvailable = false;
                return;
            }
            MODEL.IsAvailable = true;
            RequestConfiguartion(chat);
        }

        public void Reload(ChatDataTemplate chat)
        {
            if (chat.MucInfo is null || chat.MucInfo.affiliation != MUCAffiliation.OWNER)
            {
                MODEL.IsAvailable = false;
                return;
            }
            MODEL.IsAvailable = true;
            RequestConfiguartion(chat);
        }

        public void Save(ChatDataTemplate chat)
        {
            Task.Run(async () =>
            {
                MODEL.IsLoading = true;
                MODEL.Form.Form.type = DataFormType.SUBMIT;
                MessageResponseHelperResult<IQMessage> result = await chat.Client.MUC_COMMAND_HELPER.saveRoomConfigurationAsync(chat.Chat.chatJabberId, MODEL.Form.Form);
                if (result.STATE != MessageResponseHelperResultState.SUCCESS)
                {
                    Logger.Warn("Failed to save the room configuration for '" + chat.Chat.chatJabberId + "': " + result.STATE);
                }
                else if (result.RESULT is IQErrorMessage errorMessage)
                {
                    Logger.Warn("Failed to save the room configuration for '" + chat.Chat.chatJabberId + "': " + errorMessage.ToString());
                }
                else
                {
                    Logger.Info("Successfully saved the room configuration for '" + chat.Chat.chatJabberId + '\'');
                }
                MODEL.IsLoading = false;
            });
        }

        #endregion

        #region --Misc Methods (Private)--
        private void RequestConfiguartion(ChatDataTemplate chat)
        {
            Task.Run(async () =>
            {
                MODEL.Success = false;
                MODEL.IsLoading = true;
                MessageResponseHelperResult<IQMessage> result = await chat.Client.MUC_COMMAND_HELPER.requestRoomConfigurationAsync(chat.Chat.chatJabberId);

                if (result.STATE == MessageResponseHelperResultState.SUCCESS)
                {
                    if (result.RESULT is RoomConfigMessage configMessage)
                    {
                        MODEL.Form = new DataFormDataTemplate(configMessage.ROOM_CONFIG);
                        MODEL.Success = true;
                        MODEL.IsLoading = false;
                        return;
                    }
                    else if (result.RESULT is IQErrorMessage errorMessage)
                    {
                        Logger.Warn("Failed to request the room configuration for '" + chat.Chat.chatJabberId + "': " + errorMessage.ToString());
                    }
                    else
                    {
                        Logger.Warn("Failed to request the room configuration for '" + chat.Chat.chatJabberId + "': Unexpected response - " + result.RESULT?.GetType().ToString());
                    }
                }
                else
                {
                    Logger.Warn("Failed to request the room configuration for '" + chat.Chat.chatJabberId + "': " + result.STATE);
                }
                MODEL.Success = false;
                MODEL.IsLoading = false;
            });
        }

        #endregion

        #region --Misc Methods (Protected)--


        #endregion
        //--------------------------------------------------------Events:---------------------------------------------------------------------\\
        #region --Events--


        #endregion
    }
}
