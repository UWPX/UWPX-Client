using System.Collections.Generic;
using System.Xml;
using System.Xml.Linq;

namespace XMPP_API.Classes.Network.XML.Messages.XEP_0045.Configuration
{
    public class RoomConfiguration
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        public List<MUCInfoField> options;

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        /// <summary>
        /// Basic Constructor
        /// </summary>
        /// <history>
        /// 07/02/2018 Created [Fabian Sauter]
        /// </history>
        public RoomConfiguration(XmlNode node)
        {
            this.options = new List<MUCInfoField>();
            if (node != null)
            {
                foreach (XmlNode n in node.ChildNodes)
                {
                    switch (n.Name)
                    {
                        case "field":
                            this.options.Add(new MUCInfoField(n));
                            break;

                        // Ignored for now:
                        case "title":
                        case "instructions":
                            break;

                        default:
                            Logging.Logger.Warn("Unknown MUC room config element '" + n.Name + "' received!");
                            break;
                    }
                }
            }
        }

        public RoomConfiguration(List<MUCInfoField> options)
        {
            this.options = options;
        }

        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--
        public MUCInfoField getField(string var)
        {
            return options.Find((f) => { return Equals(var, f.var); });
        }

        #endregion
        //--------------------------------------------------------Misc Methods:---------------------------------------------------------------\\
        #region --Misc Methods (Public)--
        public void addToXElement(XElement n, XNamespace ns)
        {
            XElement fieldNode = new XElement(ns + "field");
            fieldNode.Add(new XAttribute("var", "FORM_TYPE"));
            fieldNode.Add(new XElement(ns + "value")
            {
                Value = Consts.XML_XEP_0045_ROOM_CONFIG_VALUE
            });
            n.Add(fieldNode);

            foreach (MUCInfoField f in options)
            {
                switch (f.type)
                {
                    case MUCInfoFieldType.TEXT_SINGLE:
                    case MUCInfoFieldType.TEXT_MULTI:
                    case MUCInfoFieldType.TEXT_PRIVATE:
                    case MUCInfoFieldType.BOOLEAN:
                    case MUCInfoFieldType.LIST_SINGLE:
                    case MUCInfoFieldType.LIST_MULTI:
                        n.Add(f.toXElement(ns));
                        break;
                }
            }
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
