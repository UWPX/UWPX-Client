using Data_Manager2.Classes.DBTables;
using Windows.UI.Xaml.Media.Imaging;
using XMPP_API.Classes;

namespace UWPX_UI_Context.Classes.DataTemplates
{
    public sealed class ChatDataTemplate : AbstractDataTemplate
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        private BitmapImage _Image;
        public BitmapImage Image
        {
            get { return _Image; }
            set { SetProperty(ref _Image, value); }
        }
        private ChatTable _Chat;
        public ChatTable Chat
        {
            get { return _Chat; }
            set { SetProperty(ref _Chat, value); }
        }
        private MUCChatInfoTable _MucInfo;
        public MUCChatInfoTable MucInfo
        {
            get { return _MucInfo; }
            set { SetProperty(ref _MucInfo, value); }
        }
        private XMPPClient _Client;
        public XMPPClient Client
        {
            get { return _Client; }
            set { SetProperty(ref _Client, value); }
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
