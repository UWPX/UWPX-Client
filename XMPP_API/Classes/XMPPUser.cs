using Shared.Classes;

namespace XMPP_API.Classes
{
    public class XMPPUser: AbstractDataTemplate
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        private string _localPart;
        public string localPart
        {
            get => _localPart;
            set => SetProperty(ref _localPart, value);
        }
        private string _password;
        public string password
        {
            get => _password;
            set => SetProperty(ref _password, value);
        }
        private string _resourcePart;
        public string resourcePart
        {
            get => _resourcePart;
            set => SetProperty(ref _resourcePart, value);
        }
        private string _domainPart;
        public string domainPart
        {
            get => _domainPart;
            set => SetProperty(ref _domainPart, value);
        }

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        /// <summary>
        /// Basic Constructor
        /// </summary>
        /// <history>
        /// 17/08/2017 Created [Fabian Sauter]
        /// </history>
        public XMPPUser(string localPart, string domainPart, string resourcePart, string password)
        {
            invokeInUiThread = false;
            this.localPart = localPart;
            this.password = password;
            this.resourcePart = resourcePart;
            this.domainPart = domainPart;
        }

        public XMPPUser(string bareJid, string recourcePart)
        {
            localPart = bareJid is null ? null : Utils.getJidLocalPart(bareJid);
            domainPart = bareJid is null ? null : Utils.getJidDomainPart(bareJid);
            resourcePart = recourcePart;
            password = null;
        }

        public XMPPUser(string userId, string domain, string resource) : this(userId, domain, resource, null)
        {
        }

        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--
        public string getBareJid()
        {
            return localPart + '@' + domainPart;
        }

        public string getFullJid()
        {
            return localPart + '@' + domainPart + '/' + resourcePart ?? "";
        }

        #endregion
        //--------------------------------------------------------Misc Methods:---------------------------------------------------------------\\
        #region --Misc Methods (Public)--
        public override bool Equals(object obj)
        {
            if (obj is XMPPUser)
            {
                XMPPUser u = obj as XMPPUser;
                return string.Equals(u.domainPart, domainPart) && string.Equals(u.resourcePart, resourcePart) && string.Equals(u.localPart, localPart) && string.Equals(u.password, password);
            }
            return false;
        }

        public XMPPUser clone()
        {
            return new XMPPUser(localPart, domainPart, resourcePart)
            {
                password = password
            };
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
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
