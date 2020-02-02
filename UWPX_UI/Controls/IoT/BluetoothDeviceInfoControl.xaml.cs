using System.ComponentModel;
using UWPX_UI_Context.Classes;
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
            get => (BLEDevice)GetValue(DeviceProperty);
            set => SetValue(DeviceProperty, value);
        }
        public static readonly DependencyProperty DeviceProperty = DependencyProperty.Register(nameof(Device), typeof(BLEDevice), typeof(BluetoothDeviceInfoControl), new PropertyMetadata(null, OnDeviceChanged));

        public bool IsInputValid
        {
            get => (bool)GetValue(IsInputValidProperty);
            set => SetValue(IsInputValidProperty, value);
        }
        public static readonly DependencyProperty IsInputValidProperty = DependencyProperty.Register(nameof(IsInputValid), typeof(bool), typeof(BluetoothDeviceInfoControl), new PropertyMetadata(false));

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        public BluetoothDeviceInfoControl()
        {
            InitializeComponent();
            UpdateViewState(State_Loading.Name);
            VIEW_MODEL.MODEL.PropertyChanged += MODEL_PropertyChanged;
            IsInputValid = VIEW_MODEL.MODEL.IsInputValid;
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

        private void MODEL_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (string.Equals(e.PropertyName, nameof(VIEW_MODEL.MODEL.IsInputValid)))
            {
                IsInputValid = VIEW_MODEL.MODEL.IsInputValid;
            }
        }

        private void AccountSelectionControl_AccountSelectionChanged(AccountSelectionControl sender, Classes.Events.AccountSelectionChangedEventArgs args)
        {
            VIEW_MODEL.MODEL.Client = args.CLIENT;
        }

        private void AccountSelectionControl_AddAccountClicked(AccountSelectionControl sender, CancelEventArgs args)
        {
            // Remove the current page from the back stack to prevent the user from navigating back to it:
            UiUtils.RemoveLastBackStackEntry();
        }
        #endregion
    }
}
