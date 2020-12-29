using System;
using System.Xml;
using Logging;

namespace XMPP_API.Classes.Network.XML.Messages.XEP_0420
{
    public class EncryptedMessage: MessageMessage
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        public readonly uint PADDING_LENGTH;
        public readonly DateTime TIME_STAMP;
        public readonly string REF_TO;
        public readonly string REF_FROM;

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        public EncryptedMessage(XmlNode node, CarbonCopyType ccType) : base(node, ccType)
        {
        }

        public EncryptedMessage(string from, string to, string message, string type, bool reciptRequested) : base(from, to, message, type, reciptRequested)
        {
            REF_TO = to;
            REF_FROM = from;
        }

        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--


        #endregion
        //--------------------------------------------------------Misc Methods:---------------------------------------------------------------\\
        #region --Misc Methods (Public)--
        public bool Verify()
        {
            if (!string.Equals(REF_FROM, FROM))
            {
                Logger.Error("From attribute of an encrypted message does not match: " + REF_FROM + " =! " + FROM);
                return false;
            }

            if (!string.Equals(REF_TO, TO))
            {
                Logger.Error("To attribute of an encrypted message does not match: " + REF_TO + " =! " + TO);
                return false;
            }

            if (!ValidateTimeStamp())
            {
                Logger.Error("Time stamp validation of an encrypted message failed: " + TIME_STAMP.ToString());
                return false;
            }

            return true;
        }

        #endregion

        #region --Misc Methods (Private)--
        private bool ValidateTimeStamp()
        {
            return true;
        }

        #endregion

        #region --Misc Methods (Protected)--


        #endregion
        //--------------------------------------------------------Events:---------------------------------------------------------------------\\
        #region --Events--


        #endregion
    }
}
