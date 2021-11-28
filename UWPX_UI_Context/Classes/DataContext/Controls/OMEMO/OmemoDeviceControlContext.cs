using System.ComponentModel;
using Storage.Classes.Models.Omemo;
using UWPX_UI_Context.Classes.DataTemplates.Controls.OMEMO;
using Windows.UI.Xaml;

namespace UWPX_UI_Context.Classes.DataContext.Controls.OMEMO
{
    public sealed class OmemoDeviceControlContext
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        public readonly OmemoDeviceControlDataTemplate MODEL = new OmemoDeviceControlDataTemplate();

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
            if (e.OldValue is OmemoDeviceModel oldDevice)
            {
                oldDevice.PropertyChanged -= NewDevice_PropertyChanged;
            }

            if (e.NewValue is OmemoDeviceModel newDevice)
            {
                newDevice.PropertyChanged += NewDevice_PropertyChanged;
                UpdateView(newDevice);
            }
        }

        public void UpdateView(OmemoDeviceModel device)
        {
            if (device is not null)
            {
                MODEL.Label = string.IsNullOrEmpty(device.deviceLabel) ? (device.deviceId == 0 ? "-" : device.deviceId.ToString()) : device.deviceLabel;
            }
        }

        #endregion

        #region --Misc Methods (Private)--


        #endregion

        #region --Misc Methods (Protected)--


        #endregion
        //--------------------------------------------------------Events:---------------------------------------------------------------------\\
        #region --Events--
        private void NewDevice_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (sender is OmemoDeviceModel device)
            {
                UpdateView(device);
            }
        }

        #endregion
    }
}
