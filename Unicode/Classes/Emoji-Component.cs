using System.Collections.Generic;

namespace NeoSmart.Unicode
{
    // This file is machine-generated based on the official Unicode Consortium publication (https://unicode.org/Public/emoji/12.0/emoji-test.txt).
    // See https://github.com/UWPX/Emoji-List-Parser for the generator.
    public static partial class Emoji
    {
        /// <summary>
        /// A (sorted) enumeration of all emoji in group: COMPONENT
        /// Only contains fully-qualified and component emoji.
        /// <summary>
#if NET20 || NET30 || NET35
        public static readonly List<SingleEmoji> Component = new List<SingleEmoji>() {
#else
        public static SortedSet<SingleEmoji> Component => new SortedSet<SingleEmoji>() {
#endif
            /* 🏻 */ LightSkinTone,
            /* 🏼 */ MediumLightSkinTone,
            /* 🏽 */ MediumSkinTone,
            /* 🏾 */ MediumDarkSkinTone,
            /* 🏿 */ DarkSkinTone,
            /* 🦰 */ RedHair,
            /* 🦱 */ CurlyHair,
            /* 🦳 */ WhiteHair,
            /* 🦲 */ Bald,
        };
    }
}
