using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace UWP_XMPP_Client.DataTemplates
{
    class BackgroundImageTemplate : INotifyPropertyChanged
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        private string _imagePath;
        public string imagePath
        {
            get
            {
                return _imagePath;
            }
            set
            {
                if (value != _imagePath)
                {
                    _imagePath = value;
                    onPropertyChanged("imagePath");
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
        private bool _selected;
        public bool selected
        {
            get
            {
                return _selected;
            }
            set
            {
                if (value != _selected)
                {
                    _selected = value;
                    onPropertyChanged("selected");
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
        /// 29/01/2017 Created [Fabian Sauter]
        /// </history>
        public BackgroundImageTemplate()
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
        protected void onPropertyChanged([CallerMemberName] string name = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        #endregion
        //--------------------------------------------------------Events:---------------------------------------------------------------------\\
        #region --Events--


        #endregion
    }
}
