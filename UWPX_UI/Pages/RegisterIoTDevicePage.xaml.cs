using System;
using System.Collections.ObjectModel;
using Logging;
using Manager.Classes;
using Microsoft.Toolkit.Uwp.UI.Animations;
using Shared.Classes;
using UWPX_UI.Controls;
using UWPX_UI.Controls.IoT;
using UWPX_UI.Controls.Settings;
using UWPX_UI_Context.Classes;
using UWPX_UI_Context.Classes.DataContext.Pages;
using UWPX_UI_Context.Classes.DataTemplates;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using XMPP_API.Classes;
using XMPP_API.Classes.Network.Events;
using XMPP_API.Classes.XmppUri;
using XMPP_API_IoT.Classes.Bluetooth.Events;

namespace UWPX_UI.Pages
{
    public sealed partial class RegisterIoTDevicePage: Page
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        public readonly RegisterIoTDevicePageContext VIEW_MODEL = new RegisterIoTDevicePageContext();

        private readonly ObservableCollection<SettingsPageButtonDataTemplate> DEVICE_TYPES = new ObservableCollection<SettingsPageButtonDataTemplate>
        {
            new SettingsPageButtonDataTemplate {Glyph = "\uE957", Name = "Standalone", Description = "Standalone devices", NavTarget = null},
            new SettingsPageButtonDataTemplate {Glyph = "\uF22C", Name = "Hub Based", Description = "Devices, that connect to a device hub", NavTarget = null},
        };

        private FrameworkElement LastPopUpElement = null;
        private string curViewState;

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        public RegisterIoTDevicePage()
        {
            InitializeComponent();
            UpdateViewState(State_0.Name);
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
            if (VisualStateManager.GoToState(this, state, true))
            {
                Logger.Debug(nameof(RegisterIoTDevicePage) + " state changed: " + curViewState + " -> " + state);
                if (string.Equals(curViewState, State_5.Name))
                {
                    btDeviceInfo_btdic.VIEW_MODEL.MODEL.Client.xmppClient.NewChatMessage -= Client_NewChatMessage;
                }
                curViewState = state;
                if (string.Equals(curViewState, State_5.Name) || string.Equals(curViewState, State_4.Name))
                {
                    btDeviceInfo_btdic.VIEW_MODEL.MODEL.Client.xmppClient.NewChatMessage -= Client_NewChatMessage;
                    btDeviceInfo_btdic.VIEW_MODEL.MODEL.Client.xmppClient.NewChatMessage += Client_NewChatMessage;
                }
            }
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

        private async void Cancel_ibtn_Click(IconButtonControl sender, RoutedEventArgs args)
        {
            await qrCodeScanner.StopAsync();
            titleBar.OnGoBackRequested();
        }

        private async void Retry_ibtn_Click(IconButtonControl sender, RoutedEventArgs args)
        {
            if (string.Equals(curViewState, State_1.Name))
            {
                await qrCodeScanner.StopAsync();
            }

            UpdateViewState(State_0.Name);
        }

        private void BtScanner_btsc_DeviceChanged(BluetoothScannerControl sender, BLEDeviceEventArgs args)
        {
            if (!(args.DEVICE is null))
            {
                UpdateViewState(State_3.Name);
            }
        }

        private async void Send3_ibtn_Click(IconButtonControl sender, RoutedEventArgs args)
        {
            UpdateViewState(State_4.Name);
            await VIEW_MODEL.SendAsync(btDeviceInfo_btdic.VIEW_MODEL.MODEL);
            UpdateViewState(State_5.Name);
        }

        private void SettingsSelectionControl_PointerEntered(object sender, Windows.UI.Xaml.Input.PointerRoutedEventArgs e)
        {
            if (!(DeviceFamilyHelper.IsMouseInteractionMode() && sender is FrameworkElement deviceModeSelection))
            {
                return;
            }

            LastPopUpElement = VisualTreeHelper.GetParent(VisualTreeHelper.GetParent(deviceModeSelection) as FrameworkElement) as FrameworkElement;
            Canvas.SetZIndex(LastPopUpElement, 10);
            LastPopUpElement.Scale(scaleX: 1.05f, scaleY: 1.05f, easingType: EasingType.Sine).Start();
        }

        private void SettingsSelectionControl_PointerExited(object sender, Windows.UI.Xaml.Input.PointerRoutedEventArgs e)
        {
            if (LastPopUpElement is null)
            {
                return;
            }

            Canvas.SetZIndex(LastPopUpElement, 0);
            LastPopUpElement.Scale(easingType: EasingType.Sine).Start();
            LastPopUpElement = null;
        }

        private async void SettingsSelectionLargeControl_Click(SettingsSelectionLargeControl sender, RoutedEventArgs args)
        {
            UpdateViewState(State_1.Name);
            VIEW_MODEL.MODEL.RegisterIoTUriAction = null;
            await qrCodeScanner.StartAsync();

        }

        private async void SettingsSelectionSmallControl_Click(SettingsSelectionSmallControl sender, RoutedEventArgs args)
        {
            UpdateViewState(State_1.Name);
            VIEW_MODEL.MODEL.RegisterIoTUriAction = null;
            await qrCodeScanner.StartAsync();
        }

        private async void QrCodeScanner_NewValidQrCode(QrCodeScannerControl sender, UWPX_UI_Context.Classes.Events.NewQrCodeEventArgs args)
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

        private void Done5_ibtn_Click(object sender, RoutedEventArgs args)
        {
            UiUtils.NavigateToPage(typeof(ChatPage));
            // Prevent navigating back to this page:
            UiUtils.RemoveLastBackStackEntry();
        }

        private async void Client_NewChatMessage(XMPPClient client, NewChatMessageEventArgs args)
        {
            if (await VIEW_MODEL.OnNewChatMessage(args.getMessage(), btDeviceInfo_btdic.VIEW_MODEL.MODEL.Jid, ConnectionHandler.INSTANCE.GetClient(client.getXMPPAccount().getBareJid())))
            {
                await SharedUtils.CallDispatcherAsync(() => UpdateViewState(State_6.Name));
            }
        }

        #endregion
    }
}
