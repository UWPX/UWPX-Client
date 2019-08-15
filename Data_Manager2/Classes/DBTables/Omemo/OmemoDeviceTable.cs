using libsignal;
using SQLite;

namespace Data_Manager2.Classes.DBTables.Omemo
{
    [Table(DBTableConsts.OMEMO_DEVICE_TABLE)]
    public class OmemoDeviceTable
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        [PrimaryKey]
        public string id { get; set; }
        [NotNull]
        public string accountId { get; set; }
        [NotNull]
        public string name { get; set; }
        public uint deviceId { get; set; }

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        /// <summary>
        /// Basic Constructor
        /// </summary>
        /// <history>
        /// 09/11/2018 Created [Fabian Sauter]
        /// </history>
        public OmemoDeviceTable()
        {
        }

        public OmemoDeviceTable(SignalProtocolAddress device, string accountId)
        {
            id = generateId(accountId, device.getName(), device.getDeviceId());
            this.accountId = accountId;
            name = device.getName();
            deviceId = device.getDeviceId();
        }

        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--


        #endregion
        //--------------------------------------------------------Misc Methods:---------------------------------------------------------------\\
        #region --Misc Methods (Public)--
        public static string generateId(string accountId, string name, uint deviceId)
        {
            return accountId + "_" + name + '_' + deviceId;
        }

        public SignalProtocolAddress toSignalProtocolAddress()
        {
            return new SignalProtocolAddress(name, deviceId);
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
