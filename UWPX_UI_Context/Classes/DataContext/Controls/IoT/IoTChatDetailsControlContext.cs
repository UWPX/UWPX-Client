using System.Collections.Generic;
using System.Threading.Tasks;
using Logging;
using Manager.Classes.Chat;
using UWPX_UI_Context.Classes.DataTemplates.Controls.IoT;
using XMPP_API.Classes;
using XMPP_API.Classes.Network.Events;
using XMPP_API.Classes.Network.XML.Messages;
using XMPP_API.Classes.Network.XML.Messages.Helper;
using XMPP_API.Classes.Network.XML.Messages.XEP_0004;
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
        public async Task LoadAsync()
        {
            MODEL.IsLoading = true;
            // Unsubscribe while we are loading:
            MODEL.Chat.Client.xmppClient.NewPubSubEvent -= Client_NewPubSubEvent;

            // Request nodes:
            string targetBareJid = MODEL.Chat.Chat.bareJid;
            MessageResponseHelperResult<IQMessage> result = await MODEL.Chat.Client.xmppClient.PUB_SUB_COMMAND_HELPER.discoNodesAsync(targetBareJid);
            if (result.STATE == MessageResponseHelperResultState.SUCCESS && result.RESULT is DiscoResponseMessage discoResponse)
            {
                await SubscribeToIoTNodesAsync(discoResponse.ITEMS, MODEL.Chat.Client.xmppClient, targetBareJid);
                await RequestUiNodeAsync(MODEL.Chat.Client.xmppClient, targetBareJid);
                await RequestSensorsNodeAsync(MODEL.Chat.Client.xmppClient, targetBareJid);
                await RequestActuatorsNodeAsync(MODEL.Chat.Client.xmppClient, targetBareJid);
            }
            else
            {
                // Workaround since prosody does not allow us to query all nodes:
                await SubscribeToNodeAsync(IoTConsts.NODE_NAME_UI, MODEL.Chat.Client.xmppClient, targetBareJid);
                await SubscribeToNodeAsync(IoTConsts.NODE_NAME_SENSORS, MODEL.Chat.Client.xmppClient, targetBareJid);
                await SubscribeToNodeAsync(IoTConsts.NODE_NAME_ACTUATORS, MODEL.Chat.Client.xmppClient, targetBareJid);

                // Request Nodes:
                await RequestUiNodeAsync(MODEL.Chat.Client.xmppClient, targetBareJid);
                await RequestSensorsNodeAsync(MODEL.Chat.Client.xmppClient, targetBareJid);
                await RequestActuatorsNodeAsync(MODEL.Chat.Client.xmppClient, targetBareJid);

                Logger.Warn("Failed to request PubSub nodes from: " + targetBareJid);
            }
            // Subscribe again:
            MODEL.Chat.Client.xmppClient.NewPubSubEvent += Client_NewPubSubEvent;
            MODEL.IsLoading = false;
        }

        public void UpdateView(ChatDataTemplate chat)
        {
            MODEL.Chat = chat;
            if (!(chat is null))
            {
                _ = LoadAsync();
            }
        }

        #endregion

        #region --Misc Methods (Private)--
        private async Task RequestUiNodeAsync(XMPPClient client, string pubSubServer)
        {
            MessageResponseHelperResult<IQMessage> result = await client.PUB_SUB_COMMAND_HELPER.requestNodeAsync(pubSubServer, IoTConsts.NODE_NAME_UI, 0);
            if (result.STATE == MessageResponseHelperResultState.SUCCESS && result.RESULT is UiNodeItemsResponseMessage uiResponse)
            {
                UpdateForm(uiResponse.form);
            }
        }

        private async Task RequestSensorsNodeAsync(XMPPClient client, string pubSubServer)
        {
            MessageResponseHelperResult<IQMessage> result = await client.PUB_SUB_COMMAND_HELPER.requestNodeAsync(pubSubServer, IoTConsts.NODE_NAME_SENSORS, 0);
            if (result.STATE == MessageResponseHelperResultState.SUCCESS && result.RESULT is SensorsNodeItemsResponseMessage sensorsResponse)
            {
                UpdateFields(sensorsResponse.VALUES);
            }
        }

        private async Task RequestActuatorsNodeAsync(XMPPClient client, string pubSubServer)
        {
            MessageResponseHelperResult<IQMessage> result = await client.PUB_SUB_COMMAND_HELPER.requestNodeAsync(pubSubServer, IoTConsts.NODE_NAME_ACTUATORS, 0);
            if (result.STATE == MessageResponseHelperResultState.SUCCESS && result.RESULT is ActuatorsNodeItemsResponseMessage actuatorsResponse)
            {
                UpdateFields(actuatorsResponse.VALUES);
            }
        }

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
                await SubscribeToNodeAsync(IoTConsts.NODE_NAME_UI, client, pubSubServer);
            }
            else
            {
                Logger.Warn("IoT node " + IoTConsts.NODE_NAME_UI + " not found!");
            }

            // Subscribe to the sensors node:
            if (foundSensorsNode)
            {
                await SubscribeToNodeAsync(IoTConsts.NODE_NAME_SENSORS, client, pubSubServer);
            }
            else
            {
                Logger.Warn("IoT node " + IoTConsts.NODE_NAME_SENSORS + " not found!");
            }

            // Subscribe to the actuators node:
            if (foundActuatorsNode)
            {
                await SubscribeToNodeAsync(IoTConsts.NODE_NAME_ACTUATORS, client, pubSubServer);
            }
            else
            {
                Logger.Warn("IoT node " + IoTConsts.NODE_NAME_ACTUATORS + " not found!");
            }
        }

        private async Task<bool> SubscribeToNodeAsync(string nodeName, XMPPClient client, string pubSubServer)
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

        private void UpdateForm(DataForm form)
        {
            if (!(MODEL.Form is null))
            {
                MODEL.Form.FieldValueChangedByUser -= Form_FieldValueChangedByUser;
            }
            MODEL.Form = new DataFormDataTemplate(form);
            if (!(MODEL.Form is null))
            {
                MODEL.Form.FieldValueChangedByUser += Form_FieldValueChangedByUser;
            }
        }

        private async Task UpdateNodeAsync(Field field)
        {
            IoTValue value = new IoTValue(field);
            IoTPubSubItem item = new IoTPubSubItem(value, field.var);
            PublishIoTNodeMessage msg = new PublishIoTNodeMessage(MODEL.Chat.Client.dbAccount.fullJid.FullJid(), MODEL.Chat.Chat.bareJid, IoTConsts.NODE_NAME_ACTUATORS, item);
            await MODEL.Chat.Client.xmppClient.SendAsync(msg);
        }

        private void UpdateFields(List<IoTValue> values)
        {
            foreach (FieldDataTemplate f in MODEL.Form.FIELDS)
            {
                foreach (IoTValue val in values)
                {
                    if (string.Equals(val.ITEM_ID, f.Var))
                    {
                        f.Value = val.VALUE;
                        break;
                    }
                }
            }
        }

        #endregion

        #region --Misc Methods (Protected)--


        #endregion
        //--------------------------------------------------------Events:---------------------------------------------------------------------\\
        #region --Events--
        private void Client_NewPubSubEvent(XMPPClient client, NewPubSubEventEventArgs args)
        {
            if (args.MSG is AbstractValueNodeEventMessage abstractNodeEvent)
            {
                Logger.Debug("New PubSub node changed event message for: " + abstractNodeEvent.NODE_NAME);
                UpdateFields(abstractNodeEvent.VALUES);
            }
            else if (args.MSG is UiNodeEventMessage uiNodeEvent)
            {
                Logger.Debug("New PubSub UI node changed event message.");
                UpdateForm(uiNodeEvent.FORM);
            }
        }

        private async void Form_FieldValueChangedByUser(DataFormDataTemplate sender, Events.FieldValueChangedByUserEventArgs args)
        {
            Logger.Debug("Field '" + args.FIELD.Var + "' has been updated by the user. New value: " + args.FIELD.Value);
            await UpdateNodeAsync(args.FIELD.Field);
        }

        #endregion
    }
}
