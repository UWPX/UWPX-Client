using System.ComponentModel;
using XMPP_API.Classes.Network.XML.Messages;

namespace XMPP_API.Classes.Events
{
    public class NewPresenceMessageEventArgs : CancelEventArgs
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        private readonly Presence PRESENCE;
        private readonly string STATUS;
        private readonly string FROM;
        private readonly string PRESENCE_TYPE;

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        /// <summary>
        /// Basic Constructor
        /// </summary>
        /// <history>
        /// 21/08/2017 Created [Fabian Sauter]
        /// </history>
        public NewPresenceMessageEventArgs(PresenceMessage message)
        {
            this.STATUS = message.getStatus();
            this.FROM = message.getFrom();
            this.PRESENCE_TYPE = message.getType();

            switch (message.getShow() ?? message.getType())
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
                case "error":
                case "unavailable":
                    this.PRESENCE = Presence.Unavailable;
                    break;
                case "unsubscribe":
                case "unsubscribed":
                case "subscribe":
                    this.PRESENCE = Presence.NotDefined;
                    break;
                case "subscribed":
                default:
                    this.PRESENCE = Presence.Online;
                    break;
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

        public string getPresenceType()
        {
            return PRESENCE_TYPE;
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
