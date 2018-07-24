using SQLite;
using XMPP_API.Classes;
using XMPP_API.Classes.Network.XML.Messages.XEP_0048;

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
        // The room subject
        public string subject { get; set; }
        // The password for the MUC
        public string password { get; set; }
        // The current state of the MUC e.g. 'ENTERING'
        public MUCState state { get; set; }
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
            switch (state)
            {
                case MUCState.ENTERING:
                case MUCState.DISCONNECTING:
                    return Presence.Chat;

                case MUCState.ENTERD:
                    return Presence.Online;

                case MUCState.ERROR:
                case MUCState.KICKED:
                case MUCState.BANED:
                    return Presence.Xa;

                case MUCState.DISCONNECTED:
                default:
                    return Presence.Unavailable;
            }
        }

        #endregion
        //--------------------------------------------------------Misc Methods:---------------------------------------------------------------\\
        #region --Misc Methods (Public)--
        public ConferenceItem toConferenceItem()
        {
            return new ConferenceItem()
            {
                autoJoin = autoEnterRoom,
                name = name,
                nick = nickname,
                password = password,
                jid = chatId
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
