using Data_Manager2.Classes;
using Logging;
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
        public readonly CustomObservableCollection<BackgroundImageSelectionControlItemDataTemplate> IMAGES = new CustomObservableCollection<BackgroundImageSelectionControlItemDataTemplate>(true);

        private BackgroundImageSelectionControlItemDataTemplate _SelectedItem;
        public BackgroundImageSelectionControlItemDataTemplate SelectedItem
        {
            get { return _SelectedItem; }
            set { SetSelectedItem(value); }
        }

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        public BackgroundImageSelectionControlDataTemplate()
        {
            LoadImages();
        }

        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--
        private void SetSelectedItem(BackgroundImageSelectionControlItemDataTemplate value)
        {
            if (SetProperty(ref _SelectedItem, value, nameof(SelectedItem)) && !(value is null))
            {
                ChatBackgroundHelper.INSTANCE.ImagePath = value.Path;
            }
        }

        #endregion
        //--------------------------------------------------------Misc Methods:---------------------------------------------------------------\\
        #region --Misc Methods (Public)--


        #endregion

        #region --Misc Methods (Private)--
        private void LoadImages()
        {
            Task.Run(async () =>
            {
                string curBackgroundImagePath = Settings.getSettingString(SettingsConsts.CHAT_BACKGROUND_IMAGE_PATH);
                StorageFolder picturesFolder = await Windows.ApplicationModel.Package.Current.InstalledLocation.GetFolderAsync(@"Assets\BackgroundImages");
                foreach (StorageFile file in await picturesFolder.GetFilesAsync())
                {
                    try
                    {
                        BackgroundImageSelectionControlItemDataTemplate img = new BackgroundImageSelectionControlItemDataTemplate
                        {
                            Path = file.Path
                        };

                        IMAGES.Add(img);

                        if (string.Equals(curBackgroundImagePath, img.Path))
                        {
                            SelectedItem = img;
                        }
                    }
                    catch (Exception e)
                    {
                        Logger.Error("Error during loading background image: " + file.Path, e);
                    }
                }
            });
        }

        #endregion

        #region --Misc Methods (Protected)--


        #endregion
        //--------------------------------------------------------Events:---------------------------------------------------------------------\\
        #region --Events--


        #endregion
    }
}
