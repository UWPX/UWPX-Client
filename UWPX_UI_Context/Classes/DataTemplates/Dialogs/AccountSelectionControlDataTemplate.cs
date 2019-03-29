using Data_Manager2.Classes;
using Shared.Classes;
using Shared.Classes.Collections;
using System.Linq;
using XMPP_API.Classes.Network;

namespace UWPX_UI_Context.Classes.DataTemplates.Dialogs
{
    public sealed class AccountSelectionControlDataTemplate : AbstractDataTemplate
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        public readonly CustomObservableCollection<XMPPClientDataTemplate> CLIENTS = new CustomObservableCollection<XMPPClientDataTemplate>(true);

        private XMPPClientDataTemplate _SelectedItem;
        public XMPPClientDataTemplate SelectedItem
        {
            get { return _SelectedItem; }
            set { SetProperty(ref _SelectedItem, value); }
        }

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        public AccountSelectionControlDataTemplate()
        {
            LoadClients();
            SetSelectedItem();
        }

        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--
        private void SetSelectedItem()
        {
            foreach (XMPPClientDataTemplate client in CLIENTS)
            {
                if (client.ConnectionState == ConnectionState.CONNECTED)
                {
                    SelectedItem = client;
                    return;
                }
            }

            if (SelectedItem is null && CLIENTS.Count > 0)
            {
                SelectedItem = CLIENTS[0];
            }
        }

        #endregion
        //--------------------------------------------------------Misc Methods:---------------------------------------------------------------\\
        #region --Misc Methods (Public)--


        #endregion

        #region --Misc Methods (Private)--
        private void LoadClients()
        {
            CLIENTS.Clear();
            CLIENTS.AddRange(ConnectionHandler.INSTANCE.getClients().Select((x) => new XMPPClientDataTemplate(x)));
        }

        #endregion

        #region --Misc Methods (Protected)--


        #endregion
        //--------------------------------------------------------Events:---------------------------------------------------------------------\\
        #region --Events--


        #endregion
    }
}
