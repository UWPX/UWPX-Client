using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Logging;
using XMPP_API.Classes.Network.XML.Messages;
using XMPP_API.Classes.Network.XML.Messages.Helper;
using XMPP_API.Classes.Network.XML.Messages.XEP_0030;

namespace XMPP_API.Classes.Network
{
    internal class DiscoFeatureHelper
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        private readonly XmppConnection CONNECTION;
        private delegate Task CheckDiscoFeaturesAsync(List<DiscoFeature> features);


        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        public DiscoFeatureHelper(XmppConnection connection)
        {
            CONNECTION = connection;
            connection.ConnectionStateChanged += Connection_ConnectionStateChanged;
        }

        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--


        #endregion
        //--------------------------------------------------------Misc Methods:---------------------------------------------------------------\\
        #region --Misc Methods (Public)--


        #endregion

        #region --Misc Methods (Private)--
        private async Task DiscoAsync()
        {
            await DiscoDomainPartAsync();
            await DiscoBareJidAsync();
        }

        private Task DiscoDomainPartAsync()
        {
            return DiscoAsync(CONNECTION.account.user.domainPart, CheckDiscoFeaturesDomainPartAsync);
        }

        private Task DiscoBareJidAsync()
        {
            return DiscoAsync(CONNECTION.account.user.getBareJid(), CheckDiscoFeaturesBareJidAsync);
        }

        private async Task DiscoAsync(string target, CheckDiscoFeaturesAsync action)
        {
            MessageResponseHelperResult<IQMessage> result = await CONNECTION.GENERAL_COMMAND_HELPER.discoAsync(target, DiscoType.INFO);
            if (result.STATE != MessageResponseHelperResultState.SUCCESS)
            {
                Logger.Error("Failed to perform server DISCO for '" + CONNECTION.account.getBareJid() + "' - " + result.STATE);
            }
            else if (result.RESULT is IQErrorMessage errorMessage)
            {
                Logger.Error("Failed to perform server DISCO for '" + CONNECTION.account.getBareJid() + "' - " + errorMessage.ERROR_OBJ.ToString());
            }
            // Success:
            else if (result.RESULT is DiscoResponseMessage disco)
            {
                await OnDiscoResponseMessage(disco, action);
                return;
            }
            else
            {
                Logger.Error("Failed to perform server DISCO for '" + CONNECTION.account.getBareJid() + "' - invalid response.");
            }

            CONNECTION.account.CONNECTION_INFO.msgCarbonsState = MessageCarbonsState.ERROR;
            CONNECTION.account.CONNECTION_INFO.pushState = PushState.ERROR;
        }

        private async Task OnDiscoResponseMessage(DiscoResponseMessage disco, CheckDiscoFeaturesAsync action)
        {
            // Print a list of all supported features:
            if (Logger.logLevel >= LogLevel.DEBUG)
            {
                StringBuilder sb = new StringBuilder("The server for '");
                sb.Append(CONNECTION.account.getBareJid());
                sb.Append("' supports the following features:\n");
                foreach (string s in disco.FEATURES.Select(x => x.VAR))
                {
                    sb.Append(s);
                    sb.Append("\n");
                }
                Logger.Debug(sb.ToString());
            }

            switch (disco.DISCO_TYPE)
            {
                case DiscoType.ITEMS:
                    break;

                case DiscoType.INFO:
                    await action(disco.FEATURES);
                    break;

                default:
                    break;
            }
        }

        private async Task CheckDiscoFeaturesDomainPartAsync(List<DiscoFeature> features)
        {
            if (CONNECTION.account.connectionConfiguration.disableMessageCarbons)
            {
                CONNECTION.account.CONNECTION_INFO.msgCarbonsState = MessageCarbonsState.DISABLED;
                Logger.Info("No need to enable message carbons for '" + CONNECTION.account.getBareJid() + "' - message carbons are disabled.");
                return;
            }

            bool foundCarbons = false;
            foreach (DiscoFeature f in features)
            {
                // Check if the server supports 'XEP-0280: Message Carbons':
                if (!foundCarbons && string.Equals(f.VAR, Consts.XML_XEP_0280_NAMESPACE))
                {
                    foundCarbons = true;
                    await CONNECTION.EnableMessageCarbonsAsync();
                    break;
                }
            }

            if (!foundCarbons)
            {
                CONNECTION.account.CONNECTION_INFO.msgCarbonsState = MessageCarbonsState.NOT_SUPPORTED;
                Logger.Warn("Unable to enable message carbons for '" + CONNECTION.account.getBareJid() + "' - not supported by the server.");
            }
        }

        private async Task CheckDiscoFeaturesBareJidAsync(List<DiscoFeature> features)
        {
            bool foundPush = !CONNECTION.account.pushEnabled || CONNECTION.account.pushNodePublished;
            if (!CONNECTION.account.pushEnabled)
            {
                CONNECTION.account.CONNECTION_INFO.pushState = PushState.DISABLED;
            }
            else if (CONNECTION.account.pushNodePublished)
            {
                CONNECTION.account.CONNECTION_INFO.pushState = PushState.ENABLED;
                Logger.Info("No need to enable push for '" + CONNECTION.account.getBareJid() + "' - already enabled");
            }

            foreach (DiscoFeature f in features)
            {
                // Check if the server supports 'XEP-0357: Push Notifications':
                if (!foundPush && string.Equals(f.VAR, Consts.XML_XEP_0357_NAMESPACE))
                {
                    foundPush = true;
                    await CONNECTION.EnablePushNotificationsAsync();
                    break;
                }
            }

            if (!foundPush)
            {
                CONNECTION.account.CONNECTION_INFO.pushState = PushState.NOT_SUPPORTED;
                Logger.Warn("Unable to enable push notifications for '" + CONNECTION.account.getBareJid() + "' - not supported by the server.");
            }
        }

        #endregion

        #region --Misc Methods (Protected)--


        #endregion
        //--------------------------------------------------------Events:---------------------------------------------------------------------\\
        #region --Events--
        private void Connection_ConnectionStateChanged(AbstractConnection sender, Events.ConnectionStateChangedEventArgs args)
        {
            if (args.newState == ConnectionState.CONNECTED)
            {
                _ = DiscoAsync();
            }
        }

        #endregion
    }
}
