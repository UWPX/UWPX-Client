using System;
using System.Threading.Tasks;
using Logging;
using Shared.Classes.Image;
using UWPX_UI_Context.Classes.DataTemplates.Dialogs;
using Windows.Graphics.Imaging;
using Windows.Storage;
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

            StorageFile file = await picker.PickSingleFileAsync();
            if (file is not null)
            {
                Logger.Debug($"New avatar: " + file.DisplayName);
                // MODEL.Image = new BitmapImage(new Uri(file.Path));
                await LoadImageAsync(file);
            }
            else
            {
                Logger.Debug("Changing avatar canceled.");
            }
            MODEL.IsLoading = false;
        }

        public void RemoveAvatar()
        {
            if(MODEL.Image is not null)
            {
                MODEL.Image = null;
                MODEL.IsSaveEnabled = MODEL.Client.dbAccount.contactInfo.avatar is not null;
            }
        }

        #endregion

        #region --Misc Methods (Private)--
        private async Task LoadImageAsync(StorageFile file)
        {
            SoftwareBitmap img = await ImageUtils.LoadImageAsync(file, 720, 720);
            if(img is not null)
            {
                SoftwareBitmapSource src = new SoftwareBitmapSource();
                await src.SetBitmapAsync(img);
                MODEL.Image = src;
                MODEL.IsSaveEnabled = true;
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
