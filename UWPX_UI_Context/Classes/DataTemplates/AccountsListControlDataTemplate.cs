using Data_Manager2.Classes;
using Shared.Classes;
using Shared.Classes.Collections;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using XMPP_API.Classes;

namespace UWPX_UI_Context.Classes.DataTemplates
{
    public sealed class AccountsListControlDataTemplate : AbstractDataTemplate
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        private bool _IsLoading;
        public bool IsLoading
        {
            get { return _IsLoading; }
            set { SetProperty(ref _IsLoading, value); }
        }

        public readonly CustomObservableCollection<AccountDataTemplate> ACCOUNTS = new CustomObservableCollection<AccountDataTemplate>(true);

        private Task loadingAccountsTask = null;

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        public AccountsListControlDataTemplate()
        {
            LoadAccounts();
        }

        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--


        #endregion
        //--------------------------------------------------------Misc Methods:---------------------------------------------------------------\\
        #region --Misc Methods (Public)--
        public void LoadAccounts()
        {
            Task.Run(async () =>
            {
                if (!(loadingAccountsTask is null))
                {
                    await loadingAccountsTask;
                }

                loadingAccountsTask = Task.Run(() =>
                {
                    IsLoading = true;
                    ConnectionHandler.INSTANCE.ClientsCollectionChanged -= INSTANCE_ClientsCollectionChanged;
                    ConnectionHandler.INSTANCE.ClientsCollectionChanged += INSTANCE_ClientsCollectionChanged;

                    ACCOUNTS.Clear();

                    CustomObservableCollection<XMPPClient> clients = ConnectionHandler.INSTANCE.getClients();
                    IEnumerable<AccountDataTemplate> accounts = clients.Select((client) =>
                    {
                        return new AccountDataTemplate
                        {
                            Client = client
                        };
                    });

                    ACCOUNTS.AddRange(accounts);
                });

                await loadingAccountsTask;
                IsLoading = false;
            });
        }

        #endregion

        #region --Misc Methods (Private)--
        private void AddClients(IList list)
        {
            foreach (object item in list)
            {
                if (item is XMPPClient client)
                {
                    ACCOUNTS.Add(new AccountDataTemplate
                    {
                        Client = client
                    });
                }
            }
        }

        private void RemoveClients(IList list)
        {
            foreach (object item in list)
            {
                if (item is XMPPClient client)
                {
                    foreach (AccountDataTemplate account in ACCOUNTS)
                    {
                        if (account.Client == client)
                        {
                            ACCOUNTS.Remove(account);
                            break;
                        }
                    }
                }
            }
        }

        #endregion

        #region --Misc Methods (Protected)--


        #endregion
        //--------------------------------------------------------Events:---------------------------------------------------------------------\\
        #region --Events--
        private void INSTANCE_ClientsCollectionChanged(ConnectionHandler handler, System.Collections.Specialized.NotifyCollectionChangedEventArgs args)
        {
            switch (args.Action)
            {
                case System.Collections.Specialized.NotifyCollectionChangedAction.Add:
                    AddClients(args.NewItems);
                    break;

                case System.Collections.Specialized.NotifyCollectionChangedAction.Move:
                    break;

                case System.Collections.Specialized.NotifyCollectionChangedAction.Remove:
                    RemoveClients(args.OldItems);
                    break;

                case System.Collections.Specialized.NotifyCollectionChangedAction.Replace:
                    RemoveClients(args.OldItems);
                    AddClients(args.NewItems);
                    break;

                case System.Collections.Specialized.NotifyCollectionChangedAction.Reset:
                default:
                    LoadAccounts();
                    break;
            }
        }

        #endregion
    }
}
