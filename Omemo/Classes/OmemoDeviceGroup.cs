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
        public readonly List<uint> DEVICE_IDS;

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        public OmemoDeviceGroup(string bareJid, List<uint> deviceIds)
        {
            BARE_JID = bareJid;
            DEVICE_IDS = deviceIds;
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
