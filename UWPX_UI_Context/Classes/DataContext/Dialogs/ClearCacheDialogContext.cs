using System.Threading.Tasks;
using Logging;
using Shared.Classes.SQLite;
using UWPX_UI_Context.Classes.DataTemplates.Dialogs;

namespace UWPX_UI_Context.Classes.DataContext.Dialogs
{
    public sealed class ClearCacheDialogContext
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        public readonly ClearCacheDialogDataTemplate MODEL = new ClearCacheDialogDataTemplate();

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--


        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--


        #endregion
        //--------------------------------------------------------Misc Methods:---------------------------------------------------------------\\
        #region --Misc Methods (Public)--
        public async Task<bool> ClearCacheAsync()
        {
            MODEL.IsCleaningCache = true;
            bool result = await Task.Run(() =>
            {
                try
                {
                    // General:
                    if (MODEL.ChatMessages)
                    {
                        AbstractDBManager.dB.RecreateTable<ChatMessageModel>();
                    }
                    if (MODEL.Chats)
                    {
                        AbstractDBManager.dB.RecreateTable<ChatModel>();
                    }
                    if (MODEL.Images)
                    {
                        AbstractDBManager.dB.RecreateTable<ImageTable>();
                    }

                    // Disco:
                    if (MODEL.DiscoFeatures)
                    {
                        AbstractDBManager.dB.RecreateTable<DiscoFeatureTable>();
                    }
                    if (MODEL.DiscoIdentities)
                    {
                        AbstractDBManager.dB.RecreateTable<DiscoIdentityTable>();
                    }
                    if (MODEL.DiscoItems)
                    {
                        AbstractDBManager.dB.RecreateTable<DiscoItemTable>();
                    }

                    // MUC:
                    if (MODEL.MucChatInfo)
                    {
                        AbstractDBManager.dB.RecreateTable<MucInfoModel>();
                    }
                    if (MODEL.MucOccupants)
                    {
                        AbstractDBManager.dB.RecreateTable<MucOccupantModel>();
                    }
                    if (MODEL.MucDirectInvites)
                    {
                        AbstractDBManager.dB.RecreateTable<MucDirectInvitationModel>();
                    }

                    // Accounts:
                    if (MODEL.Accounts)
                    {
                        AbstractDBManager.dB.RecreateTable<AccountTable>();
                    }
                    if (MODEL.PasswordVault)
                    {
                        Vault.deleteAllVaults();
                    }
                    if (MODEL.IgnoredCertErrors)
                    {
                        AbstractDBManager.dB.RecreateTable<IgnoredCertificateErrorTable>();
                    }
                    if (MODEL.ConnectionOptions)
                    {
                        AbstractDBManager.dB.RecreateTable<ConnectionOptionsTable>();
                    }

                    // OMEMO:
                    if (MODEL.OmemoDeviceListSubscriptions)
                    {
                        AbstractDBManager.dB.RecreateTable<OmemoDeviceListSubscriptionTable>();
                    }
                    if (MODEL.OmemoDevices)
                    {
                        AbstractDBManager.dB.RecreateTable<OmemoDeviceTable>();
                    }
                    if (MODEL.DiscoIdentities)
                    {
                        AbstractDBManager.dB.RecreateTable<OmemoIdentityKeyTable>();
                    }
                    if (MODEL.OmemoPreKeys)
                    {
                        AbstractDBManager.dB.RecreateTable<OmemoPreKeyTable>();
                    }
                    if (MODEL.OmemoSignedPreKeys)
                    {
                        AbstractDBManager.dB.RecreateTable<OmemoSignedPreKeyTable>();
                    }
                    if (MODEL.OmemoSessions)
                    {
                        AbstractDBManager.dB.RecreateTable<OmemoSessionStoreTable>();
                    }
                    if (MODEL.OmemoFingerprints)
                    {
                        AbstractDBManager.dB.RecreateTable<OmemoFingerprintTable>();
                    }

                    // Clients:
                    if (MODEL.ReloadClients)
                    {
                        ConnectionHandler.INSTANCE.ReloadClients();
                    }
                    return true;
                }
                catch (System.Exception e)
                {
                    Logger.Error("Failed to clear cache!", e);
                    return false;
                }
            });
            MODEL.IsCleaningCache = false;
            return result;
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
