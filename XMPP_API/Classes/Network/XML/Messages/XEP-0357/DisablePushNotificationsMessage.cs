using System.Xml.Linq;

namespace XMPP_API.Classes.Network.XML.Messages.XEP_0357
{
    public class DisablePushNotificationsMessage: IQMessage
    {
        #region --Attributes--
        public readonly string PUSH_SERVER_BARE_JID;

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        public DisablePushNotificationsMessage(string pushServerBareJid) : base(null, null, SET, getRandomId())
        {
            PUSH_SERVER_BARE_JID = pushServerBareJid;
        }

        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--


        #endregion
        //--------------------------------------------------------Misc Methods:---------------------------------------------------------------\\
        #region --Misc Methods (Public)--


        #endregion

        #region --Misc Methods (Private)--
        protected override XElement getQuery()
        {
            XNamespace ns = Consts.XML_XEP_0357_NAMESPACE;
            XElement n = new XElement(ns + "disable");
            n.Add(new XAttribute("jid", PUSH_SERVER_BARE_JID));
            return n;
        }

        #endregion

        #region --Misc Methods (Protected)--


        #endregion
        //--------------------------------------------------------Events:---------------------------------------------------------------------\\
        #region --Events--


        #endregion
    }
}
