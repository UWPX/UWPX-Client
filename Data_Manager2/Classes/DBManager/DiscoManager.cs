using Data_Manager2.Classes.DBTables;
using System.Collections.Generic;
using System.Threading.Tasks;
using XMPP_API.Classes;
using XMPP_API.Classes.Network;
using XMPP_API.Classes.Network.XML.Messages.XEP_0030;

namespace Data_Manager2.Classes.DBManager
{
    public class DiscoManager : AbstractManager
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        public static readonly DiscoManager INSTANCE = new DiscoManager();
        private TimedList<string> messageIdCache;

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        /// <summary>
        /// Basic Constructor
        /// </summary>
        /// <history>
        /// 03/01/2018 Created [Fabian Sauter]
        /// </history>
        public DiscoManager()
        {
            this.messageIdCache = new TimedList<string>();
            ConnectionHandler.INSTANCE.ClientConnected += INSTANCE_ClientConnected;
        }

        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--
        public List<DiscoFeatureTable> getAllMUCServers()
        {
            return dB.Query<DiscoFeatureTable>("SELECT * FROM DiscoFeatureTable WHERE var = 'http://jabber.org/protocol/muc';");
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
                dB.Execute("DELETE FROM DiscoIdentityTable WHERE fromServer = ?;", from);
                foreach (DiscoIdentity i in identities)
                {
                    if (from != null && i.TYPE != null && i.CATEGORY != null)
                    {
                        update(new DiscoIdentityTable()
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
                dB.Execute("DELETE FROM DiscoFeatureTable WHERE fromServer = ?;", from);
                foreach (DiscoFeature f in features)
                {
                    if (from != null && f.VAR != null)
                    {
                        update(new DiscoFeatureTable
                        {
                            id = DiscoFeatureTable.generateId(from, f.VAR),
                            fromServer = from,
                            var = f.VAR
                        });
                    }
                }
            }
        }

        private async Task addItemsAsync(List<DiscoItem> items, string from, XMPPClient client, bool requestInfo)
        {
            if (items != null)
            {
                dB.Execute("DELETE FROM DiscoItemTable WHERE fromServer = ?;", from);
                foreach (DiscoItem i in items)
                {
                    if (from != null && i.JID != null)
                    {
                        update(new DiscoItemTable()
                        {
                            id = DiscoItemTable.generateId(from, i.JID),
                            fromServer = from,
                            jid = i.JID,
                            name = i.NAME
                        });
                        if (requestInfo)
                        {
                            messageIdCache.addTimed(await client.createDiscoAsync(i.JID, DiscoType.INFO));
                        }
                    }
                }
            }
        }

        #endregion

        #region --Misc Methods (Protected)--
        protected override void createTables()
        {
            dB.CreateTable<DiscoFeatureTable>();
            dB.CreateTable<DiscoIdentityTable>();
            dB.CreateTable<DiscoItemTable>();
        }

        protected override void dropTables()
        {
            dB.DropTable<DiscoFeatureTable>();
            dB.DropTable<DiscoIdentityTable>();
            dB.DropTable<DiscoItemTable>();
        }

        #endregion
        //--------------------------------------------------------Events:---------------------------------------------------------------------\\
        #region --Events--
        private async void INSTANCE_ClientConnected(ConnectionHandler handler, Data_Manager.Classes.Events.ClientConnectedEventArgs args)
        {
            args.CLIENT.NewDiscoResponseMessage -= CLIENT_NewDiscoResponseMessage;
            args.CLIENT.NewDiscoResponseMessage += CLIENT_NewDiscoResponseMessage;

            messageIdCache.addTimed(await args.CLIENT.createDiscoAsync(args.CLIENT.getXMPPAccount().user.domain, DiscoType.ITEMS));
            messageIdCache.addTimed(await args.CLIENT.createDiscoAsync(args.CLIENT.getXMPPAccount().user.domain, DiscoType.INFO));
        }

        private void CLIENT_NewDiscoResponseMessage(XMPPClient client, XMPP_API.Classes.Network.Events.NewDiscoResponseMessageEventArgs args)
        {
            Task.Factory.StartNew(async () =>
            {
                string from = args.DISCO.getFrom();
                // Only store direct server results:
                if(from != null && !from.Contains("@") && messageIdCache.getTimed(args.DISCO.getId()) != null)
                {
                    switch (args.DISCO.DISCO_TYPE)
                    {
                        case DiscoType.ITEMS:
                            await addItemsAsync(args.DISCO.ITEMS, from, client, true);
                            break;
                        case DiscoType.INFO:
                            addFeatures(args.DISCO.FEATURES, from);
                            addIdentities(args.DISCO.IDENTITIES, from);
                            break;
                    }
                }
            });
        }
        #endregion
    }
}
