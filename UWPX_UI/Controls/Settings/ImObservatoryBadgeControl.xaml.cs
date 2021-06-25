using System;
using UWPX_UI_Context.Classes;
using UWPX_UI_Context.Classes.DataContext.Controls;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace UWPX_UI.Controls.Settings
{
    public sealed partial class ImObservatoryBadgeControl: UserControl
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        public readonly ImObservatoryBadgeControlContext VIEW_MODEL = new ImObservatoryBadgeControlContext();

        public string Rating
        {
            get => (string)GetValue(RatingProperty);
            set => SetValue(RatingProperty, value);
        }
        public static readonly DependencyProperty RatingProperty = DependencyProperty.Register(nameof(Rating), typeof(string), typeof(ImObservatoryBadgeControl), new PropertyMetadata(null, OnRatingChanged));

        public string Domain
        {
            get => (string)GetValue(DomainProperty);
            set => SetValue(DomainProperty, value);
        }
        public static readonly DependencyProperty DomainProperty = DependencyProperty.Register(nameof(Domain), typeof(string), typeof(ImObservatoryBadgeControl), new PropertyMetadata(null));

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        public ImObservatoryBadgeControl()
        {
            InitializeComponent();
        }

        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--


        #endregion
        //--------------------------------------------------------Misc Methods:---------------------------------------------------------------\\
        #region --Misc Methods (Public)--


        #endregion

        #region --Misc Methods (Private)--
        private void UpdateView(string rating)
        {
            VIEW_MODEL.UpdateView(rating);
        }

        #endregion

        #region --Misc Methods (Protected)--


        #endregion
        //--------------------------------------------------------Events:---------------------------------------------------------------------\\
        #region --Events--
        private static void OnRatingChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is ImObservatoryBadgeControl control && e.NewValue is string rating)
            {
                control.UpdateView(rating);
            }
        }

        private async void OnButtonClicked(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(Domain))
            {
                await UiUtils.LaunchUriAsync(new Uri("https://xmpp.net/result.php?domain=" + Domain + "&type=client"));
            }
        }

        #endregion
    }
}
