using System.Threading;
using System.Threading.Tasks;
using Logging;
using Manager.Classes.Chat;
using UWPX_UI_Context.Classes.DataTemplates.Dialogs;
using XMPP_API.Classes.Network.XML.Messages.XEP_0249;

namespace UWPX_UI_Context.Classes.DataContext.Dialogs
{
    public class MucInviteDialogContext
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        public readonly MucInviteDialogDataTemplate MODEL = new MucInviteDialogDataTemplate();
        private CancellationTokenSource inviteCTS;
        private Task inviteTask;

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--


        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--


        #endregion
        //--------------------------------------------------------Misc Methods:---------------------------------------------------------------\\
        #region --Misc Methods (Public)--
        public void UpdateView(ChatDataTemplate chat)
        {
            MODEL.RoomIsPasswordProtected = !string.IsNullOrEmpty(chat.Chat.muc.password);
            MODEL.IncludePassword = MODEL.RoomIsPasswordProtected;
            MODEL.Title = "Invite to: " + (string.IsNullOrEmpty(chat.Chat.muc.name) ? chat.Chat.bareJid : chat.Chat.muc.name);
        }

        public async Task InviteAsync(ChatDataTemplate chat)
        {
            inviteCTS = new CancellationTokenSource();

            // Create a new task:
            inviteTask = Task.Run(async () =>
            {
                MODEL.IsInviting = true;
                Logger.Info("Sending an XEP-0249: Direct MUC Invitation to " + MODEL.TargetBareJid + " for room " + chat.Chat.bareJid + ".");
                string pw = MODEL.IncludePassword ? chat.Chat.muc.password : null;
                await chat.Client.xmppClient.SendAsync(new DirectMUCInvitationMessage(chat.Client.dbAccount.bareJid, MODEL.TargetBareJid, chat.Chat.bareJid, pw, MODEL.Reason));
                MODEL.IsInviting = false;
            }, inviteCTS.Token);

            // Await it:
            try
            {
                await inviteTask;
            }
            catch (TaskCanceledException) { }
            inviteCTS = null;
        }

        public void Cancel()
        {
            if (!(inviteCTS is null))
            {
                inviteCTS.Cancel();
                MODEL.IsInviting = false;
                Logger.Info("Canceled XEP-0249: Direct MUC Invitation for " + MODEL.TargetBareJid + ".");
            }
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
