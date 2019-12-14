using Shared.Classes;
using Windows.UI.Xaml;

namespace UWPX_UI_Context.Classes.DataTemplates.Controls.Chat
{
    public class MucMemberControlDataTemplate: AbstractDataTemplate
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        private string _BareJid;
        public string BareJid
        {
            get => _BareJid;
            set => SetProperty(ref _BareJid, value);
        }

        private string _Nickname;
        public string Nickname
        {
            get => _Nickname;
            set => SetProperty(ref _Nickname, value);
        }

        private string _ImageBareJid;
        public string ImageBareJid
        {
            get => _ImageBareJid;
            set => SetProperty(ref _ImageBareJid, value);
        }

        private Visibility _YouVisible;
        public Visibility YouVisible
        {
            get => _YouVisible;
            set => SetProperty(ref _YouVisible, value);
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
