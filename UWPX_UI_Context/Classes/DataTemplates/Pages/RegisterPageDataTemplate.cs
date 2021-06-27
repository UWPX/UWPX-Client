using Shared.Classes;
using Shared.Classes.Collections;
using XMPP_API.Classes;

namespace UWPX_UI_Context.Classes.DataTemplates.Pages
{
    public sealed class RegisterPageDataTemplate: AbstractDataTemplate
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        public readonly CustomObservableCollection<Provider> PROVIDER = new CustomObservableCollection<Provider>(true);

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        public RegisterPageDataTemplate()
        {
            LoadProvider();
        }

        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--


        #endregion
        //--------------------------------------------------------Misc Methods:---------------------------------------------------------------\\
        #region --Misc Methods (Public)--
        public void LoadProvider()
        {
            PROVIDER.Clear();
            PROVIDER.AddRange(XMPPProviders.INSTANCE.providersB);
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
