using System.Collections.Generic;
using System.Xml;
using System.Xml.Linq;
using XMPP_API.Classes.Network.XML.Messages.XEP_0060;

namespace XMPP_API.Classes.Network.XML.Messages.XEP_0402
{
    public class BookmarksResultMessage : PubSubResultMessage
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        public List<ConferenceItem> conferences;

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        /// <summary>
        /// Basic Constructor
        /// </summary>
        /// <history>
        /// 15/07/2018 Created [Fabian Sauter]
        /// </history>
        public BookmarksResultMessage(XmlNode node) : base(node)
        {
        }

        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--
        protected override XElement getQuery()
        {
            throw new System.NotImplementedException();
        }

        #endregion
        //--------------------------------------------------------Misc Methods:---------------------------------------------------------------\\
        #region --Misc Methods (Public)--


        #endregion

        #region --Misc Methods (Private)--


        #endregion

        #region --Misc Methods (Protected)--
        protected override void loadContent(XmlNode content)
        {
            conferences = new List<ConferenceItem>();
            if (string.Equals(content.Name, "items"))
            {
                foreach (XmlNode n in content.ChildNodes)
                {
                    if (!string.IsNullOrEmpty(n.Attributes["id"]?.Value))
                    {
                        ConferenceItem conf = new ConferenceItem(n);
                        if (conf.IS_VALID)
                        {
                            conferences.Add(conf);
                        }
                    }
                }
            }
        }

        #endregion
        //--------------------------------------------------------Events:---------------------------------------------------------------------\\
        #region --Events--


        #endregion
    }
}
