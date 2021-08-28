using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Threading.Tasks;
using Logging;
using Microsoft.Toolkit.Uwp.Connectivity;
using Windows.Devices.Radios;
using XMPP_API_IoT.Classes.Bluetooth.Events;

namespace XMPP_API_IoT.Classes.Bluetooth
{
    public class BLEScanner: IDisposable
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        private readonly BluetoothLEHelper BLE_HELPER = BluetoothLEHelper.Context;
        private BLEScannerState state = BLEScannerState.DISABLED;
        private string targetMac;

        public delegate void BLEScannerStateChangedEventHandler(BLEScanner scanner, BLEScannerStateChangedEventArgs args);
        public delegate void BLEDeviceFoundEventHandler(BLEScanner scanner, BLEDeviceEventArgs args);

        public event BLEScannerStateChangedEventHandler StateChanged;
        public event BLEDeviceFoundEventHandler DeviceFound;

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        public BLEScanner(string targetMac)
        {
            this.targetMac = targetMac.ToLowerInvariant();
            // Temporary workaround for the wrong MAC on the QR Code
            // ---------------------
            if (this.targetMac.Equals("a4:cf:12:25:76:98"))
            {
                this.targetMac = "a4:cf:12:25:76:9a";
            }
            // ---------------------
        }

        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--
        private void SetState(BLEScannerState state)
        {
            if (this.state != state)
            {
                BLEScannerState oldState = this.state;
                this.state = state;
                StateChanged?.Invoke(this, new BLEScannerStateChangedEventArgs(oldState, state));
            }
        }

        /// <summary>
        /// Returns whether the device has a available Bluetooth radio.
        /// <para/>
        /// Based on: https://stackoverflow.com/questions/33013275/how-to-check-if-bluetooth-is-enabled-on-a-device
        /// </summary>
        public async Task<bool> IsBluetoothSupportedAsync()
        {
            IReadOnlyList<Radio> radios = await Radio.GetRadiosAsync();
            return radios.FirstOrDefault(radio => radio.Kind == RadioKind.Bluetooth) != null;
        }

        /// <summary>
        /// Returns whether Bluetooth is enabled on the device.
        /// <para/>
        /// Based on: https://stackoverflow.com/questions/33013275/how-to-check-if-bluetooth-is-enabled-on-a-device
        /// </summary>
        public static async Task<bool> IsBluetoothEnabledAsync()
        {
            IReadOnlyList<Radio> radios = await Radio.GetRadiosAsync();
            Radio bluetoothRadio = radios.FirstOrDefault(radio => radio.Kind == RadioKind.Bluetooth);
            return bluetoothRadio != null && bluetoothRadio.State == RadioState.On;
        }

        #endregion
        //--------------------------------------------------------Misc Methods:---------------------------------------------------------------\\
        #region --Misc Methods (Public)--
        public async Task StartAsync()
        {
            if (state == BLEScannerState.SCANNING)
            {
                Logger.Warn("Unable to start Bluetooth scanner. Already started!");
                return;
            }

            if (!await IsBluetoothSupportedAsync())
            {
                SetState(BLEScannerState.ERROR_BLUETOOTH_NOT_SUPPORTED);
                Logger.Warn("No Bluetooth radio available.");
                return;
            }

            if (!await IsBluetoothEnabledAsync())
            {
                SetState(BLEScannerState.ERROR_BLUETOOTH_DISABLED);
                Logger.Warn("Bluetooth disabled. Can't use it.");
                return;
            }

            if (!BluetoothLEHelper.Context.IsCentralRoleSupported)
            {
                SetState(BLEScannerState.ERROR_BLE_NOT_SUPPORTED);
                Logger.Warn("No Bluetooth radio supports BLE.");
                return;
            }

            BLE_HELPER.EnumerationCompleted += BLE_HELPER_EnumerationCompleted;
            BLE_HELPER.BluetoothLeDevices.CollectionChanged += BluetoothLeDevices_CollectionChanged;
            BLE_HELPER.StartEnumeration();

            SetState(BLEScannerState.SCANNING);
            Logger.Info("Bluetooth scanner started.");

            await OnDevicesChangedAsync();
        }

        public void Stop()
        {
            if (state == BLEScannerState.DISABLED)
            {
                Logger.Warn("Unable to stop Bluetooth scanner. Already stopped!");
                return;
            }

            if (state == BLEScannerState.SCANNING)
            {
                BLE_HELPER.StopEnumeration();
            }

            BLE_HELPER.EnumerationCompleted -= BLE_HELPER_EnumerationCompleted;
            BLE_HELPER.BluetoothLeDevices.CollectionChanged -= BluetoothLeDevices_CollectionChanged;

            SetState(BLEScannerState.DISABLED);
            Logger.Info("Bluetooth scanner stopped.");
        }

        public void Dispose()
        {
            Stop();
        }

        #endregion

        #region --Misc Methods (Private)--
        private async Task OnDevicesChangedAsync()
        {
            List<ObservableBluetoothLEDevice> devices = new List<ObservableBluetoothLEDevice>(BLE_HELPER.BluetoothLeDevices);
            foreach (ObservableBluetoothLEDevice device in devices)
            {
                if (string.Equals(device.BluetoothAddressAsString.ToLowerInvariant(), targetMac))
                {
                    SetState(BLEScannerState.CONNECTING);
                    BLEDevice bleDevice = await BLEDevice.FromIdAsync(device.DeviceInfo.Id);
                    if (!(bleDevice is null))
                    {
                        // Try to connect to the device:
                        if (await bleDevice.ConnectAsync())
                        {
                            // Invoke event:
                            DeviceFound?.Invoke(this, new BLEDeviceEventArgs(bleDevice));
                            return;
                        }
                    }
                    SetState(BLEScannerState.SCANNING);
                }
            }
        }

        #endregion

        #region --Misc Methods (Protected)--


        #endregion
        //--------------------------------------------------------Events:---------------------------------------------------------------------\\
        #region --Events--
        private void BLE_HELPER_EnumerationCompleted(object sender, EventArgs e)
        {
            // Make sure we do continuous enumeration while scanning is active:
            if (state == BLEScannerState.SCANNING)
            {
                BLE_HELPER.StartEnumeration();
            }
        }

        private async void BluetoothLeDevices_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (state == BLEScannerState.SCANNING)
            {
                await OnDevicesChangedAsync();
            }
        }

        #endregion
    }
}
