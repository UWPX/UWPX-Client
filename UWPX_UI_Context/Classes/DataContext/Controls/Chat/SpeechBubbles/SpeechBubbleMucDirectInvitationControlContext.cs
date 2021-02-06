using System.Linq;
using System.Threading.Tasks;
using Logging;
using Manager.Classes.Chat;
using Storage.Classes.Contexts;
using Storage.Classes.Models.Chat;
using UWPX_UI_Context.Classes.DataTemplates.Controls.Chat.SpeechBubbles;
using Windows.UI.Xaml;

namespace UWPX_UI_Context.Classes.DataContext.Controls.Chat.SpeechBubbles
{
    public class SpeechBubbleMucDirectInvitationControlContext
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        public readonly SpeechBubbleMucDirectInvitationControlDataTemplate MODEL = new SpeechBubbleMucDirectInvitationControlDataTemplate();

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--


        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--


        #endregion
        //--------------------------------------------------------Misc Methods:---------------------------------------------------------------\\
        #region --Misc Methods (Public)--
        public void UpdateView(DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue is ChatMessageDataTemplate msg)
            {
                Task.Run(() =>
                {
                    using (MainDbContext ctx = new MainDbContext())
                    {
                        MODEL.Invite = ctx.MucDirectInvitations.Where(i => i.id == msg.Message.id).FirstOrDefault();
                    }
                    if (MODEL.Invite is null)
                    {
                        Logger.Error("Failed to load MUC direct invite for: " + msg.Message.id);
                        return;
                    }

                    MODEL.Header = "You have been invited to: " + MODEL.Invite.roomJid;
                    MODEL.Room = MODEL.Invite.roomJid;
                    MODEL.Sender = msg.Chat.bareJid;
                    MODEL.Reason = MODEL.Invite.reason;
                    switch (MODEL.Invite.state)
                    {
                        case MucDirectInvitationState.ACCEPTED:
                            MODEL.Declined = false;
                            MODEL.Accepted = true;
                            break;

                        case MucDirectInvitationState.DECLINED:
                            MODEL.Declined = true;
                            MODEL.Accepted = false;
                            break;

                        default:
                            MODEL.Declined = false;
                            MODEL.Accepted = false;
                            break;
                    }
                });
            }
        }

        public void Accept(ChatMessageDataTemplate msg)
        {
            MODEL.Invite.state = MucDirectInvitationState.ACCEPTED;
            using (MainDbContext ctx = new MainDbContext())
            {
                ctx.Update(MODEL.Invite);
            }
        }

        public void Decline(ChatMessageDataTemplate msg)
        {
            MODEL.Invite.state = MucDirectInvitationState.DECLINED;
            using (MainDbContext ctx = new MainDbContext())
            {
                ctx.Update(MODEL.Invite);
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
