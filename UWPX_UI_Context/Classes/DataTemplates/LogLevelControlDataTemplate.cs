using Data_Manager2.Classes;
using Logging;
using System;
using System.Collections.ObjectModel;

namespace UWPX_UI_Context.Classes.DataTemplates
{
    public sealed class LogLevelControlDataTemplate : AbstractDataTemplate
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        private int _LogLevelSelectedIndex;
        public int LogLevelSelectedIndex
        {
            get { return _LogLevelSelectedIndex; }
            set { SetLogLevel(value); }
        }
        public readonly ObservableCollection<string> LOG_LEVELS = new ObservableCollection<string>();

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        public LogLevelControlDataTemplate()
        {
            LoadSettings();
        }

        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--
        private void SetLogLevel(int value)
        {
            if (SetProperty(ref _LogLevelSelectedIndex, value, nameof(LogLevelSelectedIndex)))
            {
                Settings.setSetting(SettingsConsts.LOG_LEVEL, value);
                Logger.logLevel = (LogLevel)value;
            }
        }

        #endregion
        //--------------------------------------------------------Misc Methods:---------------------------------------------------------------\\
        #region --Misc Methods (Public)--


        #endregion

        #region --Misc Methods (Private)--
        private void LoadSettings()
        {
            // Logs:
            foreach (LogLevel l in Enum.GetValues(typeof(LogLevel)))
            {
                switch (l)
                {
                    case LogLevel.NONE:
                        LOG_LEVELS.Add("None");
                        break;
                    case LogLevel.ERROR:
                        LOG_LEVELS.Add("Error");
                        break;
                    case LogLevel.WARNING:
                        LOG_LEVELS.Add("Warning");
                        break;
                    case LogLevel.INFO:
                        LOG_LEVELS.Add("Info");
                        break;
                    case LogLevel.DEBUG:
                        LOG_LEVELS.Add("Debug");
                        break;
                    default:
                        LOG_LEVELS.Add(l.ToString());
                        break;
                }
            }
            LogLevelSelectedIndex = (int)Logger.logLevel;
        }

        #endregion

        #region --Misc Methods (Protected)--


        #endregion
        //--------------------------------------------------------Events:---------------------------------------------------------------------\\
        #region --Events--


        #endregion
    }
}
