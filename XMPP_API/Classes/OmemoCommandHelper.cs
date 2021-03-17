using System;
using System.Threading.Tasks;
using Logging;
using XMPP_API.Classes.Network;
using XMPP_API.Classes.Network.XML.Messages;
using XMPP_API.Classes.Network.XML.Messages.Helper;
using XMPP_API.Classes.Network.XML.Messages.XEP_0060;
using XMPP_API.Classes.Network.XML.Messages.XEP_0384;

namespace XMPP_API.Classes
{
    public class OmemoCommandHelper
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        private readonly XmppConnection CONNECTION;

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        /// <summary>
        /// Basic Constructor
        /// </summary>
        /// <history>
        /// 17/03/2018 Created [Fabian Sauter]
        /// </history>
        public OmemoCommandHelper(XmppConnection connection)
        {
            CONNECTION = connection;
        }

        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--
        /// <summary>
        /// Sends an OmemoSetBundleInformationMessage for updating the given bundle information.
        /// </summary>
        /// <param name="bundleInfo">The bundle information you want to update.</param>
        /// <returns>The OmemoSetBundleInformationMessage result.</returns>
        public async Task<MessageResponseHelperResult<IQMessage>> setBundleInfoAsync(OmemoBundleInformation bundleInfo)
        {
            Predicate<IQMessage> predicate = (x) => { return true; };
            AsyncMessageResponseHelper<IQMessage> helper = new AsyncMessageResponseHelper<IQMessage>(CONNECTION, predicate);
            OmemoSetBundleInformationMessage msg = new OmemoSetBundleInformationMessage(CONNECTION.account.getFullJid(), bundleInfo);
            return await helper.startAsync(msg);
        }

        /// <summary>
        /// Sends an OmemoSetDeviceListMessage to update your OMEMO device list with the given devices.
        /// </summary>
        /// <param name="devices">The new OMEMO device list.</param>
        /// <returns>The OmemoSetDeviceListMessage result.</returns>
        public async Task<MessageResponseHelperResult<IQMessage>> setDeviceListAsync(OmemoXmlDevices devices)
        {
            Predicate<IQMessage> predicate = (x) => { return true; };
            AsyncMessageResponseHelper<IQMessage> helper = new AsyncMessageResponseHelper<IQMessage>(CONNECTION, predicate);
            OmemoSetDeviceListMessage msg = new OmemoSetDeviceListMessage(CONNECTION.account.getFullJid(), devices);
            return await helper.startAsync(msg);
        }

        #endregion
        //--------------------------------------------------------Misc Methods:---------------------------------------------------------------\\
        #region --Misc Methods (Public)--
        /// <summary>
        /// Sends an OmemoRequestBundleInformationMessage to requests the OMEMO bundle information of the given target and deviceId.
        /// </summary>
        /// <param name="toBareJid">The bare JID of the target you want to request the OMEMO bundle information from. e.g. 'conference.jabber.org'</param>
        /// <param name="deviceId">The device id you want to request the OMEMO bundle information for.</param>
        /// <returns>The OmemoRequestBundleInformationMessage result.</returns>
        public async Task<MessageResponseHelperResult<IQMessage>> requestBundleInformationAsync(string toBareJid, uint deviceId)
        {
            Predicate<IQMessage> predicate = (x) => { return x is OmemoBundleInformationResultMessage || x is IQErrorMessage; };
            AsyncMessageResponseHelper<IQMessage> helper = new AsyncMessageResponseHelper<IQMessage>(CONNECTION, predicate);
            OmemoRequestBundleInformationMessage msg = new OmemoRequestBundleInformationMessage(CONNECTION.account.getFullJid(), toBareJid, deviceId);
            return await helper.startAsync(msg);
        }

        /// <summary>
        /// Sends an OmemoRequestDeviceListMessage to requests the OMEMO device list of the given target.
        /// </summary>
        /// <param name="toBareJid">The bare JID of the target you want to request the OMEMO bundle information from. e.g. 'conference.jabber.org'</param>
        /// <param name="deviceId">The device id you want to request the OMEMO device list for.</param>
        /// <returns>The OmemoRequestDeviceListMessage result.</returns>
        public async Task<MessageResponseHelperResult<IQMessage>> requestDeviceListAsync(string toBareJid)
        {
            Logger.Info($"Requesting OMEMO device list for {toBareJid} ...");
            Predicate<IQMessage> predicate = (x) => { return x is OmemoDeviceListResultMessage || x is IQErrorMessage; };
            AsyncMessageResponseHelper<IQMessage> helper = new AsyncMessageResponseHelper<IQMessage>(CONNECTION, predicate);
            OmemoRequestDeviceListMessage msg = new OmemoRequestDeviceListMessage(CONNECTION.account.getFullJid(), toBareJid);
            return await helper.startAsync(msg);
        }

        /// <summary>
        /// Sends an OmemoSubscribeToDeviceListMessage to subscribe to targets device list.
        /// </summary>
        /// <param name="toBareJid">The bare JID of the target you want to subscribe to. e.g. 'conference.jabber.org'</param>
        /// <returns>The OmemoSubscribeToDeviceListMessage result.</returns>
        public async Task<MessageResponseHelperResult<IQMessage>> subscribeToDeviceListAsync(string toBareJid)
        {
            Predicate<IQMessage> predicate = (x) => { return x is PubSubSubscriptionMessage || x is IQErrorMessage; };
            AsyncMessageResponseHelper<IQMessage> helper = new AsyncMessageResponseHelper<IQMessage>(CONNECTION, predicate);
            OmemoSubscribeToDeviceListMessage msg = new OmemoSubscribeToDeviceListMessage(CONNECTION.account.getFullJid(), CONNECTION.account.getBareJid(), toBareJid);
            return await helper.startAsync(msg);
        }

        /// <summary>
        /// Sends a PubSubDeleteNodeMessage to delete the device list node.
        /// </summary>
        /// <returns>The PubSubDeleteNodeMessage result.</returns>
        public async Task<MessageResponseHelperResult<IQMessage>> deleteDeviceListNodeAsync()
        {
            return await CONNECTION.PUB_SUB_COMMAND_HELPER.deleteNodeAsync(null, Consts.XML_XEP_0384_DEVICE_LIST_NODE);
        }

        #endregion

        #region --Misc Methods (Private)--


        #endregion

        #region --Misc Methods (Protected)--


        #endregion
        //--------------------------------------------------------Events:---------------------------------------------------------------------\\
        #region --Events--


        #endregion
    }
}
