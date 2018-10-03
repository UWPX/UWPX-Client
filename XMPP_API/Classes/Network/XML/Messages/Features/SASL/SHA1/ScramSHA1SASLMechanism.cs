using System;
using Windows.Security.Cryptography;
using XMPP_API.Classes.Crypto;

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

        private readonly string CLIENT_NONCE_BASE_64;
        private readonly string PASSWORD_NORMALIZED;
        private string serverNonce;
        private string saltBase64;
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
        public ScramSHA1SASLMechanism(string id, string password) : this(id, password, CryptographicBuffer.EncodeToBase64String(CryptographicBuffer.GenerateRandom(CLIENT_NONCE_LENGTH)))
        {

        }

        public ScramSHA1SASLMechanism(string id, string password, string clientNonceBase64) : base(id, password)
        {
            this.PASSWORD_NORMALIZED = password.Normalize();
            this.CLIENT_NONCE_BASE_64 = clientNonceBase64;
            this.serverNonce = null;
            this.saltBase64 = null;
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
                saltBase64 = saltTemp.Substring(2);

                string itersStr = parts[2];
                if (!itersStr.StartsWith("i="))
                {
                    // Throw wrong order
                }
                itersStr = itersStr.Substring(2);
                if (!int.TryParse(itersStr, out int iters))
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
            clientFirstMsg = "n=" + ID + ",r=" + CLIENT_NONCE_BASE_64;
            string encClientFirstMsg = encodeStringBase64("n,," + clientFirstMsg);

            return new SelectSASLMechanismMessage("SCRAM-SHA-1", encClientFirstMsg);
        }

        #endregion

        #region --Misc Methods (Private)--
        private string computeAnswer(int iterations)
        {
            string clientFinalMessageBare = "c=biws,r=" + serverNonce;
            byte[] saltBytes = Convert.FromBase64String(saltBase64);
            byte[] saltedPassword = CryptoUtils.Pbkdf2Sha1(PASSWORD_NORMALIZED, saltBytes, iterations);

            byte[] clientKey = CryptoUtils.HmacSha1("Client Key", saltedPassword);
            byte[] storedKey = CryptoUtils.SHA_1(clientKey);
            string authMessage = clientFirstMsg + ',' + serverFirstMsg + ',' + clientFinalMessageBare;

            byte[] clientSignature = CryptoUtils.HmacSha1(authMessage, storedKey);
            byte[] clientProof = CryptoUtils.xor(clientKey, clientSignature);
            byte[] serverKey = CryptoUtils.HmacSha1("Server Key", saltedPassword);
            string clientFinalMessage = clientFinalMessageBare + ",p=" + Convert.ToBase64String(clientProof);

            return encodeStringBase64(clientFinalMessage);
        }

        #endregion

        #region --Misc Methods (Protected)--


        #endregion
        //--------------------------------------------------------Events:---------------------------------------------------------------------\\
        #region --Events--


        #endregion
    }
}
