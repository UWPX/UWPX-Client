using System.Collections.Generic;
using System.Xml;

namespace XMPP_API.Classes.Network.XML.Messages.XEP_0045.Configuration
{
    public class RoomConfiguration
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        public List<MUCInfoField> options;

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        /// <summary>
        /// Basic Constructor
        /// </summary>
        /// <history>
        /// 07/02/2018 Created [Fabian Sauter]
        /// </history>
        public RoomConfiguration(XmlNode node)
        {
            this.options = new List<MUCInfoField>();
            if(node != null)
            {
                foreach (XmlNode n in node.ChildNodes)
                {
                    switch (n.Name)
                    {
                        case "field":
                            this.options.Add(new MUCInfoField(n));
                            break;

                        // Ignored for now:
                        case "title":
                        case "instructions":
                            break;

                        default:
                            Logging.Logger.Warn("Unknown MUC room config element '" + n.Name + "' received!");
                            break;
                    }
                }
            }
        }

        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--


        #endregion
        //--------------------------------------------------------Misc Methods:---------------------------------------------------------------\\
        #region --Misc Methods (Public)--


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
