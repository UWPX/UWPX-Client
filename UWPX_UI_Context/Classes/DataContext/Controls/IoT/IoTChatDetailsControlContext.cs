using System.Collections.Generic;
using System.Threading.Tasks;
using Logging;
using UWPX_UI_Context.Classes.DataTemplates;
using UWPX_UI_Context.Classes.DataTemplates.Controls.IoT;
using XMPP_API.Classes;
using XMPP_API.Classes.Network.XML.Messages;
using XMPP_API.Classes.Network.XML.Messages.Helper;
using XMPP_API.Classes.Network.XML.Messages.XEP_0030;
using XMPP_API.Classes.Network.XML.Messages.XEP_0060;
using XMPP_API.Classes.Network.XML.Messages.XEP_IoT;

namespace UWPX_UI_Context.Classes.DataContext.Controls.IoT
{
    public sealed class IoTChatDetailsControlContext
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        public readonly IoTChatDetailsControlDataTemplate MODEL = new IoTChatDetailsControlDataTemplate();

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--


        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--


        #endregion
        //--------------------------------------------------------Misc Methods:---------------------------------------------------------------\\
        #region --Misc Methods (Public)--
        public async Task LoadAsync(ChatDataTemplate chat)
        {
            MODEL.IsLoading = true;
            string pubSubServer = "pubsub." + chat.Client.getXMPPAccount().user.domainPart;

            // Request nodes:
            MessageResponseHelperResult<IQMessage> result = await chat.Client.PUB_SUB_COMMAND_HELPER.discoNodesAsync(pubSubServer);
            if (result.STATE == MessageResponseHelperResultState.SUCCESS && result.RESULT is DiscoResponseMessage discoResponse)
            {
                await SubscribeToIoTNodesAsync(discoResponse.ITEMS, chat.Client, pubSubServer);
            }
            else
            {
                Logger.Warn("Failed to request PubSub nodes from: " + pubSubServer);
            }
            MODEL.IsLoading = false;
        }

        public void UpdateView(ChatDataTemplate chat)
        {
            if (!(chat is null))
            {
                Task.Run(async () =>
                {
                    await LoadAsync(chat);
                });
            }
        }

        #endregion

        #region --Misc Methods (Private)--
        private async Task SubscribeToIoTNodesAsync(List<DiscoItem> items, XMPPClient client, string pubSubServer)
        {
            bool foundUiNode = false;
            bool foundSensorsNode = false;
            bool foundActuatorsNode = false;
            foreach (DiscoItem item in items)
            {
                if (!foundUiNode && string.Equals(IoTConsts.NODE_NAME_UI, item.NODE))
                {
                    foundUiNode = true;
                    continue;
                }

                if (!foundSensorsNode && string.Equals(IoTConsts.NODE_NAME_SENSORS, item.NODE))
                {
                    foundSensorsNode = true;
                    continue;
                }

                if (!foundActuatorsNode && string.Equals(IoTConsts.NODE_NAME_ACTUATORS, item.NODE))
                {
                    foundActuatorsNode = true;
                    continue;
                }
            }

            // Subscribe to the UI node:
            if (foundUiNode)
            {
                await subscribeToNodeAsync(IoTConsts.NODE_NAME_UI, client, pubSubServer);
            }
            else
            {
                Logger.Warn("IoT node " + IoTConsts.NODE_NAME_UI + " not found!");
            }

            // Subscribe to the sensors node:
            if (foundSensorsNode)
            {
                await subscribeToNodeAsync(IoTConsts.NODE_NAME_SENSORS, client, pubSubServer);
            }
            else
            {
                Logger.Warn("IoT node " + IoTConsts.NODE_NAME_SENSORS + " not found!");
            }

            // Subscribe to the actuators node:
            if (foundActuatorsNode)
            {
                await subscribeToNodeAsync(IoTConsts.NODE_NAME_ACTUATORS, client, pubSubServer);
            }
            else
            {
                Logger.Warn("IoT node " + IoTConsts.NODE_NAME_ACTUATORS + " not found!");
            }
        }

        private async Task<bool> subscribeToNodeAsync(string nodeName, XMPPClient client, string pubSubServer)
        {
            MessageResponseHelperResult<IQMessage> result = await client.PUB_SUB_COMMAND_HELPER.requestNodeSubscriptionAsync(pubSubServer, nodeName);
            if (result.STATE == MessageResponseHelperResultState.SUCCESS)
            {
                if (result.RESULT is PubSubSubscriptionsResultMessage pubSubResult)
                {
                    Logger.Debug("Subscribed to node: " + nodeName);
                    return true;
                }
                else if (result.RESULT is IQErrorMessage errorMessage)
                {
                    Logger.Warn("Subscribing to node failed with: " + errorMessage.ERROR_OBJ.ToString());
                }
            }
            else
            {
                Logger.Warn("Subscribing to node failed with: " + result.RESULT);
            }

            return false;
        }

        #endregion

        #region --Misc Methods (Protected)--


        #endregion
        //--------------------------------------------------------Events:---------------------------------------------------------------------\\
        #region --Events--


        #endregion
    }
}
