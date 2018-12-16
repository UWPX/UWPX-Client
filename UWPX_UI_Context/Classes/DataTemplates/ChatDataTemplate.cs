using Data_Manager2.Classes.DBTables;
using Windows.UI.Xaml.Media.Imaging;
using XMPP_API.Classes;

namespace UWPX_UI_Context.Classes.DataTemplates
{
    public sealed class ChatDataTemplate : AbstractNotifyPropertyChanged
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        private BitmapImage _image;
        public BitmapImage Image
        {
            get
            {
                return _image;
            }
            set
            {
                _image = value;
                OnPropertyChanged();
            }
        }
        private ChatTable _chat;
        public ChatTable Chat
        {
            get
            {
                return _chat;
            }
            set
            {
                _chat = value;
                OnPropertyChanged();
            }
        }
        private MUCChatInfoTable _mucInfo;
        public MUCChatInfoTable MucInfo
        {
            get
            {
                return _mucInfo;
            }
            set
            {
                _mucInfo = value;
                OnPropertyChanged();
            }
        }
        private XMPPClient _client;
        public XMPPClient Client
        {
            get
            {
                return _client;
            }
            set
            {
                _client = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// The index in the chats list.
        /// </summary>
        public int Index { get; set; }


        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--


        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--


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
