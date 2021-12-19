using System.ComponentModel;
using System.Threading.Tasks;
using Logging;
using Manager.Classes.Chat;
using Storage.Classes.Models.Chat;
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
    public class MucConfigurationControlContext
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        public readonly MucConfigurationControlDataTemplate MODEL = new MucConfigurationControlDataTemplate();

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--


        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--
        private void SetError(string errorMessage)
        {
            MODEL.ErrorMarkdownText = errorMessage;
        }

        #endregion
        //--------------------------------------------------------Misc Methods:---------------------------------------------------------------\\
        #region --Misc Methods (Public)--
        public void UpdateView(DependencyPropertyChangedEventArgs args)
        {
            if (args.OldValue is ChatDataTemplate oldChat)
            {
                if (oldChat.Chat.muc is not null)
                {
                    oldChat.Chat.muc.PropertyChanged -= OnMucPropertyChanged;
                }
            }

            ChatDataTemplate newChat = null;
            if (args.NewValue is ChatDataTemplate tmp)
            {
                newChat = tmp;
                if (newChat.Chat.muc is not null)
                {
                    newChat.Chat.muc.PropertyChanged += OnMucPropertyChanged;
                    OnMucChanged(newChat.Chat.muc);
                }
            }
            MODEL.chat = newChat;
            MODEL.IsAvailable = false;
        }

        public void Reload(ChatDataTemplate chat)
        {
            if (chat is null || chat.Chat.muc.affiliation != MUCAffiliation.OWNER)
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
                MessageResponseHelperResult<IQMessage> result = await chat.Client.xmppClient.MUC_COMMAND_HELPER.saveRoomConfigurationAsync(chat.Chat.bareJid, MODEL.Form.Form);
                if (result.STATE != MessageResponseHelperResultState.SUCCESS)
                {
                    SetError("Failed to save room configuration:\n**" + result.STATE + "**");
                    Logger.Warn("Failed to save the room configuration for '" + chat.Chat.bareJid + "': " + result.STATE);
                }
                else if (result.RESULT is IQErrorMessage errorMessage)
                {
                    SetError("Failed to save room configuration:\n**" + errorMessage + "**");
                    Logger.Warn("Failed to save the room configuration for '" + chat.Chat.bareJid + "': " + errorMessage);
                }
                else
                {
                    SetError("");
                    Logger.Info("Successfully saved the room configuration for '" + chat.Chat.bareJid + '\'');
                }
                MODEL.IsLoading = false;
            });
        }

        #endregion

        #region --Misc Methods (Private)--
        private void OnMucChanged(MucInfoModel muc)
        {
            if (muc is not null && MODEL.chat is not null && muc.state == MucState.ENTERD && muc.affiliation == MUCAffiliation.OWNER)
            {
                MODEL.IsAvailable = true;
                RequestConfiguartion(MODEL.chat);
                return;
            }
            MODEL.IsAvailable = false;
        }

        private void RequestConfiguartion(ChatDataTemplate chat)
        {
            Task.Run(async () =>
            {
                MODEL.IsLoading = true;
                MessageResponseHelperResult<IQMessage> result = await chat.Client.xmppClient.MUC_COMMAND_HELPER.requestRoomConfigurationAsync(chat.Chat.bareJid);

                if (result.STATE == MessageResponseHelperResultState.SUCCESS)
                {
                    if (result.RESULT is RoomConfigMessage configMessage)
                    {
                        MODEL.Form = new DataFormDataTemplate(configMessage.ROOM_CONFIG);
                        MODEL.IsLoading = false;
                        MODEL.Success = true;
                        SetError("");
                        return;
                    }
                    else if (result.RESULT is IQErrorMessage errorMessage)
                    {
                        SetError("Failed to request room configuration:\n**" + errorMessage + "**");
                        Logger.Warn("Failed to request the room configuration for '" + chat.Chat.bareJid + "': " + errorMessage);
                    }
                    else
                    {
                        SetError("Failed to request room configuration:\n**Unexpected response - " + result.RESULT?.GetType().ToString() + "**");
                        Logger.Warn("Failed to request the room configuration for '" + chat.Chat.bareJid + "': Unexpected response - " + result.RESULT?.GetType().ToString());
                    }
                }
                else
                {
                    SetError("Failed to request room configuration:\n**" + result.STATE + "**");
                    Logger.Warn("Failed to request the room configuration for '" + chat.Chat.bareJid + "': " + result.STATE);
                }
                MODEL.IsLoading = false;
                MODEL.Success = false;
            });
        }

        #endregion

        #region --Misc Methods (Protected)--


        #endregion
        //--------------------------------------------------------Events:---------------------------------------------------------------------\\
        #region --Events--
        private void OnMucPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (sender is MucInfoModel muc && (string.Equals(e.PropertyName, nameof(MucInfoModel.affiliation)) || string.Equals(e.PropertyName, nameof(MucInfoModel.state))))
            {
                OnMucChanged(muc);
            }
        }

        #endregion
    }
}
