using System.Collections.Generic;
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
            MessageResponseHelperResult<IQMessage> result = await CONNECTION.GENERAL_COMMAND_HELPER.discoAsync(CONNECTION.account.user.domainPart, DiscoType.INFO);
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
                await OnDiscoResponseMessage(disco);
                return;
            }
            else
            {
                Logger.Error("Failed to perform server DISCO for '" + CONNECTION.account.getBareJid() + "' - invalid response.");
            }

            CONNECTION.account.CONNECTION_INFO.msgCarbonsState = MessageCarbonsState.ERROR;
            CONNECTION.account.CONNECTION_INFO.pushState = PushState.ERROR;
        }

        private async Task OnDiscoResponseMessage(DiscoResponseMessage disco)
        {
            switch (disco.DISCO_TYPE)
            {
                case DiscoType.ITEMS:
                    break;

                case DiscoType.INFO:
                    await CheckDiscoFeaturesAsync(disco.FEATURES);
                    break;

                default:
                    break;
            }
        }

        private async Task CheckDiscoFeaturesAsync(List<DiscoFeature> features)
        {
            bool foundCarbons = false;
            bool foundPush = !CONNECTION.account.pushEnabled || CONNECTION.account.pushNodePublished;
            if (!CONNECTION.account.pushEnabled)
            {
                CONNECTION.account.CONNECTION_INFO.msgCarbonsState = MessageCarbonsState.DISABLED;
            }
            else if (CONNECTION.account.pushNodePublished)
            {
                CONNECTION.account.CONNECTION_INFO.msgCarbonsState = MessageCarbonsState.ENABLED;
                Logger.Info("No need to enable to for '" + CONNECTION.account.getBareJid() + "' - already enabled");
            }

            foreach (DiscoFeature f in features)
            {
                // Check if the server supports 'XEP-0280: Message Carbons':
                if (!foundCarbons && string.Equals(f.VAR, Consts.XML_XEP_0280_NAMESPACE))
                {
                    foundCarbons = true;
                    await CONNECTION.EnableMessageCarbonsAsync();
                    continue;
                }

                // Check if the server supports 'XEP-0357: Push Notifications':
                else if (!foundPush && string.Equals(f.VAR, Consts.XML_XEP_0357_NAMESPACE))
                {
                    foundPush = true;
                    await CONNECTION.EnablePushNotificationsAsync();
                    continue;
                }
            }

            if (!foundCarbons)
            {
                CONNECTION.account.CONNECTION_INFO.msgCarbonsState = MessageCarbonsState.NOT_SUPPORTED;
                Logger.Warn("Unable to enable message carbons for '" + CONNECTION.account.getBareJid() + "' - not supported by the server.");
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
