using Shared.Classes;
using Windows.UI.Xaml;
using XMPP_API.Classes.Network.XML.Messages.XEP_0045;

namespace UWPX_UI_Context.Classes.DataTemplates.Controls.Chat.MUC
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

        private MUCRole _Role;
        public MUCRole Role
        {
            get => _Role;
            set => SetProperty(ref _Role, value);
        }

        private MUCAffiliation _Affiliation;
        public MUCAffiliation Affiliation
        {
            get => _Affiliation;
            set => SetProperty(ref _Affiliation, value);
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
