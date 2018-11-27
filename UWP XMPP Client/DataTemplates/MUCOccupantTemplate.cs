using Data_Manager2.Classes.DBTables;
using XMPP_API.Classes.Network.XML.Messages.XEP_0045;

namespace UWP_XMPP_Client.DataTemplates
{
    public class MUCOccupantTemplate
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        public MUCOccupantTable occupant;

        public string nickname
        {
            get => occupant?.nickname;
            set
            {
                if (occupant != null)
                {
                    occupant.nickname = value;
                }
            }
        }

        public string jid
        {
            get => occupant?.jid;
            set
            {
                if (occupant != null)
                {
                    occupant.jid = value;
                }
            }
        }

        public MUCAffiliation affiliation
        {
            get => occupant is null ? MUCAffiliation.NONE : occupant.affiliation;
            set
            {
                if (occupant != null)
                {
                    occupant.affiliation = value;
                }
            }
        }

        public MUCRole role
        {
            get => occupant is null ? MUCRole.VISITOR : occupant.role;
            set
            {
                if (occupant != null)
                {
                    occupant.role = value;
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
        public MUCOccupantTemplate()
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
