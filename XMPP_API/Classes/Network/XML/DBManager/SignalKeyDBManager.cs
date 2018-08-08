using Thread_Save_Components.Classes.SQLite;
using XMPP_API.Classes.Network.XML.DBEntries;

namespace XMPP_API.Classes.Network.XML.DBManager
{
    class SignalKeyDBManager : AbstractDBManager
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        public readonly SignedPreKeyTable INSTANCE = new SignedPreKeyTable();

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        /// <summary>
        /// Basic Constructor
        /// </summary>
        /// <history>
        /// 17/03/2018 Created [Fabian Sauter]
        /// </history>
        public SignalKeyDBManager()
        {
        }

        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--


        #endregion
        //--------------------------------------------------------Misc Methods:---------------------------------------------------------------\\
        #region --Misc Methods (Public)--
        protected override void createTables()
        {
            dB.CreateTable<IdentityKeyTable>();
            dB.CreateTable<SignedPreKeyTable>();
            dB.CreateTable<PreKeyTable>();
        }

        protected override void dropTables()
        {
            dB.DropTable<IdentityKeyTable>();
            dB.DropTable<SignedPreKeyTable>();
            dB.DropTable<PreKeyTable>();
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
