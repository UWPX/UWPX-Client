using System;
using System.Xml;
using System.Xml.Linq;
using XMPP_API.Classes.Network.XML.Messages.XEP_0004;

namespace XMPP_API.Classes.Network.XML.Messages.XEP_IoT
{
    public class IoTValue: IXElementable
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        public readonly string ITEM_ID;
        public readonly string VALUE;
        public readonly string UNIT;
        public readonly IoTValueType TYPE;

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        public IoTValue(string item_id, XmlNode node)
        {
            ITEM_ID = item_id;
            VALUE = node.InnerText;
            UNIT = node.Attributes["unit"]?.Value;
            // Fall back to a string value if parsing fails:
            TYPE = IoTValueType.STRING;
            Enum.TryParse(node.Attributes["type"]?.Value, out TYPE);
        }

        public IoTValue(Field field)
        {
            VALUE = field.value.ToString();
            TYPE = IoTValueType.NONE;
            UNIT = null;
        }

        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--


        #endregion
        //--------------------------------------------------------Misc Methods:---------------------------------------------------------------\\
        #region --Misc Methods (Public)--
        public XElement toXElement(XNamespace ns)
        {
            XElement varNode = new XElement(ns + "var");
            if (!(UNIT is null))
            {
                varNode.Add(new XAttribute("unit", UNIT));
            }
            if (TYPE != IoTValueType.NONE)
            {
                varNode.Add(new XAttribute("type", TYPE.ToString().ToLowerInvariant()));
            }
            if (!(varNode is null))
            {
                varNode.Value = VALUE;
            }
            return varNode;
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
