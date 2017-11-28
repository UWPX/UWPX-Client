using System.Collections.Generic;
using System.Xml;

namespace XMPP_API.Classes.Network.XML.Messages.XEP_0030
{
    public class DiscoResponseMessage : IQMessage
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        public readonly List<DiscoIdentity> IDENTITIES;
        public readonly List<DiscoFeature> FEATURES;
        public readonly StreamError ERROR_RESULT;

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
            ERROR_RESULT = null;
            XmlNode qNode = XMLUtils.getChildNode(n, "query", "xmlns", "http://jabber.org/protocol/disco#info");
            if (n != null && qNode != null)
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
                        case "error":
                            ERROR_RESULT = new StreamError(n1);
                            break;
                        default:
                            break;
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
