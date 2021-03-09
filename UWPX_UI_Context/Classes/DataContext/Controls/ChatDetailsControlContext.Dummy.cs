using System.Threading.Tasks;
using Storage.Classes.Models.Chat;
using XMPP_API.Classes;

namespace UWPX_UI_Context.Classes.DataContext.Controls
{
    public sealed partial class ChatDetailsControlContext
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        private uint dummyMessageCount = 0;

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--


        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--


        #endregion
        //--------------------------------------------------------Misc Methods:---------------------------------------------------------------\\
        #region --Misc Methods (Public)--
        public void OnIsDummyChanged(bool isDummy)
        {
            MODEL.isDummy = isDummy;
        }

        public void LoadDummyContent(ChatModel chat)
        {
            AddDummyMessage(chat, "Hi", chat.bareJid, MessageState.READ);
            AddDummyMessage(chat, "Hey, what's up?", chat.accountBareJid, MessageState.SEND);
            AddDummyMessage(chat, "That's a great app.", chat.bareJid, MessageState.READ);
            MODEL.MessageText = "Yes, its awesome :D !";
        }

        #endregion

        #region --Misc Methods (Private)--
        private void AddDummyMessage(ChatModel chat, string msg, string fromUser, MessageState state)
        {
            AddDummyMessage(chat, msg, fromUser, state, chat.omemo.enabled, false);
        }

        private void AddDummyMessage(ChatModel chat, string msg, string fromUser, MessageState state, bool isEncrypted, bool isImage)
        {
            /*MODEL.CHAT_MESSAGES.Add(new DataTemplates.ChatMessageDataTemplate
            {
                Chat = chat,
                Message = new ChatMessageModel
                {
                    message = msg,
                    chatId = chat.id,
                    fromUser = fromUser,
                    date = DateTime.Now,
                    state = state,
                    type = MessageMessage.TYPE_CHAT,
                    isImage = isImage,
                    isDummyMessage = true,
                    isEncrypted = isEncrypted
                }
            });*/
        }

        private void SendDelayedMessage(ChatModel chat, string msg, bool isEncrypted, bool isImage, int msDelay)
        {
            Task.Run(async () =>
            {
                await Task.Delay(msDelay);
                AddDummyMessage(chat, msg, chat.bareJid, MessageState.READ, isEncrypted, isImage);
            });
        }

        private void SendDummyMessage(ChatModel chat, string msg)
        {
            AddDummyMessage(chat, msg, chat.accountBareJid, MessageState.SEND);
            OnDummyMessageSend(chat);
        }

        private void OnDummyMessageSend(ChatModel chat)
        {
            switch (++dummyMessageCount)
            {
                case 1:
                    MODEL.ChatPresence = Presence.Online;
                    break;

                case 3:
                    SendDelayedMessage(chat, Localisation.GetLocalizedString("chat_details_dummy_answer_3_img"), chat.omemo.enabled, true, 3000);
                    SendDelayedMessage(chat, Localisation.GetLocalizedString("chat_details_dummy_answer_3"), chat.omemo.enabled, false, 4000);
                    MODEL.ChatPresence = Presence.Chat;
                    break;

                case 4:
                    SendDelayedMessage(chat, Localisation.GetLocalizedString("chat_details_dummy_answer_4"), chat.omemo.enabled, false, 3000);
                    MODEL.ChatPresence = Presence.Online;
                    break;

                case 7:
                    SendDelayedMessage(chat, Localisation.GetLocalizedString("chat_details_dummy_answer_7"), chat.omemo.enabled, false, 3000);
                    break;

                case 11:
                    SendDelayedMessage(chat, Localisation.GetLocalizedString("chat_details_dummy_answer_11"), chat.omemo.enabled, false, 3000);
                    break;

                case 15:
                    SendDelayedMessage(chat, Localisation.GetLocalizedString("chat_details_dummy_answer_15"), chat.omemo.enabled, false, 3000);
                    MODEL.ChatPresence = Presence.Xa;
                    break;

                case 20:
                    SendDelayedMessage(chat, Localisation.GetLocalizedString("chat_details_dummy_answer_20"), chat.omemo.enabled, false, 3000);
                    break;

                case 30:
                    SendDelayedMessage(chat, Localisation.GetLocalizedString("chat_details_dummy_answer_30"), chat.omemo.enabled, false, 3000);
                    break;

                case 50:
                    SendDelayedMessage(chat, Localisation.GetLocalizedString("chat_details_dummy_answer_50_1"), chat.omemo.enabled, false, 3000);
                    SendDelayedMessage(chat, Localisation.GetLocalizedString("chat_details_dummy_answer_50_2"), chat.omemo.enabled, false, 4000);
                    SendDelayedMessage(chat, Localisation.GetLocalizedString("chat_details_dummy_answer_50_3"), chat.omemo.enabled, false, 5000);
                    SendDelayedMessage(chat, Localisation.GetLocalizedString("chat_details_dummy_answer_50_4"), chat.omemo.enabled, false, 6000);
                    SendDelayedMessage(chat, Localisation.GetLocalizedString("chat_details_dummy_answer_50_5"), chat.omemo.enabled, false, 7000);
                    SendDelayedMessage(chat, Localisation.GetLocalizedString("chat_details_dummy_answer_50_6"), chat.omemo.enabled, false, 8000);
                    SendDelayedMessage(chat, Localisation.GetLocalizedString("chat_details_dummy_answer_50_7"), chat.omemo.enabled, true, 9000);
                    Task.Run(async () =>
                    {
                        await Task.Delay(9000);
                        MODEL.ChatPresence = Presence.Unavailable;
                    });
                    break;
            }
        }

        #endregion

        #region --Misc Methods (Protected)--


        #endregion
        //--------------------------------------------------------Events:---------------------------------------------------------------------\\
        #region --Events--


        #endregion
    }
}
