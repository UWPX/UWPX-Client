using System.ComponentModel;
using Data_Manager2.Classes.DBTables;

namespace Data_Manager2.Classes.Events
{
    public class MUCInfoChangedEventArgs: CancelEventArgs
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        public readonly MUCChatInfoTable MUC_INFO;
        public readonly bool REMOVED;

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        /// <summary>
        /// Basic Constructor
        /// </summary>
        /// <history>
        /// 25/01/2018 Created [Fabian Sauter]
        /// </history>
        public MUCInfoChangedEventArgs(MUCChatInfoTable muc, bool removed)
        {
            MUC_INFO = muc;
            REMOVED = removed;
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
