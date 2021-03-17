using System;
using System.Threading.Tasks;
using Logging;
using Shared.Classes.Collections;
using Shared.Classes.Threading;
using Storage.Classes.Contexts;
using Storage.Classes.Models.Chat;
using XMPP_API.Classes;
using XMPP_API.Classes.Network.Events;
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
        public readonly MucInfoModel INFO;
        public readonly XMPPClient CLIENT;

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        public MucJoinHelper(XMPPClient client, MucInfoModel info)
        {
            CLIENT = client;
            INFO = info;
        }

        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--


        #endregion
        //--------------------------------------------------------Misc Methods:---------------------------------------------------------------\\
        #region --Misc Methods (Public)--
        public async Task RequestReservedNicksAsync()
        {
            DiscoReservedRoomNicknamesMessages msg = new DiscoReservedRoomNicknamesMessages(CLIENT.getXMPPAccount().getFullJid(), INFO.chat.bareJid);
            await CLIENT.SendAsync(msg);
        }

        public async Task EnterRoomAsync()
        {
            using (SemaLock semaLock = INFO.NewSemaLock())
            {
                // Update MUC info:
                INFO.state = MucState.ENTERING;
                using (MainDbContext ctx = new MainDbContext())
                {
                    // Clear MUC members:
                    ctx.RemoveRange(INFO.occupants);
                    INFO.occupants.Clear();
                    INFO.OnOccupantsChanged();
                    ctx.Update(INFO);
                }
            }

            // Create message:
            JoinRoomRequestMessage msg = new JoinRoomRequestMessage(CLIENT.getXMPPAccount().getFullJid(), INFO.chat.bareJid, INFO.nickname, INFO.password);

            // Subscribe to events for receiving answers:
            CLIENT.NewMUCMemberPresenceMessage -= OnMucMemberPresenceMessage;
            CLIENT.NewMUCMemberPresenceMessage += OnMucMemberPresenceMessage;

            // Send message:
            await CLIENT.SendAsync(msg);
            Logger.Info("Entering MUC room '" + INFO.chat.bareJid + "' as '" + INFO.nickname + '\'');
        }

        public bool CanGetRemoved()
        {
            return INFO.state != MucState.ENTERING;
        }

        public void Dispose() { /* In the further this should stop connecting. */ }

        #endregion

        #region --Misc Methods (Private)--


        #endregion

        #region --Misc Methods (Protected)--


        #endregion
        //--------------------------------------------------------Events:---------------------------------------------------------------------\\
        #region --Events--
        private void OnMucMemberPresenceMessage(XMPPClient client, NewMUCMemberPresenceMessageEventArgs args)
        {
            string roomJId = Utils.getBareJidFromFullJid(args.mucMemberPresenceMessage.getFrom());
            if (!Equals(roomJId, INFO.chat.bareJid))
            {
                return;
            }

            switch (INFO.state)
            {
                case MucState.ENTERING:
                    // Evaluate status codes:
                    foreach (MUCPresenceStatusCode statusCode in args.mucMemberPresenceMessage.STATUS_CODES)
                    {
                        switch (statusCode)
                        {
                            case MUCPresenceStatusCode.PRESENCE_SELFE_REFERENCE:
                                // Remove event subscription:
                                CLIENT.NewMUCMemberPresenceMessage -= OnMucMemberPresenceMessage;

                                using (SemaLock semaLock = INFO.NewSemaLock())
                                {
                                    // Update MUC info:
                                    INFO.state = MucState.ENTERD;
                                    INFO.affiliation = args.mucMemberPresenceMessage.AFFILIATION;
                                    INFO.role = args.mucMemberPresenceMessage.ROLE;
                                    using (MainDbContext ctx = new MainDbContext())
                                    {
                                        ctx.Update(INFO);
                                    }
                                }
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
