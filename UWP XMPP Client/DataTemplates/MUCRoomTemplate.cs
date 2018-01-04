using System.ComponentModel;
using UWP_XMPP_Client.Classes;

namespace UWP_XMPP_Client.DataTemplates
{
    class MUCRoomTemplate : INotifyPropertyChanged
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        private string _jid;
        public string jid
        {
            get
            {
                return _jid;
            }
            set
            {
                if (value != _jid)
                {
                    _jid = value;
                    onPropertyChanged("jid");
                }
            }
        }
        private string _name;
        public string name
        {
            get
            {
                return _name;
            }
            set
            {
                if (value != _name)
                {
                    _name = value;
                    onPropertyChanged("name");
                }
            }
        }
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
