using NeoSmart.Unicode;
using Shared.Classes;

namespace UWPX_UI_Context.Classes.DataTemplates.Controls
{
    public sealed class EmojiPickerItemDataTemplate : AbstractDataTemplate
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        private SingleEmoji _Emoji;
        public SingleEmoji Emoji
        {
            get { return _Emoji; }
            set { SetProperty(ref _Emoji, value); }
        }

        private string _EmojiString;
        public string EmojiString
        {
            get { return _EmojiString; }
            set { SetProperty(ref _EmojiString, value); }
        }

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--


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


        #endregion
    }
}
