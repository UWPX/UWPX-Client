using System.Collections.Generic;
using System.Threading;
using Data_Manager2.Classes.DBManager.Omemo;
using Data_Manager2.Classes.DBTables;
using Data_Manager2.Classes.Events;
using Logging;
using Shared.Classes.SQLite;
using Windows.Security.Cryptography.Certificates;
using XMPP_API.Classes.Network;

namespace Data_Manager2.Classes.DBManager
{
    public class AccountDBManager: AbstractDBManager
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        public static readonly AccountDBManager INSTANCE = new AccountDBManager();
        private static readonly SemaphoreSlim ADD_ACCOUNT_SEMA = new SemaphoreSlim(1, 1);

        public delegate void AccountChangedHandler(AccountDBManager handler, AccountChangedEventArgs args);

        public event AccountChangedHandler AccountChanged;

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--


        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--
        /// <summary>
        /// Adds the given <seealso cref="XMPPAccount"/> to the db or replaces it, if it already exists.
        /// </summary>
        /// <param name="account">The account which should get inserted or replaced.</param>
        /// <param name="updateOmemoKeys">Replaces all OMEMO keys in the DB.</param>
        /// <param name="triggerAccountChanged">Triggers the <seealso cref="AccountChanged"/> event once the DB has been updated.</param>
        public void setAccount(XMPPAccount account, bool updateOmemoKeys, bool triggerAccountChanged)
        {
            dB.InsertOrReplace(new AccountTable(account));
            Vault.storePassword(account);

            saveAccountConnectionConfiguration(account);
            savePushSettings(account);

            if (updateOmemoKeys)
            {
                if (account.OMEMO_PRE_KEYS != null)
                {
                    OmemoSignalKeyDBManager.INSTANCE.setPreKeys(account.OMEMO_PRE_KEYS, account.getBareJid());
                }
                else
                {
                    OmemoSignalKeyDBManager.INSTANCE.deletePreKeys(account.getBareJid());
                }
                if (account.omemoSignedPreKey != null)
                {
                    OmemoSignalKeyDBManager.INSTANCE.setSignedPreKey(account.omemoSignedPreKeyId, account.omemoSignedPreKey, account.getBareJid());
                }
                else
                {
                    OmemoSignalKeyDBManager.INSTANCE.deleteSignedPreKey(account.omemoSignedPreKeyId, account.getBareJid());
                }
            }

            if (triggerAccountChanged)
            {
                AccountChanged?.Invoke(this, new AccountChangedEventArgs(account, false));
            }
        }

        public AccountTable getAccount(string id)
        {
            IList<AccountTable> list = dB.Query<AccountTable>(true, "SELECT * FROM " + DBTableConsts.ACCOUNT_TABLE + " WHERE id = ?;", id);
            return list.Count < 1 ? null : list[0];
        }

        /// <summary>
        /// Sets the disabled property of account and triggers the <seealso cref="AccountChanged"/> event.
        /// </summary>
        /// <param name="account">The <seealso cref="XMPPAccount"/> with updated disabled property.</param>
        public void setAccountDisabled(XMPPAccount account)
        {
            dB.Execute("UPDATE " + DBTableConsts.ACCOUNT_TABLE + " SET disabled = ? WHERE id = ?;", account.disabled, account.getBareJid());
            AccountChanged?.Invoke(this, new AccountChangedEventArgs(account, false));
        }

        /// <summary>
        /// Returns the <seealso cref="ConnectionOptionsTable"/> matching the given accountId.
        /// </summary>
        /// <param name="accountId">The id of the <seealso cref="AccountTable"/>.</param>
        public ConnectionOptionsTable getConnectionOptionsTable(string accountId)
        {
            IList<ConnectionOptionsTable> list = dB.Query<ConnectionOptionsTable>(true, "SELECT * FROM " + DBTableConsts.CONNECTION_OPTIONS_TABLE + " WHERE accountId = ?;", accountId);
            if (list.Count < 1)
            {
                return null;
            }
            else
            {
                return list[0];
            }
        }

        /// <summary>
        /// Returns a list of <seealso cref="IgnoredCertificateErrorTable"/> object matching the given accountId.
        /// </summary>
        /// <param name="accountId">The id of the <seealso cref="AccountTable"/>.</param>
        public IList<IgnoredCertificateErrorTable> getIgnoredCertificateErrorTables(string accountId)
        {
            return dB.Query<IgnoredCertificateErrorTable>(true, "SELECT * FROM " + DBTableConsts.IGNORED_CERTIFICATE_ERROR_TABLE + " WHERE accountId = ?;", accountId);
        }

