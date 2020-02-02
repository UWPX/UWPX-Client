using System.Xml.Linq;

namespace XMPP_API.Classes.Network.XML.Messages
{
    public class PresenceErrorMessage: PresenceMessage
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        public readonly PresenceErrorType ERROR_TYPE;

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        /// <summary>
        /// Basic Constructor
        /// </summary>
        /// <history>
        /// 27/11/2018 Created [Fabian Sauter]
        /// </history>
        public PresenceErrorMessage(string from, string to, PresenceErrorType errorType) : base(from, to, Presence.NotDefined, null, int.MinValue)
        {
            ERROR_TYPE = errorType;
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
            switch (ERROR_TYPE)
            {
                case PresenceErrorType.FORBIDDEN:
                    node.Add(new XElement("forbidden"));
                    break;

                case PresenceErrorType.NOT_AUTHORIZED:
                    node.Add(new XElement("not-authorized"));
                    break;

                default:
                    break;
            }
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
