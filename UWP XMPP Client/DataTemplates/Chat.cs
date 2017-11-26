using Data_Manager2.Classes.DBTables;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Windows.UI.Xaml.Media.Imaging;
using XMPP_API.Classes;

namespace UWP_XMPP_Client.DataTemplates
{
    public class Chat : INotifyPropertyChanged
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        private BitmapImage _image;
        public BitmapImage image
        {
            get
            {
                return _image;
            }
            set
            {
                if (value != _image)
                {
                    _image = value;
                    onPropertyChanged("image");
                }
            }
        }
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
        private XMPPClient _client;
        public XMPPClient client
        {
            get
            {
                return _client;
            }
            set
            {
                if (value != _client)
                {
                    _client = value;
                    onPropertyChanged("client");
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
        /// 28/08/2017 Created [Fabian Sauter]
        /// </history>
        public Chat()
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
        protected void onPropertyChanged([CallerMemberName] string name = "")
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
