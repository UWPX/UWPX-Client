using System.Xml;
using System.Xml.Linq;

namespace XMPP_API.Classes.Network.XML.Messages.XEP_0048
{
    public class ConferenceItem : IXElementable
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        public string name;
        public bool autoJoin;
        public string jid;
        public string nick;
        public string password;

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        /// <summary>
        /// Basic Constructor
        /// </summary>
        /// <history>
        /// 22/07/2018 Created [Fabian Sauter]
        /// </history>
        public ConferenceItem()
        {
        }

        public ConferenceItem(XmlNode node)
        {
            name = node.Attributes["name"]?.Value;
            autoJoin = XMLUtils.tryParseToBool(node.Attributes["autojoin"]?.Value);
            jid = node.Attributes["jid"]?.Value;

            XmlNode nNode = XMLUtils.getChildNode(node, "nick");
            nick = nNode != null ? nNode.InnerText : "";

            XmlNode pNode = XMLUtils.getChildNode(node, "password");
            password = pNode != null ? pNode.InnerText : "";
        }

        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--


        #endregion
        //--------------------------------------------------------Misc Methods:---------------------------------------------------------------\\
        #region --Misc Methods (Public)--
        public XElement toXElement(XNamespace ns)
        {
            XElement confNode = new XElement(ns + "conference");
            confNode.Add(new XAttribute("autojoin", autoJoin));
            if (!string.IsNullOrEmpty(name))
            {
                confNode.Add(new XAttribute("name", name));
            }
            if (jid != null)
            {
                confNode.Add(new XAttribute("jid", jid));
            }
            if (!string.IsNullOrEmpty(password))
            {
                confNode.Add(new XElement(ns + "password", password));
            }
            if (nick != null)
            {
                confNode.Add(new XElement(ns + "nick", nick));
            }
            return confNode;
        }

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
