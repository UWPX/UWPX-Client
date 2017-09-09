using System.ComponentModel;
using XMPP_API.Classes.Network.XML.Messages;

namespace XMPP_API.Classes.Events
{
    public class NewPresenceEventArgs : CancelEventArgs
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        private readonly Presence PRESENCE;
        private readonly string STATUS;
        private readonly string FROM;

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Construktoren--
        /// <summary>
        /// Basic Constructor
        /// </summary>
        /// <history>
        /// 21/08/2017 Created [Fabian Sauter]
        /// </history>
        public NewPresenceEventArgs(PresenceMessage message)
        {
            this.STATUS = message.getStatus();
            this.FROM = message.getFrom();
            if (message.getShow() == null)
            {
                this.PRESENCE = Presence.Online;
            }
            else
            {
                switch (message.getShow())
                {
                    case "chat":
                        this.PRESENCE = Presence.Chat;
                        break;
                    case "away":
                        this.PRESENCE = Presence.Away;
                        break;
                    case "xa":
                        this.PRESENCE = Presence.Xa;
                        break;
                    case "dnd":
                        this.PRESENCE = Presence.Dnd;
                        break;
                    case "unavailable":
                        this.PRESENCE = Presence.Unavailable;
                        break;
                }
            }
        }

        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--
        public Presence getPresence()
        {
            return PRESENCE;
        }

        public string getFrom()
        {
            return FROM;
        }

        public string getStatus()
        {
            return STATUS;
        }

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
