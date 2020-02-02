using System.ComponentModel;
using XMPP_API.Classes.Network.XML.Messages.XEP_0184;

namespace XMPP_API.Classes.Network.Events
{
    public class NewDeliveryReceiptEventArgs: CancelEventArgs
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        public readonly DeliveryReceiptMessage MSG;

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        /// <summary>
        /// Basic Constructor
        /// </summary>
        /// <history>
        /// 31/07/2018 Created [Fabian Sauter]
        /// </history>
        public NewDeliveryReceiptEventArgs(DeliveryReceiptMessage msg)
        {
            MSG = msg;
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
