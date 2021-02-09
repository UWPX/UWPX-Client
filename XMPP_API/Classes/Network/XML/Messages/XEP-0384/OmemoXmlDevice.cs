using System.Xml;
using System.Xml.Linq;

namespace XMPP_API.Classes.Network.XML.Messages.XEP_0384
{
    public class OmemoXmlDevice: IXElementable
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        public readonly uint ID;
        public readonly string LABEL;
        public readonly bool IS_VALID = false;

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        public OmemoXmlDevice(XmlNode node)
        {
            if (uint.TryParse(node.Attributes["id"].Value, out ID) && ID != 0)
            {
                IS_VALID = true;
            }
            LABEL = node.Attributes["label"]?.Value;
        }

        public OmemoXmlDevice(uint id, string label)
        {
            ID = id;
            LABEL = label;
            IS_VALID = true;
        }

        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--


        #endregion
        //--------------------------------------------------------Misc Methods:---------------------------------------------------------------\\
        #region --Misc Methods (Public)--
        public XElement toXElement(XNamespace ns)
        {
            XElement node = new XElement(ns + "device");
            node.Add(new XAttribute("id", ID));
            if (!(LABEL is null))
            {
                node.Add(new XAttribute("label", LABEL));
            }
            return node;
        }

        public override int GetHashCode()
        {
            return ID.GetHashCode();
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
