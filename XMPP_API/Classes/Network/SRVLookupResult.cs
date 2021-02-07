namespace XMPP_API.Classes.Network
{
    public class SRVLookupResult
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        public readonly bool SUCCESS;

        public readonly string SERVER_ADDRESS;
        public readonly ushort PORT;

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        public SRVLookupResult()
        {
            SUCCESS = false;
        }

        public SRVLookupResult(string serverAddress, ushort port)
        {
            SUCCESS = true;
            SERVER_ADDRESS = serverAddress;
            PORT = port;
        }

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
