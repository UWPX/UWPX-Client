using System;
using System.Xml.Linq;
using Logging;
using Manager.Classes.Toast;
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

            // Validate the sender:
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

            string body = GetValue(doc.Root, "last-message-body");
            ToastHelper.ShowSimpleToast(body);

            ToastHelper.ShowSimpleToast(s);
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
                        System.Collections.Generic.IEnumerable<XAttribute> z = n.Attributes();
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

        #endregion

        #region --Misc Methods (Protected)--


        #endregion
        //--------------------------------------------------------Events:---------------------------------------------------------------------\\
        #region --Events--


        #endregion
    }
}
