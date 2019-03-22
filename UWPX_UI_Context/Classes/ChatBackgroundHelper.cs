using Data_Manager2.Classes;
using Logging;
using Shared.Classes;
using System;
using System.Threading.Tasks;
using Windows.Storage;

namespace UWPX_UI_Context.Classes
{
    public class ChatBackgroundHelper : AbstractDataTemplate
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        private const string CUSTOM_BACKGROUND_IMAGE_FILE_NAME = "customBackgroundImage";
        private const string CUSTOM_BACKGROUND_IMAGE_FOLDER_NAME = "BackgroundImage";

        public static readonly ChatBackgroundHelper INSTANCE = new ChatBackgroundHelper();

        private string _ImagePath;
        public string ImagePath
        {
            get { return _ImagePath; }
            set { SetImagePathProperty(value); }
        }

        private string _CustomImagePath;
        public string CustomImagePath
        {
            get { return _CustomImagePath; }
            set { SetCustomImagePathProperty(value); }
        }

        private ChatBackgroundMode _BackgroundMode;
        public ChatBackgroundMode BackgroundMode
        {
            get { return _BackgroundMode; }
            set { SetBackgroundModeProperty(value); }
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

        private void SetCustomImagePathProperty(string value)
        {
            if (SetProperty(ref _CustomImagePath, value, nameof(CustomImagePath)))
            {
                Settings.setSetting(SettingsConsts.CHAT_CUSTOM_BACKGROUND_IMAGE_PATH, value);
            }
        }

        private void SetBackgroundModeProperty(ChatBackgroundMode value)
        {
            if (SetProperty(ref _BackgroundMode, value, nameof(BackgroundMode)))
            {
                Settings.setSetting(SettingsConsts.CHAT_BACKGROUND_MODE, (int)value);
            }
        }

        #endregion
        //--------------------------------------------------------Misc Methods:---------------------------------------------------------------\\
        #region --Misc Methods (Public)--
        public void Init()
        {
            ImagePath = Settings.getSettingString(SettingsConsts.CHAT_BACKGROUND_IMAGE_PATH);
            CustomImagePath = Settings.getSettingString(SettingsConsts.CHAT_CUSTOM_BACKGROUND_IMAGE_PATH);
            BackgroundMode = (ChatBackgroundMode)Settings.getSettingInt(SettingsConsts.CHAT_BACKGROUND_MODE, 0);
        }

        public async Task<string> SaveAsCustomBackgroundImageAsync(StorageFile file)
        {
            try
            {
                StorageFolder folder = await ApplicationData.Current.LocalFolder.CreateFolderAsync(CUSTOM_BACKGROUND_IMAGE_FOLDER_NAME, CreationCollisionOption.OpenIfExists);
                if (!(folder is null))
                {
                    // Delete image:
                    await DeleteCustomBackgroundImage();

                    // Save new image:
                    StorageFile f = await folder.CreateFileAsync(DateTime.Now.ToString("MM_dd_yyyy-HH_mm_ss") + file.FileType, CreationCollisionOption.OpenIfExists);
                    if (!(f is null))
                    {
                        await file.CopyAndReplaceAsync(f);
                        Logger.Info("Saved custom background image.");
                        CustomImagePath = f.Path;
                    }
                }
            }
            catch (Exception e)
            {
                Logger.Error("Failed to save image as custom background.", e);
            }
            return CustomImagePath;
        }

        public async Task DeleteCustomBackgroundImage()
        {
            try
            {
                StorageFile f = await StorageFile.GetFileFromPathAsync(CustomImagePath);
                if (!(f is null))
                {
                    await f.DeleteAsync();
                }
                Logger.Info("Deleted custom background image.");
            }
            catch (Exception e)
            {
                Logger.Error("Failed to delete custom background image!", e);
            }
            CustomImagePath = null;
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
