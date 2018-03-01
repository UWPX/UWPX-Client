using System;
using System.Text.RegularExpressions;
using XMPP_API.Classes.Network.XML.Messages.XEP_0045;

namespace XMPP_API.Classes
{
    public class Utils
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        /// <summary>
        /// The regex for a valid jabber id.
        /// Source: https://www.codesd.com/item/what-is-the-regular-expression-for-the-validation-of-jabber-id.html [09.02.2018]
        /// </summary>
        private static string JID_REGEX_PATTERN = @"^\A([a-z0-9\.\-_\+]+)@((?:[-a-z0-9]+\.)+[a-z]{2,})\Z$";

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--


        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--
        /// <summary>
        /// Checks if the given JID is a bare JID.
        /// e.g. 'coven@chat.shakespeare.lit'
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static bool isBareJid(string s)
        {
            Regex regex = new Regex(JID_REGEX_PATTERN);
            return regex.IsMatch(s);
        }

        /// <summary>
        /// Returns the bare JID from the full JID.
        /// e.g. 'coven@chat.shakespeare.lit/thirdwitch' => 'coven@chat.shakespeare.lit'
        /// </summary>
        /// <param name="jid">The full JID. e.g. 'coven@chat.shakespeare.lit/thirdwitch'.</param>
        /// <returns>Returns the bare JID. e.g. 'coven@chat.shakespeare.lit'.</returns>
        public static string getBareJidFromFullJid(string jid)
        {
            if (jid == null)
            {
                return null;
            }
            if (jid.Contains("/"))
            {
                jid = jid.Substring(0, jid.IndexOf("/"));
            }
            return jid;
        }

        /// <summary>
        /// Returns the domain from the bare JID.
        /// e.g. 'coven@chat.shakespeare.lit' => 'chat.shakespeare.lit'
        /// </summary>
        /// <param name="jid">The bare JID. e.g. 'coven@chat.shakespeare.lit'</param>
        /// <returns>Returns the domain part of the bare JID. e.g. 'chat.shakespeare.lit'</returns>
        public static string getDomainFromBareJid(string jid)
        {
            if (jid != null && jid.Contains("@"))
            {
                return jid.Substring(jid.LastIndexOf('@') + 1);
            }
            return null;
        }

        /// <summary>
        /// Returns the user part from the bare JID.
        /// e.g. 'coven@chat.shakespeare.lit' => 'coven'
        /// </summary>
        /// <param name="jid">The bare JID. e.g. 'coven@chat.shakespeare.lit'</param>
        /// <returns>Returns the user part of the bare JID. e.g. 'coven'</returns>
        public static string getUserFromBareJid(string jid)
        {
            if (jid != null && jid.Contains("@"))
            {
                return jid.Substring(0, jid.LastIndexOf('@'));
            }
            return null;
        }

        /// <summary>
        /// Returns the resource part of the given full JID.
        /// e.g. 'coven@chat.shakespeare.lit/thirdwitch' => 'thirdwitch'
        /// </summary>
        /// <param name="jid">The full JID. e.g. 'coven@chat.shakespeare.lit/thirdwitch'.</param>
        /// <returns>Returns the resource part of the given full JID. e.g. 'thirdwitch'</returns>
        public static string getResourceFromFullJid(string jid)
        {
            if(jid != null)
            {
                int index = jid.IndexOf('/');
                if(index >= 0)
                {
                    return jid.Substring(index + 1);
                }
            }
            return null;
        }

        /// <summary>
        /// Tries to parse the given string to a MUCRole.
        /// Returns MUCRole.VISITOR as a default value if it fails.
        /// </summary>
        /// <param name="role">The string that should get parsed to a MUCRole.</param>
        /// <returns>Returns the MUCRole based on the given string. Defaults to: MUCRole.VISITOR</returns>
        public static MUCRole parseMUCRole(string role)
        {
            MUCRole r = MUCRole.VISITOR;
            Enum.TryParse(role.ToUpper(), out r);
            return r;
        }

        /// <summary>
        /// Tries to parse the given string to a MUCAffiliation.
        /// Returns MUCAffiliation.NONE as a default value if it fails.
        /// </summary>
        /// <param name="affiliation">The string that should get parsed to a MUCAffiliation.</param>
        /// <returns>Returns the MUCAffiliation based on the given string. Defaults to: MUCAffiliation.NONE</returns>
        public static MUCAffiliation parseMUCAffiliation(string affiliation)
        {
            MUCAffiliation a = MUCAffiliation.NONE;
            Enum.TryParse(affiliation.ToUpper(), out a);
            return a;
        }

        /// <summary>
        /// Converts the given MUCRole to the equivalent string representation.
        /// e.g. MUCRole.VISITOR => 'visitor'
        /// </summary>
        public static string mucRoleToString(MUCRole role)
        {
            return role.ToString().ToLower();
        }

        /// <summary>
        /// Converts the given MUCAffiliation to the equivalent string representation.
        /// e.g. MUCAffiliation.NONE => 'none'
        /// </summary>
        public static string mucAffiliationToString(MUCAffiliation affiliation)
        {
            return affiliation.ToString().ToLower();
        }

        /// <summary>
        /// Converts the given string to a presence.
        /// 'null' and empty strings return Presence.Online.
        /// The default return value is Presence.Unavailable.
        /// e.g. 'chat' => Presence.Chat, '' => return Presence.Online
        /// </summary>
        /// <param name="presence">The string that should get converted to a presence.</param>
        public static Presence parsePresence(string presence)
        {
            switch (presence?.ToLower())
            {
                case "chat":
                    return Presence.Chat;

                case "away":
                    return Presence.Away;

                case "xa":
                    return Presence.Xa;

                case "dnd":
                    return Presence.Dnd;

                case null:
                case "":
                    return Presence.Online;

                case "unavailable":
                default:
                    return Presence.Unavailable;
            }
        }

        /// <summary>
        /// Converts the given presence to its string representation.
        /// e.g. Presence.Unavailable => 'unavailable'
        /// </summary>
        /// <param name="presence">The presence, that should get converted to a string.</param>
        /// <returns>Returns the string representation of the given presence.</returns>
        public static string presenceToString(Presence presence)
        {
            switch (presence)
            {
                case Presence.Unavailable:
                    return "unavailable";

                case Presence.Dnd:
                    return "dnd";

                case Presence.Xa:
                    return "xa";

                case Presence.Away:
                    return "away";

                case Presence.Online:
                    return "online";

                case Presence.Chat:
                    return "chat";

                default:
                    return presence.ToString().ToLower();
            }
        }

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
