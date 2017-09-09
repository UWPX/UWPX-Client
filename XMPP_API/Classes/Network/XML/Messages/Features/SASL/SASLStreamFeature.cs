using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace XMPP_API.Classes.Network.XML.Messages.Features.SASL
{
    class SASLStreamFeature : StreamFeature
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        private readonly ArrayList MECHANISMS;

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Construktoren--
        /// <summary>
        /// Basic Constructor
        /// </summary>
        /// <history>
        /// 22/08/2017 Created [Fabian Sauter]
        /// </history>
        public SASLStreamFeature(string name, XmlNode mechanismsNode) : base(name, true)
        {
            this.MECHANISMS = new ArrayList(3);
            loadMechanisms(mechanismsNode);
        }

        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--
        public ArrayList getMechanisms()
        {
            return MECHANISMS;
        }

        #endregion
        //--------------------------------------------------------Misc Methods:---------------------------------------------------------------\\
        #region --Misc Methods (Public)--


        #endregion

        #region --Misc Methods (Private)--
        private void loadMechanisms(XmlNode mechanismsNode)
        {
            if(mechanismsNode == null)
            {
                return;
            }

            foreach (XmlNode n in mechanismsNode.ChildNodes)
            {
                if(n != null && n.Name != null && n.Name.Equals("mechanism") && n.InnerText != null)
                {
                    MECHANISMS.Add(n.InnerText);
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
