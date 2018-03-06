using Data_Manager2.Classes.Events;
using Data_Manager2.Classes.DBTables;
using System.Collections.Generic;
using XMPP_API.Classes.Network;

namespace Data_Manager2.Classes.DBManager
{
    public class AccountDBManager : AbstractDBManager
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        public static AccountDBManager INSTANCE = new AccountDBManager();

        public delegate void AccountChangedHandler(AccountDBManager handler, AccountChangedEventArgs args);

        public event AccountChangedHandler AccountChanged;

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        /// <summary>
        /// Basic Constructor
        /// </summary>
        /// <history>
        /// 17/11/2017 Created [Fabian Sauter]
        /// </history>
        public AccountDBManager()
        {

        }

        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--
        /// <summary>
        /// Adds the given XMPPAccount to the db or replaces it, if it already exists.
        /// </summary>
        /// <param name="account">The account which should get inserted or replaced.</param>
        public void setAccount(XMPPAccount account, bool triggerAccountChanged)
        {
            update(new AccountTable(account));
            Vault.storePassword(account);
            if (triggerAccountChanged)
            {
                AccountChanged?.Invoke(this, new AccountChangedEventArgs(account, false));
            }
        }

        /// <summary>
        /// Sets the disabled property of account and triggers the AccountChanged event.
        /// </summary>
        /// <param name="id">The AccountTable id</param>
        /// <param name="disabled">Account disable true or false.</param>
        public void setAccountDisabled(XMPPAccount account, bool disabled)
        {
            dB.Execute("UPDATE " + DBTableConsts.ACCOUNT_TABLE + " SET disabled = ? WHERE id = ?;", disabled, account.getIdAndDomain());
            AccountChanged?.Invoke(this, new AccountChangedEventArgs(account, false));
        }

        #endregion
        //--------------------------------------------------------Misc Methods:---------------------------------------------------------------\\
        #region --Misc Methods (Public)--
        /// <summary>
        /// Deletes the given account.
        /// </summary>
        /// <param name="account">The account to delete.</param>
        public void deleteAccount(XMPPAccount account, bool triggerAccountChanged)
        {
            dB.Execute("DELETE FROM " + DBTableConsts.ACCOUNT_TABLE + " WHERE id = ?;", account.getIdAndDomain());
            Vault.deletePassword(account);
            if (triggerAccountChanged)
            {
                AccountChanged?.Invoke(this, new AccountChangedEventArgs(account, true));
            }
        }

        /// <summary>
        /// Returns all XMPPAccounts from the db.
        /// Loads all passwords from their corresponding Vault objects.
        /// </summary>
        /// <returns>A list of XMPPAccount.</returns>
        public IList<XMPPAccount> loadAllAccounts()
        {
            IList<XMPPAccount> results = new List<XMPPAccount>();
            IList<AccountTable> accounts = dB.Query<AccountTable>(true, "SELECT * FROM " + DBTableConsts.ACCOUNT_TABLE + ";");
            for (int i = 0; i < accounts.Count; i++)
            {
                XMPPAccount acc = accounts[i].toXMPPAccount();
                Vault.loadPassword(acc);
                results.Add(acc);
            }
            return results;
        }

        /// <summary>
        /// Deletes the old account and inserts the new account.
        /// </summary>
        /// <param name="oldAccount">The old account, which should get deleted.</param>
        /// <param name="account">The account, that should get inserted.</param>
        public void replaceAccount(XMPPAccount oldAccount, XMPPAccount account)
        {
            deleteAccount(oldAccount, true);
            setAccount(account, true);
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
