using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Data_Manager2.Classes;
using NeoSmart.Unicode;
using Shared.Classes;
using Shared.Classes.Collections;
using UWPX_UI_Context.Classes.Collections.Toolkit;

namespace UWPX_UI_Context.Classes.DataTemplates.Controls
{
    public sealed class EmojiPickerControlDataTemplate: AbstractDataTemplate
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        public readonly CustomObservableCollection<SingleEmoji> EMOJI_RECENT = new CustomObservableCollection<SingleEmoji>(true);
        public readonly AdvancedCollectionView EMOJI_SMILEYS_FILTERED;
        public readonly AdvancedCollectionView EMOJI_PEOPLE_FILTERED;
        public readonly AdvancedCollectionView EMOJI_FOOD_FILTERED;
        public readonly AdvancedCollectionView EMOJI_OBJECTS_FILTERED;
        public readonly AdvancedCollectionView EMOJI_SYMBOLS_FILTERED;
        public readonly AdvancedCollectionView EMOJI_TRANSPORTATIONS_FILTERED;

        private object _SelectedList;
        public object SelectedList
        {
            get => _SelectedList;
            set => SetProperty(ref _SelectedList, value);
        }

        private bool _IsLoading;
        public bool IsLoading
        {
            get => _IsLoading;
            set => SetProperty(ref _IsLoading, value);
        }

        private string _EmojiQuery;
        public string EmojiQuery
        {
            get => _EmojiQuery;
            set => SetEmojiQueryProperty(value);
        }

        private bool _IsRecentChecked;
        public bool IsRecentChecked
        {
            get => _IsRecentChecked;
            set => SetIsRecentCheckedProperty(value);
        }
        private bool _IsSmileysChecked;
        public bool IsSmileysChecked
        {
            get => _IsSmileysChecked;
            set => SetIsSmileysCheckedCheckedProperty(value);
        }
        private bool _IsPeopleChecked;
        public bool IsPeopleChecked
        {
            get => _IsPeopleChecked;
            set => SetIsPeopleCheckedCheckedProperty(value);
        }
        private bool _IsObjectsChecked;
        public bool IsObjectsChecked
        {
            get => _IsObjectsChecked;
            set => SetIsObjectsCheckedCheckedProperty(value);
        }
        private bool _IsFoodChecked;
        public bool IsFoodChecked
        {
            get => _IsFoodChecked;
            set => SetIsFoodCheckedCheckedProperty(value);
        }
        private bool _IsTransportationsChecked;
        public bool IsTransportationsChecked
        {
            get => _IsTransportationsChecked;
            set => SetIsTransportationsCheckedCheckedProperty(value);
        }
        private bool _IsSymbolsChecked;
        public bool IsSymbolsChecked
        {
            get => _IsSymbolsChecked;
            set => SetIsSymbolsCheckedCheckedProperty(value);
        }

        /// <summary>
        /// The currently selected skin tone or <see cref="Emoji.ZeroWidthJoiner"/> for yellow.
        /// </summary>
        private Codepoint curSkinTone = Emoji.ZeroWidthJoiner;

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        public EmojiPickerControlDataTemplate()
        {
            LoadRecentEmoji();
            EMOJI_SMILEYS_FILTERED = new AdvancedCollectionView
            {
                Filter = EmojiFilter
            };
            EMOJI_PEOPLE_FILTERED = new AdvancedCollectionView
            {
                Filter = EmojiSkinToneFilter
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
                SelectedList = EMOJI_RECENT;
            }
        }

        private void SetIsSmileysCheckedCheckedProperty(bool value)
        {
            if (SetProperty(ref _IsSmileysChecked, value, nameof(IsSmileysChecked)) && value)
            {
                LoadEmoji(EMOJI_SMILEYS_FILTERED, new[] { Emoji.SmileysAndEmotion });
            }
        }

        private void SetIsPeopleCheckedCheckedProperty(bool value)
        {
            if (SetProperty(ref _IsPeopleChecked, value, nameof(IsPeopleChecked)) && value)
            {
                LoadEmoji(EMOJI_PEOPLE_FILTERED, new[] { Emoji.PeopleAndBody, Emoji.Component });
            }
        }

