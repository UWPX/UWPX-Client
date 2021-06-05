using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace Shared.Classes.AppCenter
{
    public class TrackErrorEventArgs: CancelEventArgs
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        public readonly Exception EXCEPTION;
        public readonly string DESCRIPTION_MD;
        public readonly Dictionary<string, string> PAYLOAD;

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        public TrackErrorEventArgs(Exception ex, string descriptionMd, Dictionary<string, string> payload)
        {
            EXCEPTION = ex;
            DESCRIPTION_MD = descriptionMd;
            PAYLOAD = payload;
        }

        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--


        #endregion
        //--------------------------------------------------------Misc Methods:---------------------------------------------------------------\\
        #region --Misc Methods (Public)--
        public string ToMarkdown()
        {
            StringBuilder sb = new StringBuilder("**Description:**\n");
            sb.Append(DESCRIPTION_MD);
            sb.Append("\n\n");

            sb.Append("**Exception:**\n");
            sb.Append(EXCEPTION.Message);
            sb.Append("\n");
            sb.Append(EXCEPTION.StackTrace);

            if (!(PAYLOAD is null) && PAYLOAD.Count > 0)
            {
                sb.Append("\n\n");
                sb.Append("**Misc:**\n");
                foreach (KeyValuePair<string, string> pair in PAYLOAD)
                {
                    sb.Append(pair.Key);
                    sb.Append(':');
                    sb.Append(pair.Value);
                    sb.Append("\n");
                }
            }
            return sb.ToString();
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
