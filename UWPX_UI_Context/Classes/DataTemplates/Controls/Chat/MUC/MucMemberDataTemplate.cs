using System;
using Manager.Classes.Chat;
using Shared.Classes;
using Storage.Classes.Models.Chat;

namespace UWPX_UI_Context.Classes.DataTemplates.Controls.Chat.MUC
{
    public class MucMemberDataTemplate: AbstractDataTemplate, IComparable
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        private MucOccupantModel _Member;
        public MucOccupantModel Member
        {
            get => _Member;
            set => SetProperty(ref _Member, value);
        }

        private ChatDataTemplate _Chat;
        public ChatDataTemplate Chat
        {
            get => _Chat;
            set => SetProperty(ref _Chat, value);
        }

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--


        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--


        #endregion
        //--------------------------------------------------------Misc Methods:---------------------------------------------------------------\\
        #region --Misc Methods (Public)--
        public int CompareTo(object obj)
        {
            return obj is MucMemberDataTemplate other ? Member.nickname.CompareTo(other.Member.nickname) : -1;
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
