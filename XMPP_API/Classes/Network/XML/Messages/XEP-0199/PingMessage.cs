using System.Xml;
using System.Xml.Linq;

namespace XMPP_API.Classes.Network.XML.Messages.XEP_0199
{
    public class PingMessage: IQMessage
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--


        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        public PingMessage(XmlNode answer) : base(answer) { }

        public PingMessage(string from, string to) : base(from, to, GET, getRandomId()) { }

        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--


        #endregion
        //--------------------------------------------------------Misc Methods:---------------------------------------------------------------\\
        #region --Misc Methods (Public)--
        public override XElement toXElement()
        {
            XElement iq = base.toXElement();
            XNamespace ns = Consts.XML_XEP_0199_NAMESPACE;
            iq.Add(new XElement(ns + "ping"));
            return iq;
        }

        public PongMessage generateResponse()
        {
            return new PongMessage(getTo(), getFrom(), ID);
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
