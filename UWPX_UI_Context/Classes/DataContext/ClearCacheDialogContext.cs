using Data_Manager2.Classes;
using Data_Manager2.Classes.DBTables;
using Data_Manager2.Classes.DBTables.Omemo;
using Logging;
using Shared.Classes.SQLite;
using System.Threading.Tasks;
using UWPX_UI_Context.Classes.DataTemplates;

namespace UWPX_UI_Context.Classes.DataContext
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
                        AbstractDBManager.dB.RecreateTable<ChatMessageTable>();
                    }
                    if (MODEL.Chats)
                    {
                        AbstractDBManager.dB.RecreateTable<ChatTable>();
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
                        AbstractDBManager.dB.RecreateTable<MUCChatInfoTable>();
                    }
                    if (MODEL.MucOccupants)
                    {
                        AbstractDBManager.dB.RecreateTable<MUCOccupantTable>();
                    }
                    if (MODEL.MucDirectInvites)
                    {
                        AbstractDBManager.dB.RecreateTable<MUCDirectInvitationTable>();
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

                    // Clients:
                    if (MODEL.ReloadClients)
                    {
                        ConnectionHandler.INSTANCE.reloadClients();
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
