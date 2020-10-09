using System.Xml;
using Logging;

namespace XMPP_API.Classes.Network.XML.Messages.XEP_0059
{
    public class Set
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        public readonly string AFTER;
        public readonly string BEFORE;
        public readonly uint? COUNT;
        public readonly string FIRST;
        public readonly uint? FIRST_INDEX;
        public readonly uint? INDEX;
        public readonly string LAST;
        public readonly string MAX;

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        public Set(XmlNode answer)
        {
            XmlNode afterNode = XMLUtils.getChildNode(answer, "after");
            if (!(afterNode is null))
            {
                AFTER = afterNode.InnerText;
            }

            XmlNode beforeNode = XMLUtils.getChildNode(answer, "before");
            if (!(beforeNode is null))
            {
                BEFORE = beforeNode.InnerText;
            }

            XmlNode countNode = XMLUtils.getChildNode(answer, "count");
            if (!(countNode is null))
            {
                if (uint.TryParse(countNode.InnerText, out uint count))
                {
                    COUNT = count;
                }
                else
                {
                    Logger.Error("Failed to parse XEP-0313 SET count node value as uint: " + countNode.InnerText);
                }
            }

            XmlNode firstNode = XMLUtils.getChildNode(answer, "first");
            if (!(firstNode is null))
            {
                FIRST = firstNode.InnerText;
                string tmp = firstNode.Attributes["index"]?.Value;
                if (!(tmp is null))
                {
                    if (uint.TryParse(tmp, out uint index))
                    {
                        FIRST_INDEX = index;
                    }
                    else
                    {
                        Logger.Error("Failed to parse XEP-0313 SET first_index node value as uint: " + tmp);
                    }
                }
            }

            XmlNode indexNode = XMLUtils.getChildNode(answer, "index");
            if (!(indexNode is null))
            {
                if (uint.TryParse(indexNode.InnerText, out uint index))
                {
                    INDEX = index;
                }
                else
                {
                    Logger.Error("Failed to parse XEP-0313 SET index node value as uint: " + indexNode.InnerText);
                }
            }

            XmlNode lastNode = XMLUtils.getChildNode(answer, "last");
            if (!(lastNode is null))
            {
                LAST = lastNode.InnerText;
            }

            XmlNode maxNode = XMLUtils.getChildNode(answer, "max");
            if (!(maxNode is null))
            {
                MAX = maxNode.InnerText;
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
