using System;
using System.Linq;
using System.Text;
using XMPP_API.Classes.Network.XML.Messages.XEP_0045;

namespace XMPP_API.Classes
{
    public static class Utils
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        /// <summary>
        /// Defines the excluded characters for JID local parts.
        /// Source: RFC 7622 (https://tools.ietf.org/html/rfc7622#section-3.3)
        /// </summary>
        private static readonly char[] JID_LOCAL_PART_EXCLUDED_CHARS = new char[] { '"', '&', '\'', '/', ':', '<', '>', '@' };

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--


        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--
        /// <summary>
        /// Returns the domain part from the given bare/full JID.
        /// Based on RFC 7622 (https://tools.ietf.org/html/rfc7622#section-3.2).
        /// e.g. 'coven@chat.shakespeare.lit' => 'chat.shakespeare.lit'
        /// </summary>
        /// <param name="jid">The JID you want to retrieve the domain part from.</param>
        /// <returns>The domain part of the given bare/full JID.</returns>
        public static string getJidDomainPart(string jid)
        {
            // Remove JID resource part:
            int index = jid.IndexOf('/');
            if (index >= 0)
            {
                jid = jid.Substring(0, index);
            }

            // Remove JID local part:
            index = jid.IndexOf('@');
            if (index >= 0)
            {
                jid = jid.Substring(index + 1);
            }

            // Remove label separator (dot):
            if (jid.EndsWith("."))
            {
                jid = jid.Substring(0, jid.Length - 1);
            }
            return jid;
        }

        /// <summary>
        /// Returns the local part from the given bare/full JID.
        /// Based on RFC 7622 (https://tools.ietf.org/html/rfc7622#section-3.3).
        /// e.g. 'coven@chat.shakespeare.lit' => 'coven'
        /// </summary>
        /// <param name="jid">The JID you want to retrieve the local part from.</param>
        /// <returns>The local part of the given bare/full JID.</returns>
        public static string getJidLocalPart(string jid)
        {
            int index = jid.IndexOf('@');
            if (index >= 0)
            {
                return jid.Substring(0, index);
            }
            return "";
        }

        /// <summary>
        /// Returns the resource part from the given bare/full JID.
        /// Based on RFC 7622 (https://tools.ietf.org/html/rfc7622#section-3.4).
        /// e.g. 'coven@chat.shakespeare.lit/thirdwitch' => 'thirdwitch'
        /// </summary>
        /// <param name="jid">The JID you want to retrieve the resource part from.</param>
        /// <returns>The resource part of the given bare/full JID.</returns>
        public static string getJidResourcePart(string jid)
        {
            int index = jid.IndexOf('/');
            if (index >= 0)
            {
                return jid.Substring(index + 1);
            }
            return "";
        }

