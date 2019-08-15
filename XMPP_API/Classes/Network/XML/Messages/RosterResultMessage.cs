using System.Collections;
using System.Xml;

namespace XMPP_API.Classes.Network.XML.Messages
{
    public class RosterResultMessage : IQMessage
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        public readonly ArrayList ITEMS;

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        /// <summary>
        /// Basic Constructor
        /// </summary>
        /// <history>
        /// 02/11/2018 Created [Fabian Sauter]
        /// </history>
        public RosterResultMessage(XmlNode n) : base(n)
        {
            XmlNode query = XMLUtils.getChildNode(n, "query", Consts.XML_XMLNS, Consts.XML_ROSTER_NAMESPACE);
            ITEMS = new ArrayList();
            if (query != null)
            {
                foreach (XmlNode n1 in query.ChildNodes)
                {
                    if (n1.Name.Equals("presence"))
                    {
                        ITEMS.Add(new PresenceMessage(n1));
                    }
                    else
                    {
                        ITEMS.Add(new RosterItem(n1));
                    }
                }
            }
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


        #endregion
    }
}
