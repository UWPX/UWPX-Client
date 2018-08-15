using System;
using System.Collections.Generic;
using System.Xml.Linq;
using XMPP_API.Classes.Network.XML.Messages.XEP_0334;

namespace XMPP_API.Classes.Network.XML.Messages
{
    public abstract class AbstractMessage
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        public const byte NO_RESTART = 0;
        public const byte HARD_RESTART = 1;
        public const byte SOFT_RESTART = 2;

        public readonly string ID;
        protected bool cacheUntilSend;
        private bool processed;
        private byte restartConnection;
        private static readonly Random R = new Random();

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
            for (int i = 0; i < 5; i++)
            {
                s += (R.Next() + 1000).ToString();
                if (i < 4)
                {
                    s += '-';
                }
            }
            return s;
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

        /// <summary>
        /// Adds the XEP-0334 (Message Processing Hints) to the given node.
        /// </summary>
        /// <param name="node">The node where the hints should get added to.</param>
        /// <param name="hints">A list of hints that should get added to the given node.</param>
        protected void addMPHints(XElement node, IList<MessageProcessingHint> hints)
        {
            XNamespace ns = Consts.XML_XEP_0334_NAMESPACE;
            foreach (MessageProcessingHint hint in hints)
            {
                switch (hint)
                {
                    case MessageProcessingHint.STORE:
                        if (ns == null)
                        {
                            node.Add(new XElement(ns + "store"));
                        }
                        else
                        {
                            node.Add(new XElement(ns + "store"));
                        }
                        break;

                    case MessageProcessingHint.NO_PERMANENT_STORE:
                        if (ns == null)
                        {
                            node.Add(new XElement(ns + "no-permanent-store"));
                        }
                        else
                        {
                            node.Add(new XElement(ns + "no-permanent-store"));
                        }
                        break;

                    case MessageProcessingHint.NO_STORE:
                        if (ns == null)
                        {
                            node.Add(new XElement(ns + "no-store"));
                        }
                        else
                        {
                            node.Add(new XElement(ns + "no-store"));
                        }
                        break;

                    case MessageProcessingHint.NO_COPIES:
                        if (ns == null)
                        {
                            node.Add(new XElement(ns + "no-copy"));
                        }
                        else
                        {
                            node.Add(new XElement(ns + "no-copy"));
                        }
                        break;
                }
            }
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
