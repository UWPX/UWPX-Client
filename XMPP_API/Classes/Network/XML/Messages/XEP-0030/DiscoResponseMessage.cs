using System.Collections.Generic;
using System.Xml;
using System.Xml.Linq;

namespace XMPP_API.Classes.Network.XML.Messages.XEP_0030
{
    public class DiscoResponseMessage : IQMessage
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        public readonly List<DiscoIdentity> IDENTITIES;
        public readonly List<DiscoFeature> FEATURES;
        public readonly List<DiscoItem> ITEMS;
        public readonly StreamErrorMessage ERROR_RESULT;
        public readonly DiscoType DISCO_TYPE;
        public readonly bool isPartialList;

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        /// <summary>
        /// Basic Constructor
        /// </summary>
        /// <history>
        /// 10/11/2017 Created [Fabian Sauter]
        /// </history>
        public DiscoResponseMessage(XmlNode n) : base(n)
        {
            IDENTITIES = new List<DiscoIdentity>();
            FEATURES = new List<DiscoFeature>();
            ITEMS = new List<DiscoItem>();
            ERROR_RESULT = null;
            XmlNode qNode = XMLUtils.getChildNode(n, "query", Consts.XML_XMLNS, Consts.XML_XEP_0030_INFO_NAMESPACE);
            if (qNode == null)
            {
                qNode = XMLUtils.getChildNode(n, "query", Consts.XML_XMLNS, Consts.XML_XEP_0030_ITEMS_NAMESPACE);
                if (qNode == null)
                {
                    Logging.Logger.Warn("Invalid disco result message received! " + n.ToString().Replace('\n', ' '));
                    DISCO_TYPE = DiscoType.UNKNOWN;
                    return;
                }
                DISCO_TYPE = DiscoType.ITEMS;
            }
            else
            {
                DISCO_TYPE = DiscoType.INFO;
            }

            // Load content:
            if (qNode != null)
            {
                foreach (XmlNode n1 in qNode.ChildNodes)
                {
                    switch (n1.Name)
                    {
                        case "feature":
                            FEATURES.Add(new DiscoFeature(n1));
                            break;
                        case "identity":
                            IDENTITIES.Add(new DiscoIdentity(n1));
                            break;
                        case "item":
                            ITEMS.Add(new DiscoItem(n1));
                            break;
                        case "error":
                            ERROR_RESULT = new StreamErrorMessage(n1);
                            break;
                    }
                }
                isPartialList = doesNodeContainPartialList(qNode);
            }

            // Sort disco items alphabetically:
            ITEMS.Sort((a, b) =>
            {
                if (a == null || a.NAME == null)
                {
                    if (b == null || b.NAME == null)
                    {
                        return 0;
                    }
                    return -1;
                }

                if (b == null || b.NAME == null)
                {
                    return 1;
                }

                return a.NAME.CompareTo(b.NAME);
            });

        }

        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--
        protected override XElement getQuery()
        {
            throw new System.NotImplementedException();
        }

        #endregion
        //--------------------------------------------------------Misc Methods:---------------------------------------------------------------\\
        #region --Misc Methods (Public)--


        #endregion

        #region --Misc Methods (Private)--


        #endregion

        #region --Misc Methods (Protected)--
        protected bool doesNodeContainPartialList(XmlNode n)
        {
            return XMLUtils.getChildNode(n, "set", Consts.XML_XMLNS, "http://jabber.org/protocol/rsm") != null;
        }

        #endregion
        //--------------------------------------------------------Events:---------------------------------------------------------------------\\
        #region --Events--


        #endregion
    }
}
