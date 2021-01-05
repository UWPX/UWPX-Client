using System;
using System.Collections.Generic;
using Omemo.Classes.Keys;

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
        public readonly List<Tuple<uint, Bundle>> DEVICE_IDS;

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        public OmemoDeviceGroup(string bareJid, List<Tuple<uint, Bundle>> deviceIds)
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
