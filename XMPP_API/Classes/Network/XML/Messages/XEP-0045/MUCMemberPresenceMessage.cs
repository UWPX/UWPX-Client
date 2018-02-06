using System.Collections.Generic;
using System.Xml;

namespace XMPP_API.Classes.Network.XML.Messages.XEP_0045
{
    public class MUCMemberPresenceMessage : PresenceMessage
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        public readonly List<MUCPresenceStatusCode> STATUS_CODES;
        public readonly string AFFILIATION;
        public readonly string JID;
        public readonly string NICKNAME;
        public readonly string ROLE;
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
            XmlNode xNode = XMLUtils.getChildNode(node, "x", "xmlns", "http://jabber.org/protocol/muc#user");
            if (xNode != null)
            {
                NICKNAME = Utils.getResourceFromFullJid(FROM);
                STATUS_CODES = new List<MUCPresenceStatusCode>();
                foreach (XmlNode n in xNode.ChildNodes)
                {
                    switch (n.Name)
                    {
                        case "item":
                            AFFILIATION = n.Attributes["affiliation"]?.Value;
                            ROLE = n.Attributes["role"]?.Value;
                            JID = n.Attributes["jid"]?.Value;
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
            if(eNode != null){
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

            switch (code)
            {
                case 100:
                    return MUCPresenceStatusCode.SEE_FULL_JID_ANYBODY;
                case 101:
                    return MUCPresenceStatusCode.AFFILIATION_CHANGED_WHILE_NOT_CONNECTED;
                case 102:
                    return MUCPresenceStatusCode.ROOM_SHOWS_UNAVAILABLE_MEMBERS;
                case 103:
                    return MUCPresenceStatusCode.PRESENCE_CHANGED_ROOMNICK;
                case 104:
                    return MUCPresenceStatusCode.ROOM_CONFIG_CHANGED_NON_PRIVACY_RELATED;
                case 110:
                    return MUCPresenceStatusCode.PRESENCE_SELFE_REFERENCE;
                case 170:
                    return MUCPresenceStatusCode.ROOM_LOGGING_ENABLED;
                case 171:
                    return MUCPresenceStatusCode.ROOM_LOGGING_DISABLED;
                case 172:
                    return MUCPresenceStatusCode.ROOM_NON_ANONYMOUS;
                case 173:
                    return MUCPresenceStatusCode.ROOM_SEMI_ANONYMOUS;
                case 174:
                    return MUCPresenceStatusCode.ROOM_FULLY_ANONYMOUS;
                case 201:
                    return MUCPresenceStatusCode.NEW_ROOM_CREATED;
                case 210:
                    return MUCPresenceStatusCode.MEMBER_NICK_CHANGED;
                case 301:
                    return MUCPresenceStatusCode.MEMBER_GOT_BANED;
                case 303:
                    return MUCPresenceStatusCode.ROOM_NICK_CHANGED;
                case 307:
                    return MUCPresenceStatusCode.MEMBER_GOT_KICKED;
                case 321:
                    return MUCPresenceStatusCode.MEMBER_GOT_REMOVED_AFFILIATION_CHANGED;
                case 322:
                    return MUCPresenceStatusCode.MEMBER_GOT_REMOVED_ROOM_CHANGED_TO_MEMBERS_ONLY;
                case 332:
                    return MUCPresenceStatusCode.MEMBER_GOT_REMOVED_SYSTEM_SHUTDOWN;
                default:
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
