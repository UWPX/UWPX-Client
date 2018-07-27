using Data_Manager2.Classes.Events;
using Data_Manager2.Classes.DBTables;
using System.Collections.Generic;
using XMPP_API.Classes.Network.XML.Messages.XEP_0048;

namespace Data_Manager2.Classes.DBManager
{
    public class MUCDBManager : AbstractDBManager
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        public static readonly MUCDBManager INSTANCE = new MUCDBManager();

        public delegate void MUCInfoChangedHandler(MUCDBManager handler, MUCInfoChangedEventArgs args);
        public delegate void MUCOccupantChangedHandler(MUCDBManager handler, MUCOccupantChangedEventArgs args);

        public event MUCInfoChangedHandler MUCInfoChanged;
        public event MUCOccupantChangedHandler MUCOccupantChanged;

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
        public void setMUCChatInfoTableValue(string id, object IdValue, string name, object value)
        {
            dB.Execute("UPDATE " + DBTableConsts.MUC_CHAT_INFO_TABLE + " SET " + name + "= ? WHERE " + id + "= ?", value, IdValue);
        }

        public MUCChatInfoTable getMUCInfo(string chatId)
        {
            IList<MUCChatInfoTable> list = dB.Query<MUCChatInfoTable>(true, "SELECT * FROM " + DBTableConsts.MUC_CHAT_INFO_TABLE + " WHERE chatId = ?;", chatId);
            if (list != null && list.Count > 0)
            {
                return list[0];
            }
            return null;
        }

        public List<ConferenceItem> getXEP0048ConferenceItemsForAccount(string userAccountId)
        {
            IList<MUCChatInfoTable> list = dB.Query<MUCChatInfoTable>(true, "SELECT i.*, c.chatJabberId AS chatId FROM " + DBTableConsts.MUC_CHAT_INFO_TABLE + " i JOIN " + DBTableConsts.CHAT_TABLE + " c ON i.chatId = c.id WHERE c.inRoster = ? AND c.userAccountId = ?;", true, userAccountId);
            List<ConferenceItem> conferences = new List<ConferenceItem>();
            foreach (var mucInfo in list)
            {
                conferences.Add(mucInfo.toConferenceItem());
            }
            return conferences;
        }

        public void setMUCOccupant(MUCOccupantTable occupant, bool delete, bool triggerMUCOccupantChanged)
        {
            if (delete)
            {
                dB.Delete(occupant);

                if (triggerMUCOccupantChanged)
                {
                    onMUCOccupantChanged(occupant, delete);
                }
            }
            else
            {
                update(occupant);

                if (triggerMUCOccupantChanged)
                {
                    onMUCOccupantChanged(occupant.id, delete);
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

        public List<MUCOccupantTable> getAllMUCMembers(string chatId)
        {
            return dB.Query<MUCOccupantTable>(true, "SELECT * FROM " + DBTableConsts.MUC_OCCUPANT_TABLE + " WHERE chatId = ?;", chatId);
        }

        public MUCOccupantTable getMUCOccupant(string chatId, string nickname)
        {
            return getMUCOccupant(MUCOccupantTable.generateId(chatId, nickname));
        }

        public MUCOccupantTable getMUCOccupant(string id)
        {
            List<MUCOccupantTable> list = dB.Query<MUCOccupantTable>(true, "SELECT * FROM " + DBTableConsts.MUC_OCCUPANT_TABLE + " WHERE id = ?;", id);
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

        public void setMUCInfoNickname(string chatId, string nickname, bool triggerMUCChanged)
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
        public void deleteAllOccupantsforChat(string chatId)
        {
            dB.Execute("DELETE FROM " + DBTableConsts.MUC_OCCUPANT_TABLE + " WHERE chatId = ?;", chatId);
        }

        public void resetMUCState(string userAccountId, bool triggerMUCChanged)
        {
            // Semi join:
            List<MUCChatInfoTable> list = dB.Query<MUCChatInfoTable>(true, "SELECT * FROM " + DBTableConsts.MUC_CHAT_INFO_TABLE + " WHERE chatId IN (SELECT i.chatId FROM " + DBTableConsts.CHAT_TABLE + " c JOIN " + DBTableConsts.MUC_CHAT_INFO_TABLE + " i ON c.id = i.chatId WHERE c.userAccountId = ? AND chatType = ?) AND state != ?;", userAccountId, ChatType.MUC, MUCState.DISCONNECTED);
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

        private void onMUCOccupantChanged(MUCOccupantTable occupants, bool delete)
        {
            if (occupants != null)
            {
                MUCOccupantChanged?.Invoke(this, new MUCOccupantChangedEventArgs(occupants, delete));
            }
        }

        private void onMUCOccupantChanged(string id, bool delete)
        {
            onMUCOccupantChanged(getMUCOccupant(id), delete);
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
            dB.CreateTable<MUCOccupantTable>();
        }

        protected override void dropTables()
        {
            dB.DropTable<MUCChatInfoTable>();
            dB.DropTable<MUCOccupantTable>();
        }

        #endregion
        //--------------------------------------------------------Events:---------------------------------------------------------------------\\
        #region --Events--


        #endregion
    }
}
