using SQLite;
using System;
using System.Collections.Generic;
using System.Threading;

namespace Thread_Save_Components.Classes.SQLite
{
    public class TSSQLiteConnection
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
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
            return DB_CONNECTIONS[DB_PATH].Item3.CreateCommand(cmdText, args);
        }

        public void BeginTransaction()
        {
            Tuple<bool, Mutex, SQLiteConnection> connection = DB_CONNECTIONS[DB_PATH];
            connection.Item2.WaitOne();
            if (connection.Item1)
            {
                connection.Item3.BeginTransaction();
                DB_CONNECTIONS[DB_PATH] = new Tuple<bool, Mutex, SQLiteConnection>(true, connection.Item2, connection.Item3);
            }
            connection.Item2.ReleaseMutex();
        }

        public List<T> ExecuteCommand<T>(bool readOnly, SQLiteCommand cmd) where T : new()
        {
            return cmd.ExecuteQuery<T>();
        }

        public int InsertOrReplace(object obj)
        {
            return DB_CONNECTIONS[DB_PATH].Item3.InsertOrReplace(obj);
        }

        public int InsertAll(IEnumerable<object> objects, bool runInTransaction = true)
        {
            return DB_CONNECTIONS[DB_PATH].Item3.InsertAll(objects);
        }

        public void Close()
        {
            DB_CONNECTION_MUTEX.WaitOne();
            Tuple<bool, Mutex, SQLiteConnection> connection = DB_CONNECTIONS[DB_PATH];
            connection.Item3.Close();
            connection.Item2.Dispose();
            DB_CONNECTIONS.Remove(DB_PATH);
            DB_CONNECTION_MUTEX.ReleaseMutex();
        }

        public int Execute(string query, params object[] args)
        {
            return DB_CONNECTIONS[DB_PATH].Item3.Execute(query, args);
        }

        public void Commit()
        {
            Tuple<bool, Mutex, SQLiteConnection> connection = DB_CONNECTIONS[DB_PATH];
            connection.Item2.WaitOne();
            if (connection.Item1)
            {
                connection.Item3.Commit();
                DB_CONNECTIONS[DB_PATH] = new Tuple<bool, Mutex, SQLiteConnection>(false, connection.Item2, connection.Item3);
            }
            connection.Item2.ReleaseMutex();
        }

        /// <param name="readOnly">Unused/placeholder!</param>
        public List<T> Query<T>(bool readOnly, string query, params object[] args) where T : new()
        {
            return DB_CONNECTIONS[DB_PATH].Item3.Query<T>(query, args);
        }

        public CreateTableResult CreateTable<T>() where T : new()
        {
            return DB_CONNECTIONS[DB_PATH].Item3.CreateTable<T>();
        }

        public int DropTable<T>() where T : new()
        {
            return DB_CONNECTIONS[DB_PATH].Item3.DropTable<T>();
        }

        public CreateTableResult RecreateTable<T>() where T : new()
        {
            DB_CONNECTIONS[DB_PATH].Item3.DropTable<T>();
            return DB_CONNECTIONS[DB_PATH].Item3.CreateTable<T>();
        }

        public int Delete(object objectToDelete)
        {
            return DB_CONNECTIONS[DB_PATH].Item3.Delete(objectToDelete);
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
