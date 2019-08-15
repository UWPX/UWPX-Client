using System.Xml.Linq;

namespace XMPP_API.Classes.Network.XML.Messages
{
    public class ScramSha1ChallengeSolutionMessage : AbstractMessage
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        public readonly string SOLUTION;

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        /// <summary>
        /// Basic Constructor
        /// </summary>
        /// <history>
        /// 22/08/2017 Created [Fabian Sauter]
        /// </history>
        public ScramSha1ChallengeSolutionMessage(string solution)
        {
            SOLUTION = solution;
        }

        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--


        #endregion
        //--------------------------------------------------------Misc Methods:---------------------------------------------------------------\\
        #region --Misc Methods (Public)--
        public override XElement toXElement()
        {
            XNamespace ns = Consts.XML_SASL_NAMESPACE;
            XElement node = new XElement(ns + "response")
            {
                Value = SOLUTION
            };
            return node;
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
