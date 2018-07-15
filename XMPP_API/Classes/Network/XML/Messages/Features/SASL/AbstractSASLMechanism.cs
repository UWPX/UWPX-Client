using System;
using System.Text;
using Windows.Security.Cryptography;
using Windows.Storage.Streams;

namespace XMPP_API.Classes.Network.XML.Messages.Features.SASL
{
    public abstract class AbstractSASLMechanism
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        protected readonly string ID;
        protected readonly string PASSWORD;

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        /// <summary>
        /// Basic Constructor
        /// </summary>
        /// <history>
        /// 22/08/2017 Created [Fabian Sauter]
        /// </history>
        public AbstractSASLMechanism(string id, string password)
        {
            this.ID = id;
            this.PASSWORD = password;
        }

        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--
        public abstract SelectSASLMechanismMessage getSelectSASLMechanismMessage();

        #endregion
        //--------------------------------------------------------Misc Methods:---------------------------------------------------------------\\
        #region --Misc Methods (Public)--
        public abstract AbstractMessage generateResponse(AbstractMessage msg);

        #endregion

        #region --Misc Methods (Private)--


        #endregion

        #region --Misc Methods (Protected)--
        protected static long nextInt64()
        {
            var bytes = new byte[sizeof(long)];
            IBuffer buffer = CryptographicBuffer.GenerateRandom(sizeof(long));
            CryptographicBuffer.CopyToByteArray(buffer, out bytes);
            return BitConverter.ToInt64(bytes, 0);
        }

        protected string encodeStringBase64(string s)
        {
            return Convert.ToBase64String(Encoding.ASCII.GetBytes(s));
        }

        protected string decodeStringBase64(string s)
        {
            return Encoding.ASCII.GetString(Convert.FromBase64String(s));
        }

        #endregion
        //--------------------------------------------------------Events:---------------------------------------------------------------------\\
        #region --Events--


        #endregion
    }
}
