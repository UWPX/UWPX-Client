using SQLite.Net.Attributes;

namespace Data_Manager2.Classes.DBTables
{
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


        #endregion
        //--------------------------------------------------------Misc Methods:---------------------------------------------------------------\\
        #region --Misc Methods (Public)--


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