        /// <summary>
        /// Returns a list of all accounts from the DB.
        /// </summary>
        private List<AccountTable> getAccounts()
        {
            return dB.Query<AccountTable>(true, "SELECT * FROM " + DBTableConsts.ACCOUNT_TABLE + ";");
        }

        /// <summary>
        /// Returns how many accounts are currently in the DB.
        /// </summary>
        public int getAccountCount()
        {
            return getAccounts().Count;
        }

        #endregion
        //--------------------------------------------------------Misc Methods:---------------------------------------------------------------\\
        #region --Misc Methods (Public)--
        /// <summary>
        /// Deletes the given account.
        /// </summary>
        /// <param name="account">The account to delete.</param>
        /// <param name="deleteAllKeys">Whether to delete all OMEMO keys.</param>
        public void deleteAccount(XMPPAccount account, bool triggerAccountChanged, bool deleteAllKeys)
        {
            dB.Execute("DELETE FROM " + DBTableConsts.ACCOUNT_TABLE + " WHERE id = ?;", account.getBareJid());
            dB.Execute("DELETE FROM " + DBTableConsts.IGNORED_CERTIFICATE_ERROR_TABLE + " WHERE accountId = ?;", account.getBareJid());
            dB.Execute("DELETE FROM " + DBTableConsts.CONNECTION_OPTIONS_TABLE + " WHERE accountId = ?;", account.getBareJid());
            dB.Execute("DELETE FROM " + DBTableConsts.PUSH_TABLE + " WHERE accountId = ?;", account.getBareJid());
            if (deleteAllKeys)
            {
                OmemoDeviceDBManager.INSTANCE.deleteAllForAccount(account.getBareJid());
                OmemoSignalKeyDBManager.INSTANCE.deleteAllForAccount(account.getBareJid());
            }
            else
            {
                OmemoSignalKeyDBManager.INSTANCE.deletePreKeys(account.getBareJid());
                OmemoSignalKeyDBManager.INSTANCE.deleteSignedPreKey(account.omemoSignedPreKeyId, account.getBareJid());
            }
            Vault.deletePassword(account);

            if (triggerAccountChanged)
            {
                AccountChanged?.Invoke(this, new AccountChangedEventArgs(account, true));
            }
        }

