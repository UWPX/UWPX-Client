using System;
using System.Threading.Tasks;
using UWP_XMPP_Client.Classes;
using UWP_XMPP_Client.DataTemplates;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.Storage.Pickers;
using Windows.Storage;
using Logging;

namespace UWP_XMPP_Client.Pages.SettingsPages
{
    public sealed partial class PersonalizeSettingsPage : Page
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        private CustomObservableCollection<BackgroundImageTemplate> backgroundImages;

        public BackgroundImageTemplate CustomBackgroundImage
        {
            get { return (BackgroundImageTemplate)GetValue(CustomBackgroundImageProperty); }
            set
            {
                SetValue(CustomBackgroundImageProperty, value);
            }
        }
        public static readonly DependencyProperty CustomBackgroundImageProperty = DependencyProperty.Register("customBackgroundImage", typeof(BackgroundImageTemplate), typeof(PersonalizeSettingsPage), null);

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        /// <summary>
        /// Basic Constructor
        /// </summary>
        /// <history>
        /// 04/09/2017 Created [Fabian Sauter]
        /// </history>
        public PersonalizeSettingsPage()
        {
            this.InitializeComponent();
            Windows.UI.Core.SystemNavigationManager.GetForCurrentView().BackRequested += AbstractBackRequestPage_BackRequested;
            this.backgroundImages = BackgroundImageCache.backgroundImages;
            this.CustomBackgroundImage = BackgroundImageCache.customBackgroundImage;
        }

        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--


        #endregion
        //--------------------------------------------------------Misc Methods:---------------------------------------------------------------\\
        #region --Misc Methods (Public)--


        #endregion

        #region --Misc Methods (Private)--
        private async Task browseBackgroundAsync()
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
                string path = await BackgroundImageCache.saveAsCustomBackgroundImageAsync(file);
                if (path != null)
                {
                    BackgroundImageCache.setCustomBackgroundImage();
                    CustomBackgroundImage = null;
                    CustomBackgroundImage = BackgroundImageCache.customBackgroundImage;
                    chatDetailsDummy_cdc.loadBackgrundImage();
                    Logger.Info("Custom background image set to: " + file.Path);
                }
                else
                {
                    showInfo("Failed to pick image!");
                    Logger.Warn("Failed to set image as background image. Path is null!");
                }
            }
            else
            {
                showInfo("Selection canceled!");
            }
        }

        private void showInfo(string text)
        {
            info_ian.Show(text, 5000);
        }

        private void setAppTheme(ElementTheme theme)
        {
            App.RootTheme = theme;
        }

        #endregion

        #region --Misc Methods (Protected)--


        #endregion
        //--------------------------------------------------------Events:---------------------------------------------------------------------\\
        #region --Events--
        private void AbstractBackRequestPage_BackRequested(object sender, Windows.UI.Core.BackRequestedEventArgs e)
        {
            Frame rootFrame = Window.Current.Content as Frame;
            if (rootFrame == null)
            {
                return;
            }
            if (rootFrame.CanGoBack && e.Handled == false)
            {
                e.Handled = true;
                rootFrame.GoBack();
            }
        }

        private void AdaptiveGridView_ItemClick(object sender, ItemClickEventArgs e)
        {
            if (e.ClickedItem is BackgroundImageTemplate)
            {
                BackgroundImageTemplate img = e.ClickedItem as BackgroundImageTemplate;
                BackgroundImageCache.setExampleBackgroundImage(img);
                chatDetailsDummy_cdc.loadBackgrundImage();
            }
        }

        private void remove_ibtn_Click(object sender, RoutedEventArgs args)
        {
            BackgroundImageCache.removeBackgroundImage();
            chatDetailsDummy_cdc.loadBackgrundImage();
        }

        private async void deleteCustomImage_btn_Click(object sender, RoutedEventArgs e)
        {
            await BackgroundImageCache.deleteCustomBackgroundImage();
            CustomBackgroundImage = BackgroundImageCache.customBackgroundImage;
        }

        private void setImage_btn_Click(object sender, RoutedEventArgs e)
        {
            if (CustomBackgroundImage != null && !CustomBackgroundImage.selected)
            {
                BackgroundImageCache.setCustomBackgroundImage();
                chatDetailsDummy_cdc.loadBackgrundImage();
            }
        }

        private async void browseImage_btn_Click(object sender, RoutedEventArgs e)
        {
            await browseBackgroundAsync();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            switch (App.RootTheme)
            {
                case ElementTheme.Default:
                    systemTheme_rbtn.IsChecked = true;
                    break;

                case ElementTheme.Light:
                    lightTheme_rbtn.IsChecked = true;

                    break;
                case ElementTheme.Dark:
                    darkTheme_rbtn.IsChecked = true;
                    break;
            }
        }

        private void lightTheme_rbtn_Checked(object sender, RoutedEventArgs e)
        {
            setAppTheme(ElementTheme.Light);
        }

        private void darkTheme_rbtn_Checked(object sender, RoutedEventArgs e)
        {
            setAppTheme(ElementTheme.Dark);
        }

        private void systemTheme_rbtn_Checked(object sender, RoutedEventArgs e)
        {
            setAppTheme(ElementTheme.Default);
        }

        #endregion
    }
}
