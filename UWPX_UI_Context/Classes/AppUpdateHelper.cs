using Data_Manager2.Classes;
using Data_Manager2.Classes.DBManager;
using Data_Manager2.Classes.DBTables;
using Data_Manager2.Classes.DBTables.Omemo;
using Logging;
using Shared.Classes.SQLite;
using System;
using Windows.ApplicationModel;
using XMPP_API.Classes.Network;

namespace UWPX_UI_Context.Classes
{
    public static class AppUpdateHelper
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--


        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--


        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--
        /// <summary>
        /// Returns the current package version.
        /// </summary>
        /// <returns>The current package version.</returns>
        private static PackageVersion GetPackageVersion()
        {
            return Package.Current.Id.Version;
        }

        /// <summary>
        /// Saves the given package version in the application storage.
        /// </summary>
        /// <param name="version">The package version, that should get saved.</param>
        private static void SetVersion(PackageVersion version)
        {
            Settings.setSetting(SettingsConsts.VERSION_MAJOR, version.Major);
            Settings.setSetting(SettingsConsts.VERSION_MINOR, version.Minor);
            Settings.setSetting(SettingsConsts.VERSION_BUILD, version.Build);
            Settings.setSetting(SettingsConsts.VERSION_REVISION, version.Revision);
        }

        /// <summary>
        /// Returns the package version from the application storage.
        /// Default: major = 0, minor = 0, build = 0, revision = 0.
        /// </summary>
        /// <returns>A not null package version.</returns>
        private static PackageVersion GetLastStartedVersion()
        {
            return new PackageVersion()
            {
                Major = Settings.getSettingUshort(SettingsConsts.VERSION_MAJOR),
                Minor = Settings.getSettingUshort(SettingsConsts.VERSION_MINOR),
                Build = Settings.getSettingUshort(SettingsConsts.VERSION_BUILD),
                Revision = Settings.getSettingUshort(SettingsConsts.VERSION_REVISION),
            };
        }

        #endregion
        //--------------------------------------------------------Misc Methods:---------------------------------------------------------------\\
        #region --Misc Methods (Public)--
        /// <summary>
        /// Gets called on App start and performs update task e.g. migrate the DB to a new format.
        /// </summary>
        public static void OnAppStart()
        {
            PackageVersion versionLastStart = GetLastStartedVersion();

            // Check if version != 0.0.0.0 => first ever start of the app:
            if (!(versionLastStart.Major == versionLastStart.Minor && versionLastStart.Build == versionLastStart.Revision && versionLastStart.Minor == versionLastStart.Build && versionLastStart.Major == 0) || Settings.getSettingBoolean(SettingsConsts.INITIALLY_STARTED))
            {
                if (!Compare(versionLastStart, GetPackageVersion()))
                {
                    // Large DB changes in version 0.2.0.0:
                    if (versionLastStart.Major <= 0 && versionLastStart.Minor < 2)
                    {
                        try
                        {
                            Logger.Info("Started updating DB to version 0.2.0.0.");
                            AbstractDBManager.dB.RecreateTable<ChatTable>();
                            Logger.Info("Finished updating DB to version 0.2.0.0.");
                        }
                        catch (Exception e)
                        {
                            Logger.Error("Error during updating DB to version 0.2.0.0", e);
                        }
                    }

                    // Accounts got reset in version 0.4.0.0:
                    if (versionLastStart.Major <= 0 && versionLastStart.Minor < 4)
                    {
                        try
                        {
                            Logger.Info("Started all vaults...");
                            Vault.deleteAllVaults();
                            Logger.Info("Finished deleting all vaults. Update to version 0.4.0.0 done.");
                        }
                        catch (Exception e)
                        {
                            Logger.Error("Error during deleting all vaults for version 0.4.0.0!", e);
                        }
                    }

                    // Generate OMEMO keys and device id for each account created before version 0.9.0.0:
                    if (versionLastStart.Major <= 0 && versionLastStart.Minor < 9)
                    {
                        Logger.Info("Started generating OMEMO keys for accounts...");
                        foreach (XMPPAccount account in AccountDBManager.INSTANCE.loadAllAccounts())
                        {
                            Logger.Info("Generating OMEMO keys for: " + account.getIdAndDomain());
                            account.generateOmemoKeys();
                            AccountDBManager.INSTANCE.setAccount(account, false);
                        }
                        Logger.Info("Finished generating OMEMO keys for accounts. Update to version 0.9.0.0 done.");
                    }

                    // Drop all OMEMO  tables since they have drastically changed in 0.11.0.0:
                    if (versionLastStart.Major <= 0 && versionLastStart.Minor < 11)
                    {
                        Logger.Info("Started dropping OMEMO tables...");
                        AbstractDBManager.dB.RecreateTable<OmemoDeviceTable>();
                        AbstractDBManager.dB.RecreateTable<OmemoDeviceListSubscriptionTable>();
                        AbstractDBManager.dB.RecreateTable<OmemoPreKeyTable>();
                        AbstractDBManager.dB.RecreateTable<OmemoIdentityKeyTable>();
                        AbstractDBManager.dB.RecreateTable<OmemoSessionStoreTable>();
                        AbstractDBManager.dB.RecreateTable<OmemoSignedPreKeyTable>();
                        foreach (XMPPAccount account in AccountDBManager.INSTANCE.loadAllAccounts())
                        {
                            Logger.Info("Reseting OMEMO keys for: " + account.getIdAndDomain());
                            account.omemoBundleInfoAnnounced = false;
                            account.omemoDeviceId = 0;
                            account.omemoKeysGenerated = false;
                            account.omemoSignedPreKeyId = 0;
                            AccountDBManager.INSTANCE.setAccount(account, false);
                        }
                        Logger.Info("Finished dropping OMEMO tables. Update to version 0.11.0.0 done.");
                    }
                }
            }
            SetVersion(GetPackageVersion());
        }

        #endregion

        #region --Misc Methods (Private)--
        /// <summary>
        /// Checks if PackageVersion a is equal to PackageVersion b.
        /// </summary>
        /// <param name="a">The PackageVersion of the last app start.</param> 0.1.0.0
        /// <param name="b">The current PackageVersion.</param> 0.2.0.0
        /// <returns>Returns true, if the current PackageVersion equals the last app start PackageVersion.</returns>
        private static bool Compare(PackageVersion a, PackageVersion b)
        {
            return a.Major == b.Major && a.Minor == b.Minor && a.Build == b.Build && a.Revision == b.Revision;
        }

        #endregion

        #region --Misc Methods (Protected)--


        #endregion
        //--------------------------------------------------------Events:---------------------------------------------------------------------\\
        #region --Events--


        #endregion
    }
}
