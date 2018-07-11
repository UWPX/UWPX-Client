using Logging;
using System;
using System.Collections.Generic;
using Windows.Security.Credentials;
using XMPP_API.Classes.Network;

namespace Data_Manager2.Classes
{
    public class Vault
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        private const string VAULT_NAME_PREFIX = "XMPP_LOGIN_DATA_VAULT_2_";
        private static readonly PasswordVault PASSWORD_VAULT = new PasswordVault();

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        /// <summary>
        /// Basic Constructor
        /// </summary>
        /// <history>
        /// 17/11/2017 Created [Fabian Sauter]
        /// </history>
        public Vault()
        {

        }

        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--
        /// <summary>
        /// Returns the corresponding PasswordCredential object for the given XMPPAccount.
        /// Will return null if an error occurs or none exists.
        /// </summary>
        /// <param name="account">The XMPPAccount you want to retrieve the PasswordCredential for.</param>
        private static PasswordCredential getPasswordCredentialForAccount(XMPPAccount account)
        {
            string vaultName = VAULT_NAME_PREFIX + account.getIdAndDomain();
            try
            {
                return PASSWORD_VAULT.Retrieve(vaultName, account.user.userId);
            }
            catch (Exception)
            {
            }

            return null;
        }

        /// <summary>
        /// Returns all PasswordCredential objects stored in the vault.
        /// </summary>
        /// <returns>A read only list of PasswordCredential objects.</returns>
        public static IReadOnlyList<PasswordCredential> getAll()
        {
            try
            {
                return PASSWORD_VAULT.RetrieveAll();
            }
            catch (Exception)
            {
                return new List<PasswordCredential>();
            }
        }

        #endregion
        //--------------------------------------------------------Misc Methods:---------------------------------------------------------------\\
        #region --Misc Methods (Public)--
        /// <summary>
        /// Tries to load the password for the given XMPPAccount and sets the password property.
        /// If no password got found, an empty string will get set as the password property.
        /// </summary>
        /// <param name="account">The XMPPAccount, the password should get loaded for.</param>
        public static void loadPassword(XMPPAccount account)
        {
            PasswordCredential passwordCredential = getPasswordCredentialForAccount(account);
            if (passwordCredential == null)
            {
                Logger.Warn("No password found for: " + account.user.getIdAndDomain());
                account.user.userPassword = "";
                return;
            }
            passwordCredential.RetrievePassword();
            account.user.userPassword = passwordCredential.Password;
        }

        /// <summary>
        /// Creates a secure password vault for the given account and stores the password in it.
        /// </summary>
        /// <param name="account">The XMPPAccount a password vault should get created for.</param>
        public static void storePassword(XMPPAccount account)
        {
            // Delete existing password vaults:
            deletePassword(account);

            //removeAll();

            // Store the new password:
            if(!string.IsNullOrEmpty(account.user.userPassword))
            {
                string vaultName = VAULT_NAME_PREFIX + account.getIdAndDomain();
                PASSWORD_VAULT.Add(new PasswordCredential(vaultName, account.user.userId, account.user.userPassword));
            }
        }

        /// <summary>
        /// Deletes the password vault for the given XMPPAccount, if one exists.
        /// </summary>
        /// <param name="account">The XMPPAccount for the corresponding vault that should get deleted.</param>
        public static void deletePassword(XMPPAccount account)
        {
            PasswordCredential passwordCredential = getPasswordCredentialForAccount(account);
            deletePassword(passwordCredential);
        }

        /// <summary>
        /// Deletes the given password vault.
        /// </summary>
        /// <param name="passwordCredential">The PasswordCredential that should get deleted.</param>
        public static void deletePassword(PasswordCredential passwordCredential)
        {
            if (passwordCredential != null)
            {
                PASSWORD_VAULT.Remove(passwordCredential);
            }
        }

        /// <summary>
        /// Deletes all vaults.
        /// </summary>
        public static void deleteAllVaults()
        {
            foreach (var item in PASSWORD_VAULT.RetrieveAll())
            {
                PASSWORD_VAULT.Remove(item);
            }
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
