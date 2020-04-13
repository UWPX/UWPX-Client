using System.Xml.Linq;
using XMPP_API.Classes.Network.XML.Messages.XEP_0004;
using XMPP_API.Classes.Network.XML.Messages.XEP_0060;

namespace XMPP_API.Classes.Network.XML.Messages.XEP_0357
{
    public class EnablePushNotificationsMessage: IQMessage
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        public readonly string PUSH_SERVER_BARE_JID;
        public readonly string NODE;
        public readonly string SECRET;

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        public EnablePushNotificationsMessage(string pushServerBareJid, string node, string secret) : base(null, null, SET, getRandomId())
        {
            PUSH_SERVER_BARE_JID = pushServerBareJid;
            NODE = node;
            SECRET = secret;
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
            XElement n = new XElement(ns + "enable");
            n.Add(new XAttribute("jid", PUSH_SERVER_BARE_JID));
            n.Add(new XAttribute("node", NODE));

            PubSubPublishOptions options = PubSubPublishOptions.getDefaultPublishOptions();
            options.OPTIONS.fields.Add(new Field() { var = "secret", value = SECRET });
            options.OPTIONS.addToXElement(n);
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
