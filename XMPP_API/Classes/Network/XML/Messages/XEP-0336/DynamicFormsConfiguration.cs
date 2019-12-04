using System.Xml;
using System.Xml.Linq;

namespace XMPP_API.Classes.Network.XML.Messages.XEP_0336
{
    public class DynamicFormsConfiguration
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        public DynamicFormsFlags flags;
        /// <summary>
        /// Only has a value if the DynamicFormsFlags.ERROR flag in flags is set.
        /// </summary>
        public readonly string errorMessage;

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        public DynamicFormsConfiguration() { }

        public DynamicFormsConfiguration(XmlNode node)
        {
            foreach (XmlNode n in node.ChildNodes)
            {
                switch (n.Name)
                {
                    case "xdd:readOnly":
                        flags |= DynamicFormsFlags.READ_ONLY;
                        break;

                    case "xdd:postBack":
                        flags |= DynamicFormsFlags.POST_BACK;
                        break;

                    case "xdd:notSame":
                        flags |= DynamicFormsFlags.NOT_SAME;
                        break;

                    case "xdd:error":
                        flags |= DynamicFormsFlags.ERROR;
                        errorMessage = n.InnerText;
                        break;

                    default:
                        break;
                }
            }
        }

        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--


        #endregion
        //--------------------------------------------------------Misc Methods:---------------------------------------------------------------\\
        #region --Misc Methods (Public)--
        public void addToNode(XElement node)
        {
            if (flags.HasFlag(DynamicFormsFlags.READ_ONLY))
            {
                node.Add(new XElement("xdd:readOnly"));
            }
            if (flags.HasFlag(DynamicFormsFlags.POST_BACK))
            {
                node.Add(new XElement("xdd:postBack"));
            }
            if (flags.HasFlag(DynamicFormsFlags.NOT_SAME))
            {
                node.Add(new XElement("xdd:notSame"));
            }
            if (flags.HasFlag(DynamicFormsFlags.ERROR))
            {
                XElement errorNode = new XElement("xdd:error");
                errorNode.SetValue(errorMessage);
                node.Add(errorNode);
            }
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
