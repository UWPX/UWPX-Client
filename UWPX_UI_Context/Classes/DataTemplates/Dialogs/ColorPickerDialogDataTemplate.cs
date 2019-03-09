using Shared.Classes;
using Windows.UI;

namespace UWPX_UI_Context.Classes.DataTemplates.Dialogs
{
    public sealed class ColorPickerDialogDataTemplate : AbstractDataTemplate
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        private Color _SelectedColor;
        public Color SelectedColor
        {
            get { return _SelectedColor; }
            set { SetProperty(ref _SelectedColor, value); }
        }
        private bool _Confirmed;
        public bool Confirmed
        {
            get { return _Confirmed; }
            set { SetProperty(ref _Confirmed, value); }
        }

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        public ColorPickerDialogDataTemplate(Color color)
        {
            this.SelectedColor = color;
            this.Confirmed = false;
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
