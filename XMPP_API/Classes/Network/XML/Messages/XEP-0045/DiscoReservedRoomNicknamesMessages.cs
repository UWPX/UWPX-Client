using System;
using System.Xml.Linq;
using XMPP_API.Classes.Network.XML.Messages.XEP_0030;

namespace XMPP_API.Classes.Network.XML.Messages.XEP_0045
{
    class DiscoReservedRoomNicknamesMessages : DiscoResponseMessage
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
        /// 06/01/2018 Created [Fabian Sauter]
        /// </history>
        public DiscoReservedRoomNicknamesMessages(string from, string to) : base(from, to, DiscoType.INFO)
        {
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
            XElement qNode = node.Element("query");
            if (qNode == null)
            {
                throw new InvalidOperationException("Node does not contain a 'query' node!");
            }
            qNode.Add(new XElement("node", "x-roomuser-item"));
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
