using System.Xml;
using XMPP_API.Classes.Network.XML.Messages.XEP_0059;

namespace XMPP_API.Classes.Network.XML.Messages.XEP_0313
{
    public class QueryArchiveFinishMessage: IQMessage
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        public readonly Set RESULT_SET;
        public readonly bool COMPLETE;

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        public QueryArchiveFinishMessage(XmlNode answer) : base(answer)
        {
            XmlNode finNode = XMLUtils.getChildNode(answer, "fin", Consts.XML_XMLNS, Consts.XML_XEP_0313_NAMESPACE);
            if (!(finNode is null))
            {
                COMPLETE = XMLUtils.tryParseToBool(finNode.Attributes["complete"]?.Value);
                XmlNode setNode = XMLUtils.getChildNode(finNode, "set", Consts.XML_XMLNS, Consts.XML_XEP_0059_NAMESPACE);
                if (!(setNode is null))
                {
                    RESULT_SET = new Set(setNode);
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
