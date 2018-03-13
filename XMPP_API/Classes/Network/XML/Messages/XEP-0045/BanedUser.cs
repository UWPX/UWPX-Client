using System.Xml;
using System.Xml.Linq;

namespace XMPP_API.Classes.Network.XML.Messages.XEP_0045
{
    public class BanedUser
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        public string jid;
        public string reason;
        public MUCAffiliation affiliation;

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        /// <summary>
        /// Basic Constructor
        /// </summary>
        /// <history>
        /// 12/03/2018 Created [Fabian Sauter]
        /// </history>
        public BanedUser(string jid, string reason, MUCAffiliation affiliation)
        {
            this.jid = jid;
            this.reason = reason;
            this.affiliation = affiliation;
        }

        public BanedUser(XmlNode n)
        {
            jid = n.Attributes["jid"]?.Value;
            affiliation = Utils.parseMUCAffiliation(n.Attributes["affiliation"]?.Value);
            XmlNode reasonNode = XMLUtils.getChildNode(n, "reason");
            reason = reasonNode?.InnerText;
        }

        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--


        #endregion
        //--------------------------------------------------------Misc Methods:---------------------------------------------------------------\\
        #region --Misc Methods (Public)--
        public void addToNode(XElement node, XNamespace ns)
        {
            XElement itemNode = new XElement(ns + "item");
            if (jid != null)
            {
                itemNode.Add(new XAttribute("jid", jid));
            }

            itemNode.Add(new XAttribute("affiliation", Utils.mucAffiliationToString(affiliation)));

            if (reason != null)
            {
                itemNode.Add(new XElement(ns + "reason") { Value = reason });
            }

            node.Add(itemNode);
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
