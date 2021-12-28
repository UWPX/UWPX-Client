using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Logging;
using Manager.Classes.Toast;
using Microsoft.Toolkit.Uwp.Notifications;
using Push.Classes.Messages;
using Storage.Classes;
using Storage.Classes.Contexts;
using Storage.Classes.Models.Account;
using Windows.ApplicationModel.AppService;
using Windows.ApplicationModel.Background;
using Windows.Foundation.Collections;
using Windows.Networking.PushNotifications;
using Windows.UI.Notifications;

namespace Push_BackgroundTask.Classes
{
    /// <summary>
    /// Background tasks are limited to 30 seconds.
    /// Source: https://docs.microsoft.com/en-us/windows/uwp/launch-resume/guidelines-for-background-tasks#background-task-guidance
    /// </summary>
    public sealed class PushBackgroundTask: IBackgroundTask
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        private BackgroundTaskDeferral deferral;
        private AppServiceConnection appServiceConnection;

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--


        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--
        private AccountModel GetAccount(string accountId)
        {
            List<AccountModel> accounts;
            using (MainDbContext ctx = new MainDbContext())
            {
                accounts = ctx.Accounts.ToList();
            }
            return accounts.Where(a => string.Equals(Push.Classes.Utils.ToAccountId(a.bareJid), accountId)).FirstOrDefault();
        }

        private async Task<bool> IsAppRunningAsync()
        {
            AppServiceConnectionStatus result = await appServiceConnection.OpenAsync();
            if (result == AppServiceConnectionStatus.Success)
            {
                ValueSet request = new ValueSet
                {
                    { "request", "is_running" }
                };
                AppServiceResponse response = await appServiceConnection.SendMessageAsync(request);
                if (response.Status == AppServiceResponseStatus.Success)
                {
                    return string.Equals(response.Message["response"] as string, "true");
                }
            }
            return false;
        }

        private async Task<bool> IsAccountConnectingAsync(string bareJid)
        {
            ValueSet request = new ValueSet { { "request", "is_connecting" }, { "bare_jid", bareJid } };
            AppServiceResponse response = await appServiceConnection.SendMessageAsync(request);
            if (response.Status == AppServiceResponseStatus.Success)
            {
                return string.Equals(response.Message["response"] as string, "true");
            }
            return false;
        }

        #endregion
        //--------------------------------------------------------Misc Methods:---------------------------------------------------------------\\
        #region --Misc Methods (Public)--
        public async void Run(IBackgroundTaskInstance taskInstance)
        {
            deferral = taskInstance.GetDeferral();
            if (taskInstance.TriggerDetails is RawNotification notification)
            {
                if (Settings.GetSettingBoolean(SettingsConsts.PUSH_ENABLED))
                {
                    // ToastHelper.ShowSimpleToast("Processing started.");
                    BindableString statusText = new BindableString("status_text");
                    ToastNotification progressToast = PopProgressiveToast(statusText);
                    try
                    {
                        await ParseAndShowNotificationAsync(notification.Content, progressToast, statusText);
                    }
                    catch (Exception e)
                    {
                        Logger.Error("Failed to process push notification.", e);
                    }
                    ToastNotificationManager.History.Remove(progressToast.Tag);
                    // ToastHelper.ShowSimpleToast("Processing finished.");
                }
                else
                {
                    Logger.Warn("Received a push notification, but push is disabled. Dropping it.");
                }
            }
            deferral.Complete();
        }

        #endregion

        #region --Misc Methods (Private)--
        private void UpdateProgress(ToastNotification toast, BindableString title, string value)
        {
            NotificationData data = new NotificationData
            {
                SequenceNumber = 0
            };
            data.Values[title.BindingName] = value;
            ToastNotificationManager.CreateToastNotifier().Update(data, toast.Tag);
        }

