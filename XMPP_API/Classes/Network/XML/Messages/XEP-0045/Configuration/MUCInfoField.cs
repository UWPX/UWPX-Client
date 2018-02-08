using System.Collections.Generic;
using System.Xml;

namespace XMPP_API.Classes.Network.XML.Messages.XEP_0045.Configuration
{
    public class MUCInfoField
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        public MUCInfoFieldType type;
        public string var;
        public string label;
        public object value;
        public List<MUCInfoOption> options;
        public List<MUCInfoOption> selectedOptions;

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        /// <summary>
        /// Basic Constructor
        /// </summary>
        /// <history>
        /// 08/02/2018 Created [Fabian Sauter]
        /// </history>
        public MUCInfoField(XmlNode node)
        {
            this.var = node.Attributes["var"]?.Value;
            this.label = node.Attributes["label"]?.Value;

            switch (node.Attributes["type"]?.Value)
            {
                case "boolean":
                    this.value = XMLUtils.tryParseToBool(getValue(node));
                    this.type = MUCInfoFieldType.BOOLEAN;
                    break;

                case "text-private":
                    this.value = getValue(node);
                    this.type = MUCInfoFieldType.TEXT_PRIVATE;
                    break;

                case "text-single":
                    this.value = getValue(node);
                    this.type = MUCInfoFieldType.TEXT_SINGLE;
                    break;

                case "text-multi":
                    this.type = MUCInfoFieldType.TEXT_MULTI;
                    break;

                case "fixed":
                    this.value = getValue(node);
                    this.type = MUCInfoFieldType.FIXED;
                    break;

                case "list-single":
                    this.options = getOptions(node);
                    this.selectedOptions = getSelectedOptions(node);
                    this.type = MUCInfoFieldType.LIST_SINGLE;
                    break;

                case "list-multi":
                    this.options = getOptions(node);
                    this.selectedOptions = getSelectedOptions(node);
                    this.type = MUCInfoFieldType.LIST_MULTI;
                    break;

                case "hidden":
                default:
                    this.value = getValue(node);
                    this.type = MUCInfoFieldType.HIDDEN;
                    break;
            }
        }

        public MUCInfoField()
        {
            this.type = MUCInfoFieldType.HIDDEN;
            this.var = null;
            this.label = null;
            this.options = null;
        }

        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--
        private string getValue(XmlNode node)
        {
            XmlNode value = XMLUtils.getChildNode(node, "value");
            return value?.InnerText;
        }

        private List<MUCInfoOption> getOptions(XmlNode node)
        {
            List<MUCInfoOption> list = new List<MUCInfoOption>();

            foreach (XmlNode n in node.ChildNodes)
            {
                if (Equals(n.Name, "option"))
                {
                    list.Add(new MUCInfoOption(n));
                }
            }

            return list;
        }

        private List<MUCInfoOption> getSelectedOptions(XmlNode node)
        {
            List<MUCInfoOption> list = new List<MUCInfoOption>();

            foreach (XmlNode n in node.ChildNodes)
            {
                if (Equals(n.Name, "value"))
                {
                    list.Add(new MUCInfoOption() { label = null, value = n.InnerText });
                }
            }

            return list;
        }

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
