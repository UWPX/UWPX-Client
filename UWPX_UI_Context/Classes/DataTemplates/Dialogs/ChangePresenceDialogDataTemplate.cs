using System.Linq;
using Shared.Classes;
using Shared.Classes.Collections;
using XMPP_API.Classes;

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

        private XMPPClient _Client;
        public XMPPClient Client
        {
            get => _Client;
            set => SetClientProperty(value);
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
        private void SetClientProperty(XMPPClient value)
        {
            if (SetProperty(ref _Client, value, nameof(Client)) && !(value is null))
            {
                SelectedItem = PRESENCES.Where(x => value.getXMPPAccount().presence == x.Presence).FirstOrDefault();
                Status = Client.getXMPPAccount().status;
            }
        }

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