        private void SetIsObjectsCheckedCheckedProperty(bool value)
        {
            if (SetProperty(ref _IsObjectsChecked, value, nameof(IsObjectsChecked)) && value)
            {
                LoadEmoji(EMOJI_OBJECTS_FILTERED, new[] { Emoji.Objects, Emoji.Flags });
            }
        }

        private void SetIsFoodCheckedCheckedProperty(bool value)
        {
            if (SetProperty(ref _IsFoodChecked, value, nameof(IsFoodChecked)) && value)
            {
                LoadEmoji(EMOJI_FOOD_FILTERED, new[] { Emoji.FoodAndDrink, Emoji.AnimalsAndNature });
            }
        }

        private void SetIsTransportationsCheckedCheckedProperty(bool value)
        {
            if (SetProperty(ref _IsTransportationsChecked, value, nameof(IsTransportationsChecked)) && value)
            {
                LoadEmoji(EMOJI_TRANSPORTATIONS_FILTERED, new[] { Emoji.TravelAndPlaces, Emoji.Activities });
            }
        }

        private void SetIsSymbolsCheckedCheckedProperty(bool value)
        {
            if (SetProperty(ref _IsSymbolsChecked, value, nameof(IsSymbolsChecked)) && value)
            {
                LoadEmoji(EMOJI_SYMBOLS_FILTERED, new[] { Emoji.Symbols });
            }
        }

        #endregion
        //--------------------------------------------------------Misc Methods:---------------------------------------------------------------\\
        #region --Misc Methods (Public)--
        public void OnSkinToneSelected(Codepoint skinTone)
        {
            if (curSkinTone != skinTone)
            {
                curSkinTone = skinTone;
                UpdateFilter();
            }
        }

        #endregion

        #region --Misc Methods (Private)--
        private void UpdateFilter()
        {
            Task.Run(() =>
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
            });
        }

        private bool EmojiFilter(object o)
        {
            return o is SingleEmoji emoji && (string.IsNullOrEmpty(EmojiQuery) || emoji.SearchTerms.Any((x) => x.ToLower().Contains(EmojiQuery)));
        }

        private bool EmojiSkinToneFilter(object o)
        {
            return o is SingleEmoji emoji
                && (string.IsNullOrEmpty(EmojiQuery) || emoji.SearchTerms.Any((x) => x.ToLower().Contains(EmojiQuery)))
                && ((curSkinTone == Emoji.ZeroWidthJoiner && emoji.SkinTones == SingleEmoji.NoSkinTones) || emoji.SkinTones.Contains(curSkinTone));
        }

        private void LoadRecentEmoji()
        {
            Task.Run(() =>
            {
                IsLoading = true;
                if (Settings.LOCAL_OBJECT_STORAGE_HELPER.KeyExists(SettingsConsts.CHAT_RECENT_EMOJI))
                {
                    int[] result = Settings.LOCAL_OBJECT_STORAGE_HELPER.Read<int[]>(SettingsConsts.CHAT_RECENT_EMOJI);
                    foreach (int i in result)
                    {
                        if (TryGetEmoji(i, out SingleEmoji emoji))
                        {
                            EMOJI_RECENT.Add(emoji);
                        }
                    }
                }
                IsLoading = false;
            });
        }

        private bool TryGetEmoji(int i, out SingleEmoji emoji)
        {
            IEnumerable<SingleEmoji> result = Emoji.All.Where((x) => x.SortOrder == i);
            if (result.Count() > 0)
            {
                emoji = result.First();
                return true;
            }

            emoji = Emoji.Abacus;
            return false;
        }

        private void LoadEmoji(AdvancedCollectionView target, SortedSet<SingleEmoji>[] sources)
        {
            Task.Run(async () =>
            {
                IsLoading = true;
                if (target.Source.Count <= 0)
                {
                    SortedSet<SingleEmoji> result = sources[0];
                    for (int i = 1; i < sources.Length; i++)
                    {
                        result.UnionWith(sources[i]);
                    }
                    List<SingleEmoji> emoji = result.Where((x) => x.HasGlyph).ToList();
                    await SharedUtils.CallDispatcherAsync(() => target.Source = emoji);
                }
                target.RefreshFilter();
                SelectedList = target;
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
