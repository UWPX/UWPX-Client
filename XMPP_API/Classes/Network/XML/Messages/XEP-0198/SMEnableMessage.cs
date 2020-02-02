using System.Xml;
using System.Xml.Linq;

namespace XMPP_API.Classes.Network.XML.Messages.XEP_0198
{
    public class SMEnableMessage: AbstractMessage
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        public readonly string ID;
        public readonly bool RESUME;
        public readonly string LOCATION;
        public readonly uint MAX;

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        /// <summary>
        /// Basic Constructor
        /// </summary>
        /// <history>
        /// 06/05/2018 Created [Fabian Sauter]
        /// </history>
        public SMEnableMessage()
        {
            ID = null;
            RESUME = false;
            LOCATION = null;
            MAX = 0;
        }

        public SMEnableMessage(XmlNode node)
        {
            foreach (XAttribute att in node.Attributes)
            {
                switch (att.Name.ToString())
                {
                    case "id":
                        ID = att.Value;
                        break;

                    case "location":
                        LOCATION = att.Value;
                        break;

                    case "max":
                        if (uint.TryParse(att.Value, out MAX))
                        {
                            MAX = 0;
                        }
                        break;

                    case "resume":
                        RESUME = XMLUtils.tryParseToBool(att.Value);
                        break;

                    default:
                        break;
                }
            }
        }

        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--


        #endregion
        //--------------------------------------------------------Misc Methods:---------------------------------------------------------------\\
        #region --Misc Methods (Public)--
        public override XElement toXElement()
        {
            XNamespace ns = Consts.XML_XEP_0198_NAMESPACE;
            XElement node = new XElement(ns + "enable");
            if (ID != null)
            {
                node.Add(new XAttribute("id", ID));
            }
            if (LOCATION != null)
            {
                node.Add(new XAttribute("location", LOCATION));
            }
            if (MAX > 0)
            {
                node.Add(new XAttribute("max", MAX));
            }
            if (RESUME)
            {
                node.Add(new XAttribute("resume", RESUME.ToString().ToUpper()));
            }
            return node;
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
