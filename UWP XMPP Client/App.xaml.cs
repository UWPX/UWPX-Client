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
using Data_Manager2.Classes.Toast;
using Windows.UI.Notifications;
using Data_Manager2.Classes.DBTables;
using System.Text;
using Microsoft.AppCenter.Analytics;

namespace UWP_XMPP_Client
{
    sealed partial class App : Application
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        private readonly string APP_CENTER_SECRET = "523e7039-f6cb-4bf1-9000-53277ed97c53";
        private readonly string HOCKEY_APP_SECRET = "6e35320f3a4142f28060011b25e36f24";

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

        private bool isRunning;

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        public App()
        {
            this.isRunning = false;

            //Crash reports capturing:
            if (!Settings.getSettingBoolean(SettingsConsts.DISABLE_CRASH_REPORTING))
            {
                // Setup Hockey App crashes:
                HockeyClient.Current.Configure(HOCKEY_APP_SECRET);

                // Setup App Center crashes, push:
                setupAppCenter();
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
        /// Sets up App Center crash and push support.
        /// </summary>
        private void setupAppCenter()
        {
            try
            {
                Microsoft.AppCenter.AppCenter.Start(APP_CENTER_SECRET, typeof(Crashes));
#if DEBUG
                Microsoft.AppCenter.AppCenter.Start(APP_CENTER_SECRET, typeof(Analytics), typeof(Push)); // Only enable analytics and push for debug builds
#endif

                if (!Microsoft.AppCenter.AppCenter.Configured)
                {
                    Push.PushNotificationReceived -= Push_PushNotificationReceived;
                    Push.PushNotificationReceived += Push_PushNotificationReceived;
                }
            }
            catch (Exception e)
            {
                Logger.Error("Failed to start APPCenter!", e);
                throw e;
            }
            Logger.Info("App Center crash reporting registered.");
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

        private async Task onActivatedOrLaunchedAsync(IActivatedEventArgs args)
        {
            // Sets the log level:
            initLogLevel();

            // Register background tasks:
            Logger.Info("Registering background tasks...");
            await BackgroundTaskHelper.registerToastBackgroundTaskAsync();
            Logger.Info("Finished registering background tasks.");

            // Init all db managers to force event subscriptions:
            initAllDBManagers();

            // Set default background:
            if (!Settings.getSettingBoolean(SettingsConsts.INITIALLY_STARTED))
            {
                Settings.setSetting(SettingsConsts.CHAT_EXAMPLE_BACKGROUND_IMAGE_NAME, "light_bulb.jpeg");
            }
            // Loads all background images into the cache:
            BackgroundImageCache.loadCache();

            // Setup push server connection:
            if (!Settings.getSettingBoolean(SettingsConsts.DISABLE_PUSH))
            {
                Push_App_Server.Classes.PushManager.init();
            }

            isRunning = true;

            // Do not repeat app initialization when the Window already has content,
            // just ensure that the window is active
            if (!(Window.Current.Content is Frame rootFrame))
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

            if(args is ProtocolActivatedEventArgs protocolActivationArgs)
            {
                Logger.Info("App activated by protocol activation with: " + protocolActivationArgs.Uri.ToString());

                // If we're currently not on a page, navigate to the main page
                if (rootFrame.Content == null)
                {
                    if (!Settings.getSettingBoolean(SettingsConsts.INITIALLY_STARTED))
                    {
                        rootFrame.Navigate(typeof(AddAccountPage), "App.xaml.cs"); // ToDo add arguments
                    }
                    else
                    {
                        rootFrame.Navigate(typeof(ChatPage), "App.xaml.cs"); // ToDo add arguments
                    }
                }
            }
            else if (args is ToastNotificationActivatedEventArgs toastActivationArgs)
            {
                Logger.Info("App activated by toast with: " + toastActivationArgs.Argument);
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
                    rootFrame.Navigate(typeof(ChatPage), ToastActivationArgumentParser.parseArguments(toastActivationArgs.Argument));
                }
                if (rootFrame.BackStack.Count == 0)
                {
                    rootFrame.BackStack.Add(new PageStackEntry(typeof(ChatPage), null, null));
                }
            }
            else if (args is LaunchActivatedEventArgs launchActivationArgs)
            {
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
                Enum.TryParse(themeString, out theme);
            }
            RootTheme = theme;

            Window.Current.Activate();

            // Connect to all clients:
            ConnectionHandler.INSTANCE.connectAll();
        }

        #endregion

        #region --Misc Methods (Protected)--
        protected override void OnBackgroundActivated(BackgroundActivatedEventArgs args)
        {
            var deferral = args.TaskInstance.GetDeferral();

            switch (args.TaskInstance.Task.Name)
            {
                case BackgroundTaskHelper.TOAST_BACKGROUND_TASK_NAME:
                    ToastNotificationActionTriggerDetail details = args.TaskInstance.TriggerDetails as ToastNotificationActionTriggerDetail;
                    if (details != null)
                    {
                        initLogLevel();

                        string arguments = details.Argument;
                        var userInput = details.UserInput;

                        Logger.Debug("App activated in background through toast with: " + arguments);
                        AbstractToastActivation abstractToastActivation = ToastActivationArgumentParser.parseArguments(arguments);

                        if (abstractToastActivation is MarkChatAsReadToastActivation markChatAsRead)
                        {
                            ToastHelper.removeToastGroup(markChatAsRead.CHAT_ID);
                            ChatDBManager.INSTANCE.markAllMessagesAsRead(markChatAsRead.CHAT_ID);
                        }
                        else if (abstractToastActivation is MarkMessageAsReadToastActivation markMessageAsRead)
                        {
                            ChatDBManager.INSTANCE.markMessageAsRead(markMessageAsRead.CHAT_MESSAGE_ID);
                        }
                        else if (abstractToastActivation is SendReplyToastActivation sendReply)
                        {
                            ChatTable chat = ChatDBManager.INSTANCE.getChat(sendReply.CHAT_ID);
                            if (chat != null && userInput[ToastHelper.TEXT_BOX_ID] != null)
                            {
                                if (isRunning)
                                {

                                }
                                else
                                {

                                }
                            }
                        }
                    }
                    break;

                default:
                    break;
            }

            deferral.Complete();
        }

        protected async override void OnLaunched(LaunchActivatedEventArgs args)
        {
            await onActivatedOrLaunchedAsync(args);
        }

        protected async override void OnActivated(IActivatedEventArgs args)
        {
            await onActivatedOrLaunchedAsync(args);
        }

        #endregion
        //--------------------------------------------------------Events:---------------------------------------------------------------------\\
        #region --Events--
        private async void OnSuspending(object sender, SuspendingEventArgs e)
        {
            isRunning = false;

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
            isRunning = true;
        }

        void OnNavigationFailed(object sender, NavigationFailedEventArgs e)
        {
            throw new Exception("Failed to load Page " + e.SourcePageType.FullName);
        }

        private async void Push_PushNotificationReceived(object sender, PushNotificationReceivedEventArgs e)
        {
            // Add the notification message and title to the message:
            StringBuilder pushSummary = new StringBuilder("Push notification received:\n");
            pushSummary.Append($"\tNotification title: {e.Title}\n");
            pushSummary.Append($"\tMessage: {e.Message}");

            // If there is custom data associated with the notification, print the entries:
            if (e.CustomData != null)
            {
                pushSummary.Append("\n\tCustom data:\n");
                foreach (var key in e.CustomData.Keys)
                {
                    pushSummary.Append($"\t\t{key} : {e.CustomData[key]}\n");
                }
            }

            // Log notification summary:
            Logger.Info(pushSummary.ToString());

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
