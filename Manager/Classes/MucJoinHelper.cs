using System;
using System.Threading.Tasks;
using Logging;
using Shared.Classes.Collections;
using Shared.Classes.Threading;
using Storage.Classes.Contexts;
using Storage.Classes.Models.Chat;
using XMPP_API.Classes;
using XMPP_API.Classes.Network.Events;
using XMPP_API.Classes.Network.XML.Messages.Helper;
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
        public Task RequestReservedNicksAsync()
        {
            DiscoReservedRoomNicknamesMessages msg = new DiscoReservedRoomNicknamesMessages(CLIENT.getXMPPAccount().getFullJid(), INFO.chat.bareJid);
            return CLIENT.SendAsync(msg);
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
                    INFO.occupants.ForEach(o => o.Remove(ctx, true));
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
            CLIENT.NewMUCPresenceErrorMessage -= OnMucPresenceErrorMessage;
            CLIENT.NewMUCPresenceErrorMessage += OnMucPresenceErrorMessage;

            // Send message:
            await CLIENT.SendAsync(msg);
            Logger.Info($"Entering MUC room '{INFO.chat.bareJid}' as '{INFO.nickname }'...");
        }

        public bool CanGetRemoved()
        {
            return INFO.state != MucState.ENTERING;
        }

        public void Dispose() { /* In the further this should stop connecting. */ }

        #endregion

        #region --Misc Methods (Private)--
        private async Task<string> RequestMucIdentityAsync(string roomJId)
        {
            Logger.Debug($"Requesting the room name for '{roomJId}'...");
            MessageResponseHelperResult<ExtendedDiscoResponseMessage> result = await CLIENT.MUC_COMMAND_HELPER.requestRoomInfoAsync(roomJId);
            if (result.STATE == MessageResponseHelperResultState.SUCCESS)
            {
                if (result.RESULT.IDENTITIES.Count <= 0)
                {
                    Logger.Warn($"Querying the room name for '{roomJId}' failed: No identity provided.");
                    return null;
                }
                string name = result.RESULT.IDENTITIES[0].NAME;
                Logger.Debug($"Received '{name}' as room name for '{roomJId}'.");
                return name;
            }
            Logger.Warn($"Querying the room name for '{roomJId}' failed: {result.STATE}");
            return null;
        }

        #endregion

        #region --Misc Methods (Protected)--


        #endregion
        //--------------------------------------------------------Events:---------------------------------------------------------------------\\
        #region --Events--
        private async void OnMucMemberPresenceMessage(XMPPClient client, NewMUCMemberPresenceMessageEventArgs args)
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
                                CLIENT.NewMUCPresenceErrorMessage -= OnMucPresenceErrorMessage;

                                string name = await RequestMucIdentityAsync(roomJId);

                                bool updateBookmarks = false;
                                using (SemaLock semaLock = INFO.NewSemaLock())
                                {
                                    // Update MUC info:
                                    INFO.state = MucState.ENTERD;
                                    INFO.affiliation = args.mucMemberPresenceMessage.AFFILIATION;
                                    INFO.role = args.mucMemberPresenceMessage.ROLE;
                                    if (!(name is null))
                                    {
                                        if (!string.Equals(name, INFO.name))
                                        {
                                            Logger.Debug($"New MUC name ({name}) found for '{roomJId}'.");
                                            INFO.name = name;
                                            updateBookmarks = true;
                                        }
                                    }
                                    INFO.Update();
                                }

                                // Update bookmarks in case the name changed:
                                if (updateBookmarks)
                                {
                                    await ConnectionHandler.INSTANCE.GetClient(CLIENT.getXMPPAccount().getBareJid()).client.PublishBookmarksAsync();
                                }

                                Logger.Info($"Entered MUC room '{roomJId}' as '{INFO.nickname}' with role '{INFO.role}' and affiliation '{INFO.affiliation}'");
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

        private void OnMucPresenceErrorMessage(XMPPClient client, NewMUCPresenceErrorMessageEventArgs args)
        {
            // Remove event subscription:
            CLIENT.NewMUCMemberPresenceMessage -= OnMucMemberPresenceMessage;
            CLIENT.NewMUCPresenceErrorMessage -= OnMucPresenceErrorMessage;

            using (SemaLock semaLock = INFO.NewSemaLock())
            {
                // Update MUC info:
                INFO.state = MucState.ERROR;
                INFO.Update();
            }
            Logger.Error($"Failed to join '{INFO.chat.bareJid}' with: {args.mucPresenceErrorMessage.ERROR}");
        }

        #endregion
    }
}
