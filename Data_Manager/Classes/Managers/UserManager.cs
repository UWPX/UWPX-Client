using Data_Manager.Classes.DBEntries;
using System.Collections.Generic;
using XMPP_API.Classes;
using XMPP_API.Classes.Network;

namespace Data_Manager.Classes.Managers
{
    public class UserManager : AbstractManager
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        public static readonly UserManager INSTANCE = new UserManager();

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        /// <summary>
        /// Basic Constructor
        /// </summary>
        /// <history>
        /// 26/08/2017 Created [Fabian Sauter]
        /// </history>
        public UserManager()
        {

        }

        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--
        /// <summary>
        /// Adds a given XMPPUser to the db or replaces him, if he already exists.
        /// </summary>
        /// <param name="user">The User to insert or replace.</param>
        public void setUser(XMPPUser user, XMPPAccount account)
        {
            update(new UserEntry(user, account));
        }

        /// <summary>
        /// Adds a given ServerConnectionConfiguration to the db or replaces him, if he already exists.
        /// Passwords get stored Vault objects.
        /// </summary>
        /// <param name="account">The ServerConnectionConfiguration to insert or replace.</param>
        public void setAccount(XMPPAccount account)
        {
            update(new UserAccountEntry(account));
            Vault.storePassword(account);
        }

        /// <summary>
        /// Replaces the given account in the db.
        /// </summary>
        /// <param name="oldAccount">The old account.</param>
        /// <param name="newAccount">The new account.</param>
        public void replaceAccount(XMPPAccount oldAccount, XMPPAccount newAccount)
        {
            deleteAccount(oldAccount);
            setAccount(newAccount);
        }

        /// <summary>
        /// Deletes the given account.
        /// </summary>
        /// <param name="account">The account to delete.</param>
        public void deleteAccount(XMPPAccount account)
        {
            dB.Query<UserAccountEntry>("DELETE FROM UserAccountEntry WHERE userAccountEntryId LIKE ?", account.getIdAndDomain());
            Vault.deletePassword(account);
        }

        /// <summary>
        /// Returns all ServerConnectionConfiguration from the db.
        /// It also loads all passwords from their Vault objects.
        /// </summary>
        /// <returns>A list of ServerConnectionConfiguration.</returns>
        public IList<XMPPAccount> getAccounts()
        {
            IList<XMPPAccount> results = new List<XMPPAccount>();
            IList<UserAccountEntry> accounts = dB.Query<UserAccountEntry>("SELECT * FROM UserAccountEntry");
            for (int i = 0; i < accounts.Count; i++)
            {
                XMPPAccount acc = accounts[i].toServerConnectionConfiguration();
                Vault.loadPassword(acc);
                results.Add(acc);
            }
            return results;
        }

        /// <summary>
        /// Returns all XMPPUsers that match to the given account from the db.
        /// </summary>
        /// <returns>A IList of XMPPUsers.</returns>
        public IList<XMPPUser> getUsersForAccount(XMPPAccount account)
        {
            IList<UserEntry> list = dB.Query<UserEntry>("SELECT * FROM UserEntry WHERE userAccountEntryId LIKE ?", account.getIdAndDomain());
            List<XMPPUser> result = new List<XMPPUser>();
            foreach (UserEntry user in list)
            {
                result.Add(user.toXMPPUser());
            }
            return result;
        }

        #endregion
        //--------------------------------------------------------Misc Methods:---------------------------------------------------------------\\
        #region --Misc Methods (Public)--


        #endregion

        #region --Misc Methods (Private)--


        #endregion

        #region --Misc Methods (Protected)--
        protected override void createTables()
        {
            dB.CreateTable<UserAccountEntry>();
        }

        protected override void dropTables()
        {
            dB.DropTable<UserAccountEntry>();
        }
        #endregion
        //--------------------------------------------------------Events:---------------------------------------------------------------------\\
        #region --Events--


        #endregion
    }
}
