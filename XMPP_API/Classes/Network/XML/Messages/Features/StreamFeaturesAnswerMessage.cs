using System;
using System.Collections;
using System.Xml;
using XMPP_API.Classes.Network.XML.Messages.Features.SASL;

namespace XMPP_API.Classes.Network.XML.Messages.Features
{
    class StreamFeaturesMessage : AbstractMessage
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        private readonly XmlNode STREAM_FEATURES_NODE;
        private readonly ArrayList FEATURES;

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Construktoren--
        /// <summary>
        /// Basic Constructor
        /// </summary>
        /// <history>
        /// 20/08/2017 Created [Fabian Sauter]
        /// </history>
        public StreamFeaturesMessage(XmlNode node)
        {
            this.STREAM_FEATURES_NODE = node;
            this.FEATURES = new ArrayList(5);
            loadFeatures();
        }

        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--
        public ArrayList getFeatures()
        {
            return FEATURES;
        }

        public bool isEmpty()
        {
            return STREAM_FEATURES_NODE == null;
        }

        public bool containsFeature(string name)
        {
            foreach (StreamFeature f in FEATURES)
            {
                if(f != null && f.getName() != null && f.getName().Equals(name))
                {
                    return true;
                }
            }
            return false;
        }

        #endregion
        //--------------------------------------------------------Misc Methods:---------------------------------------------------------------\\
        #region --Misc Methods (Public)--
        public override string toXmlString()
        {
            throw new NotImplementedException();
        }

        #endregion

        #region --Misc Methods (Private)--
        private void loadFeatures()
        {
            if(STREAM_FEATURES_NODE == null)
            {
                return;
            }
            foreach (XmlNode n in STREAM_FEATURES_NODE)
            {
                if(n.Name != null && n.Name.Equals("starttls"))
                {
                    FEATURES.Add(new TLSStreamFeature(n.Name, n.FirstChild != null && n.FirstChild.Name.Equals("required")));
                }
                else if(n.Name != null && n.Name.Equals("mechanisms") && n.Attributes["xmlns"] != null)
                {
                    FEATURES.Add(new SASLStreamFeature(n.Name, n));
                }
                else
                {
                    FEATURES.Add(new StreamFeature(n.Name, n.FirstChild != null && n.FirstChild.Name.Equals("required")));
                }
            }
        }

        #endregion

        #region --Misc Methods (Protected)--


        #endregion
        //--------------------------------------------------------Events:---------------------------------------------------------------------\\
        #region --Events--


        #endregion
    }
}
