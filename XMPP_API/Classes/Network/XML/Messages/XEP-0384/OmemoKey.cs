using System;
using System.Xml;
using System.Xml.Linq;
using Omemo.Classes.Messages;

namespace XMPP_API.Classes.Network.XML.Messages.XEP_0384
{
    public class OmemoKey: IXElementable
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        public readonly uint DEVICE_ID;
        public readonly bool KEY_EXCHANGE = false;
        public readonly string BASE64_PAYLOAD;


        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        public OmemoKey(Tuple<uint, IOmemoMessage> msg)
        {
            DEVICE_ID = msg.Item1;
            KEY_EXCHANGE = msg.Item2 is OmemoKeyExchangeMessage;
            BASE64_PAYLOAD = Convert.ToBase64String(msg.Item2.ToByteArray());
        }

        public OmemoKey(XmlNode node)
        {
            BASE64_PAYLOAD = node.InnerText;
            KEY_EXCHANGE = XMLUtils.tryParseToBool(node.Attributes["kex"]?.Value);
            if (uint.TryParse(node.Attributes["rid"]?.Value, out uint rid))
            {
                DEVICE_ID = rid;
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
            XElement keyNode = new XElement(ns + "key", BASE64_PAYLOAD);
            keyNode.Add(new XAttribute("rid", DEVICE_ID));
            if (KEY_EXCHANGE)
            {
                keyNode.Add(new XAttribute("kex", KEY_EXCHANGE));
            }
            return keyNode;
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
