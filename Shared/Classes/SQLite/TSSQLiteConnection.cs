using Logging;
using SQLite;
using System;
using System.Collections.Generic;
using System.Threading;

namespace Shared.Classes.SQLite
{
    public class TSSQLiteConnection : IDisposable
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        /// <summary>
        /// Can handle multiple DB connections.
        /// The Tuple contains the following properties:
        /// Tuple(bool=In transaction, Mutex=A mutex to lock transactions, SQLiteConnection=The actual SQLite connection)
        /// </summary>
        protected static readonly Dictionary<string, Tuple<bool, Mutex, SQLiteConnection>> DB_CONNECTIONS = new Dictionary<string, Tuple<bool, Mutex, SQLiteConnection>>();
        private static readonly Mutex DB_CONNECTION_MUTEX = new Mutex();
        private readonly string DB_PATH;

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        /// <summary>
        /// Basic Constructor
        /// </summary>
        /// <history>
        /// 05/01/2018 Created [Fabian Sauter]
        /// </history>
        public TSSQLiteConnection(string dBPath)
        {
            this.DB_PATH = dBPath;

            DB_CONNECTION_MUTEX.WaitOne();
            if (!DB_CONNECTIONS.ContainsKey(dBPath))
            {
                DB_CONNECTIONS[dBPath] = new Tuple<bool, Mutex, SQLiteConnection>(false, new Mutex(), new SQLiteConnection(dBPath, SQLiteOpenFlags.ReadWrite | SQLiteOpenFlags.Create | SQLiteOpenFlags.SharedCache));
            }
            DB_CONNECTION_MUTEX.ReleaseMutex();
        }

        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--


        #endregion
        //--------------------------------------------------------Misc Methods:---------------------------------------------------------------\\
        #region --Misc Methods (Public)--
        public SQLiteCommand CreateCommand(string cmdText, params object[] args)
        {
            return SharedUtils.RetryOnException(() => DB_CONNECTIONS[DB_PATH].Item3.CreateCommand(cmdText, args));
        }

        public void BeginTransaction()
        {
            Tuple<bool, Mutex, SQLiteConnection> connection = DB_CONNECTIONS[DB_PATH];
            connection.Item2.WaitOne();
            if (connection.Item1)
            {
                SharedUtils.RetryOnException(() => connection.Item3.BeginTransaction());
                DB_CONNECTIONS[DB_PATH] = new Tuple<bool, Mutex, SQLiteConnection>(true, connection.Item2, connection.Item3);
            }
            connection.Item2.ReleaseMutex();
        }

        public int DeleteAll<T>()
        {
            try
            {
                return SharedUtils.RetryOnException(() => DB_CONNECTIONS[DB_PATH].Item3.DeleteAll<T>());
            }
            catch (Exception e)
            {
                Logger.Error("Failed to execute DB delete all!", e);
                return -1;
            }
        }

        public List<T> ExecuteCommand<T>(bool readOnly, SQLiteCommand cmd) where T : new()
        {
            try
            {
                return SharedUtils.RetryOnException(() => cmd.ExecuteQuery<T>());
            }
            catch (Exception e)
            {
                Logger.Error("Failed to execute DB execute command!", e);
                return new List<T>();
            }
        }

        public int InsertOrReplace(object obj)
        {
            try
            {
                return SharedUtils.RetryOnException(() => DB_CONNECTIONS[DB_PATH].Item3.InsertOrReplace(obj)); ;
            }
            catch (Exception e)
            {
                Logger.Error("Failed to execute DB insert or replace!", e);
                return -1;
            }
        }

        public int Insert(object obj)
        {
            try
            {
                return SharedUtils.RetryOnException(() => DB_CONNECTIONS[DB_PATH].Item3.Insert(obj));
            }
            catch (Exception e)
            {
                Logger.Error("Failed to execute DB insert!", e);
                return -1;
            }
        }

        public int InsertAll(IEnumerable<object> objects, bool runInTransaction = true)
        {
            try
            {
                return SharedUtils.RetryOnException(() => DB_CONNECTIONS[DB_PATH].Item3.InsertAll(objects));
            }
            catch (Exception e)
            {
                Logger.Error("Failed to execute DB insert all!", e);
                return -1;
            }
        }

        public void Close()
        {
            DB_CONNECTION_MUTEX.WaitOne();
            Tuple<bool, Mutex, SQLiteConnection> connection = DB_CONNECTIONS[DB_PATH];
            SharedUtils.RetryOnException(() => connection.Item3.Close());
            connection.Item2.Dispose();
            DB_CONNECTIONS.Remove(DB_PATH);
            DB_CONNECTION_MUTEX.ReleaseMutex();
        }

        public int Execute(string query, params object[] args)
        {
            try
            {
                return SharedUtils.RetryOnException(() => DB_CONNECTIONS[DB_PATH].Item3.Execute(query, args));
            }
            catch (Exception e)
            {
                Logger.Error("Failed to execute DB execute!", e);
                return -1;
            }
        }

        public void Commit()
        {
            Tuple<bool, Mutex, SQLiteConnection> connection = DB_CONNECTIONS[DB_PATH];
            connection.Item2.WaitOne();
            if (connection.Item1)
            {
                SharedUtils.RetryOnException(() => connection.Item3.Commit());
                DB_CONNECTIONS[DB_PATH] = new Tuple<bool, Mutex, SQLiteConnection>(false, connection.Item2, connection.Item3);
            }
            connection.Item2.ReleaseMutex();
        }

        /// <param name="readOnly">Unused/placeholder!</param>
        public List<T> Query<T>(bool readOnly, string query, params object[] args) where T : new()
        {
            try
            {
                return SharedUtils.RetryOnException(() => DB_CONNECTIONS[DB_PATH].Item3.Query<T>(query, args));
            }
            catch (Exception e)
            {
                Logger.Error("Failed to execute DB query!", e);
                return new List<T>();
            }
        }

        public CreateTableResult CreateTable<T>() where T : new()
        {
            return SharedUtils.RetryOnException(() => DB_CONNECTIONS[DB_PATH].Item3.CreateTable<T>());
        }

        public int DropTable<T>() where T : new()
        {
            return SharedUtils.RetryOnException(() => DB_CONNECTIONS[DB_PATH].Item3.DropTable<T>());
        }

        public CreateTableResult RecreateTable<T>() where T : new()
        {
            SharedUtils.RetryOnException(() => DB_CONNECTIONS[DB_PATH].Item3.DropTable<T>());
            return SharedUtils.RetryOnException(() => DB_CONNECTIONS[DB_PATH].Item3.CreateTable<T>());
        }

        public int Delete(object objectToDelete)
        {
            try
            {
                return SharedUtils.RetryOnException(() => DB_CONNECTIONS[DB_PATH].Item3.Delete(objectToDelete));
            }
            catch (Exception e)
            {
                Logger.Error("Failed to execute DB delete!", e);
                return -1;
            }
        }

        public void Dispose()
        {
            foreach (var connection in DB_CONNECTIONS)
            {
                try
                {
                    connection.Value.Item3?.Close();
                }
                catch (Exception e)
                {
                    Logger.Error("Failed to close DB!", e);
                }
            }
        }

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
