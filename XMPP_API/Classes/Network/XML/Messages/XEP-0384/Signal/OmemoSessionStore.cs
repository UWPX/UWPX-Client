using libsignal;
using libsignal.state;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XMPP_API.Classes.Network.XML.Messages.XEP_0384.Signal
{
    public class OmemoSessionStore : SessionStore
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--


        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        /// <summary>
        /// Basic Constructor
        /// </summary>
        /// <history>
        /// 08/08/2018 Created [Fabian Sauter]
        /// </history>
        public OmemoSessionStore()
        {
        }
        
        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--
        public List<uint> GetSubDeviceSessions(string name)
        {
            throw new NotImplementedException();
        }

        #endregion
        //--------------------------------------------------------Misc Methods:---------------------------------------------------------------\\
        #region --Misc Methods (Public)--
        public bool ContainsSession(SignalProtocolAddress address)
        {
            throw new NotImplementedException();
        }

        public void DeleteAllSessions(string name)
        {
            throw new NotImplementedException();
        }

        public void DeleteSession(SignalProtocolAddress address)
        {
            throw new NotImplementedException();
        }

        public SessionRecord LoadSession(SignalProtocolAddress address)
        {
            throw new NotImplementedException();
        }

        public void StoreSession(SignalProtocolAddress address, SessionRecord record)
        {
            throw new NotImplementedException();
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
