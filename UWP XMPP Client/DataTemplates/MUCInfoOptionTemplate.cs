using System.ComponentModel;
using System.Runtime.CompilerServices;
using XMPP_API.Classes.Network.XML.Messages.XEP_0045.Configuration;

namespace UWP_XMPP_Client.DataTemplates
{
    class MUCInfoOptionTemplate : INotifyPropertyChanged
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        private MUCInfoField _option;
        public MUCInfoField option
        {
            get
            {
                return _option;
            }
            set
            {
                if (value != _option)
                {
                    _option = value;
                    onPropertyChanged("option");
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
        /// 07/02/2018 Created [Fabian Sauter]
        /// </history>
        public MUCInfoOptionTemplate()
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
