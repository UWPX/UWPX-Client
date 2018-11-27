namespace XMPP_API.Classes
{
    public class XMPPUser
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        public string userId;
        public string userPassword;
        public string resource;
        public string domain;

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        /// <summary>
        /// Basic Constructor
        /// </summary>
        /// <history>
        /// 17/08/2017 Created [Fabian Sauter]
        /// </history>
        public XMPPUser(string userId, string userPassword, string domain, string resource)
        {
            this.userId = userId;
            this.userPassword = userPassword;
            this.resource = resource;
            this.domain = domain;
        }

        public XMPPUser(string userIDAndDomain, string recource)
        {
            this.userId = userIDAndDomain is null ? null : userIDAndDomain.Substring(0, userIDAndDomain.IndexOf('@'));
            this.domain = userIDAndDomain is null ? null : userIDAndDomain.Substring(userIDAndDomain.IndexOf('@') + 1);
            this.resource = recource;
            this.userPassword = null;
        }

        public XMPPUser(string userId, string domain, string resource) : this(userId, null, domain, resource)
        {
        }

        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--
        public string getIdAndDomain()
        {
            return userId + '@' + domain;
        }

        public string getIdDomainAndResource()
        {
            return userId + '@' + domain + '/' + resource ?? "";
        }

        #endregion
        //--------------------------------------------------------Misc Methods:---------------------------------------------------------------\\
        #region --Misc Methods (Public)--
        public override bool Equals(object obj)
        {
            if(obj is XMPPUser)
            {
                XMPPUser u = obj as XMPPUser;
                return string.Equals(u.domain, domain) && string.Equals(u.resource, resource) && string.Equals(u.userId, userId) && string.Equals(u.userPassword, userPassword);
            }
            return false;
        }

        public XMPPUser clone()
        {
            return new XMPPUser(userId, domain, resource)
            {
                userPassword = userPassword
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
