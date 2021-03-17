using Shared.Classes;

namespace UWPX_UI_Context.Classes.DataTemplates.Controls
{
    public sealed class OmemoDeviceListControlDataTemplate: AbstractDataTemplate
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        private bool _RefreshingDevices;
        public bool RefreshingDevices
        {
            get => _RefreshingDevices;
            set => SetProperty(ref _RefreshingDevices, value);
        }
        private bool _ResettingDevices;
        public bool ResettingDevices
        {
            get => _ResettingDevices;
            set => SetProperty(ref _ResettingDevices, value);
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
