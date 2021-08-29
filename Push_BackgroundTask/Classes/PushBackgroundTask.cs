using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;
using Logging;
using Manager.Classes.Toast;
using Storage.Classes;
using Storage.Classes.Contexts;
using Storage.Classes.Models.Account;
using Storage.Classes.Models.Chat;
using Windows.ApplicationModel.AppService;
using Windows.ApplicationModel.Background;
using Windows.Foundation.Collections;
using Windows.Networking.PushNotifications;
using XMPP_API.Classes;
using XMPP_API.Classes.Network.XML;

namespace Push_BackgroundTask.Classes
{
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

        private ChatModel GetChat(string accountBareJid, string chatBareJid)
        {
            using (MainDbContext ctx = new MainDbContext())
            {
                return ctx.GetChat(accountBareJid, chatBareJid);
            }
        }

        private string GetAccountId(XElement node)
        {
            XElement accountNode = XMLUtils.getNodeFromXElement(node, "account");
            if (!(accountNode is null))
            {
                XAttribute idAttribute = accountNode.Attribute("id");
                if (!(idAttribute is null) && !string.IsNullOrEmpty(idAttribute.Value))
                {
                    return idAttribute.Value;
                }
            }
            return null;
        }

        private string GetValue(XElement node, string var)
        {
            XElement xNode = XMLUtils.getNodeFromXElement(node, "x");
            if (!(xNode is null))
            {
                foreach (XElement n in xNode.Elements())
                {
                    if (string.Equals(n.Name.LocalName, "field"))
                    {
                        XAttribute attribute = n.Attribute("var");
                        if (!(attribute is null) && string.Equals(attribute.Value, var))
                        {
                            XElement valueNode = XMLUtils.getNodeFromXElement(n, "value");
                            if (!(valueNode is null))
                            {
                                return valueNode.Value;
                            }
                        }
                    }
                }
            }
            return null;
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

        private async Task<bool> IsAccountConnectedAsync(string bareJid)
        {
            ValueSet request = new ValueSet
                {
                    { "request", "is_connected" },
                    {"bare_jid", bareJid }
};
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
                    await ParseAndShowNotificationAsync(notification.Content);
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
        private async Task ParseAndShowNotificationAsync(string s)
        {
            if (string.IsNullOrEmpty(s))
            {
                Logger.Warn("Received an empty push notification...");
                return;
            }
            Logger.Debug(s);

            XDocument doc;
            try
            {
                doc = XDocument.Parse(s);
            }
            catch (Exception e)
            {
                Logger.Error("Failed to parse push notification.", e);
                return;
            }

            // Test push:
            if (string.Equals(doc.Root.Name.LocalName, "test"))
            {
                ToastHelper.ShowSimpleToast("Here is your test push message. It got successfully received from the push server!🎉");
                Logger.Info("Test push message received.");
                return;
            }

            // Validate account:
            string accountId = GetAccountId(doc.Root);
            if (accountId is null)
            {
                Logger.Warn("Received a push notification without a valid account ID. Dropping it.");
                return;
            }
            AccountModel account = GetAccount(accountId);
            if (account is null)
            {
                Logger.Warn($"Received a push notification for an unknown account ID '{accountId}'. Dropping it.");
                return;
            }

            // Check if the account is enabled:
            // if (!account.enabled)
            // {
            //     Logger.Info($"Received a push notification for a deactivated account ('{account.bareJid}'). Dropping it.");
            // }

            // Validate sender:
            string from = GetValue(doc.Root, "last-message-sender");
            if (from is null)
            {
                Logger.Warn("Received a push notification without the 'last-message-sender' property. Dropping it.");
                return;
            }
            string chatBareJid = null;
            if (Utils.isBareJid(from))
            {
                chatBareJid = from;
            }
            else if (Utils.isFullJid(from))
            {
                chatBareJid = Utils.getBareJidFromFullJid(from);
            }

            if (chatBareJid is null)
            {
                Logger.Warn($"Received a push notification with a invalid 'last-message-sender' property ('{from}'). Dropping it.");
                return;
            }

            // Body:
            string body = GetValue(doc.Root, "last-message-body");
            if (string.IsNullOrEmpty(body))
            {
                body = "You received a new message ✉.";
            }

            // Init the app service connection:
            appServiceConnection = new AppServiceConnection
            {
                AppServiceName = "uwpx.status",
                PackageFamilyName = Windows.ApplicationModel.Package.Current.Id.FamilyName
            };

            bool appRunning = await IsAppRunningAsync();
            if (appRunning)
            {
                bool accountConnected = await IsAccountConnectedAsync(account.bareJid);
                if (accountConnected)
                {
                    Logger.Info("No need to toast push. Account already connected.");
                    return;
                }
            }

            // Get chat:
            ChatModel chat = GetChat(account.bareJid, chatBareJid);
            if (chat is null)
            {
                ToastHelper.ShowPushChatTextToast(body, chatBareJid);
                if (!appRunning)
                {
                    ToastHelper.IncBadgeCount();
                }
            }
            else if (!chat.muted)
            {
                ToastHelper.ShowPushChatTextToast(body, chat);
                if (!appRunning)
                {
                    ToastHelper.IncBadgeCount();
                }
            }
            else
            {
                Logger.Debug("Muted chat. Discarding push.");
            }
        }

        #endregion

        #region --Misc Methods (Protected)--


        #endregion
        //--------------------------------------------------------Events:---------------------------------------------------------------------\\
        #region --Events--


        #endregion
    }
}
