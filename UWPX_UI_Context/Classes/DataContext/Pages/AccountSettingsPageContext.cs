using System.Threading.Tasks;
using Manager.Classes;
using Push.Classes;
using Storage.Classes;
using UWPX_UI_Context.Classes.DataTemplates.Pages;

namespace UWPX_UI_Context.Classes.DataContext.Pages
{
    public sealed class AccountSettingsPageContext
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        public readonly AccountSettingsPageDataTemplate MODEL = new AccountSettingsPageDataTemplate();

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        public AccountSettingsPageContext()
        {
            LoadSettings();
        }

        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--


        #endregion
        //--------------------------------------------------------Misc Methods:---------------------------------------------------------------\\
        #region --Misc Methods (Public)--
        public Task ReconnectAllAsync()
        {
            return ConnectionHandler.INSTANCE.ReconnectAllAsync();
        }

        public void OnLoaded()
        {
            PushManager.INSTANCE.StateChanged -= PushManager_StateChanged;
            PushManager.INSTANCE.StateChanged += PushManager_StateChanged;
            MODEL.PushManagerState = PushManager.INSTANCE.GetState().ToString();
            MODEL.PushInitialized = PushManager.INSTANCE.GetState() == PushManagerState.INITIALIZED;
            MODEL.PushError = PushManager.INSTANCE.GetState() == PushManagerState.ERROR;
        }

        public void OnUnloaded()
        {
            PushManager.INSTANCE.StateChanged -= PushManager_StateChanged;
        }

        public Task RequestTestPushAsync()
        {
            return PushManager.INSTANCE.RequestTestPushMessageAsync();
        }

        public Task InitPushForAccountsAsync()
        {
            return PushManager.INSTANCE.InitPushForAccountsAsync();
        }

        public void InitPush()
        {
            PushManager.INSTANCE.Init();
        }

        #endregion

        #region --Misc Methods (Private)--
        private void LoadSettings()
        {
            MODEL.DebugSettingsEnabled = Settings.GetSettingBoolean(SettingsConsts.DEBUG_SETTINGS_ENABLED);
            MODEL.PushEnabled = Settings.GetSettingBoolean(SettingsConsts.PUSH_ENABLED);
            MODEL.ChannelUri = Settings.GetSettingString(SettingsConsts.PUSH_CHANNEL_URI);
            MODEL.ChannelCreatedDate = Settings.GetSettingDateTime(SettingsConsts.PUSH_CHANNEL_CREATED_DATE_TIME).ToString("MM/dd/yy H:mm:ss");
        }

        #endregion

        #region --Misc Methods (Protected)--


        #endregion
        //--------------------------------------------------------Events:---------------------------------------------------------------------\\
        #region --Events--
        private void PushManager_StateChanged(PushManager sender, Push.Classes.Events.PushManagerStateChangedEventArgs args)
        {
            MODEL.PushManagerState = args.NEW_STATE.ToString();
            MODEL.PushInitialized = args.NEW_STATE == PushManagerState.INITIALIZED;
            MODEL.PushError = args.NEW_STATE == PushManagerState.ERROR;
            LoadSettings();
        }

        #endregion
    }
}
