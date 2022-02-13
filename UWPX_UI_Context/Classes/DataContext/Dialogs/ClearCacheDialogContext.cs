using System.Threading.Tasks;
using Logging;
using Manager.Classes;
using Storage.Classes;
using Storage.Classes.Contexts;
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
                    using (MainDbContext ctx = new MainDbContext())
                    {
                        // Chats:
                        if (MODEL.ChatMessages)
                        {
                            ctx.RemoveRange(ctx.ChatMessages);
                        }
                        if (MODEL.Chats)
                        {
                            ctx.RemoveRange(ctx.Chats);
                        }
                        if (MODEL.Images)
                        {
                            ctx.RemoveRange(ctx.ChatMessageReceivedImages);
                        }
                        if (MODEL.SpamMessages)
                        {
                            ctx.RemoveRange(ctx.SpamMessages);
                        }

                        // Disco:


                        // MUC:
                        if (MODEL.MucChatInfo)
                        {
                            ctx.RemoveRange(ctx.MucInfos);
                        }
                        if (MODEL.MucOccupants)
                        {
                            ctx.RemoveRange(ctx.MucOccupants);
                        }
                        if (MODEL.MucDirectInvites)
                        {
                            ctx.RemoveRange(ctx.MucDirectInvitations);
                        }

                        // Accounts:
                        if (MODEL.Accounts)
                        {
                            ctx.RemoveRange(ctx.Accounts);
                        }
                        if (MODEL.PasswordVault)
                        {
                            Vault.DeleteAllVaults();
                        }
                        if (MODEL.Jids)
                        {
                            ctx.RemoveRange(ctx.Jids);
                        }
                        if (MODEL.Servers)
                        {
                            ctx.RemoveRange(ctx.Servers);
                        }
                        if (MODEL.MamRequests)
                        {
                            ctx.RemoveRange(ctx.MamRequests);
                        }

                        // OMEMO:
                        if (MODEL.OmemoDeviceListSubscriptions)
                        {
                            ctx.RemoveRange(ctx.DeviceListSubscriptions);
                        }
                        if (MODEL.OmemoDevices)
                        {
                            ctx.RemoveRange(ctx.Devices);
                        }
                        if (MODEL.DiscoIdentities)
                        {
                            ctx.RemoveRange(ctx.IdentityKeyPairs);
                        }
                        if (MODEL.OmemoPreKeys)
                        {
                            ctx.RemoveRange(ctx.PreKeys);
                        }
                        if (MODEL.OmemoSignedPreKeys)
                        {
                            ctx.RemoveRange(ctx.SignedPreKeys);
                        }
                        if (MODEL.OmemoSessions)
                        {
                            ctx.RemoveRange(ctx.Sessions);
                        }
                        if (MODEL.OmemoFingerprints)
                        {
                            ctx.RemoveRange(ctx.Fingerprints);
                        }
                        if (MODEL.SkippedMessageKeyGroup)
                        {
                            ctx.RemoveRange(ctx.SkippedMessageKeyGroup);
                        }
                        if (MODEL.SkippedMessageKeyGroups)
                        {
                            ctx.RemoveRange(ctx.SkippedMessageKeyGroups);
                        }
                        if (MODEL.SkippedMessageKeys)
                        {
                            ctx.RemoveRange(ctx.SkippedMessageKeys);
                        }

                        // Clients:
                        if (MODEL.ReloadClients)
                        {
                            ConnectionHandler.INSTANCE.ReloadClients();
                        }
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
