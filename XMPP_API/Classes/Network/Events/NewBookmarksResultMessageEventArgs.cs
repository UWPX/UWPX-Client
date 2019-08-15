using System;
using XMPP_API.Classes.Network.XML.Messages.XEP_0048;

namespace XMPP_API.Classes.Network.Events
{
    public class NewBookmarksResultMessageEventArgs : EventArgs
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        public readonly BookmarksResultMessage BOOKMARKS_MESSAGE;

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        /// <summary>
        /// Basic Constructor
        /// </summary>
        /// <history>
        /// 02/01/2018 Created [Fabian Sauter]
        /// </history>
        public NewBookmarksResultMessageEventArgs(BookmarksResultMessage bookmarks_message)
        {
            BOOKMARKS_MESSAGE = bookmarks_message;
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
