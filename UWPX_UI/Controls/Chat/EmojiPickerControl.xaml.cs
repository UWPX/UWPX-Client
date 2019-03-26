using NeoSmart.Unicode;
using UWPX_UI.Classes.Events;
using UWPX_UI_Context.Classes.DataContext.Controls;
using Windows.UI.Xaml.Controls;

namespace UWPX_UI.Controls.Chat
{
    public sealed partial class EmojiPickerControl : UserControl
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        public readonly EmojiPickerControlContext VIEW_MODEL = new EmojiPickerControlContext();

        public delegate void EmojiSelectedHandler(EmojiPickerControl sender, EmojiSelectedEventArgs args);
        public event EmojiSelectedHandler EmojiSelected;

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        public EmojiPickerControl()
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

        #endregion        
    }
}
