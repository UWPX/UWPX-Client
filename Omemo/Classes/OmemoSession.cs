using Omemo.Classes.Keys;

namespace Omemo.Classes
{
    public class OmemoSession
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        public readonly DoubleRachet RACHET;
        public readonly EphemeralKeyPair EPHEMERAL_KEY_PAIR;

        // State variables:


        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        public OmemoSession()
        {
            RACHET = new DoubleRachet(this);
            EPHEMERAL_KEY_PAIR = KeyHelper.GenerateEphemeralKeyPair();
        }

        public OmemoSession(byte[] data)
        {

        }

        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--


        #endregion
        //--------------------------------------------------------Misc Methods:---------------------------------------------------------------\\
        #region --Misc Methods (Public)--
        public byte[] Serialize()
        {

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
