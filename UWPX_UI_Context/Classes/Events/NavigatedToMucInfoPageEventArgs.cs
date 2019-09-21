using Data_Manager2.Classes.DBTables;
using XMPP_API.Classes;

namespace UWPX_UI_Context.Classes.Events
{
    public class NavigatedToMucInfoPageEventArgs
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        public readonly ChatTable CHAT;
        public readonly XMPPClient CLIENT;
        public readonly MUCChatInfoTable MUC_INFO;

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        public NavigatedToMucInfoPageEventArgs(XMPPClient client, ChatTable chat, MUCChatInfoTable mucInfo)
        {
            CHAT = chat;
            CLIENT = client;
            MUC_INFO = mucInfo;
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
