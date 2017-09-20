using System.Xml;

namespace XMPP_API.Classes.Network.XML
{
    class XMLUtils
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--


        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--


        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--
        public static XmlNode getChildNode(XmlNode node, string name)
        {
            return getChildNode(node, name, null, null);
        }

        public static XmlNode getChildNode(XmlNode node, string name, string attribute, string attributeValue)
        {
            if (node.HasChildNodes)
            {
                foreach (XmlNode n in node.ChildNodes)
                {
                    if (n.Name.Equals(name))
                    {
                        if (attribute != null)
                        {
                            if (n.Attributes[attribute] != null && n.Attributes[attribute].Value.Equals(attributeValue))
                            {
                                return n;
                            }
                        }
                        else
                        {
                            return n;
                        }
                    }
                }
            }
            return null;
        }

        public static XmlAttribute getAttribute(XmlNode node, string name)
        {
            if (node.Attributes != null)
            {
                return node.Attributes[name];
            }
            return null;
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
