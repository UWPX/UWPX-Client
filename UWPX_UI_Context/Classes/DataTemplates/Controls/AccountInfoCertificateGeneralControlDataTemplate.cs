﻿using Logging;
using Shared.Classes;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Threading.Tasks;
using Windows.Networking.Sockets;
using Windows.Security.Cryptography.Certificates;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.UI.Xaml;
using XMPP_API.Classes.Network;

namespace UWPX_UI_Context.Classes.DataTemplates.Controls
{
    public sealed class AccountInfoCertificateGeneralControlDataTemplate : AbstractDataTemplate
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        private string _ValidFromShort;
        public string ValidFromShort
        {
            get { return _ValidFromShort; }
            set { SetProperty(ref _ValidFromShort, value); }
        }
        private string _ValidToShort;
        public string ValidToShort
        {
            get { return _ValidToShort; }
            set { SetProperty(ref _ValidToShort, value); }
        }
        private string _IssuedTo;
        public string IssuedTo
        {
            get { return _IssuedTo; }
            set { SetProperty(ref _IssuedTo, value); }
        }
        private string _IssuedFrom;
        public string IssuedFrom
        {
            get { return _IssuedFrom; }
            set { SetProperty(ref _IssuedFrom, value); }
        }
        private StreamSocketInformation _SocketInfo;
        public StreamSocketInformation SocketInfo
        {
            get { return _SocketInfo; }
            set { SetProperty(ref _SocketInfo, value); }
        }

        public readonly ObservableCollection<CertificateDetailDataTemplate> DETAILS = new ObservableCollection<CertificateDetailDataTemplate>();

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        public AccountInfoCertificateGeneralControlDataTemplate()
        {
            ResetValues();
        }

        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--


        #endregion
        //--------------------------------------------------------Misc Methods:---------------------------------------------------------------\\
        #region --Misc Methods (Public)--
        public void UpdateViewModel(DependencyPropertyChangedEventArgs e)
        {
            if (e.OldValue is AccountDataTemplate accountOld)
            {
                Unsubscribe(accountOld);
            }

            ResetValues();
            if (e.NewValue is AccountDataTemplate accountNew)
            {
                Subscribe(accountNew);
                SocketInfo = accountNew.Account?.CONNECTION_INFO?.socketInfo;
                LoadValuesFromCert(accountNew.Account?.CONNECTION_INFO?.socketInfo?.ServerCertificate);
            }
        }

        public async Task ExportCertificateAsync()
        {
            try
            {
                if (SocketInfo is null || SocketInfo.ServerCertificate is null)
                {
                    return;
                }

                FileSavePicker picker = new FileSavePicker()
                {
                    SuggestedStartLocation = PickerLocationId.Desktop,
                    SuggestedFileName = SocketInfo.ServerCertificate.Subject?.Replace('.', '_').Replace(' ', '-'),
                    DefaultFileExtension = ".cer"
                };
                picker.FileTypeChoices.Add("Certificate", new List<string>() { ".cer" });

                StorageFile file = await picker.PickSaveFileAsync();

                if (file != null)
                {
                    var blob = SocketInfo.ServerCertificate.GetCertificateBlob();
                    await FileIO.WriteBufferAsync(file, blob);
                }
            }
            catch (Exception e)
            {
                Logger.Error("Failed to export certificate: ", e);
            }
        }

        #endregion

        #region --Misc Methods (Private)--
        private void ResetValues()
        {
            ValidFromShort = "-";
            ValidToShort = "-";
            IssuedTo = "-";
            IssuedFrom = "-";

            DETAILS.Clear();
        }

        private void Unsubscribe(AccountDataTemplate account)
        {
            account.Account.CONNECTION_INFO.PropertyChanged -= CONNECTION_INFO_PropertyChanged;
        }

        private void Subscribe(AccountDataTemplate account)
        {
            account.Account.CONNECTION_INFO.PropertyChanged += CONNECTION_INFO_PropertyChanged;
        }

        private void LoadValuesFromCert(Certificate cert)
        {
            ResetValues();

            if (cert is null)
            {
                return;
            }

            ValidFromShort = cert.ValidFrom.ToString("dd.MM.yyyy");
            ValidToShort = cert.ValidTo.ToString("dd.MM.yyyy");
            IssuedFrom = cert.Issuer;
            IssuedTo = cert.Subject;

            DETAILS.Add(new CertificateDetailDataTemplate()
            {
                Value = cert.FriendlyName,
                Name = "Friendly Name"
            });
            DETAILS.Add(new CertificateDetailDataTemplate()
            {
                Value = cert.HasPrivateKey.ToString(),
                Name = "Has private key"
            });
            DETAILS.Add(new CertificateDetailDataTemplate()
            {
                Value = cert.IsPerUser.ToString(),
                Name = "Is per user"
            });
            DETAILS.Add(new CertificateDetailDataTemplate()
            {
                Value = cert.Issuer,
                Name = "Issuer"
            });
            DETAILS.Add(new CertificateDetailDataTemplate()
            {
                Value = cert.KeyAlgorithmName,
                Name = "Key algorithm Name"
            });
            DETAILS.Add(new CertificateDetailDataTemplate()
            {
                Value = cert.KeyStorageProviderName,
                Name = "Key storage provider Name"
            });
            DETAILS.Add(new CertificateDetailDataTemplate()
            {
                Value = cert.ValidTo.ToString("dd.MM.yyyy HH:mm"),
                Name = "Valid to"
            });
            DETAILS.Add(new CertificateDetailDataTemplate()
            {
                Value = cert.ValidFrom.ToString("dd.MM.yyyy HH:mm"),
                Name = "Valid from"
            });
            DETAILS.Add(new CertificateDetailDataTemplate()
            {
                Value = cert.Subject,
                Name = "Subject"
            });
            DETAILS.Add(new CertificateDetailDataTemplate()
            {
                Value = ByteArryToString(cert.SerialNumber),
                Name = "Serial number"
            });
            DETAILS.Add(new CertificateDetailDataTemplate()
            {
                Value = cert.SignatureAlgorithmName,
                Name = "Signature algorithm Name"
            });
            DETAILS.Add(new CertificateDetailDataTemplate()
            {
                Value = cert.SignatureHashAlgorithmName,
                Name = "Signature hash algorithm Name"
            });
        }

        private string ByteArryToString(byte[] data)
        {
            StringBuilder s = new StringBuilder();
            foreach (byte b in data)
            {
                s.Append(b);
                s.Append(' ');
            }
            return s.ToString();
        }

        #endregion

        #region --Misc Methods (Protected)--


        #endregion
        //--------------------------------------------------------Events:---------------------------------------------------------------------\\
        #region --Events--
        private void CONNECTION_INFO_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (sender is ConnectionInformation connectionInfo)

                switch (e.PropertyName)
                {
                    case nameof(connectionInfo.socketInfo):
                        SocketInfo = connectionInfo?.socketInfo;
                        LoadValuesFromCert(connectionInfo?.socketInfo?.ServerCertificate);
                        break;
                }
        }

        #endregion
    }
}
