using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Logging;
using Manager.Classes.Chat;
using Shared.Classes.Collections;
using Shared.Classes.Threading;
using Storage.Classes;
using Storage.Classes.Contexts;
using Storage.Classes.Models.Chat;
using XMPP_API.Classes;
using XMPP_API.Classes.Network.Events;
using XMPP_API.Classes.Network.XML.Messages;
using XMPP_API.Classes.Network.XML.Messages.XEP_0045;

namespace Manager.Classes
{
    public class MucHandler
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        public const string TYPE_CHAT_INFO = "chat_info";

        public static readonly MucHandler INSTANCE = new MucHandler();
        private readonly TSTimedList<MucJoinHelper> TIMED_LIST;
        private readonly TimeSpan JOIN_DELAY = TimeSpan.FromSeconds(5);
        private Dictionary<string, CancellationTokenSource> joinDelayToken = new Dictionary<string, CancellationTokenSource>();
        private readonly SemaphoreSlim OCCUPANT_CREATION_SEMA = new SemaphoreSlim(1);
        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        public MucHandler()
        {
            TIMED_LIST = new TSTimedList<MucJoinHelper>
            {
                itemTimeoutInMs = (int)TimeSpan.FromSeconds(20).TotalMilliseconds
            };
        }

        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--


        #endregion
        //--------------------------------------------------------Misc Methods:---------------------------------------------------------------\\
        #region --Misc Methods (Public)--
        public void OnClientConnected(XMPPClient client)
        {
            client.NewMUCMemberPresenceMessage -= OnNewMucMemberPresenceMessage;
            client.NewMUCMemberPresenceMessage += OnNewMucMemberPresenceMessage;
            client.NewValidMessage -= OnNewValidMessage;
            client.NewValidMessage += OnNewValidMessage;

            ResetMucState(client.getXMPPAccount().getBareJid());
            if (!Settings.GetSettingBoolean(SettingsConsts.DISABLE_AUTO_JOIN_MUC))
            {
                Logger.Info("Entering all MUC rooms for '" + client.getXMPPAccount().getBareJid() + '\'');
                EnterAllMucs(client);
            }
        }

        private void ResetMucState(string accountBareJid)
        {
            IEnumerable<MucInfoModel> mucs = DataCache.INSTANCE.GetMucs(accountBareJid);
            using (MainDbContext ctx = new MainDbContext())
            {
                foreach (MucInfoModel muc in mucs)
                {
                    if (muc.state != MucState.DISCONNECTED)
                    {
                        muc.state = MucState.DISCONNECTED;
                        ctx.Update(muc);
                    }
                }
            }
        }

        public void OnClientDisconnected(XMPPClient client) { }

        public void OnClientDisconnecting(XMPPClient client)
        {
            client.NewMUCMemberPresenceMessage -= OnNewMucMemberPresenceMessage;
            client.NewValidMessage -= OnNewValidMessage;

            // Cancel the join delay:
            string accountBareJid = client.getXMPPAccount().getBareJid();
            if (joinDelayToken.TryGetValue(accountBareJid, out CancellationTokenSource token) && !(token is null) && !token.IsCancellationRequested)
            {
                token.Cancel();
                joinDelayToken.Remove(accountBareJid);
            }

            // Set all MUCs to disconnected:
            ResetMucState(accountBareJid);
        }

        public void OnMUCRoomSubjectMessage(MUCRoomSubjectMessage mucRoomSubject)
        {
            string to = Utils.getBareJidFromFullJid(mucRoomSubject.getTo());
            string from = Utils.getBareJidFromFullJid(mucRoomSubject.getFrom());
            ChatModel chat;
            using (SemaLock semaLock = DataCache.INSTANCE.NewChatSemaLock())
            {
                chat = DataCache.INSTANCE.GetChat(to, from, semaLock);
            }
            if (chat is null || chat.muc is null || (chat.muc.state == MucState.DISCONNECTED && string.Equals(chat.muc.subject, mucRoomSubject.SUBJECT)))
            {
                return;
            }
            chat.muc.state = MucState.DISCONNECTED;
            chat.muc.subject = mucRoomSubject.SUBJECT;
            using (MainDbContext ctx = new MainDbContext())
            {
                ctx.Update(chat.muc);
            }
        }

        public async Task EnterMucAsync(XMPPClient client, MucInfoModel info)
        {
            MucJoinHelper helper = new MucJoinHelper(client, info);
            TIMED_LIST.AddTimed(helper);

            await helper.EnterRoomAsync();
        }

        public async Task LeaveRoomAsync(XMPPClient client, MucInfoModel info)
        {
            StopMucJoinHelper(info.chat);
            info.state = MucState.DISCONNECTING;
            using (MainDbContext ctx = new MainDbContext())
            {
                ctx.Update(info);
            }
            await SendMucLeaveMessageAsync(client, info);
            info.state = MucState.DISCONNECTED;
            using (MainDbContext ctx = new MainDbContext())
            {
                ctx.Update(info);
            }
        }

