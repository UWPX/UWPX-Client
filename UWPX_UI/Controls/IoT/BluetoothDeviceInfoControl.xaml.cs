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

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        public BluetoothDeviceInfoControl()
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
        private void UpdateView(DependencyPropertyChangedEventArgs args)
        {
            VIEW_MODEL.UpdateView(args);
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
                bluetoothDeviceInfoControl.UpdateView(e);
            }
        }

        private void UserControl_Unloaded(object sender, RoutedEventArgs e)
        {

        }
        #endregion
    }
}
