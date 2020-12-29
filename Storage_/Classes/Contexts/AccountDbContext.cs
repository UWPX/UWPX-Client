using Microsoft.EntityFrameworkCore;
using Storage.Classes.Models.Account;

namespace Storage.Classes.Contexts
{
    public class AccountDbContext: AbstractDbContext
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        public DbSet<Account> Accounts { get; set; }
        public DbSet<Jid> Jids { get; set; }
        public DbSet<Server> Servers { get; set; }
        public DbSet<IgnoredCertificateError> IgnoredCertificateErrors { get; set; }

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--


        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--


        #endregion
        //--------------------------------------------------------Misc Methods:---------------------------------------------------------------\\
        #region --Misc Methods (Public)--


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
