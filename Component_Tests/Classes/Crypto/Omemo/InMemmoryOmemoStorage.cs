using System.Collections.Generic;
using Omemo.Classes;

namespace Component_Tests.Classes.Crypto.Omemo
{
    public class InMemmoryOmemoStorage: IOmemoStorage
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        public readonly Dictionary<OmemoProtocolAddress, OmemoSession> SESSIONS = new Dictionary<OmemoProtocolAddress, OmemoSession>();

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--


        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--


        #endregion
        //--------------------------------------------------------Misc Methods:---------------------------------------------------------------\\
        #region --Misc Methods (Public)--
        public OmemoSession LoadSession(OmemoProtocolAddress address)
        {
            return SESSIONS.ContainsKey(address) ? SESSIONS[address] : null;
        }

        public void StoreSession(OmemoProtocolAddress address, OmemoSession session)
        {
            SESSIONS[address] = session;
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
