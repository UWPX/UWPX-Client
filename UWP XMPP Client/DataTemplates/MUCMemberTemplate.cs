using Data_Manager2.Classes.DBTables;
using XMPP_API.Classes.Network.XML.Messages.XEP_0045;

namespace UWP_XMPP_Client.DataTemplates
{
    class MUCMemberTemplate
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        public MUCMemberTable member;

        public string nickname
        {
            get => member?.nickname;
            set
            {
                if (member != null)
                {
                    member.nickname = value;
                }
            }
        }

        public string jid
        {
            get => member?.jid;
            set
            {
                if (member != null)
                {
                    member.jid = value;
                }
            }
        }

        public MUCAffiliation affiliation
        {
            get => member == null ? MUCAffiliation.NONE : member.affiliation;
            set
            {
                if (member != null)
                {
                    member.affiliation = value;
                }
            }
        }

        public MUCRole role
        {
            get => member == null ? MUCRole.VISITOR : member.role;
            set
            {
                if (member != null)
                {
                    member.role = value;
                }
            }
        }

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        /// <summary>
        /// Basic Constructor
        /// </summary>
        /// <history>
        /// 06/02/2018 Created [Fabian Sauter]
        /// </history>
        public MUCMemberTemplate()
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
