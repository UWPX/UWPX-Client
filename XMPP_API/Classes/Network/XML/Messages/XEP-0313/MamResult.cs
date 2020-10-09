using System.Collections.Generic;
using System.Diagnostics;

namespace XMPP_API.Classes.Network.XML.Messages.XEP_0313
{
    public class MamResult
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        public readonly string FIRST;
        public readonly string LAST;
        public readonly uint INDEX;
        public readonly uint COUNT;
        public readonly bool COMPLETE;
        public readonly List<QueryArchiveResultMessage> RESULTS;

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        public MamResult(QueryArchiveFinishMessage msg, List<QueryArchiveResultMessage> results)
        {
            if (!msg.COMPLETE)
            {
                Debug.Assert(string.IsNullOrEmpty(msg.RESULT_SET.FIRST));
                Debug.Assert(!(msg.RESULT_SET.FIRST_INDEX is null));
                Debug.Assert(string.IsNullOrEmpty(msg.RESULT_SET.LAST));

                FIRST = msg.RESULT_SET.FIRST;
                LAST = msg.RESULT_SET.LAST;
                INDEX = (uint)msg.RESULT_SET.FIRST_INDEX;
            }
            else
            {
                COMPLETE = true;
            }
            Debug.Assert(!(msg.RESULT_SET.COUNT is null));
            COUNT = (uint)msg.RESULT_SET.COUNT;

            RESULTS = results;
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
