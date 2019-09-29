using System.Collections.Generic;
using System.Xml;

namespace XMPP_API.Classes.Network.XML.Messages.XEP_0045
{
    public class MUCMemberPresenceMessage: PresenceMessage
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        public readonly List<MUCPresenceStatusCode> STATUS_CODES;
        public readonly string JID;
        public readonly string NICKNAME;
        public readonly string FROM_NICKNAME;
        public readonly MUCAffiliation AFFILIATION;
        public readonly MUCRole ROLE;
        public readonly string ERROR_MESSAGE;
        public readonly string ERROR_TYPE;

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        /// <summary>
        /// Basic Constructor
        /// </summary>
        /// <history>
        /// 07/01/2018 Created [Fabian Sauter]
        /// </history>
        public MUCMemberPresenceMessage(XmlNode node) : base(node)
        {
            FROM_NICKNAME = Utils.getJidResourcePart(FROM);
            STATUS_CODES = new List<MUCPresenceStatusCode>();
            XmlNode xNode = XMLUtils.getChildNode(node, "x", Consts.XML_XMLNS, Consts.XML_XEP_0045_NAMESPACE_USER);
            if (xNode != null)
            {
                foreach (XmlNode n in xNode.ChildNodes)
                {
                    switch (n.Name)
                    {
                        case "item":
                            AFFILIATION = Utils.parseMUCAffiliation(n.Attributes["affiliation"]?.Value);
                            ROLE = Utils.parseMUCRole(n.Attributes["role"]?.Value);
                            JID = n.Attributes["jid"]?.Value;
                            NICKNAME = n.Attributes["nick"]?.Value;
                            break;

                        case "status":
                            string s = n.Attributes["code"]?.Value;
                            if (s != null)
                            {
                                STATUS_CODES.Add(parseStatusCode(s));
                            }
                            break;
                    }
                }
            }

            XmlNode eNode = XMLUtils.getChildNode(node, "error");
            if (eNode != null)
            {
                ERROR_TYPE = eNode.Attributes["type"]?.Value;
                ERROR_MESSAGE = eNode.InnerText;
            }
        }

        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--


        #endregion
        //--------------------------------------------------------Misc Methods:---------------------------------------------------------------\\
        #region --Misc Methods (Public)--


        #endregion

        #region --Misc Methods (Private)--
        private MUCPresenceStatusCode parseStatusCode(string status)
        {
            int code = -1;
            int.TryParse(status, out code);

            try
            {
                return (MUCPresenceStatusCode)code;
            }
            catch (System.Exception)
            {
                Logging.Logger.Warn("Unknown MUC presence status code " + code + " received! Please report this!");
                return MUCPresenceStatusCode.UNKNOWN;
            }
        }

        #endregion

        #region --Misc Methods (Protected)--


        #endregion
        //--------------------------------------------------------Events:---------------------------------------------------------------------\\
        #region --Events--


        #endregion
    }
}
