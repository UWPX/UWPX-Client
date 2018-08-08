using libsignal;
using libsignal.protocol;
using System;
using System.Text;
using System.Xml.Linq;

namespace XMPP_API.Classes.Network.XML.Messages.XEP_0384
{
    public class OmemoMessageMessage : MessageMessage
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        public readonly uint SID;
        public readonly uint KEY_RID;
        public readonly string BASE_64_KEY;
        public readonly uint PRE_KEY_RID;
        public readonly string BASE_64_PRE_KEY;
        public readonly string BASE_64_IV;
        public string BASE_64_PAYLOAD { get; private set; }
        public bool ENCRYPTED { get; private set; }

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        /// <summary>
        /// Basic Constructor
        /// </summary>
        /// <history>
        /// 08/08/2018 Created [Fabian Sauter]
        /// </history>
        public OmemoMessageMessage(string from, string to, string message, string type, bool reciptRequested) : base(from, to, message, type, reciptRequested)
        {

        }

        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--
        /// <summary>
        /// Takes the TO property and returns the bare JID e.g 'coven@chat.shakespeare.lit'
        /// </summary>
        /// <returns>The bare JID e.g 'coven@chat.shakespeare.lit'</returns>
        public string getBareChatJid()
        {
            return Utils.getBareJidFromFullJid(TO);
        }

        #endregion
        //--------------------------------------------------------Misc Methods:---------------------------------------------------------------\\
        #region --Misc Methods (Public)--
        /// <summary>
        /// Encrypts the content of MESSAGE with the given SessionCipher and saves the result in BASE_64_PAYLOAD.
        /// Sets ENCRYPTED to true.
        /// </summary>
        /// <param name="cipher">The SessionCipher for encrypting the content of MESSAGE.</param>
        public void encrypt(SessionCipher cipher)
        {
            byte[] encoded = Encoding.Unicode.GetBytes(MESSAGE);
            CiphertextMessage ciphertextMessage = cipher.encrypt(encoded);
            BASE_64_PAYLOAD = Convert.ToBase64String(ciphertextMessage.serialize());
            ENCRYPTED = true;
        }

        /// <summary>
        /// Decrypts the content of BASE_64_PAYLOAD with the given SessionCipher and saves the result in MESSAGE.
        /// Sets ENCRYPTED to false.
        /// </summary>
        /// <param name="cipher">The SessionCipher for decrypting the content of BASE_64_PAYLOAD.</param>
        public void decrypt(SessionCipher cipher)
        {
            ENCRYPTED = false;
        }

        public override XElement toXElement()
        {
            if (!ENCRYPTED)
            {
                throw new InvalidOperationException("Message not encrypted! Call encrypt(...) first.");
            }

            XElement msgNode = base.toXElement();

            XNamespace ns = Consts.XML_XEP_0384_NAMESPACE;
            XElement encNode = new XElement(ns + "encrypted");

            encNode.Add(new XElement(ns + "payload")
            {
                Value = BASE_64_PAYLOAD
            });
            XElement headerNode = new XElement(ns + "header");
            headerNode.Add(new XAttribute("sid", SID));

            XElement keyNode = new XElement(ns + "key")
            {
                Value = BASE_64_KEY
            };
            keyNode.Add(new XAttribute("rid", KEY_RID));
            headerNode.Add(keyNode);
            XElement preKeyNode = new XElement(ns + "key")
            {
                Value = BASE_64_PRE_KEY
            };
            preKeyNode.Add(new XAttribute("rid", PRE_KEY_RID));
            preKeyNode.Add(new XAttribute("prekey", true));
            headerNode.Add(preKeyNode);
            headerNode.Add(new XElement(ns + "iv")
            {
                Value = BASE_64_IV
            });
            msgNode.Add(encNode);

            return msgNode;
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
