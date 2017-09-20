using System.Collections;
using System.Xml;

namespace XMPP_API.Classes.Network.XML.Messages
{
    public class RoosterMessage : IQMessage
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        private ArrayList items;

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        /// <summary>
        /// Basic Constructor
        /// </summary>
        /// <history>
        /// 28/08/2017 Created [Fabian Sauter]
        /// </history>
        public RoosterMessage(XmlNode answer) : base(answer)
        {
            loadItems();
        }

        public RoosterMessage(string from, string to) : base(from, to, GET, getRandomId(), Consts.XML_QUERY_ROOSTER)
        {
        }

        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--
        public ArrayList getItems()
        {
            return items;
        }

        #endregion
        //--------------------------------------------------------Misc Methods:---------------------------------------------------------------\\
        #region --Misc Methods (Public)--


        #endregion

        #region --Misc Methods (Private)--
        private void loadItems()
        {
            items = new ArrayList();
            foreach (XmlNode n in ANSWER.ChildNodes)
            {
                if(n.Name.Equals("presence"))
                {
                    items.Add(new PresenceMessage(n));
                }
                else
                {
                    items.Add(new RosterItem(n));
                }
            }
        }

        #endregion

        #region --Misc Methods (Protected)--


        #endregion
        //--------------------------------------------------------Events:---------------------------------------------------------------------\\
        #region --Events--


        #endregion
    }
}
