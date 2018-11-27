using Logging;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Threading.Tasks;
using UWP_XMPP_Client.DataTemplates;
using Windows.Security.Cryptography.Certificates;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace UWP_XMPP_Client.Controls
{
    public sealed partial class CertificateControl : UserControl
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        public Certificate Cert
        {
            get { return (Certificate)GetValue(CertProperty); }
            set
            {
                SetValue(CertProperty, value);
                updateCertDetails();
            }
        }
        public static readonly DependencyProperty CertProperty = DependencyProperty.Register("Cert", typeof(Certificate), typeof(CertificateControl), null);

        private readonly ObservableCollection<CertificateDetailsTemplate> CERT_DETAILS;

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        /// <summary>
        /// Basic Constructor
        /// </summary>
        /// <history>
        /// 25/05/2018 Created [Fabian Sauter]
        /// </history>
        public CertificateControl()
        {
            this.CERT_DETAILS = new ObservableCollection<CertificateDetailsTemplate>();
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
        private void updateCertDetails()
        {
            CERT_DETAILS.Clear();
            if (Cert is null)
            {
                validFormShort_run.Text = "-";
                validToShort_run.Text = "-";
                return;
            }

            validFormShort_run.Text = Cert.ValidFrom.ToString("dd.MM.yyyy");
            validToShort_run.Text = Cert.ValidTo.ToString("dd.MM.yyyy");

            CERT_DETAILS.Add(new CertificateDetailsTemplate()
            {
                value = Cert.FriendlyName,
                name = "Friendly name"
            });
            CERT_DETAILS.Add(new CertificateDetailsTemplate()
            {
                value = Cert.HasPrivateKey.ToString(),
                name = "Has private key"
            });
            CERT_DETAILS.Add(new CertificateDetailsTemplate()
            {
                value = Cert.IsPerUser.ToString(),
                name = "Is per user"
            });
            CERT_DETAILS.Add(new CertificateDetailsTemplate()
            {
                value = Cert.Issuer,
                name = "Issuer"
            });
            CERT_DETAILS.Add(new CertificateDetailsTemplate()
            {
                value = Cert.KeyAlgorithmName,
                name = "Key algorithm name"
            });
            CERT_DETAILS.Add(new CertificateDetailsTemplate()
            {
                value = Cert.KeyStorageProviderName,
                name = "Key storage provider name"
            });
            CERT_DETAILS.Add(new CertificateDetailsTemplate()
            {
                value = Cert.ValidTo.ToString("dd.MM.yyyy HH:mm"),
                name = "Valid to"
            });
            CERT_DETAILS.Add(new CertificateDetailsTemplate()
            {
                value = Cert.ValidFrom.ToString("dd.MM.yyyy HH:mm"),
                name = "Valid from"
            });
            CERT_DETAILS.Add(new CertificateDetailsTemplate()
            {
                value = Cert.Subject,
                name = "Subject"
            });
            CERT_DETAILS.Add(new CertificateDetailsTemplate()
            {
                value = byteArryToString(Cert.SerialNumber),
                name = "Serial number"
            });
            CERT_DETAILS.Add(new CertificateDetailsTemplate()
            {
                value = Cert.SignatureAlgorithmName,
                name = "Signature algorithm name"
            });
            CERT_DETAILS.Add(new CertificateDetailsTemplate()
            {
                value = Cert.SignatureHashAlgorithmName,
                name = "Signature hash algorithm name"
            });
        }

        private string byteArryToString(byte[] b)
        {
            StringBuilder s = new StringBuilder();
            for (int i = 0; i < b.Length; i++)
            {
                s.Append(b[i]);
                if (i < b.Length)
                {
                    s.Append(' ');
                }
            }
            return s.ToString();
        }

        private async Task exportCertificate()
        {
            try
            {
                FileSavePicker picker = new FileSavePicker()
                {
                    SuggestedStartLocation = PickerLocationId.Desktop,
                    SuggestedFileName = Cert.Subject?.Replace('.', '_').Replace(' ', '-'),
                    DefaultFileExtension = ".cer"
                };
                picker.FileTypeChoices.Add("Certificate", new List<string>() { ".cer" });

                StorageFile file = await picker.PickSaveFileAsync();

                if (file != null)
                {
                    var blob = Cert.GetCertificateBlob();
                    await FileIO.WriteBufferAsync(file, blob);
                }
            }
            catch (Exception e)
            {
                Logger.Error("Failed to export certificate: ", e);
            }
        }

        #endregion

        #region --Misc Methods (Protected)--


        #endregion
        //--------------------------------------------------------Events:---------------------------------------------------------------------\\
        #region --Events--
        private async void exportCert_btn_Click(object sender, RoutedEventArgs e)
        {
            if (Cert != null)
            {
                await exportCertificate();
            }
        }

        #endregion
    }
}
