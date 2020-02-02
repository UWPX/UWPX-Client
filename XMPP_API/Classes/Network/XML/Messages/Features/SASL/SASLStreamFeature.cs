using System.Collections;
using System.Xml;

namespace XMPP_API.Classes.Network.XML.Messages.Features.SASL
{
    internal class SASLStreamFeature: StreamFeature
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        public readonly ArrayList MECHANISMS;

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        /// <summary>
        /// Basic Constructor
        /// </summary>
        /// <history>
        /// 22/08/2017 Created [Fabian Sauter]
        /// </history>
        public SASLStreamFeature(XmlNode n) : base(n)
        {
            MECHANISMS = new ArrayList(3);
            loadMechanisms(n);
        }

        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--


        #endregion
        //--------------------------------------------------------Misc Methods:---------------------------------------------------------------\\
        #region --Misc Methods (Public)--


        #endregion

        #region --Misc Methods (Private)--
        private void loadMechanisms(XmlNode mechanismsNode)
        {
            if (mechanismsNode is null)
            {
                return;
            }

            foreach (XmlNode n in mechanismsNode.ChildNodes)
            {
                switch (n.Name)
                {
                    case "mechanism" when n.InnerText != null:
                        MECHANISMS.Add(n.InnerText);
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
