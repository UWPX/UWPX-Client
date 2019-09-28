using Data_Manager2.Classes;
using Shared.Classes;

namespace UWPX_UI_Context.Classes.DataTemplates.Controls.Chat
{
    public class MucInfoControlDataTemplate: AbstractDataTemplate
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        private string _MucName;
        public string MucName
        {
            get => _MucName;
            set => SetProperty(ref _MucName, value);
        }

        private string _MucSubject;
        public string MucSubject
        {
            get => _MucSubject;
            set => SetProperty(ref _MucSubject, value);
        }

        private string _ChatBareJid;
        public string ChatBareJid
        {
            get => _ChatBareJid;
            set => SetProperty(ref _ChatBareJid, value);
        }

        private string _BookmarkText;
        public string BookmarkText
        {
            get => _BookmarkText;
            set => SetProperty(ref _BookmarkText, value);
        }

        private MUCState _MucState;
        public MUCState MucState
        {
            get => _MucState;
            set => SetProperty(ref _MucState, value);
        }

        private bool _DifferentMucName;
        public bool DifferentMucName
        {
            get => _DifferentMucName;
            set => SetProperty(ref _DifferentMucName, value);
        }

        private string _MuteGlyph;
        public string MuteGlyph
        {
            get => _MuteGlyph;
            set => SetProperty(ref _MuteGlyph, value);
        }

        private string _MuteTooltip;
        public string MuteTooltip
        {
            get => _MuteTooltip;
            set => SetProperty(ref _MuteTooltip, value);
        }

        private bool _EnterMucAvailable;
        public bool EnterMucAvailable
        {
            get => _EnterMucAvailable;
            set => SetProperty(ref _EnterMucAvailable, value);
        }

        private bool _AutoJoin;
        public bool AutoJoin
        {
            get => _AutoJoin;
            set => SetProperty(ref _AutoJoin, value);
        }

        private string _Nickname;
        public string Nickname
        {
            get => _Nickname;
            set => SetProperty(ref _Nickname, value);
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
