using System;

namespace XMPP_API_IoT.Classes.Bluetooth.Events
{
    public class BLEDeviceStateChangedEventArgs: EventArgs
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        public readonly BLEDeviceState OLD_STATE;
        public readonly BLEDeviceState NEW_STATE;

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        public BLEDeviceStateChangedEventArgs(BLEDeviceState oldState, BLEDeviceState newState)
        {
            OLD_STATE = oldState;
            NEW_STATE = newState;
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
