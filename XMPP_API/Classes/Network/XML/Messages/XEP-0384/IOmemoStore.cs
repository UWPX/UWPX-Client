using libsignal.state;
using System.Collections.Generic;

namespace XMPP_API.Classes.Network.XML.Messages.XEP_0384
{
    public interface IOmemoStore : SignalProtocolStore, IOmemoDeviceStore
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--


        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--


        #endregion
        //--------------------------------------------------------Misc Methods:---------------------------------------------------------------\\
        #region --Misc Methods (Public)--
        IList<PreKeyRecord> LoadPreKeys();

        void StorePreKeys(IList<PreKeyRecord> preKeys);

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
