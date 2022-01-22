using Shared.Classes.Image;
using UWPX_UI_Context.Classes.DataTemplates.Controls.Chat;
using Windows.Graphics.Imaging;

namespace UWPX_UI_Context.Classes.DataContext.Controls.Chat
{
    public class EditImageControlContext
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        public readonly EditImageControlDataTemplate MODEL = new EditImageControlDataTemplate();

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--


        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--


        #endregion
        //--------------------------------------------------------Misc Methods:---------------------------------------------------------------\\
        #region --Misc Methods (Public)--
        public void SetImage(SoftwareBitmap img)
        {
            MODEL.OriginalImage = img;
            MODEL.WritableImage = ImageUtils.ToWritableBitmap(img);
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
