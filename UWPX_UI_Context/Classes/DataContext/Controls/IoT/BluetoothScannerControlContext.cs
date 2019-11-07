using System.Threading.Tasks;
using UWPX_UI_Context.Classes.DataTemplates.Controls.IoT;
using XMPP_API_IoT.Classes.Bluetooth;
using XMPP_API_IoT.Classes.Bluetooth.Events;

namespace UWPX_UI_Context.Classes.DataContext.Controls.IoT
{
    public class BluetoothScannerControlContext
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        public readonly BluetoothScannerControlDataTemplate MODEL = new BluetoothScannerControlDataTemplate();

        public BLEScanner scanner { get; private set; }

        public delegate void BLEScannerStateChangedEventHandler(BluetoothScannerControlContext ctx, BLEScannerStateChangedEventArgs args);
        public delegate void BLEDeviceFoundEventHandler(BluetoothScannerControlContext ctx, BLEDeviceEventArgs args);

        public event BLEScannerStateChangedEventHandler ScannerStateChanged;
        public event BLEDeviceFoundEventHandler ScannerDeviceFound;

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        public BluetoothScannerControlContext()
        {

        }

        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--


        #endregion
        //--------------------------------------------------------Misc Methods:---------------------------------------------------------------\\
        #region --Misc Methods (Public)--
        public async Task StartAsync(string mac)
        {
            if (!(scanner is null))
            {
                Stop();
            }
            scanner = new BLEScanner(mac);
            scanner.StateChanged += Scanner_StateChanged;
            scanner.DeviceFound += Scanner_DeviceFound;
            await scanner.StartAsync();
        }

        public void Stop()
        {
            if (scanner is null)
            {
                return;
            }
            scanner.StateChanged -= Scanner_StateChanged;
            scanner.DeviceFound -= Scanner_DeviceFound;
            scanner.Stop();
            scanner.Dispose();
            scanner = null;
        }

        #endregion

        #region --Misc Methods (Private)--


        #endregion

        #region --Misc Methods (Protected)--


        #endregion
        //--------------------------------------------------------Events:---------------------------------------------------------------------\\
        #region --Events--
        private void Scanner_DeviceFound(BLEScanner scanner, BLEDeviceEventArgs args)
        {
            ScannerDeviceFound?.Invoke(this, args);
            Stop();
        }

        private void Scanner_StateChanged(BLEScanner scanner, BLEScannerStateChangedEventArgs args)
        {
            ScannerStateChanged?.Invoke(this, args);

            switch (args.NEW_STATE)
            {
                case BLEScannerState.DISABLED:
                    break;

                case BLEScannerState.SCANNING:
                    break;

                case BLEScannerState.CONNECTING:
                    break;

                case BLEScannerState.ERROR_BLE_NOT_SUPPORTED:
                    MODEL.ErrorMsg = "Your device does not support Bluetooth LE!";
                    break;

                case BLEScannerState.ERROR_BLUETOOTH_NOT_SUPPORTED:
                    MODEL.ErrorMsg = "No Bluetooth radio available!";
                    break;

                case BLEScannerState.ERROR_BLUETOOTH_DISABLED:
                    MODEL.ErrorMsg = "Bluetooth disabled!";
                    break;
            }
        }

        #endregion
    }
}
