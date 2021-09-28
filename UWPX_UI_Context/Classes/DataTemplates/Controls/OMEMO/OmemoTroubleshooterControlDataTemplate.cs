using Shared.Classes;

namespace UWPX_UI_Context.Classes.DataTemplates.Controls.OMEMO
{
    public class OmemoTroubleshooterControlDataTemplate: AbstractDataTemplate
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        private bool _Troubleshooting;
        public bool Troubleshooting
        {
            get => _Troubleshooting;
            set => SetProperty(ref _Troubleshooting, value);
        }

        private bool _Fixing;
        public bool Fixing
        {
            get => _Fixing;
            set => SetProperty(ref _Fixing, value);
        }

        private bool _Working;
        public bool Working
        {
            get => _Working;
            set => SetProperty(ref _Working, value);
        }

        private string _StatusText;
        public string StatusText
        {
            get => _StatusText;
            set => SetProperty(ref _StatusText, value);
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
