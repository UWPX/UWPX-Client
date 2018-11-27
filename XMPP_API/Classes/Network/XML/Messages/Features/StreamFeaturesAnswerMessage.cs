using System;
using System.Collections;
using System.Xml;
using System.Xml.Linq;
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
        #region --Constructors--
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
            return STREAM_FEATURES_NODE is null;
        }

        public bool containsFeature(string name)
        {
            foreach (StreamFeature f in FEATURES)
            {
                if (string.Equals(f.NAME, name))
                {
                    return true;
                }
            }
            return false;
        }

        public bool containsFeature(string name, string ns)
        {
            foreach (StreamFeature f in FEATURES)
            {
                if (string.Equals(f.NAME, name) && string.Equals(f.NAMESPACE, ns))
                {
                    return true;
                }
            }
            return false;
        }

        #endregion
        //--------------------------------------------------------Misc Methods:---------------------------------------------------------------\\
        #region --Misc Methods (Public)--
        public override XElement toXElement()
        {
            throw new NotImplementedException();
        }

        #endregion

        #region --Misc Methods (Private)--
        private void loadFeatures()
        {
            if (STREAM_FEATURES_NODE is null)
            {
                return;
            }
            foreach (XmlNode n in STREAM_FEATURES_NODE)
            {
                switch (n.Name)
                {
                    case "starttls":
                        FEATURES.Add(new TLSStreamFeature(n));
                        break;

                    case "mechanisms" when string.Equals(n.NamespaceURI, Consts.XML_SASL_NAMESPACE):
                        FEATURES.Add(new SASLStreamFeature(n));
                        break;

                    default:
                        FEATURES.Add(new StreamFeature(n));
                        break;
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
