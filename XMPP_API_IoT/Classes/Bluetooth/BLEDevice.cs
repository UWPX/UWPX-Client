using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Logging;
using Microsoft.Toolkit.Uwp.Connectivity;
using Windows.Devices.Bluetooth;
using Windows.Devices.Bluetooth.GenericAttributeProfile;
using Windows.Security.Cryptography;
using Windows.Storage.Streams;
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

        public readonly CharacteristicsCache CACHE = new CharacteristicsCache();
        private readonly Dictionary<Guid, GattCharacteristic> CHARACTERISTICS = new Dictionary<Guid, GattCharacteristic>();

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
                    await StartCacheAsync();
                    Logger.Info("Characteristics cache started.");
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

        public async Task<string> ReadStringAsync(GattCharacteristic c)
        {
            byte[] data = await ReadBytesAsync(c);
            return !(data is null) ? BitConverter.ToString(data) : null;
        }

        public async Task<short> ReadShortAsync(GattCharacteristic c)
        {
            byte[] data = await ReadBytesAsync(c);
            return !(data is null) ? BitConverter.ToInt16(data, 0) : (short)-1;
        }

        public async Task<byte[]> ReadBytesAsync(GattCharacteristic characteristic)
        {
            GattReadResult vRes = await characteristic.ReadValueAsync();
            return vRes.Status == GattCommunicationStatus.Success ? ReadBytesFromBuffer(vRes.Value) : null;
        }

        public async Task<byte[]> ReadBytesAsync(Guid uuid)
        {
            CHARACTERISTICS.TryGetValue(uuid, out GattCharacteristic c);
            return c != null ? await ReadBytesAsync(c) : null;
        }

        #endregion

        #region --Misc Methods (Private)--
        private async Task StartCacheAsync()
        {
            // Get all services:
            GattDeviceServicesResult sResult = await DEVICE.GetGattServicesAsync();
            if (sResult.Status == GattCommunicationStatus.Success)
            {
                CHARACTERISTICS.Clear();
                foreach (GattDeviceService s in sResult.Services)
                {
                    // Get all characteristics:
                    GattCharacteristicsResult cResult = await s.GetCharacteristicsAsync();
                    if (cResult.Status == GattCommunicationStatus.Success)
                    {
                        foreach (GattCharacteristic c in cResult.Characteristics)
                        {
                            CHARACTERISTICS.Add(c.Uuid, c);
                            if (c.CharacteristicProperties.HasFlag(GattCharacteristicProperties.Read))
                            {
                                await LoadCharacteristicValueAsync(c);
                            }
                        }
                    }
                }
                Logger.Debug("Finished requesting characteristics.");

                await SubscribeToCharacteristicsAsync(CharacteristicsCache.SUBSCRIBE_TO_CHARACTERISTICS);
            }
            else
            {
                Logger.Warn("Failed to request GetGattServicesAsync() - " + sResult.Status.ToString());
            }
        }

        private async Task SubscribeToCharacteristicsAsync(Guid[] uuids)
        {
            foreach (Guid uuid in uuids)
            {
                if (CHARACTERISTICS.ContainsKey(uuid))
                {
                    GattCharacteristic c = CHARACTERISTICS[uuid];
                    if (await SubscribeToCharacteristicAsync(c))
                    {
                        await LoadCharacteristicValueAsync(c);
                    }
                }
            }
        }

        /// <summary>
        /// Subscribes to the given GattCharacteristic if it allows it.
        /// </summary>
        /// <param name="c">The GattCharacteristic you want to subscribe to.</param>
        /// <returns>Returns true on success.</returns>
        private async Task<bool> SubscribeToCharacteristicAsync(GattCharacteristic c)
        {
            // Check if characteristic supports subscriptions:
            GattClientCharacteristicConfigurationDescriptorValue cccdValue = GattClientCharacteristicConfigurationDescriptorValue.None;
            if (c.CharacteristicProperties.HasFlag(GattCharacteristicProperties.Indicate))
            {
                cccdValue = GattClientCharacteristicConfigurationDescriptorValue.Indicate;
            }
            else if (c.CharacteristicProperties.HasFlag(GattCharacteristicProperties.Notify))
            {
                cccdValue = GattClientCharacteristicConfigurationDescriptorValue.Notify;
            }
            else
            {
                return false;
            }

            // Set subscribed:
            GattCommunicationStatus status;
            try
            {
                status = await c.WriteClientCharacteristicConfigurationDescriptorAsync(cccdValue);
            }
            catch (Exception e)
            {
                Logger.Warn("Failed to subscribe to characteristic " + c.Uuid + " with " + e.ToString());
                return false;
            }

            // Add event handler:
            if (status == GattCommunicationStatus.Success)
            {
                c.ValueChanged -= C_ValueChanged;
                c.ValueChanged += C_ValueChanged;
                Logger.Debug("Subscribed to characteristic: " + c.Uuid);
                return true;
            }
            else
            {
                Logger.Warn("Failed to subscribe to characteristic " + c.Uuid + " with " + status);
            }
            return false;
        }

        /// <summary>
        /// Unsubscribes from the given GattCharacteristic if subscribed to it.
        /// </summary>
        /// <param name="c">The GattCharacteristic you want to unsubscribe from.</param>
        /// <returns>Returns true on success.</returns>
        private async Task<bool> UnsubscribeFromCharacteristicAsync(GattCharacteristic c)
        {
            try
            {
                GattCommunicationStatus status = await c.WriteClientCharacteristicConfigurationDescriptorAsync(GattClientCharacteristicConfigurationDescriptorValue.None);

                // Add event handler:
                if (status == GattCommunicationStatus.Success)
                {
                    c.ValueChanged -= C_ValueChanged;
                    Logger.Debug("Unsubscribed from characteristic: " + c.Uuid);
                    return true;
                }
                else
                {
                    Logger.Warn("Failed to unsubscribe from characteristic " + c.Uuid + " with " + status);
                }
            }
            catch (UnauthorizedAccessException)
            {
            }
            return false;
        }

        /// <summary>
        /// Loads the characteristics value from the given GattCharacteristic
        /// and adds the value to the characteristics dictionary.
        /// </summary>
        /// <param name="c">The characteristic the value should get added to the characteristics dictionary.</param>
        private async Task LoadCharacteristicValueAsync(GattCharacteristic c)
        {
            byte[] data = null;
            if (c.CharacteristicProperties.HasFlag(GattCharacteristicProperties.Read))
            {
                try
                {
                    // Load value from characteristic:
                    data = await ReadBytesAsync(c);

                    if (!(data is null))
                    {
                        CACHE.AddToDictionary(c.Uuid, data);
                    }
                }
                catch (Exception e)
                {
                    Logger.Error("Loading value from characteristic " + c.Uuid + " failed!", e);
                }
            }
            else
            {
                Logger.Debug("Unable to load value for characteristic " + c.Uuid + " - no READ property!");
            }
        }

        /// <summary>
        /// Reads all available bytes from the given buffer and converts them to big endian if necessary. 
        /// </summary>
        /// <param name="buffer">The buffer object you want to read from.</param>
        /// <returns>All available bytes from the buffer in big endian.</returns>
        private byte[] ReadBytesFromBuffer(IBuffer buffer)
        {
            DataReader reader = DataReader.FromBuffer(buffer);
            byte[] data = new byte[reader.UnconsumedBufferLength];
            reader.ReadBytes(data);
            return data;
        }

        public Task<GattWriteResult> WriteStringAsync(Guid uuid, string data)
        {
            return WriteBytesAsync(uuid, Encoding.UTF8.GetBytes(data));
        }

        public async Task<GattWriteResult> WriteBytesAsync(Guid uuid, byte[] data)
        {
            CHARACTERISTICS.TryGetValue(uuid, out GattCharacteristic c);
            if (c != null)
            {
                IBuffer buffer = CryptographicBuffer.CreateFromByteArray(data);
                GattWriteResult result = await c.WriteValueWithResultAsync(buffer);
                if (result.Status == GattCommunicationStatus.Success)
                {
                    // Convert to little endian:
                    CACHE.AddToDictionary(uuid, data);
                }
                return result;
            }
            else
            {
                Logger.Warn("Failed to write to write bytes to: " + uuid.ToString() + " - not loaded.");
            }
            return null;
        }

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

        private void C_ValueChanged(GattCharacteristic sender, GattValueChangedEventArgs args)
        {
            // Read bytes:
            byte[] data = ReadBytesFromBuffer(args.CharacteristicValue);

            // Insert characteristic and its value into a dictionary:
            CACHE.AddToDictionary(sender.Uuid, data, args.Timestamp.DateTime);
        }

        #endregion
    }
}
