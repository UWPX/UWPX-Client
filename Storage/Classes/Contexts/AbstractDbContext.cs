﻿using System.IO;
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
        private readonly AbstractSqlMigration MIGRATION;

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        public AbstractDbContext()
        {
            Database.EnsureCreated();
        }

        protected AbstractDbContext(AbstractSqlMigration migration)
        {
            MIGRATION = migration;
        }

        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--


        #endregion
        //--------------------------------------------------------Misc Methods:---------------------------------------------------------------\\
        #region --Misc Methods (Public)--
        public void ApplyMigration(AbstractSqlMigration migration)
        {
            migration.ApplyMigration(Database);
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