        /// <summary>
        /// Returns all <seealso cref="XMPPAccount"/> from the db.
        /// Loads all passwords from their corresponding Vault objects.
        /// </summary>
        /// <returns>A list of <seealso cref="XMPPAccount"/> objects.</returns>
        public IList<XMPPAccount> loadAllAccounts()
        {
            IList<XMPPAccount> results = new List<XMPPAccount>();
            IList<AccountTable> accounts = getAccounts();
            for (int i = 0; i < accounts.Count; i++)
            {
                XMPPAccount acc = accounts[i].toXMPPAccount();
                Vault.loadPassword(acc);
                loadAccountConnectionConfiguration(acc);
                loadPushSettings(acc);
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
            ADD_ACCOUNT_SEMA.Wait();
            Logger.Debug("LOCKED replaceAccount()");
            dB.BeginTransaction();
            try
            {
                deleteAccount(oldAccount, true, false);
                setAccount(account, true, true);
            }
            catch (System.Exception e)
            {
                Logger.Error("Cough an exception in AccountDBManager.replaceAccount()", e);
            }
            finally
            {
                dB.Commit();
                ADD_ACCOUNT_SEMA.Release();
                Logger.Debug("UNLOCKED replaceAccount()");
            }
        }

        /// <summary>
        /// Loads the <seealso cref="ConnectionConfiguration"/> of the given <seealso cref="XMPPAccount"/> from the db.
        /// </summary>
        /// <param name="account">The XMPPAccount you want to load the ConnectionConfiguration for.</param>
        public void loadAccountConnectionConfiguration(XMPPAccount account)
        {
            // Load general options:
            ConnectionOptionsTable optionsTable = getConnectionOptionsTable(account.getBareJid());
            if (optionsTable != null)
            {
                optionsTable.toConnectionConfiguration(account.connectionConfiguration);
            }

            // Load ignored certificate errors:
            IList<IgnoredCertificateErrorTable> ignoredCertificates = getIgnoredCertificateErrorTables(account.getBareJid());
            if (ignoredCertificates != null)
            {
                foreach (IgnoredCertificateErrorTable i in ignoredCertificates)
                {
                    account.connectionConfiguration.IGNORED_CERTIFICATE_ERRORS.Add(i.certificateError);
                }
            }
        }

        /// <summary>
        /// Saves the <seealso cref="ConnectionConfiguration"/> for the given <seealso cref="XMPPAccount"/> in the DB.
        /// </summary>
        /// <param name="account">The <seealso cref="XMPPAccount"/> containing the <seealso cref="ConnectionConfiguration"/> you want to save to the db.</param>
        public void saveAccountConnectionConfiguration(XMPPAccount account)
        {
            // Save general options:
            ConnectionOptionsTable optionsTable = new ConnectionOptionsTable(account.connectionConfiguration)
            {
                accountId = account.getBareJid()
            };
            dB.InsertOrReplace(optionsTable);

            // Save ignored certificate errors:
            dB.Execute("DELETE FROM " + DBTableConsts.IGNORED_CERTIFICATE_ERROR_TABLE + " WHERE accountId = ?;", account.getBareJid());
            foreach (ChainValidationResult i in account.connectionConfiguration.IGNORED_CERTIFICATE_ERRORS)
            {
                dB.InsertOrReplace(new IgnoredCertificateErrorTable()
                {
                    accountId = account.getBareJid(),
                    certificateError = i,
                    id = IgnoredCertificateErrorTable.generateId(account.getBareJid(), i)
                });
            }
        }

        /// <summary>
        /// Saves the push settings for the given account. If <seealso cref="XMPPAccount.pushEnabled"/> is set to false, will remove all entries from the <seealso cref="PushTable"/>.
        /// </summary>
        public void savePushSettings(XMPPAccount account)
        {
            if (account.pushEnabled)
            {
                dB.InsertOrReplace(new PushTable { accountId = account.getBareJid(), node = account.pushNode, secret = account.pushNodeSecret, published = account.pushNodePublished, serverBareJid = account.pushServerBareJid });
            }
            else
            {
                dB.Execute("DELETE FROM " + DBTableConsts.PUSH_TABLE + " WHERE accountId = ?;", account.getBareJid());
            }
        }

        /// <summary>
        /// Loads the push settings for the given account.
        /// If no configuration is available, <seealso cref="XMPPAccount.pushEnabled"/> will be set to false and <seealso cref="XMPPAccount.pushNode"/>, <seealso cref="XMPPAccount.pushServerBareJid"/> and <seealso cref="XMPPAccount.pushNodeSecret"/> will be set to null.
        /// <seealso cref="XMPPAccount.pushNodePublished"/> will be set to false.
        /// </summary>
        public void loadPushSettings(XMPPAccount account)
        {
            IList<PushTable> list = dB.Query<PushTable>(true, "SELECT * FROM " + DBTableConsts.PUSH_TABLE + " WHERE accountId = ?;", account.getBareJid());
            if (list.Count > 0)
            {
                account.pushNode = list[0].node;
                account.pushNodeSecret = list[0].secret;
                account.pushNodePublished = list[0].published;
                account.pushServerBareJid = list[0].serverBareJid;
                account.pushEnabled = true;
            }
            else
            {
                account.pushNode = null;
                account.pushNodeSecret = null;
                account.pushServerBareJid = null;
                account.pushNodePublished = false;
                account.pushEnabled = false;
            }
        }

        #endregion

        #region --Misc Methods (Private)--


        #endregion

        #region --Misc Methods (Protected)--
        protected override void CreateTables()
        {
            dB.CreateTable<AccountTable>();
            dB.CreateTable<ConnectionOptionsTable>();
            dB.CreateTable<IgnoredCertificateErrorTable>();
            dB.CreateTable<PushTable>();
        }

        protected override void DropTables()
        {
            dB.DropTable<AccountTable>();
            dB.DropTable<ConnectionOptionsTable>();
            dB.DropTable<IgnoredCertificateErrorTable>();
            dB.DropTable<PushTable>();
        }

        #endregion
        //--------------------------------------------------------Events:---------------------------------------------------------------------\\
        #region --Events--


        #endregion
    }
}
