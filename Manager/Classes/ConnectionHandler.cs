using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Shared.Classes.Collections;
using Shared.Classes.Network;
using Storage.Classes.Contexts;
using Storage.Classes.Events;
using Storage.Classes.Models.Account;

namespace Manager.Classes
{
    public class ConnectionHandler
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        private static readonly SemaphoreSlim CLIENT_SEMA = new SemaphoreSlim(1);
        public static readonly ConnectionHandler INSTANCE = new ConnectionHandler();
        private readonly CustomObservableCollection<ClientConnectionHandler> CLIENTS;
        private readonly DownloadHandler DOWNLOAD_HANDLER = new DownloadHandler();
        public readonly ImageDownloadHandler IMAGE_DOWNLOAD_HANDLER;

        public delegate void ClientConnectedHandler(ConnectionHandler handler, ClientConnectedEventArgs args);
        public delegate void ClientsCollectionChangedHandler(ConnectionHandler handler, NotifyCollectionChangedEventArgs args);

        public event ClientConnectedHandler ClientConnected;
        public event ClientsCollectionChangedHandler ClientsCollectionChanged;
        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        public ConnectionHandler()
        {
            IMAGE_DOWNLOAD_HANDLER = new ImageDownloadHandler(DOWNLOAD_HANDLER);
            Task.Run(async () => await IMAGE_DOWNLOAD_HANDLER.ContinueDownloadsAsync());
            CLIENTS = new CustomObservableCollection<ClientConnectionHandler>(false);
            CLIENTS.CollectionChanged += CLIENTS_CollectionChanged;
            LoadClients();
        }

        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--
        /// <summary>
        /// Returns the <see cref="Client"/> that matches the given bare JID.
        /// </summary>
        /// <param name="bareJid">The bare JID of the requested XMPPClient. e.g. 'alice@jabber.org'</param>
        public Client GetClient(string bareJid)
        {
            foreach (ClientConnectionHandler handler in CLIENTS)
            {
                if (handler.client.dbAccount.bareJid.Equals(bareJid))
                {
                    return handler.client;
                }
            }
            return null;
        }

        /// <summary>
        /// Returns the <see cref="ClientConnectedHandler"/> that matches the given bare JID.
        /// </summary>
        /// <param name="bareJid">The bare JID of the requested XMPPClient. e.g. 'alice@jabber.org'</param>
        public ClientConnectionHandler GetClientConnection(string bareJid)
        {
            foreach (ClientConnectionHandler handler in CLIENTS)
            {
                if (handler.client.dbAccount.bareJid.Equals(bareJid))
                {
                    return handler;
                }
            }
            return null;
        }

        /// <summary>
        /// Returns all available XMPPClients.
        /// The CustomObservableCollection won't invoke the UI thread if a change occurs.
        /// </summary>
        public CustomObservableCollection<ClientConnectionHandler> GetClients()
        {
            return CLIENTS;
        }

        #endregion
        //--------------------------------------------------------Misc Methods:---------------------------------------------------------------\\
        #region --Misc Methods (Public)--
        /// <summary>
        /// Starts connecting all XMPPClients.
        /// </summary>
        public void ConnectAll()
        {
            Parallel.ForEach(CLIENTS, (handler) =>
            {
                if (!handler.client.dbAccount.disabled)
                {
                    handler.Connect();
                }
            });
        }

        /// <summary>
        /// Disconnects all XMPPClients in parallel.
        /// </summary>
        public void DisconnectAll()
        {
            Parallel.ForEach(CLIENTS, async (handler) => await handler.DisconnectAsync());
        }

        /// <summary>
        /// Disconnects all XMPPClients.
        /// </summary>
        public async Task DisconnectAllAsync()
        {
            Task[] tasks = new Task[CLIENTS.Count];
            CLIENT_SEMA.Wait();
            for (int i = 0; i < CLIENTS.Count; i++)
            {
                tasks[i] = CLIENTS[i].DisconnectAsync();
            }
            CLIENT_SEMA.Release();

            for (int i = 0; i < tasks.Length; i++)
            {
                await tasks[i].ConfigureAwait(true);
            }
        }

