﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using XMPP_API.Classes.Network;
using XMPP_API.Classes.Network.XML.Messages;
using XMPP_API.Classes.Network.XML.Messages.Helper;
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
        private readonly XmppConnection CONNECTION;

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        /// <summary>
        /// Basic Constructor
        /// </summary>
        /// <history>
        /// 08/03/2018 Created [Fabian Sauter]
        /// </history>
        public MUCCommandHelper(XmppConnection connection)
        {
            CONNECTION = connection;
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
        public MessageResponseHelper<IQMessage> requestRooms(string server, MessageResponseHelper<IQMessage>.OnMessageHandler onMessage, MessageResponseHelper<IQMessage>.OnTimeoutHandler onTimeout)
        {
            return CONNECTION.GENERAL_COMMAND_HELPER.createDisco(server, DiscoType.ITEMS, onMessage, onTimeout);
        }

        /// <summary>
        /// Sends a DiscoRequestMessage and requests the MUC room info for the given room.
        /// </summary>
        /// <param name="roomJid">The bare JID if the room you would like to request the information for. e.g. 'witches@conference.jabber.org'</param>
        /// <param name="onMessage">The method that should get executed once the helper receives a new valid message.</param>
        /// <param name="onTimeout">The method that should get executed once the helper timeout gets triggered.</param>
        /// <returns>Returns a MessageResponseHelper listening for DiscoRequestMessage answers.</returns>
        public MessageResponseHelper<ExtendedDiscoResponseMessage> requestRoomInfo(string roomJid, MessageResponseHelper<ExtendedDiscoResponseMessage>.OnMessageHandler onMessage, MessageResponseHelper<ExtendedDiscoResponseMessage>.OnTimeoutHandler onTimeout)
        {
            MessageResponseHelper<ExtendedDiscoResponseMessage> helper = new MessageResponseHelper<ExtendedDiscoResponseMessage>(CONNECTION, onMessage, onTimeout);
            DiscoRequestMessage disco = new DiscoRequestMessage(CONNECTION.account.getFullJid(), roomJid, DiscoType.INFO);
            helper.start(disco);
            return helper;
        }

        /// <summary>
        /// Sends a <see cref="RequestRoomConfigurationMessage"/> and requests the current room configuration.
        /// </summary>
        /// <param name="roomJid">The bare JID if the room you would like to request the room configuration for. e.g. 'witches@conference.jabber.org'</param>
        /// <returns>The <see cref="RequestRoomConfigurationMessage"/> result</returns>
        public Task<MessageResponseHelperResult<IQMessage>> requestRoomConfigurationAsync(string roomJid)
        {
            Predicate<IQMessage> predicate = (x) => { return x is RoomConfigMessage || x is IQErrorMessage; };
            AsyncMessageResponseHelper<IQMessage> helper = new AsyncMessageResponseHelper<IQMessage>(CONNECTION, predicate);
            RequestRoomConfigurationMessage msg = new RequestRoomConfigurationMessage(roomJid, MUCAffiliation.OWNER);
            return helper.startAsync(msg);
        }

        /// <summary>
        /// Sends a <see cref="RoomConfigMessage"/> and saves the given room configuration.
        /// </summary>
        /// <param name="roomJid">The bare JID if the room you would like to save the room configuration for. e.g. 'witches@conference.jabber.org'</param>
        /// <param name="roomConfiguration">The new room configuration.</param>
        /// <returns>The <see cref="RoomConfigMessage"/> result</returns>
        public Task<MessageResponseHelperResult<IQMessage>> saveRoomConfigurationAsync(string roomJid, DataForm roomConfiguration)
        {
            Predicate<IQMessage> predicate = (x) => { return true; };
            AsyncMessageResponseHelper<IQMessage> helper = new AsyncMessageResponseHelper<IQMessage>(CONNECTION, predicate);
            RoomConfigMessage msg = new RoomConfigMessage(CONNECTION.account.getFullJid(), roomJid, roomConfiguration, MUCAffiliation.OWNER);
            return helper.startAsync(msg);
        }

        /// <summary>
        /// Sends a MUCChangeNicknameMessage for changing your own MUC nickname.
        /// </summary>
        /// <param name="roomJid">The bare JID if the room you would like to change your nickname for. e.g. 'witches@conference.jabber.org'</param>
        /// <param name="newNickname">The new nickname for the given room.</param>
        /// <param name="onMessage">The method that should get executed once the helper receives a new valid message.</param>
        /// <param name="onTimeout">The method that should get executed once the helper timeout gets triggered.</param>
        /// <returns>Returns a MessageResponseHelper listening for MUCChangeNicknameMessage answers.</returns>
        public MessageResponseHelper<PresenceMessage> changeNickname(string roomJid, string newNickname, MessageResponseHelper<PresenceMessage>.OnMessageHandler onMessage, MessageResponseHelper<PresenceMessage>.OnTimeoutHandler onTimeout)
        {
            MessageResponseHelper<PresenceMessage> helper = new MessageResponseHelper<PresenceMessage>(CONNECTION, onMessage, onTimeout)
            {
                matchId = false
            };
            MUCChangeNicknameMessage msg = new MUCChangeNicknameMessage(CONNECTION.account.getFullJid(), roomJid, newNickname);
            helper.start(msg);
            return helper;
        }

        /// <summary>
        /// Sends a MUCChangeNicknameMessage for changing your own MUC nickname.
        /// </summary>
        /// <param name="roomJid">The bare JID if the room you would like to change your nickname for. e.g. 'witches@conference.jabber.org'</param>
        /// <param name="newNickname">The new nickname for the given room.</param>
        /// <returns>Returns a MessageResponseHelper listening for MUCChangeNicknameMessage answers.</returns>
        public Task<MessageResponseHelperResult<MUCMemberPresenceMessage>> changeNicknameAsync(string roomJid, string newNickname)
        {
            Predicate<MUCMemberPresenceMessage> predicate = (x) =>
            {
                return x.getFrom().Contains(roomJid) &&
                    ((x.STATUS_CODES.Contains(MUCPresenceStatusCode.PRESENCE_SELFE_REFERENCE) && x.STATUS_CODES.Contains(MUCPresenceStatusCode.MEMBER_NICK_CHANGED)) ||
                        (x.STATUS_CODES.Contains(MUCPresenceStatusCode.PRESENCE_SELFE_REFERENCE) && x.STATUS_CODES.Contains(MUCPresenceStatusCode.ROOM_NICK_CHANGED)) ||
                        !string.IsNullOrEmpty(x.ERROR_TYPE));
            };
            AsyncMessageResponseHelper<MUCMemberPresenceMessage> helper = new AsyncMessageResponseHelper<MUCMemberPresenceMessage>(CONNECTION, predicate)
            {
                matchId = false
            };
            MUCChangeNicknameMessage msg = new MUCChangeNicknameMessage(CONNECTION.account.getFullJid(), roomJid, newNickname);
            return helper.startAsync(msg);
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
        public MessageResponseHelper<IQMessage> kickOccupant(string roomJid, string nickname, string reason, MessageResponseHelper<IQMessage>.OnMessageHandler onMessage, MessageResponseHelper<IQMessage>.OnTimeoutHandler onTimeout)
        {
            MessageResponseHelper<IQMessage> helper = new MessageResponseHelper<IQMessage>(CONNECTION, onMessage, onTimeout);
            KickOccupantMessage msg = new KickOccupantMessage(CONNECTION.account.getFullJid(), roomJid, nickname, reason);
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
        public MessageResponseHelper<IQMessage> banOccupant(string roomJid, string jid, string reason, MessageResponseHelper<IQMessage>.OnMessageHandler onMessage, MessageResponseHelper<IQMessage>.OnTimeoutHandler onTimeout)
        {
            MessageResponseHelper<IQMessage> helper = new MessageResponseHelper<IQMessage>(CONNECTION, onMessage, onTimeout);
            BanOccupantMessage msg = new BanOccupantMessage(CONNECTION.account.getFullJid(), roomJid, jid, reason);
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
        public MessageResponseHelper<IQMessage> requestBanList(string roomJid, MessageResponseHelper<IQMessage>.OnMessageHandler onMessage, MessageResponseHelper<IQMessage>.OnTimeoutHandler onTimeout)
        {
            MessageResponseHelper<IQMessage> helper = new MessageResponseHelper<IQMessage>(CONNECTION, onMessage, onTimeout);
            BanListMessage msg = new BanListMessage(CONNECTION.account.getFullJid(), roomJid);
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
        public MessageResponseHelper<IQMessage> updateBanList(string roomJid, List<BanedUser> changedUsers, MessageResponseHelper<IQMessage>.OnMessageHandler onMessage, MessageResponseHelper<IQMessage>.OnTimeoutHandler onTimeout)
        {
            MessageResponseHelper<IQMessage> helper = new MessageResponseHelper<IQMessage>(CONNECTION, onMessage, onTimeout);
            UpdateBanListMessage msg = new UpdateBanListMessage(CONNECTION.account.getFullJid(), roomJid, changedUsers);
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
