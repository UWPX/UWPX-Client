﻿using System.Threading.Tasks;
using UWPX_UI_Context.Classes.DataTemplates.Controls;
using Windows.UI.Xaml;

namespace UWPX_UI_Context.Classes.DataContext.Controls
{
    public sealed class AccountInfoCertificateGeneralControlContext
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        public readonly AccountInfoCertificateGeneralControlDataTemplate MODEL = new AccountInfoCertificateGeneralControlDataTemplate();

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--


        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--


        #endregion
        //--------------------------------------------------------Misc Methods:---------------------------------------------------------------\\
        #region --Misc Methods (Public)--
        public void UpdateViewModel(DependencyPropertyChangedEventArgs e)
        {
            MODEL.UpdateViewModel(e);
        }

        public Task ExportCertificateAsync()
        {
            return MODEL.ExportCertificateAsync();
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
