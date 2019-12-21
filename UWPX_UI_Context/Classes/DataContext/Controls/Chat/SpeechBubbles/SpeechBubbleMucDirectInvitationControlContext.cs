using System.Threading.Tasks;
using Data_Manager2.Classes;
using Data_Manager2.Classes.DBManager;
using Logging;
using UWPX_UI_Context.Classes.DataTemplates;
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
                    MODEL.Invite = ChatDBManager.INSTANCE.getMUCDirectInvitation(msg.Message.id);
                    if (MODEL.Invite is null)
                    {
                        Logger.Error("Failed to load MUC direct invite for: " + msg.Message.id);
                        return;
                    }

                    MODEL.Header = "You have been invited to: " + MODEL.Invite.roomJid;
                    MODEL.Room = MODEL.Invite.roomJid;
                    MODEL.Sender = msg.Chat.chatJabberId;
                    MODEL.Reason = MODEL.Invite.reason;
                    switch (MODEL.Invite.state)
                    {
                        case MUCDirectInvitationState.ACCEPTED:
                            MODEL.Declined = false;
                            MODEL.Accepted = true;
                            break;

                        case MUCDirectInvitationState.DECLINED:
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

        public async Task AcceptAsync(ChatMessageDataTemplate msg)
        {
            await Task.Run(() => ChatDBManager.INSTANCE.setMUCDirectInvitationState(msg.Message.id, MUCDirectInvitationState.ACCEPTED));
        }

        public async Task DeclineAsync(ChatMessageDataTemplate msg)
        {
            await Task.Run(async () => await ChatDBManager.INSTANCE.deleteChatMessageAsync(msg.Message, true));
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
