namespace XMPP_API.Classes.Network.XML.Messages.XEP_0045
{
    // Source: https://xmpp.org/registrar/mucstatus.html
    public enum MUCPresenceStatusCode
    {
        UNKNOWN = -1,
        /// <summary>
        /// Inform user that any occupant is allowed to see the user's full JID.
        /// </summary>
        SEE_FULL_JID_ANYBODY = 100, // 100
        /// <summary>
        /// Inform user that his or her affiliation changed while not in the room.
        /// </summary>
        AFFILIATION_CHANGED_WHILE_NOT_CONNECTED = 101, // 101
        /// <summary>
        /// Inform occupants that room now shows unavailable members.
        /// </summary>
        ROOM_SHOWS_UNAVAILABLE_MEMBERS = 102, // 102
        /// <summary>
        /// Inform occupants that room now does not show unavailable members.
        /// </summary>
        PRESENCE_CHANGED_ROOMNICK = 103, // 103
        /// <summary>
        /// Inform occupants that a non-privacy-related room configuration change has occurred.
        /// </summary>
        ROOM_CONFIG_CHANGED_NON_PRIVACY_RELATED = 104, // 104
        /// <summary>
        /// Inform user that presence refers to one of its own room occupants.
        /// </summary>
        PRESENCE_SELFE_REFERENCE = 110, // 110
        /// <summary>
        /// Inform occupants that room logging is now enabled.
        /// </summary>
        ROOM_LOGGING_ENABLED = 170, // 170
        /// <summary>
        /// Inform occupants that room logging is now disabled.
        /// </summary>
        ROOM_LOGGING_DISABLED = 171, // 171
        /// <summary>
        /// Inform occupants that the room is now non-anonymous.
        /// </summary>
        ROOM_NON_ANONYMOUS = 172, // 172
        /// <summary>
        /// Inform occupants that the room is now semi-anonymous.
        /// </summary>
        ROOM_SEMI_ANONYMOUS = 173, // 173
        /// <summary>
        /// Inform occupants that the room is now fully-anonymous.
        /// </summary>
        ROOM_FULLY_ANONYMOUS = 174, // 174
        /// <summary>
        /// Inform user that a new room has been created.
        /// </summary>
        NEW_ROOM_CREATED = 201, // 201
        /// <summary>
        /// Inform user that the service has assigned or modified the occupant's roomnick.
        /// </summary>
        MEMBER_NICK_CHANGED = 210, // 210
        /// <summary>
        /// Inform user that he or she has been banned from the room.
        /// </summary>
        MEMBER_GOT_BANED = 301, // 301
        /// <summary>
        /// Inform all occupants of new room nickname.
        /// </summary>
        ROOM_NICK_CHANGED = 303, // 303
        /// <summary>
        /// Inform user that he or she has been kicked from the room.
        /// </summary>
        MEMBER_GOT_KICKED = 307, // 307
        /// <summary>
        /// Inform user that he or she is being removed from the room because of an affiliation change.
        /// </summary>
        MEMBER_GOT_REMOVED_AFFILIATION_CHANGED = 321, // 321
        /// <summary>
        /// Inform user that he or she is being removed from the room because the room has been changed to members-only and the user is not a member.
        /// </summary>
        MEMBER_GOT_REMOVED_ROOM_CHANGED_TO_MEMBERS_ONLY = 322, // 322
        /// <summary>
        /// Inform user that he or she is being removed from the room because of a system shutdown.
        /// </summary>
        MEMBER_GOT_REMOVED_SYSTEM_SHUTDOWN = 332 // 332

    }
}
