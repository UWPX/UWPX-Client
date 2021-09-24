using Shared.Classes;
using Shared.Classes.Collections;
using Storage.Classes.Models.Omemo;

namespace UWPX_UI_Context.Classes.DataTemplates.Controls
{
    public sealed class OmemoDeviceListControlDataTemplate: AbstractDataTemplate
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        private bool _ResettingDevices;
        public bool ResettingDevices
        {
            get => _ResettingDevices;
            set => SetProperty(ref _ResettingDevices, value);
        }

        private bool _RefreshingDevices;
        public bool RefreshingDevices
        {
            get => _RefreshingDevices;
            set => SetProperty(ref _RefreshingDevices, value);
        }

        private bool _Loading;
        public bool Loading
        {
            get => _Loading;
            set => SetProperty(ref _Loading, value);
        }

        public readonly CustomObservableCollection<OmemoDeviceModel> DEVICES = new CustomObservableCollection<OmemoDeviceModel>(true);

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
