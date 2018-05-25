using System;
using UWP_XMPP_Client.Pages;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using UWP_XMPP_Client.Classes;
using Data_Manager2.Classes;
using Data_Manager2.Classes.DBManager;
using System.Threading.Tasks;
using Logging;
using Microsoft.AppCenter.Push;
using Microsoft.AppCenter.Crashes;
using Microsoft.HockeyApp;
using UWP_XMPP_Client.Dialogs;
using Windows.ApplicationModel.Core;
using Windows.UI.Core;

namespace UWP_XMPP_Client
{
    sealed partial class App : Application
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        /// <summary>
        /// Gets or sets (with LocalSettings persistence) the RequestedTheme of the root element.
        /// </summary>
        public static ElementTheme RootTheme
        {
            get
            {
                if (Window.Current.Content is FrameworkElement rootElement)
                {
                    return rootElement.RequestedTheme;
                }

                return ElementTheme.Default;
            }
            set
            {
                if (Window.Current.Content is FrameworkElement rootElement)
                {
                    rootElement.RequestedTheme = value;
                }
                Settings.setSetting(SettingsConsts.APP_REQUESTED_THEME, value.ToString());
            }
        }

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        public App()
        {
            //Crash reports capturing:
            if (!Settings.getSettingBoolean(SettingsConsts.DISABLE_CRASH_REPORTING))
            {
                // Setup Hockey App crashes:
                // HockeyClient.Current.Configure("6e35320f3a4142f28060011b25e36f24");

                // Setup App Center crashes:
                try
                {
                    Microsoft.AppCenter.AppCenter.Start("6e35320f-3a41-42f2-8060-011b25e36f24", typeof(Crashes));
                }
                catch (Exception e)
                {
                    Logger.Error("Failed to start APPCenter!", e);
                    throw e;
                }
                Logger.Info("App Center crash reporting registered.");
            }

            // Init buy content helper:
            BuyContentHelper.INSTANCE.init();

            this.InitializeComponent();
            this.Suspending += OnSuspending;
            this.Resuming += App_Resuming;
            this.UnhandledException += App_UnhandledException;

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
        /// Sets up App Center push support.
        /// </summary>
        private void setupAppCenterPush(LaunchActivatedEventArgs args)
        {
            // Setup App Center push:
            Microsoft.AppCenter.AppCenter.Start("6e35320f-3a41-42f2-8060-011b25e36f24", typeof(Push));
            if (!Microsoft.AppCenter.AppCenter.Configured)
            {
                Push.PushNotificationReceived -= Push_PushNotificationReceived;
                Push.PushNotificationReceived += Push_PushNotificationReceived;
            }

            Logger.Info("App Center push registered.");
        }


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
                Settings.setSetting(SettingsConsts.CHAT_EXAMPLE_BACKGROUND_IMAGE_NAME, "space.jpeg");
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

                Push.CheckLaunchedFromNotification(launchActivationArgs);

                // If launched with arguments (not a normal primary tile/applist launch)
                if (launchActivationArgs.Arguments.Length > 0)
                {
                    Logger.Debug(launchActivationArgs.Arguments);
                    // TODO: Handle arguments for cases = launching from secondary Tile, so we navigate to the correct page
                    //throw new NotImplementedException();
                }

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

            // Set requested theme:
            string themeString = Settings.getSettingString(SettingsConsts.APP_REQUESTED_THEME);
            ElementTheme theme = ElementTheme.Dark;
            if (themeString != null)
            {
                bool b = Enum.TryParse(themeString, out theme);
            }
            RootTheme = theme;

            Window.Current.Activate();

            // Connect to all clients:
            ConnectionHandler.INSTANCE.connectAll();
        }

        #endregion

        #region --Misc Methods (Protected)--
        protected override void OnLaunched(LaunchActivatedEventArgs args)
        {
            onActivatedOrLaunched(args);

            setupAppCenterPush(args);
        }

        protected override void OnActivated(IActivatedEventArgs args)
        {
            onActivatedOrLaunched(args);
        }

        #endregion
        //--------------------------------------------------------Events:---------------------------------------------------------------------\\
        #region --Events--
        private async void OnSuspending(object sender, SuspendingEventArgs e)
        {
            var deferral = e.SuspendingOperation.GetDeferral();
            // TODO re-implement transfer socket ownership:
            //await ConnectionHandler.INSTANCE.transferSocketOwnershipAsync();

            // Disconnect all clients:
            await ConnectionHandler.INSTANCE.disconnectAllAsync();
            deferral.Complete();
        }

        private void App_Resuming(object sender, object e)
        {
            // Connect to all clients:
            ConnectionHandler.INSTANCE.connectAll();
        }

        void OnNavigationFailed(object sender, NavigationFailedEventArgs e)
        {
            throw new Exception("Failed to load Page " + e.SourcePageType.FullName);
        }

        private async void Push_PushNotificationReceived(object sender, PushNotificationReceivedEventArgs e)
        {
            // Add the notification message and title to the message:
            string pushSummary = $"Push notification received:" +
                                $"\n\tNotification title: {e.Title}" +
                                $"\n\tMessage: {e.Message}";

            // If there is custom data associated with the notification, print the entries:
            if (e.CustomData != null)
            {
                pushSummary += "\n\tCustom data:\n";
                foreach (var key in e.CustomData.Keys)
                {
                    pushSummary += $"\t\t{key} : {e.CustomData[key]}\n";
                }
            }

            // Log notification summary:
            Logger.Info(pushSummary);

            // Show push dialog:
            if (e.CustomData.TryGetValue("markdown", out string markdownText))
            {
                await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, async () =>
                {
                    AppCenterPushDialog dialog = new AppCenterPushDialog(e.Title, markdownText);
                    await UiUtils.showDialogAsyncQueue(dialog);
                });
            }
        }

        private void App_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            Logger.Error("Unhanded exception: ", e.Exception);
        }

        #endregion
    }
}
