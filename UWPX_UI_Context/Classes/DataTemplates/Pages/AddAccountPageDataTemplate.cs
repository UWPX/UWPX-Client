using Shared.Classes;
using XMPP_API.Classes;

namespace UWPX_UI_Context.Classes.DataTemplates.Pages
{
    public sealed class AddAccountPageDataTemplate : AbstractDataTemplate
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        private string _JidText;
        public string JidText
        {
            get { return _JidText; }
            set { SetJidText(value); }
        }
        private bool _IsValidJid;
        public bool IsValidJid
        {
            get { return _IsValidJid; }
            set { SetProperty(ref _IsValidJid, value); }
        }

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--


        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--
        private void SetJidText(string value)
        {
            if (SetProperty(ref _JidText, value, nameof(JidText)))
            {
                IsValidJid = Utils.isBareJid(value);
            }
        }

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
