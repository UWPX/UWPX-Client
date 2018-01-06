using System.Xml.Linq;

namespace XMPP_API.Classes.Network.XML.Messages
{
    class SelectSASLMechanismMessage : AbstractMessage
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        private readonly string MECHANISM;
        private readonly string VALUE;

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
            this.MECHANISM = mechanism;
            this.VALUE = value;
        }

        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--


        #endregion
        //--------------------------------------------------------Misc Methods:---------------------------------------------------------------\\
        #region --Misc Methods (Public)--
        public override XElement toXElement()
        {
            XNamespace ns = "urn:ietf:params:xml:ns:xmpp-sasl";
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
