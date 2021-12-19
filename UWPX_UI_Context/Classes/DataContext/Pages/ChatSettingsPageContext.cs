using System;
using UWPX_UI_Context.Classes.DataTemplates.Pages;
using Windows.Foundation;

namespace UWPX_UI_Context.Classes.DataContext.Pages
{
    public class ChatSettingsPageContext
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        public readonly ChatSettingsPageDataTemplate MODEL = new ChatSettingsPageDataTemplate();

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--


        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--


        #endregion
        //--------------------------------------------------------Misc Methods:---------------------------------------------------------------\\
        #region --Misc Methods (Public)--
        public IAsyncOperation<bool> OnWhatIsOmemoClickedAsync()
        {
            return UiUtils.LaunchUriAsync(new Uri("https://conversations.im/omemo/"));
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
