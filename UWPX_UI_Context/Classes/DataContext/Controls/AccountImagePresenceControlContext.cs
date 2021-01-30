using Storage.Classes.Models.Chat;
using UWPX_UI_Context.Classes.DataTemplates.Controls;

namespace UWPX_UI_Context.Classes.DataContext.Controls
{
    public sealed class AccountImagePresenceControlContext
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        public readonly AccountImagePresenceControlDataTemplate MODEL = new AccountImagePresenceControlDataTemplate();

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--


        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--


        #endregion
        //--------------------------------------------------------Misc Methods:---------------------------------------------------------------\\
        #region --Misc Methods (Public)--
        public void UpdateView(ChatType chatType, string bareJid)
        {
            MODEL.UpdateView(chatType, bareJid);
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
