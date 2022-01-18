using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Logging;
using XMPP_API.Classes.Events;
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
        private delegate Task CheckDiscoResponseAsync(DiscoResponseMessage disco, string discoTarget);
        private readonly Dictionary<string, HashSet<string>> DISCO_INFO_RESULT = new Dictionary<string, HashSet<string>>();
        private readonly Dictionary<string, List<DiscoItem>> DISCO_ITEM_RESULT = new Dictionary<string, List<DiscoItem>>();

        public delegate void DicoFeaturesDicoveredEventHandler(DiscoFeatureHelper sender, DicoFeaturesDicoveredEventArgs args);
        public event DicoFeaturesDicoveredEventHandler DicoFeaturesDicovered;

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

        public List<DiscoItem> GetDiscoItems(string discoTarget)
        {
            return DISCO_ITEM_RESULT.ContainsKey(discoTarget) ? new List<DiscoItem>() : DISCO_ITEM_RESULT[discoTarget];
        }

        #endregion

        #region --Misc Methods (Private)--
        private async Task DiscoDomainPartAsync()
        {
            bool result = await DiscoAsync(CONNECTION.account.user.domainPart, CheckDiscoFeaturesDomainPartAsync, DiscoType.INFO);
            if (result)
            {
                result = await DiscoAsync(CONNECTION.account.user.domainPart, CheckDiscoItemsAsync, DiscoType.ITEMS);
                if (result)
                {
                    return;
                }
            }
            CONNECTION.account.CONNECTION_INFO.msgCarbonsState = MessageCarbonsState.ERROR;
        }

        private async Task DiscoBareJidAsync()
        {
            bool result = await DiscoAsync(CONNECTION.account.user.getBareJid(), CheckDiscoFeaturesBareJidAsync, DiscoType.INFO);
            if (result)
            {
                await DiscoAsync(CONNECTION.account.user.getBareJid(), CheckDiscoItemsAsync, DiscoType.ITEMS);
            }
        }

        private async Task<bool> DiscoAsync(string discoTarget, CheckDiscoResponseAsync action, DiscoType discoType)
        {
            MessageResponseHelperResult<IQMessage> result = await CONNECTION.GENERAL_COMMAND_HELPER.discoAsync(discoTarget, discoType);
            if (result.STATE != MessageResponseHelperResultState.SUCCESS)
            {
                Logger.Error($"Failed to perform server DISCO#{discoType} for '{CONNECTION.account.getBareJid()}' - {result.STATE}");
                return false;
            }

            if (result.RESULT is IQErrorMessage errorMessage)
            {
                Logger.Error($"Failed to perform server DISCO#{discoType} for '{CONNECTION.account.getBareJid()}' - {errorMessage.ERROR_OBJ}");
                return false;
            }

            // Success:
            if (result.RESULT is DiscoResponseMessage disco)
            {
                await OnDiscoResponseMessage(disco, action, discoTarget);
                return true;
            }

            Logger.Error($"Failed to perform server DISCO#{discoType} for '{CONNECTION.account.getBareJid()}' - invalid response.");
            return false;
        }

        private async Task OnDiscoResponseMessage(DiscoResponseMessage disco, CheckDiscoResponseAsync action, string discoTarget)
        {
            // Print a list of all supported features:
            if (Logger.logLevel >= LogLevel.DEBUG)
            {
                StringBuilder sb = new StringBuilder("Disco#");
                sb.Append(disco.DISCO_TYPE);
                sb.Append(" for '");
                sb.Append(CONNECTION.account.getBareJid());
                if (disco.DISCO_TYPE == DiscoType.INFO)
                {
                    sb.Append("' reported the following features:\n");
                    foreach (string s in disco.FEATURES.Select(x => x.VAR))
                    {
                        sb.Append(s);
                        sb.Append("\n");
                    }
                }
                else
                {
                    sb.Append("' reported the following items:\n");
                    foreach (DiscoItem i in disco.ITEMS)
                    {
                        sb.Append(i.JID);
                        if (!string.IsNullOrEmpty(i.NODE))
                        {
                            sb.Append(" -> ");
                            sb.Append(i.NODE);
                        }
                        if (!string.IsNullOrEmpty(i.NAME))
                        {
                            sb.Append($" ({i.NAME})");
                        }
                        sb.Append("\n");
                    }
                }
                Logger.Debug(sb.ToString());
            }

            await action(disco, discoTarget);
        }

#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
        private async Task CheckDiscoItemsAsync(DiscoResponseMessage disco, string discoTarget)
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
        {
            DISCO_ITEM_RESULT[discoTarget] = disco.ITEMS;
            Logger.Info($"Disco items for '{discoTarget}' updated.");
        }

        private async Task CheckDiscoFeaturesDomainPartAsync(DiscoResponseMessage disco, string discoTarget)
        {
            AddFeaturesForTarget(disco.FEATURES, discoTarget);
            DicoFeaturesDicovered?.Invoke(this, new DicoFeaturesDicoveredEventArgs(DISCO_INFO_RESULT[discoTarget], discoTarget));

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
            Logger.Info($"Disco features for '{discoTarget}' updated.");
        }

#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
        private async Task CheckDiscoFeaturesBareJidAsync(DiscoResponseMessage disco, string discoTarget)
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
        {
            AddFeaturesForTarget(disco.FEATURES, discoTarget);
            DicoFeaturesDicovered?.Invoke(this, new DicoFeaturesDicoveredEventArgs(DISCO_INFO_RESULT[discoTarget], discoTarget));
        }

        #endregion

        #region --Misc Methods (Protected)--


        #endregion
        //--------------------------------------------------------Events:---------------------------------------------------------------------\\
        #region --Events--


        #endregion
    }
}
