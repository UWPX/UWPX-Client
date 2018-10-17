using System.Collections.Generic;
using UWP_XMPP_Client.Classes.Collections;
using UWP_XMPP_Client.DataTemplates;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace UWP_XMPP_Client.Controls.Omemo
{
    public sealed partial class OmemoDeviceListControl : UserControl
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        public string Header
        {
            get { return (string)GetValue(HeaderProperty); }
            set { SetValue(HeaderProperty, value); }
        }
        public static readonly DependencyProperty HeaderProperty = DependencyProperty.Register(nameof(Header), typeof(string), typeof(OmemoDeviceListControl), new PropertyMetadata(""));

        private readonly CustomObservableCollection<UintTemplate> DEVICES;

        public class BindModel
        {
            public uint value { get; set; }
        }

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        /// <summary>
        /// Basic Constructor
        /// </summary>
        /// <history>
        /// 15/08/2018 Created [Fabian Sauter]
        /// </history>
        public OmemoDeviceListControl()
        {
            this.DEVICES = new CustomObservableCollection<UintTemplate>();
            this.InitializeComponent();
        }

        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--
        public void setDevices(IList<uint> devices)
        {
            DEVICES.Clear();
            List<UintTemplate> dev = new List<UintTemplate>();
            foreach (uint d in devices)
            {
                dev.Add(new UintTemplate
                {
                    value = d
                });
            }
            DEVICES.AddRange(dev);
            updateDeviceInfo();
        }

        #endregion
        //--------------------------------------------------------Misc Methods:---------------------------------------------------------------\\
        #region --Misc Methods (Public)--


        #endregion

        #region --Misc Methods (Private)--
        private void updateDeviceInfo()
        {
            if (DEVICES.Count != 1)
            {
                devicesInfo_tbx.Text = DEVICES.Count + " devices found.";
            }
            else
            {
                devicesInfo_tbx.Text = "1 device found.";
            }

        }

        #endregion

        #region --Misc Methods (Protected)--


        #endregion
        //--------------------------------------------------------Events:---------------------------------------------------------------------\\
        #region --Events--


        #endregion
    }
}
