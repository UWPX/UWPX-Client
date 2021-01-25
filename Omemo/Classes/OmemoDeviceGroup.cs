using System.Collections.Generic;

namespace Omemo.Classes
{
    /// <summary>
    /// Represents a group of OMEMO capable devices, owned by the same owner.
    /// </summary>
    public class OmemoDeviceGroup
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        public readonly string BARE_JID;
        public readonly Dictionary<uint, OmemoSessionModel> SESSIONS = new Dictionary<uint, OmemoSessionModel>();

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        public OmemoDeviceGroup(string bareJid)
        {
            BARE_JID = bareJid;
        }

        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--


        #endregion
        //--------------------------------------------------------Misc Methods:---------------------------------------------------------------\\
        #region --Misc Methods (Public)--


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
