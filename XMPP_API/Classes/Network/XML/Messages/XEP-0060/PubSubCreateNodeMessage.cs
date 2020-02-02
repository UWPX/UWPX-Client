using System.Xml.Linq;
using XMPP_API.Classes.Network.XML.Messages.XEP_0004;

namespace XMPP_API.Classes.Network.XML.Messages.XEP_0060
{
    public class PubSubCreateNodeMessage: AbstractPubSubMessage
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        public readonly string NODE_NAME;
        public readonly DataForm NODE_CONFIG;

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        /// <summary>
        /// Basic Constructor
        /// </summary>
        /// <history>
        /// 17/07/2018 Created [Fabian Sauter]
        /// </history>
        public PubSubCreateNodeMessage(string from, string to, string nodeName) : this(from, to, nodeName, null)
        {
        }

        public PubSubCreateNodeMessage(string from, string to, string nodeName, DataForm nodeConfig) : base(from, to)
        {
            NODE_NAME = nodeName;
            NODE_CONFIG = nodeConfig;
        }

        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--
        protected override void addContent(XElement node, XNamespace ns)
        {
            XElement createNode = new XElement(ns + "create");
            createNode.Add(new XAttribute("node", NODE_NAME));
            node.Add(createNode);

            if (NODE_CONFIG != null)
            {
                XElement configNode = new XElement(ns + "configure");
                NODE_CONFIG.addToXElement(configNode);
                node.Add(configNode);
            }
        }

        public DataForm getConfigTemplate()
        {
            DataForm config = new DataForm(DataFormType.SUBMIT);
            config.fields.Add(new Field
            {
                var = "FORM_TYPE",
                type = FieldType.HIDDEN,
                value = Consts.XML_XEP_0060_NAMESPACE_NODE_CONFIG
            });

            return config;
        }

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
