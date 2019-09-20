using System;
using System.Collections.Generic;
using System.Xml;
using Logging;

namespace XMPP_API.Classes.Network.XML.Messages.XEP_0060
{
    public class PubSubAffiliationsResultMessage: AbstractPubSubResultMessage
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        public readonly Dictionary<string, PubSubAffiliation> AFFILIATIONS = new Dictionary<string, PubSubAffiliation>();

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        /// <summary>
        /// Basic Constructor
        /// </summary>
        /// <history>
        /// 20/09/2019 Created [Fabian Sauter]
        /// </history>
        public PubSubAffiliationsResultMessage(XmlNode node) : base(node) { }

        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--


        #endregion
        //--------------------------------------------------------Misc Methods:---------------------------------------------------------------\\
        #region --Misc Methods (Public)--


        #endregion

        #region --Misc Methods (Private)--
        private void loadAffiliation(XmlNode affNode)
        {
            XmlAttribute nodeAttr = affNode.Attributes["node"];
            XmlAttribute affAttr = affNode.Attributes["affiliation"];
            if (nodeAttr is null || string.IsNullOrEmpty(nodeAttr.Value) || affAttr is null || string.IsNullOrEmpty(affAttr.Value))
            {
                return;
            }

            PubSubAffiliation affiliation = PubSubAffiliation.ERROR;
            if (Enum.TryParse(affAttr.Value.ToUpper().Replace('-', '_'), out affiliation))
            {
                if (AFFILIATIONS.ContainsKey(nodeAttr.Value))
                {
                    Logger.Warn("Received PubSub found multiple affiliations for node: " + nodeAttr.Value);
                }
                else
                {
                    AFFILIATIONS[nodeAttr.Value] = affiliation;
                }
            }
            else
            {
                Logger.Warn("Unable to parse PubSub affiliation: " + affAttr.Value);
            }
        }

        #endregion

        #region --Misc Methods (Protected)--
        protected override void loadContent(XmlNodeList content)
        {
            foreach (XmlNode n in content)
            {
                if (string.Equals(n.Name, "affiliations"))
                {
                    foreach (XmlNode subNode in n.ChildNodes)
                    {
                        if (string.Equals(subNode.Name, "affiliation "))
                        {
                            loadAffiliation(subNode);
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
