using System;
using System.Threading.Tasks;
using Logging;
using Shared.Classes.Image;
using Storage.Classes.Contexts;
using Storage.Classes.Models.Account;
using UWPX_UI_Context.Classes.DataTemplates.Dialogs;
using Windows.Graphics.Imaging;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.UI.Xaml.Media.Imaging;
using XMPP_API.Classes.Crypto;
using XMPP_API.Classes.Network.XML.Messages;
using XMPP_API.Classes.Network.XML.Messages.Helper;
using XMPP_API.Classes.Network.XML.Messages.XEP_0084;

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
                await LoadImageAsync(file);
            }
            else
            {
                Logger.Debug("Changing avatar canceled.");
            }
            MODEL.IsLoading = false;
        }

        public async Task RemoveAvatarAsync()
        {
            if (MODEL.Image is not null)
            {
                await MODEL.SetImageAsync(null, false);
                MODEL.IsSaveEnabled = MODEL.Client.dbAccount.contactInfo.avatar is not null;
            }
        }

        public async Task<bool> SaveAsync()
        {
            if (!await PublishAvatarAsync())
            {
                MODEL.Error = true;
                MODEL.ErrorText = "Failed to publish avatar.";
                return false;
            }
            if(!UpdateDB())
            {
                MODEL.Error = true;
                MODEL.ErrorText = "Failed to save avatar to DB.";
                return false;
            }
            return true;
        }

        #endregion

        #region --Misc Methods (Private)--
        private async Task LoadImageAsync(StorageFile file)
        {
            try
            {
                SoftwareBitmap img = await ImageUtils.LoadImageAsync(file, 512, 512);
                if (img is not null)
                {
                    SoftwareBitmapSource src = new SoftwareBitmapSource();
                    await src.SetBitmapAsync(img);
                    await MODEL.SetImageAsync(img, string.Equals(file.ContentType, "image/gif"));
                    MODEL.IsSaveEnabled = true;
                }
            }
            catch (Exception e)
            {
                Logger.Error($"Failed to load image from '{file.Path}'.", e);
            }
        }

        private bool UpdateDB()
        {
            ContactInfoModel contactInfo = MODEL.Client.dbAccount.contactInfo;
            // Remove image:
            if (MODEL.Image is null)
            {
                using (MainDbContext ctx = new MainDbContext())
                {
                    contactInfo.avatar.Remove(ctx, true);
                    contactInfo.avatar = null;
                    ctx.Update(contactInfo);
                }
            }
            // Add image
            else
            {
                using (MainDbContext ctx = new MainDbContext())
                {
                    if(contactInfo.avatar is not null)
                    {
                        ctx.Remove(contactInfo.avatar);
                    }
                    contactInfo.avatar = MODEL.Image;
                    ctx.Add(contactInfo.avatar);
                    ctx.Update(contactInfo);
                }
            }

            return true;
        }

        private async Task<bool> PublishMetadataAsync(AvatarMetadataDataPubSubItem metadata)
        {
            MessageResponseHelperResult<IQMessage> result = await MODEL.Client.xmppClient.PUB_SUB_COMMAND_HELPER.publishAvatarMetadataAsync(metadata);
            if (result.STATE == MessageResponseHelperResultState.SUCCESS)
            {
                if (result.RESULT is IQErrorMessage errorMsg)
                {
                    Logger.Error($"Failed to publish avatar metadata: {errorMsg.ERROR_OBJ}");
                    return false;
                }
                Logger.Info("Successfully published avatar metadata.");
                return true;
            }
            Logger.Error($"Failed to publish avatar metadata: {result.STATE}");
            return false;
        }

        private async Task<bool> PublishAvatarAsync(AvatarDataPubSubItem avatar)
        {
            MessageResponseHelperResult<IQMessage> result = await MODEL.Client.xmppClient.PUB_SUB_COMMAND_HELPER.publishAvatarAsync(avatar);
            if (result.STATE == MessageResponseHelperResultState.SUCCESS)
            {
                if (result.RESULT is IQErrorMessage errorMsg)
                {
                    Logger.Error($"Failed to publish avatar: {errorMsg.ERROR_OBJ}");
                    return false;
                }
                Logger.Info("Successfully published avatar.");
                return true;
            }
            Logger.Error($"Failed to publish avatar: {result.STATE}");
            return false;
        }

        private async Task<bool> PublishAvatarAsync()
        {
            SoftwareBitmap img = await MODEL.Image.GetSoftwareBitmapAsync();
            if (img is null)
            {
                return await PublishMetadataAsync(null);
            }

            byte[] imgData = await ImageUtils.ToByteArrayAsync(img, string.Equals(MODEL.Image.type, ImageUtils.IANA_MEDIA_TYPE_GIF));
            byte[] imgHash = ImageUtils.HashImage(imgData);
            string imgHashBase16 = CryptoUtils.byteArrayToHexString(imgHash);
            AvatarMetadataDataPubSubItem metadata = new AvatarMetadataDataPubSubItem(imgHashBase16, new AvatarInfo((uint)imgData.Length, (ushort)img.PixelHeight, (ushort)img.PixelWidth, imgHashBase16, "image/png"));

            if(!await PublishMetadataAsync(metadata))
            {
                return false;
            }

            AvatarDataPubSubItem avatar = new AvatarDataPubSubItem(Convert.ToBase64String(imgData), imgHashBase16);
            return await PublishAvatarAsync(avatar);
        }

        #endregion

        #region --Misc Methods (Protected)--


        #endregion
        //--------------------------------------------------------Events:---------------------------------------------------------------------\\
        #region --Events--


        #endregion
    }
}
