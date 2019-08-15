using System.Xml;
using System.Xml.Linq;

namespace XMPP_API.Classes.Network.XML.Messages.XEP_0384
{
    public class OmemoKey : IXElementable
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        public readonly uint REMOTE_DEVICE_ID;
        public readonly bool IS_PRE_KEY;
        public readonly string BASE_64_KEY;

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        /// <summary>
        /// Basic Constructor
        /// </summary>
        /// <history>
        /// 12/08/2018 Created [Fabian Sauter]
        /// </history>
        public OmemoKey(uint remoteDeviceId, bool isPreKey, string base64Key)
        {
            REMOTE_DEVICE_ID = remoteDeviceId;
            IS_PRE_KEY = isPreKey;
            BASE_64_KEY = base64Key;
        }

        public OmemoKey(XmlNode node)
        {
            BASE_64_KEY = node.InnerText;
            XmlAttribute isPreKeyAtt = node.Attributes["prekey"];
            IS_PRE_KEY = isPreKeyAtt != null && XMLUtils.tryParseToBool(isPreKeyAtt.Value);
            uint.TryParse(node.Attributes["rid"].Value, out uint rid);
            REMOTE_DEVICE_ID = rid;
        }

        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--


        #endregion
        //--------------------------------------------------------Misc Methods:---------------------------------------------------------------\\
        #region --Misc Methods (Public)--
        public XElement toXElement(XNamespace ns)
        {
            XElement keyNode = new XElement(ns + "key")
            {
                Value = BASE_64_KEY
            };
            keyNode.Add(new XAttribute("rid", REMOTE_DEVICE_ID));
            if (IS_PRE_KEY)
            {
                keyNode.Add(new XAttribute("prekey", true));
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
