using Data_Manager2.Classes;
using Data_Manager2.Classes.DBTables;
using System;

namespace UWPX_UI_Context.Classes.DataTemplates
{
    public sealed class SpeechBubbleContentDataTemplate : AbstractDataTemplate
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        private string _Text;
        public string Text
        {
            get { return _Text; }
            set
            {
                _Text = value;
                OnPropertyChanged();
            }
        }
        private string _NicknameText;
        public string NicknameText
        {
            get { return _NicknameText; }
            set
            {
                _NicknameText = value;
                OnPropertyChanged();
            }
        }
        private DateTime _Date;
        public DateTime Date
        {
            get { return _Date; }
            set
            {
                _Date = value;
                OnPropertyChanged();
            }
        }
        private bool _IsEncrypted;
        public bool IsEncrypted
        {
            get { return _IsEncrypted; }
            set
            {
                _IsEncrypted = value;
                OnPropertyChanged();
            }
        }
        private bool _IsCarbonCopy;
        public bool IsCarbonCopy
        {
            get { return _IsCarbonCopy; }
            set
            {
                _IsCarbonCopy = value;
                OnPropertyChanged();
            }
        }
        private string _StateIconText;
        public string StateIconText
        {
            get { return _StateIconText; }
            set
            {
                _StateIconText = value;
                OnPropertyChanged();
            }
        }
        private bool _IsDelivered;
        public bool IsDelivered
        {
            get { return _IsDelivered; }
            set
            {
                _IsDelivered = value;
                OnPropertyChanged();
            }
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
        public void UpdateView(ChatTable chat, ChatMessageTable message)
        {
            if (chat is null || message is null)
            {
                return;
            }

            Text = message.message;
            IsCarbonCopy = message.isCC;
            IsEncrypted = message.isEncrypted;
            Date = message.date;
            IsDelivered = false;

            switch (message.state)
            {
                case MessageState.SENDING:
                    StateIconText = "\uE724";
                    break;

                case MessageState.SEND:
                    StateIconText = "\uE725";
                    break;

                case MessageState.DELIVERED:
                    StateIconText = "\uE725";
                    IsDelivered = true;
                    break;

                case MessageState.UNREAD:
                    StateIconText = "\uEA63";
                    break;

                case MessageState.READ:
                    StateIconText = "\uEA64";
                    break;

                case MessageState.TO_ENCRYPT:
                    StateIconText = "\uE724";
                    break;

                case MessageState.ENCRYPT_FAILED:
                    StateIconText = "\uEA39";
                    break;

                default:
                    StateIconText = "\uE9CE";
                    break;
            }

            if (chat.chatType == ChatType.MUC)
            {
                NicknameText = message.fromUser;
            }
            else
            {
                NicknameText = string.Empty;
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
