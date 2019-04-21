using NeoSmart.Unicode;
using UWPX_UI.Classes.Events;
using UWPX_UI_Context.Classes.DataContext.Controls;
using UWPX_UI_Context.Classes.DataTemplates.Controls;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;

namespace UWPX_UI.Controls.Chat
{
    public sealed partial class EmojiPickerControl: UserControl
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        public readonly EmojiPickerControlContext VIEW_MODEL = new EmojiPickerControlContext();

        public delegate void EmojiSelectedHandler(EmojiPickerControl sender, EmojiSelectedEventArgs args);
        public event EmojiSelectedHandler EmojiSelected;

        private string nextVisualState;

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        public EmojiPickerControl()
        {
            InitializeComponent();
            GoToVisualState("Recent");
            emoji_gridv.Items.VectorChanged += Items_VectorChanged;
            VIEW_MODEL.MODEL.PropertyChanged += MODEL_PropertyChanged;
            UpdateNonFoundVisibility();
        }

        private void MODEL_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (string.Equals(e.PropertyName, nameof(EmojiPickerControlDataTemplate.IsLoading)))
            {
                UpdateLayout();
            }
        }

        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--


        #endregion
        //--------------------------------------------------------Misc Methods:---------------------------------------------------------------\\
        #region --Misc Methods (Public)--


        #endregion

        #region --Misc Methods (Private)--
        private void GoToVisualState(string state)
        {
            nextVisualState = state;
            VisualStateManager.GoToState(this, state, true);
        }

        private void UpdateNonFoundVisibility()
        {
            nonFound_stck.Visibility = emoji_gridv.Items.Count <= 0 && !VIEW_MODEL.MODEL.IsLoading ? Visibility.Visible : Visibility.Collapsed;
        }

        #endregion

        #region --Misc Methods (Protected)--


        #endregion
        //--------------------------------------------------------Events:---------------------------------------------------------------------\\
        #region --Events--
        private void Emojis_grid_ItemClick(object sender, ItemClickEventArgs e)
        {
            if (e.ClickedItem is SingleEmoji emoji)
            {
                VIEW_MODEL.OnEmojiClicked(emoji);
                EmojiSelected?.Invoke(this, new EmojiSelectedEventArgs(emoji));
            }
        }

        private void EmojiGroup_tglbtn_Unchecked(object sender, RoutedEventArgs e)
        {
            if (sender is ToggleButton btn && btn.Tag is string state)
            {
                if (string.Equals(nextVisualState, state))
                {
                    GoToVisualState("Dummy");
                    GoToVisualState(state);
                }
            }
        }

        private void EmojiGroup_tglbtn_Checked(object sender, RoutedEventArgs e)
        {
            if (sender is ToggleButton btn && btn.Tag is string state)
            {
                GoToVisualState(state);
            }
        }

        private void Items_VectorChanged(Windows.Foundation.Collections.IObservableVector<object> sender, Windows.Foundation.Collections.IVectorChangedEventArgs @event)
        {
            UpdateNonFoundVisibility();
        }

        private void SkinToneRadioButton_Checked(object sender, RoutedEventArgs e)
        {
            if (sender is RadioButton radioButton)
            {
                switch (radioButton.Tag)
                {

                    case "0":
                        VIEW_MODEL.OnSkinToneSelected(Emoji.ZeroWidthJoiner);
                        break;

                    case "1":
                        VIEW_MODEL.OnSkinToneSelected(Emoji.SkinTones.Light);
                        break;

                    case "2":
                        VIEW_MODEL.OnSkinToneSelected(Emoji.SkinTones.MediumLight);
                        break;

                    case "3":
                        VIEW_MODEL.OnSkinToneSelected(Emoji.SkinTones.Medium);
                        break;

                    case "4":
                        VIEW_MODEL.OnSkinToneSelected(Emoji.SkinTones.MediumDark);
                        break;

                    case "5":
                        VIEW_MODEL.OnSkinToneSelected(Emoji.SkinTones.Dark);
                        break;
                }
            }
        }

        #endregion
    }
}
