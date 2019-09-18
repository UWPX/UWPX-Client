using UWPX_UI_Context.Classes.DataContext.Controls.IoT;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using XMPP_API_IoT.Classes.Bluetooth;

namespace UWPX_UI.Controls.IoT
{
    public sealed partial class BluetoothDeviceInfoControl: UserControl
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        public readonly BluetoothDeviceInfoControlContext VIEW_MODEL = new BluetoothDeviceInfoControlContext();

        public BLEDevice Device
        {
            get { return (BLEDevice)GetValue(DeviceProperty); }
            set { SetValue(DeviceProperty, value); }
        }
        public static readonly DependencyProperty DeviceProperty = DependencyProperty.Register(nameof(Device), typeof(BLEDevice), typeof(BluetoothDeviceInfoControl), new PropertyMetadata(null, OnDeviceChanged));

        public bool InputValid
        {
            get { return (bool)GetValue(InputValidProperty); }
            set { SetValue(InputValidProperty, value); }
        }
        public static readonly DependencyProperty InputValidProperty = DependencyProperty.Register(nameof(InputValid), typeof(bool), typeof(BluetoothDeviceInfoControl), new PropertyMetadata(false));

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        public BluetoothDeviceInfoControl()
        {
            InitializeComponent();
            UpdateViewState(State_Loading.Name);
        }

        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--


        #endregion
        //--------------------------------------------------------Misc Methods:---------------------------------------------------------------\\
        #region --Misc Methods (Public)--


        #endregion

        #region --Misc Methods (Private)--
        private void LoadInfos(BLEDevice device)
        {
            UpdateViewState(State_Loading.Name);
            if (!(device is null))
            {
                VIEW_MODEL.LoadInfos(device);
                UpdateViewState(State_Success.Name);
            }
        }

        private void UpdateViewState(string state)
        {
            VisualStateManager.GoToState(this, state, true);
        }

        #endregion

        #region --Misc Methods (Protected)--


        #endregion
        //--------------------------------------------------------Events:---------------------------------------------------------------------\\
        #region --Events--
        private static void OnDeviceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is BluetoothDeviceInfoControl bluetoothDeviceInfoControl)
            {
                if (e.NewValue is BLEDevice device)
                {
                    bluetoothDeviceInfoControl.LoadInfos(device);
                }
                else
                {
                    bluetoothDeviceInfoControl.LoadInfos(null);
                }
            }
        }

        private void UserControl_Unloaded(object sender, RoutedEventArgs e)
        {

        }
        #endregion
    }
}
