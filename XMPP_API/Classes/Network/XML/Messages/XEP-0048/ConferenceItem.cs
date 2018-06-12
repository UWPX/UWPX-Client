using System.Xml;
using System.Xml.Linq;

namespace XMPP_API.Classes.Network.XML.Messages.XEP_0048
{
    public class ConferenceItem : IXElementable
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        public string name;
        public string jid;
        public bool minimize;
        public bool autoJoin;

        public string nick;
        public string password;

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        /// <summary>
        /// Basic Constructor
        /// </summary>
        /// <history>
        /// 08/06/2018 Created [Fabian Sauter]
        /// </history>
        public ConferenceItem(XmlNode node)
        {
            if (node != null)
            {
                name = node.Attributes["name"]?.Value;
                jid = node.Attributes["jid"]?.Value;
                minimize = XMLUtils.tryParseToBool(node.Attributes["minimize"]?.Value);
                autoJoin = XMLUtils.tryParseToBool(node.Attributes["autojoin"]?.Value);

                XmlNode nNode = XMLUtils.getChildNode(node, "nick");
                if (nNode != null)
                {
                    nick = nNode.InnerText;
                }

                XmlNode pNode = XMLUtils.getChildNode(node, "password");
                if (pNode != null)
                {
                    password = pNode.InnerText;
                }
            }
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
            confNode.Add(new XAttribute("minimize", minimize));
            if (jid != null)
            {
                confNode.Add(new XAttribute("jid", jid));
            }
            if (name != null)
            {
                confNode.Add(new XAttribute("name", name));
            }
            if (nick != null)
            {
                confNode.Add(new XElement(ns + "nick", nick));
            }
            if (password != null)
            {
                confNode.Add(new XElement(ns + "password", password));
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
