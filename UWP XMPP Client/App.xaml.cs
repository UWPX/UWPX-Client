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

namespace UWP_XMPP_Client
{
    sealed partial class App : Application
    {
        public App()
        {
            //Crash reports capturing:
            if (!Settings.getSettingBoolean(SettingsConsts.DISABLE_CRASH_REPORTING))
            {
                HockeyClient.Current.Configure("6e35320f3a4142f28060011b25e36f24");
            }

            this.InitializeComponent();
            this.Suspending += OnSuspending;

            // Perform App update tasks if necessary:
            AppUpdateHandler.onAppStart();
        }

        /// <summary>
        /// Wird aufgerufen, wenn die Anwendung durch den Endbenutzer normal gestartet wird. Weitere Einstiegspunkte
        /// werden z. B. verwendet, wenn die Anwendung gestartet wird, um eine bestimmte Datei zu öffnen.
        /// </summary>
        /// <param name="e">Details über Startanforderung und -prozess.</param>
        protected override void OnLaunched(LaunchActivatedEventArgs args)
        {
            onActivatedOrLaunched(args);
        }

        /// <summary>
        /// Wird aufgerufen, wenn die Navigation auf eine bestimmte Seite fehlschlägt
        /// </summary>
        /// <param name="sender">Der Rahmen, bei dem die Navigation fehlgeschlagen ist</param>
        /// <param name="e">Details über den Navigationsfehler</param>
        void OnNavigationFailed(object sender, NavigationFailedEventArgs e)
        {
            throw new Exception("Failed to load Page " + e.SourcePageType.FullName);
        }

        /// <summary>
        /// Wird aufgerufen, wenn die Ausführung der Anwendung angehalten wird.  Der Anwendungszustand wird gespeichert,
        /// ohne zu wissen, ob die Anwendung beendet oder fortgesetzt wird und die Speicherinhalte dabei
        /// unbeschädigt bleiben.
        /// </summary>
        /// <param name="sender">Die Quelle der Anhalteanforderung.</param>
        /// <param name="e">Details zur Anhalteanforderung.</param>
        private void OnSuspending(object sender, SuspendingEventArgs e)
        {
            var deferral = e.SuspendingOperation.GetDeferral();
            // TODO re-implement transfer socket ownership:
            //await ConnectionHandler.INSTANCE.transferSocketOwnershipAsync();
            deferral.Complete();
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

        protected override void OnActivated(IActivatedEventArgs args)
        {
            onActivatedOrLaunched(args);
        }

        private void onActivatedOrLaunched(IActivatedEventArgs args)
        {
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
    }
}
