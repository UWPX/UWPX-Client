namespace XMPP_API.Classes.Network.XML.Messages.Helper
{
    public class MessageResponseHelperResult<T> where T : AbstractAddressableMessage
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        public readonly MessageResponseHelperResultState STATE;
        public readonly T RESULT;

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        internal MessageResponseHelperResult(MessageResponseHelperResultState state) : this(state, null) { }

        internal MessageResponseHelperResult(MessageResponseHelperResultState state, T result)
        {
            STATE = state;
            RESULT = result;
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
