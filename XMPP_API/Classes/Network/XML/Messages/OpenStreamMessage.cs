using System.Xml.Linq;

namespace XMPP_API.Classes.Network.XML.Messages
{
    internal class OpenStreamMessage: AbstractAddressableMessage
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--


        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        /// <summary>
        /// Basic Constructor
        /// </summary>
        /// <history>
        /// 29/01/2017 Created [Fabian Sauter]
        /// </history>
        public OpenStreamMessage(string from, string to) : base(from, to)
        {
        }

        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--


        #endregion
        //--------------------------------------------------------Misc Methods:---------------------------------------------------------------\\
        #region --Misc Methods (Public)--
        public override string toXmlString()
        {
            string s = Consts.XML_HEADER + Consts.XML_STREAM_START;
            s += " " + new XAttribute("from", FROM).ToString();
            s += " " + new XAttribute("to", TO).ToString();
            s += " " + new XAttribute("version", Consts.XML_VERSION).ToString();
            s += " " + new XAttribute(XNamespace.Xml + "lang", Consts.XML_LANG).ToString();
            s += " " + Consts.XML_XMLNS + "=\"" + Consts.XML_CLIENT + '\"';
            s += " " + Consts.XML_STREAM_NAMESPACE + '>';
            return s;
        }

        /// <summary>
        /// Not used in OpenStreamMessage.
        /// Use toXmlString() instead.
        /// </summary>
        public override XElement toXElement()
        {
            throw new System.NotImplementedException();
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
