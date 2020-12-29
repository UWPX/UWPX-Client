using System.Xml;
using System.Xml.Linq;

namespace XMPP_API.Classes.Network.XML.Messages.XEP_0384
{
    public class OmemoDevice: IXElementable
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        public readonly uint id;
        public readonly string label;
        public readonly bool isValid = false;

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        public OmemoDevice(XmlNode node)
        {
            if (uint.TryParse(node.Attributes["id"].Value, out id) && id != 0)
            {
                isValid = true;
            }
            label = node.Attributes["label"]?.Value;
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
            node.Add(new XAttribute("id", id));
            node.Add(new XAttribute("label", label));
            return node;
        }

        public override int GetHashCode()
        {
            return id.GetHashCode();
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
