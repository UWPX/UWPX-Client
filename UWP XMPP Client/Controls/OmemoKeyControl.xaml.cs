using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

namespace UWP_XMPP_Client.Controls
{
    public sealed partial class OmemoKeyControl : UserControl
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        public byte[] PubKey
        {
            get { return (byte[])GetValue(PubKeyProperty); }
            set { SetValue(PubKeyProperty, value); }
        }
        public static readonly DependencyProperty PubKeyProperty = DependencyProperty.Register(nameof(PubKey), typeof(byte[]), typeof(OmemoKeyControl), new PropertyMetadata(null));

        public byte[] PrivKey
        {
            get { return (byte[])GetValue(PrivKeyProperty); }
            set { SetValue(PrivKeyProperty, value); }
        }
        public static readonly DependencyProperty PrivKeyProperty = DependencyProperty.Register(nameof(PrivKey), typeof(byte[]), typeof(OmemoKeyControl), new PropertyMetadata(null));

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        /// <summary>
        /// Basic Constructor
        /// </summary>
        /// <history>
        /// 05/08/2018 Created [Fabian Sauter]
        /// </history>
        public OmemoKeyControl()
        {
            this.InitializeComponent();
        }

        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--


        #endregion
        //--------------------------------------------------------Misc Methods:---------------------------------------------------------------\\
        #region --Misc Methods (Public)--


        #endregion

        #region --Misc Methods (Private)--


        #endregion

        #region --Misc Methods (Protected)--


        #endregion
        //--------------------------------------------------------Events:---------------------------------------------------------------------\\
        #region --Events--


        #endregion
    }
}
