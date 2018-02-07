using System.Collections.Generic;
using System.Xml;

namespace XMPP_API.Classes.Network.XML.Messages.XEP_0045.Configuration
{
    public class RoomConfiguration
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        public List<AbstractConfigrurationOption> options;

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
            this.options = new List<AbstractConfigrurationOption>();
            if(node != null)
            {
                foreach (XmlNode n in node.ChildNodes)
                {
                    switch (n.Name)
                    {
                        case "title":
                            this.options.Add(new Title(n));
                            break;

                        case "instructions":
                            this.options.Add(new Instructions(n));
                            break;

                        case "field":
                            this.options.Add(new Field(n));
                            break;

                        case "option":
                            this.options.Add(new Option(n));
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
