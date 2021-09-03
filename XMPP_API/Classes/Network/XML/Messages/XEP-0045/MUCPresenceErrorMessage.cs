using System.Xml;
using System.Xml.Linq;

namespace XMPP_API.Classes.Network.XML.Messages.XEP_0045
{
    public class MUCPresenceErrorMessage: PresenceMessage
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        public readonly Error ERROR;

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        public MUCPresenceErrorMessage(string from, string to, Error error) : base(from, to, "error")
        {
            ERROR = error;
        }

        public MUCPresenceErrorMessage(XmlNode node) : base(node)
        {
            XmlNode errorNode = XMLUtils.getChildNode(node, "error");
            if (errorNode is null)
            {
                ERROR = new Error();
            }
            else
            {
                ERROR = new Error(errorNode);
            }
        }

        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--


        #endregion
        //--------------------------------------------------------Misc Methods:---------------------------------------------------------------\\
        #region --Misc Methods (Public)--
        public override XElement toXElement()
        {
            XElement node = base.toXElement();
            node.Add(ERROR.toXElement(XNamespace.None));
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
