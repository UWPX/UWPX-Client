using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using Logging;
using Microsoft.EntityFrameworkCore;
using Storage.Classes.Migrations;
using Windows.Storage;

namespace Storage.Classes.Contexts
{
    public abstract class AbstractDbContext: DbContext
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        private static readonly string DB_PATH = Path.Combine(ApplicationData.Current.LocalFolder.Path, "uwpx.db");

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        public AbstractDbContext()
        {
            Database.EnsureCreated();
        }

        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--


        #endregion
        //--------------------------------------------------------Misc Methods:---------------------------------------------------------------\\
        #region --Misc Methods (Public)--
        public bool ApplyMigration(AbstractSqlMigration migration)
        {
            try
            {
                migration.ApplyMigration(Database);
                return true;
            }
            catch (System.Exception e)
            {
                Logger.Error("Failed to apply DB migration.", e);
#if DEBUG
                if (Debugger.IsAttached)
                {
                    Debugger.Break();
                }
#endif
            }
            return false;
        }

        public async Task RecreateDb()
        {
            await Database.EnsureDeletedAsync();
            await Database.EnsureCreatedAsync();
        }

        #endregion

        #region --Misc Methods (Private)--


        #endregion

        #region --Misc Methods (Protected)--
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlite("Data Source=" + DB_PATH);
            }
        }

        #endregion
        //--------------------------------------------------------Events:---------------------------------------------------------------------\\
        #region --Events--


        #endregion
    }
}
