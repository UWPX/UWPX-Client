using System.Text;
using Manager.Classes;
using Shared.Classes;
using Storage.Classes;
using UWPX_UI_Context.Classes.DataTemplates.Pages;
using XMPP_API.Classes.Crypto;

namespace UWPX_UI_Context.Classes.DataContext.Pages
{
    public sealed class DebugSettingsPageContext
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        public readonly DebugSettingsPageDataTemplate MODEL = new DebugSettingsPageDataTemplate();

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        public DebugSettingsPageContext()
        {
            LoadSettings();
        }

        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--


        #endregion
        //--------------------------------------------------------Misc Methods:---------------------------------------------------------------\\
        #region --Misc Methods (Public)--
        public void ResetSpamRegex()
        {
            MODEL.SpamRegex = SpamHelper.DEFAULT_SPAM_REGEX;
        }

        #endregion

        #region --Misc Methods (Private)--
        private void LoadSettings()
        {
            // Debug:
            MODEL.DisableTcpTimeout = Settings.GetSettingBoolean(SettingsConsts.DEBUG_DISABLE_TCP_TIMEOUT);
            MODEL.DisableTlsTimeout = Settings.GetSettingBoolean(SettingsConsts.DEBUG_DISABLE_TLS_TIMEOUT);

            // Spam:
            MODEL.SpamRegex = Settings.GetSettingString(SettingsConsts.SPAM_REGEX, SpamHelper.DEFAULT_SPAM_REGEX);
            MODEL.SpamDetectionNewChatsOnly = !Settings.GetSettingBoolean(SettingsConsts.SPAM_DETECTION_FOR_ALL_CHAT_MESSAGES);
            MODEL.SpamDetectionEnabled = Settings.GetSettingBoolean(SettingsConsts.SPAM_DETECTION_ENABLED);

            // Device ID:
            MODEL.DeviceID = SharedUtils.GetUniqueDeviceId();
            MODEL.DeviceNonce = CryptoUtils.byteArrayToHexString(SharedUtils.GetDeviceNonce());

            StringBuilder accountIdsSb = new StringBuilder();
            foreach (ClientConnectionHandler c in ConnectionHandler.INSTANCE.GetClients())
            {
                accountIdsSb.Append(c.client.dbAccount.bareJid);
                accountIdsSb.Append(" ➔ ");
                accountIdsSb.Append(Push.Classes.Utils.ToAccountId(c.client.dbAccount.bareJid));
                accountIdsSb.Append('\n');
            }
            MODEL.AccountIds = accountIdsSb.ToString().Trim();
        }

        #endregion

        #region --Misc Methods (Protected)--


        #endregion
        //--------------------------------------------------------Events:---------------------------------------------------------------------\\
        #region --Events--


        #endregion
    }
}
