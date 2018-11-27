using System.Collections.Generic;
using System.Xml;
using System.Xml.Linq;

namespace XMPP_API.Classes.Network.XML.Messages.XEP_0004
{
    public class Field
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        public string var;
        public string label;
        public object value;
        public FieldType type;
        public List<FieldOption> options;
        public List<FieldOption> selectedOptions;

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        /// <summary>
        /// Basic Constructor
        /// </summary>
        /// <history>
        /// 02/06/2018 Created [Fabian Sauter]
        /// </history>
        public Field()
        {
            this.options = new List<FieldOption>();
            this.selectedOptions = new List<FieldOption>();
            this.value = null;
            this.var = null;
            this.label = null;
        }

        public Field(XmlNode node)
        {
            this.var = node.Attributes["var"]?.Value;
            this.label = node.Attributes["label"]?.Value;

            switch (node.Attributes["type"]?.Value)
            {
                case "boolean":
                    this.value = XMLUtils.tryParseToBool(getValue(node));
                    this.type = FieldType.BOOLEAN;
                    break;

                case "text-private":
                    this.value = getValue(node);
                    this.type = FieldType.TEXT_PRIVATE;
                    break;

                case "text-single":
                    this.value = getValue(node);
                    this.type = FieldType.TEXT_SINGLE;
                    break;

                case "text-multi":
                    this.type = FieldType.TEXT_MULTI;
                    break;

                case "fixed":
                    this.value = getValue(node);
                    this.type = FieldType.FIXED;
                    break;

                case "list-single":
                    this.options = getOptions(node);
                    this.selectedOptions = getSelectedOptions(node, this.options);
                    this.type = FieldType.LIST_SINGLE;
                    break;

                case "list-multi":
                    this.options = getOptions(node);
                    this.selectedOptions = getSelectedOptions(node, this.options);
                    this.type = FieldType.LIST_MULTI;
                    break;

                case "hidden":
                    this.value = getValue(node);
                    this.type = FieldType.HIDDEN;
                    break;

                default:
                    this.value = getValue(node);
                    this.type = FieldType.NONE;
                    break;
            }
        }

        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--
        private string getValue(XmlNode node)
        {
            XmlNode value = XMLUtils.getChildNode(node, "value");
            return value?.InnerText;
        }

        private List<FieldOption> getOptions(XmlNode node)
        {
            List<FieldOption> list = new List<FieldOption>();

            foreach (XmlNode n in node.ChildNodes)
            {
                if (Equals(n.Name, "option"))
                {
                    list.Add(new FieldOption(n));
                }
            }

            return list;
        }

        private List<FieldOption> getSelectedOptions(XmlNode node, List<FieldOption> options)
        {
            List<FieldOption> list = new List<FieldOption>();

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
            if (type != FieldType.NONE)
            {
                fieldNode.Add(new XAttribute("type", type.ToString().ToLowerInvariant().Replace('_', '-')));
            }
            switch (type)
            {
                case FieldType.LIST_SINGLE:
                case FieldType.TEXT_SINGLE:
                case FieldType.TEXT_MULTI:
                case FieldType.TEXT_PRIVATE:
                    fieldNode.Add(new XElement(ns + "value")
                    {
                        Value = (value?.ToString()) ?? ""
                    });
                    break;

                case FieldType.LIST_MULTI:
                    foreach (FieldOption o in selectedOptions)
                    {
                        fieldNode.Add(o.toXElement(ns));
                    }
                    break;

                case FieldType.BOOLEAN:
                    fieldNode.Add(new XElement(ns + "value")
                    {
                        Value = (bool)value ? "1" : "0"
                    });
                    break;

                case FieldType.HIDDEN:
                case FieldType.NONE:
                    fieldNode.Add(new XElement(ns + "value")
                    {
                        Value = value is null ? "" : (string)value,
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
