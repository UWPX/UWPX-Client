using System.Collections.Generic;
using System.Threading.Tasks;
using Logging;
using Manager.Classes;
using UWPX_UI_Context.Classes.DataTemplates.Dialogs;
using UWPX_UI_Context.Classes.DataTemplates.Pages;
using XMPP_API.Classes;
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
        public void SetAccount(string fullJid)
        {
            IList<XMPPAccount> accounts = AccountDBManager.INSTANCE.loadAllAccounts();

            foreach (XMPPAccount account in accounts)
            {
                if (string.Equals(account.getFullJid(), fullJid))
                {
                    SetAccount(account);
                    return;
                }
            }
            Logger.Error("Failed to load account for full JID: " + fullJid);
        }

        public void SetAccount(XMPPAccount account)
        {
            MODEL.Account = account;
        }

        #endregion
        //--------------------------------------------------------Misc Methods:---------------------------------------------------------------\\
        #region --Misc Methods (Public)--
        public async Task CreateAccountAsync()
        {
            await Task.Run(async () =>
            {
                XMPPUser user = new XMPPUser(MODEL.BareJidText, Utils.getRandomResourcePart());
                XMPPAccount account = new XMPPAccount(user)
                {
                    color = UiUtils.GenRandomHexColor(),
                };

                // Look up the DNS SRV record:
                await account.dnsSrvLookupAsync();
                SetAccount(account);
            });
        }

        public async Task SaveAccountAsync()
        {
            await Task.Run(() => AccountDBManager.INSTANCE.setAccount(MODEL.Account, true, true));
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
                await Task.Run(async () =>
                {
                    Logger.Info("Deleting account: " + MODEL.Account.getBareJid());
                    try
                    {
                        await ConnectionHandler.INSTANCE.RemoveAccountAsync(MODEL.Account.getBareJid());
                    }
                    catch (System.Exception e)
                    {
                        Logger.Error("Failed to disconnect account for deletion.", e);
                    }

                    if (!dataTemplate.KeepChatMessages)
                    {
                        await ChatDBManager.INSTANCE.deleteAllChatMessagesForAccountAsync(MODEL.Account.getBareJid());
                    }
                    if (!dataTemplate.KeepChats)
                    {
                        ChatDBManager.INSTANCE.deleteAllChatsForAccount(MODEL.Account.getBareJid());
                    }
                    AccountDBManager.INSTANCE.deleteAccount(MODEL.Account, true, true);
                    Logger.Info("Finished deleting account: " + MODEL.Account.getBareJid());
                });
            }
        }

        public void ChangeCertRequirements(CertificateRequirementsDialogDataTemplate dataTemplate)
        {
            if (dataTemplate.Confirmed)
            {
                MODEL.Account.connectionConfiguration.IGNORED_CERTIFICATE_ERRORS.Clear();
                foreach (CertificateRequirementDataTemplate item in dataTemplate.ITEMS)
                {
                    if (!item.Required)
                    {
                        MODEL.Account.connectionConfiguration.IGNORED_CERTIFICATE_ERRORS.Add(item.CertError);
                    }
                }
            }
        }

        public async Task OnWhatIsAJidAsync()
        {
            await UiUtils.LaunchUriAsync(new System.Uri("https://uwpx.org/support/"));
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
