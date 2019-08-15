using System.Xml.Linq;

namespace XMPP_API.Classes.Network.XML.Messages.XEP_0030
{
    public class DiscoRequestMessage : IQMessage
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        public readonly DiscoType DISCO_TYPE;

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        /// <summary>
        /// Basic Constructor
        /// </summary>
        /// <history>
        /// 10/11/2017 Created [Fabian Sauter]
        /// </history>
        public DiscoRequestMessage(string from, string to, DiscoType discoType) : base(from, to, GET, getRandomId())
        {
            DISCO_TYPE = discoType;
        }

        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--
        protected override XElement getQuery()
        {
            XNamespace ns;
            switch (DISCO_TYPE)
            {
                case DiscoType.ITEMS:
                    ns = Consts.XML_XEP_0030_ITEMS_NAMESPACE;
                    break;
                case DiscoType.INFO:
                    ns = Consts.XML_XEP_0030_INFO_NAMESPACE;
                    break;
                default:
                    Logging.Logger.Error("Unable to get disco query for type: " + DISCO_TYPE + ". Returning info query!");
                    ns = Consts.XML_XEP_0030_INFO_NAMESPACE;
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
