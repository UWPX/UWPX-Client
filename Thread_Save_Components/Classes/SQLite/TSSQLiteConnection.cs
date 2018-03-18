using SQLite;
using System.Collections.Generic;
using Thread_Save_Components.Classes.Threading;

namespace Thread_Save_Components.Classes.SQLite
{
    public class TSSQLiteConnection
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        protected SQLiteConnection dB;

        public const int MAX_READ_COUNT = 10;

        private static readonly MySemaphoreSlim requestSema = new MySemaphoreSlim(1, 1);
        private static readonly MySemaphoreSlim readSema = new MySemaphoreSlim(MAX_READ_COUNT, MAX_READ_COUNT); // Allow up to MAX_READ_COUNT threads to read from the DB in parallel
        private static readonly MySemaphoreSlim writeSema = new MySemaphoreSlim(1, 1);

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
            dB = new SQLiteConnection(dBPath);
        }

        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--


        #endregion
        //--------------------------------------------------------Misc Methods:---------------------------------------------------------------\\
        #region --Misc Methods (Public)--
        public SQLiteCommand CreateCommand(string cmdText, params object[] args)
        {
            return dB.CreateCommand(cmdText, args);
        }

        public List<T> ExecuteCommand<T>(bool readOnly, SQLiteCommand cmd) where T : class
        {
            List<T> list;
            if (readOnly)
            {
                requestSema.Wait();
                readSema.Wait();
                requestSema.Release();

                list = cmd.ExecuteQuery<T>();

                readSema.Release();
            }
            else
            {
                requestSema.Wait();
                writeSema.Wait();
                readSema.WaitCount(MAX_READ_COUNT);
                requestSema.Release();

                list = cmd.ExecuteQuery<T>();

                writeSema.Release();
                readSema.Release(MAX_READ_COUNT);
            }
            return list;
        }

        public int InsertOrReplace(object obj)
        {
            requestSema.Wait();
            writeSema.Wait();
            readSema.WaitCount(MAX_READ_COUNT);
            requestSema.Release();

            int i = dB.InsertOrReplace(obj);

            writeSema.Release();
            readSema.Release(MAX_READ_COUNT);

            return i;
        }

        public void Close()
        {
            requestSema.Wait();
            writeSema.Wait();
            readSema.WaitCount(MAX_READ_COUNT);
            requestSema.Release();

            dB.Close();

            writeSema.Release();
            readSema.Release(MAX_READ_COUNT);
        }

        public int Execute(string query, params object[] args)
        {
            requestSema.Wait();
            writeSema.Wait();
            readSema.WaitCount(MAX_READ_COUNT);
            requestSema.Release();

            int i = dB.Execute(query, args);

            writeSema.Release();
            readSema.Release(MAX_READ_COUNT);

            return i;
        }

        public void Commit()
        {
            requestSema.Wait();
            writeSema.Wait();
            readSema.WaitCount(MAX_READ_COUNT);
            requestSema.Release();

            dB.Commit();

            writeSema.Release();
            readSema.Release(MAX_READ_COUNT);
        }

        public List<T> Query<T>(bool readOnly, string query, params object[] args) where T : new()
        {
            List<T> list;
            if (readOnly)
            {
                requestSema.Wait();
                readSema.Wait();
                requestSema.Release();

                list = dB.Query<T>(query, args);

                readSema.Release();
            }
            else
            {
                requestSema.Wait();
                writeSema.Wait();
                readSema.WaitCount(MAX_READ_COUNT);
                requestSema.Release();

                list = dB.Query<T>(query, args);

                writeSema.Release();
                readSema.Release(MAX_READ_COUNT);
            }
            return list;
        }

        public int CreateTable<T>() where T : class
        {
            requestSema.Wait();
            writeSema.Wait();
            readSema.WaitCount(MAX_READ_COUNT);
            requestSema.Release();

            int i = dB.CreateTable<T>();

            writeSema.Release();
            readSema.Release(MAX_READ_COUNT);

            return i;
        }

        public int DropTable<T>() where T : class
        {
            requestSema.Wait();
            writeSema.Wait();
            readSema.WaitCount(MAX_READ_COUNT);
            requestSema.Release();

            int i = dB.DropTable<T>();

            writeSema.Release();
            readSema.Release(MAX_READ_COUNT);

            return i;
        }

        public int RecreateTable<T>() where T : class
        {
            requestSema.Wait();
            writeSema.Wait();
            readSema.WaitCount(MAX_READ_COUNT);
            requestSema.Release();

            dB.DropTable<T>();
            int i = dB.CreateTable<T>();

            writeSema.Release();
            readSema.Release(MAX_READ_COUNT);

            return i;
        }

        public int Delete(object objectToDelete)
        {
            requestSema.Wait();
            writeSema.Wait();
            readSema.WaitCount(MAX_READ_COUNT);
            requestSema.Release();

            int i = dB.Delete(objectToDelete);

            writeSema.Release();
            readSema.Release(MAX_READ_COUNT);

            return i;
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
