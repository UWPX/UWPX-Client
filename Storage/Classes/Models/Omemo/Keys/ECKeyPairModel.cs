using System.ComponentModel.DataAnnotations;
using Omemo.Classes.Keys;

namespace Storage.Classes.Models.Omemo.Keys
{
    public class ECKeyPairModel: AbstractOmemoModel
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        [Key]
        public int id { get; set; }
        public byte[] privKey { get; set; }
        public byte[] pubKey { get; set; }

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        public ECKeyPairModel() { }

        public ECKeyPairModel(ECKeyPair key)
        {
            privKey = key.privKey.key;
            pubKey = key.pubKey.key;
        }

        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--


        #endregion
        //--------------------------------------------------------Misc Methods:---------------------------------------------------------------\\
        #region --Misc Methods (Public)--
        public ECKeyPair ToECKeyPair()
        {
            return new ECKeyPair(new ECPrivKey(privKey), new ECPubKey(pubKey));
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
