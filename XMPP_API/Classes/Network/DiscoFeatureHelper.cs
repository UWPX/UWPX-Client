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
    public class DiscoFeatureHelper
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        private readonly XmppConnection CONNECTION;
        private delegate Task CheckDiscoFeaturesAsync(List<DiscoFeature> features, string discoTarget);
        private readonly Dictionary<string, HashSet<string>> DISCO_INFO_RESULT = new Dictionary<string, HashSet<string>>();


        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        public DiscoFeatureHelper(XmppConnection connection)
        {
            CONNECTION = connection;
        }

        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--


        #endregion
        //--------------------------------------------------------Misc Methods:---------------------------------------------------------------\\
        #region --Misc Methods (Public)--
        public async Task DiscoAsync()
        {
            await DiscoDomainPartAsync();
            await DiscoBareJidAsync();
        }

        public bool HasFeature(string feature, string discoTarget)
        {
            return DISCO_INFO_RESULT.ContainsKey(discoTarget) && DISCO_INFO_RESULT[discoTarget].Contains(feature);
        }

        #endregion

        #region --Misc Methods (Private)--
        private async Task DiscoDomainPartAsync()
        {
            await DiscoAsync(CONNECTION.account.user.domainPart, CheckDiscoFeaturesDomainPartAsync);
        }

        private async Task DiscoBareJidAsync()
        {
            await DiscoAsync(CONNECTION.account.user.getBareJid(), CheckDiscoFeaturesBareJidAsync);
        }

        private async Task DiscoAsync(string discoTarget, CheckDiscoFeaturesAsync action)
        {
            MessageResponseHelperResult<IQMessage> result = await CONNECTION.GENERAL_COMMAND_HELPER.discoAsync(discoTarget, DiscoType.INFO);
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
                await OnDiscoResponseMessage(disco, action, discoTarget);
                return;
            }
            else
            {
                Logger.Error("Failed to perform server DISCO for '" + CONNECTION.account.getBareJid() + "' - invalid response.");
            }

            CONNECTION.account.CONNECTION_INFO.msgCarbonsState = MessageCarbonsState.ERROR;
            CONNECTION.account.CONNECTION_INFO.pushState = PushState.ERROR;
        }

        private async Task OnDiscoResponseMessage(DiscoResponseMessage disco, CheckDiscoFeaturesAsync action, string discoTarget)
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
                    await action(disco.FEATURES, discoTarget);
                    break;

                default:
                    break;
            }
        }

        private async Task CheckDiscoFeaturesDomainPartAsync(List<DiscoFeature> features, string discoTarget)
        {
            AddFeaturesForTarget(features, discoTarget);

            if (CONNECTION.account.connectionConfiguration.disableMessageCarbons)
            {
                CONNECTION.account.CONNECTION_INFO.msgCarbonsState = MessageCarbonsState.DISABLED;
                Logger.Info("No need to enable message carbons for '" + CONNECTION.account.getBareJid() + "' - message carbons are disabled.");
                return;
            }

            // Check if the server supports 'XEP-0280: Message Carbons':
            bool supportsCarbons = HasFeature(Consts.XML_XEP_0280_NAMESPACE, discoTarget);
            if (supportsCarbons)
            {
                await CONNECTION.EnableMessageCarbonsAsync();
            }
            else
            {
                CONNECTION.account.CONNECTION_INFO.msgCarbonsState = MessageCarbonsState.NOT_SUPPORTED;
                Logger.Warn("Unable to enable message carbons for '" + CONNECTION.account.getBareJid() + "' - not supported by the server.");
            }
        }

        private void AddFeaturesForTarget(List<DiscoFeature> features, string discoTarget)
        {
            HashSet<string> featureSet = new HashSet<string>();
            foreach (DiscoFeature f in features)
            {
                featureSet.Add(f.VAR);
            }
            DISCO_INFO_RESULT[discoTarget] = featureSet;
            Logger.Info("Features for " + discoTarget + " updated.");
        }

        private async Task CheckDiscoFeaturesBareJidAsync(List<DiscoFeature> features, string discoTarget)
        {
            AddFeaturesForTarget(features, discoTarget);

            // Check if the server supports 'XEP-0357: Push Notifications':
            bool supportsPush = HasFeature(Consts.XML_XEP_0357_NAMESPACE, discoTarget);
            if (supportsPush)
            {
                if (CONNECTION.account.pushEnabled)
                {
                    if (!CONNECTION.account.pushPublished)
                    {
                        await CONNECTION.EnablePushNotificationsAsync();
                    }
                    else
                    {
                        Logger.Info("No need to enable push for '" + CONNECTION.account.getBareJid() + "' - already enabled");
                    }
                    CONNECTION.account.CONNECTION_INFO.pushState = PushState.ENABLED;
                }
                else
                {
                    if (!CONNECTION.account.pushPublished)
                    {
                        await CONNECTION.DisbalePushNotificationsAsync();
                    }
                    else
                    {
                        Logger.Info("No need to disable push for '" + CONNECTION.account.getBareJid() + "' - already disabled");
                    }
                    CONNECTION.account.CONNECTION_INFO.pushState = PushState.DISABLED;
                }
            }
            else
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


        #endregion
    }
}