        #endregion

        #region --Misc Methods (Private)--
        private void StopMucJoinHelper(ChatModel chat)
        {
            foreach (MucJoinHelper h in TIMED_LIST.GetEntries())
            {
                if (string.Equals(h.INFO.chat.bareJid, chat.bareJid))
                {
                    h.Dispose();
                }
            }
        }

        private async Task SendMucLeaveMessageAsync(XMPPClient client, MucInfoModel info)
        {
            string from = client.getXMPPAccount().getFullJid();
            string to = info.chat.bareJid + '/' + info.nickname;
            LeaveRoomMessage msg = new LeaveRoomMessage(from, to);
            await client.SendAsync(msg);
        }

        private async Task<bool> DelayAsync(string bareJid)
        {
            CancellationTokenSource token;
            try
            {
                if (joinDelayToken.TryGetValue(bareJid, out token) && !(token is null)) // Sometimes throws a NullReferenceException
                {
                    if (!token.IsCancellationRequested)
                    {
                        token.Cancel();
                    }
                    token.Dispose();
                }
            }
            catch (NullReferenceException) { }
            token = new CancellationTokenSource();
            joinDelayToken[bareJid] = token;

            Logger.Info("Delaying MUC joining for " + JOIN_DELAY.TotalSeconds + " seconds.");
            try
            {
                await Task.Delay(JOIN_DELAY, token.Token);
                Logger.Info("MUC join delay elapsed. Joining MUCs...");
                return true;
            }
            catch (TaskCanceledException) { }
            catch (ObjectDisposedException) { }
            Logger.Info("MUC joining has been canceled.");
            return false;
        }

        private void EnterAllMucs(XMPPClient client)
        {
            Task.Run(async () =>
            {
                // Delay joining MUCs for a couple of seconds to prevent a message overload:
                if (!await DelayAsync(client.getXMPPAccount().getBareJid()))
                {
                    // Delay has been canceled:
                    return;
                }

                IEnumerable<MucInfoModel> mucs = DataCache.INSTANCE.GetMucs(client.getXMPPAccount().getBareJid());
                foreach (MucInfoModel muc in mucs)
                {
                    await EnterMucAsync(client, muc);
                }
            });
        }

        private async Task OnMucErrorMessageAsync(XMPPClient client, MUCErrorMessage errorMessage)
        {
            string room = Utils.getBareJidFromFullJid(errorMessage.getFrom());
            if (room is null)
            {
                return;
            }
            ChatModel chat;
            using (SemaLock semaLock = DataCache.INSTANCE.NewChatSemaLock())
            {
                chat = DataCache.INSTANCE.GetChat(client.getXMPPAccount().getBareJid(), room, semaLock);
            }
            if (chat is null || chat.muc is null)
            {
                return;
            }
            Logger.Error("Received an error message for MUC: " + chat.bareJid + "\n" + errorMessage.ERROR_MESSAGE);
            StopMucJoinHelper(chat);
            if (chat.muc.state != MucState.DISCONNECTED)
            {
                await SendMucLeaveMessageAsync(client, chat.muc);
            }
            switch (errorMessage.ERROR_CODE)
            {
                // No access - user got baned:
                case 403:
                    chat.muc.state = MucState.BANED;
                    AddChatInfoMessage(chat, room, "Unable to join room!\nYou are baned from this room.");
                    using (MainDbContext ctx = new MainDbContext())
                    {
                        ctx.Update(chat.muc);
                    }
                    return;

                default:
                    chat.muc.state = MucState.ERROR;
                    break;
            }

            // Add an error chat message:
            ChatMessageModel msg = new ChatMessageModel()
            {
                stableId = errorMessage.ID ?? AbstractMessage.getRandomId(),
                chatId = chat.id,
                date = DateTime.Now,
                fromBareJid = Utils.getBareJidFromFullJid(errorMessage.getFrom()),
                isImage = false,
                message = "Code: " + errorMessage.ERROR_CODE + "\nType: " + errorMessage.ERROR_TYPE + "\nMessage:\n" + errorMessage.ERROR_MESSAGE,
                state = MessageState.UNREAD,
                type = MessageMessage.TYPE_ERROR
            };
            DataCache.INSTANCE.AddChatMessage(msg, chat);
        }

        private void AddOccupantKickedMessage(ChatModel chat, string roomJid, string nickname)
        {
            string msg = nickname + " got kicked from the room.";
            AddChatInfoMessage(chat, roomJid, msg);
        }

        private void AddOccupantBanedMessage(ChatModel chat, string roomJid, string nickname)
        {
            string msg = nickname + " got baned from the room.";
            AddChatInfoMessage(chat, roomJid, msg);
        }

