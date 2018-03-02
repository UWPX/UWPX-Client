using System.Collections.Generic;
using System.Xml;
using System.Xml.Linq;

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
                    this.selectedOptions = getSelectedOptions(node, this.options);
                    this.type = MUCInfoFieldType.LIST_SINGLE;
                    break;

                case "list-multi":
                    this.options = getOptions(node);
                    this.selectedOptions = getSelectedOptions(node, this.options);
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

        private List<MUCInfoOption> getSelectedOptions(XmlNode node, List<MUCInfoOption> options)
        {
            List<MUCInfoOption> list = new List<MUCInfoOption>();

            foreach (XmlNode n in node.ChildNodes)
            {
                if (Equals(n.Name, "value"))
                {
                    for (int i = 0; i < options.Count; i++)
                    {
                        if (Equals(options[i].value, n.InnerText))
                        {
                            list.Add(options[i]);
                            break;
                        }
                    }
                }
            }

            return list;
        }

        #endregion
        //--------------------------------------------------------Misc Methods:---------------------------------------------------------------\\
        #region --Misc Methods (Public)--
        public XElement toXElement(XNamespace ns)
        {
            XElement fieldNode = new XElement(ns + "field");
            fieldNode.Add(new XAttribute("var", var));
            switch (type)
            {
                case MUCInfoFieldType.TEXT_SINGLE:
                case MUCInfoFieldType.TEXT_MULTI:
                case MUCInfoFieldType.TEXT_PRIVATE:
                    fieldNode.Add(new XElement(ns + "value")
                    {
                        Value = (value?.ToString()) ?? ""
                    });
                    break;

                case MUCInfoFieldType.LIST_SINGLE:
                case MUCInfoFieldType.LIST_MULTI:
                    foreach (MUCInfoOption o in selectedOptions)
                    {
                        fieldNode.Add(o.toXElement(ns));
                    }
                    break;

                case MUCInfoFieldType.BOOLEAN:
                    fieldNode.Add(new XElement(ns + "value")
                    {
                        Value = (bool)value ? "1" : "0"
                    });
                    break;
            }
            return fieldNode;
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
