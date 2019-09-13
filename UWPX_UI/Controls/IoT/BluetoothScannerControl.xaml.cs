using System.Threading.Tasks;
using UWPX_UI_Context.Classes.DataContext.Controls.IoT;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using XMPP_API.Classes.XmppUri;
using XMPP_API_IoT.Classes.Bluetooth;
using XMPP_API_IoT.Classes.Bluetooth.Events;

namespace UWPX_UI.Controls.IoT
{
    public sealed partial class BluetoothScannerControl: UserControl
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        public readonly BluetoothScannerControlContext VIEW_MODEL = new BluetoothScannerControlContext();

        public RegisterIoTUriAction RegisterIoTUriAction
        {
            get { return (RegisterIoTUriAction)GetValue(RegisterIoTUriActionProperty); }
            set { SetValue(RegisterIoTUriActionProperty, value); }
        }
        public static readonly DependencyProperty RegisterIoTUriActionProperty = DependencyProperty.Register(nameof(RegisterIoTUriAction), typeof(RegisterIoTUriAction), typeof(BluetoothScannerControl), new PropertyMetadata(null, OnRegisterIoTUriActionChanged));

        public BLEDevice Device
        {
            get { return (BLEDevice)GetValue(DeviceProperty); }
            set { SetValue(DeviceProperty, value); }
        }
        public static readonly DependencyProperty DeviceProperty = DependencyProperty.Register(nameof(Device), typeof(BLEDevice), typeof(BluetoothScannerControl), new PropertyMetadata(null, OnDeviceChanged));

        public delegate void DeviceChangedEventHandler(BluetoothScannerControl sender, BLEDeviceEventArgs args);

        public event DeviceChangedEventHandler DeviceChanged;

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        public BluetoothScannerControl()
        {
            InitializeComponent();
            UpdateViewState(State_Scanning.Name);
        }

        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--


        #endregion
        //--------------------------------------------------------Misc Methods:---------------------------------------------------------------\\
        #region --Misc Methods (Public)--
        public async Task StartAsync()
        {
            VIEW_MODEL.ScannerDeviceFound += VIEW_MODEL_ScannerDeviceFound;
            VIEW_MODEL.ScannerStateChanged += VIEW_MODEL_ScannerStateChanged;

            await VIEW_MODEL.StartAsync(RegisterIoTUriAction.MAC);
        }

        public void Stop()
        {
            VIEW_MODEL.ScannerDeviceFound -= VIEW_MODEL_ScannerDeviceFound;
            VIEW_MODEL.ScannerStateChanged -= VIEW_MODEL_ScannerStateChanged;

            VIEW_MODEL.Stop();
        }

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
        private async static void OnRegisterIoTUriActionChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is BluetoothScannerControl bluetoothScannerControl)
            {
                bluetoothScannerControl.Device = null;
                if (e.NewValue is RegisterIoTUriAction)
                {
                    await bluetoothScannerControl.StartAsync();
                }
                else
                {
                    bluetoothScannerControl.Stop();
                }
            }
        }

        private void UserControl_Unloaded(object sender, RoutedEventArgs e)
        {
            Stop();
        }

        private void VIEW_MODEL_ScannerStateChanged(BluetoothScannerControlContext ctx, BLEScannerStateChangedEventArgs args)
        {
            switch (args.NEW_STATE)
            {
                case BLEScannerState.DISABLED:
                    break;

                case BLEScannerState.SCANNING:
                    UpdateViewState(State_Scanning.Name);
                    break;

                case BLEScannerState.ERROR_BLE_NOT_SUPPORTED:
                case BLEScannerState.ERROR_BLUETOOTH_NOT_SUPPORTED:
                case BLEScannerState.ERROR_BLUETOOTH_DISABLED:
                default:
                    UpdateViewState(State_Error.Name);
                    break;
            }
        }

        private void VIEW_MODEL_ScannerDeviceFound(BluetoothScannerControlContext ctx, BLEDeviceEventArgs args)
        {
            UpdateViewState(State_Success.Name);
            Device = args.DEVICE;
        }

        private static void OnDeviceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is BluetoothScannerControl bluetoothScannerControl)
            {
                bluetoothScannerControl.DeviceChanged?.Invoke(bluetoothScannerControl, new BLEDeviceEventArgs(bluetoothScannerControl.Device));
            }
        }

        #endregion
    }
}
