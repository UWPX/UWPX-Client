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
        public static bool loaded;

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--


        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--


        #endregion
        //--------------------------------------------------------Misc Methods:---------------------------------------------------------------\\
        #region --Misc Methods (Public)--
        public static void loadCache()
        {
            if (loaded)
            {
                return;
            }

            Task.Factory.StartNew(async () =>
            {
                Logger.Info("Started loading background images...");
                DateTime timeStart = DateTime.Now;
                try
                {
                    string selectedImageName = Settings.getSettingString(SettingsConsts.CHAT_BACKGROUND_IMAGE_NAME);
                    backgroundImages = new CustomObservableCollection<BackgroundImageTemplate>();
                    selectedImage = null;
                    ImageCache.Instance.MaxMemoryCacheCount = 100;
                    StorageFolder picturesFolder = await Windows.ApplicationModel.Package.Current.InstalledLocation.GetFolderAsync(@"Assets\BackgroundImages");
                    foreach (StorageFile file in await picturesFolder.GetFilesAsync())
                    {
                        try
                        {
                            Uri imgUri = new Uri(file.Path);
                            /*await ImageCache.Instance.PreCacheAsync(imgUri, true, true);
                            BitmapImage img = await ImageCache.Instance.GetFromCacheAsync(imgUri);
                            if(img == null)
                            {
                                continue;
                            }*/
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
            });
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
