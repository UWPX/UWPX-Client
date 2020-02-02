using System;
using Windows.Security.Cryptography;
using XMPP_API.Classes.Crypto;
using XMPP_API.Classes.Network.XML.Messages.Processor;

namespace XMPP_API.Classes.Network.XML.Messages.Features.SASL.SHA1
{
    public class ScramSHA1SASLMechanism: AbstractSASLMechanism
    {
        // https://stackoverflow.com/questions/29298346/xmpp-sasl-scram-sha1-authentication
        // https://xmpp.org/rfcs/rfc6120.html#examples-c2s-sasl
        // https://wiki.xmpp.org/web/SASLandSCRAM-SHA-1
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        protected const byte CLIENT_NONCE_LENGTH = 32;

        protected readonly string CLIENT_NONCE_BASE_64;
        protected readonly string PASSWORD_NORMALIZED;
        protected string serverNonce;
        protected string saltBase64;
        protected string clientFirstMsg;
        protected string serverFirstMsg;

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        /// <summary>
        /// Basic Constructor
        /// </summary>
        /// <history>
        /// 22/08/2017 Created [Fabian Sauter]
        /// </history>
        public ScramSHA1SASLMechanism(string id, string password, SASLConnection saslConnection) : this(id, password, CryptographicBuffer.EncodeToBase64String(CryptographicBuffer.GenerateRandom(CLIENT_NONCE_LENGTH)), saslConnection)
        {

        }

        public ScramSHA1SASLMechanism(string id, string password, string clientNonceBase64, SASLConnection saslConnection) : base(id, password, saslConnection)
        {
            PASSWORD_NORMALIZED = password.Normalize();
            CLIENT_NONCE_BASE_64 = clientNonceBase64;
            serverNonce = null;
            saltBase64 = null;
            clientFirstMsg = null;
            serverFirstMsg = null;
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
                    onSaslError("SCRAM-SHA: Invalid server first message received: " + serverFirstMsg);
                    return null;
                }

                string sNonce = parts[0];
                if (!sNonce.StartsWith("r="))
                {
                    onSaslError("SCRAM-SHA: Invalid order for server first message received: " + serverFirstMsg);
                    return null;
                }
                serverNonce = sNonce.Substring(2);

                string saltTemp = parts[1];
                if (!saltTemp.StartsWith("s="))
                {
                    onSaslError("SCRAM-SHA: Invalid order for server first message received: " + serverFirstMsg);
                    return null;
                }
                saltBase64 = saltTemp.Substring(2);

                string itersStr = parts[2];
                if (!itersStr.StartsWith("i="))
                {
                    onSaslError("SCRAM-SHA: Invalid order for server first message received: " + serverFirstMsg);
                    return null;
                }
                itersStr = itersStr.Substring(2);
                int iters = -1;
                if (!int.TryParse(itersStr, out iters))
                {
                    onSaslError("SCRAM-SHA: Could not parse iterations for server first message: " + serverFirstMsg);
                    return null;
                }
                else if (!isValidIterationsCount(iters))
                {
                    onSaslError("SCRAM-SHA: Invalid iterations count " + itersStr + " received!");
                    return null;
                }

                return new ScramSha1ChallengeSolutionMessage(computeAnswer(iters));
            }
            onSaslError("SCRAM-SHA: Invalid challenge message received!");
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
        protected string computeAnswer(int iterations)
        {
            string clientFinalMessageBare = "c=biws,r=" + serverNonce;
            byte[] saltBytes = Convert.FromBase64String(saltBase64);
            byte[] saltedPassword = CryptoUtils.Pbkdf2Sha1(PASSWORD_NORMALIZED, saltBytes, iterations);

            byte[] clientKey = CryptoUtils.HmacSha1("Client Key", saltedPassword);
            byte[] storedKey = CryptoUtils.SHA_1(clientKey);
            string authMessage = clientFirstMsg + ',' + serverFirstMsg + ',' + clientFinalMessageBare;

            byte[] clientSignature = CryptoUtils.HmacSha1(authMessage, storedKey);
            byte[] clientProof = CryptoUtils.xor(clientKey, clientSignature);
            string clientFinalMessage = clientFinalMessageBare + ",p=" + Convert.ToBase64String(clientProof);

            return encodeStringBase64(clientFinalMessage);
        }

        protected virtual bool isValidIterationsCount(int iters)
        {
            return iters > 0;
        }

        #endregion

        #region --Misc Methods (Protected)--


        #endregion
        //--------------------------------------------------------Events:---------------------------------------------------------------------\\
        #region --Events--


        #endregion
    }
}
