using System;
using System.Threading.Tasks;
using Logging;
using Manager.Classes;
using Manager.Classes.Chat;
using Manager.Classes.Toast;
using Push.Classes;
using Shared.Classes;
using Storage.Classes;
using UWPX_UI.Classes;
using UWPX_UI.Dialogs;
using UWPX_UI_Context.Classes;
using Windows.ApplicationModel.Activation;
using Windows.Graphics.Display;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;
using XMPP_API.Classes;
using XMPP_API.Classes.XmppUri;

namespace UWPX_UI.Pages
{
    public sealed partial class ExtendedSplashScreenPage: Page
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        private readonly IActivatedEventArgs ACTIVATION_ARGS;
        private readonly Frame ROOT_FRAME;
        private SplashScreenImageScale curImageScale = SplashScreenImageScale.TINY;
        private double deviceScaleFactor;

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        public ExtendedSplashScreenPage(IActivatedEventArgs args, Frame rootFrame)
        {
            ACTIVATION_ARGS = args;
            ROOT_FRAME = rootFrame;
            InitializeComponent();

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
                    curImageScale = SplashScreenImageScale.HUGE;
                }
            }
            else if (ROOT_FRAME.ActualWidth >= 2000 || ROOT_FRAME.ActualHeight >= 2000)
            {
                if (curImageScale != SplashScreenImageScale.LARGE)
                {
                    background_img.Source = new BitmapImage(new Uri("ms-appx:///Assets/Images/SplashScreen/splash_screen_3000.png", UriKind.Absolute));
                    curImageScale = SplashScreenImageScale.LARGE;
                }
            }
            else if (ROOT_FRAME.ActualWidth >= 1000 || ROOT_FRAME.ActualHeight >= 1000)
            {
                if (curImageScale != SplashScreenImageScale.MEDIUM)
                {
                    background_img.Source = new BitmapImage(new Uri("ms-appx:///Assets/Images/SplashScreen/splash_screen_2000.png", UriKind.Absolute));
                    curImageScale = SplashScreenImageScale.MEDIUM;
                }
            }
            else if (ROOT_FRAME.ActualWidth >= 800 || ROOT_FRAME.ActualHeight >= 800)
            {
                if (curImageScale != SplashScreenImageScale.SMALL)
                {
                    background_img.Source = new BitmapImage(new Uri("ms-appx:///Assets/Images/SplashScreen/splash_screen_1000.png", UriKind.Absolute));
                    curImageScale = SplashScreenImageScale.SMALL;
                }
            }
            else if (curImageScale != SplashScreenImageScale.TINY)
            {
                background_img.Source = new BitmapImage(new Uri("ms-appx:///Assets/Images/SplashScreen/splash_screen_800.png", UriKind.Absolute));
                curImageScale = SplashScreenImageScale.TINY;
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
                if (ACTIVATION_ARGS.PreviousExecutionState != ApplicationExecutionState.Running)
                {
                    ACTIVATION_ARGS.SplashScreen.Dismissed += SPLASH_SCREEN_Dismissed;
                    return;
                }
            }

            if (ACTIVATION_ARGS.PreviousExecutionState == ApplicationExecutionState.Running)
            {
                Loaded += ExtendedSplashScreenPage_Loaded;
            }
        }

        private async Task LoadAppAsync()
        {
            // Setup listening for theme changes:
            ThemeUtils.SetupThemeListener();

            // Setup listener for window activated changes:
            UiUtils.SetupWindowActivatedListener();

            // Setup window:
            UiUtils.SetupWindow(Application.Current);

            // Setup App Center crashes, push:
            AppCenterHelper.SetupAppCenter();

            // Perform app update tasks if necessary:
            await AppUpdateHelper.OnAppStartAsync();

            // Register background tasks:
            Logger.Info("Registering background tasks...");
            await BackgroundTaskHelper.RegisterBackgroundTasksAsync();
            Logger.Info("Finished registering background tasks.");

            // Initialize the chat background helper:
            ChatBackgroundHelper.INSTANCE.Init();

            // Initialize the curated list of XMPP server providers:
            await XMPPProviders.INSTANCE.initAsync();

            // Load all chats:
            await DataCache.INSTANCE.InitAsync();

            // Connect to all clients:
            ConnectionHandler.INSTANCE.ConnectAll();

            // Initialize push:
            PushManager.INSTANCE.Init();

            // Remove the messages will be send later toasts:
            ToastHelper.RemoveToastGroup(ToastHelper.WILL_BE_SEND_LATER_TOAST_GROUP);

            // Update badge notification count:
            ToastHelper.UpdateBadgeNumber();

            // Show initial start dialog:
            if (!Storage.Classes.Settings.GetSettingBoolean(SettingsConsts.HIDE_INITIAL_START_DIALOG_ALPHA))
            {
                InitialStartDialog initialStartDialog = new InitialStartDialog();
                await UiUtils.ShowDialogAsync(initialStartDialog);
            }

            // Show what's new dialog:
            if (!Storage.Classes.Settings.GetSettingBoolean(SettingsConsts.HIDE_WHATS_NEW_DIALOG))
            {
                WhatsNewDialog whatsNewDialog = new WhatsNewDialog();
                await UiUtils.ShowDialogAsync(whatsNewDialog);
                if (whatsNewDialog.VIEW_MODEL.MODEL.ToDonatePageNavigated)
                {
                    if (!Storage.Classes.Settings.GetSettingBoolean(SettingsConsts.INITIALLY_STARTED))
                    {
                        PerformInitialStartSetup();
                    }
                    return;
                }
            }

            EvaluateActivationArgs();
        }

        private void PerformInitialStartSetup()
        {
            // By default enable the emoji button for all non mobile devices since the touch keyboard already adds an emoji keyboard:
            Storage.Classes.Settings.SetSetting(SettingsConsts.CHAT_ENABLE_EMOJI_BUTTON, DeviceFamilyHelper.GetDeviceFamilyType() != DeviceFamilyType.Mobile);
            // By default enter to send is enabled for all devices:
            Storage.Classes.Settings.SetSetting(SettingsConsts.ENTER_TO_SEND_MESSAGES, true);
            Storage.Classes.Settings.SetSetting(SettingsConsts.INITIALLY_STARTED, true);
            Storage.Classes.Settings.SetSetting(SettingsConsts.PUSH_ENABLED, true);
        }

        private void EvaluateActivationArgs()
        {
            // Initially started?
            if (!Storage.Classes.Settings.GetSettingBoolean(SettingsConsts.INITIALLY_STARTED))
            {
                PerformInitialStartSetup();

                ROOT_FRAME.Navigate(typeof(RegisterPage), typeof(ChatPage));
            }
            else if (ACTIVATION_ARGS is ProtocolActivatedEventArgs protocolActivationArgs)
            {
                Logger.Info("App activated by protocol activation with: " + protocolActivationArgs.Uri.ToString());

                // If we're currently not on a page, navigate to the main page:
                ROOT_FRAME.Navigate(typeof(ChatPage), UriUtils.parse(protocolActivationArgs.Uri));
            }
            else if (ACTIVATION_ARGS is ToastNotificationActivatedEventArgs toastActivationArgs)
            {
                Logger.Info("App activated by toast with: " + toastActivationArgs.Argument);
                // If empty args, no specific action (just launch the app):
                if (string.IsNullOrEmpty(toastActivationArgs.Argument))
                {
                    Logger.Warn("Toast activation with no argument!");
                    if (ROOT_FRAME.Content is null)
                    {
                        ROOT_FRAME.Navigate(typeof(ChatPage));
                    }
                    else
                    {
                        ROOT_FRAME.Navigate(typeof(ChatPage));
                    }
                }
                else
                {
                    ROOT_FRAME.Navigate(typeof(ChatPage), ToastActivationArgumentParser.ParseArguments(toastActivationArgs.Argument));
                }
                if (ROOT_FRAME.BackStack.Count == 0)
                {
                    ROOT_FRAME.BackStack.Add(new PageStackEntry(typeof(ChatPage), null, null));
                }
            }
            else if (ACTIVATION_ARGS is LaunchActivatedEventArgs launchActivationArgs)
            {
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

        private void ExtendedSplashScreenPage_Loaded(object sender, RoutedEventArgs e)
        {
            Loaded -= ExtendedSplashScreenPage_Loaded;

            // Update badge notification count:
            ToastHelper.UpdateBadgeNumber();

            EvaluateActivationArgs();
        }

        #endregion
    }
}
