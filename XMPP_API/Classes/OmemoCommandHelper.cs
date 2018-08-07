using System;
using XMPP_API.Classes.Network.XML.Messages;
using XMPP_API.Classes.Network.XML.Messages.XEP_0384;

namespace XMPP_API.Classes
{
    public class OmemoCommandHelper
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        private readonly XMPPClient CLIENT;

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        /// <summary>
        /// Basic Constructor
        /// </summary>
        /// <history>
        /// 17/03/2018 Created [Fabian Sauter]
        /// </history>
        public OmemoCommandHelper(XMPPClient client)
        {
            this.CLIENT = client;
        }

        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--


        #endregion
        //--------------------------------------------------------Misc Methods:---------------------------------------------------------------\\
        #region --Misc Methods (Public)--
        /// <summary>
        /// Sends a OmemoRequestBundleInformationMessage to requests the OMEMO bundle information of the given target and deviceId.
        /// </summary>
        /// <param name="to">The bare JID of the target you want to request the OMEMO bundle information from. e.g. 'conference.jabber.org'</param>
        /// <param name="deviceId">The device id you want to request the OMEMO bundle information for.</param>
        /// <param name="onMessage">The method that should get executed once the helper receives a new valid message.</param>
        /// <param name="onTimeout">The method that should get executed once the helper timeout gets triggered.</param>
        /// <returns>Returns a MessageResponseHelper listening for OmemoRequestBundleInformationMessage answers.</returns>
        public MessageResponseHelper<IQMessage> requestBundleInformation(string to, uint deviceId, Func<IQMessage, bool> onMessage, Action onTimeout)
        {
            MessageResponseHelper<IQMessage> helper = new MessageResponseHelper<IQMessage>(CLIENT, onMessage, onTimeout);
            OmemoRequestBundleInformationMessage msg = new OmemoRequestBundleInformationMessage(CLIENT.getXMPPAccount().getIdDomainAndResource(), to, deviceId);
            helper.start(msg);
            return helper;
        }

        /// <summary>
        /// Sends a OmemoRequestDeviceListMessage to requests the OMEMO device list of the given target.
        /// </summary>
        /// <param name="to">The bare JID of the target you want to request the OMEMO device list from. e.g. 'conference.jabber.org'</param>
        /// <param name="onMessage">The method that should get executed once the helper receives a new valid message.</param>
        /// <param name="onTimeout">The method that should get executed once the helper timeout gets triggered.</param>
        /// <returns>Returns a MessageResponseHelper listening for OmemoRequestDeviceListMessage answers.</returns>
        public MessageResponseHelper<IQMessage> requestDeviceList(string to, Func<IQMessage, bool> onMessage, Action onTimeout)
        {
            MessageResponseHelper<IQMessage> helper = new MessageResponseHelper<IQMessage>(CLIENT, onMessage, onTimeout);
            OmemoRequestDeviceListMessage msg = new OmemoRequestDeviceListMessage(CLIENT.getXMPPAccount().getIdDomainAndResource(), to);
            helper.start(msg);
            return helper;
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
