using Data_Manager2.Classes.DBTables;
using System;

namespace Data_Manager2.Classes.Events
{
    public class MUCMemberChangedEventArgs : EventArgs
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        public readonly MUCMemberTable MUC_MEMBER;
        public readonly bool REMOVED;

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        /// <summary>
        /// Basic Constructor
        /// </summary>
        /// <history>
        /// 06/03/2018 Created [Fabian Sauter]
        /// </history>
        public MUCMemberChangedEventArgs(MUCMemberTable member, bool removed)
        {
            this.MUC_MEMBER = member;
            this.REMOVED = removed;
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
