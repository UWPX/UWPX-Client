using System;
using System.Threading.Tasks;
using Logging;
using Shared.Classes.Collections;
using Storage.Classes.Contexts;
using Storage.Classes.Models.Chat;
using XMPP_API.Classes;
using XMPP_API.Classes.Network.XML.Messages.XEP_0045;

/// <summary>
/// https://xmpp.org/extensions/xep-0045.html
/// </summary>
namespace Manager.Classes
{
    public class MucJoinHelper: ITimedEntry, IDisposable
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        /// <summary>
        /// The Full JID of the room e.g. 'coven@chat.shakespeare.lit'.
        /// </summary>
        public readonly ChatModel CHAT;
        public readonly MucInfoModel INFO;
        public readonly XMPPClient CLIENT;

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        /// <summary>
        /// Basic Constructor
        /// </summary>
        /// <history>
        /// 06/01/2018 Created [Fabian Sauter]
        /// </history>
        public MucJoinHelper(XMPPClient client, ChatModel chat, MucInfoModel info)
        {
            CLIENT = client;
            CHAT = chat;
            INFO = info;
        }

        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--


        #endregion
        //--------------------------------------------------------Misc Methods:---------------------------------------------------------------\\
        #region --Misc Methods (Public)--
        public async Task requestReservedNicksAsync()
        {
            DiscoReservedRoomNicknamesMessages msg = new DiscoReservedRoomNicknamesMessages(CLIENT.getXMPPAccount().getFullJid(), CHAT.bareJid);
            await CLIENT.SendAsync(msg);
        }

        public async Task enterRoomAsync()
        {
            // Update MUC info:
            INFO.state = MucState.ENTERING;
            using (MainDbContext ctx = new MainDbContext()) // TODO: Continue here
            {
                ctx.Update(info);
            }
            MUCDBManager.INSTANCE.setMUCState(INFO.chatId, INFO.state, true);

            // Clear MUC members:
            MUCDBManager.INSTANCE.deleteAllOccupantsforChat(CHAT.id);

            // Create message:
            JoinRoomRequestMessage msg = new JoinRoomRequestMessage(CLIENT.getXMPPAccount().getFullJid(), CHAT.chatJabberId, INFO.nickname, INFO.password);

            // Subscribe to events for receiving answers:
            CLIENT.NewMUCMemberPresenceMessage -= CLIENT_NewMUCMemberPresenceMessage;
            CLIENT.NewMUCMemberPresenceMessage += CLIENT_NewMUCMemberPresenceMessage;

            // Send message:
            await CLIENT.SendAsync(msg);
            Logger.Info("Entering MUC room '" + CHAT.chatJabberId + "' as '" + INFO.nickname + '\'');
        }

        public bool CanGetRemoved()
        {
            switch (INFO.state)
            {
                case MUCState.ENTERING:
                    return false;

                default:
                    return true;
            }
        }

        public void Dispose()
        {
        }

        #endregion

        #region --Misc Methods (Private)--


        #endregion

        #region --Misc Methods (Protected)--


        #endregion
        //--------------------------------------------------------Events:---------------------------------------------------------------------\\
        #region --Events--
        private void CLIENT_NewMUCMemberPresenceMessage(XMPPClient client, XMPP_API.Classes.Network.Events.NewMUCMemberPresenceMessageEventArgs args)
        {
            string roomJId = Utils.getBareJidFromFullJid(args.mucMemberPresenceMessage.getFrom());
            if (!Equals(roomJId, CHAT.chatJabberId))
            {
                return;
            }

            switch (INFO.state)
            {
                case MUCState.ENTERING:
                    // Evaluate status codes:
                    foreach (MUCPresenceStatusCode statusCode in args.mucMemberPresenceMessage.STATUS_CODES)
                    {
                        switch (statusCode)
                        {
                            case MUCPresenceStatusCode.PRESENCE_SELFE_REFERENCE:
                                // Remove event subscription:
                                CLIENT.NewMUCMemberPresenceMessage -= CLIENT_NewMUCMemberPresenceMessage;

                                // Update MUC info:
                                INFO.state = MUCState.ENTERD;
                                INFO.affiliation = args.mucMemberPresenceMessage.AFFILIATION;
                                INFO.role = args.mucMemberPresenceMessage.ROLE;
                                MUCDBManager.INSTANCE.setMUCChatInfo(INFO, false, true);
                                Logger.Info("Entered MUC room '" + roomJId + "' as '" + INFO.nickname + "' with role '" + INFO.role + "' and affiliation '" + INFO.affiliation + '\'');
                                break;

                            default:
                                break;
                        }
                    }
                    break;

                default:
                    break;
            }
        }

        #endregion
    }
}
