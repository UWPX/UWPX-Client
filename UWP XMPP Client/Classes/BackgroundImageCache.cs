using Data_Manager2.Classes;
using Logging;
using Microsoft.Toolkit.Uwp.UI;
using System;
using System.Threading.Tasks;
using UWP_XMPP_Client.DataTemplates;
using Windows.Storage;

namespace UWP_XMPP_Client.Classes
{
    class BackgroundImageCache
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        public static CustomObservableCollection<BackgroundImageTemplate> backgroundImages;
        public static BackgroundImageTemplate selectedImage;
        public static BackgroundImageTemplate customBackgroundImage;
        public static bool loaded;

        public const byte EXAMPLE_BACKGROUND = 0;
        public const byte CUSTOM_BACKGROUND = 1;
        public const byte NONE_BACKGROUND = 2;

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--


        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--
        public static void setCustomBackgroundImage()
        {
            Settings.setSetting(SettingsConsts.CHAT_BACKGROUND_MODE, CUSTOM_BACKGROUND);
            if (selectedImage != null)
            {
                selectedImage.selected = false;
            }
            selectedImage = customBackgroundImage;
            if (customBackgroundImage != null)
            {
                customBackgroundImage.selected = true;
            }
        }

        public static void setExampleBackgroundImage(BackgroundImageTemplate img)
        {
            Settings.setSetting(SettingsConsts.CHAT_BACKGROUND_MODE, EXAMPLE_BACKGROUND);
            if (selectedImage != null)
            {
                selectedImage.selected = false;
            }
            selectedImage = img;
            img.selected = true;
        }

        #endregion
        //--------------------------------------------------------Misc Methods:---------------------------------------------------------------\\
        #region --Misc Methods (Public)--
        public static void removeBackgroundImage()
        {
            Settings.setSetting(SettingsConsts.CHAT_BACKGROUND_MODE, NONE_BACKGROUND);
            selectedImage = null;
            for (int i = 0; i < backgroundImages.Count; i++)
            {
                backgroundImages[i].selected = false;
            }

            if (customBackgroundImage != null)
            {
                customBackgroundImage.selected = false;
            }
        }

        public static async Task<string> saveAsBackgroundImageAsync(StorageFile file)
        {
            StorageFolder folder = await ApplicationData.Current.LocalFolder.CreateFolderAsync("BackgroundImage", CreationCollisionOption.OpenIfExists);
            if (folder != null)
            {
                StorageFile f = await folder.CreateFileAsync("img", CreationCollisionOption.OpenIfExists);
                if (f != null)
                {
                    await file.CopyAndReplaceAsync(f);

                    customBackgroundImage = new BackgroundImageTemplate()
                    {
                        imagePath = f.Path,
                        name = "custom",
                        selected = false
                    };
                    return f.Path;
                }
            }
            return null;
        }

        public static void loadCache()
        {
            if (loaded)
            {
                return;
            }

            Task.WaitAny(Task.Run(async () =>
            {
                Logger.Info("Started loading background images...");
                DateTime timeStart = DateTime.Now;
                try
                {
                    try
                    {
                        StorageFolder customImagefolder = await ApplicationData.Current.LocalFolder.CreateFolderAsync("BackgroundImage", CreationCollisionOption.OpenIfExists);
                        if (customImagefolder != null)
                        {
                            StorageFile f = await customImagefolder.CreateFileAsync("img", CreationCollisionOption.OpenIfExists);
                            if (f != null)
                            {
                                customBackgroundImage = new BackgroundImageTemplate()
                                {
                                    imagePath = f.Path,
                                    name = "custom",
                                    selected = false
                                };
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        Logger.Error("Error during loading the custom background image!", e);
                    }

                    byte backgroundMode = Settings.getSettingByte(SettingsConsts.CHAT_BACKGROUND_MODE);
                    string selectedImageName = null;
                    switch (backgroundMode)
                    {
                        case CUSTOM_BACKGROUND:
                            selectedImage = customBackgroundImage;
                            break;

                        case NONE_BACKGROUND:
                            selectedImage = null;
                            break;

                        default:
                            selectedImageName = Settings.getSettingString(SettingsConsts.CHAT_BACKGROUND_IMAGE_NAME);
                            break;
                    }

                    backgroundImages = new CustomObservableCollection<BackgroundImageTemplate>();
                    ImageCache.Instance.MaxMemoryCacheCount = 100;
                    StorageFolder picturesFolder = await Windows.ApplicationModel.Package.Current.InstalledLocation.GetFolderAsync(@"Assets\BackgroundImages");
                    foreach (StorageFile file in await picturesFolder.GetFilesAsync())
                    {
                        try
                        {
                            Uri imgUri = new Uri(file.Path);
                            bool isSelectedImage = selectedImageName != null && selectedImageName.Equals(file.Name);
                            BackgroundImageTemplate bgI = new BackgroundImageTemplate
                            {
                                imagePath = file.Path,
                                name = file.Name,
                                selected = isSelectedImage
                            };

                            backgroundImages.Add(bgI);
                            if (isSelectedImage)
                            {
                                selectedImage = bgI;
                            }
                        }
                        catch (Exception e)
                        {
                            Logger.Error("Error during loading a background image!", e);
                        }
                    }
                    Logger.Info("Finished loading background images in: " + DateTime.Now.Subtract(timeStart).TotalMilliseconds + "ms.");
                }
                catch (Exception e)
                {
                    Logger.Error("Error during loading background images!", e);
                }
                loaded = true;
            }));
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
