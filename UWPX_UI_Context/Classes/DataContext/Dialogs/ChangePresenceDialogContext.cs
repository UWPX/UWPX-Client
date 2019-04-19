﻿using System.Threading.Tasks;
using Data_Manager2.Classes.DBManager;
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
        public async Task SavePresenceAsync()
        {
            await Task.Run(async () =>
            {
                // Save presence and status:
                MODEL.Client.getXMPPAccount().presence = MODEL.SelectedItem.Presence;
                MODEL.Client.getXMPPAccount().status = MODEL.Status;

                AccountDBManager.INSTANCE.setAccount(MODEL.Client.getXMPPAccount(), false);

                // Send the updated presence and status to the server:
                await MODEL.Client.GENERAL_COMMAND_HELPER.setPreseceAsync(MODEL.SelectedItem.Presence, MODEL.Status);
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
