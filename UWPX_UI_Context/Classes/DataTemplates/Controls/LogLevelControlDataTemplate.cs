using System;
using System.Collections.ObjectModel;
using Logging;
using Shared.Classes;
using Storage.Classes;

namespace UWPX_UI_Context.Classes.DataTemplates.Controls
{
    public sealed class LogLevelControlDataTemplate: AbstractDataTemplate
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        private LogLevelDataTemplate _LogLevelSelectedItem;
        public LogLevelDataTemplate LogLevelSelectedItem
        {
            get => _LogLevelSelectedItem;
            set => SetLogLevel(value);
        }
        public readonly ObservableCollection<LogLevelDataTemplate> LOG_LEVELS = new ObservableCollection<LogLevelDataTemplate>();

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
        private void SetLogLevel(LogLevelDataTemplate value)
        {
            if (SetProperty(ref _LogLevelSelectedItem, value, nameof(LogLevelSelectedItem)))
            {
                Settings.SetSetting(SettingsConsts.LOG_LEVEL, (int)value.LogLevel);
                Logger.logLevel = value.LogLevel;
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
                LogLevelDataTemplate tmp = new LogLevelDataTemplate
                {
                    LogLevel = l
                };
                LOG_LEVELS.Add(tmp);

                if (l == Logger.logLevel)
                {
                    LogLevelSelectedItem = tmp;
                }
            }
        }

        #endregion

        #region --Misc Methods (Protected)--


        #endregion
        //--------------------------------------------------------Events:---------------------------------------------------------------------\\
        #region --Events--


        #endregion
    }
}