        /// <summary>
        /// Disconnects and removes the given account from the client list.
        /// </summary>
        /// <param name="accountId">The account id of the client you would like to remove.</param>
        public Task RemoveAccountAsync(string accountId)
        {
            return Task.Run(() =>
            {
                CLIENT_SEMA.Wait();
                for (int i = 0; i < CLIENTS.Count; i++)
                {
                    if (string.Equals(CLIENTS[i].client.dbAccount.bareJid, accountId))
                    {
                        CLIENTS[i].DisconnectAsync().Wait();
                        CLIENTS.RemoveAt(i);
                    }
                }
                CLIENT_SEMA.Release();
            });
        }

        /// <summary>
        /// Reconnects all XMPPClients.
        /// </summary>
        public void ReconnectAll()
        {
            Parallel.ForEach(CLIENTS, async (c) => await ReconnectClientAsync(c));
        }

        /// <summary>
        /// Reloads all clients from the DB.
        /// Disconnects all existing clients first.
        /// </summary>
        public void ReloadClients()
        {
            DisconnectAll();
            LoadClients();
            ConnectAll();
        }

        /// <summary>
        /// Loads the given account and adds it to the list of clients (<see cref="CLIENTS"/>) and connects it in case it is not disabled.
        /// </summary>
        /// <param name="account">The <see cref="AccountModel"/> that should be added.</param>
        public void AddAccount(AccountModel account)
        {
            CLIENT_SEMA.Wait();
            ClientConnectionHandler handler = LoadClient(account);
            CLIENT_SEMA.Release();
            if (!handler.client.dbAccount.disabled)
            {
                handler.Connect();
            }
        }

        #endregion

        #region --Misc Methods (Private)--
        /// <summary>
        /// Loads all XMPPClients from the DB and inserts them into the clients list.
        /// </summary>
        private void LoadClients()
        {
            using (MainDbContext ctx = new MainDbContext())
            {
                CLIENT_SEMA.Wait();
                CLIENTS.Clear();
                IEnumerable<AccountModel> accounts = ctx.Accounts.Where(x => true).Include(ctx.GetIncludePaths(typeof(AccountModel)));
                foreach (AccountModel account in accounts)
                {
                    LoadClient(account);
                }
                CLIENT_SEMA.Release();
            }
        }

        /// <summary>
        /// Loads the given account and adds it to the list of clients (<see cref="CLIENTS"/>).
        /// </summary>
        /// <param name="account">The <see cref="AccountModel"/> that should be added.</param>
        private ClientConnectionHandler LoadClient(AccountModel account)
        {
            ClientConnectionHandler client = new ClientConnectionHandler(account);
            client.ClientConnected += OnClientConnected;
            CLIENTS.Add(client);
            return client;
        }

        /// <summary>
        /// Reconnects a given client.
        /// </summary>
        /// <param name="handler">The client, for which a reconnect should get performed.</param>
        /// <returns></returns>
        private async Task ReconnectClientAsync(ClientConnectionHandler handler)
        {
            if (handler.client.dbAccount.disabled)
            {
                // Only disconnect if the client is disabled:
                await handler.DisconnectAsync();
            }
            else
            {
                await handler.ReconnectAsync();
            }
        }

        #endregion

        #region --Misc Methods (Protected)--


        #endregion
        //--------------------------------------------------------Events:---------------------------------------------------------------------\\
        #region --Events--
        private void CLIENTS_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            ClientsCollectionChanged?.Invoke(this, e);
        }

        private void OnClientConnected(ClientConnectionHandler handler, ClientConnectedEventArgs args)
        {
            ClientConnected?.Invoke(this, args);
        }
        #endregion
    }
}
