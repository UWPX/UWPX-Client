using System.Xml;

namespace XMPP_API.Classes.Network.XML.Messages.XEP_0045.Configuration
{
    public abstract class AbstractConfigrurationOption
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        public string type;
        public string var;
        public string label;
        public object value;

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        /// <summary>
        /// Basic Constructor
        /// </summary>
        /// <history>
        /// 07/02/2018 Created [Fabian Sauter]
        /// </history>
        public AbstractConfigrurationOption(XmlNode node)
        {
            this.type = node.Attributes["type"]?.Value;
            this.var = node.Attributes["var"]?.Value;
            this.label = node.Attributes["label"]?.Value;

            XmlNode value = XMLUtils.getChildNode(node, "value");
            if (value != null)
            {
                switch (type)
                {
                    case "boolean":
                        this.value = XMLUtils.tryParseToBool(value.InnerText);
                        break;

                    case "text-private":
                    case "text-single":
                    default:
                        this.value = value.InnerText;
                        break;
                }
            }
            this.value = value == null ? null : value.InnerText;
        }

        public AbstractConfigrurationOption()
        {
            this.type = null;
            this.var = null;
            this.label = null;
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
