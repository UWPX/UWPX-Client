using System.Xml;
using System.Xml.Linq;

namespace XMPP_API.Classes.Network.XML.Messages.XEP_0198
{
    public class SMAnswerMessage: AbstractMessage
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        public readonly int HANDLE;

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        /// <summary>
        /// Basic Constructor
        /// </summary>
        /// <history>
        /// 06/05/2018 Created [Fabian Sauter]
        /// </history>
        public SMAnswerMessage(int handle)
        {
            HANDLE = handle;
        }

        public SMAnswerMessage(XmlNode node)
        {
            XmlAttribute att = node.Attributes["h"];
            if (att is null || !int.TryParse(att.Value, out HANDLE))
            {
                HANDLE = -1;
            }
        }

        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--


        #endregion
        //--------------------------------------------------------Misc Methods:---------------------------------------------------------------\\
        #region --Misc Methods (Public)--
        public override XElement toXElement()
        {
            XNamespace ns = Consts.XML_XEP_0198_NAMESPACE;
            XElement node = new XElement(ns + "a");
            node.Add(new XAttribute("h", HANDLE));

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
