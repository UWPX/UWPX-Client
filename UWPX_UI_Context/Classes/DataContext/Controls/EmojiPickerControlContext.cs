using System.Linq;
using Data_Manager2.Classes;
using NeoSmart.Unicode;
using UWPX_UI_Context.Classes.DataTemplates.Controls;

namespace UWPX_UI_Context.Classes.DataContext.Controls
{
    public sealed class EmojiPickerControlContext
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        public readonly EmojiPickerControlDataTemplate MODEL = new EmojiPickerControlDataTemplate();

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--


        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--


        #endregion
        //--------------------------------------------------------Misc Methods:---------------------------------------------------------------\\
        #region --Misc Methods (Public)--
        public void OnEmojiClicked(SingleEmoji emoji)
        {
            if (!MODEL.EMOJI_RECENT.Contains(emoji))
            {
                if (MODEL.EMOJI_RECENT.Count >= 32)
                {
                    MODEL.EMOJI_RECENT.RemoveAt(MODEL.EMOJI_RECENT.Count - 1);
                }
                MODEL.EMOJI_RECENT.Insert(0, emoji);
                StoreRecentEmojis();
            }
        }

        public void OnSkinToneSelected(Codepoint skinTone)
        {
            MODEL.OnSkinToneSelected(skinTone);
        }

        #endregion

        #region --Misc Methods (Private)--
        private void StoreRecentEmojis()
        {
            Settings.LOCAL_OBJECT_STORAGE_HELPER.Save(SettingsConsts.CHAT_RECENT_EMOJI, MODEL.EMOJI_RECENT.Select(x => x.SortOrder).ToArray());
        }

        #endregion

        #region --Misc Methods (Protected)--


        #endregion
        //--------------------------------------------------------Events:---------------------------------------------------------------------\\
        #region --Events--


        #endregion
    }
}
