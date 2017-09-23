
namespace XMPP_API.Classes.Network
{
    public class XMPPAccount
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        public int port;
        public XMPPUser user;
        public string serverAddress;
        public int presencePriorety;
        public bool disabled;

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        /// <summary>
        /// Basic Constructor
        /// </summary>
        /// <history>
        /// 17/08/2017 Created [Fabian Sauter]
        /// </history>
        public XMPPAccount(XMPPUser user, string serverAddress, int port)
        {
            this.user = user;
            this.serverAddress = serverAddress;
            this.port = port;
        }

        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--


        #endregion
        //--------------------------------------------------------Misc Methods:---------------------------------------------------------------\\
        #region --Misc Methods (Public)--
        public string getIdAndDomain()
        {
            return user.getIdAndDomain();
        }

        public string getIdDomainAndResource()
        {
            return user.getIdDomainAndResource();
        }

        public override bool Equals(object obj)
        {
            if (obj is XMPPAccount)
            {
                XMPPAccount o = obj as XMPPAccount;
                return o.disabled == disabled && o.port == port && o.presencePriorety == presencePriorety && string.Equals(o.serverAddress, serverAddress) && XMPPUser.Equals(o.user, user);
            }
            return false;
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
