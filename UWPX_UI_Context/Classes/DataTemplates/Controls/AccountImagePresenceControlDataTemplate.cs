using Data_Manager2.Classes;
using Shared.Classes;

namespace UWPX_UI_Context.Classes.DataTemplates.Controls
{
    public sealed class AccountImagePresenceControlDataTemplate : AbstractDataTemplate
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        private string _Initials;
        public string Initials
        {
            get { return _Initials; }
            set { SetProperty(ref _Initials, value); }
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
        public void UpdateView(ChatType chatType, string bareJid)
        {
            switch (chatType)
            {
                case ChatType.CHAT:
                    Initials = string.IsNullOrEmpty(bareJid) ? "" : bareJid[0].ToString().ToUpperInvariant();
                    break;

                case ChatType.MUC:
                    Initials = "\uE902";
                    break;
            }
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
