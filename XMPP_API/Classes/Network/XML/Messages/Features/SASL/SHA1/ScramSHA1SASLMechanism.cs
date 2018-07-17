using System;
using System.Linq;
using System.Text;
using Windows.Security.Cryptography;

namespace XMPP_API.Classes.Network.XML.Messages.Features.SASL.SHA1
{
    public class ScramSHA1SASLMechanism : AbstractSASLMechanism
    {
        // https://stackoverflow.com/questions/29298346/xmpp-sasl-scram-sha1-authentication
        // https://xmpp.org/rfcs/rfc6120.html#examples-c2s-sasl
        // https://wiki.xmpp.org/web/SASLandSCRAM-SHA-1
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        private const byte CLIENT_NONCE_LENGTH = 32;

        private readonly string CLIENT_NONCE;
        private readonly string PASSWORD_NORMALIZED;
        private string serverNonce;
        private string salt;
        private string clientFirstMsg;
        private string serverFirstMsg;

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        /// <summary>
        /// Basic Constructor
        /// </summary>
        /// <history>
        /// 22/08/2017 Created [Fabian Sauter]
        /// </history>
        public ScramSHA1SASLMechanism(string id, string password) : base(id, password)
        {
            this.PASSWORD_NORMALIZED = password.Normalize();
            this.CLIENT_NONCE = CryptographicBuffer.EncodeToHexString(CryptographicBuffer.GenerateRandom(CLIENT_NONCE_LENGTH));
            this.serverNonce = null;
            this.salt = null;
            this.clientFirstMsg = null;
            this.serverFirstMsg = null;
        }

        public ScramSHA1SASLMechanism(string id, string password, string clientNonce) : base(id, password)
        {
            this.PASSWORD_NORMALIZED = password.Normalize();
            this.CLIENT_NONCE = CryptographicBuffer.EncodeToHexString(CryptographicBuffer.GenerateRandom(CLIENT_NONCE_LENGTH));
            this.CLIENT_NONCE = "fyko+d2lbbFgONRv9qkxdawL";
            this.serverNonce = null;
            this.salt = null;
            this.clientFirstMsg = null;
            this.serverFirstMsg = null;
        }

        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--


        #endregion
        //--------------------------------------------------------Misc Methods:---------------------------------------------------------------\\
        #region --Misc Methods (Public)--
        public override AbstractMessage generateResponse(AbstractMessage msg)
        {
            if (msg is ScramSHA1ChallengeMessage challenge)
            {
                serverFirstMsg = decodeStringBase64(challenge.CHALLENGE);

                serverFirstMsg = "r=fyko+d2lbbFgONRv9qkxdawL3rfcNHYJY1ZVvWVs7j,s=QSXCR+Q6sek8bf92,i=4096";


                string[] parts = serverFirstMsg.Split(',');
                if (parts.Length != 3)
                {
                    // Throw not 3 parts
                }

                string sNonce = parts[0];
                if (!sNonce.StartsWith("r="))
                {
                    // Throw wrong order
                }
                serverNonce = sNonce.Substring(2);

                string saltTemp = parts[1];
                if (!saltTemp.StartsWith("s="))
                {
                    // Throw wrong order
                }
                salt = decodeStringBase64(saltTemp.Substring(2));

                string itersStr = parts[2];
                if (!itersStr.StartsWith("i="))
                {
                    // Throw wrong order
                }
                itersStr = itersStr.Substring(2);
                if (!uint.TryParse(itersStr, out uint iters))
                {
                    // Throw could not pars iterations
                }

                return new ScramSha1ChallengeSolutionMessage(computeAnswer(iters));
            }
            // Throw wrong message
            return null;
        }

        public override SelectSASLMechanismMessage getSelectSASLMechanismMessage()
        {
            clientFirstMsg = "n=" + ID + ",r=" + CLIENT_NONCE;
            string encClientFirstMsg = encodeStringBase64("n,," + clientFirstMsg);

            return new SelectSASLMechanismMessage("SCRAM-SHA-1", encClientFirstMsg);
        }

        #endregion

        #region --Misc Methods (Private)--
        private string computeAnswer(uint iterations)
        {
            byte[] b = Encoding.ASCII.GetBytes(salt);
            byte[] b1 = Encoding.ASCII.GetBytes("A%ÂGä:±é<mÿv");
            byte[] b2 = hexStringToByteArray("4125c247e43ab1e93c6dff76");


            string clientFinalMessageBare = "c=biws,r=" + serverNonce;
            string saltedPassword = CryptoUtils.PBKDF2_SHA_1(PASSWORD_NORMALIZED, salt, iterations);
            string saltedPassword2 = CryptoUtils.PBKDF2_SHA_1(PASSWORD_NORMALIZED, "A%ÂGä:±é<mÿv", iterations);

            byte[] b3 = hexStringToByteArray("1d96ee3a529b5a5f9e47c01f229a2cb8a6e15f7d");
            byte[] b4 = hexStringToByteArray(saltedPassword);
            byte[] b5 = hexStringToByteArray(saltedPassword2);

            string clientKey = CryptoUtils.HMAC_SHA_1(saltedPassword, hexStringToByteArray("e234c47bf6c36696dd6d852b99aaa2ba26555728"));
            string storedKey = CryptoUtils.SHA_1(clientKey);
            string authMessage = clientFirstMsg + ',' + serverFirstMsg + ',' + clientFinalMessageBare;
            string clientSignature = CryptoUtils.HMAC_SHA_1(storedKey, authMessage);
            string clientProof = CryptoUtils.xorStrings(clientKey, authMessage);
            string serverKey = CryptoUtils.HMAC_SHA_1(saltedPassword, hexStringToByteArray("e9d94660c39d65c38fbad91c358f14da0eef2bd6"));
            string serverSignature = CryptoUtils.HMAC_SHA_1(serverKey, authMessage);
            string clientFinalMessage = clientFinalMessageBare + ",p=" + encodeStringBase64(clientProof);
            return encodeStringBase64(clientFinalMessage);
        }

        public static byte[] hexStringToByteArray(string hex)
        {
            return Enumerable.Range(0, hex.Length)
                             .Where(x => x % 2 == 0)
                             .Select(x => Convert.ToByte(hex.Substring(x, 2), 16))
                             .ToArray();
        }

        #endregion

        #region --Misc Methods (Protected)--


        #endregion
        //--------------------------------------------------------Events:---------------------------------------------------------------------\\
        #region --Events--


        #endregion
    }
}
