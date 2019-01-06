using System;
using XMPP_API.Classes.Network.XML.Messages.Processor;

namespace XMPP_API.Classes.Network.XML.Messages.Features.SASL.Plain
{
    class PlainSASLMechanism : AbstractSASLMechanism
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--


        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        /// <summary>
        /// Basic Constructor
        /// </summary>
        /// <history>
        /// 24/08/2017 Created [Fabian Sauter]
        /// </history>
        public PlainSASLMechanism(string id, string password, SASLConnection saslConnection) : base(id, password, saslConnection)
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
            throw new NotImplementedException();
        }

        public override SelectSASLMechanismMessage getSelectSASLMechanismMessage()
        {
            return new SelectSASLMechanismMessage("PLAIN", encodeStringBase64("\0" + ID + "\0" + PASSWORD));
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
