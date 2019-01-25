using Data_Manager2.Classes.DBManager;
using Data_Manager2.Classes.Events;
using Logging;
using Shared.Classes;
using Shared.Classes.Collections;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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

        public readonly CustomObservableCollection<AccountDataTemplate> ACCOUNTS = new CustomObservableCollection<AccountDataTemplate>();

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
                    AccountDBManager.INSTANCE.AccountChanged -= INSTANCE_AccountChanged;
                    AccountDBManager.INSTANCE.AccountChanged += INSTANCE_AccountChanged;

                    ACCOUNTS.Clear();

                    try
                    {
                        IEnumerable<AccountDataTemplate> accounts = AccountDBManager.INSTANCE.loadAllAccounts().Select((x) =>
                        {
                            return new AccountDataTemplate
                            {
                                Account = x
                            };
                        });

                        ACCOUNTS.AddRange(accounts);
                    }
                    catch (Exception e)
                    {
                        Logger.Error("Failed to load accounts for the accounts settings page.", e);
                    }
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
        private void INSTANCE_AccountChanged(AccountDBManager handler, AccountChangedEventArgs args)
        {
            IEnumerable<AccountDataTemplate> accounts = ACCOUNTS.Where((account) => string.Equals(account.Account.getIdAndDomain(), args.ACCOUNT.getIdAndDomain()));
            foreach (AccountDataTemplate account in accounts)
            {
                if (args.REMOVED)
                {
                    ACCOUNTS.Remove(account);
                }
                else
                {
                    account.Account = args.ACCOUNT;
                }
            }
        }

        #endregion
    }
}
