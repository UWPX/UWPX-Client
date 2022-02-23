using System;
using System.Threading.Tasks;
using Logging;
using Storage.Classes;
using Storage.Classes.Contexts;
using Storage.Classes.Migrations;
using Windows.ApplicationModel;
using Windows.Storage;

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
            Settings.SetSetting(SettingsConsts.VERSION_MAJOR, version.Major);
            Settings.SetSetting(SettingsConsts.VERSION_MINOR, version.Minor);
            Settings.SetSetting(SettingsConsts.VERSION_BUILD, version.Build);
            Settings.SetSetting(SettingsConsts.VERSION_REVISION, version.Revision);
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
                Major = Settings.GetSettingUshort(SettingsConsts.VERSION_MAJOR),
                Minor = Settings.GetSettingUshort(SettingsConsts.VERSION_MINOR),
                Build = Settings.GetSettingUshort(SettingsConsts.VERSION_BUILD),
                Revision = Settings.GetSettingUshort(SettingsConsts.VERSION_REVISION),
            };
        }

        #endregion
        //--------------------------------------------------------Misc Methods:---------------------------------------------------------------\\
        #region --Misc Methods (Public)--
        /// <summary>
        /// Gets called on App start and performs update task e.g. migrate the DB to a new format.
        /// </summary>
        public static async Task OnAppStartAsync()
        {
            // For debugging:
            /*using (MainDbContext ctx = new MainDbContext())
            {
                if (ctx.Accounts.Count() <= 0)
                {
                    await ctx.RecreateDb();
                }
            }*/
            PackageVersion versionLastStart = GetLastStartedVersion();

            // Check if version != 0.0.0.0 => first ever start of the App:
            if (!(versionLastStart.Major == 0 && versionLastStart.Major == versionLastStart.Minor && versionLastStart.Minor == versionLastStart.Revision && versionLastStart.Revision == versionLastStart.Build) || Settings.GetSettingBoolean(SettingsConsts.INITIALLY_STARTED))
            {
                if (!Compare(versionLastStart, GetPackageVersion()))
                {
                    if (versionLastStart.Major <= 0 && versionLastStart.Minor < 31)
                    {
                        try
                        {
                            Logger.Info("Started updating DB to version 0.31.0.0.");
                            Logger.Info("Clearing old application data...");
                            await ApplicationData.Current.ClearAsync();
                            Logger.Info("Old application data cleared.");
                            Logger.Info("Finished updating DB to version 0.31.0.0.");
                        }
                        catch (Exception e)
                        {
                            Logger.Error("Error during updating DB to version 0.31.0.0", e);
                        }
                    }

                    if (versionLastStart.Major <= 0 && versionLastStart.Minor < 38)
                    {
                        try
                        {
                            Logger.Info("Started updating DB to version 0.38.0.0.");
                            Logger.Info("Clearing old application data...");
                            await ApplicationData.Current.ClearAsync();
                            Logger.Info("Old application data cleared.");
                            Logger.Info("Finished updating DB to version 0.38.0.0.");
                        }
                        catch (Exception e)
                        {
                            Logger.Error("Error during updating DB to version 0.38.0.0", e);
                        }
                    }

                    if (versionLastStart.Major <= 0 && versionLastStart.Minor < 39)
                    {
                        try
                        {
                            Logger.Info("Started updating DB to version 0.39.0.0.");
                            Logger.Info("Resetting the DB...");
                            using (MainDbContext ctx = new MainDbContext())
                            {
                                await ctx.RecreateDb();
                            }
                            Logger.Info("DB reset.");
                            Logger.Info("Finished updating DB to version 0.39.0.0.");
                        }
                        catch (Exception e)
                        {
                            Logger.Error("Error during updating DB to version 0.39.0.0", e);
                        }
                    }

                    if (versionLastStart.Major <= 0 && versionLastStart.Minor < 41)
                    {
                        try
                        {
                            Logger.Info("Started updating DB to version 0.41.0.0.");
                            Logger.Info("Resetting the DB...");
                            using (MainDbContext ctx = new MainDbContext())
                            {
                                if (!ctx.ApplyMigration(new Migration_0_41_0_0()))
                                {
#if !DEBUG
                                    Logger.Error("Since migration to the new DB failed for version 0.41.0.0, we are recreating the DB");
                                    await ctx.RecreateDb();
#endif
                                }
                            }
                            Logger.Info("DB reset.");
                            Logger.Info("Finished updating DB to version 0.41.0.0.");
                        }
                        catch (Exception e)
                        {
                            Logger.Error("Error during updating DB to version 0.41.0.0", e);
                        }
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
        /// <param name="a">The PackageVersion of the last App start.</param> 0.1.0.0
        /// <param name="b">The current PackageVersion.</param> 0.2.0.0
        /// <returns>Returns true, if the current PackageVersion equals the last App start PackageVersion.</returns>
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
