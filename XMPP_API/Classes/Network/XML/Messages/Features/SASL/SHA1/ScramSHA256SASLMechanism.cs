namespace XMPP_API.Classes.Network.XML.Messages.Features.SASL.SHA1
{
    public class ScramSHA256SASLMechanism : ScramSHA1SASLMechanism
    {
        // https://tools.ietf.org/html/rfc7677
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
        /// 06/01/2019 Created [Fabian Sauter]
        /// </history>
        public ScramSHA256SASLMechanism(string id, string password) : base(id, password)
        {
        }

        public ScramSHA256SASLMechanism(string id, string password, string clientNonceBase64) : base(id, password, clientNonceBase64)
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
