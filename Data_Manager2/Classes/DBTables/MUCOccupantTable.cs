using SQLite;
using XMPP_API.Classes.Network.XML.Messages.XEP_0045;

namespace Data_Manager2.Classes.DBTables
{
    [Table(DBTableConsts.MUC_OCCUPANT_TABLE)]
    public class MUCOccupantTable
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        [PrimaryKey]
        // Generated in generateId()
        public string id { get; set; }
        [NotNull]
        // The id entry of the ChatTable
        public string chatId { get; set; }
        [NotNull]
        // The user nickname e.g. 'thirdwitch'
        public string nickname { get; set; }
        // The full jabber id of the user e.g. 'coven@chat.shakespeare.lit/thirdwitch'
        public string jid { get; set; }
        // The affiliation of the user e.g. 'owner', 'admin', 'member' or 'none'
        public MUCAffiliation affiliation { get; set; }
        // The role of the user e.g. 'moderator', 'participant' or 'visitor'
        public MUCRole role { get; set; }

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        /// <summary>
        /// Basic Constructor
        /// </summary>
        /// <history>
        /// 08/01/2018 Created [Fabian Sauter]
        /// </history>
        public MUCOccupantTable()
        {

        }

        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--


        #endregion
        //--------------------------------------------------------Misc Methods:---------------------------------------------------------------\\
        #region --Misc Methods (Public)--
        public static string generateId(string chatId, string nickname)
        {
            return chatId + '_' + nickname;
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
