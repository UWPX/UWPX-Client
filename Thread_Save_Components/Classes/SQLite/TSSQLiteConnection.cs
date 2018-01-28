using SQLite.Net;
using SQLite.Net.Platform.WinRT;
using System.Collections.Generic;
using System.Threading;

namespace Thread_Save_Components.Classes.SQLite
{
    public class TSSQLiteConnection
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        protected SQLiteConnection dB;

        private static readonly SemaphoreSlim requestSema = new SemaphoreSlim(1, 1);
        private static readonly SemaphoreSlim readSema = new SemaphoreSlim(10, 10); // Allow up to 10 threads to read from the db in parallel
        private static readonly SemaphoreSlim writeSema = new SemaphoreSlim(1, 1);

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
            dB = new SQLiteConnection(new SQLitePlatformWinRT(), dBPath);
        }

        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--


        #endregion
        //--------------------------------------------------------Misc Methods:---------------------------------------------------------------\\
        #region --Misc Methods (Public)--
        public int InsertOrReplace(object obj)
        {
            requestSema.Wait();
            writeSema.Wait();
            readSema.Wait();
            requestSema.Release();

            int i = dB.InsertOrReplace(obj);

            writeSema.Release();
            readSema.Release();

            return i;
        }

        public void Close()
        {
            requestSema.Wait();
            writeSema.Wait();
            readSema.Wait();
            requestSema.Release();

            dB.Close();

            writeSema.Release();
            readSema.Release();
        }

        public int Execute(string query, params object[] args)
        {
            requestSema.Wait();
            writeSema.Wait();
            readSema.Wait();
            requestSema.Release();

            int i = dB.Execute(query, args);

            writeSema.Release();
            readSema.Release();

            return i;
        }

        public List<T> Query<T>(bool readOnly, string query, params object[] args) where T : class
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
                readSema.Wait();
                requestSema.Release();

                list = dB.Query<T>(query, args);

                writeSema.Release();
                readSema.Release();
            }
            return list;
        }

        public int CreateTable<T>() where T : class
        {
            requestSema.Wait();
            writeSema.Wait();
            readSema.Wait();
            requestSema.Release();

            int i = dB.CreateTable<T>();

            writeSema.Release();
            readSema.Release();

            return i;
        }

        public int DropTable<T>() where T : class
        {
            requestSema.Wait();
            writeSema.Wait();
            readSema.Wait();
            requestSema.Release();

            int i = dB.DropTable<T>();

            writeSema.Release();
            readSema.Release();

            return i;
        }

        public int RecreateTable<T>() where T : class
        {
            requestSema.Wait();
            writeSema.Wait();
            readSema.Wait();
            requestSema.Release();

            dB.DropTable<T>();
            int i = dB.CreateTable<T>();

            writeSema.Release();
            readSema.Release();

            return i;
        }

        public int Delete(object objectToDelete)
        {
            requestSema.Wait();
            writeSema.Wait();
            readSema.Wait();
            requestSema.Release();

            int i = dB.Delete(objectToDelete);

            writeSema.Release();
            readSema.Release();

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
