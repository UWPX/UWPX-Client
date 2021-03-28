using System.Xml;
using System.Xml.Linq;
using Logging;

namespace XMPP_API.Classes.Network.XML.Messages.XEP_0059
{
    public class Set: IXElementable
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        public string after;
        public string before;
        public uint? count;
        public string first;
        public uint? firstIndex;
        public uint? index;
        public string last;
        public uint? max;

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        public Set() { }

        public Set(XmlNode answer)
        {
            XmlNode afterNode = XMLUtils.getChildNode(answer, "after");
            if (!(afterNode is null))
            {
                after = afterNode.InnerText;
            }

            XmlNode beforeNode = XMLUtils.getChildNode(answer, "before");
            if (!(beforeNode is null))
            {
                before = beforeNode.InnerText;
            }

            XmlNode countNode = XMLUtils.getChildNode(answer, "count");
            if (!(countNode is null))
            {
                if (uint.TryParse(countNode.InnerText, out uint count))
                {
                    this.count = count;
                }
                else
                {
                    Logger.Error("Failed to parse XEP-0313 SET count node value as uint: " + countNode.InnerText);
                }
            }

            XmlNode firstNode = XMLUtils.getChildNode(answer, "first");
            if (!(firstNode is null))
            {
                first = firstNode.InnerText;
                string tmp = firstNode.Attributes["index"]?.Value;
                if (!(tmp is null))
                {
                    if (uint.TryParse(tmp, out uint index))
                    {
                        firstIndex = index;
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
                    this.index = index;
                }
                else
                {
                    Logger.Error("Failed to parse XEP-0313 SET index node value as uint: " + indexNode.InnerText);
                }
            }

            XmlNode lastNode = XMLUtils.getChildNode(answer, "last");
            if (!(lastNode is null))
            {
                last = lastNode.InnerText;
            }

            XmlNode maxNode = XMLUtils.getChildNode(answer, "max");
            if (!(maxNode is null))
            {
                if (uint.TryParse(maxNode.InnerText, out uint max))
                {
                    this.max = max;
                }
                else
                {
                    Logger.Error("Failed to parse XEP-0313 SET max node value as uint: " + maxNode.InnerText);
                }
            }
        }

        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--


        #endregion
        //--------------------------------------------------------Misc Methods:---------------------------------------------------------------\\
        #region --Misc Methods (Public)--
        public XElement toXElement(XNamespace ns)
        {
            XNamespace rsmNs = Consts.XML_XEP_0059_NAMESPACE;
            XElement setNode = new XElement(rsmNs + "set");
            if (!(after is null))
            {
                setNode.Add(new XElement(rsmNs + "after", after));
            }
            if (!(before is null))
            {
                setNode.Add(new XElement(rsmNs + "before", before));
            }
            if (!(count is null))
            {
                setNode.Add(new XElement(rsmNs + "count", count));
            }
            if (!(first is null))
            {
                XElement firstNode = new XElement(rsmNs + "first", first);
                if (!(firstIndex is null))
                {
                    firstNode.Add(new XAttribute(rsmNs + "index", firstIndex));
                }
                setNode.Add(firstNode);
            }
            if (!(firstIndex is null))
            {
                setNode.Add(new XElement(rsmNs + "first", after));
            }
            if (!(index is null))
            {
                setNode.Add(new XElement(rsmNs + "index", index));
            }
            if (!(last is null))
            {
                setNode.Add(new XElement(rsmNs + "last", last));
            }
            if (!(max is null))
            {
                setNode.Add(new XElement(rsmNs + "max", max));
            }
            return setNode;
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
