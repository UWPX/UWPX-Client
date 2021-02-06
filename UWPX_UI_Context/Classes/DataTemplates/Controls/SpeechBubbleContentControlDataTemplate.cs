using System;
using System.Threading.Tasks;
using Manager.Classes.Toast;
using Shared.Classes;
using Storage.Classes.Contexts;
using Storage.Classes.Models.Chat;

namespace UWPX_UI_Context.Classes.DataTemplates.Controls
{
    public sealed class SpeechBubbleContentControlDataTemplate: AbstractDataTemplate
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        private string _Text;
        public string Text
        {
            get => _Text;
            set => SetProperty(ref _Text, value);
        }
        private string _NicknameText;
        public string NicknameText
        {
            get => _NicknameText;
            set => SetProperty(ref _NicknameText, value);
        }
        private DateTime _Date;
        public DateTime Date
        {
            get => _Date;
            set => SetProperty(ref _Date, value);
        }
        private bool _IsEncrypted;
        public bool IsEncrypted
        {
            get => _IsEncrypted;
            set => SetProperty(ref _IsEncrypted, value);
        }
        private bool _IsCarbonCopy;
        public bool IsCarbonCopy
        {
            get => _IsCarbonCopy;
            set => SetProperty(ref _IsCarbonCopy, value);
        }
        private string _MessageType;
        public string MessageType
        {
            get => _MessageType;
            set => SetProperty(ref _MessageType, value);
        }
        private bool _IsImage;
        public bool IsImage
        {
            get => _IsImage;
            set => SetProperty(ref _IsImage, value);
        }
        private MessageState _State;
        public MessageState State
        {
            get => _State;
            set => SetProperty(ref _State, value);
        }

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--


        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--


        #endregion
        //--------------------------------------------------------Misc Methods:---------------------------------------------------------------\\
        #region --Misc Methods (Public)--
        public void UpdateView(ChatModel chat, ChatMessageModel message)
        {
            if (chat is null || message is null)
            {
                return;
            }

            Text = message.message;
            IsCarbonCopy = message.isCC;
            IsEncrypted = message.isEncrypted;
            Date = message.date;
            MessageType = message.type;
            IsImage = message.isImage;
            State = message.state;
            NicknameText = chat.chatType == ChatType.MUC ? message.fromBareJid : string.Empty;

            if (State == MessageState.UNREAD)
            {
                // Mark message as read and update the badge notification count:
                Task.Run(() =>
                {
                    using (MainDbContext ctx = new MainDbContext())
                    {
                        ctx.Update(message);
                    }
                    ToastHelper.UpdateBadgeNumber();
                });
            }
        }

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
