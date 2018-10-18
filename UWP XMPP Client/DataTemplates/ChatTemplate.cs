using Data_Manager2.Classes.DBTables;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Windows.UI.Xaml.Media.Imaging;
using XMPP_API.Classes;

namespace UWP_XMPP_Client.DataTemplates
{
    public class ChatTemplate : INotifyPropertyChanged
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
                _image = value;
                onPropertyChanged();
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
                _chat = value;
                onPropertyChanged();
            }
        }
        private MUCChatInfoTable _mucInfo;
        public MUCChatInfoTable mucInfo
        {
            get
            {
                return _mucInfo;
            }
            set
            {
                _mucInfo = value;
                onPropertyChanged();
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
                _client = value;
                onPropertyChanged();
            }
        }

        /// <summary>
        /// The index in the chats list.
        /// </summary>
        public int index;

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        /// <summary>
        /// Basic Constructor
        /// </summary>
        /// <history>
        /// 28/08/2017 Created [Fabian Sauter]
        /// </history>
        public ChatTemplate() : this(null, null, null, null)
        {
        }

        public ChatTemplate(XMPPClient client, ChatTable chat, BitmapImage image) : this(client, chat, null, image)
        {
        }

        public ChatTemplate(XMPPClient client, ChatTable chat, MUCChatInfoTable info, BitmapImage image)
        {
            this._client = client;
            this._chat = chat;
            this._mucInfo = mucInfo;
            this._image = image;
            this.index = -1;
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
