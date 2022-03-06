﻿using System.Linq;
using System.Threading.Tasks;
using Manager.Classes;
using Manager.Classes.Chat;
using Push.Classes;
using Storage.Classes;
using Storage.Classes.Contexts;
using Storage.Classes.Models.Account;
using UWPX_UI_Context.Classes.DataTemplates.Dialogs;
using UWPX_UI_Context.Classes.DataTemplates.Pages;
using Windows.Foundation;
using XMPP_API.Classes.Network;

namespace UWPX_UI_Context.Classes.DataContext.Pages
{
    public sealed class AddAccountPageContext
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        public readonly AddAccountPageDataTemplate MODEL = new AddAccountPageDataTemplate();

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--


        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--


        #endregion
        //--------------------------------------------------------Misc Methods:---------------------------------------------------------------\\
        #region --Misc Methods (Public)--
        public Task CreateAccountAsync()
        {
            return Task.Run(async () =>
             {
                 JidModel jid = new JidModel
                 {
                     localPart = XMPP_API.Classes.Utils.getJidLocalPart(MODEL.BareJidText),
                     domainPart = XMPP_API.Classes.Utils.getJidDomainPart(MODEL.BareJidText),
                     resourcePart = XMPP_API.Classes.Utils.getRandomResourcePart()
                 };
                 AccountModel account = new AccountModel(jid, UiUtils.GenRandomHexColor());
                 account.omemoInfo.GenerateOmemoKeys();

                 // Look up the DNS SRV record:
                 SRVLookupResult result = await XMPPAccount.dnsSrvLookupAsync(jid.domainPart);
                 if (result.SUCCESS)
                 {
                     account.server.address = result.SERVER_ADDRESS;
                     account.server.port = result.PORT;
                 }
                 MODEL.Account = account;
             });
        }

        public Task SaveAccountAsync()
        {
            return Task.Run(async () =>
             {
                 // Cleanup password vault:
                 using (MainDbContext ctx = new MainDbContext())
                 {
                     Vault.CleanupVault(ctx.Accounts.Select(a => a.bareJid).ToList());
                 }

                 // Update the password:
                 XMPPAccount account = MODEL.Account.ToXMPPAccount();
                 account.user.password = MODEL.Password;
                 Vault.StorePassword(account);

                 // Push:
                 MODEL.Account.push.state = Settings.GetSettingBoolean(SettingsConsts.PUSH_ENABLED) ? PushState.ENABLING : PushState.DISABLED;

                 // New account add it to the DB:
                 if (MODEL.OldAccount is null)
                 {
                     MODEL.Account.Add();
                     ConnectionHandler.INSTANCE.AddAccount(MODEL.Account);

                     // Initialize Push:
                     PushManager.INSTANCE.Init();
                 }
                 // Update the old account:
                 else
                 {
                     MODEL.Account.Update();
                     await ConnectionHandler.INSTANCE.UpdateAccountAsync(MODEL.Account);
                 }
             });
        }

        public void ColorSelected(ColorPickerDialogDataTemplate dataTemplate)
        {
            if (dataTemplate.Confirmed)
            {
                MODEL.Account.color = UiUtils.ColorToHexString(dataTemplate.SelectedColor);
            }
        }

        public async Task DeleteAccountAsync(DeleteAccountConfirmDialogDataTemplate dataTemplate)
        {
            if (dataTemplate.Confirmed)
            {
                // Delete the account:
                await DataCache.INSTANCE.DeleteAccountAsync(MODEL.Account);

                // Update push accounts:
                if (Settings.GetSettingBoolean(SettingsConsts.PUSH_ENABLED))
                {
                    await PushManager.INSTANCE.InitPushForAccountsAsync();
                }
            }
        }

        public void ChangeCertRequirements(CertificateRequirementsDialogDataTemplate dataTemplate)
        {
            if (dataTemplate.Confirmed)
            {
                MODEL.Account.server.ignoredCertificateErrors.Clear();
                foreach (CertificateRequirementDataTemplate item in dataTemplate.ITEMS)
                {
                    if (!item.Required)
                    {
                        MODEL.Account.server.ignoredCertificateErrors.Add(item.CertError);
                    }
                }
            }
        }

        public IAsyncOperation<bool> OnWhatIsAJidAsync()
        {
            return UiUtils.LaunchUriAsync(new System.Uri("https://uwpx.org/support/"));
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
