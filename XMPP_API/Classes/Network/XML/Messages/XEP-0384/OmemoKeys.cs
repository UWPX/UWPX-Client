using System.Collections.Generic;
using System.Xml;
using System.Xml.Linq;

namespace XMPP_API.Classes.Network.XML.Messages.XEP_0384
{
    public class OmemoKeys: IXElementable
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        public readonly List<OmemoKey> KEYS = new List<OmemoKey>();
        public readonly string BARE_JID;

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        public OmemoKeys(List<OmemoKey> keys, string bareJid)
        {
            KEYS = keys;
            BARE_JID = bareJid;
        }

        public OmemoKeys(XmlNode node)
        {
            BARE_JID = node.Attributes["jid"]?.Value;
            foreach (XmlNode keyNode in node.ChildNodes)
            {
                if (string.Equals(keyNode.Name, "key"))
                {
                    KEYS.Add(new OmemoKey(keyNode));
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
            XElement keysNode = new XElement("keys");
            keysNode.Add(new XAttribute("jid", BARE_JID));
            foreach (OmemoKey key in KEYS)
            {
                keysNode.Add(key.toXElement(ns));
            }
            return keysNode;
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
