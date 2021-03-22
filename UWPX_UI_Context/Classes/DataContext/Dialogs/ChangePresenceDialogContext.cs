using System;
using System.Threading.Tasks;
using Logging;
using UWPX_UI_Context.Classes.DataTemplates.Dialogs;

namespace UWPX_UI_Context.Classes.DataContext.Dialogs
{
    public class ChangePresenceDialogContext
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        public readonly ChangePresenceDialogDataTemplate MODEL = new ChangePresenceDialogDataTemplate();

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--


        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--


        #endregion
        //--------------------------------------------------------Misc Methods:---------------------------------------------------------------\\
        #region --Misc Methods (Public)--
        public async Task<bool> SavePresenceAsync()
        {
            return await Task.Run(async () =>
            {
                MODEL.IsSaving = true;
                // Save presence and status:
                MODEL.Client.xmppClient.getXMPPAccount().presence = MODEL.SelectedItem.Presence;
                MODEL.Client.xmppClient.getXMPPAccount().status = MODEL.Status;
                try
                {
                    // Send the updated presence and status to the server:
                    await MODEL.Client.xmppClient.GENERAL_COMMAND_HELPER.setPreseceAsync(MODEL.SelectedItem.Presence, MODEL.Status);
                }
                catch (Exception e)
                {
                    Logger.Error("Failed to update presence for account: " + MODEL.Client.dbAccount.bareJid, e);
                    MODEL.ErrorText = "Saving presence failed - view logs!";
                    return false;
                }
                finally
                {
                    MODEL.IsSaving = false;
                }

                // Update the DB:
                MODEL.Client.dbAccount.presence = MODEL.SelectedItem.Presence;
                MODEL.Client.dbAccount.status = MODEL.Status;
                MODEL.Client.dbAccount.Update();
                return true;
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
