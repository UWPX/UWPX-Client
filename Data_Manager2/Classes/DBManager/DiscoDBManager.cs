using Data_Manager2.Classes.DBTables;
using Shared.Classes.SQLite;
using System;
using System.Collections.Generic;
using XMPP_API.Classes;
using XMPP_API.Classes.Network.XML.Messages;
using XMPP_API.Classes.Network.XML.Messages.XEP_0030;

namespace Data_Manager2.Classes.DBManager
{
    public class DiscoDBManager : AbstractDBManager
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        public static readonly DiscoDBManager INSTANCE = new DiscoDBManager();
        private readonly List<MessageResponseHelper<IQMessage>> RESPONSE_HELPERS;

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        /// <summary>
        /// Basic Constructor
        /// </summary>
        /// <history>
        /// 03/01/2018 Created [Fabian Sauter]
        /// </history>
        public DiscoDBManager()
        {
            this.RESPONSE_HELPERS = new List<MessageResponseHelper<IQMessage>>();
            ConnectionHandler.INSTANCE.ClientConnected += INSTANCE_ClientConnected;
        }

        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--
        public List<DiscoFeatureTable> getAllMUCServers()
        {
            return dB.Query<DiscoFeatureTable>(true, "SELECT * FROM " + DBTableConsts.DISCO_FEATURE_TABLE + " WHERE var = 'http://jabber.org/protocol/muc';");
        }

        #endregion
        //--------------------------------------------------------Misc Methods:---------------------------------------------------------------\\
        #region --Misc Methods (Public)--


        #endregion

        #region --Misc Methods (Private)--
        private void addIdentities(List<DiscoIdentity> identities, string from)
        {
            if (identities != null)
            {
                dB.Execute("DELETE FROM " + DBTableConsts.DISCO_IDENTITY_TABLE + " WHERE fromServer = ?;", from);
                foreach (DiscoIdentity i in identities)
                {
                    if (from != null && i.TYPE != null && i.CATEGORY != null)
                    {
                        dB.InsertOrReplace(new DiscoIdentityTable()
                        {
                            id = DiscoIdentityTable.generateId(from, i.TYPE),
                            fromServer = from,
                            name = i.NAME,
                            category = i.CATEGORY,
                            type = i.TYPE
                        });
                    }
                }
            }
        }

        private void addFeatures(List<DiscoFeature> features, string from)
        {
            if (features != null)
            {
                dB.Execute("DELETE FROM " + DBTableConsts.DISCO_FEATURE_TABLE + " WHERE fromServer = ?;", from);
                foreach (DiscoFeature f in features)
                {
                    if (from != null && f.VAR != null)
                    {
                        dB.InsertOrReplace(new DiscoFeatureTable
                        {
                            id = DiscoFeatureTable.generateId(from, f.VAR),
                            fromServer = from,
                            var = f.VAR
                        });
                    }
                }
            }
        }

        private void addItems(List<DiscoItem> items, string from, XMPPClient client, bool requestInfo)
        {
            if (items != null)
            {
                dB.Execute("DELETE FROM " + DBTableConsts.DISCO_ITEM_TABLE + " WHERE fromServer = ?;", from);
                foreach (DiscoItem i in items)
                {
                    if (from != null && i.JID != null)
                    {
                        dB.InsertOrReplace(new DiscoItemTable()
                        {
                            id = DiscoItemTable.generateId(from, i.JID),
                            fromServer = from,
                            jid = i.JID,
                            name = i.NAME
                        });
                        if (requestInfo)
                        {
                            MessageResponseHelper<IQMessage> helper = client.GENERAL_COMMAND_HELPER.createDisco(i.JID, DiscoType.INFO, onDiscoMsg, onDiscoTimeout);
                            RESPONSE_HELPERS.Add(helper);
                        }
                    }
                }
            }
        }

        private void onDiscoTimeout(MessageResponseHelper<IQMessage> helper)
        {
            RESPONSE_HELPERS.Remove(helper);
            Logging.Logger.Debug("[" + nameof(DiscoDBManager) + "] timeout for: " + helper.sendId);
        }

        private bool onDiscoMsg(MessageResponseHelper<IQMessage> helper, IQMessage msg)
        {
            if (msg is DiscoResponseMessage response && helper.getMessageSender() is XMPPClient client)
            {
                switch (response.DISCO_TYPE)
                {
                    case DiscoType.ITEMS:
                        addItems(response.ITEMS, response.getFrom(), client, true);
                        break;

                    case DiscoType.INFO:
                        addFeatures(response.FEATURES, response.getFrom());
                        addIdentities(response.IDENTITIES, response.getFrom());
                        break;

                    default:
                        throw new InvalidOperationException("[" + nameof(DiscoDBManager) + "] Unexpected value for DISCO_TYPE: " + response.DISCO_TYPE);
                }
            }
            return false;
        }

        #endregion

        #region --Misc Methods (Protected)--
        protected override void CreateTables()
        {
            dB.CreateTable<DiscoFeatureTable>();
            dB.CreateTable<DiscoIdentityTable>();
            dB.CreateTable<DiscoItemTable>();
        }

        protected override void DropTables()
        {
            dB.DropTable<DiscoFeatureTable>();
            dB.DropTable<DiscoIdentityTable>();
            dB.DropTable<DiscoItemTable>();
        }

        #endregion
        //--------------------------------------------------------Events:---------------------------------------------------------------------\\
        #region --Events--
        private void INSTANCE_ClientConnected(ConnectionHandler handler, Events.ClientConnectedEventArgs args)
        {
            //messageIdCache.addTimed(await args.CLIENT.createDiscoAsync(args.CLIENT.getXMPPAccount().user.domain, DiscoType.ITEMS));
            //messageIdCache.addTimed(await args.CLIENT.createDiscoAsync(args.CLIENT.getXMPPAccount().user.domain, DiscoType.INFO));
        }
        #endregion
    }
}
