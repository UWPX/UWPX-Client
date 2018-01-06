namespace XMPP_API.Classes.Network.XML.Messages.XEP_0030
{
    public class DiscoRequestMessage : IQMessage
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--


        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        /// <summary>
        /// Basic Constructor
        /// </summary>
        /// <history>
        /// 10/11/2017 Created [Fabian Sauter]
        /// </history>
        public DiscoRequestMessage(string from, string to, DiscoType type) : base(from, to, GET, getRandomId(), getQuerryFromType(type))
        {
        }

        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--
        private static string getQuerryFromType(DiscoType type)
        {
            switch (type)
            {
                case DiscoType.ITEMS:
                    return "<query xmlns='http://jabber.org/protocol/disco#items'/>";
                case DiscoType.INFO:
                    return "<query xmlns='http://jabber.org/protocol/disco#info'/>";
                default:
                    Logging.Logger.Error("Unable to get disco query for type: " + type + ". Returning info query!");
                    return "<query xmlns='http://jabber.org/protocol/disco#info'/>";
            }
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
