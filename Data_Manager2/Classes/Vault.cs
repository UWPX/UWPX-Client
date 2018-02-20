using Logging;
using System;
using Windows.Security.Credentials;
using XMPP_API.Classes.Network;

namespace Data_Manager2.Classes
{
    class Vault
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        private const string VAULT_NAME_PREFIX = "XMPP_LOGIN_DATA_VAULT_2_";

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


        #endregion
        //--------------------------------------------------------Misc Methods:---------------------------------------------------------------\\
        #region --Misc Methods (Public)--
        /// <summary>
        /// Tries to load the password for the given XMPPAccount and sets the password property.
        /// If no password got found, it will save an empty string as the password property.
        /// </summary>
        /// <param name="account">The Account, the password should get loaded for.</param>
        public static void loadPassword(XMPPAccount account)
        {
            PasswordVault vault = new PasswordVault();
            string vaultName = VAULT_NAME_PREFIX + account.getIdAndDomain();
            PasswordCredential passwordCredential;
            try
            {
                passwordCredential = vault.Retrieve(vaultName, account.user.userId);
            }
            catch (Exception e)
            {
                Logger.Error("Error during loadPassword - Vault", e);
                account.user.userPassword = "";
                return;
            }
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
        /// <param name="account">The Account, a password vault should get created for.</param>
        public static void storePassword(XMPPAccount account)
        {
            PasswordVault vault = new PasswordVault();
            string vaultName = VAULT_NAME_PREFIX + account.getIdAndDomain();
            vault.Add(new PasswordCredential(vaultName, account.user.userId, account.user.userPassword));
        }

        /// <summary>
        /// Deletes the password vault for the given XMPPAccount, if one exists.
        /// </summary>
        /// <param name="account">The XMPPAccount the corresponding vault should get deleted.</param>
        public static void deletePassword(XMPPAccount account)
        {
            PasswordVault vault = new PasswordVault();
            string vaultName = VAULT_NAME_PREFIX + account.getIdAndDomain();
            PasswordCredential passwordCredential = null;
            try
            {
                passwordCredential = vault.Retrieve(vaultName, account.user.userId);
            }
            catch (Exception e)
            {
                Logger.Error("Unable to delete vault!", e);
                return;
            }
            if (passwordCredential != null)
            {
                vault.Remove(passwordCredential);
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
