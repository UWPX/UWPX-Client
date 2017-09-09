namespace XMPP_API.Classes.Network.XML.Messages.Features
{
    class StreamFeature
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        private readonly string NAME;
        private readonly bool REQUIRED;

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Construktoren--
        /// <summary>
        /// Basic Constructor
        /// </summary>
        /// <history>
        /// 20/08/2017 Created [Fabian Sauter]
        /// </history>
        public StreamFeature(string name, bool required)
        {
            this.NAME = name;
            this.REQUIRED = required;
        }

        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--
        public bool isRequired()
        {
            return REQUIRED;
        }

        public string getName()
        {
            return NAME;
        }

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
