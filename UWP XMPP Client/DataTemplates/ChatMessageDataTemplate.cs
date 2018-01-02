using Data_Manager2.Classes.DBTables;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace UWP_XMPP_Client.DataTemplates
{
    class ChatMessageDataTemplate : INotifyPropertyChanged
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        private ChatTable _chat;
        public ChatTable chat
        {
            get
            {
                return _chat;
            }
            set
            {
                if (value != _chat)
                {
                    _chat = value;
                    onPropertyChanged("chat");
                }
            }
        }
        private ChatMessageTable _message;
        public ChatMessageTable message
        {
            get
            {
                return _message;
            }
            set
            {
                if (value != _message)
                {
                    _message = value;
                    onPropertyChanged("message");
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged = delegate { };

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        /// <summary>
        /// Basic Constructor
        /// </summary>
        /// <history>
        /// 02/01/2018 Created [Fabian Sauter]
        /// </history>
        public ChatMessageDataTemplate()
        {

        }

        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--


        #endregion
        //--------------------------------------------------------Misc Methods:---------------------------------------------------------------\\
        #region --Misc Methods (Public)--


        #endregion

        #region --Misc Methods (Private)--
        private void onPropertyChanged([CallerMemberName] string name = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        #endregion

        #region --Misc Methods (Protected)--


        #endregion
        //--------------------------------------------------------Events:---------------------------------------------------------------------\\
        #region --Events--


        #endregion
    }
}