        private ToastNotification PopProgressiveToast(BindableString title)
        {
            ToastContent toastContent = new ToastContent
            {
                Visual = new ToastVisual
                {
                    BindingGeneric = new ToastBindingGeneric
                    {
                        Children =
                        {
                            new AdaptiveProgressBar
                            {
                                Value = AdaptiveProgressBarValue.Indeterminate,
                                Title = title,
                                Status = ""
                            }
                        }
                    }
                },
            };

            ToastNotification toast = new ToastNotification(toastContent.GetXml())
            {
                Data = new NotificationData(),
                Tag = "toast_progress_tag"
            };
            toast.Data.Values["status_text"] = "Started";
            // ToastNotificationManager.CreateToastNotifier().Show(toast);
            return toast;
        }

        private async Task ParseAndShowNotificationAsync(string s, ToastNotification progressToast, BindableString statusText)
        {
            UpdateProgress(progressToast, statusText, "Started");
            if (string.IsNullOrEmpty(s))
            {
                Logger.Warn("Received an empty push notification...");
                return;
            }
            Logger.Trace(s);

            AbstractMessage msg = MessageParser.Parse(s);
            if (msg is null)
            {
                Logger.Warn("Invalid push message received: " + s);
                return;
            }
            UpdateProgress(progressToast, statusText, "Parsed");

            if (msg is TestPushMessage)
            {
                ToastHelper.ShowSimpleToast("Here is your test push message. It got successfully received from the push server!🎉");
                Logger.Info("Test push message received.");
                return;
            }
            else if (msg is PushMessage pushMsg)
            {
                await HandelPushMessageAsync(pushMsg, progressToast, statusText);
            }
            else
            {
                Logger.Warn("Invalid push message action received: " + msg.action);
                return;
            }
        }

        private async Task HandelPushMessageAsync(PushMessage pushMsg, ToastNotification progressToast, BindableString statusText)
        {
            // Get account:
            UpdateProgress(progressToast, statusText, "Getting account...");
            AccountModel account = GetAccount(pushMsg.accountId);
            if (account is null)
            {
                Logger.Warn($"Received a push notification for an unknown account ID '{pushMsg.accountId}'. Dropping it.");
                return;
            }

            // Check if the account is enabled:
            if (!account.enabled)
            {
                Logger.Info($"Received a push notification for a deactivated account ('{account.bareJid}'). Dropping it.");
            }

            // Init the app service connection:
            appServiceConnection = new AppServiceConnection
            {
                AppServiceName = "uwpx.status",
                PackageFamilyName = Windows.ApplicationModel.Package.Current.Id.FamilyName
            };

            UpdateProgress(progressToast, statusText, "Checking if the app is running...");
            bool appRunning = await IsAppRunningAsync();
            if (appRunning)
            {
                bool accountConnected = await IsAccountConnectingAsync(account.bareJid);
                UpdateProgress(progressToast, statusText, $"App is running. Account connected: {accountConnected}");
                if (accountConnected)
                {
                    Logger.Info("No need to toast push. Account already connecting/connected.");
                    return;
                }
            }

            if (pushMsg.pendingSubscriptionCount > 0)
            {
                ToastHelper.ShowAccountMessageToast($"You might have received new subscription requests while you were away.", account.bareJid, "subscription");
            }
            else
            {
                ToastHelper.ShowAccountMessageToast($"📨 You might have received new messages while you were away.", account.bareJid, "message");
            }

            /*UpdateProgress(progressToast, statusText, $"Getting client for: {account.bareJid}");
            ClientConnectionHandler client = ConnectionHandler.INSTANCE.GetClient(account.bareJid);
            UpdateProgress(progressToast, statusText, $"Connecting client '{account.bareJid}'...");
            Logger.Info($"Connecting '{account.bareJid}'...");
            await client.ConnectAsync();
            UpdateProgress(progressToast, statusText, $"Client '{account.bareJid}' connected. Waiting 10 seconds...");
            await Task.Delay(TimeSpan.FromSeconds(5));
            UpdateProgress(progressToast, statusText, $"Disconnecting client '{account.bareJid}'...");
            await client.DisconnectAsync();
            UpdateProgress(progressToast, statusText, $"Client '{account.bareJid}' disconnected.");
            Logger.Info($"'{account.bareJid}' disconnected.");*/
        }

        #endregion

        #region --Misc Methods (Protected)--


        #endregion
        //--------------------------------------------------------Events:---------------------------------------------------------------------\\
        #region --Events--


        #endregion
    }
}
