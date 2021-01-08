namespace Omemo.Classes
{
    public class OmemoProtocolAddress
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        public readonly string BARE_JID;
        public readonly uint DEVICE_ID;

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        public OmemoProtocolAddress(string bareJid, uint deviceId)
        {
            BARE_JID = bareJid;
            DEVICE_ID = deviceId;
        }

        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--


        #endregion
        //--------------------------------------------------------Misc Methods:---------------------------------------------------------------\\
        #region --Misc Methods (Public)--
        public override bool Equals(object obj)
        {
            return obj is OmemoProtocolAddress address && address.DEVICE_ID == DEVICE_ID && string.Equals(address.BARE_JID, BARE_JID);
        }

        public override int GetHashCode()
        {
            return ((int)DEVICE_ID) ^ BARE_JID.GetHashCode();
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
