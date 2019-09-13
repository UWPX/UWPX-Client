using System;
using System.Threading;
using System.Threading.Tasks;
using Logging;
using Microsoft.Toolkit.Uwp.Connectivity;
using Windows.Devices.Bluetooth;
using XMPP_API_IoT.Classes.Bluetooth.Events;

namespace XMPP_API_IoT.Classes.Bluetooth
{
    public class BLEDevice
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        private BLEDeviceState state = BLEDeviceState.DISCONNECTED;
        public readonly BluetoothLEDevice DEVICE;
        public readonly ObservableBluetoothLEDevice OBSERVALBLE_DEVICE;

        public delegate void BLEScannerStateChangedEventHandler(BLEDevice device, BLEDeviceStateChangedEventArgs args);

        public event BLEScannerStateChangedEventHandler StateChanged;

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        private BLEDevice(BluetoothLEDevice device)
        {
            DEVICE = device;
            DEVICE.ConnectionStatusChanged += DEVICE_ConnectionStatusChanged;
            OBSERVALBLE_DEVICE = new ObservableBluetoothLEDevice(device.DeviceInformation);
        }

        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--
        private void SetState(BLEDeviceState state)
        {
            if (this.state != state)
            {
                BLEDeviceState oldState = this.state;
                this.state = state;
                StateChanged?.Invoke(this, new BLEDeviceStateChangedEventArgs(oldState, state));
            }
        }

        #endregion
        //--------------------------------------------------------Misc Methods:---------------------------------------------------------------\\
        #region --Misc Methods (Public)--
        public static async Task<BLEDevice> FromIdAsync(string deviceId)
        {
            try
            {
                CancellationTokenSource cancellation = new CancellationTokenSource(1000);
                BluetoothLEDevice device = await BluetoothLEDevice.FromIdAsync(deviceId).AsTask(cancellation.Token);
                Logger.Info("BLE device found.");
                return new BLEDevice(device);
            }
            catch (TaskCanceledException)
            {
                Logger.Error("BLE device connection failed. Timeout!");
                return null;
            }
            catch (Exception e)
            {
                Logger.Error("BLE device connection failed.", e);
                return null;
            }
        }

        public async Task<bool> ConnectAsync()
        {
            if (!OBSERVALBLE_DEVICE.IsConnected)
            {
                try
                {
                    await OBSERVALBLE_DEVICE.ConnectAsync();
                    Logger.Info("BLE device connection established.");
                }
                catch (Exception e)
                {
                    Logger.Error("BLE device connection failed", e);
                    return false;
                }
            }
            else
            {
                Logger.Info("Unable to connect to BLE device. Already connected.");
            }
            return true;
        }

        #endregion

        #region --Misc Methods (Private)--


        #endregion

        #region --Misc Methods (Protected)--


        #endregion
        //--------------------------------------------------------Events:---------------------------------------------------------------------\\
        #region --Events--
        private void DEVICE_ConnectionStatusChanged(BluetoothLEDevice sender, object args)
        {
            switch (sender.ConnectionStatus)
            {
                case BluetoothConnectionStatus.Disconnected:
                    SetState(BLEDeviceState.DISCONNECTED);
                    break;

                case BluetoothConnectionStatus.Connected:
                    SetState(BLEDeviceState.CONNECTED);
                    break;
            }
        }

        #endregion
    }
}
