using Data_Manager2.Classes;
using Data_Manager2.Classes.DBManager;
using Data_Manager2.Classes.DBTables;
using System;
using Windows.ApplicationModel;

namespace UWP_XMPP_Client.Classes
{
    class AppUpdateHandler
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--


        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        /// <summary>
        /// Basic Constructor
        /// </summary>
        /// <history>
        /// 17/01/2018 Created [Fabian Sauter]
        /// </history>
        public AppUpdateHandler()
        {

        }

        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--
        /// <summary>
        /// Returns the current package version.
        /// </summary>
        /// <returns>The current package version.</returns>
        private PackageVersion getPackageVersion()
        {
            return Package.Current.Id.Version;
        }

        /// <summary>
        /// Saves the given package version in the application storage.
        /// </summary>
        /// <param name="version">The package version, that should get saved.</param>
        private void setVersion(PackageVersion version)
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
        private PackageVersion getLastStartedVersion()
        {
            return new PackageVersion()
            {
                Major = Settings.getSettingUshort(SettingsConsts.VERSION_MAJOR),
                Minor = Settings.getSettingUshort(SettingsConsts.VERSION_MINOR),
                Build = Settings.getSettingUshort(SettingsConsts.VERSION_BUILD),
                Revision = Settings.getSettingUshort(SettingsConsts.VERSION_REVISION),
            };
        }

        /// <summary>
        /// Checks if PackageVersion a is equal to PackageVersion b.
        /// </summary>
        /// <param name="a">The PackageVersion of the last app start.</param> 0.1.0.0
        /// <param name="b">The current PackageVersion.</param> 0.2.0.0
        /// <returns>Returns true, if the current PackageVersion equals the last app start PackageVersion.</returns>
        private bool compare(PackageVersion a, PackageVersion b)
        {
            return a.Major == b.Major && a.Minor == b.Minor && a.Build == b.Build && a.Revision == b.Revision;
        }

        #endregion
        //--------------------------------------------------------Misc Methods:---------------------------------------------------------------\\
        #region --Misc Methods (Public)--
        /// <summary>
        /// Gets called on App start and performs update task e.g. migrate the DB to a new format.
        /// </summary>
        public static void onAppStart()
        {
            AppUpdateHandler handler = new AppUpdateHandler();
            PackageVersion versionLastStart = handler.getLastStartedVersion();

            // Check if version != 0.0.0.0 => first ever start of the app:
            if(!(versionLastStart.Major == versionLastStart.Minor && versionLastStart.Build == versionLastStart.Revision && versionLastStart.Minor == versionLastStart.Build && versionLastStart.Major == 0) || Settings.getSettingBoolean(SettingsConsts.INITIALLY_STARTED))
            {
                if(!handler.compare(versionLastStart, handler.getPackageVersion()))
                {
                    // Large db changes on version 0.2.0.0
                    if(versionLastStart.Major <= 0 && versionLastStart.Minor < 2)
                    {
                        try
                        {
                            Logging.Logger.Info("Started updating db to version 0.2.0.0.");
                            AbstractManager.dB.RecreateTable<ChatTable>();
                            Logging.Logger.Info("Finished updating db to version 0.2.0.0.");
                        }
                        catch (Exception e)
                        {
                            Logging.Logger.Error("Error during updating db to version 0.2.0.0", e);
                        }
                    }
                }
            }
            handler.setVersion(handler.getPackageVersion());
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
