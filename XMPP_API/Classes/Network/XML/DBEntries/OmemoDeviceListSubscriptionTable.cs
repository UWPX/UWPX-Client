using SQLite;
using System;

namespace XMPP_API.Classes.Network.XML.DBEntries
{
    [Table(DBTableConsts.OMEMO_DEVICE_LIST_SUBSCRIPTION_TABLE)]
    class OmemoDeviceListSubscriptionTable
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        [PrimaryKey]
        public string id { get; set; }
        [NotNull]
        public string chatJid { get; set; }
        [NotNull]
        public string accountId { get; set; }
        public OmemoDeviceListSubscriptionState state { get; set; }
        public DateTime lastUpdateReceived { get; set; }

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        /// <summary>
        /// Basic Constructor
        /// </summary>
        /// <history>
        /// 18/08/2018 Created [Fabian Sauter]
        /// </history>
        public OmemoDeviceListSubscriptionTable()
        {
        }

        public OmemoDeviceListSubscriptionTable(string chatJid, string accountId, OmemoDeviceListSubscriptionState state, DateTime lastUpdateReceived)
        {
            this.id = generateId(chatJid, accountId);
            this.chatJid = chatJid;
            this.accountId = accountId;
            this.state = state;
            this.lastUpdateReceived = lastUpdateReceived;
        }

        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--


        #endregion
        //--------------------------------------------------------Misc Methods:---------------------------------------------------------------\\
        #region --Misc Methods (Public)--
        public static string generateId(string chatJid, string accountId)
        {
            return chatJid + '_' + accountId;
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
