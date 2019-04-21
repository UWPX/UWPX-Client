using System;
using NLog;
using NLog.Targets;
using Windows.Foundation.Diagnostics;

namespace Logging
{
    internal class WindowsLogChannel: TargetWithLayout
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        private readonly LoggingChannel CHANNEL = new LoggingChannel("UWPX_LOG_CHANNEL", null, new Guid("4bd2826e-54a1-4ba9-bf63-92b73ea1ac4a"));

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--


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
        protected override void Write(LogEventInfo logEvent)
        {
            string text = Layout.Render(logEvent);
            if (logEvent.Level == NLog.LogLevel.Debug)
            {
                CHANNEL.LogEvent(text, null, LoggingLevel.Verbose); // Workaround because else the channel will only show "stringmessage:," (https://stackoverflow.com/questions/43651340/empty-etw-message-in-windows-device-portal)
            }
            else if (logEvent.Level == NLog.LogLevel.Error)
            {
                CHANNEL.LogEvent(text, null, LoggingLevel.Error);
            }
            else if (logEvent.Level == NLog.LogLevel.Fatal)
            {
                CHANNEL.LogEvent(text, null, LoggingLevel.Critical);
            }
            else if (logEvent.Level == NLog.LogLevel.Info)
            {
                CHANNEL.LogEvent(text, null, LoggingLevel.Information);
            }
            else if (logEvent.Level == NLog.LogLevel.Warn)
            {
                CHANNEL.LogEvent(text, null, LoggingLevel.Warning);
            }
            else if (logEvent.Level == NLog.LogLevel.Trace)
            {
                CHANNEL.LogEvent(text, null, LoggingLevel.Information);
            }
        }

        #endregion
        //--------------------------------------------------------Events:---------------------------------------------------------------------\\
        #region --Events--


        #endregion
    }
}
