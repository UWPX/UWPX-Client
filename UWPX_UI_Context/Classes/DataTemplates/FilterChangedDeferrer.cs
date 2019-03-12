using System;

namespace UWPX_UI_Context.Classes.DataTemplates
{
    public class FilterChangedDeferrer : IDisposable
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        private readonly ChatFilterDataTemplate CHAT_FILTER;

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        public FilterChangedDeferrer(ChatFilterDataTemplate chatFilter)
        {
            this.CHAT_FILTER = chatFilter;
            this.CHAT_FILTER.DeferRefreshUp();
        }

        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--


        #endregion
        //--------------------------------------------------------Misc Methods:---------------------------------------------------------------\\
        #region --Misc Methods (Public)--
        public void Dispose()
        {
            if (CHAT_FILTER is null)
            {
                return;
            }

            CHAT_FILTER.DeferRefreshDown();
            CHAT_FILTER.RefreshFilter();
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
