using System;
using System.Threading.Tasks;
using Logging;
using UWPX_UI_Context.Classes.DataTemplates.Dialogs;
using Windows.Storage.Pickers;
using Windows.UI.Xaml.Media.Imaging;

namespace UWPX_UI_Context.Classes.DataContext.Dialogs
{
    public class EditProfileDialogContext
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        public readonly EditProfileDialogDataTemplate MODEL = new EditProfileDialogDataTemplate();

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--


        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--


        #endregion
        //--------------------------------------------------------Misc Methods:---------------------------------------------------------------\\
        #region --Misc Methods (Public)--
        public async Task ChangeAvatarAsync()
        {
            MODEL.IsLoading = true;
            FileOpenPicker picker = new FileOpenPicker
            {
                ViewMode = PickerViewMode.Thumbnail,
                SuggestedStartLocation = PickerLocationId.PicturesLibrary
            };
            picker.FileTypeFilter.Add(".jpg");
            picker.FileTypeFilter.Add(".jpeg");
            picker.FileTypeFilter.Add(".png");
            picker.FileTypeFilter.Add(".gif");
            picker.FileTypeFilter.Add(".svg");
            picker.SuggestedStartLocation = PickerLocationId.PicturesLibrary;

            Windows.Storage.StorageFile file = await picker.PickSingleFileAsync();
            if (file is not null)
            {
                Logger.Debug($"New avatar: " + file.DisplayName);
                MODEL.Image = new BitmapImage(new Uri(file.Path));
            }
            else
            {
                Logger.Debug("Changing avatar canceled.");
            }
            MODEL.IsLoading = false;
        }

        public async Task RemoveAvatarAsync()
        {
            MODEL.Image = null;
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
