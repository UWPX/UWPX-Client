using Logging;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

        private ObservableCollection<CertificateDetailsTemplate> certDetails;

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
            this.certDetails = new ObservableCollection<CertificateDetailsTemplate>();
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
            certDetails.Clear();
            if (Cert == null)
            {
                validFormShort_run.Text = "-";
                validToShort_run.Text = "-";
                return;
            }

            validFormShort_run.Text = Cert.ValidFrom.ToString("dd.MM.yyyy");
            validToShort_run.Text = Cert.ValidTo.ToString("dd.MM.yyyy");

            certDetails.Add(new CertificateDetailsTemplate()
            {
                value = Cert.FriendlyName,
                name = "Friendly name"
            });
            certDetails.Add(new CertificateDetailsTemplate()
            {
                value = Cert.HasPrivateKey.ToString(),
                name = "Has private key"
            });
            certDetails.Add(new CertificateDetailsTemplate()
            {
                value = Cert.IsPerUser.ToString(),
                name = "Is per user"
            });
            certDetails.Add(new CertificateDetailsTemplate()
            {
                value = Cert.Issuer,
                name = "Issuer"
            });
            certDetails.Add(new CertificateDetailsTemplate()
            {
                value = Cert.KeyAlgorithmName,
                name = "Key algorithm name"
            });
            certDetails.Add(new CertificateDetailsTemplate()
            {
                value = Cert.KeyStorageProviderName,
                name = "Key storage provider name"
            });
            certDetails.Add(new CertificateDetailsTemplate()
            {
                value = Cert.ValidTo.ToString("dd.MM.yyyy HH:mm"),
                name = "Valid to"
            });
            certDetails.Add(new CertificateDetailsTemplate()
            {
                value = Cert.ValidFrom.ToString("dd.MM.yyyy HH:mm"),
                name = "Valid from"
            });
            certDetails.Add(new CertificateDetailsTemplate()
            {
                value = Cert.Subject,
                name = "Subject"
            });
            certDetails.Add(new CertificateDetailsTemplate()
            {
                value = byteArryToString(Cert.SerialNumber),
                name = "Serial number"
            });
            certDetails.Add(new CertificateDetailsTemplate()
            {
                value = Cert.SignatureAlgorithmName,
                name = "Signature algorithm name"
            });
            certDetails.Add(new CertificateDetailsTemplate()
            {
                value = Cert.SignatureHashAlgorithmName,
                name = "Signature hash algorithmName"
            });
        }

        private string byteArryToString(byte[] b)
        {
            string s = "";
            for (int i = 0; i < b.Length; i++)
            {
                s += b[i];
                if (i < b.Length)
                {
                    s += " ";
                }
            }
            return s;
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
