using XMPP_API.Classes.Network.XML.Messages.Features.SASL.SHA1;
using XMPP_API.Classes.Network.XML.Messages.Processor;

namespace XMPP_API.Classes.Network.XML.Messages.Features.SASL.SHA256
{
    // https://tools.ietf.org/html/rfc7677
    public class ScramSHA256SASLMechanism : ScramSHA1SASLMechanism
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--


        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        public ScramSHA256SASLMechanism(string id, string password, SASLConnection saslConnection) : base(id, password, saslConnection)
        {
        }

        public ScramSHA256SASLMechanism(string id, string password, string clientNonceBase64, SASLConnection saslConnection) : base(id, password, clientNonceBase64, saslConnection)
        {
        }

        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--


        #endregion
        //--------------------------------------------------------Misc Methods:---------------------------------------------------------------\\
        #region --Misc Methods (Public)--
        public override SelectSASLMechanismMessage getSelectSASLMechanismMessage()
        {
            clientFirstMsg = "n=" + ID + ",r=" + CLIENT_NONCE_BASE_64;
            string encClientFirstMsg = encodeStringBase64("n,," + clientFirstMsg);

            return new SelectSASLMechanismMessage("SCRAM-SHA-256", encClientFirstMsg);
        }

        #endregion

        #region --Misc Methods (Private)--

        #endregion

        #region --Misc Methods (Protected)--
        protected override bool isValidIterationsCount(int iters)
        {
            return iters >= 4096;
        }

        #endregion
        //--------------------------------------------------------Events:---------------------------------------------------------------------\\
        #region --Events--


        #endregion
    }
}
