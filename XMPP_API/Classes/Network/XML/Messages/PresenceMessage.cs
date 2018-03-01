using System.Xml;
using System.Xml.Linq;

namespace XMPP_API.Classes.Network.XML.Messages
{
    public class PresenceMessage : AbstractAddressableMessage
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        public readonly Presence PRESENCE;
        public readonly string STATUS;
        public readonly string TYPE;
        public readonly int PRIORETY;

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        /// <summary>
        /// Basic Constructor
        /// </summary>
        /// <history>
        /// 25/08/2017 Created [Fabian Sauter]
        /// </history>
        public PresenceMessage(string from, string to, Presence presence, string status, int priorety) : base(from, to)
        {
            this.PRESENCE = presence;
            this.STATUS = status;
            this.PRIORETY = priorety;
            this.cacheUntilSend = true;
        }

        public PresenceMessage(string from, string to, string type) : base(from, to)
        {
            this.TYPE = type;
            this.PRIORETY = int.MinValue;
            this.cacheUntilSend = true;
        }

        public PresenceMessage(int priorety) : this(null, null, Presence.NotDefined, null, priorety)
        {

        }

        public PresenceMessage(XmlNode node) : base(node.Attributes["from"]?.Value, node.Attributes["to"]?.Value)
        {
            if (node.Attributes["type"] != null)
            {
                this.TYPE = node.Attributes["type"].Value;
            }
            this.PRESENCE = Utils.parsePresence(XMLUtils.getChildNode(node, "show")?.InnerText);
            this.STATUS = XMLUtils.getChildNode(node, "status")?.InnerText;
        }

        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--


        #endregion
        //--------------------------------------------------------Misc Methods:---------------------------------------------------------------\\
        #region --Misc Methods (Public)--
        public override XElement toXElement()
        {
            XElement node = new XElement("presence");
            if (FROM != null)
            {
                node.Add(new XAttribute("from", FROM));
            }
            if (TO != null)
            {
                node.Add(new XAttribute("to", TO));
            }
            if (TYPE != null)
            {
                node.Add(new XAttribute("type", TYPE));
            }
            if (PRESENCE != Presence.NotDefined)
            {
                node.Add(new XElement("show", Utils.presenceToString(PRESENCE)));
            }
            if (PRIORETY > -128 && PRIORETY < 129)
            {
                node.Add(new XElement("priority", PRIORETY));
            }
            if (STATUS != null)
            {
                node.Add(new XElement("status", STATUS));
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
