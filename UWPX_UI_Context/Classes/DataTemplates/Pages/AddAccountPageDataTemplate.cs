﻿using System.Linq;
using Manager.Classes;
using Shared.Classes;
using Storage.Classes.Contexts;
using Storage.Classes.Models.Account;
using XMPP_API.Classes;

namespace UWPX_UI_Context.Classes.DataTemplates.Pages
{
    public sealed class AddAccountPageDataTemplate: AbstractDataTemplate
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        private string _BareJidText;
        public string BareJidText
        {
            get => _BareJidText;
            set => SetBareJidProperty(value);
        }
        private bool _AccountExists;
        public bool AccountExists
        {
            get => _AccountExists;
            set => SetProperty(ref _AccountExists, value);
        }
        private bool _IsValidBareJid;
        public bool IsValidBareJid
        {
            get => _IsValidBareJid;
            set => SetIsValidBareJidProperty(value);
        }
        private bool _Step1ValidJid;
        public bool Step1ValidJid
        {
            get => _Step1ValidJid;
            set => SetProperty(ref _Step1ValidJid, value);
        }
        private string _Password;
        public string Password
        {
            get => _Password;
            set => SetProperty(ref _Password, value);
        }
        private AccountModel _Account;
        public AccountModel Account
        {
            get => _Account;
            internal set => SetAccountProperty(value);
        }
        private AccountModel _OldAccount;
        public AccountModel OldAccount
        {
            get => _OldAccount;
            set => SetOldAccountProperty(value);
        }

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--


        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--
        private void SetAccountProperty(AccountModel value)
        {
            if (SetProperty(ref _Account, value, nameof(Account)))
            {
                BareJidText = Account.bareJid;
            }
        }

        private void SetOldAccountProperty(AccountModel value)
        {
            if (SetProperty(ref _OldAccount, value, nameof(OldAccount)))
            {
                // Load a copy of the account for editing it:
                using (MainDbContext ctx = new MainDbContext())
                {
                    Account = ctx.Accounts.Where(a => string.Equals(a.bareJid, value.bareJid)).Include(ctx.GetIncludePaths(typeof(AccountModel))).FirstOrDefault();
                }
            }
        }

        private void SetBareJidProperty(string value)
        {
            if (SetProperty(ref _BareJidText, value, nameof(BareJidText)))
            {
                AccountExists = ConnectionHandler.INSTANCE.GetClient(value) is not null;
                IsValidBareJid = Utils.isBareJid(value);
                Step1ValidJid = IsValidBareJid && !AccountExists;
            }
        }

        private void SetIsValidBareJidProperty(bool value)
        {
            if (SetProperty(ref _IsValidBareJid, value, nameof(IsValidBareJid)) && value && Account is not null)
            {
                // Update domain and local part if needed:
                string domainPart = Utils.getJidDomainPart(BareJidText);
                if (string.Equals(Account.server.address, Account.fullJid.domainPart))
                {
                    Account.server.address = domainPart;
                }
                Account.fullJid.domainPart = domainPart;
                Account.fullJid.localPart = Utils.getJidLocalPart(BareJidText);
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
