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
        public readonly CustomObservableCollection<SingleEmoji> EMOJI_RECENT;
        public readonly AdvancedCollectionView EMOJI_SMILEYS_FILTERED;
        public readonly AdvancedCollectionView EMOJI_PEOPLE_FILTERED;
        public readonly AdvancedCollectionView EMOJI_FOOD_FILTERED;
        public readonly AdvancedCollectionView EMOJI_OBJECTS_FILTERED;
        public readonly AdvancedCollectionView EMOJI_SYMBOLS_FILTERED;
        public readonly AdvancedCollectionView EMOJI_TRANSPORTATIONS_FILTERED;

        private object _SelectedList;
        public object SelectedList
        {
            get { return _SelectedList; }
            set { SetProperty(ref _SelectedList, value); }
        }

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
            EMOJI_SMILEYS_FILTERED = new AdvancedCollectionView
            {
                Filter = EmojiFilter
            };
            EMOJI_PEOPLE_FILTERED = new AdvancedCollectionView
            {
                Filter = EmojiFilter
            };
            EMOJI_FOOD_FILTERED = new AdvancedCollectionView
            {
                Filter = EmojiFilter
            };
            EMOJI_OBJECTS_FILTERED = new AdvancedCollectionView
            {
                Filter = EmojiFilter
            };
            EMOJI_SYMBOLS_FILTERED = new AdvancedCollectionView
            {
                Filter = EmojiFilter
            };
            EMOJI_TRANSPORTATIONS_FILTERED = new AdvancedCollectionView
            {
                Filter = EmojiFilter
            };
            IsRecentChecked = true;
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
                UpdateFilter();
            }
        }

        private void SetIsRecentCheckedProperty(bool value)
        {
            if (SetProperty(ref _IsRecentChecked, value, nameof(IsRecentChecked)) && value)
            {
                LoadRecentEmoji();
                SelectedList = EMOJI_RECENT;
            }
        }

        private void SetIsSmileysCheckedCheckedProperty(bool value)
        {
            if (SetProperty(ref _IsSmileysChecked, value, nameof(IsSmileysChecked)) && value)
            {
                LoadSmileysEmoji();
                SelectedList = EMOJI_SMILEYS_FILTERED;
            }
        }

        private void SetIsPeopleCheckedCheckedProperty(bool value)
        {
            if (SetProperty(ref _IsPeopleChecked, value, nameof(IsPeopleChecked)) && value)
            {
                LoadPeopleEmoji();
                SelectedList = EMOJI_PEOPLE_FILTERED;
            }
        }

        private void SetIsObjectsCheckedCheckedProperty(bool value)
        {
            if (SetProperty(ref _IsObjectsChecked, value, nameof(IsObjectsChecked)) && value)
            {
                LoadObjectEmoji();
                SelectedList = EMOJI_OBJECTS_FILTERED;
            }
        }

        private void SetIsFoodCheckedCheckedProperty(bool value)
        {
            if (SetProperty(ref _IsFoodChecked, value, nameof(IsFoodChecked)) && value)
            {
                LoadFoodEmoji();
                SelectedList = EMOJI_FOOD_FILTERED;
            }
        }

        private void SetIsTransportationsCheckedCheckedProperty(bool value)
        {
            if (SetProperty(ref _IsTransportationsChecked, value, nameof(IsTransportationsChecked)) && value)
            {
                LoadTransportationsEmoji();
                SelectedList = EMOJI_TRANSPORTATIONS_FILTERED;
            }
        }

        private void SetIsSymbolsCheckedCheckedProperty(bool value)
        {
            if (SetProperty(ref _IsSymbolsChecked, value, nameof(IsSymbolsChecked)) && value)
            {
                LoadSymbolEmoji();
                SelectedList = EMOJI_SYMBOLS_FILTERED;
            }
        }

        #endregion
        //--------------------------------------------------------Misc Methods:---------------------------------------------------------------\\
        #region --Misc Methods (Public)--


        #endregion

        #region --Misc Methods (Private)--
        private void UpdateFilter()
        {
            IsLoading = true;
            if (IsSmileysChecked)
            {
                EMOJI_SMILEYS_FILTERED.RefreshFilter();
            }
            else if (IsPeopleChecked)
            {
                EMOJI_PEOPLE_FILTERED.RefreshFilter();
            }
            else if (IsObjectsChecked)
            {
                EMOJI_OBJECTS_FILTERED.RefreshFilter();
            }
            else if (IsSymbolsChecked)
            {
                EMOJI_SYMBOLS_FILTERED.RefreshFilter();
            }
            else if (IsFoodChecked)
            {
                EMOJI_FOOD_FILTERED.RefreshFilter();
            }
            else if (IsTransportationsChecked)
            {
                EMOJI_TRANSPORTATIONS_FILTERED.RefreshFilter();
            }
            IsLoading = false;
        }

        private bool EmojiFilter(object o)
        {
            return o is SingleEmoji emoji && (string.IsNullOrEmpty(EmojiQuery) || emoji.SearchTerms.Any((x) => x.ToLower().Contains(EmojiQuery)));
        }

        private void LoadRecentEmoji()
        {

        }

        private void LoadSmileysEmoji()
        {
            Task.Run(async () =>
            {
                IsLoading = true;
                if (EMOJI_SMILEYS_FILTERED.Source.Count <= 0)
                {
                    List<SingleEmoji> emoji = Emoji.SmileysAndEmotion.Union(Emoji.AnimalsAndNature).Where((x) => x.HasGlyph).ToList();
                    await SharedUtils.CallDispatcherAsync(() => EMOJI_SMILEYS_FILTERED.Source = emoji);
                }
                EMOJI_SMILEYS_FILTERED.RefreshFilter();
                IsLoading = false;
            });
        }

        private void LoadPeopleEmoji()
        {
            Task.Run(async () =>
            {
                IsLoading = true;
                if (EMOJI_PEOPLE_FILTERED.Source.Count <= 0)
                {
                    List<SingleEmoji> emoji = Emoji.PeopleAndBody.Union(Emoji.Activities).Union(Emoji.Component).Where((x) => x.HasGlyph).ToList();
                    await SharedUtils.CallDispatcherAsync(() => EMOJI_PEOPLE_FILTERED.Source = emoji);
                }
                EMOJI_PEOPLE_FILTERED.RefreshFilter();
                IsLoading = false;
            });
        }

        private void LoadObjectEmoji()
        {
            Task.Run(async () =>
            {
                IsLoading = true;
                if (EMOJI_OBJECTS_FILTERED.Source.Count <= 0)
                {
                    List<SingleEmoji> emoji = Emoji.Objects.Union(Emoji.Flags).Where((x) => x.HasGlyph).ToList();
                    await SharedUtils.CallDispatcherAsync(() => EMOJI_OBJECTS_FILTERED.Source = emoji);
                }
                EMOJI_OBJECTS_FILTERED.RefreshFilter();
                IsLoading = false;
            });
        }

        private void LoadSymbolEmoji()
        {
            Task.Run(async () =>
            {
                IsLoading = true;
                if (EMOJI_SYMBOLS_FILTERED.Source.Count <= 0)
                {
                    List<SingleEmoji> emoji = Emoji.Symbols.Where((x) => x.HasGlyph).ToList();
                    await SharedUtils.CallDispatcherAsync(() => EMOJI_SYMBOLS_FILTERED.Source = emoji);
                }
                EMOJI_SYMBOLS_FILTERED.RefreshFilter();
                IsLoading = false;
            });
        }

        private void LoadFoodEmoji()
        {
            Task.Run(async () =>
            {
                IsLoading = true;
                if (EMOJI_FOOD_FILTERED.Source.Count <= 0)
                {
                    List<SingleEmoji> emoji = Emoji.FoodAndDrink.Union(Emoji.AnimalsAndNature).Where((x) => x.HasGlyph).ToList();
                    await SharedUtils.CallDispatcherAsync(() => EMOJI_FOOD_FILTERED.Source = emoji);
                }
                EMOJI_FOOD_FILTERED.RefreshFilter();
                IsLoading = false;
            });
        }

        private void LoadTransportationsEmoji()
        {
            Task.Run(async () =>
            {
                IsLoading = true;
                if (EMOJI_TRANSPORTATIONS_FILTERED.Source.Count <= 0)
                {
                    List<SingleEmoji> emoji = Emoji.TravelAndPlaces.Where((x) => x.HasGlyph).ToList();
                    await SharedUtils.CallDispatcherAsync(() => EMOJI_TRANSPORTATIONS_FILTERED.Source = emoji);
                }
                EMOJI_TRANSPORTATIONS_FILTERED.RefreshFilter();
                IsLoading = false;
            });
        }

        #endregion

        #region --Misc Methods (Protected)--


        #endregion
        //--------------------------------------------------------Events:---------------------------------------------------------------------\\
        #region --Events--


        #endregion
    }
}
