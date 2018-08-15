using System.Xml.Linq;

namespace XMPP_API.Classes.Network.XML.Messages.XEP_0045
{
    public class JoinRoomRequestMessage : PresenceMessage
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        public readonly string ROOM_PASSWORD;

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        /// <summary>
        /// Basic Constructor
        /// </summary>
        /// <history>
        /// 06/01/2018 Created [Fabian Sauter]
        /// </history>
        public JoinRoomRequestMessage(string from, string room, string nick, string roomPassword) : base(from, room + '/' + nick, Presence.NotDefined, null, int.MinValue)
        {
            this.ROOM_PASSWORD = roomPassword;
        }

        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--


        #endregion
        //--------------------------------------------------------Misc Methods:---------------------------------------------------------------\\
        #region --Misc Methods (Public)--
        public override XElement toXElement()
        {
            XElement node = base.toXElement();
            XNamespace ns = Consts.XML_XEP_0045_NAMESPACE;
            XElement xNode = new XElement(ns + "x");
            if (!string.IsNullOrEmpty(ROOM_PASSWORD))
            {
                xNode.Add(new XElement(ns + "password")
                {
                    Value = ROOM_PASSWORD
                });
            }
            node.Add(xNode);
            return node;
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
