using System.Xml;

namespace XMPP_API.Classes.Network.XML.Messages
{
    public class PresenceMessage : AbstractAddressableMessage
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        private readonly string SHOW;
        private readonly string STATUS;
        private readonly int PRIORETY;

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Construktoren--
        /// <summary>
        /// Basic Constructor
        /// </summary>
        /// <history>
        /// 25/08/2017 Created [Fabian Sauter]
        /// </history>
        public PresenceMessage(string from, string to, string show, string status, int priorety) : base(from, to)
        {
            this.SHOW = show;
            this.STATUS = status;
            this.PRIORETY = priorety;
        }

        public PresenceMessage(string show, string status, int priorety) : this(null, null, show, status, priorety)
        {
        }

        public PresenceMessage() : this(null, null, null, null, int.MinValue)
        {

        }

        public PresenceMessage(int priorety) : this(null, null, null, null, priorety)
        {

        }

        public PresenceMessage(XmlNode node) : base(node.Attributes["from"]?.Value, node.Attributes["to"]?.Value)
        {
            if(node.Attributes["type"] != null && node.Attributes["type"].Value.Equals("unavailable"))
            {
                this.SHOW = node.Attributes["type"].Value;
            }
            else
            {
                this.SHOW = XMLUtils.getChildNode(node, "show")?.InnerText;
            }
            this.STATUS = XMLUtils.getChildNode(node, "status")?.InnerText;
        }

        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--
        public string getStatus()
        {
            return STATUS;
        }

        public string getShow()
        {
            return SHOW;
        }

        #endregion
        //--------------------------------------------------------Misc Methods:---------------------------------------------------------------\\
        #region --Misc Methods (Public)--
        public override string toXmlString()
        {
            string s = "<presence";
            if(FROM != null)
            {
                s += " from='" + FROM + "'";
            }

            if (TO != null)
            {
                s += " to='" + TO + "'";
            }
            s += '>';

            if(SHOW != null)
            {
                s += "<show>" + SHOW + "</show>";
            }
            if(PRIORETY > -128 && PRIORETY < 129)
            {
                s += "<priority>" + PRIORETY + "</priority>";
            }

            if (STATUS != null)
            {
                s += "<status>" + STATUS + "</status>";
            }
            s += "</presence>";
            return s;
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
