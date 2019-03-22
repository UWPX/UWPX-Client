using Data_Manager2.Classes;
using Shared.Classes;
using Shared.Classes.Collections;
using System;
using System.Threading.Tasks;
using Windows.Storage;

namespace UWPX_UI_Context.Classes.DataTemplates.Controls
{
    public sealed class BackgroundImageSelectionControlDataTemplate : AbstractDataTemplate
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        private const string SPLASH_SCREEN_IMAGE_PATH = "ms-appx:///Assets/Images/SplashScreen/splash_screen_800.png";

        public readonly CustomObservableCollection<BackgroundImageSelectionControlItemDataTemplate> IMAGES = new CustomObservableCollection<BackgroundImageSelectionControlItemDataTemplate>(true);

        private BackgroundImageSelectionControlItemDataTemplate _SelectedItem;
        public BackgroundImageSelectionControlItemDataTemplate SelectedItem
        {
            get { return _SelectedItem; }
            set { SetSelectedItem(value); }
        }

        private bool _IsNoBackgroundChecked;
        public bool IsNoBackgroundChecked
        {
            get { return _IsNoBackgroundChecked; }
            set { SetIsNoBackgroundCheckedProperty(value); }
        }

        private bool _IsImageBackgroundChecked;
        public bool IsImageBackgroundChecked
        {
            get { return _IsImageBackgroundChecked; }
            set { SetIsImageBackgroundCheckedProperty(value); }
        }

        private bool _IsCustomImageBackgroundSelected;
        public bool IsCustomImageBackgroundSelected
        {
            get { return _IsCustomImageBackgroundSelected; }
            set { SetIsCustomImageBackgroundSelectedProperty(value); }
        }

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        public BackgroundImageSelectionControlDataTemplate()
        {
            LoadImages();
            LoadSettings();
        }

        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--
        private void SetSelectedItem(BackgroundImageSelectionControlItemDataTemplate value)
        {
            if (SetProperty(ref _SelectedItem, value, nameof(SelectedItem)) && !(value is null))
            {
                ChatBackgroundHelper.INSTANCE.ImagePath = value.Path;
                IsCustomImageBackgroundSelected = false;
                SetBackgroundMode();
            }
        }

        private void SetIsImageBackgroundCheckedProperty(bool value)
        {
            if (SetProperty(ref _IsImageBackgroundChecked, value, nameof(IsImageBackgroundChecked)) && value)
            {
                IsNoBackgroundChecked = false;
                SetBackgroundMode();
            }
        }

        private void SetIsNoBackgroundCheckedProperty(bool value)
        {
            if (SetProperty(ref _IsNoBackgroundChecked, value, nameof(IsNoBackgroundChecked)) && value)
            {
                IsImageBackgroundChecked = false;
                SetBackgroundMode();
            }
        }

        private void SetIsCustomImageBackgroundSelectedProperty(bool value)
        {
            if (SetProperty(ref _IsCustomImageBackgroundSelected, value, nameof(IsCustomImageBackgroundSelected)) && value)
            {
                SelectedItem = null;
            }
        }

        private void SetBackgroundMode()
        {
            if (IsNoBackgroundChecked)
            {
                ChatBackgroundHelper.INSTANCE.BackgroundMode = ChatBackgroundMode.NONE;
            }
            else if (IsCustomImageBackgroundSelected)
            {
                ChatBackgroundHelper.INSTANCE.BackgroundMode = ChatBackgroundMode.CUSTOM_IMAGE;
            }
            else if (!(SelectedItem is null))
            {
                if (string.Equals(SelectedItem.Path, SPLASH_SCREEN_IMAGE_PATH))
                {
                    ChatBackgroundHelper.INSTANCE.BackgroundMode = ChatBackgroundMode.SPLASH_IMAGE;
                }
                else
                {
                    ChatBackgroundHelper.INSTANCE.BackgroundMode = ChatBackgroundMode.IMAGE;
                }
            }
        }

        #endregion
        //--------------------------------------------------------Misc Methods:---------------------------------------------------------------\\
        #region --Misc Methods (Public)--


        #endregion

        #region --Misc Methods (Private)--
        private void LoadSettings()
        {
            switch (ChatBackgroundHelper.INSTANCE.BackgroundMode)
            {
                case ChatBackgroundMode.SPLASH_IMAGE:
                case ChatBackgroundMode.IMAGE:
                    IsImageBackgroundChecked = true;
                    break;

                case ChatBackgroundMode.CUSTOM_IMAGE:
                    IsImageBackgroundChecked = true;
                    IsCustomImageBackgroundSelected = true;
                    break;

                case ChatBackgroundMode.NONE:
                    IsNoBackgroundChecked = true;
                    break;
            }
        }

        private void LoadImages()
        {
            Task.Run(async () =>
            {
                string curBackgroundImagePath = Settings.getSettingString(SettingsConsts.CHAT_BACKGROUND_IMAGE_PATH, SPLASH_SCREEN_IMAGE_PATH);

                IMAGES.Clear();
                AddImage(new BackgroundImageSelectionControlItemDataTemplate
                {
                    Path = SPLASH_SCREEN_IMAGE_PATH
                }, curBackgroundImagePath);

                StorageFolder picturesFolder = await Windows.ApplicationModel.Package.Current.InstalledLocation.GetFolderAsync(@"Assets\BackgroundImages");
                foreach (StorageFile file in await picturesFolder.GetFilesAsync())
                {
                    AddImage(new BackgroundImageSelectionControlItemDataTemplate
                    {
                        Path = file.Path
                    }, curBackgroundImagePath);
                }
            });
        }

        private void AddImage(BackgroundImageSelectionControlItemDataTemplate img, string curBackgroundImagePath)
        {
            IMAGES.Add(img);

            if (ChatBackgroundHelper.INSTANCE.BackgroundMode != ChatBackgroundMode.CUSTOM_IMAGE && string.Equals(curBackgroundImagePath, img.Path))
            {
                SelectedItem = img;
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
