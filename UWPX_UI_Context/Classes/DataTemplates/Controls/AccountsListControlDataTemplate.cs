using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Data_Manager2.Classes;
using Shared.Classes;
using Shared.Classes.Collections;
using XMPP_API.Classes;

namespace UWPX_UI_Context.Classes.DataTemplates.Controls
{
    public sealed class AccountsListControlDataTemplate: AbstractDataTemplate
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        private bool _IsLoading;
        public bool IsLoading
        {
            get => _IsLoading;
            set => SetProperty(ref _IsLoading, value);
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

                IsLoading = true;
                loadingAccountsTask = Task.Run(() =>
                {
                    ACCOUNTS.Clear();
                    CustomObservableCollection<ClientConnectionHandler> clients = ConnectionHandler.INSTANCE.GetClients();
                    IEnumerable<AccountDataTemplate> accounts = clients.Select((client) =>
                    {
                        return new AccountDataTemplate
                        {
                            Client = client.client
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


        #endregion

        #region --Misc Methods (Protected)--


        #endregion
        //--------------------------------------------------------Events:---------------------------------------------------------------------\\
        #region --Events--


        #endregion
    }
}
