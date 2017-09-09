using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace XMPP_API.Classes.Network.XML.Messages
{
    public abstract class AbstractAddressableMessage : AbstractMessage
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        protected readonly string FROM;
        protected readonly string TO;

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Construktoren--
        /// <summary>
        /// Basic Constructor
        /// </summary>
        /// <history>
        /// 20/08/2017 Created [Fabian Sauter]
        /// </history>
        public AbstractAddressableMessage(string from, string to)
        {
            FROM = from;
            TO = to;
        }

        public AbstractAddressableMessage(string from, string to, string id) : base(id)
        {
            FROM = from;
            TO = to;
        }

        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--
        public string getFrom()
        {
            return FROM;
        }

        public string getTo()
        {
            return TO;
        }

        #endregion
        //--------------------------------------------------------Misc Methods:---------------------------------------------------------------\\
        #region --Misc Methods (Public)--


        #endregion

        #region --Misc Methods (Private)--


        #endregion

        #region --Misc Methods (Protected)--
        protected string buildHeader()
        {
            string s = Consts.XML_HEADER + Consts.XML_STREAM_START;
            s += Consts.XML_FROM + "'" + FROM + "'";
            s += Consts.XML_TO + "'" + TO + "'";
            s += Consts.XML_VERSION + Consts.XML_LANG + Consts.XML_CLIENT + Consts.XML_STREAM_NAMESPACE + '>';
            return s;
        }

        #endregion
        //--------------------------------------------------------Events:---------------------------------------------------------------------\\
        #region --Events--


        #endregion
    }
}
