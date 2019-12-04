using System;
using System.Xml;

namespace XMPP_API.Classes.Network.XML.Messages.XEP_IoT
{
    public class IoTValue
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        public readonly string ID;
        public readonly string VALUE;
        public readonly string UNIT;
        public readonly IoTValueTypes TYPE;

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        public IoTValue(string id, XmlNode node)
        {
            ID = id;
            VALUE = node.InnerText;
            UNIT = node.Attributes["unit"]?.Value;
            // Fall back to a string value if parsing fails:
            TYPE = IoTValueTypes.STRING;
            Enum.TryParse(node.Attributes["type"]?.Value, out TYPE);
        }

        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--


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
