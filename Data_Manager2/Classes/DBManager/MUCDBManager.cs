using Data_Manager2.Classes.Events;
using Data_Manager2.Classes.DBTables;
using System.Collections.Generic;

namespace Data_Manager2.Classes.DBManager
{
    public class MUCDBManager : AbstractDBManager
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        public static readonly MUCDBManager INSTANCE = new MUCDBManager();

        public delegate void MUCInfoChangedHandler(MUCDBManager handler, MUCInfoChangedEventArgs args);
        public delegate void MUCMemberChangedHandler(MUCDBManager handler, MUCMemberChangedEventArgs args);

        public event MUCInfoChangedHandler MUCInfoChanged;
        public event MUCMemberChangedHandler MUCMemberChanged;

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        /// <summary>
        /// Basic Constructor
        /// </summary>
        /// <history>
        /// 15/02/2018 Created [Fabian Sauter]
        /// </history>
        public MUCDBManager()
        {
            resetMUCJoinStates();
        }

        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--
        public MUCChatInfoTable getMUCInfo(string chatId)
        {
            IList<MUCChatInfoTable> list = dB.Query<MUCChatInfoTable>(true, "SELECT * FROM " + DBTableConsts.MUC_CHAT_INFO_TABLE + " WHERE chatId = ?;", chatId);
            if (list != null && list.Count > 0)
            {
                return list[0];
            }
            return null;
        }

        public void setMUCMember(MUCMemberTable member, bool delete, bool triggerMUCMemberChanged)
        {
            if (delete)
            {
                dB.Delete(member);

                if (triggerMUCMemberChanged)
                {
                    onMUCMemberChanged(member, delete);
                }
            }
            else
            {
                update(member);

                if (triggerMUCMemberChanged)
                {
                    onMUCMemberChanged(member.id, delete);
                }
            }
        }

        public void setMUCState(string chatId, MUCState state, bool triggerMUCChanged)
        {
            dB.Execute("UPDATE " + DBTableConsts.MUC_CHAT_INFO_TABLE + " SET state = ? WHERE chatId = ?", state, chatId);
            if (triggerMUCChanged)
            {
                onMUCInfoChanged(chatId);
            }
        }

        public List<MUCMemberTable> getAllMUCMembers(string chatId)
        {
            return dB.Query<MUCMemberTable>(true, "SELECT * FROM " + DBTableConsts.MUC_MEMBER_TABLE + " WHERE chatId = ?;", chatId);
        }

        public MUCMemberTable getMUCMember(string chatId, string nickname)
        {
            return getMUCMember(MUCMemberTable.generateId(chatId, nickname));
        }

        public MUCMemberTable getMUCMember(string id)
        {
            List<MUCMemberTable> list = dB.Query<MUCMemberTable>(true, "SELECT * FROM " + DBTableConsts.MUC_MEMBER_TABLE + " WHERE id = ?;", id);
            if (list.Count <= 0)
            {
                return null;
            }
            return list[0];
        }

        public void setMUCChatInfo(MUCChatInfoTable info, bool delete, bool triggerMUCChanged)
        {
            if (info != null)
            {
                if (delete)
                {
                    dB.Delete(info);
                }
                else
                {
                    update(info);
                }

                if (triggerMUCChanged)
                {
                    MUCInfoChanged?.Invoke(this, new MUCInfoChangedEventArgs(info, delete));
                }
            }
        }

        public void setMUCSubject(string chatId, string subject, bool triggerMUCChanged)
        {
            dB.Execute("UPDATE " + DBTableConsts.MUC_CHAT_INFO_TABLE + " SET subject = ? WHERE chatId = ?", subject, chatId);
            if (triggerMUCChanged)
            {
                onMUCInfoChanged(chatId);
            }
        }

        public void setMUCAutoEnter(string chatId, bool autoEnterRoom, bool triggerMUCChanged)
        {
            dB.Execute("UPDATE " + DBTableConsts.MUC_CHAT_INFO_TABLE + " SET autoEnterRoom = ? WHERE chatId = ?", autoEnterRoom, chatId);
            if (triggerMUCChanged)
            {
                onMUCInfoChanged(chatId);
            }
        }

        public void setNickname(string chatId, string nickname, bool triggerMUCChanged)
        {
            dB.Execute("UPDATE " + DBTableConsts.MUC_CHAT_INFO_TABLE + " SET nickname = ? WHERE chatId = ?;", nickname, chatId);
            if (triggerMUCChanged)
            {
                onMUCInfoChanged(chatId);
            }
        }

        #endregion
        //--------------------------------------------------------Misc Methods:---------------------------------------------------------------\\
        #region --Misc Methods (Public)--
        public void deleteAllMUCMembersforChat(string chatId)
        {
            dB.Execute("DELETE FROM " + DBTableConsts.MUC_MEMBER_TABLE + " WHERE chatId = ?;", chatId);
        }

        public void resetMUCState(string userAccountId, bool triggerMUCChanged)
        {
            // Semi join:
            List<MUCChatInfoTable> list = dB.Query<MUCChatInfoTable>(true, "SELECT * FROM " + DBTableConsts.MUC_CHAT_INFO_TABLE + " WHERE EXISTS (SELECT * FROM " + DBTableConsts.CHAT_TABLE + " c JOIN " + DBTableConsts.MUC_CHAT_INFO_TABLE + " i ON c.id = i.chatId WHERE c.userAccountId = ? AND chatType = ?) AND state != ?;", userAccountId, ChatType.MUC, MUCState.DISCONNECTED);
            foreach (MUCChatInfoTable info in list)
            {
                setMUCState(info.chatId, MUCState.DISCONNECTED, triggerMUCChanged);
            }
        }

        #endregion

        #region --Misc Methods (Private)--
        private void onMUCInfoChanged(string chatId)
        {
            MUCChatInfoTable info = getMUCInfo(chatId);
            if (info != null)
            {
                MUCInfoChanged?.Invoke(this, new MUCInfoChangedEventArgs(info, false));
            }
        }

        private void onMUCMemberChanged(MUCMemberTable member, bool delete)
        {
            if (member != null)
            {
                MUCMemberChanged?.Invoke(this, new MUCMemberChangedEventArgs(member, delete));
            }
        }

        private void onMUCMemberChanged(string id, bool delete)
        {
            onMUCMemberChanged(getMUCMember(id), delete);
        }

        private void resetMUCJoinStates()
        {
            dB.Execute("UPDATE " + DBTableConsts.MUC_CHAT_INFO_TABLE + " SET state = ?;", MUCState.DISCONNECTED);
        }

        #endregion

        #region --Misc Methods (Protected)--
        protected override void createTables()
        {
            dB.CreateTable<MUCChatInfoTable>();
            dB.CreateTable<MUCMemberTable>();
        }

        protected override void dropTables()
        {
            dB.DropTable<MUCChatInfoTable>();
            dB.DropTable<MUCMemberTable>();
        }

        #endregion
        //--------------------------------------------------------Events:---------------------------------------------------------------------\\
        #region --Events--


        #endregion
    }
}
