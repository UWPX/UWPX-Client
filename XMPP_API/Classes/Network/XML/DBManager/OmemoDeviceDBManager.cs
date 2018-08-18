using System;
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

        public void setDeviceListSubscription(OmemoDeviceListSubscriptionTable subscriptionTable)
        {
            dB.InsertOrReplace(subscriptionTable);
        }

        public List<OmemoDeviceTable> getDevices(string chatJid, string accountJid)
        {
            return dB.Query<OmemoDeviceTable>(true, "SELECT * FROM " + DBTableConsts.OMEMO_DEVICE_TABLE + " WHERE chatJid = ? AND accountJid = ?;", chatJid, accountJid);
        }

        public OmemoDevices getOmemoDevices(string chatJid, string accountJid)
        {
            List<OmemoDeviceTable> deviceTables = getDevices(chatJid, accountJid);
            OmemoDevices devices = new OmemoDevices();
            List<uint> deviceIds = new List<uint>();
            foreach (OmemoDeviceTable device in deviceTables)
            {
                devices.DEVICES.Add(device.deviceId);
            }
            return devices;
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

        public OmemoDeviceListSubscriptionTable getDeviceListSubscription(string chatJid, string accountJid)
        {
            List<OmemoDeviceListSubscriptionTable> subscriptions = dB.Query<OmemoDeviceListSubscriptionTable>(true, "SELECT * FROM " + DBTableConsts.OMEMO_DEVICE_LIST_SUBSCRIPTION_TABLE + " WHERE id = ?;", OmemoDeviceListSubscriptionTable.generateId(chatJid, accountJid));
            if (subscriptions == null || subscriptions.Count <= 0)
            {
                return new OmemoDeviceListSubscriptionTable(chatJid, accountJid, OmemoDeviceListSubscriptionState.NONE, DateTime.MinValue);
            }
            return subscriptions[0];
        }

        #endregion
        //--------------------------------------------------------Misc Methods:---------------------------------------------------------------\\
        #region --Misc Methods (Public)--
        public void deleteAllForChat(string chatJid)
        {
            deleteDevicesForChat(chatJid);
            dB.Execute("DELETE FROM " + DBTableConsts.OMEMO_DEVICE_LIST_SUBSCRIPTION_TABLE + " WHERE chatJid = ?;", chatJid);
        }

        public void deleteDevicesForChat(string chatJid)
        {
            dB.Execute("DELETE FROM " + DBTableConsts.OMEMO_DEVICE_TABLE + " WHERE chatJid = ?;", chatJid);
        }

        public void deleteAllForAccount(string accountId)
        {
            dB.Execute("DELETE FROM " + DBTableConsts.OMEMO_DEVICE_TABLE + " WHERE accountId = ?;", accountId);
            dB.Execute("DELETE FROM " + DBTableConsts.OMEMO_DEVICE_LIST_SUBSCRIPTION_TABLE + " WHERE accountId = ?;", accountId);
        }

        #endregion

        #region --Misc Methods (Private)--


        #endregion

        #region --Misc Methods (Protected)--
        protected override void createTables()
        {
            dB.CreateTable<OmemoDeviceTable>();
            dB.CreateTable<OmemoDeviceListSubscriptionTable>();
        }

        protected override void dropTables()
        {
            dB.DropTable<OmemoDeviceTable>();
            dB.DropTable<OmemoDeviceListSubscriptionTable>();
        }

        #endregion
        //--------------------------------------------------------Events:---------------------------------------------------------------------\\
        #region --Events--


        #endregion
    }
}
