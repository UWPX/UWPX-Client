using System.Collections.Generic;
using Omemo.Classes;

namespace XMPP_API.Classes.Network.XML.Messages.XEP_0384.Session
{
    public class OmemoSessions
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        public readonly OmemoDeviceGroup SRC_DEVICE_GROUP;
        public readonly OmemoDeviceGroup DST_DEVICE_GROUP;

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        public OmemoSessions(OmemoDeviceGroup srcDeviceGroup, OmemoDeviceGroup dstDeviceGroup)
        {
            SRC_DEVICE_GROUP = srcDeviceGroup;
            DST_DEVICE_GROUP = dstDeviceGroup;
        }

        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--


        #endregion
        //--------------------------------------------------------Misc Methods:---------------------------------------------------------------\\
        #region --Misc Methods (Public)--
        public List<OmemoDeviceGroup> toList()
        {
            return new List<OmemoDeviceGroup>() { SRC_DEVICE_GROUP, DST_DEVICE_GROUP };
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
