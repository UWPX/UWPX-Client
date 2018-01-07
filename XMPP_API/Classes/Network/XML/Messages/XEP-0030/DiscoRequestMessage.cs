using System.Xml.Linq;

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
        private static XElement getQuerryFromType(DiscoType type)
        {
            XNamespace ns;
            switch (type)
            {
                case DiscoType.ITEMS:
                    ns = "http://jabber.org/protocol/disco#items";
                    break;
                case DiscoType.INFO:
                    ns = "http://jabber.org/protocol/disco#info";
                    break;
                default:
                    Logging.Logger.Error("Unable to get disco query for type: " + type + ". Returning info query!");
                    ns = "http://jabber.org/protocol/disco#info";
                    break;
            }
            return new XElement(ns + "query");
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
