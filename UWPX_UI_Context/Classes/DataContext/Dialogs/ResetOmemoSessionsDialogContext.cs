using System.Threading.Tasks;
using Logging;
using Manager.Classes.Chat;
using Storage.Classes.Contexts;
using Storage.Classes.Models.Omemo;
using UWPX_UI_Context.Classes.DataTemplates.Dialogs;

namespace UWPX_UI_Context.Classes.DataContext.Dialogs
{
    public class ResetOmemoSessionsDialogContext
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        public readonly ResetOmemoSessionsDialogDataTemplate MODEL = new ResetOmemoSessionsDialogDataTemplate();

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--


        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--


        #endregion
        //--------------------------------------------------------Misc Methods:---------------------------------------------------------------\\
        #region --Misc Methods (Public)--
        public Task ResetAsync(ChatDataTemplate chat)
        {
            return Task.Run(() =>
            {
                MODEL.Working = true;
                Logger.Info($"Resetting OMEMO sessions for '{chat.Chat.bareJid}'...");
                int count = 0;
                using (MainDbContext ctx = new MainDbContext())
                {
                    foreach (OmemoDeviceModel device in chat.Chat.omemoInfo.devices)
                    {
                        if (device.session is not null)
                        {
                            ctx.Remove(device.session);
                            device.session = null;
                            ctx.Update(device);
                            count++;
                        }
                    }
                }
                Logger.Info($"Resetting {count} OMEMO sessions for '{chat.Chat.bareJid}' done.");

                if (MODEL.ResetOwnDeviceSessions)
                {
                    Logger.Info("Resetting own OMEMO sessions...");
                    count = 0;
                    using (MainDbContext ctx = new MainDbContext())
                    {
                        foreach (OmemoDeviceModel device in chat.Client.dbAccount.omemoInfo.devices)
                        {
                            if (device.session is not null)
                            {
                                ctx.Remove(device.session);
                                device.session = null;
                                ctx.Update(device);
                                count++;
                            }
                        }
                    }
                    Logger.Info($"Resetting {count} own OMEMO sessions done.");
                }
                MODEL.Working = false;
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
