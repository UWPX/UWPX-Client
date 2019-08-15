using SQLite;
using System;
using XMPP_API.Classes.Network.XML.Messages.XEP_0384;

namespace Data_Manager2.Classes.DBTables.Omemo
{
    [Table(DBTableConsts.OMEMO_DEVICE_LIST_SUBSCRIPTION_TABLE)]
    public class OmemoDeviceListSubscriptionTable
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        [PrimaryKey]
        public string id { get; set; }
        [NotNull]
        public string accountId { get; set; }
        [NotNull]
        public string name { get; set; }
        public OmemoDeviceListSubscriptionState state { get; set; }
        public DateTime lastUpdateReceived { get; set; }

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        /// <summary>
        /// Basic Constructor
        /// </summary>
        /// <history>
        /// 09/11/2018 Created [Fabian Sauter]
        /// </history>
        public OmemoDeviceListSubscriptionTable()
        {
        }

        public OmemoDeviceListSubscriptionTable(string accountId, string name, OmemoDeviceListSubscriptionState state, DateTime lastUpdateReceived)
        {
            id = generateId(accountId, name);
            this.accountId = accountId;
            this.name = name;
            this.state = state;
            this.lastUpdateReceived = lastUpdateReceived;
        }

        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--


        #endregion
        //--------------------------------------------------------Misc Methods:---------------------------------------------------------------\\
        #region --Misc Methods (Public)--
        public static string generateId(string accountId, string name)
        {
            return accountId + '_' + name;
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
