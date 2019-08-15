using System;
using System.Collections.Generic;

namespace UWP_XMPP_Client.Classes.Events
{
    public class SelectionChangedMultiEventArgs : EventArgs
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        public readonly List<object> SELECTED_ITEMS;

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        /// <summary>
        /// Basic Constructor
        /// </summary>
        /// <history>
        /// 02/03/2018 Created [Fabian Sauter]
        /// </history>
        public SelectionChangedMultiEventArgs(List<object> selectedItems)
        {
            SELECTED_ITEMS = selectedItems;
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
