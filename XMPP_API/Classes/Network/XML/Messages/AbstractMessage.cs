using System;
using System.Xml.Linq;

namespace XMPP_API.Classes.Network.XML.Messages
{
    public abstract class AbstractMessage
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        public static readonly byte NO_RESTART = 0;
        public static readonly byte HARD_RESTART = 1;
        public static readonly byte SOFT_RESTART = 2;

        protected readonly string ID;
        protected bool cacheUntilSend;
        private bool processed;
        private byte restartConnection;
        private static Random r = new Random();

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        /// <summary>
        /// Basic Constructor
        /// </summary>
        /// <history>
        /// 17/08/2017 Created [Fabian Sauter]
        /// </history>
        public AbstractMessage() : this(getRandomId())
        {

        }

        public AbstractMessage(string id)
        {
            processed = false;
            this.restartConnection = NO_RESTART;
            this.ID = id ?? getRandomId();
            this.cacheUntilSend = false;
        }

        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--
        public bool isProcessed()
        {
            return processed;
        }

        public void setProcessed()
        {
            processed = true;
        }

        public void setRestartConnection(byte restartConnection)
        {
            this.restartConnection = restartConnection;
        }

        public byte getRestartConnection()
        {
            return restartConnection;
        }

        public static string getRandomId()
        {
            string s = "";
            for(int i = 0; i < 5; i++)
            {
                s += (r.Next() + 1000).ToString();
                if (i < 4)
                {
                    s += '-';
                }
            }
            return s;
        }

        public string getId()
        {
            return ID;
        }

        public bool shouldSaveUntilSend()
        {
            return cacheUntilSend;
        }

        #endregion
        //--------------------------------------------------------Misc Methods:---------------------------------------------------------------\\
        #region --Misc Methods (Public)--
        public virtual string toXmlString()
        {
            return toXElement().ToString();
        }

        public abstract XElement toXElement();

        public override string ToString()
        {
            return toXmlString();
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
