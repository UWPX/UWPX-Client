using XMPP_API.Classes.Network.XML.Messages.XEP_0045;

namespace XMPP_API.Classes.Network.Events
{
    public class NewMUCMemberPresenceMessageEventArgs
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        public readonly MUCMemberPresenceMessage mucMemberPresenceMessage;

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        /// <summary>
        /// Basic Constructor
        /// </summary>
        /// <history>
        /// 06/02/2018 Created [Fabian Sauter]
        /// </history>
        public NewMUCMemberPresenceMessageEventArgs(MUCMemberPresenceMessage mucMemberPresenceMessage)
        {
            this.mucMemberPresenceMessage = mucMemberPresenceMessage;
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
