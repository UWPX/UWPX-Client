using Data_Manager.Classes.Events;
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

        public event MUCInfoChangedHandler MUCInfoChanged;

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

        public void setMUCMember(MUCMemberTable member, bool delete)
        {
            if (member != null)
            {
                if (delete)
                {
                    dB.Delete(member);
                }
                else
                {
                    update(member);
                }
            }
        }

        public void setMUCState(string chatId, MUCState state, bool triggerMUCChanged)
        {
            dB.Execute("UPDATE " + DBTableConsts.MUC_CHAT_INFO_TABLE + " SET state = ? WHERE chatId = ?", state, chatId);
            if (triggerMUCChanged)
            {
                MUCChatInfoTable info = getMUCInfo(chatId);
                if (info != null)
                {
                    MUCInfoChanged?.Invoke(this, new MUCInfoChangedEventArgs(info, false));
                }
            }
        }

        public List<MUCMemberTable> getAllMUCMembers(string chatId)
        {
            return dB.Query<MUCMemberTable>(true, "SELECT * FROM " + DBTableConsts.MUC_MEMBER_TABLE + " WHERE chatId = ?;", chatId);
        }

        public MUCMemberTable getMUCMember(string chatId, string nickname)
        {
            List<MUCMemberTable> list = dB.Query<MUCMemberTable>(true, "SELECT * FROM " + DBTableConsts.MUC_MEMBER_TABLE + " WHERE id = ?;", MUCMemberTable.generateId(chatId, nickname));
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
