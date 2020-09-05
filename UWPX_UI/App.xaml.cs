using System;
using System.Text;
using System.Threading.Tasks;
using Data_Manager2.Classes;
using Data_Manager2.Classes.DBManager;
using Data_Manager2.Classes.DBTables;
using Data_Manager2.Classes.Toast;
using Logging;
using Push.Classes;
using Push.Classes.Events;
using Shared.Classes;
using UWPX_UI.Dialogs;
using UWPX_UI.Pages;
using UWPX_UI_Context.Classes;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.ApplicationModel.Background;
using Windows.ApplicationModel.Core;
using Windows.Foundation.Collections;
using Windows.Networking.PushNotifications;
using Windows.UI.Core;
using Windows.UI.Notifications;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using XMPP_API.Classes;
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
            object o = Settings.getSetting(SettingsConsts.LOG_LEVEL);
            if (o is int)
            {
                try
                {
                    Logger.logLevel = (LogLevel)o;
                }
                catch (Exception e)
                {
                    Logger.Error("Failed to parse log level (" + o.ToString() + "). Resetting it to LogLevel.INFO: " + e.Message);
                    Settings.setSetting(SettingsConsts.LOG_LEVEL, (int)LogLevel.INFO);
                    Logger.logLevel = LogLevel.INFO;
                }
            }
            else
            {
                Settings.setSetting(SettingsConsts.LOG_LEVEL, (int)LogLevel.INFO);
                Logger.logLevel = LogLevel.INFO;
            }
        }

        private void InitPush()
        {
            PushManager.INSTANCE.StateChanged -= PushManager_StateChanged;
            PushManager.INSTANCE.StateChanged += PushManager_StateChanged;
            PushManager.INSTANCE.Init();
        }

        private void OnActivatedOrLaunched(IActivatedEventArgs args)
        {
            // Sets the log level:
            InitLogger();

            // Override resources to increase the UI performance on mobile devices:
            if (DeviceFamilyHelper.GetDeviceFamilyType() == DeviceFamilyType.Mobile)
            {
                ThemeUtils.OverrideThemeResources();
            }

            isRunning = true;

            // Initialize push:
            InitPush();

            // Do not repeat app initialization when the Window already has content,
            // just ensure that the window is active
            if (!(Window.Current.Content is Frame rootFrame))
            {
                // Create a Frame to act as the navigation context and navigate to the first page:
                rootFrame = new Frame();

                rootFrame.NavigationFailed += OnNavigationFailed;

                // Place the frame in the current Window
                Window.Current.Content = rootFrame;
            }

            ExtendedSplashScreenPage extendedSplashScreen = new ExtendedSplashScreenPage(args, rootFrame, AppCenter_PushNotificationReceived);
            rootFrame.Content = extendedSplashScreen;

            Window.Current.Activate();
        }

        private async Task SendChatMessageAsync(ChatTable chat, string message)
        {
            AccountTable account = AccountDBManager.INSTANCE.getAccount(chat.userAccountId);
            if (account is null)
            {
                Logger.Warn("Unable to send message - no such account: " + chat.userAccountId);
                return;
            }

            string fromBareJid = account.userId + '@' + account.domain;
            string fromFullJid = fromBareJid + '/' + account.resource;
            string to = chat.chatJabberId;
            string chatType = chat.chatType == ChatType.CHAT ? MessageMessage.TYPE_CHAT : MessageMessage.TYPE_GROUPCHAT;
            bool reciptRequested = true;

            MessageMessage toSendMsg;
            if (chat.omemoEnabled)
            {
                if (chat.chatType == ChatType.CHAT)
                {
                    toSendMsg = new OmemoMessageMessage(fromFullJid, to, message, chatType, reciptRequested);
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
                MUCChatInfoTable mucInfo = MUCDBManager.INSTANCE.getMUCInfo(chat.id);
                toSendMsg = new MessageMessage(fromFullJid, to, message, chatType, mucInfo.nickname, reciptRequested);
            }

            // Create a copy for the DB:
            ChatMessageTable toSendMsgDB = new ChatMessageTable(toSendMsg, chat)
            {
                state = toSendMsg is OmemoMessageMessage ? MessageState.TO_ENCRYPT : MessageState.SENDING
            };

            // Set the chat message id for later identification:
            toSendMsg.chatMessageId = toSendMsgDB.id;

            // Update chat last active:
            chat.lastActive = DateTime.Now;

            // Update DB:
            await ChatDBManager.INSTANCE.setChatMessageAsync(toSendMsgDB, true, false);
            ChatDBManager.INSTANCE.setChat(chat, false, true);

            Logger.Info("Added to send message in background");

            if (isRunning)
            {
                XMPPClient client = ConnectionHandler.INSTANCE.getClient(fromBareJid);
                if (client is null)
                {
                    Logger.Error("Unable to send message in background - no such client: " + fromBareJid);
                }
                // Send the message:
                else if (toSendMsg is OmemoMessageMessage toSendOmemoMsg)
                {
                    await client.sendOmemoMessageAsync(toSendOmemoMsg, chat.chatJabberId, client.getXMPPAccount().getBareJid());
                }
                else
                {
                    await client.SendAsync(toSendMsg);
                }
            }
            else
            {
                ToastHelper.showWillBeSendLaterToast(chat);
            }
        }

        #endregion

        #region --Misc Methods (Protected)--
        protected override async void OnBackgroundActivated(BackgroundActivatedEventArgs args)
        {
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
                        AbstractToastActivation abstractToastActivation = ToastActivationArgumentParser.parseArguments(arguments);

                        if (abstractToastActivation is null)
                        {
                            Logger.Warn("Unable to evaluate toast activation string - unknown format");
                        }
                        else if (abstractToastActivation is MarkChatAsReadToastActivation markChatAsRead)
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
                            if (!(chat is null) && userInput[ToastHelper.TEXT_BOX_ID] is string text)
                            {
                                string trimedText = text.Trim(UiUtils.TRIM_CHARS);
                                await SendChatMessageAsync(chat, trimedText);
                            }
                            ChatDBManager.INSTANCE.markMessageAsRead(sendReply.CHAT_MESSAGE_ID);
                        }

                        ToastHelper.UpdateBadgeNumber();
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
            await ConnectionHandler.INSTANCE.disconnectAllAsync();
            deferral.Complete();
        }

        private void OnAppResuming(object sender, object e)
        {
            // Connect to all clients:
            ConnectionHandler.INSTANCE.connectAll();
            isRunning = true;

            // Initialize push:
            InitPush();
        }

        private void OnNavigationFailed(object sender, NavigationFailedEventArgs e)
        {
            throw new Exception("Failed to load Page " + e.SourcePageType.FullName);
        }

        private async void AppCenter_PushNotificationReceived(object sender, Microsoft.AppCenter.Push.PushNotificationReceivedEventArgs e)
        {
            // Add the notification message and title to the message:
            StringBuilder pushSummary = new StringBuilder("Push notification received:\n");
            pushSummary.Append($"\tNotification title: {e.Title}\n");
            pushSummary.Append($"\tMessage: {e.Message}");

            // If there is custom data associated width the notification, print the entries:
            if (e.CustomData != null)
            {
                pushSummary.Append("\n\tCustom data:\n");
                foreach (string key in e.CustomData.Keys)
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

        private void App_UnhandledException(object sender, Windows.UI.Xaml.UnhandledExceptionEventArgs e)
        {
            Logger.Error("Unhanded exception: ", e.Exception);
        }

        private async void PushManager_StateChanged(PushManager sender, PushManagerStateChangedEventArgs args)
        {
            if (args.NEW_STATE == PushManagerState.INITIALIZED)
            {
                PushNotificationChannel channel = PushManager.INSTANCE.GetChannel();
                channel.PushNotificationReceived -= WNS_PushNotificationReceived;
                channel.PushNotificationReceived += WNS_PushNotificationReceived;

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
            else if (args.NEW_STATE == PushManagerState.DEAKTIVATED && args.OLD_STATE == PushManagerState.INITIALIZED)
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
        }

        private void WNS_PushNotificationReceived(PushNotificationChannel sender, PushNotificationReceivedEventArgs args)
        {
            if (args.NotificationType == PushNotificationType.Raw)
            {
                RawNotification notification = args.RawNotification;
                ToastHelper.showSimpleToast(notification.Content);
            }
        }

        #endregion
    }
}
