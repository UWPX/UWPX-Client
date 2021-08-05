using System;
using System.Threading;
using System.Threading.Tasks;
using Logging;
using Storage.Classes.Models.Account;
using XMPP_API.Classes;
using XMPP_API.Classes.Events;
using XMPP_API.Classes.Network;
using XMPP_API.Classes.Network.Events;
using XMPP_API.Classes.Network.XML.Messages;
using XMPP_API.Classes.Network.XML.Messages.Helper;

namespace Manager.Classes.Push
{
    public class ClientPushManager
    {
        private enum PushManagerState
        {
            UNKNOWN,
            INITIALIZED,
            DEACTIVATED
        }
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        private readonly Client CLIENT;
        private PushManagerState pushManagerState = PushManagerState.UNKNOWN;
        private bool discoDone;
        private readonly SemaphoreSlim UPDATE_SEMA = new SemaphoreSlim(1);

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        public ClientPushManager(Client client)
        {
            CLIENT = client;
            CLIENT.xmppClient.connection.DISCO_HELPER.DicoFeaturesDicovered += OnDiscoFeaturesDiscovered;
            CLIENT.xmppClient.ConnectionStateChanged += OnClientConnectionStateChanged;
        }

        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--


        #endregion
        //--------------------------------------------------------Misc Methods:---------------------------------------------------------------\\
        #region --Misc Methods (Public)--
        /// <summary>
        /// Should be called once the PushManager changes its state to INITIALIZED.
        /// </summary>
        public void OnPushManagerInitialized()
        {
            pushManagerState = PushManagerState.INITIALIZED;
            TryUpdateClientPush();
        }

        /// <summary>
        /// Should be called once the PushManager changes its state to DEACTIVATED.
        /// </summary>
        public void OnPushManagerDeactivated()
        {
            pushManagerState = PushManagerState.DEACTIVATED;
            TryUpdateClientPush();
        }

        /// <summary>
        /// Should be called once anything like the node, secret, client state or push manager state changes to update the client push configuration.
        /// </summary>
        public void TryUpdateClientPush()
        {
            if (discoDone && CLIENT.xmppClient.isConnected() && pushManagerState != PushManagerState.UNKNOWN)
            {
                Task.Run(async () =>
                {
                    await UPDATE_SEMA.WaitAsync();
                    try
                    {
                        await UpdateClientPushAsync();
                    }
                    catch (Exception e)
                    {
                        Logger.Error($"Failed to update push for '{CLIENT.dbAccount.bareJid}' with: ", e);
                    }
                    UPDATE_SEMA.Release();
                });
            }
        }

        #endregion

        #region --Misc Methods (Private)--
        private async Task UpdateClientPushAsync()
        {
            PushAccountModel push = CLIENT.dbAccount.push;
            // Check if push is supported by the server:
            if (push.state != PushState.DISABLED && push.state != PushState.DISABLING && !CLIENT.xmppClient.connection.DISCO_HELPER.HasFeature(Consts.XML_XEP_0357_NAMESPACE, CLIENT.dbAccount.bareJid))
            {
                if (push.state != PushState.NOT_SUPPORTED)
                {
                    push.state = PushState.NOT_SUPPORTED;
                    push.Update();
                }
                Logger.Info($"Failed to enable push for account '{CLIENT.dbAccount.bareJid}' - not supported by the server.");
                return;
            }

            if (push.state == PushState.NOT_SUPPORTED)
            {
                push.state = PushState.ENABLING;
                push.Update();
            }

            switch (push.state)
            {
                case PushState.DISABLING:
                    await DisablePushAsync();
                    break;

                case PushState.ENABLING:
                    await EnablePushAsync();
                    break;

                case PushState.DISABLED:
                    Logger.Info($"Push for account '{CLIENT.dbAccount.bareJid}' already disabled.");
                    break;

                case PushState.ENABLED:
                    Logger.Info($"Push for account '{CLIENT.dbAccount.bareJid}' already enabled.");
                    break;

                // Should not happen:
                default:
                    throw new InvalidOperationException($"Should not happen! {nameof(PushState)} == {push.state}");
            }
        }

        private async Task EnablePushAsync()
        {
            PushAccountModel push = CLIENT.dbAccount.push;

            MessageResponseHelperResult<IQMessage> result = await CLIENT.xmppClient.GENERAL_COMMAND_HELPER.enablePushNotificationsAsync(push.bareJid, push.node, push.secret);
            if (result.STATE == MessageResponseHelperResultState.SUCCESS)
            {
                if (result.RESULT is IQErrorMessage errorMessage)
                {
                    Logger.Error($"Failed to enable push notifications for '{CLIENT.dbAccount.bareJid}' - " + errorMessage.ERROR_OBJ.ToString());
                }
                else if (result.RESULT.TYPE != IQMessage.RESULT)
                {
                    Logger.Error($"Failed to enable push notifications for '{CLIENT.dbAccount.bareJid}' - server responded with: " + result.RESULT.TYPE);
                }
                else
                {
                    push.state = PushState.ENABLED;
                    push.Update();
                    Logger.Info($"Successfully enabled push notifications for: '{CLIENT.dbAccount.bareJid}'");
                }
            }
            else
            {
                Logger.Error($"Failed to enable push notifications for '{CLIENT.dbAccount.bareJid}' - " + result.STATE);
            }
        }

        private async Task DisablePushAsync()
        {
            PushAccountModel push = CLIENT.dbAccount.push;

            MessageResponseHelperResult<IQMessage> result = await CLIENT.xmppClient.GENERAL_COMMAND_HELPER.disablePushNotificationsAsync(push.bareJid);
            if (result.STATE == MessageResponseHelperResultState.SUCCESS)
            {
                if (result.RESULT is IQErrorMessage errorMessage)
                {
                    Logger.Error($"Failed to disable push notifications for '{CLIENT.dbAccount.bareJid}' - " + errorMessage.ERROR_OBJ.ToString());
                }
                else if (result.RESULT.TYPE != IQMessage.RESULT)
                {
                    Logger.Error($"Failed to disable push notifications for '{CLIENT.dbAccount.bareJid}' - server responded with: " + result.RESULT.TYPE);
                }
                else
                {
                    push.state = PushState.ENABLED;
                    push.Update();
                    Logger.Info($"Successfully disabled push notifications for: '{CLIENT.dbAccount.bareJid}'");
                }
            }
            else
            {
                Logger.Error($"Failed to disable push notifications for '{CLIENT.dbAccount.bareJid}' - " + result.STATE);
            }
        }

        #endregion

        #region --Misc Methods (Protected)--


        #endregion
        //--------------------------------------------------------Events:---------------------------------------------------------------------\\
        #region --Events--
        private void OnDiscoFeaturesDiscovered(DiscoFeatureHelper sender, DicoFeaturesDicoveredEventArgs args)
        {
            if (args.TARGET.Equals(CLIENT.dbAccount.bareJid))
            {
                discoDone = true;
                TryUpdateClientPush();
            }
        }

        private void OnClientConnectionStateChanged(XMPPClient client, ConnectionStateChangedEventArgs args)
        {
            if (args.oldState == ConnectionState.CONNECTED)
            {
                discoDone = false;
            }
        }

        #endregion
    }
}
