using System;
using Shared.Classes;
using UWPX_UI.Controls;
using UWPX_UI.Controls.IoT;
using UWPX_UI_Context.Classes;
using UWPX_UI_Context.Classes.DataContext.Pages;
using UWPX_UI_Context.Classes.Events;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using XMPP_API.Classes.XmppUri;
using XMPP_API_IoT.Classes.Bluetooth.Events;

namespace UWPX_UI.Pages
{
    public sealed partial class RegisterIoTDevicePage: Page
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        public readonly RegisterIoTDevicePageContext VIEW_MODEL = new RegisterIoTDevicePageContext();

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        public RegisterIoTDevicePage()
        {
            InitializeComponent();
            UpdateViewState(State_1.Name);
        }

        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--


        #endregion
        //--------------------------------------------------------Misc Methods:---------------------------------------------------------------\\
        #region --Misc Methods (Public)--


        #endregion

        #region --Misc Methods (Private)--
        private void UpdateViewState(string state)
        {
            VisualStateManager.GoToState(this, state, true);
        }

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

        private async void WhatIsAnIoTDevice_link_Click(object sender, RoutedEventArgs e)
        {
            await UiUtils.LaunchUriAsync(new Uri(Localisation.GetLocalizedString("RegisterIoTDevicePage_what_is_an_iot_device_url")));
        }

        private void Cancel_ibtn_Click(IconButtonControl sender, RoutedEventArgs args)
        {
            titleBar.OnGoBackRequested();
        }

        private async void QrCodeScannerControl_NewValidQrCode(QrCodeScannerControl sender, NewQrCodeEventArgs args)
        {
            IUriAction action = UriUtils.parse(new Uri(args.QR_CODE));
            if (action is RegisterIoTUriAction registerIoTUriAction)
            {
                await SharedUtils.CallDispatcherAsync(async () =>
                {
                    await qrCodeScanner.StopAsync();

                    UpdateViewState(State_2.Name);
                    VIEW_MODEL.MODEL.RegisterIoTUriAction = registerIoTUriAction;
                });
            }
        }

        private async void Retry_ibtn_Click(IconButtonControl sender, RoutedEventArgs args)
        {
            VIEW_MODEL.MODEL.RegisterIoTUriAction = null;
            UpdateViewState(State_1.Name);

            await qrCodeScanner.StartAsync();
        }

        private void BtScanner_btsc_DeviceChanged(BluetoothScannerControl sender, BLEDeviceEventArgs args)
        {
            if (!(args.DEVICE is null))
            {
                UpdateViewState(State_3.Name);
            }
        }

        #endregion
    }
}
