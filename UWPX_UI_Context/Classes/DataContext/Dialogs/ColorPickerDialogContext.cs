using UWPX_UI_Context.Classes.DataTemplates.Dialogs;
using Windows.UI;

namespace UWPX_UI_Context.Classes.DataContext.Dialogs
{
    public sealed class ColorPickerDialogContext
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        public readonly ColorPickerDialogDataTemplate MODEL;

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        public ColorPickerDialogContext(Color color)
        {
            MODEL = new ColorPickerDialogDataTemplate(color);
        }

        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--


        #endregion
        //--------------------------------------------------------Misc Methods:---------------------------------------------------------------\\
        #region --Misc Methods (Public)--
        public void Confirm()
        {
            MODEL.Confirmed = true;
        }

        public void Cancel()
        {
            MODEL.Confirmed = false;
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
