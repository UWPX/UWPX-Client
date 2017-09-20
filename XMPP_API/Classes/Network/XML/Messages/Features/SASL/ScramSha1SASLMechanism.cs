using System;
using System.Globalization;
using System.Text;
using Windows.Security.Cryptography;
using Windows.Security.Cryptography.Core;
using Windows.Storage.Streams;

namespace XMPP_API.Classes.Network.XML.Messages.Features.SASL
{
    class ScramSha1SASLMechanism : AbstractSASLMechanism
    {
        // https://stackoverflow.com/questions/29298346/xmpp-sasl-scram-sha1-authentication
        // https://xmpp.org/rfcs/rfc6120.html#XEP-0138
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        private string clientNonce;
        private string serverNonce;
        private string salt;
        private string initialMessage;
        private string serverFirstMessage;

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        /// <summary>
        /// Basic Constructor
        /// </summary>
        /// <history>
        /// 22/08/2017 Created [Fabian Sauter]
        /// </history>
        public ScramSha1SASLMechanism(string id, string password) : base(id, password)
        {
            
        }

        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--
        

        #endregion
        //--------------------------------------------------------Misc Methods:---------------------------------------------------------------\\
        #region --Misc Methods (Public)--
        public override AbstractMessage generateResponse(AbstractMessage msg)
        {
            if (msg is ScramSha1ChallengeMessage)
            {
                ScramSha1ChallengeMessage c = msg as ScramSha1ChallengeMessage;
                string challengeMessage = c.getChallengeMessage();
                challengeMessage = decodeStringBase64(challengeMessage);
                //string challengeMessage = "r=fyko+d2lbbFgONRv9qkxdawL3rfcNHYJY1ZVvWVs7j,s=QSXCR+Q6sek8bf92,i=4096";
                if (serverFirstMessage == null)
                {
                    serverFirstMessage = challengeMessage;
                }
                int iterations = -1;
                string[] parts = challengeMessage.Split(',');
                foreach (string s in parts)
                {
                    switch (s[0])
                    {
                        case 'r':
                            string sN = s.Substring(2);
                            if(sN.StartsWith(clientNonce))
                            {
                                serverNonce = sN;
                            }
                            else
                            {
                                // Error server nonce does not start with client nonce
                            }
                            break;
                        case 's':
                            salt = decodeStringBase64(s.Substring(2));
                            break;
                        case 'i':
                            try
                            {
                                iterations = int.Parse(s.Substring(2));
                            }
                            catch (Exception)
                            {
                                // Error invalid iterations
                                throw;
                            }
                            break;
                        default:
                            // Error - Invalid challengeMessage
                            break;
                    }
                }
                return new ScramSha1ChallengeSolutionMessage(computeAnswer(iterations));
            }
            return null;
        }

        public override SelectSASLMechanismMessage getSelectSASLMechanismMessage()
        {
            clientNonce = nextInt64().ToString(CultureInfo.InvariantCulture);
            //clientNonce = "fyko+d2lbbFgONRv9qkxdawL";
            initialMessage = "n,,n=" + ID + ",r=" + clientNonce;
            initialMessage = encodeStringBase64(initialMessage);
            
            return new SelectSASLMechanismMessage("SCRAM-SHA-1", initialMessage);
        }

        #endregion

        #region --Misc Methods (Private)--
        private string computeAnswer(int iterations)
        {
            //string testPW = "pencil";
            //string testinitialMessage = "n,,n=user,r=fyko+d2lbbFgONRv9qkxdawL";
            string clientFinalMessageBare = "c=biws,r=" + serverNonce;
            string saltedPassword = new PBKDF(PASSWORD.Normalize(), salt, KeyDerivationAlgorithmNames.Pbkdf2Sha1, (uint)iterations).hash;
            string clientKey = new PBKDF(saltedPassword, "Client Key", KeyDerivationAlgorithmNames.Sp800108CtrHmacSha1).hash;
            string storedKey = hashMessage(clientKey, "SHA1");
            string authMessage = initialMessage + ',' + serverFirstMessage + ',' + clientFinalMessageBare;
            string clientSignature = new PBKDF(storedKey, authMessage, KeyDerivationAlgorithmNames.Sp800108CtrHmacSha1).hash;
            string clientProof = xorStrings(clientKey, authMessage);
            string serverKey = new PBKDF(saltedPassword, "Server Key", KeyDerivationAlgorithmNames.Sp800108CtrHmacSha1).hash;
            string serverSignature = new PBKDF(serverKey, authMessage, KeyDerivationAlgorithmNames.Sp800108CtrHmacSha1).hash;
            string clientFinalMessage = clientFinalMessageBare + ",p=" + encodeStringBase64(clientProof);
            return encodeStringBase64(clientFinalMessage);
        }

        private string xorStrings(string a, string b)
        {
            char[] key = b.ToCharArray();
            char[] output = new char[a.Length];

            for (int i = 0; i < a.Length; i++)
            {
                output[i] = (char)(a[i] ^ key[i % key.Length]);
            }

            return new string(output);
        }

        // Source: https://docs.microsoft.com/en-us/uwp/api/windows.security.cryptography.core.hashalgorithmprovider
        private string hashMessage(string msg, string algName)
        {
            // Convert the message string to binary data.
            IBuffer buffUtf8Msg = CryptographicBuffer.ConvertStringToBinary(msg, BinaryStringEncoding.Utf8);

            // Create a HashAlgorithmProvider object.
            HashAlgorithmProvider objAlgProv = HashAlgorithmProvider.OpenAlgorithm(algName);

            // Demonstrate how to retrieve the name of the hashing algorithm.
            String strAlgNameUsed = objAlgProv.AlgorithmName;

            // Hash the message.
            IBuffer buffHash = objAlgProv.HashData(buffUtf8Msg);

            // Verify that the hash length equals the length specified for the algorithm.
            if (buffHash.Length != objAlgProv.HashLength)
            {
                throw new Exception("There was an error creating the hash");
            }

            // Convert the hash to a string (for display).
            String strHashBase64 = CryptographicBuffer.EncodeToBase64String(buffHash);

            // Return the encoded string
            return strHashBase64;
        }

        #endregion

        #region --Misc Methods (Protected)--


        #endregion
        //--------------------------------------------------------Events:---------------------------------------------------------------------\\
        #region --Events--


        #endregion
    }
}
