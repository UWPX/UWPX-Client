using Storage.Classes.Models.Omemo;
using UWPX_UI_Context.Classes.DataContext.Controls.OMEMO;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace UWPX_UI.Controls.OMEMO
{
    public sealed partial class OmemoDeviceControl: UserControl
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        public OmemoDeviceModel Device
        {
            get => (OmemoDeviceModel)GetValue(DeviceProperty);
            set => SetValue(DeviceProperty, value);
        }
        public static readonly DependencyProperty DeviceProperty = DependencyProperty.Register(nameof(Device), typeof(OmemoDeviceModel), typeof(OmemoDeviceControl), new PropertyMetadata(null, OnDeviceChanged));

        public readonly OmemoDeviceControlContext VIEW_MODEL = new OmemoDeviceControlContext();
        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        public OmemoDeviceControl()
        {
            InitializeComponent();
        }

        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--


        #endregion
        //--------------------------------------------------------Misc Methods:---------------------------------------------------------------\\
        #region --Misc Methods (Public)--
        private void UpdateView(DependencyPropertyChangedEventArgs e)
        {
            VIEW_MODEL.UpdateView(e);
        }

        #endregion

        #region --Misc Methods (Private)--


        #endregion

        #region --Misc Methods (Protected)--


        #endregion
        //--------------------------------------------------------Events:---------------------------------------------------------------------\\
        #region --Events--
        private static void OnDeviceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is OmemoDeviceControl control)
            {
                control.UpdateView(e);
            }
        }

        #endregion
    }
}
