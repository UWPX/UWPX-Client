using System.ComponentModel;
using System.Runtime.CompilerServices;
using UWP_XMPP_Client.Classes;
using XMPP_API.Classes;

namespace UWP_XMPP_Client.DataTemplates
{
    class MUCRoomTemplate : INotifyPropertyChanged
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        private MUCRoomInfo _roomInfo;
        public MUCRoomInfo roomInfo
        {
            get
            {
                return _roomInfo;
            }
            set
            {
                if (value != _roomInfo)
                {
                    _roomInfo = value;
                    onPropertyChanged("roomInfo");
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
        /// 04/01/2018 Created [Fabian Sauter]
        /// </history>
        public MUCRoomTemplate()
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


        #endregion

        #region --Misc Methods (Protected)--


        #endregion
        //--------------------------------------------------------Events:---------------------------------------------------------------------\\
        #region --Events--
        private void onPropertyChanged([CallerMemberName] string name = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        #endregion
    }
}