        /// <summary>
        /// Checks whether the given JID domain part fulfills the RFC 7622 rules.
        /// Based on RFC 7622 (https://tools.ietf.org/html/rfc7622#section-3.2).
        /// </summary>
        /// <param name="domainPart">The domain part string to check.</param>
        /// <returns>True if fulfills the RFC 7622 rules.</returns>
        public static bool isValidJidDomainPart(string domainPart)
        {
            if (domainPart is null)
            {
                return false;
            }

            // Check valid length:
            int byteCount = Encoding.UTF8.GetByteCount(domainPart);
            if (byteCount <= 0 || byteCount > 1024)
            {
                return false;
            }

            // Check if valid FQDN, IPv4 address or IPv6 address:
            if (Uri.CheckHostName(domainPart) == UriHostNameType.Unknown)
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// Checks whether the given JID local part fulfills the RFC 7622 rules.
        /// Based on RFC 7622 (https://tools.ietf.org/html/rfc7622#section-3.3).
        /// </summary>
        /// <param name="localPart">The local part string to check.</param>
        /// <returns>True if fulfills the RFC 7622 rules.</returns>
        public static bool isValidJidLocalPart(string localPart)
        {
            if (localPart is null)
            {
                return false;
            }

            // Check valid length:
            int byteCount = Encoding.UTF8.GetByteCount(localPart);
            if (byteCount <= 0 || byteCount > 1024)
            {
                return false;
            }

            // Check if contains excluded characters:
            if (JID_LOCAL_PART_EXCLUDED_CHARS.Any(localPart.Contains))
            {
                return false;
            }

            // Check for whitespaces:
            if(localPart.Any(char.IsWhiteSpace))
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Checks whether the given JID resource part fulfills the RFC 7622 rules.
        /// Based on RFC 7622 (https://tools.ietf.org/html/rfc7622#section-3.4).
        /// </summary>
        /// <param name="resourcePart">The resource part string to check.</param>
        /// <returns>True if fulfills the RFC 7622 rules.</returns>
        public static bool isValidJidResourcePart(string resourcePart)
        {
            if (resourcePart is null)
            {
                return false;
            }

            // Check valid length:
            int byteCount = Encoding.UTF8.GetByteCount(resourcePart);
            if (byteCount <= 0 || byteCount > 1024)
            {
                return false;
            }

            // Check if starts/ends with whitespaces:
            if(char.IsWhiteSpace(resourcePart[0]) || char.IsWhiteSpace(resourcePart[resourcePart.Length - 1]))
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Checks if the given string is a bare JID.
        /// Based on RFC 7622 (https://tools.ietf.org/html/rfc7622).
        /// e.g. 'coven@chat.shakespeare.lit'
        /// </summary>
        /// <param name="s">The string, that should get checked.</param>
        public static bool isBareJid(string s)
        {
            if (!s.Contains("@"))
            {
                return false;
            }

            string localPart = getJidLocalPart(s);
            if (!isValidJidLocalPart(localPart))
            {
                return false;
            }

            string domainPart = getJidDomainPart(s);
            if (!isValidJidDomainPart(domainPart))
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Checks if the given string is a bare JID.
        /// Based on RFC 7622 (https://tools.ietf.org/html/rfc7622).
        /// e.g. 'coven@chat.shakespeare.lit'
        /// </summary>
        /// <param name="s">The string, that should get checked.</param>
        public static bool isFullJid(string s)
        {
            if (!s.Contains("@") || !s.Contains("/"))
            {
                return false;
            }

            string localPart = getJidLocalPart(s);
            if (!isValidJidLocalPart(localPart))
            {
                return false;
            }

            string domainPart = getJidDomainPart(s);
            if (!isValidJidDomainPart(domainPart))
            {
                return false;
            }

            string resourcePart = getJidResourcePart(s);
            if (!isValidJidResourcePart(resourcePart))
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Checks if the given string is a valid server address.
        /// e.g. 'chat.shakespeare.lit' or '192.168.178.42'
        /// </summary>
        /// <param name="s">The string, that should get checked.</param>
        public static bool isValidServerAddress(string s)
        {
            return Uri.CheckHostName(s) != UriHostNameType.Unknown;
        }

        /// <summary>
        /// Returns the bare JID from the full JID.
        /// e.g. 'coven@chat.shakespeare.lit/thirdwitch' => 'coven@chat.shakespeare.lit'
        /// </summary>
        /// <param name="jid">The full JID. e.g. 'coven@chat.shakespeare.lit/thirdwitch'.</param>
        /// <returns>Returns the bare JID. e.g. 'coven@chat.shakespeare.lit'.</returns>
        public static string getBareJidFromFullJid(string jid)
        {
            if (jid is null)
            {
                return null;
            }

            // Remove JID resource part:
            int index = jid.IndexOf('/');
            if (index >= 0)
            {
                jid = jid.Substring(0, index);
            }
            return jid;
        }

        #endregion
        //--------------------------------------------------------Misc Methods:---------------------------------------------------------------\\
        #region --Misc Methods (Public)--
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

        #region --Misc Methods (Private)--


        #endregion

        #region --Misc Methods (Protected)--


        #endregion
        //--------------------------------------------------------Events:---------------------------------------------------------------------\\
        #region --Events--


        #endregion
    }
}
