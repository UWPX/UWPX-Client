using System.Collections.Generic;
using System.Xml;
using XMPP_API.Classes.Network.XML.Messages.XEP_0030;

namespace XMPP_API.Classes.Network.XML.Messages.XEP_0045
{
    public class ExtendedDiscoResponseMessage : DiscoResponseMessage
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        public readonly List<DiscoField> FIELDS;

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        /// <summary>
        /// Basic Constructor
        /// </summary>
        /// <history>
        /// 05/01/2018 Created [Fabian Sauter]
        /// </history>
        public ExtendedDiscoResponseMessage(XmlNode n) : base(n)
        {
            this.FIELDS = new List<DiscoField>();

            XmlNode qNode = XMLUtils.getChildNode(n, "query", "xmlns", "http://jabber.org/protocol/disco#info");
            if (qNode != null)
            {
                XmlNode xNode = XMLUtils.getChildNode(qNode, "x", "xmlns", "jabber:x:data");
                if(xNode != null)
                {
                    foreach (XmlNode n1 in xNode.ChildNodes)
                    {
                        switch (n1.Name)
                        {
                            case "field":
                                FIELDS.Add(new DiscoField(n1));
                                break;
                        }
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
