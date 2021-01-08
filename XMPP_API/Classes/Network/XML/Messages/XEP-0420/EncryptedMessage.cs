using System;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using Logging;

namespace XMPP_API.Classes.Network.XML.Messages.XEP_0420
{
    public class EncryptedMessage: MessageMessage
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        public readonly string REF_TO;
        public readonly string REF_FROM;
        public DateTime timeStamp;

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
                Logger.Error("Time stamp validation of an encrypted message failed: " + timeStamp.ToString());
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

        /// <summary>
        /// Generates a random 
        /// </summary>
        /// <param name="length"></param>
        /// <param name="r"></param>
        /// <returns></returns>
        private string randomString(uint length, Random r)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, (int)length).Select(s => s[r.Next(s.Length)]).ToArray());
        }
        #endregion

        #region --Misc Methods (Protected)--
        /// <summary>
        /// Generates the padding element and returns it. Uses a random amount of characters for padding between the given min and max.
        /// <para/>
        /// It is suggested to use a value >0 and <200 as padding.
        /// </summary>
        /// <param name="minPadding">The inclusive lower bound.</param>
        /// <param name="maxPadding">The exclusive upper bound.</param>
        /// <param name="ns">The namespace the element should be added to.</param>
        protected XElement generatePaddingNode(uint minPadding, uint maxPadding, XNamespace ns)
        {
            Random r = new Random();
            return new XElement(ns + "rpad", randomString((uint)r.Next((int)minPadding, (int)maxPadding), r));
        }
    }

    #endregion
    //--------------------------------------------------------Events:---------------------------------------------------------------------\\
    #region --Events--


    #endregion
}
