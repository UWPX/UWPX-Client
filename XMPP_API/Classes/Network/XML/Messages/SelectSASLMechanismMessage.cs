using System.Xml.Linq;

namespace XMPP_API.Classes.Network.XML.Messages
{
    public class SelectSASLMechanismMessage : AbstractMessage
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        public readonly string MECHANISM;
        public readonly string VALUE;

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        /// <summary>
        /// Basic Constructor
        /// </summary>
        /// <history>
        /// 22/08/2017 Created [Fabian Sauter]
        /// </history>
        public SelectSASLMechanismMessage(string mechanism, string value)
        {
            MECHANISM = mechanism;
            VALUE = value;
        }

        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--


        #endregion
        //--------------------------------------------------------Misc Methods:---------------------------------------------------------------\\
        #region --Misc Methods (Public)--
        public override XElement toXElement()
        {
            XNamespace ns = Consts.XML_SASL_NAMESPACE;
            XElement node = new XElement(ns + "auth");
            node.Add(new XAttribute("mechanism", MECHANISM));
            node.Value = VALUE;
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
