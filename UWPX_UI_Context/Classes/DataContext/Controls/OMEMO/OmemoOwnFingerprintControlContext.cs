using Manager.Classes;
using UWPX_UI_Context.Classes.DataTemplates.Controls.OMEMO;
using Windows.UI.Xaml;

namespace UWPX_UI_Context.Classes.DataContext.Controls.OMEMO
{
    public class OmemoOwnFingerprintControlContext
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        public readonly OmemoOwnFingerprintControlDataTemplate MODEL = new OmemoOwnFingerprintControlDataTemplate();

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--


        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--


        #endregion
        //--------------------------------------------------------Misc Methods:---------------------------------------------------------------\\
        #region --Misc Methods (Public)--
        public void UpdateView(DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue is Client client)
            {
                MODEL.UpdateView(client);
            }
            else
            {
                MODEL.UpdateView(null);
            }
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
