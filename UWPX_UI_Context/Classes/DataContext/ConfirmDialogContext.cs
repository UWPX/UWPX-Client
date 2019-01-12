using System;
using System.Threading.Tasks;
using UWPX_UI_Context.Classes.DataTemplates;

namespace UWPX_UI_Context.Classes.DataContext
{
    public sealed class ConfirmDialogContext
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        public readonly ConfirmDialogDataTemplate MODEL = new ConfirmDialogDataTemplate();

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--


        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--


        #endregion
        //--------------------------------------------------------Misc Methods:---------------------------------------------------------------\\
        #region --Misc Methods (Public)--
        public void OnPositive()
        {
            MODEL.Confirmed = true;
        }

        public void OnNegative()
        {
            MODEL.Confirmed = false;
        }

        public async Task OnLinkClickedAsync(string link)
        {
            await UiUtils.LaunchUriAsync(new Uri(link));
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
