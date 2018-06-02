using System;
using System.Collections.Generic;
using XMPP_API.Classes.Network.XML.Messages;
using XMPP_API.Classes.Network.XML.Messages.XEP_0004;
using XMPP_API.Classes.Network.XML.Messages.XEP_0030;
using XMPP_API.Classes.Network.XML.Messages.XEP_0045;
using XMPP_API.Classes.Network.XML.Messages.XEP_0045.Configuration;

namespace XMPP_API.Classes
{
    public class MUCCommandHelper
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
        /// 08/03/2018 Created [Fabian Sauter]
        /// </history>
        public MUCCommandHelper(XMPPClient client)
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
        /// Sends a DiscoRequestMessage and requests all MUC rooms for the given server.
        /// </summary>
        /// <param name="server">The server the rooms should get requested for. e.g. 'conference.jabber.org'</param>
        /// <param name="onMessage">The method that should get executed once the helper receives a new valid message.</param>
        /// <param name="onTimeout">The method that should get executed once the helper timeout gets triggered.</param>
        /// <returns>Returns a MessageResponseHelper listening for DiscoRequestMessage answers.</returns>
        public MessageResponseHelper<IQMessage> requestRooms(string server, Func<IQMessage, bool> onMessage, Action onTimeout)
        {
            MessageResponseHelper<IQMessage> helper = new MessageResponseHelper<IQMessage>(CLIENT, onMessage, onTimeout);
            DiscoRequestMessage disco = new DiscoRequestMessage(CLIENT.getXMPPAccount().getIdDomainAndResource(), server, DiscoType.ITEMS);
            helper.start(disco);
            return helper;
        }

        /// <summary>
        /// Sends a DiscoRequestMessage and requests the MUC room info for the given room.
        /// </summary>
        /// <param name="roomJid">The bare JID if the room you would like to request the information for. e.g. 'witches@conference.jabber.org'</param>
        /// <param name="onMessage">The method that should get executed once the helper receives a new valid message.</param>
        /// <param name="onTimeout">The method that should get executed once the helper timeout gets triggered.</param>
        /// <returns>Returns a MessageResponseHelper listening for DiscoRequestMessage answers.</returns>
        public MessageResponseHelper<ExtendedDiscoResponseMessage> requestRoomInfo(string roomJid, Func<ExtendedDiscoResponseMessage, bool> onMessage, Action onTimeout)
        {
            MessageResponseHelper<ExtendedDiscoResponseMessage> helper = new MessageResponseHelper<ExtendedDiscoResponseMessage>(CLIENT, onMessage, onTimeout);
            DiscoRequestMessage disco = new DiscoRequestMessage(CLIENT.getXMPPAccount().getIdDomainAndResource(), roomJid, DiscoType.INFO);
            helper.start(disco);
            return helper;
        }

        /// <summary>
        /// Sends a RequestRoomConfigurationMessage and requests the current room configuration.
        /// </summary>
        /// <param name="roomJid">The bare JID if the room you would like to request the room configuration for. e.g. 'witches@conference.jabber.org'</param>
        /// <param name="configLevel">The requested configuration level (the senders affiliation).</param>
        /// <param name="onMessage">The method that should get executed once the helper receives a new valid message.</param>
        /// <param name="onTimeout">The method that should get executed once the helper timeout gets triggered.</param>
        /// <returns>Returns a MessageResponseHelper listening for RequestRoomConfigurationMessage answers.</returns>
        public MessageResponseHelper<IQMessage> requestRoomConfiguration(string roomJid, MUCAffiliation configLevel, Func<IQMessage, bool> onMessage, Action onTimeout)
        {
            MessageResponseHelper<IQMessage> helper = new MessageResponseHelper<IQMessage>(CLIENT, onMessage, onTimeout);
            RequestRoomConfigurationMessage msg = new RequestRoomConfigurationMessage(roomJid, configLevel);
            helper.start(msg);
            return helper;
        }

        /// <summary>
        /// Sends a RoomInfoMessage and saves the given room configuration.
        /// </summary>
        /// <param name="roomJid">The bare JID if the room you would like to save the room configuration for. e.g. 'witches@conference.jabber.org'</param>
        /// <param name="roomConfiguration">The new room configuration.</param>
        /// <param name="configLevel">The requested configuration level (the senders affiliation).</param>
        /// <param name="onMessage">The method that should get executed once the helper receives a new valid message.</param>
        /// <param name="onTimeout">The method that should get executed once the helper timeout gets triggered.</param>
        /// <returns>Returns a MessageResponseHelper listening for RoomInfoMessage answers.</returns>
        public MessageResponseHelper<IQMessage> saveRoomConfiguration(string roomJid, DataForm roomConfiguration, MUCAffiliation configLevel, Func<IQMessage, bool> onMessage, Action onTimeout)
        {
            MessageResponseHelper<IQMessage> helper = new MessageResponseHelper<IQMessage>(CLIENT, onMessage, onTimeout);
            RoomInfoMessage msg = new RoomInfoMessage(CLIENT.getXMPPAccount().getIdDomainAndResource(), roomJid, roomConfiguration, configLevel);
            helper.start(msg);
            return helper;
        }

