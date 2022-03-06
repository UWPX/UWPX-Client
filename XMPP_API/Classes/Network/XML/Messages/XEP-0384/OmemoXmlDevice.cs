using System.Diagnostics;
using System.Xml;
using System.Xml.Linq;
using Newtonsoft.Json.Linq;
using Omemo.Classes;

namespace XMPP_API.Classes.Network.XML.Messages.XEP_0384
{
    public class OmemoXmlDevice: IXElementable
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        public readonly uint ID;
        public string label;
        public readonly bool IS_VALID = false;

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        public OmemoXmlDevice(XmlNode node)
        {
            if (uint.TryParse(node.Attributes["id"].Value, out ID))
            {
                IS_VALID = CryptoUtils.IsValidDeviceId(ID);
            }
            label = node.Attributes["label"]?.Value;
        }

        public OmemoXmlDevice(uint id, string label)
        {
            if(id <= 0)
            {
                Debugger.Break();
            }
            ID = id;
            this.label = label;
            IS_VALID = CryptoUtils.IsValidDeviceId(id);
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
            if (!(label is null))
            {
                node.Add(new XAttribute("label", label));
            }
            return node;
        }

        public override int GetHashCode()
        {
            return ID.GetHashCode();
        }

        public override string ToString()
        {
            JObject json = new();
            json[nameof(IS_VALID)] = IS_VALID;
            json[nameof(label)] = label;
            json[nameof(ID)] = ID;
            return json.ToString();
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
