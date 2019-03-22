using NeoSmart.Unicode;
using Shared.Classes;
using Shared.Classes.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UWPX_UI_Context.Classes.Collections.Toolkit;

namespace UWPX_UI_Context.Classes.DataTemplates.Controls
{
    public sealed class EmojiPickerControlDataTemplate : AbstractDataTemplate
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        public readonly CustomObservableCollection<EmojiPickerItemDataTemplate> EMOJIS = new CustomObservableCollection<EmojiPickerItemDataTemplate>(true);
        public readonly AdvancedCollectionView EMOJIS_FILTERED;

        private bool _IsLoading;
        public bool IsLoading
        {
            get { return _IsLoading; }
            set { SetProperty(ref _IsLoading, value); }
        }

        private string _EmojiQuery;
        public string EmojiQuery
        {
            get { return _EmojiQuery; }
            set { SetEmojiQueryProperty(value); }
        }

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        public EmojiPickerControlDataTemplate()
        {
            EMOJIS_FILTERED = new AdvancedCollectionView(EMOJIS)
            {
                Filter = EmojiFilter
            };
            LoadEmojis();
        }

        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--
        private void SetEmojiQueryProperty(string value)
        {
            if (!(value is null))
            {
                value = value.ToLower();
            }
            if (SetProperty(ref _EmojiQuery, value, nameof(EmojiQuery)))
            {
                IsLoading = true;
                EMOJIS_FILTERED.RefreshFilter();
                IsLoading = false;
            }
        }

        #endregion
        //--------------------------------------------------------Misc Methods:---------------------------------------------------------------\\
        #region --Misc Methods (Public)--


        #endregion

        #region --Misc Methods (Private)--
        private void LoadEmojis()
        {
            Task.Run(() =>
            {
                IsLoading = true;
                List<EmojiPickerItemDataTemplate> emojis = new List<EmojiPickerItemDataTemplate>();
                foreach (SingleEmoji emoji in Emoji.All)
                {
                    emojis.Add(new EmojiPickerItemDataTemplate
                    {
                        Emoji = emoji,
                        EmojiString = emoji.ToString()
                    });
                }
                EMOJIS.Clear();
                EMOJIS.AddRange(emojis);
                IsLoading = false;
            });
        }

        private bool EmojiFilter(object o)
        {
            return o is EmojiPickerItemDataTemplate emoji && (string.IsNullOrEmpty(EmojiQuery) || emoji.Emoji.SearchTerms.Any((x) => x.ToLower().Contains(EmojiQuery)));
        }

        #endregion

        #region --Misc Methods (Protected)--


        #endregion
        //--------------------------------------------------------Events:---------------------------------------------------------------------\\
        #region --Events--


        #endregion
    }
}