        /// <summary>
        /// Sends a MUCChangeNicknameMessage for changing your own MUC nickname.
        /// </summary>
        /// <param name="roomJid">The bare JID if the room you would like to change your nickname for. e.g. 'witches@conference.jabber.org'</param>
        /// <param name="newNickname">The new nickname for the given room.</param>
        /// <param name="onMessage">The method that should get executed once the helper receives a new valid message.</param>
        /// <param name="onTimeout">The method that should get executed once the helper timeout gets triggered.</param>
        /// <returns>Returns a MessageResponseHelper listening for MUCChangeNicknameMessage answers.</returns>
        public MessageResponseHelper<PresenceMessage> changeNickname(string roomJid, string newNickname, Func<PresenceMessage, bool> onMessage, Action onTimeout)
        {
            MessageResponseHelper<PresenceMessage> helper = new MessageResponseHelper<PresenceMessage>(CLIENT, onMessage, onTimeout)
            {
                matchId = false
            };
            MUCChangeNicknameMessage msg = new MUCChangeNicknameMessage(CLIENT.getXMPPAccount().getIdDomainAndResource(), roomJid, newNickname);
            helper.start(msg);
            return helper;
        }

        /// <summary>
        /// Sends a KickOccupantMessage to kick the given occupant from the given room.
        /// </summary>
        /// <param name="roomJid">The bare JID if the room you would like to kick the user for. e.g. 'witches@conference.jabber.org'</param>
        /// <param name="nickname">The occupants nickname that should get kicked.</param>
        /// <param name="reason">An optional reason why the occupant should get kicked.</param>
        /// <param name="onMessage">The method that should get executed once the helper receives a new valid message.</param>
        /// <param name="onTimeout">The method that should get executed once the helper timeout gets triggered.</param>
        /// <returns>Returns a MessageResponseHelper listening for KickOccupantMessage answers.</returns>
        public MessageResponseHelper<IQMessage> kickOccupant(string roomJid, string nickname, string reason, Func<IQMessage, bool> onMessage, Action onTimeout)
        {
            MessageResponseHelper<IQMessage> helper = new MessageResponseHelper<IQMessage>(CLIENT, onMessage, onTimeout);
            KickOccupantMessage msg = new KickOccupantMessage(CLIENT.getXMPPAccount().getIdDomainAndResource(), roomJid, nickname, reason);
            helper.start(msg);
            return helper;
        }

        /// <summary>
        /// Sends a BanOccupantMessage to ban the given occupant from the given room.
        /// </summary>
        /// <param name="roomJid">The bare JID if the room you would like to ban the user for. e.g. 'witches@conference.jabber.org'</param>
        /// <param name="jid">The bare JID of the occupant you want to ban. e.g. 'witch@jabber.org'</param>
        /// <param name="reason">An optional reason why the occupant should get kicked.</param>
        /// <param name="onMessage">The method that should get executed once the helper receives a new valid message.</param>
        /// <param name="onTimeout">The method that should get executed once the helper timeout gets triggered.</param>
        /// <returns>Returns a MessageResponseHelper listening for BanOccupantMessage answers.</returns>
        public MessageResponseHelper<IQMessage> banOccupant(string roomJid, string jid, string reason, Func<IQMessage, bool> onMessage, Action onTimeout)
        {
            MessageResponseHelper<IQMessage> helper = new MessageResponseHelper<IQMessage>(CLIENT, onMessage, onTimeout);
            BanOccupantMessage msg = new BanOccupantMessage(CLIENT.getXMPPAccount().getIdDomainAndResource(), roomJid, jid, reason);
            helper.start(msg);
            return helper;
        }

        /// <summary>
        /// Sends a BanListMessage for requesting the ban list of the given room.
        /// Only the room owner and admins can request this list.
        /// </summary>
        /// <param name="roomJid">The bare JID if the room you would like to request the ban list for. e.g. 'witches@conference.jabber.org'</param>
        /// <param name="onMessage">The method that should get executed once the helper receives a new valid message.</param>
        /// <param name="onTimeout">The method that should get executed once the helper timeout gets triggered.</param>
        /// <returns>Returns a MessageResponseHelper listening for BanListMessage answers.</returns>
        public MessageResponseHelper<IQMessage> requestBanList(string roomJid, Func<IQMessage, bool> onMessage, Action onTimeout)
        {
            MessageResponseHelper<IQMessage> helper = new MessageResponseHelper<IQMessage>(CLIENT, onMessage, onTimeout);
            BanListMessage msg = new BanListMessage(CLIENT.getXMPPAccount().getIdDomainAndResource(), roomJid);
            helper.start(msg);
            return helper;
        }

        /// <summary>
        /// Sends a UpdateBanListMessage which updates the ban list for the given room.
        /// </summary>
        /// <param name="roomJid">The bare JID if the room you would like to update the ban list for. e.g. 'witches@conference.jabber.org'</param>
        /// <param name="changedUsers"></param>
        /// <param name="onMessage">The method that should get executed once the helper receives a new valid message.</param>
        /// <param name="onTimeout">The method that should get executed once the helper timeout gets triggered.</param>
        /// <returns>Returns a MessageResponseHelper listening for UpdateBanListMessage answers.</returns>
        public MessageResponseHelper<IQMessage> updateBanList(string roomJid, List<BanedUser> changedUsers, Func<IQMessage, bool> onMessage, Action onTimeout)
        {
            MessageResponseHelper<IQMessage> helper = new MessageResponseHelper<IQMessage>(CLIENT, onMessage, onTimeout);
            UpdateBanListMessage msg = new UpdateBanListMessage(CLIENT.getXMPPAccount().getIdDomainAndResource(), roomJid, changedUsers);
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
