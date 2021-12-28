using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Logging;
using Manager.Classes;
using Manager.Classes.Chat;
using Manager.Classes.Toast;
using Push.Classes;
using Push.Classes.Events;
using Shared.Classes.AppCenter;
using Shared.Classes.Threading;
using Storage.Classes;
using Storage.Classes.Contexts;
using Storage.Classes.Models.Chat;
using UWPX_UI.Dialogs;
using UWPX_UI.Pages;
using UWPX_UI_Context.Classes;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.ApplicationModel.AppService;
using Windows.ApplicationModel.Background;
using Windows.Foundation.Collections;
using Windows.UI.Notifications;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using XMPP_API.Classes.Network.XML.Messages;
using XMPP_API.Classes.Network.XML.Messages.XEP_0384;

namespace UWPX_UI
{
    public sealed partial class App: Application
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        private bool isRunning;

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        public App()
        {
            isRunning = false;

            // Set requested theme:
            ElementTheme theme = ThemeUtils.LoadRequestedTheme();
            RequestedTheme = ThemeUtils.GetActualTheme(theme);

            InitializeComponent();
            Suspending += OnSuspending;
            Resuming += OnAppResuming;
            UnhandledException += App_UnhandledException;
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
        /// Sets the log level for the logger class.
        /// </summary>
        private void InitLogger()
        {
            object o = Settings.GetSetting(SettingsConsts.LOG_LEVEL);
            if (o is int)
            {
                try
                {
                    Logger.logLevel = (LogLevel)o;
                }
                catch (Exception e)
                {
                    Logger.Error("Failed to parse log level (" + o.ToString() + "). Resetting it to LogLevel.INFO: " + e.Message);
                    Settings.SetSetting(SettingsConsts.LOG_LEVEL, (int)LogLevel.INFO);
                    Logger.logLevel = LogLevel.INFO;
                }
            }
            else
            {
                Settings.SetSetting(SettingsConsts.LOG_LEVEL, (int)LogLevel.INFO);
                Logger.logLevel = LogLevel.INFO;
            }
        }

        private void SubscribeToPushManagerEvents()
        {
            PushManager.INSTANCE.StateChanged -= PushManager_StateChanged;
            PushManager.INSTANCE.StateChanged += PushManager_StateChanged;
        }

        private void OnActivatedOrLaunched(IActivatedEventArgs args)
        {
            // Sets the log level:
            InitLogger();

            isRunning = true;

            // Initialize push:
            SubscribeToPushManagerEvents();

            // Register a handler to show a dialog when we catch a crash:
            AppCenterCrashHelper.INSTANCE.OnTrackError += OnTrackError;

            // Do not repeat app initialization when the Window already has content,
            // just ensure that the window is active
            if (Window.Current.Content is not Frame rootFrame)
            {
                // Create a Frame to act as the navigation context and navigate to the first page:
                rootFrame = new Frame();

                rootFrame.NavigationFailed += OnNavigationFailed;

                // Place the frame in the current Window
                Window.Current.Content = rootFrame;
            }

            ExtendedSplashScreenPage extendedSplashScreen = new ExtendedSplashScreenPage(args, rootFrame);
            rootFrame.Content = extendedSplashScreen;

            Window.Current.Activate();
        }

        private async Task SendChatMessageAsync(ChatModel chat, string message)
        {
            string fromFullJid;
            using (MainDbContext ctx = new MainDbContext())
            {
                fromFullJid = ctx.Accounts.Where(a => string.Equals(a.bareJid, chat.accountBareJid)).Select(a => a.fullJid.FullJid()).FirstOrDefault();
            }

            if (fromFullJid is null)
            {
                Logger.Error($"Failed to send message from background. Account '{chat.bareJid}' does not exist.");
                return;
            }

            string to = chat.bareJid;
            string chatType = chat.chatType == ChatType.CHAT ? MessageMessage.TYPE_CHAT : MessageMessage.TYPE_GROUPCHAT;
            bool reciptRequested = true;

            MessageMessage toSendMsg;
            if (chat.omemoInfo.enabled)
            {
                if (chat.chatType == ChatType.CHAT)
                {
                    toSendMsg = new OmemoEncryptedMessage(fromFullJid, to, message, chatType, reciptRequested);
                }
                else
                {
                    // ToDo: Add MUC OMEMO support
                    throw new NotImplementedException("Sending encrypted messages for MUC is not supported right now!");
                }
            }
            else if (chat.chatType == ChatType.CHAT)
            {
                toSendMsg = new MessageMessage(fromFullJid, to, message, chatType, reciptRequested);
            }
            else
            {
                toSendMsg = new MessageMessage(fromFullJid, to, message, chatType, chat.muc.nickname, reciptRequested);
            }

            // Create a copy for the DB:
            ChatMessageModel toSendMsgDB = new ChatMessageModel(toSendMsg, chat)
            {
                state = toSendMsg is OmemoEncryptedMessage ? MessageState.TO_ENCRYPT : MessageState.SENDING
            };

            // Set the chat message id for later identification:
            toSendMsg.chatMessageId = toSendMsgDB.id;

            // Update chat last active:
            chat.lastActive = DateTime.Now;

            // Update DB:
            chat.Update();
            await DataCache.INSTANCE.AddChatMessageAsync(toSendMsgDB, chat);

            Logger.Info("Added to send message in background");

            if (isRunning)
            {
                Client client = ConnectionHandler.INSTANCE.GetClient(chat.bareJid).client;
                if (client is null)
                {
                    Logger.Error($"Failed to send message from background. Client '{chat.bareJid}' does not exist.");
                }
                // Send the message:
                else if (toSendMsg is OmemoEncryptedMessage toSendOmemoMsg)
                {
                    await client.xmppClient.sendOmemoMessageAsync(toSendOmemoMsg, chat.bareJid, client.dbAccount.bareJid, client.dbAccount.omemoInfo.trustedKeysOnly, chat.omemoInfo.trustedKeysOnly);
                }
                else
                {
                    await client.xmppClient.SendAsync(toSendMsg);
                }
            }
            else
            {
                ToastHelper.ShowWillBeSendLaterToast(chat);
            }
        }

        #endregion

        #region --Misc Methods (Protected)--
        protected override async void OnBackgroundActivated(BackgroundActivatedEventArgs args)
        {
            base.OnBackgroundActivated(args);

            if (args.TaskInstance.TriggerDetails is AppServiceTriggerDetails appServiceTriggerDetails)
            {
                Logger.Info("App service background activation.");
                BackgroundTaskDeferral appServiceDeferral = args.TaskInstance.GetDeferral();
                args.TaskInstance.Canceled += (IBackgroundTaskInstance sender, BackgroundTaskCancellationReason reason) => appServiceDeferral.Complete();
                AppServiceConnection appServiceConnection = appServiceTriggerDetails.AppServiceConnection;
                appServiceConnection.ServiceClosed += (AppServiceConnection sender, AppServiceClosedEventArgs args) => appServiceDeferral.Complete();
                appServiceConnection.RequestReceived += OnAppServiceRequestReceived;
                return;
            }

            BackgroundTaskDeferral deferral = args.TaskInstance.GetDeferral();

            switch (args.TaskInstance.Task.Name)
            {
                case BackgroundTaskHelper.TOAST_BACKGROUND_TASK_NAME:
                    if (args.TaskInstance.TriggerDetails is ToastNotificationActionTriggerDetail details)
                    {
                        InitLogger();

                        string arguments = details.Argument;
                        ValueSet userInput = details.UserInput;

                        Logger.Debug("App activated in background through toast with: " + arguments);
                        AbstractToastActivation abstractToastActivation = ToastActivationArgumentParser.ParseArguments(arguments);

                        if (abstractToastActivation is null)
                        {
                            Logger.Warn("Unable to evaluate toast activation string - unknown format");
                        }
                        else if (abstractToastActivation is MarkChatAsReadToastActivation markChatAsRead)
                        {
                            ToastHelper.RemoveToastGroup(ToastHelper.GetChatToastGroup(markChatAsRead.CHAT_ID.ToString()));
                            DataCache.INSTANCE.MarkAllChatMessagesAsRead(markChatAsRead.CHAT_ID);
                        }
                        else if (abstractToastActivation is MarkMessageAsReadToastActivation markMessageAsRead)
                        {
                            DataCache.INSTANCE.MarkChatMessageAsRead(markMessageAsRead.CHAT_ID, markMessageAsRead.CHAT_MESSAGE_ID);
                        }
                        else if (abstractToastActivation is SendReplyToastActivation sendReply)
                        {
                            ChatModel chat;
                            using (SemaLock semaLock = DataCache.INSTANCE.NewChatSemaLock())
                            {
                                chat = DataCache.INSTANCE.GetChat(sendReply.CHAT_ID, semaLock);
                            }
                            if (chat is not null && userInput[ToastHelper.TEXT_BOX_ID] is string text)
                            {
                                string trimedText = text.Trim(UiUtils.TRIM_CHARS);
                                await SendChatMessageAsync(chat, trimedText);
                            }
                            DataCache.INSTANCE.MarkChatMessageAsRead(sendReply.CHAT_ID, sendReply.CHAT_MESSAGE_ID);
                        }
                    }
                    break;

                default:
                    break;
            }

            deferral.Complete();
        }

        protected override void OnLaunched(LaunchActivatedEventArgs args)
        {
            OnActivatedOrLaunched(args);
        }

        protected override void OnActivated(IActivatedEventArgs args)
        {
            OnActivatedOrLaunched(args);
        }

        #endregion
        //--------------------------------------------------------Events:---------------------------------------------------------------------\\
        #region --Events--
        private async void OnSuspending(object sender, SuspendingEventArgs e)
        {
            isRunning = false;

            SuspendingDeferral deferral = e.SuspendingOperation.GetDeferral();
            // TODO re-implement transfer socket ownership:
            //await ConnectionHandler.INSTANCE.transferSocketOwnershipAsync();

            // Disconnect all clients:
            await ConnectionHandler.INSTANCE.DisconnectAllAsync();
            deferral.Complete();
        }

        private void OnAppResuming(object sender, object e)
        {
            // Connect to all clients:
            ConnectionHandler.INSTANCE.ConnectAll();
            isRunning = true;

            // Actually initializing the push manager will happen later in extended splash screen once all clients are loaded:
            SubscribeToPushManagerEvents();

            // Remove toasts:
            ToastHelper.RemoveToastGroup(ToastHelper.WILL_BE_SEND_LATER_TOAST_GROUP);
            ToastHelper.RemoveChatToastGroups();
        }

        private void OnNavigationFailed(object sender, NavigationFailedEventArgs e)
        {
            throw new Exception("Failed to load Page " + e.SourcePageType.FullName);
        }

        private void App_UnhandledException(object sender, Windows.UI.Xaml.UnhandledExceptionEventArgs e)
        {
            e.Handled = true;
#if DEBUG
            if (Debugger.IsAttached)
            {
                Debugger.Break();
            }
#endif
            Logger.Error("Unhanded exception: ", e.Exception);
        }

        private async void PushManager_StateChanged(PushManager sender, PushManagerStateChangedEventArgs args)
        {
            if (args.NEW_STATE == PushManagerState.INITIALIZED)
            {
                // Setup done, now send an updated list of all push accounts:
                if (PushManager.ShouldUpdatePushForAccounts())
                {
                    await PushManager.INSTANCE.InitPushForAccountsAsync();
                }
                else
                {
                    Logger.Info("No need to update push accounts on the push server.");
                }

                // Inform the clients that the PushManager initialized successfully:
                foreach (ClientConnectionHandler c in ConnectionHandler.INSTANCE.GetClients())
                {
                    c.client.PUSH_MANAGER.OnPushManagerInitialized();
                }
            }
            else if (args.NEW_STATE == PushManagerState.DEAKTIVATED)
            {
                if (args.OLD_STATE == PushManagerState.INITIALIZED)
                {
                    // Setup done, now send an updated list of all push accounts:
                    if (PushManager.ShouldUpdatePushForAccounts())
                    {
                        await PushManager.INSTANCE.InitPushForAccountsAsync();
                    }
                    else
                    {
                        Logger.Info("No need to update push accounts on the push server.");
                    }
                }
                // Inform the clients that the PushManager got disabled:
                foreach (ClientConnectionHandler c in ConnectionHandler.INSTANCE.GetClients())
                {
                    c.client.PUSH_MANAGER.OnPushManagerInitialized();
                }
            }
        }

        private static async Task OnTrackError(AppCenterCrashHelper sender, TrackErrorEventArgs args)
        {
            if (!Settings.GetSettingBoolean(SettingsConsts.ALWAYS_REPORT_CRASHES_WITHOUT_ASKING))
            {
                ReportCrashDialog dialog = new ReportCrashDialog(args);
                await UiUtils.ShowDialogAsync(dialog);
                if (!dialog.VIEW_MODEL.MODEL.Report)
                {
                    args.Cancel = true;
                }
                else if (dialog.AlwaysReport)
                {
                    Settings.SetSetting(SettingsConsts.ALWAYS_REPORT_CRASHES_WITHOUT_ASKING, true);
                }
            }
        }

        private async void OnAppServiceRequestReceived(AppServiceConnection sender, AppServiceRequestReceivedEventArgs args)
        {
            AppServiceDeferral msgDeferral = args.GetDeferral();
            ValueSet msg = args.Request.Message;
            try
            {
                string request = msg["request"] as string;

                ValueSet response = new();
                bool validRequest = true;
                switch (request)
                {
                    case "is_running":
                        response.Add("response", isRunning && UiUtils.IsWindowActivated ? "true" : "false");
                        break;

                    case "is_connected":
                        string bareJid = msg["bare_jid"] as string;
                        Client client = ConnectionHandler.INSTANCE.GetClient(bareJid)?.client;
                        bool connected = client is not null && client.xmppClient.isConnected();
                        response.Add("response", connected ? "true" : "false");
                        break;

                    default:
                        Logger.Warn($"Unknown app service request '{request}' received!");
                        validRequest = false;
                        break;
                }

                if (validRequest)
                {
                    _ = await args.Request.SendResponseAsync(response);
                }
            }
            catch (Exception e)
            {
                Logger.Error("Processing the app service request failed.", e);
            }

            msgDeferral.Complete();
        }

        #endregion
    }
}
