using System.Security.Cryptography;
using System.Text;
using Shared.Classes;
using XMPP_API.Classes.Crypto;

namespace Push.Classes
{
    public static class Utils
    {
        /// <summary>
        /// Generates a SHA256 hex string based on the given bare JID and the current device ID.
        /// </summary>
        /// <param name="bareJid">The bare JID (e.g. 'someone@example.com').</param>
        /// <returns>A SHA256 hex string.</returns>
        public static string ToAccountId(string bareJid)
        {
            string input = SharedUtils.GetUniqueDeviceId() + bareJid;
            byte[] result;
            using (SHA256 sha = SHA256.Create())
            {
                result = sha.ComputeHash(Encoding.UTF8.GetBytes(input));
            }
            return CryptoUtils.byteArrayToHexString(result);
        }
    }
}
