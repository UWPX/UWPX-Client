using System;
using System.Threading.Tasks;
using Manager.Classes;
using Shared.Classes;
using Storage.Classes.Models.Account;
using Windows.Graphics.Imaging;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using XMPP_API.Classes;
using XMPP_API.Classes.Network;
using XMPP_API.Classes.Network.Events;

namespace UWPX_UI_Context.Classes.DataTemplates.Dialogs
{
    public class EditProfileDialogDataTemplate: AbstractDataTemplate
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        private bool _IsSaving;
        public bool IsSaving
        {
            get => _IsSaving;
            set => SetIsSavingProperty(value);
        }

        private bool _IsSaveEnabled;
        public bool IsSaveEnabled
        {
            get => _IsSaveEnabled;
            set => SetProperty(ref _IsSaveEnabled, value);
        }

        private bool _Error;
        public bool Error
        {
            get => _Error;
            set => SetProperty(ref _Error, value);
        }

        private string _ErrorText;
        public string ErrorText
        {
            get => _ErrorText;
            set => SetProperty(ref _ErrorText, value);
        }

        private bool _IsLoading;
        public bool IsLoading
        {
            get => _IsLoading;
            set => SetProperty(ref _IsLoading, value);
        }

        private Client _Client;
        public Client Client
        {
            get => _Client;
            set => SetClientProperty(value);
        }

        private string _BareJid;
        public string BareJid
        {
            get => _BareJid;
            set => SetProperty(ref _BareJid, value);
        }

        private ImageModel _Image;
        public ImageModel Image
        {
            get => _Image;
            set => SetProperty(ref _Image, value);
        }

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--


        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--
        private void SetIsSavingProperty(bool value)
        {
            if (SetProperty(ref _IsSaving, value, nameof(IsSaving)))
            {
                IsSaveEnabled = !value;
            }
        }

        private async void SetClientProperty(Client value)
        {
            Client old = Client;
            if (SetProperty(ref _Client, value, nameof(Client)))
            {
                if (old is not null)
                {
                    old.xmppClient.ConnectionStateChanged -= OnClientConnectionStateChanged;
                }
                if (value is not null)
                {
                    value.xmppClient.ConnectionStateChanged += OnClientConnectionStateChanged;
                    BareJid = value.dbAccount.bareJid;
                    Image = value.dbAccount.contactInfo.avatar is null ? null : value.dbAccount.contactInfo.avatar;
                }
                CheckForErrors();
            }
        }

        #endregion
        //--------------------------------------------------------Misc Methods:---------------------------------------------------------------\\
        #region --Misc Methods (Public)--
        public async Task SetImageAsync(SoftwareBitmap img)
        {
            if (img is null)
            {
                Image = null;
            }
            else
            {
                Image = new ImageModel();
                await Image.SetImageAsync(img);
            }
        }

        #endregion

        #region --Misc Methods (Private)--
        private void CheckForErrors()
        {
            if (Client is null)
            {
                ErrorText = "No valid account selected!";
                IsSaveEnabled = false;
            }
            else if (Client.xmppClient.getConnetionState() != ConnectionState.CONNECTED)
            {
                ErrorText = "Account not connected!";
                IsSaveEnabled = false;
            }
        }

        #endregion

        #region --Misc Methods (Protected)--


        #endregion
        //--------------------------------------------------------Events:---------------------------------------------------------------------\\
        #region --Events--
        private void OnClientConnectionStateChanged(XMPPClient client, ConnectionStateChangedEventArgs args)
        {
            CheckForErrors();
        }

        #endregion
    }
}
