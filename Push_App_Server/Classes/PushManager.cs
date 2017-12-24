using Data_Manager2.Classes;
using System.Threading.Tasks;

namespace Push_App_Server.Classes
{
    public class PushManager
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--


        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        /// <summary>
        /// Basic Constructor
        /// </summary>
        /// <history>
        /// 19/11/2017 Created [Fabian Sauter]
        /// </history>
        public PushManager()
        {
        }

        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--


        #endregion
        //--------------------------------------------------------Misc Methods:---------------------------------------------------------------\\
        #region --Misc Methods (Public)--
        public static void init()
        {
            //ConnectionHandler.INSTANCE.ClientConnected += INSTANCE_ClientConnected;
        }

        #endregion

        #region --Misc Methods (Private)--


        #endregion

        #region --Misc Methods (Protected)--


        #endregion
        //--------------------------------------------------------Events:---------------------------------------------------------------------\\
        #region --Events--
        private static void INSTANCE_ClientConnected(ConnectionHandler handler, Data_Manager.Classes.Events.ClientConnectedEventArgs args)
        {
            Task.Factory.StartNew(async () =>
            {
                DataWriter dW = new DataWriter(args.CLIENT);
                await dW.connectAndSendAsync();
            });
        }

        #endregion
    }
}
