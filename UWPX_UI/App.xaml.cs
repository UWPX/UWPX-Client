using Data_Manager2.Classes;
using Data_Manager2.Classes.DBManager;
using Data_Manager2.Classes.DBTables;
using Data_Manager2.Classes.Toast;
using Logging;
using Microsoft.AppCenter.Analytics;
using Microsoft.AppCenter.Crashes;
using Microsoft.AppCenter.Push;
using System;
using System.Text;
using System.Threading.Tasks;
using UWPX_UI.Dialogs;
using UWPX_UI.Pages;
using UWPX_UI_Context.Classes;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.ApplicationModel.Core;
using Windows.UI.Core;
using Windows.UI.Notifications;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace UWPX_UI
{
    sealed partial class App : Application
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        private readonly string APP_CENTER_SECRET = "523e7039-f6cb-4bf1-9000-53277ed97c53";

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

            // Setup App Center crashes, push:
            SetupAppCenter();

            this.InitializeComponent();
            this.Suspending += OnSuspending;
            this.Resuming += App_Resuming;
            this.UnhandledException += App_UnhandledException;

            // Perform App update tasks if necessary:
            AppUpdateHelper.OnAppStart();
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
        private void SetupAppCenter()
        {
            try
            {
                Microsoft.AppCenter.AppCenter.Start(APP_CENTER_SECRET, typeof(Crashes));
                if (Settings.getSettingBoolean(SettingsConsts.DISABLE_CRASH_REPORTING))
                {
                    Crashes.Instance.InstanceEnabled = false;
                    Logger.Info("AppCenter crash reporting is disabled.");
                }

                Microsoft.AppCenter.AppCenter.Start(APP_CENTER_SECRET, typeof(Analytics));
                if (Settings.getSettingBoolean(SettingsConsts.DISABLE_ANALYTICS))
                {
                    Analytics.SetEnabledAsync(false);
                    Logger.Info("AppCenter analytics are disabled.");
                }
#if DEBUG
                // Only enable push for debug builds:
                Microsoft.AppCenter.AppCenter.Start(APP_CENTER_SECRET, typeof(Push));
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
        /// Inits all DB managers to force event subscriptions.
        /// </summary>
        private async Task InitDBManagersAsync()
        {
            await Task.Run(() =>
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
        private void InitLogger()
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

        private async Task OnActivatedOrLaunchedAsync(IActivatedEventArgs args)
        {
            // Prevent window from extending into title bar:
            CoreApplication.GetCurrentView().TitleBar.ExtendViewIntoTitleBar = false;

            // Sets the log level:
            InitLogger();

            // Setup window:
            UiUtils.SetupWindow(Current);

            // Register background tasks:
            Logger.Info("Registering background tasks...");
            await BackgroundTaskHelper.RegisterBackgroundTasksAsync();
            Logger.Info("Finished registering background tasks.");

            // Init all db managers to force event subscriptions:
            await InitDBManagersAsync();


            // Loads all background images into the cache:
            await AppBackgroundHelper.InitAsync();

            // Setup push server connection:
            /*if (!Settings.getSettingBoolean(SettingsConsts.DISABLE_PUSH))
            {
                Push_App_Server.Classes.PushManager.init();
            }*/

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

            if (!Settings.getSettingBoolean(SettingsConsts.INITIALLY_STARTED))
            {
                // rootFrame.Navigate(typeof(AddAccountPage), "App.xaml.cs");
            }
            else if (args is ProtocolActivatedEventArgs protocolActivationArgs)
            {
                Logger.Info("App activated by protocol activation with: " + protocolActivationArgs.Uri.ToString());

                // If we're currently not on a page, navigate to the main page
                rootFrame.Navigate(typeof(ChatPage), protocolActivationArgs); // ToDo add arguments
            }
            else if (args is ToastNotificationActivatedEventArgs toastActivationArgs)
            {
                Logger.Info("App activated by toast with: " + toastActivationArgs.Argument);
                // If empty args, no specific action (just launch the app)
                if (string.IsNullOrEmpty(toastActivationArgs.Argument))
                {
                    Logger.Warn("Toast activation with no argument!");
                    if (rootFrame.Content is null)
                    {
                        rootFrame.Navigate(typeof(ChatPage));
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
                rootFrame.Navigate(typeof(ChatPage));
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
                        InitLogger();

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
                                // ToDo: Send reply
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
            await OnActivatedOrLaunchedAsync(args);
        }

        protected async override void OnActivated(IActivatedEventArgs args)
        {
            await OnActivatedOrLaunchedAsync(args);
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

            // If there is custom data associated width the notification, print the entries:
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
                    await UiUtils.ShowDialogAsync(dialog);
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
