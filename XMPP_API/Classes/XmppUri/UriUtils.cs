using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using Logging;
using Omemo.Classes;
using Omemo.Classes.Keys;
using Shared.Classes;
using Windows.Foundation;
using XMPP_API.Classes.Network.XML.Messages.XEP_0384;

namespace XMPP_API.Classes.XmppUri
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
                queryPairs.Keys.Select(key => !string.IsNullOrWhiteSpace(queryPairs[key]) ? string.Format("{0}={1}", WebUtility.UrlEncode(key), WebUtility.UrlEncode(queryPairs[key])) : WebUtility.UrlEncode(key)));

            UriBuilder builder = new UriBuilder
            {
                Scheme = "xmpp",
                Host = bareJid,
                Query = query
            };

            return builder.Uri;
        }

        public static string toXmppUriString(Uri uri)
        {
            StringBuilder sb = new StringBuilder(uri.Scheme);
            sb.Append(':');
            sb.Append(uri.Host);
            sb.Append(uri.Query);
            return sb.ToString();
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
            // Throws a NullReferenceException if the given Uri is only "xmpp:"
            try
            {
                return new WwwFormUrlDecoder(uri.Query);
            }
            catch (NullReferenceException e)
            {
                Logger.Error("Failed to decode XMPP Uri: " + uri.ToString(), e);
                return null;
            }
        }

        /// <summary>
        /// [WIP]<para/>
        /// Parses XMPP IRIs and URIs based on RFC 5122 and returns the result.
        /// </summary>
        /// <param name="uri">The URI or IRI that should get parsed.</param>
        /// <returns>The URI or IRI result or null if an error occurred.</returns>
        public static IUriAction parse(Uri uri)
        {
            if (!string.IsNullOrEmpty(uri?.OriginalString))
            {
                if (string.Equals(uri.Scheme.ToLowerInvariant(), "xmpp"))
                {
                    string tmp = uri.OriginalString;

                    // 1. remove 'xmpp:'
                    tmp = tmp.Substring(5);

                    // 2. Authority
                    string authority = null;
                    if (tmp.StartsWith("//"))
                    {
                        tmp.Substring(2);
                        int authEnd = tmp.IndexOf('/');
                        if (authEnd < 0)
                        {
                            authEnd = tmp.IndexOf('?');
                            if (authEnd < 0)
                            {
                                authEnd = tmp.IndexOf('#');
                                if (authEnd < 0)
                                {
                                    authEnd = tmp.Length <= 0 ? 0 : tmp.Length - 1;
                                }
                            }
                            authority = tmp.Substring(0, authEnd);
                            tmp = tmp.Substring(authEnd + 1);
                        }
                    }

                    if (string.Equals(uri.AbsolutePath, "iot-register"))
                    {
                        WwwFormUrlDecoder query = parseUriQuery(uri);
                        if (query is null)
                        {
                            return null;
                        }

                        IWwwFormUrlDecoderEntry macEntry = query.FirstOrDefault(x => x.Name.StartsWith("mac"));
                        if (macEntry is null || string.IsNullOrEmpty(macEntry.Value))
                        {
                            Logger.Error("None or invalid IoT MAC address: " + uri.OriginalString);
                            return null;
                        }
                        IWwwFormUrlDecoderEntry algoEntry = query.FirstOrDefault(x => x.Name.StartsWith("algo"));
                        if (algoEntry is null || string.IsNullOrEmpty(algoEntry.Value))
                        {
                            Logger.Error("None or invalid IoT key algorithm: " + uri.OriginalString);
                            return null;
                        }
                        IWwwFormUrlDecoderEntry keyEntry = query.FirstOrDefault(x => x.Name.StartsWith("key"));
                        if (keyEntry is null || string.IsNullOrEmpty(keyEntry.Value))
                        {
                            Logger.Error("None or invalid IoT key: " + uri.OriginalString);
                            return null;
                        }
                        return new RegisterIoTUriAction(macEntry.Value, algoEntry.Value, keyEntry.Value);
                    }
                    else
                    {
                        // Check if is OMEMO fingerprint URI:
                        WwwFormUrlDecoder query = parseUriQuery(uri);
                        if (query is null)
                        {
                            return null;
                        }

                        IWwwFormUrlDecoderEntry entry = query.FirstOrDefault(x => x.Name.StartsWith("omemo-sid-"));
                        if (!(entry is null))
                        {
                            ECPubKeyModel pubKey = null;
                            try
                            {
                                byte[] fingerprintBytes = SharedUtils.HexStringToByteArray(entry.Value);
                                pubKey = new ECPubKeyModel(fingerprintBytes);
                            }
                            catch (Exception e)
                            {
                                Logger.Error("Failed to parse XMPP URI. Parsing fingerprint failed: " + entry.Value, e);
                                return null;
                            }

                            if (uint.TryParse(entry.Name.Replace("omemo-sid-", "").Trim(), out uint deviceId))
                            {
                                OmemoProtocolAddress address = new OmemoProtocolAddress(uri.LocalPath, deviceId);
                                return new OmemoFingerprintUriAction(new OmemoFingerprint(pubKey, address));
                            }
                            else
                            {
                                Logger.Warn("Failed to parse XMPP URI. Invalid device ID: " + entry.Name);
                            }
                        }
                    }
                }
                else
                {
                    Logger.Warn("Failed to parse XMPP URI. No 'xmpp' scheme.");
                }
            }
            return null;
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
