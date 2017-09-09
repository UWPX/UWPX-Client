using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XMPP_API.Classes.Network.XML
{
    class MessageIdCash
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        private readonly ArrayList LIST;
        private int counter;

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Construktoren--
        /// <summary>
        /// Basic Constructor
        /// </summary>
        /// <history>
        /// 28/08/2017 Created [Fabian Sauter]
        /// </history>
        public MessageIdCash()
        {
            this.LIST = new ArrayList();
            counter = 30;
        }

        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--
        public bool contains(string id)
        {
            clean();
            foreach (MessageIdCashElement e in LIST)
            {
                if(e.ID.Equals(id) && e.isValid())
                {
                    LIST.Remove(e);
                    return true;
                }
            }
            return false;
        }

        #endregion
        //--------------------------------------------------------Misc Methods:---------------------------------------------------------------\\
        #region --Misc Methods (Public)--
        public void add(string id)
        {
            LIST.Add(new MessageIdCashElement(id));
        }

        #endregion

        #region --Misc Methods (Private)--
        private void clean()
        {
            counter--;
            if(counter <= 0)
            {
                // Clean all old ids
                foreach (MessageIdCashElement e in LIST)
                {
                    if (!e.isValid())
                    {
                        LIST.Remove(e);
                    }
                }
                counter = 30;
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
