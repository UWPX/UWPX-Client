using NeoSmart.Unicode;
using UWPX_UI.Classes.Events;
using UWPX_UI_Context.Classes.DataContext.Controls;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;

namespace UWPX_UI.Controls.Chat
{
    public sealed partial class EmojiPickerControl : UserControl
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
            this.InitializeComponent();
            GoToVisualState("Recent");
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

        #endregion

        #region --Misc Methods (Protected)--


        #endregion
        //--------------------------------------------------------Events:---------------------------------------------------------------------\\
        #region --Events--
        private void Emojis_grid_ItemClick(object sender, ItemClickEventArgs e)
        {
            if (e.ClickedItem is SingleEmoji emoji)
            {
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

        #endregion
    }
}
