using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;

namespace UWP_XMPP_Client.Controls
{
    public sealed partial class BackgroundImageSelectionControl : UserControl
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        public string ImagePath
        {
            get { return (string)GetValue(ImagePathProperty); }
            set
            {
                SetValue(ImagePathProperty, value);
                showImage();
            }
        }
        public static readonly DependencyProperty ImagePathProperty = DependencyProperty.Register("ImagePath", typeof(string), typeof(BackgroundImageSelectionControl), null);

        public bool Selected
        {
            get { return (bool)GetValue(SelectedProperty); }
            set
            {
                SetValue(SelectedProperty, value);
                showSelected();
            }
        }
        public static readonly DependencyProperty SelectedProperty = DependencyProperty.Register("Selected", typeof(bool), typeof(BackgroundImageSelectionControl), null);



        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        /// <summary>
        /// Basic Constructor
        /// </summary>
        /// <history>
        /// 16/09/2017 Created [Fabian Sauter]
        /// </history>
        public BackgroundImageSelectionControl()
        {
            this.InitializeComponent();
        }

        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--


        #endregion
        //--------------------------------------------------------Misc Methods:---------------------------------------------------------------\\
        #region --Misc Methods (Public)--


        #endregion

        #region --Misc Methods (Private)--
        private void showSelected()
        {
            if (Selected)
            {
                imageBorder_brdr.BorderBrush = new SolidColorBrush((Color)Application.Current.Resources["SystemAccentColor"]);
                check_rltvp.Visibility = Visibility.Visible;
            }
            else
            {
                imageBorder_brdr.BorderBrush = (SolidColorBrush)Application.Current.Resources["SystemControlBackgroundBaseMediumBrush"];
                check_rltvp.Visibility = Visibility.Collapsed;
            }
        }

        private void showImage()
        {
            if (ImagePath is null)
            {
                return;
            }
            image_img.Source = new BitmapImage(new System.Uri(ImagePath));
        }

        #endregion

        #region --Misc Methods (Protected)--


        #endregion
        //--------------------------------------------------------Events:---------------------------------------------------------------------\\
        #region --Events--


        #endregion
    }
}
