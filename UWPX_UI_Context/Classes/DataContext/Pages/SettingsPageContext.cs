﻿using Logging;
using Storage.Classes;
using UWPX_UI_Context.Classes.DataTemplates.Pages;

namespace UWPX_UI_Context.Classes.DataContext.Pages
{
    public sealed class SettingsPageContext
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        public readonly SettingsPageDataTemplate MODEL = new SettingsPageDataTemplate();
        private int versionTappCount = 0;

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--


        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--


        #endregion
        //--------------------------------------------------------Misc Methods:---------------------------------------------------------------\\
        #region --Misc Methods (Public)--
        public void OnVersionTextTapped()
        {
            versionTappCount++;
            if (versionTappCount >= 5)
            {
                versionTappCount = 0;

                bool debugSettingsEnabled = !Settings.GetSettingBoolean(SettingsConsts.DEBUG_SETTINGS_ENABLED);
                Settings.SetSetting(SettingsConsts.DEBUG_SETTINGS_ENABLED, debugSettingsEnabled);
                MODEL.DebugSettingsEnabled = debugSettingsEnabled;
                if (debugSettingsEnabled)
                {
                    Logger.Info("Debug settings enabled.");
                }
                else
                {
                    Logger.Info("Debug settings disabled.");
                }
            }
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
