using Shared.Classes;

namespace UWPX_UI_Context.Classes.DataTemplates.Controls.Chat.MUC
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
        private string _BookmarkText;
        public string BookmarkText
        {
            get => _BookmarkText;
            set => SetProperty(ref _BookmarkText, value);
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
