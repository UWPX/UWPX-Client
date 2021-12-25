using System;
using System.Xml.Linq;

namespace XMPP_API.Classes.Network.XML.Messages.XEP_0084
{
    public class AvatarInfo: IXElementable
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        public readonly uint BYTES;
        public readonly ushort HEIGHT;
        public readonly ushort WEIGHT;
        public readonly string ID;
        public readonly string TYPE;
        public readonly Uri URL;

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        public AvatarInfo(uint bytes, ushort height, ushort weight, string id, string type)
        {
            BYTES = bytes;
            HEIGHT = height;
            WEIGHT = weight;
            ID = id;
            TYPE = type;
        }

        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--


        #endregion
        //--------------------------------------------------------Misc Methods:---------------------------------------------------------------\\
        #region --Misc Methods (Public)--
        public XElement toXElement(XNamespace ns)
        {
            XElement infoNode = new XElement(ns + "info");
            infoNode.Add(new XAttribute("bytes", BYTES));
            infoNode.Add(new XAttribute("id", ID));
            infoNode.Add(new XAttribute("type", TYPE));
            if (URL is not null)
            {
                infoNode.Add(new XAttribute("url", URL));
            }
            if (HEIGHT > 0)
            {
                infoNode.Add(new XAttribute("height", HEIGHT));
            }
            if (WEIGHT > 0)
            {
                infoNode.Add(new XAttribute("weight", WEIGHT));
            }

            return infoNode;
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
