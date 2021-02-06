using System;
using System.Threading.Tasks;
using Shared.Classes;
using Shared.Classes.Collections;
using Windows.Storage;

namespace UWPX_UI_Context.Classes.DataTemplates.Controls
{
    public sealed class BackgroundImageSelectionControlDataTemplate: AbstractDataTemplate
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        private const string SPLASH_SCREEN_IMAGE_PATH = "ms-appx:///Assets/Images/SplashScreen/splash_screen_800.png";

        public readonly CustomObservableCollection<BackgroundImageSelectionControlItemDataTemplate> IMAGES = new CustomObservableCollection<BackgroundImageSelectionControlItemDataTemplate>(true);

        private BackgroundImageSelectionControlItemDataTemplate _SelectedItem;
        public BackgroundImageSelectionControlItemDataTemplate SelectedItem
        {
            get => _SelectedItem;
            set => SetSelectedItem(value);
        }

        private bool _IsNoBackgroundChecked;
        public bool IsNoBackgroundChecked
        {
            get => _IsNoBackgroundChecked;
            set => SetIsNoBackgroundCheckedProperty(value);
        }

        private bool _IsImageBackgroundChecked;
        public bool IsImageBackgroundChecked
        {
            get => _IsImageBackgroundChecked;
            set => SetIsImageBackgroundCheckedProperty(value);
        }

        private bool _IsCustomImageBackgroundSelected;
        public bool IsCustomImageBackgroundSelected
        {
            get => _IsCustomImageBackgroundSelected;
            set => SetIsCustomImageBackgroundSelectedProperty(value);
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
                ChatBackgroundHelper.INSTANCE.BackgroundMode = ChatBackgroundMode.None;
            }
            else if (IsCustomImageBackgroundSelected)
            {
                ChatBackgroundHelper.INSTANCE.BackgroundMode = ChatBackgroundMode.CustomImage;
            }
            else if (!(SelectedItem is null))
            {
                ChatBackgroundHelper.INSTANCE.BackgroundMode = string.Equals(SelectedItem.Path, SPLASH_SCREEN_IMAGE_PATH) ? ChatBackgroundMode.SplashImage : ChatBackgroundMode.Image;
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
                case ChatBackgroundMode.SplashImage:
                case ChatBackgroundMode.Image:
                    IsImageBackgroundChecked = true;
                    break;

                case ChatBackgroundMode.CustomImage:
                    IsImageBackgroundChecked = true;
                    IsCustomImageBackgroundSelected = true;
                    break;

                case ChatBackgroundMode.None:
                    IsNoBackgroundChecked = true;
                    break;
            }
        }

        private void LoadImages()
        {
            Task.Run(async () =>
            {
                string curBackgroundImagePath = Settings.GetSettingString(SettingsConsts.CHAT_BACKGROUND_IMAGE_PATH, SPLASH_SCREEN_IMAGE_PATH);

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

            if (ChatBackgroundHelper.INSTANCE.BackgroundMode != ChatBackgroundMode.CustomImage && string.Equals(curBackgroundImagePath, img.Path))
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
