using System.Xml.Linq;
using XMPP_API.Classes.Network.XML.Messages.XEP_0004;

namespace XMPP_API.Classes.Network.XML.Messages.XEP_0060
{
    public class PubSubPublishOptions
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        public readonly DataForm OPTIONS;

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        /// <summary>
        /// Basic Constructor
        /// </summary>
        /// <history>
        /// 02/06/2018 Created [Fabian Sauter]
        /// </history>
        public PubSubPublishOptions(DataForm options)
        {
            OPTIONS = options;
        }

        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--
        public static PubSubPublishOptions getDefaultPublishOptions()
        {
            DataForm form = new DataForm(DataFormType.SUBMIT);
            form.FIELDS.Add(new Field()
            {
                var = "FORM_TYPE",
                value = "http://jabber.org/protocol/pubsub#publish-options",
                type = FieldType.HIDDEN
            });
            return new PubSubPublishOptions(form);
        }

        #endregion
        //--------------------------------------------------------Misc Methods:---------------------------------------------------------------\\
        #region --Misc Methods (Public)--
        public XElement toXElement(XNamespace ns)
        {
            XElement pubSubOptNode = new XElement(ns + "publish-options");
            OPTIONS.addToXElement(pubSubOptNode);
            return pubSubOptNode;
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
