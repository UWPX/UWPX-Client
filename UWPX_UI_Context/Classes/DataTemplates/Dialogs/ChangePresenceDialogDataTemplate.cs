using System.Linq;
using Manager.Classes;
using Shared.Classes;
using Shared.Classes.Collections;
using XMPP_API.Classes;
using XMPP_API.Classes.Network;

namespace UWPX_UI_Context.Classes.DataTemplates.Dialogs
{
    public class ChangePresenceDialogDataTemplate: AbstractDataTemplate
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        public readonly CustomObservableCollection<PresenceDataTemplate> PRESENCES = new CustomObservableCollection<PresenceDataTemplate>(true)
        {
            new PresenceDataTemplate(Presence.Online),
            new PresenceDataTemplate(Presence.Chat),
            new PresenceDataTemplate(Presence.Away),
            new PresenceDataTemplate(Presence.Dnd),
            new PresenceDataTemplate(Presence.Xa),
            new PresenceDataTemplate(Presence.Unavailable),
        };

        private PresenceDataTemplate _SelectedItem;
        public PresenceDataTemplate SelectedItem
        {
            get => _SelectedItem;
            set => SetProperty(ref _SelectedItem, value);
        }

        private string _Status;
        public string Status
        {
            get => _Status;
            set => SetProperty(ref _Status, value);
        }

        private Client _Client;
        public Client Client
        {
            get => _Client;
            set => SetClientProperty(value);
        }

        private bool _IsSaveEnabled;
        public bool IsSaveEnabled
        {
            get => _IsSaveEnabled;
            set => SetProperty(ref _IsSaveEnabled, value);
        }

        private bool _IsSaving;
        public bool IsSaving
        {
            get => _IsSaving;
            set => SetProperty(ref _IsSaving, value);
        }

        private string _ErrorText;
        public string ErrorText
        {
            get => _ErrorText;
            set => SetProperty(ref _ErrorText, value);
        }

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        public ChangePresenceDialogDataTemplate()
        {
            SelectedItem = PRESENCES[0];
        }

        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--
        private void SetClientProperty(Client value)
        {
            Client old = Client;
            if (SetProperty(ref _Client, value, nameof(Client)))
            {
                if (old is not null)
                {
                    old.xmppClient.ConnectionStateChanged -= Client_ConnectionStateChanged;
                }
                if (value is not null)
                {
                    SelectedItem = PRESENCES.Where(x => value.xmppClient.getXMPPAccount().presence == x.Presence).FirstOrDefault();
                    Status = Client.xmppClient.getXMPPAccount().status;
                    value.xmppClient.ConnectionStateChanged += Client_ConnectionStateChanged;
                }
                CheckForErrors();
            }
        }

        #endregion
        //--------------------------------------------------------Misc Methods:---------------------------------------------------------------\\
        #region --Misc Methods (Public)--


        #endregion

        #region --Misc Methods (Private)--
        private void CheckForErrors()
        {
            if (Client is null)
            {
                ErrorText = "No valid account selected!";
            }
            else if (Client.xmppClient.getConnetionState() != ConnectionState.CONNECTED)
            {
                ErrorText = "Account not connected!";
            }
            else if (SelectedItem is null || SelectedItem.Presence == Presence.NotDefined)
            {
                ErrorText = "No valid presence selected!";
            }
            else
            {
                IsSaveEnabled = true;
                return;
            }
            IsSaveEnabled = false;
        }

        #endregion

        #region --Misc Methods (Protected)--


        #endregion
        //--------------------------------------------------------Events:---------------------------------------------------------------------\\
        #region --Events--
        private void Client_ConnectionStateChanged(XMPPClient client, XMPP_API.Classes.Network.Events.ConnectionStateChangedEventArgs args)
        {
            CheckForErrors();
        }

        #endregion
    }
}
