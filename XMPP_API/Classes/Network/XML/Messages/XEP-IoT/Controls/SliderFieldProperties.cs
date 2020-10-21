using System.Xml;

namespace XMPP_API.Classes.Network.XML.Messages.XEP_IoT.Controls
{
    public class SliderFieldProperties
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        public readonly double MIN;
        public readonly double MAX;
        public readonly double STEPS;

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        public SliderFieldProperties(XmlNode node)
        {
            MIN = double.Parse(node.Attributes["min"].Value);
            MAX = double.Parse(node.Attributes["max"].Value);
            STEPS = double.Parse(node.Attributes["steps"].Value);
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
