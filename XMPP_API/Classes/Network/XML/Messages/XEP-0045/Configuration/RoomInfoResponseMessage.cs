using System.Xml;

namespace XMPP_API.Classes.Network.XML.Messages.XEP_0045.Configuration
{
    public class RoomInfoResponseMessage : IQMessage
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        public bool isRoomConfigrationAllowed;
        public RoomConfiguration roomConfig;

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        /// <summary>
        /// Basic Constructor
        /// </summary>
        /// <history>
        /// 07/07/2018 Created [Fabian Sauter]
        /// </history>
        public RoomInfoResponseMessage(XmlNode answer) : base(answer)
        {
            XmlNode qNode = XMLUtils.getChildNode(answer, "query", "xmlns", Consts.MUC_ROOM_INFO_NAMESPACE_REGEX);
            if(qNode != null)
            {
                XmlNode x = XMLUtils.getChildNode(qNode, "x", "xmlns", "jabber:x:data");
                if (x != null)
                {
                    this.isRoomConfigrationAllowed = true;
                    this.roomConfig = new RoomConfiguration(x);
                    return;
                }
                else
                {
                    isRoomConfigrationAllowed = false;
                }
            }
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
