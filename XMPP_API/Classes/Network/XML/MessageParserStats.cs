namespace XMPP_API.Classes.Network.XML
{
    public class MessageParserStats
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        private long totalMsgParseTimeMs;
        public long msgParseCount { get; private set; }
        public long avgParseTimeMs { get; private set; }
        public long minParseTimeMs { get; private set; }
        public long maxParseTimeMs { get; private set; }

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        /// <summary>
        /// Basic Constructor
        /// </summary>
        /// <history>
        /// 31/07/2018 Created [Fabian Sauter]
        /// </history>
        public MessageParserStats()
        {
            reset();
        }

        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--


        #endregion
        //--------------------------------------------------------Misc Methods:---------------------------------------------------------------\\
        #region --Misc Methods (Public)--
        public void onMeasurement(long elapsedMs)
        {
            msgParseCount++;
            if (elapsedMs < minParseTimeMs || minParseTimeMs < 0)
            {
                minParseTimeMs = elapsedMs;
            }
            else if (elapsedMs > maxParseTimeMs)
            {
                maxParseTimeMs = elapsedMs;
            }
            totalMsgParseTimeMs += elapsedMs;
            avgParseTimeMs = totalMsgParseTimeMs / msgParseCount;
        }

        public void reset()
        {
            msgParseCount = 0;
            totalMsgParseTimeMs = 0;
            avgParseTimeMs = 0;
            minParseTimeMs = -1;
            maxParseTimeMs = 0;
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
