using Data_Manager2.Classes.DBTables;
using Data_Manager2.Classes.DBTables.Omemo;
using libsignal;
using System;
using System.Collections.Generic;
using Thread_Save_Components.Classes.SQLite;
using XMPP_API.Classes.Network.XML.Messages.XEP_0384;

namespace Data_Manager2.Classes.DBManager.Omemo
{
    public class OmemoDeviceDBManager : AbstractDBManager
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
        /// 09/11/2018 Created [Fabian Sauter]
        /// </history>
        public OmemoDeviceDBManager()
        {
        }

        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--
        public void setDevices(IList<SignalProtocolAddress> devices, string accountId)
        {
            if (devices.Count <= 0)
            {
                return;
            }

            dB.BeginTransaction();
            deleteDevices(devices[0].getName());
            foreach (SignalProtocolAddress d in devices)
            {
                dB.Insert(new OmemoDeviceTable(d, accountId));
            }
            dB.Commit();
        }

        public void setDevice(SignalProtocolAddress device, string accountId)
        {
            dB.InsertOrReplace(new OmemoDeviceTable(device, accountId));
        }

        public IList<SignalProtocolAddress> getDevices(string name, string accountId)
        {
            IList<SignalProtocolAddress> devices = new List<SignalProtocolAddress>();
            IList<OmemoDeviceTable> list = dB.Query<OmemoDeviceTable>(true, "SELECT * FROM " + DBTableConsts.OMEMO_DEVICE_TABLE + " WHERE name = ? AND accountId = ?;", name, accountId);
            foreach (OmemoDeviceTable d in list)
            {
                devices.Add(d.toSignalProtocolAddress());
            }
            return devices;
        }

        public Tuple<OmemoDeviceListSubscriptionState, DateTime> getOmemoDeviceListSubscription(string name, string accountId)
        {
            List<OmemoDeviceListSubscriptionTable> subscriptions = dB.Query<OmemoDeviceListSubscriptionTable>(true, "SELECT * FROM " + DBTableConsts.OMEMO_DEVICE_LIST_SUBSCRIPTION_TABLE + " WHERE id = ?;", OmemoDeviceListSubscriptionTable.generateId(accountId, name));
            if (subscriptions == null || subscriptions.Count <= 0)
            {
                return new Tuple<OmemoDeviceListSubscriptionState, DateTime>(OmemoDeviceListSubscriptionState.NONE, DateTime.MinValue);
            }
            return new Tuple<OmemoDeviceListSubscriptionState, DateTime>(subscriptions[0].state, subscriptions[0].lastUpdateReceived);
        }

        public void setOmemoDeviceListSubscription(string name, Tuple<OmemoDeviceListSubscriptionState, DateTime> lastUpdate, string accountId)
        {
            dB.InsertOrReplace(new OmemoDeviceListSubscriptionTable(accountId, name, lastUpdate.Item1, lastUpdate.Item2));
        }

        #endregion
        //--------------------------------------------------------Misc Methods:---------------------------------------------------------------\\
        #region --Misc Methods (Public)--
        public void deleteDevices(string name)
        {
            dB.Execute("DELETE FROM " + DBTableConsts.OMEMO_DEVICE_TABLE + " WHERE name = ?;", name);
        }

        public void deleteDeviceListSubscriptions(string name)
        {
            dB.Execute("DELETE FROM " + DBTableConsts.OMEMO_DEVICE_LIST_SUBSCRIPTION_TABLE + " WHERE name = ?;", name);
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
