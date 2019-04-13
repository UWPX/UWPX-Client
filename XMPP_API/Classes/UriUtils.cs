using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using Windows.Foundation;

namespace XMPP_API.Classes
{
    public static class UriUtils
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        public const string URI_SCHEME = "xmpp";

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--


        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--
        /// <summary>
        /// Combines the user info and host to a bare JID.
        /// </summary>
        /// <param name="uri">The <see cref="System.Uri"/> that should be used.</param>
        /// <returns>A bare JID or null if the host or user info is empty, white space only or null.</returns>
        public static string getBareJidFromUri(Uri uri)
        {
            if (string.IsNullOrWhiteSpace(uri.Host) || string.IsNullOrWhiteSpace(uri.UserInfo))
            {
                return null;
            }
            StringBuilder sb = new StringBuilder(uri.UserInfo);
            sb.Append('@');
            sb.Append(uri.Host);
            return sb.ToString();
        }

        #endregion
        //--------------------------------------------------------Misc Methods:---------------------------------------------------------------\\
        #region --Misc Methods (Public)--
        /// <summary>
        /// Builds a <see cref="System.Uri"/> object from the given <paramref name="bareJid"/> and <paramref name="queryPairs"/>.
        /// Uses <see cref="URI_SCHEME"/> as scheme.
        /// </summary>
        /// <param name="bareJid">The bare JID that should be used as host.</param>
        /// <param name="queryPairs">All queries.</param>
        /// <returns>The <see cref="System.Uri"/> object representing the given attributes.</returns>
        public static Uri buildUri(string bareJid, Dictionary<string, string> queryPairs)
        {
            string query = string.Join("&",
                queryPairs.Keys.Where(key => !string.IsNullOrWhiteSpace(queryPairs[key]))
                .Select(key => string.Format("{0}={1}", WebUtility.UrlEncode(key), WebUtility.UrlEncode(queryPairs[key]))));

            UriBuilder builder = new UriBuilder
            {
                Scheme = "xmpp",
                Host = bareJid,
                Query = query
            };

            return builder.Uri;
        }

        /// <summary>
        /// Builds a <see cref="System.Uri"/> object from the given <paramref name="queryPairs"/>.
        /// Uses <see cref="URI_SCHEME"/> as scheme.
        /// </summary>
        /// <param name="queryPairs">All queries.</param>
        /// <returns>The <see cref="System.Uri"/> object representing the given attributes.</returns>
        public static Uri buildUri(Dictionary<string, string> queryPairs)
        {
            return buildUri("", queryPairs);
        }

        /// <summary>
        /// Parses the given <see cref="System.Uri"/> query to a <see cref="WwwFormUrlDecoder"/> object.
        /// </summary>
        /// <param name="uri">The <see cref="System.Uri"/> thats query part should be parsed.</param>
        /// <returns>A list of attributes and values font in the given <see cref="System.Uri"/>.</returns>
        public static WwwFormUrlDecoder parseUriQuery(Uri uri)
        {
            return new WwwFormUrlDecoder(uri.Query);
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
