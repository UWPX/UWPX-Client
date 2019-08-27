using System;
using Logging;
using UWPX_UI.Controls;
using UWPX_UI_Context.Classes;
using UWPX_UI_Context.Classes.DataContext.Pages;
using UWPX_UI_Context.Classes.Events;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace UWPX_UI.Pages
{
    public sealed partial class RegisterIoTDevicePage: Page
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        public readonly AddIoTDevicePageContext VIEW_MODEL = new AddIoTDevicePageContext();

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        public RegisterIoTDevicePage()
        {
            InitializeComponent();
        }

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
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            titleBar.OnPageNavigatedTo();
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            titleBar.OnPageNavigatedFrom();
        }

        private async void WhatIsAnIoTDevice_link_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            await UiUtils.LaunchUriAsync(new Uri(Localisation.GetLocalizedString("RegisterIoTDevicePage_what_is_an_iot_device_url")));
        }

        private void Cancel_ibtn_Click(Controls.IconButtonControl sender, Windows.UI.Xaml.RoutedEventArgs args)
        {
            titleBar.OnGoBackRequested();
        }

        private void QrCodeScannerControl_NewValidQrCode(QrCodeScannerControl sender, NewQrCodeEventArgs args)
        {
            Logger.Info("Scanned QR Code: " + args.QR_CODE);
        }

        #endregion
    }
}
