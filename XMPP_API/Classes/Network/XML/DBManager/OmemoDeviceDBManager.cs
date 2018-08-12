using System.Collections.Generic;
using Thread_Save_Components.Classes.SQLite;
using XMPP_API.Classes.Network.XML.DBEntries;
using XMPP_API.Classes.Network.XML.Messages.XEP_0384;

namespace XMPP_API.Classes.Network.XML.DBManager
{
    class OmemoDeviceDBManager : AbstractDBManager
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        public static readonly OmemoDeviceDBManager INSTANCE = new OmemoDeviceDBManager();

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        /// <summary>
        /// Basic Constructor
        /// </summary>
        /// <history>
        /// 12/08/2018 Created [Fabian Sauter]
        /// </history>
        public OmemoDeviceDBManager()
        {
        }

        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--
        public void setDevices(List<OmemoDeviceTable> devices, string chatJid)
        {
            dB.BeginTransaction();
            deleteDevicesForChat(chatJid);
            foreach (OmemoDeviceTable device in devices)
            {
                dB.InsertOrReplace(device);
            }
            dB.Commit();
        }

        public void setDevices(OmemoDevices devices, string chatJid, string acountJid)
        {
            List<OmemoDeviceTable> deviceTables = new List<OmemoDeviceTable>();
            foreach (uint deviceId in devices.DEVICES)
            {
                deviceTables.Add(new OmemoDeviceTable()
                {
                    id = OmemoDeviceTable.generateId(chatJid, acountJid, deviceId),
                    accountJid = acountJid,
                    chatJid = chatJid,
                    deviceId = deviceId
                });
            }
            setDevices(deviceTables, chatJid);
        }

        public List<OmemoDeviceTable> getDevices(string chatJid, string accountJid)
        {
            return dB.Query<OmemoDeviceTable>(true, "SELECT * FROM " + DBTableConsts.OMEMO_DEVICE_TABLE + " WHERE chatJid = ? AND accountJid = ?;", chatJid, accountJid);
        }

        public List<uint> getDeviceIds(string chatJid, string accountJid)
        {
            List<OmemoDeviceTable> devices = getDevices(chatJid, accountJid);
            List<uint> deviceIds = new List<uint>();
            foreach (OmemoDeviceTable device in devices)
            {
                deviceIds.Add(device.deviceId);
            }
            return deviceIds;
        }

        #endregion
        //--------------------------------------------------------Misc Methods:---------------------------------------------------------------\\
        #region --Misc Methods (Public)--
        public void deleteDevicesForChat(string chatJid)
        {
            dB.Query<OmemoDeviceTable>(true, "DELETE FROM " + DBTableConsts.OMEMO_DEVICE_TABLE + " WHERE chatJid = ?;", chatJid);
        }

        public void deleteDevicesForAccount(string accountId)
        {
            dB.Query<OmemoDeviceTable>(true, "DELETE FROM " + DBTableConsts.OMEMO_DEVICE_TABLE + " WHERE accountId = ?;", accountId);
        }

        #endregion

        #region --Misc Methods (Private)--


        #endregion

        #region --Misc Methods (Protected)--
        protected override void createTables()
        {
            dB.CreateTable<OmemoDeviceTable>();
        }

        protected override void dropTables()
        {
            dB.DropTable<OmemoDeviceTable>();
        }

        #endregion
        //--------------------------------------------------------Events:---------------------------------------------------------------------\\
        #region --Events--


        #endregion
    }
}
