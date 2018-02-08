using System.Xml;

namespace XMPP_API.Classes.Network.XML.Messages.XEP_0045.Configuration
{
    public class MUCInfoOption
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        public string label;
        public string value;

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        /// <summary>
        /// Basic Constructor
        /// </summary>
        /// <history>
        /// 08/02/2018 Created [Fabian Sauter]
        /// </history>
        public MUCInfoOption()
        {
            this.label = null;
            this.value = null;
        }

        public MUCInfoOption(XmlNode node)
        {
            this.label = node.Attributes["label"]?.Value;
            this.value = getValue(node);
        }

        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--
        private string getValue(XmlNode node)
        {
            XmlNode value = XMLUtils.getChildNode(node, "value");
            return value?.InnerText;
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
