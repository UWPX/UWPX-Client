using System.Text.RegularExpressions;

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
        public static bool isJid(string s)
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
