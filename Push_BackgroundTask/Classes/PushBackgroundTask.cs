using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Logging;
using Manager.Classes.Toast;
using Storage.Classes.Contexts;
using Storage.Classes.Models.Chat;
using Windows.ApplicationModel.Background;
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

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--


        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--
        private string GetAccountBareJid(string accountId)
        {
            List<string> accounts;
            using (MainDbContext ctx = new MainDbContext())
            {
                accounts = ctx.Accounts.Select(a => a.bareJid).ToList();
            }
            return accounts.Where(a => string.Equals(Push.Classes.Utils.ToAccountId(a), accountId)).FirstOrDefault();
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

        private static bool IsTestMessage(XElement node)
        {
            return !(XMLUtils.getNodeFromXElement(node, "test") is null);
        }

        #endregion
        //--------------------------------------------------------Misc Methods:---------------------------------------------------------------\\
        #region --Misc Methods (Public)--
        public void Run(IBackgroundTaskInstance taskInstance)
        {
            deferral = taskInstance.GetDeferral();
            if (taskInstance.TriggerDetails is RawNotification notification)
            {
                ParseAndShowNotification(notification.Content);
            }
            deferral.Complete();
        }

        #endregion

        #region --Misc Methods (Private)--
        private void ParseAndShowNotification(string s)
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
            if (IsTestMessage(doc.Root))
            {
                ToastHelper.ShowSimpleToast("Here is your test push message successfully received from the push server!🎉");
                Logger.Info("Test push message received.");
                return;
            }

            // Validate account:
            string accountId = GetAccountId(doc.Root);
            if (accountId is null)
            {
                Logger.Warn($"Received a push notification without a valid account ID. Dropping it.");
                return;
            }
            string accountBareJid = GetAccountBareJid(accountId);
            if (accountBareJid is null)
            {
                Logger.Warn($"Received a push notification for an unknown account ID '{accountId}'. Dropping it.");
                return;
            }

            // Validate sender:
            string from = GetValue(doc.Root, "last-message-sender");
            if (from is null)
            {
                Logger.Warn("Received a push notification without the 'last-message-sender' property. Dropping it.");
                return;
            }
            string bareJid = null;
            if (Utils.isBareJid(from))
            {
                bareJid = from;
            }
            else if (Utils.isFullJid(from))
            {
                bareJid = Utils.getBareJidFromFullJid(from);
            }

            if (bareJid is null)
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

            // Get chat:
            ChatModel chat = GetChat(accountBareJid, bareJid);
            if (chat is null)
            {
                ToastHelper.ShowPushChatTextToast(body, bareJid);
            }
            else
            {
                ToastHelper.ShowPushChatTextToast(body, chat);
            }
            ToastHelper.SetBadgeNewMessages(true);
        }

        #endregion

        #region --Misc Methods (Protected)--


        #endregion
        //--------------------------------------------------------Events:---------------------------------------------------------------------\\
        #region --Events--


        #endregion
    }
}
