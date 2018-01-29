using SQLite.Net.Attributes;
using XMPP_API.Classes;
using XMPP_API.Classes.Network.XML.Messages.XEP_0048_1_0;

namespace Data_Manager2.Classes.DBTables
{
    [Table(DBTableConsts.MUC_CHAT_INFO_TABLE)]
    public class MUCChatInfoTable
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        [PrimaryKey]
        // The id entry of the ChatTable
        public string chatId { get; set; }
        // The name of the chat
        public string name { get; set; }
        [NotNull]
        // The nickname for the chat
        public string nickname { get; set; }
        // The chat description
        public string description { get; set; }
        // The password for the MUC
        public string password { get; set; }
        // The current state for entering the MUC
        public MUCEnterState enterState { get; set; }
        // Whether to automatically enter the room as soon as the client is connected
        public bool autoEnterRoom { get; set; }

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        /// <summary>
        /// Basic Constructor
        /// </summary>
        /// <history>
        /// 08/01/2018 Created [Fabian Sauter]
        /// </history>
        public MUCChatInfoTable()
        {

        }

        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--
        public Presence getMUCPresence()
        {
            switch (enterState)
            {
                case MUCEnterState.ENTERING:
                    return Presence.Chat;
                case MUCEnterState.ENTERD:
                    return Presence.Online;
                case MUCEnterState.ERROR:
                    return Presence.Xa;
                case MUCEnterState.DISCONNECTED:
                default:
                    return Presence.Unavailable;
            }
        }

        #endregion
        //--------------------------------------------------------Misc Methods:---------------------------------------------------------------\\
        #region --Misc Methods (Public)--
        public ConferenceItem toConferenceItem(ChatTable muc)
        {
            return new ConferenceItem()
            {
                autoJoin = autoEnterRoom,
                jid = muc.chatJabberId,
                minimize = false,
                name = name,
                nick = nickname,
                password = password
            };
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
