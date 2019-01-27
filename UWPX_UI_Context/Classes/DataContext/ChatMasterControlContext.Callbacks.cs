using XMPP_API.Classes.Network.XML.Messages;

namespace UWPX_UI_Context.Classes.DataContext
{
    public sealed partial class ChatMasterControlContext
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--


        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--


        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--


        #endregion
        //--------------------------------------------------------Misc Methods:---------------------------------------------------------------\\
        #region --Misc Methods (Public)--


        #endregion

        #region --Misc Methods (Private)--
        private void OnUpdateBookmarksTimeout(MessageResponseHelper<IQMessage> helper)
        {
            InvokeOnError("Error", "Failed to update bookmark!\nServer did not respond in time.");
        }

        private bool OnUpdateBookmarksMessage(MessageResponseHelper<IQMessage> helper, IQMessage msg)
        {
            if (msg is IQErrorMessage errMsg)
            {
                InvokeOnError("Error", "Failed to bookmark!\nServer responded: " + errMsg.ERROR_OBJ.ERROR_NAME);
                return true;
            }
            if (string.Equals(msg.TYPE, IQMessage.RESULT))
            {
                return true;
            }
            return false;
        }

        #endregion

        #region --Misc Methods (Protected)--


        #endregion
        //--------------------------------------------------------Events:---------------------------------------------------------------------\\
        #region --Events--


        #endregion
    }
}
