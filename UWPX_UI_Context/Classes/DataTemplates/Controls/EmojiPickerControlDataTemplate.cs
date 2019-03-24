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

        private bool _IsRecentChecked;
        public bool IsRecentChecked
        {
            get { return _IsRecentChecked; }
            set { SetIsRecentCheckedProperty(value); }
        }
        private bool _IsSmileysChecked;
        public bool IsSmileysChecked
        {
            get { return _IsSmileysChecked; }
            set { SetIsSmileysCheckedCheckedProperty(value); }
        }
        private bool _IsPeopleChecked;
        public bool IsPeopleChecked
        {
            get { return _IsPeopleChecked; }
            set { SetIsPeopleCheckedCheckedProperty(value); }
        }
        private bool _IsObjectsChecked;
        public bool IsObjectsChecked
        {
            get { return _IsObjectsChecked; }
            set { SetIsObjectsCheckedCheckedProperty(value); }
        }
        private bool _IsFoodChecked;
        public bool IsFoodChecked
        {
            get { return _IsFoodChecked; }
            set { SetIsFoodCheckedCheckedProperty(value); }
        }
        private bool _IsTransportationsChecked;
        public bool IsTransportationsChecked
        {
            get { return _IsTransportationsChecked; }
            set { SetIsTransportationsCheckedCheckedProperty(value); }
        }
        private bool _IsSymbolsChecked;
        public bool IsSymbolsChecked
        {
            get { return _IsSymbolsChecked; }
            set { SetIsSymbolsCheckedCheckedProperty(value); }
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
            IsRecentChecked = true;

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

        private void SetIsRecentCheckedProperty(bool value)
        {
            if (SetProperty(ref _IsRecentChecked, value, nameof(IsRecentChecked)) && value)
            {
                IsSmileysChecked = false;
                IsPeopleChecked = false;
                IsObjectsChecked = false;
                IsFoodChecked = false;
                IsTransportationsChecked = false;
                IsSymbolsChecked = false;
            }
        }

        private void SetIsSmileysCheckedCheckedProperty(bool value)
        {
            if (SetProperty(ref _IsSmileysChecked, value, nameof(IsSmileysChecked)) && value)
            {
                IsRecentChecked = false;
                IsPeopleChecked = false;
                IsObjectsChecked = false;
                IsFoodChecked = false;
                IsTransportationsChecked = false;
                IsSymbolsChecked = false;
            }
        }

        private void SetIsPeopleCheckedCheckedProperty(bool value)
        {
            if (SetProperty(ref _IsPeopleChecked, value, nameof(IsPeopleChecked)) && value)
            {
                IsRecentChecked = false;
                IsSmileysChecked = false;
                IsObjectsChecked = false;
                IsFoodChecked = false;
                IsTransportationsChecked = false;
                IsSymbolsChecked = false;
            }
        }

        private void SetIsObjectsCheckedCheckedProperty(bool value)
        {
            if (SetProperty(ref _IsObjectsChecked, value, nameof(IsObjectsChecked)) && value)
            {
                IsRecentChecked = false;
                IsSmileysChecked = false;
                IsPeopleChecked = false;
                IsFoodChecked = false;
                IsTransportationsChecked = false;
                IsSymbolsChecked = false;
            }
        }

        private void SetIsFoodCheckedCheckedProperty(bool value)
        {
            if (SetProperty(ref _IsFoodChecked, value, nameof(IsFoodChecked)) && value)
            {
                IsRecentChecked = false;
                IsSmileysChecked = false;
                IsPeopleChecked = false;
                IsObjectsChecked = false;
                IsTransportationsChecked = false;
                IsSymbolsChecked = false;
            }
        }

        private void SetIsTransportationsCheckedCheckedProperty(bool value)
        {
            if (SetProperty(ref _IsTransportationsChecked, value, nameof(IsTransportationsChecked)) && value)
            {
                IsRecentChecked = false;
                IsSmileysChecked = false;
                IsPeopleChecked = false;
                IsObjectsChecked = false;
                IsFoodChecked = false;
                IsSymbolsChecked = false;
            }
        }

        private void SetIsSymbolsCheckedCheckedProperty(bool value)
        {
            if (SetProperty(ref _IsSymbolsChecked, value, nameof(IsSymbolsChecked)) && value)
            {
                IsRecentChecked = false;
                IsSmileysChecked = false;
                IsPeopleChecked = false;
                IsObjectsChecked = false;
                IsFoodChecked = false;
                IsTransportationsChecked = false;
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