        private void AddChatInfoMessage(ChatModel chat, string fromBareJid, string message)
        {
            ChatMessageModel msg = new ChatMessageModel
            {
                chatId = chat.id,
                fromBareJid = fromBareJid,
                fromNickname = fromBareJid,
                date = DateTime.Now,
                isImage = false,
                message = message,
                state = MessageState.UNREAD,
                type = TYPE_CHAT_INFO,
                stableId = AbstractMessage.getRandomId()
            };
            DataCache.INSTANCE.AddChatMessage(msg, chat);
        }

        #endregion

        #region --Misc Methods (Protected)--


        #endregion
        //--------------------------------------------------------Events:---------------------------------------------------------------------\\
        #region --Events--
        private void OnNewMucMemberPresenceMessage(XMPPClient client, NewMUCMemberPresenceMessageEventArgs args)
        {
            Task.Run(async () =>
            {
                MUCMemberPresenceMessage msg = args.mucMemberPresenceMessage;
                string roomJid = Utils.getBareJidFromFullJid(msg.getFrom());
                if (roomJid is null)
                {
                    return;
                }
                SemaLock semaLock = DataCache.INSTANCE.NewChatSemaLock();
                ChatModel chat = DataCache.INSTANCE.GetChat(client.getXMPPAccount().getBareJid(), roomJid, semaLock);
                semaLock.Dispose();
                if (chat is null || chat.muc is null)
                {
                    return;
                }

                // Prevent multiple accesses to the same occupant at the same time:
                await OCCUPANT_CREATION_SEMA.WaitAsync();
                MucOccupantModel occupant = chat.muc.occupants.Where(o => string.Equals(o.nickname, msg.FROM_NICKNAME)).FirstOrDefault();
                bool newOccupant = occupant is null;
                if (newOccupant)
                {
                    occupant = new MucOccupantModel()
                    {
                        nickname = msg.FROM_NICKNAME
                    };
                }
                occupant.affiliation = msg.AFFILIATION;
                occupant.role = msg.ROLE;
                occupant.fullJid = msg.JID;

                bool isUnavailable = Equals(msg.TYPE, "unavailable");
                bool nicknameChanged = false;
                if (isUnavailable)
                {
                    if (msg.STATUS_CODES.Contains(MUCPresenceStatusCode.PRESENCE_SELFE_REFERENCE))
                    {
                        // Nickname got changed by user or room:
                        if (msg.STATUS_CODES.Contains(MUCPresenceStatusCode.MEMBER_NICK_CHANGED) || msg.STATUS_CODES.Contains(MUCPresenceStatusCode.ROOM_NICK_CHANGED))
                        {
                            nicknameChanged = true;

                            // Update MUC info:
                            chat.muc.nickname = msg.NICKNAME;

                            // Update the user nickname:
                            occupant.nickname = msg.NICKNAME;
                        }
                        // Occupant got kicked:
                        else if (msg.STATUS_CODES.Contains(MUCPresenceStatusCode.MEMBER_GOT_KICKED))
                        {
                            chat.muc.state = MucState.KICKED;
                        }
                        else if (msg.STATUS_CODES.Contains(MUCPresenceStatusCode.MEMBER_GOT_BANED))
                        {
                            chat.muc.state = MucState.BANED;
                        }
                        else
                        {
                            chat.muc.state = MucState.DISCONNECTED;
                        }
                    }


                    if (msg.STATUS_CODES.Contains(MUCPresenceStatusCode.MEMBER_GOT_KICKED))
                    {
                        // Add kicked chat message:
                        AddOccupantKickedMessage(chat, roomJid, occupant.nickname);
                    }

                    if (msg.STATUS_CODES.Contains(MUCPresenceStatusCode.MEMBER_GOT_BANED))
                    {
                        // Add baned chat message:
                        AddOccupantBanedMessage(chat, roomJid, occupant.nickname);
                    }
                }

                // If the type equals 'unavailable', a user left the room:
                using (MainDbContext ctx = new MainDbContext())
                {
                    if (isUnavailable && !nicknameChanged)
                    {
                        if (!newOccupant)
                        {
                            chat.muc.occupants.Remove(occupant);
                            ctx.Remove(occupant);
                            ctx.Update(chat.muc);
                        }
                    }
                    else
                    {
                        if (newOccupant)
                        {
                            chat.muc.occupants.Add(occupant);
                            ctx.Update(chat.muc);
                            // ctx.Add(occupant);
                        }
                        else
                        {
                            ctx.Update(occupant);
                        }
                    }
                }

                if (newOccupant)
                {
                    using (MainDbContext ctx = new MainDbContext())
                    {
                        ctx.Update(chat.muc);
                    }
                }
                OCCUPANT_CREATION_SEMA.Release();
            });
        }

        private async void OnNewValidMessage(IMessageSender sender, NewValidMessageEventArgs args)
        {
            if (args.MESSAGE is MUCErrorMessage)
            {
                await OnMucErrorMessageAsync((XMPPClient)sender, args.MESSAGE as MUCErrorMessage);
            }
        }

        #endregion
    }
}
