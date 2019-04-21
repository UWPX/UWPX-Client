using System;
using System.Threading.Tasks;
using Logging;
using UWPX_UI_Context.Classes.DataTemplates.Controls;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.UI.Xaml;

namespace UWPX_UI_Context.Classes.DataContext.Controls
{
    public sealed class CustomBackgroundImageSelectionControlContext
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        public readonly CustomBackgroundImageSelectionControlDataTemplate MODEL = new CustomBackgroundImageSelectionControlDataTemplate();

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--


        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--


        #endregion
        //--------------------------------------------------------Misc Methods:---------------------------------------------------------------\\
        #region --Misc Methods (Public)--
        public async Task BrowseImageAsync()
        {
            FileOpenPicker picker = new FileOpenPicker
            {
                ViewMode = PickerViewMode.Thumbnail,
                SuggestedStartLocation = PickerLocationId.PicturesLibrary
            };
            picker.FileTypeFilter.Add(".jpg");
            picker.FileTypeFilter.Add(".jpeg");
            picker.FileTypeFilter.Add(".png");

            StorageFile file = await picker.PickSingleFileAsync();

            if (file != null)
            {
                MODEL.ImagePath = await ChatBackgroundHelper.INSTANCE.SaveAsCustomBackgroundImageAsync(file);
                if (MODEL.ImagePath is null)
                {
                    Logger.Warn("Failed to set image as background image. Path is null!");
                }
                else
                {
                    Logger.Info("Custom background image set to: " + file.Path);
                }
            }
        }

        public void OnIsSelectedChanged(DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue is bool b && b)
            {
                ChatBackgroundHelper.INSTANCE.BackgroundMode = ChatBackgroundMode.CustomImage;
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
