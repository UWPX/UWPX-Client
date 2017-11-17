using Data_Manager2.Classes.DBTables;
using System.Collections.Generic;
using XMPP_API.Classes.Network;

namespace Data_Manager2.Classes.DBManager
{
    class AccountManager : AbstractManager
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        public static AccountManager INSTANCE = new AccountManager();

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        /// <summary>
        /// Basic Constructor
        /// </summary>
        /// <history>
        /// 17/11/2017 Created [Fabian Sauter]
        /// </history>
        public AccountManager()
        {

        }

        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--
        /// <summary>
        /// Adds the given XMPPAccount to the db or replaces it, if it already exists.
        /// </summary>
        /// <param name="account">The account which should get inserted or replaced.</param>
        public void setAccount(XMPPAccount account)
        {
            update(new AccountTable(account));
            Vault.storePassword(account);
        }

        #endregion
        //--------------------------------------------------------Misc Methods:---------------------------------------------------------------\\
        #region --Misc Methods (Public)--
        /// <summary>
        /// Deletes the given account.
        /// </summary>
        /// <param name="account">The account to delete.</param>
        public void deleteAccount(XMPPAccount account)
        {
            dB.Execute("DELETE FROM AccountTable WHERE id LIKE ?;", account.getIdAndDomain());
            Vault.deletePassword(account);
        }

        /// <summary>
        /// Returns all XMPPAccounts from the db.
        /// Loads all passwords from their corresponding Vault objects.
        /// </summary>
        /// <returns>A list of XMPPAccount.</returns>
        public IList<XMPPAccount> loadAllAccounts()
        {
            IList<XMPPAccount> results = new List<XMPPAccount>();
            IList<AccountTable> accounts = dB.Query<AccountTable>("SELECT * FROM AccountTable;");
            for (int i = 0; i < accounts.Count; i++)
            {
                XMPPAccount acc = accounts[i].toXMPPAccount();
                Vault.loadPassword(acc);
                results.Add(acc);
            }
            return results;
        }

        #endregion

        #region --Misc Methods (Private)--


        #endregion

        #region --Misc Methods (Protected)--
        protected override void createTables()
        {
            dB.CreateTable<AccountTable>();
        }

        protected override void dropTables()
        {
            dB.DropTable<AccountTable>();
        }

        #endregion
        //--------------------------------------------------------Events:---------------------------------------------------------------------\\
        #region --Events--


        #endregion
    }
}
