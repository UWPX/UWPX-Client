using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Shared.Classes.Collections;
using Storage.Classes.Contexts;
using Storage.Classes.Models.Account;

namespace Manager.Classes.Chat
{
    public class ChatHandler
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        public static readonly ChatHandler INSTANCE = new ChatHandler();
        private bool initialized = false;

        private readonly SemaphoreSlim CHATS_SEMA = new SemaphoreSlim(1);
        private readonly CustomObservableCollection<Storage.Classes.Models.Chat.Chat> CHATS = new CustomObservableCollection<Storage.Classes.Models.Chat.Chat>(false);
        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--


        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--
        public Storage.Classes.Models.Chat.Chat GetChat(Account dbAccount, string chatJid)
        {
            CHATS_SEMA.Wait();
            Storage.Classes.Models.Chat.Chat result = CHATS.Where(c => string.Equals(c.accountBareJid, dbAccount.bareJid) && string.Equals(c.bareJid, chatJid)).FirstOrDefault();
            CHATS_SEMA.Release();
            return result;
        }

        #endregion
        //--------------------------------------------------------Misc Methods:---------------------------------------------------------------\\
        #region --Misc Methods (Public)--
        public async Task InitAsync()
        {
            Debug.Assert(!initialized);
            initialized = true;
            await LoadChatsAsync();
        }

        #endregion

        #region --Misc Methods (Private)--
        private async Task LoadChatsAsync()
        {
            using (ChatDbContext ctx = new ChatDbContext())
            {
                await CHATS_SEMA.WaitAsync();
                CHATS.AddRange(ctx.Chats);
                CHATS_SEMA.Release();
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
