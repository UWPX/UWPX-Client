using System.Xml;
using XMPP_API.Classes.Network.XML.Messages.XEP_0030;

namespace XMPP_API.Classes.Network.XML.Messages.XEP_0045
{
    public class DiscoField: IDiscoItem
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        public readonly string VAR;
        public readonly string LABEL;
        public readonly string VALUE;
        public readonly string TYPE;

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        /// <summary>
        /// Basic Constructor
        /// </summary>
        /// <history>
        /// 05/01/2018 Created [Fabian Sauter]
        /// </history>
        public DiscoField(XmlNode n)
        {
            if (n != null)
            {
                VAR = n.Attributes["var"]?.Value;
                LABEL = n.Attributes["label"]?.Value;
                TYPE = n.Attributes["type"]?.Value;

                XmlNode vNode = XMLUtils.getChildNode(n, "value");
                VALUE = vNode?.InnerText;
            }
        }

        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--
        public bool isHidden()
        {
            return Equals(TYPE, "hidden");
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
