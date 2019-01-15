using Data_Manager2.Classes;
using Data_Manager2.Classes.DBManager;
using Data_Manager2.Classes.Toast;
using Logging;
using Microsoft.AppCenter.Push;
using System;
using System.Threading.Tasks;
using UWPX_UI_Context.Classes;
using Windows.ApplicationModel.Activation;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using BackgroundTaskHelper = UWPX_UI_Context.Classes.BackgroundTaskHelper;

namespace UWPX_UI.Pages
{
    public sealed partial class ExtendedSplashScreenPage : Page
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        private readonly IActivatedEventArgs ACTIVATION_ARGS;
        private readonly Frame ROOT_FRAME;
        private readonly EventHandler<PushNotificationReceivedEventArgs> APP_CENTER_PUSH_CALLBACK;

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        public ExtendedSplashScreenPage(IActivatedEventArgs args, Frame rootFrame, EventHandler<PushNotificationReceivedEventArgs> appCenterPushCallback)
        {
            this.ACTIVATION_ARGS = args;
            this.ROOT_FRAME = rootFrame;
            this.APP_CENTER_PUSH_CALLBACK = appCenterPushCallback;
            this.InitializeComponent();

            SetupSplashScreen();
        }

        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--


        #endregion
        //--------------------------------------------------------Misc Methods:---------------------------------------------------------------\\
        #region --Misc Methods (Public)--


        #endregion

        #region --Misc Methods (Private)--
        private void PositionLogoImage()
        {
            logo_img.SetValue(Canvas.LeftProperty, ACTIVATION_ARGS.SplashScreen.ImageLocation.X);
            logo_img.SetValue(Canvas.TopProperty, ACTIVATION_ARGS.SplashScreen.ImageLocation.Y);
            logo_img.Height = ACTIVATION_ARGS.SplashScreen.ImageLocation.Height;
            logo_img.Width = ACTIVATION_ARGS.SplashScreen.ImageLocation.Width;
        }

        private void SetupSplashScreen()
        {
            if (!(ACTIVATION_ARGS.SplashScreen is null))
            {
                PositionLogoImage();
                ACTIVATION_ARGS.SplashScreen.Dismissed += SPLASH_SCREEN_Dismissed;
            }
        }

        private async Task LoadAppAsync()
        {
            // Setup App Center crashes, push:
            AppCenterHelper.SetupAppCenter(APP_CENTER_PUSH_CALLBACK);

            // Perform App update tasks if necessary:
            AppUpdateHelper.OnAppStart();

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

            if (ACTIVATION_ARGS.PreviousExecutionState == ApplicationExecutionState.Terminated)
            {
                // TODO: Load state from previously suspended application
            }

            // Connect to all clients:
            ConnectionHandler.INSTANCE.connectAll();

            EvaluateActivationArgs();
        }

        private void EvaluateActivationArgs()
        {
            /*if (!Data_Manager2.Classes.Settings.getSettingBoolean(SettingsConsts.INITIALLY_STARTED))
            {
                ROOT_FRAME.Navigate(typeof(SettingsPage));
            }
            else*/
            if (ACTIVATION_ARGS is ProtocolActivatedEventArgs protocolActivationArgs)
            {
                Logger.Info("App activated by protocol activation with: " + protocolActivationArgs.Uri.ToString());

                // If we're currently not on a page, navigate to the main page
                ROOT_FRAME.Navigate(typeof(ChatPage), protocolActivationArgs); // ToDo add arguments
            }
            else if (ACTIVATION_ARGS is ToastNotificationActivatedEventArgs toastActivationArgs)
            {
                Logger.Info("App activated by toast with: " + toastActivationArgs.Argument);
                // If empty args, no specific action (just launch the app)
                if (string.IsNullOrEmpty(toastActivationArgs.Argument))
                {
                    Logger.Warn("Toast activation with no argument!");
                    if (ROOT_FRAME.Content is null)
                    {
                        ROOT_FRAME.Navigate(typeof(ChatPage));
                    }
                }
                else
                {
                    ROOT_FRAME.Navigate(typeof(ChatPage), ToastActivationArgumentParser.parseArguments(toastActivationArgs.Argument));
                }
                if (ROOT_FRAME.BackStack.Count == 0)
                {
                    ROOT_FRAME.BackStack.Add(new PageStackEntry(typeof(ChatPage), null, null));
                }
            }
            else if (ACTIVATION_ARGS is LaunchActivatedEventArgs launchActivationArgs)
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
                ROOT_FRAME.Navigate(typeof(ChatPage));
            }
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

        #endregion

        #region --Misc Methods (Protected)--


        #endregion
        //--------------------------------------------------------Events:---------------------------------------------------------------------\\
        #region --Events--
        private async void SPLASH_SCREEN_Dismissed(SplashScreen sender, object args)
        {
            await UiUtils.CallDispatcherAsync(async () => await LoadAppAsync());
        }

        #endregion
    }
}
