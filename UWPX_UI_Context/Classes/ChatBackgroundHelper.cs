using Data_Manager2.Classes;
using Shared.Classes;

namespace UWPX_UI_Context.Classes
{
    public class ChatBackgroundHelper : AbstractDataTemplate
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        public static readonly ChatBackgroundHelper INSTANCE = new ChatBackgroundHelper();

        private string _ImagePath;
        public string ImagePath
        {
            get { return _ImagePath; }
            set { SetImagePathProperty(value); }
        }

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--


        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--
        private void SetImagePathProperty(string value)
        {
            if (SetProperty(ref _ImagePath, value, nameof(ImagePath)))
            {
                Settings.setSetting(SettingsConsts.CHAT_BACKGROUND_IMAGE_PATH, value);
            }
        }

        #endregion
        //--------------------------------------------------------Misc Methods:---------------------------------------------------------------\\
        #region --Misc Methods (Public)--
        public void Init()
        {
            ImagePath = Settings.getSettingString(SettingsConsts.CHAT_BACKGROUND_IMAGE_PATH);
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
