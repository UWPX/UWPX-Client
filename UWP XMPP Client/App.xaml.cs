using System;
using UWP_XMPP_Client.Pages;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using UWP_XMPP_Client.Classes;
using Microsoft.HockeyApp;
using Data_Manager2.Classes;
using Data_Manager2.Classes.DBManager;
using System.Threading.Tasks;
using Logging;
using Windows.ApplicationModel.Store;

namespace UWP_XMPP_Client
{
    sealed partial class App : Application
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        private LicenseInformation licenseInformation;

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        public App()
        {
            //Crash reports capturing:
            if (!Settings.getSettingBoolean(SettingsConsts.DISABLE_CRASH_REPORTING))
            {
                HockeyClient.Current.Configure("6e35320f3a4142f28060011b25e36f24");
            }

#if DEBUG
            licenseInformation = CurrentAppSimulator.LicenseInformation;
#else
            licenseInformation = CurrentApp.LicenseInformation;
#endif

            this.InitializeComponent();
            this.Suspending += OnSuspending;

            // Perform App update tasks if necessary:
            AppUpdateHandler.onAppStart();
        }


        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--


        #endregion
        //--------------------------------------------------------Misc Methods:---------------------------------------------------------------\\
        #region --Misc Methods (Public)--


        #endregion

        #region --Misc Methods (Private)--
        /// <summary>
        /// Inits all db managers in a new task to force event subscriptions.
        /// </summary>
        private void initAllDBManagers()
        {
            Task.Run(() =>
            {
                AccountDBManager.INSTANCE.initManager();
                ChatDBManager.INSTANCE.initManager();
                DiscoDBManager.INSTANCE.initManager();
                ImageDBManager.INSTANCE.initManager();
                MUCDBManager.INSTANCE.initManager();
            });
        }

        /// <summary>
        /// Sets the log level for the logger class.
        /// </summary>
        private void initLogLevel()
        {
            object o = Settings.getSetting(SettingsConsts.LOG_LEVEL);
            if (o is int)
            {
                Logger.logLevel = (LogLevel)o;
            }
            else
            {
                Settings.setSetting(SettingsConsts.LOG_LEVEL, (int)LogLevel.INFO);
                Logger.logLevel = LogLevel.INFO;
            }
        }

        private void onActivatedOrLaunched(IActivatedEventArgs args)
        {
            // Sets the log level:
            initLogLevel();

            // Init all db managers to force event subscriptions:
            initAllDBManagers();

            // Set default background:
            if (!Settings.getSettingBoolean(SettingsConsts.INITIALLY_STARTED))
            {
                Settings.setSetting(SettingsConsts.CHAT_BACKGROUND_IMAGE_NAME, "space.jpeg");
            }
            // Loads all background images into the cache:
            BackgroundImageCache.loadCache();

            // Setup push server connection:
            if (!Settings.getSettingBoolean(SettingsConsts.DISABLE_PUSH))
            {
                Push_App_Server.Classes.PushManager.init();
            }

            Frame rootFrame = Window.Current.Content as Frame;

            // Do not repeat app initialization when the Window already has content,
            // just ensure that the window is active
            if (rootFrame == null)
            {
                // Create a Frame to act as the navigation context and navigate to the first page:
                rootFrame = new Frame();

                rootFrame.NavigationFailed += OnNavigationFailed;

                if (args.PreviousExecutionState == ApplicationExecutionState.Terminated)
                {
                    // TODO: Load state from previously suspended application
                }

                // Place the frame in the current Window
                Window.Current.Content = rootFrame;
            }

            if (args is ToastNotificationActivatedEventArgs)
            {
                var toastActivationArgs = args as ToastNotificationActivatedEventArgs;

                // If empty args, no specific action (just launch the app)
                if (string.IsNullOrEmpty(toastActivationArgs.Argument))
                {
                    if (rootFrame.Content == null)
                    {
                        if (!Settings.getSettingBoolean(SettingsConsts.INITIALLY_STARTED))
                        {
                            rootFrame.Navigate(typeof(AddAccountPage), "App.xaml.cs");
                        }
                        else
                        {
                            rootFrame.Navigate(typeof(ChatPage), "App.xaml.cs");
                        }
                    }
                }
                else
                {
                    rootFrame.Navigate(typeof(ChatPage), args);
                }
                if (rootFrame.BackStack.Count == 0)
                {
                    rootFrame.BackStack.Add(new PageStackEntry(typeof(ChatPage), null, null));
                }
            }
            else if (args is LaunchActivatedEventArgs)
            {
                var launchActivationArgs = args as LaunchActivatedEventArgs;

                // If launched with arguments (not a normal primary tile/applist launch)
                if (launchActivationArgs.Arguments.Length > 0)
                {
                    // TODO: Handle arguments for cases = launching from secondary Tile, so we navigate to the correct page
                    throw new NotImplementedException();
                }

                // Otherwise if launched normally
                else
                {
                    // If we're currently not on a page, navigate to the main page
                    if (rootFrame.Content == null)
                    {
                        if (!Settings.getSettingBoolean(SettingsConsts.INITIALLY_STARTED))
                        {
                            rootFrame.Navigate(typeof(AddAccountPage), "App.xaml.cs");
                        }
                        else
                        {
                            rootFrame.Navigate(typeof(ChatPage), "App.xaml.cs");
                        }
                    }
                }
            }
            Window.Current.Activate();
        }

        #endregion

        #region --Misc Methods (Protected)--
        protected override void OnLaunched(LaunchActivatedEventArgs args)
        {
            onActivatedOrLaunched(args);
        }

        protected override void OnActivated(IActivatedEventArgs args)
        {
            onActivatedOrLaunched(args);
        }

        #endregion
        //--------------------------------------------------------Events:---------------------------------------------------------------------\\
        #region --Events--
        private void OnSuspending(object sender, SuspendingEventArgs e)
        {
            var deferral = e.SuspendingOperation.GetDeferral();
            // TODO re-implement transfer socket ownership:
            //await ConnectionHandler.INSTANCE.transferSocketOwnershipAsync();
            deferral.Complete();
        }

        void OnNavigationFailed(object sender, NavigationFailedEventArgs e)
        {
            throw new Exception("Failed to load Page " + e.SourcePageType.FullName);
        }

        #endregion
    }
}
