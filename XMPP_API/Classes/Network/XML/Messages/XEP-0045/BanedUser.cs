using System.Xml;

namespace XMPP_API.Classes.Network.XML.Messages.XEP_0045
{
    public class BanedUser
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        public string jid;
        public string reason;
        public MUCAffiliation affiliation;

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        /// <summary>
        /// Basic Constructor
        /// </summary>
        /// <history>
        /// 12/03/2018 Created [Fabian Sauter]
        /// </history>
        public BanedUser(string jid, string reason, MUCAffiliation affiliation)
        {
            this.jid = jid;
            this.reason = reason;
            this.affiliation = affiliation;
        }

        public BanedUser(XmlNode n)
        {
            jid = n.Attributes["jid"]?.Value;
            affiliation = Utils.parseMUCAffiliation(n.Attributes["affiliation"]?.Value);
            XmlNode reasonNode = XMLUtils.getChildNode(n, "reason");
            reason = reasonNode?.InnerText;
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
