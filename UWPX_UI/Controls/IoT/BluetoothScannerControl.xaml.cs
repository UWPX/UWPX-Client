using System.Threading.Tasks;
using UWPX_UI_Context.Classes.DataContext.Controls.IoT;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using XMPP_API.Classes.XmppUri;

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

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        public BluetoothScannerControl()
        {
            InitializeComponent();
        }

        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--


        #endregion
        //--------------------------------------------------------Misc Methods:---------------------------------------------------------------\\
        #region --Misc Methods (Public)--
        public async Task StartAsync()
        {
            await VIEW_MODEL.StartAsync(RegisterIoTUriAction.MAC);
        }

        public void Stop()
        {
            VIEW_MODEL.Stop();
        }

        #endregion

        #region --Misc Methods (Private)--


        #endregion

        #region --Misc Methods (Protected)--


        #endregion
        //--------------------------------------------------------Events:---------------------------------------------------------------------\\
        #region --Events--
        private async static void OnRegisterIoTUriActionChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is BluetoothScannerControl bluetoothScannerControl)
            {
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

        #endregion
    }
}
