using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Shared.Classes.Collections;
using Storage.Classes.Contexts;

namespace Manager.Classes.Client
{
    public class ClientHandler
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        public static readonly ClientHandler INSTANCE = new ClientHandler();
        private bool initialized = false;

        private readonly CustomObservableCollection<Client> CLIENTS = new CustomObservableCollection<Client>(false);
        private readonly SemaphoreSlim CLIENTS_SEMA = new SemaphoreSlim(1);
        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        public ClientHandler() { }

        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--


        #endregion
        //--------------------------------------------------------Misc Methods:---------------------------------------------------------------\\
        #region --Misc Methods (Public)--
        public async Task InitAsync()
        {
            Debug.Assert(!initialized);
            initialized = true;
            await LoadClientsAsync();
        }

        #endregion

        #region --Misc Methods (Private)--
        private async Task LoadClientsAsync()
        {
            using (AccountDbContext ctx = new AccountDbContext())
            {
                await CLIENTS_SEMA.WaitAsync();
                CLIENTS.AddRange(ctx.Accounts.Select(dbAccount => new Client(dbAccount)));
                CLIENTS_SEMA.Release();
            }
        }

        #endregion

        #region --Misc Methods (Protected)--


        #endregion
        //--------------------------------------------------------Events:---------------------------------------------------------------------\\
        #region --Events--


        #endregion
    }
}
