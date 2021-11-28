using System;
using Storage.Classes.Models.Omemo;
using UWPX_UI_Context.Classes.DataContext.Controls.OMEMO;
using UWPX_UI_Context.Classes.DataTemplates.Controls.Chat;
using UWPX_UI_Context.Classes.Events;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace UWPX_UI.Controls.OMEMO
{
    public sealed partial class OmemoTrustFingerprintControl: UserControl
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        public OmemoFingerprintDataTemplate Fingerprint
        {
            get => (OmemoFingerprintDataTemplate)GetValue(FingerprintProperty);
            set => SetValue(FingerprintProperty, value);
        }
        public static readonly DependencyProperty FingerprintProperty = DependencyProperty.Register(nameof(Fingerprint), typeof(OmemoFingerprintDataTemplate), typeof(OmemoTrustFingerprintControl), new PropertyMetadata(null, OnFingerprintChanged));

        public delegate void OmemoFingerprintTrustChangedEventHandler(object sender, OmemoFingerprintTrustChangedEventArgs e);
        public event OmemoFingerprintTrustChangedEventHandler OmemoFingerprintTrustChanged;

        public readonly OmemoTrustFingerprintControlContext VIEW_MODEL = new OmemoTrustFingerprintControlContext();
        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        public OmemoTrustFingerprintControl()
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
        private void UpdateView(DependencyPropertyChangedEventArgs e)
        {
            VIEW_MODEL.UpdateView(e);
        }

        #endregion

        #region --Misc Methods (Protected)--


        #endregion
        //--------------------------------------------------------Events:---------------------------------------------------------------------\\
        #region --Events--
        private void Trust_tgls_Toggled(object sender, RoutedEventArgs e)
        {
            Fingerprint.Fingerprint.trusted = trust_tgls.IsOn;
            OmemoFingerprintTrustChanged?.Invoke(this, new OmemoFingerprintTrustChangedEventArgs(Fingerprint.Fingerprint));
        }

        private static void OnFingerprintChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is OmemoTrustFingerprintControl control)
            {
                control.UpdateView(e);
            }
        }

        #endregion
    }
}
