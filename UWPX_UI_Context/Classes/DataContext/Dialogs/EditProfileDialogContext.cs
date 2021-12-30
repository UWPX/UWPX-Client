using System;
using System.Threading.Tasks;
using Logging;
using Shared.Classes.Image;
using Storage.Classes.Contexts;
using Storage.Classes.Models.Account;
using UWPX_UI_Context.Classes.DataTemplates.Dialogs;
using Windows.Graphics.Imaging;
using Windows.Storage;
using Windows.Storage.FileProperties;
using Windows.Storage.Pickers;
using Windows.UI.Xaml.Media.Imaging;
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
        private const ulong ONE_MEGABYTE = 1 * 1024 * 1024;

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
            MODEL.Error = false;
            FileOpenPicker picker = new FileOpenPicker
            {
                ViewMode = PickerViewMode.Thumbnail,
                SuggestedStartLocation = PickerLocationId.PicturesLibrary
            };
            picker.FileTypeFilter.Add(".jpg");
            picker.FileTypeFilter.Add(".jpeg");
            picker.FileTypeFilter.Add(".png");
            picker.FileTypeFilter.Add(".gif");
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
            MODEL.Error = false;
        }

        public async Task<bool> SaveAsync()
        {
            DateTime oldAvatarLastUpdate = MODEL.Client.dbAccount.contactInfo.lastAvatarUpdate;
            ImageModel oldAvatar = UpdateDB();

            if (!await PublishAvatarAsync())
            {
                MODEL.Error = true;
                MODEL.ErrorText = "Failed to publish avatar. Reverting DB...";

                ContactInfoModel contactInfo = MODEL.Client.dbAccount.contactInfo;
                if (oldAvatar != contactInfo.avatar)
                {
                    using (MainDbContext ctx = new MainDbContext())
                    {
                        if (contactInfo.avatar is not null)
                        {
                            contactInfo.avatar.Remove(ctx, true);
                        }
                        if (oldAvatar is not null)
                        {
                            ctx.Add(oldAvatar);
                        }
                        contactInfo.avatar = oldAvatar;
                        contactInfo.lastAvatarUpdate = oldAvatarLastUpdate;
                        ctx.Update(contactInfo);
                    }
                }
                return false;
            }
            return true;
        }

        #endregion

        #region --Misc Methods (Private)--
        private async Task LoadImageAsync(StorageFile file)
        {
            BasicProperties properties = await file.GetBasicPropertiesAsync();
            if (properties.Size > ONE_MEGABYTE)
            {
                MODEL.Error = true;
                MODEL.ErrorText = "Image to large! It has to be less than 1 MiB.";
                return;
            }

            try
            {
                SoftwareBitmap img = await ImageUtils.LoadImageAsync(file, 512, 512);
                if (img is not null)
                {
                    SoftwareBitmapSource src = new SoftwareBitmapSource();
                    await src.SetBitmapAsync(img);
                    await MODEL.SetImageAsync(img, string.Equals(file.ContentType, ImageUtils.IANA_MEDIA_TYPE_GIF));
                    MODEL.IsSaveEnabled = true;
                }
            }
            catch (Exception e)
            {
                MODEL.Error = true;
                MODEL.ErrorText = "Failed to open image.";
                Logger.Error($"Failed to load image from '{file.Path}'.", e);
            }
        }

        private ImageModel UpdateDB()
        {
            ContactInfoModel contactInfo = MODEL.Client.dbAccount.contactInfo;
            ImageModel oldAvatar = contactInfo.avatar;
            if (contactInfo.avatar != MODEL.Image)
            {
                using (MainDbContext ctx = new MainDbContext())
                {
                    if (oldAvatar is not null)
                    {
                        ctx.Remove(oldAvatar);
                    }
                    contactInfo.avatar = null;
                    contactInfo.lastAvatarUpdate = DateTime.Now;
                    ctx.Update(contactInfo);
                }

                if (MODEL.Image is not null)
                {
                    ImageModel img = new ImageModel
                    {
                        data = MODEL.Image.data,
                        hash = MODEL.Image.hash,
                        type = MODEL.Image.type
                    };
                    using (MainDbContext ctx = new MainDbContext())
                    {
                        ctx.Add(img);
                        contactInfo.avatar = img;
                        ctx.Update(contactInfo);
                    }
                }
            }

            return oldAvatar;
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
            string imgHashBase16 = ImageUtils.HashImage(imgData);

            AvatarDataPubSubItem avatar = new AvatarDataPubSubItem(Convert.ToBase64String(imgData), imgHashBase16);
            if (!await PublishAvatarAsync(avatar))
            {
                return false;
            }

            AvatarMetadataDataPubSubItem metadata = new AvatarMetadataDataPubSubItem(imgHashBase16, new AvatarInfo((uint)imgData.Length, (ushort)img.PixelHeight, (ushort)img.PixelWidth, imgHashBase16, "image/png"));
            return await PublishMetadataAsync(metadata);
        }

        #endregion

        #region --Misc Methods (Protected)--


        #endregion
        //--------------------------------------------------------Events:---------------------------------------------------------------------\\
        #region --Events--


        #endregion
    }
}
