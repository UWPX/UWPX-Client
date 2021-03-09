using System.Threading.Tasks;
using Manager.Classes.Chat;
using Manager.Classes.Toast;
using Shared.Classes;
using Storage.Classes.Models.Chat;

namespace UWPX_UI_Context.Classes.DataTemplates.Controls
{
    public sealed class SpeechBubbleContentControlDataTemplate: AbstractDataTemplate
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        private ChatMessageDataTemplate _Message;
        public ChatMessageDataTemplate Message
        {
            get => _Message;
            set => SetMessageProperty(value);
        }

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--


        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--
        private void SetMessageProperty(ChatMessageDataTemplate value)
        {
            if (SetProperty(ref _Message, value, nameof(Message)) && !(value is null) && value.Message.state == MessageState.UNREAD)
            {
                value.Message.state = MessageState.READ;
                Task.Run(() =>
                {
                    value.Message.Update();
                    ToastHelper.UpdateBadgeNumber();
                });
            }
        }

        #endregion
        //--------------------------------------------------------Misc Methods:---------------------------------------------------------------\\
        #region --Misc Methods (Public)--


        #endregion

        #region --Misc Methods (Private)--


        #endregion

        #region --Misc Methods (Protected)--


        #endregion
        //--------------------------------------------------------Events:---------------------------------------------------------------------\\
        #region --Events--


        #endregion
    }
}
