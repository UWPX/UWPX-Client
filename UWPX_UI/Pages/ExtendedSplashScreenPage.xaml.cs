using Data_Manager2.Classes;
using Data_Manager2.Classes.DBManager;
using Data_Manager2.Classes.Toast;
using Logging;
using Microsoft.AppCenter.Push;
using Shared.Classes;
using System;
using System.Threading.Tasks;
using UWPX_UI.Classes;
using UWPX_UI.Dialogs;
using UWPX_UI_Context.Classes;
using Windows.ApplicationModel.Activation;
using Windows.Graphics.Display;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Imaging;
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
        private SplashScreenImageScale curImageScale = SplashScreenImageScale.TINY;
        private double deviceScaleFactor;

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
        private void SetImageScale()
        {
            if (ROOT_FRAME.ActualWidth == 0 || ROOT_FRAME.ActualHeight == 0)
            {
                return;
            }

            if (ROOT_FRAME.ActualWidth >= 3000 || ROOT_FRAME.ActualHeight >= 3000)
            {
                if (curImageScale != SplashScreenImageScale.HUGE)
                {
                    background_img.Source = new BitmapImage(new Uri("ms-appx:///Assets/Images/SplashScreen/splash_screen_4000.png", UriKind.Absolute));
                    logo_img.Source = new BitmapImage(new Uri("ms-appx:///Assets/SplashScreen.scale-400.png", UriKind.Absolute));
                    curImageScale = SplashScreenImageScale.HUGE;
                }
            }
            else if (ROOT_FRAME.ActualWidth >= 2000 || ROOT_FRAME.ActualHeight >= 2000)
            {
                if (curImageScale != SplashScreenImageScale.LARGE)
                {
                    background_img.Source = new BitmapImage(new Uri("ms-appx:///Assets/Images/SplashScreen/splash_screen_3000.png", UriKind.Absolute));
                    logo_img.Source = new BitmapImage(new Uri("ms-appx:///Assets/SplashScreen.scale-200.png", UriKind.Absolute));
                    curImageScale = SplashScreenImageScale.LARGE;
                }
            }
            else if (ROOT_FRAME.ActualWidth >= 1000 || ROOT_FRAME.ActualHeight >= 1000)
            {
                if (curImageScale != SplashScreenImageScale.MEDIUM)
                {
                    background_img.Source = new BitmapImage(new Uri("ms-appx:///Assets/Images/SplashScreen/splash_screen_2000.png", UriKind.Absolute));
                    logo_img.Source = new BitmapImage(new Uri("ms-appx:///Assets/SplashScreen.scale-150.png", UriKind.Absolute));
                    curImageScale = SplashScreenImageScale.MEDIUM;
                }
            }
            else if (ROOT_FRAME.ActualWidth >= 800 || ROOT_FRAME.ActualHeight >= 800)
            {
                if (curImageScale != SplashScreenImageScale.SMALL)
                {
                    background_img.Source = new BitmapImage(new Uri("ms-appx:///Assets/Images/SplashScreen/splash_screen_1000.png", UriKind.Absolute));
                    logo_img.Source = new BitmapImage(new Uri("ms-appx:///Assets/SplashScreen.scale-125.png", UriKind.Absolute));
                    curImageScale = SplashScreenImageScale.SMALL;
                }
            }
            else
            {
                if (curImageScale != SplashScreenImageScale.TINY)
                {
                    background_img.Source = new BitmapImage(new Uri("ms-appx:///Assets/Images/SplashScreen/splash_screen_800.png", UriKind.Absolute));
                    logo_img.Source = new BitmapImage(new Uri("ms-appx:///Assets/SplashScreen.scale-100.png", UriKind.Absolute));
                    curImageScale = SplashScreenImageScale.TINY;
                }
            }
        }

        #endregion
        //--------------------------------------------------------Misc Methods:---------------------------------------------------------------\\
        #region --Misc Methods (Public)--


        #endregion

        #region --Misc Methods (Private)--
        private void PositionLogoImage()
        {
            logo_img.SetValue(Canvas.LeftProperty, ACTIVATION_ARGS.SplashScreen.ImageLocation.X);
            logo_img.SetValue(Canvas.TopProperty, ACTIVATION_ARGS.SplashScreen.ImageLocation.Y);

            if (Windows.Foundation.Metadata.ApiInformation.IsTypePresent("Windows.Phone.UI.Input.HardwareButtons"))
            {
                logo_img.Height = ACTIVATION_ARGS.SplashScreen.ImageLocation.Height / deviceScaleFactor;
                logo_img.Width = ACTIVATION_ARGS.SplashScreen.ImageLocation.Width / deviceScaleFactor;
            }
            else
            {
                logo_img.Height = ACTIVATION_ARGS.SplashScreen.ImageLocation.Height;
                logo_img.Width = ACTIVATION_ARGS.SplashScreen.ImageLocation.Width;
            }
        }

        private void SetupSplashScreen()
        {
            Window.Current.SizeChanged += Current_SizeChanged;
            deviceScaleFactor = DisplayInformation.GetForCurrentView().RawPixelsPerViewPixel;

            SetImageScale();
            if (!(ACTIVATION_ARGS.SplashScreen is null))
            {
                PositionLogoImage();
                ACTIVATION_ARGS.SplashScreen.Dismissed += SPLASH_SCREEN_Dismissed;
            }
        }

        private async Task LoadAppAsync()
        {
            // Set requested theme:
            UiUtils.LoadRequestedTheme();

            // Setup listening for theme changes:
            UiUtils.SetupThemeListener();

            // Setup window:
            UiUtils.SetupWindow(Application.Current);

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

            // Show initial start dialog:
            if (!Data_Manager2.Classes.Settings.getSettingBoolean(SettingsConsts.HIDE_INITIAL_START_DIALOG_ALPHA))
            {
                InitialStartDialog initialStartDialog = new InitialStartDialog();
                await UiUtils.ShowDialogAsync(initialStartDialog);
            }

            // Show what's new dialog:
            if (!Data_Manager2.Classes.Settings.getSettingBoolean(SettingsConsts.HIDE_WHATS_NEW_DIALOG))
            {
                WhatsNewDialog whatsNewDialog = new WhatsNewDialog();
                await UiUtils.ShowDialogAsync(whatsNewDialog);
                if (whatsNewDialog.VIEW_MODEL.MODEL.ToDonatePageNavigated)
                {
                    if (!Data_Manager2.Classes.Settings.getSettingBoolean(SettingsConsts.INITIALLY_STARTED))
                    {
                        Data_Manager2.Classes.Settings.setSetting(SettingsConsts.INITIALLY_STARTED, true);
                    }
                    return;
                }
            }

            EvaluateActivationArgs();
        }

        private void EvaluateActivationArgs()
        {
            // Initially started?
            if (!Data_Manager2.Classes.Settings.getSettingBoolean(SettingsConsts.INITIALLY_STARTED))
            {
                Data_Manager2.Classes.Settings.setSetting(SettingsConsts.INITIALLY_STARTED, true);

                ROOT_FRAME.Navigate(typeof(AddAccountPage), typeof(ChatPage));
            }
            else if (ACTIVATION_ARGS is ProtocolActivatedEventArgs protocolActivationArgs)
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
                SpamDBManager.INSTANCE.initManager();
            });
        }

        #endregion

        #region --Misc Methods (Protected)--


        #endregion
        //--------------------------------------------------------Events:---------------------------------------------------------------------\\
        #region --Events--
        private async void SPLASH_SCREEN_Dismissed(SplashScreen sender, object args)
        {
            await SharedUtils.CallDispatcherAsync(async () => await LoadAppAsync());
        }

        private void Current_SizeChanged(object sender, Windows.UI.Core.WindowSizeChangedEventArgs e)
        {
            SetImageScale();
            PositionLogoImage();
        }

        private void Grid_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            SetImageScale();
            PositionLogoImage();
        }

        #endregion
    }
}
