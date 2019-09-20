// This class is a part of the fork from neosmarts unicode.net (https://github.com/neosmart/unicode.net)
// Source: https://github.com/UWPX/unicode.net
// Original license:
// MIT License:
// https://github.com/UWPX/unicode.net/blob/master/LICENSE

using System.Collections.Generic;
using System.Linq;

namespace NeoSmart.Unicode
{
    //We hereby declare emoji to be a zero plural marker noun (in short, "emoji" is both the singular and the plural form)
    //this class refers to emoji in the plural
    public static partial class Emoji
    {
        static Emoji()
        {
            //populate the list of available emoji in the system font
            //for now, the font is hard-coded to Segoe UI Emoji
        }

        /// <summary>
        /// ZWJ is used to combine multiple emoji codepoints into a single emoji symbol.
        /// </summary>
        public static readonly Codepoint ZeroWidthJoiner = Codepoints.ZWJ;

        public static readonly Codepoint ObjectReplacementCharacter = Codepoints.ORC;

        public static readonly Codepoint Keycap = Codepoints.Keycap;

        /// <summary>
        /// The Emoji VS indicates that the preceding (non-emoji) unicode codepoint should be represented as an emoji.
        /// </summary>
        public static readonly Codepoint VariationSelector = Codepoints.VariationSelectors.EmojiSymbol;

        public static class SkinTones
        {
            /// <summary>
            /// 🏻 light skin tone.
            /// </summary>
            public static readonly Codepoint Light = new Codepoint("U+1F3FB");
            /// <summary>
            /// 🏻 light skin tone.
            /// </summary>
            public static readonly Codepoint Fitzpatrick12 = Light;
            /// <summary>
            /// 🏼 medium-light skin tone.
            /// </summary>
            public static readonly Codepoint MediumLight = new Codepoint("U+1F3FC");
            /// <summary>
            /// 🏼 medium-light skin tone.
            /// </summary>
            public static readonly Codepoint Fitzpatrick3 = MediumLight;
            /// <summary>
            /// 🏽 medium skin tone.
            /// </summary>
            public static readonly Codepoint Medium = new Codepoint("U+1F3FD");
            /// <summary>
            /// 🏽 medium skin tone.
            /// </summary>
            public static readonly Codepoint Fitzpatrick4 = Medium;
            /// <summary>
            /// 🏾 medium-dark skin tone.
            /// </summary>
            public static readonly Codepoint MediumDark = new Codepoint("U+1F3FE");
            /// <summary>
            /// 🏾 medium-dark skin tone.
            /// </summary>
            public static readonly Codepoint Fitzpatrick5 = MediumDark;
            /// <summary>
            /// 🏿 dark skin tone.
            /// </summary>
            public static readonly Codepoint Dark = new Codepoint("U+1F3FF");
            /// <summary>
            /// 🏿 dark skin tone.
            /// </summary>
            public static readonly Codepoint Fitzpatrick6 = Dark;

            /// <summary>
            /// Helper object, most useful for checking if a codepoint is a skin tone quickly.
            /// </summary>
            public static readonly List<Codepoint> All = new List<Codepoint>() { Light, MediumLight, Medium, MediumDark, Dark };
        }

        /// <summary>
        /// Combines multiple emoji with a zero-width-joiner to (potentially) create a new symbol
        /// </summary>
        /// <param name="emoji"></param>
        /// <returns></returns>
        public static string Combine(IEnumerable<SingleEmoji> emoji)
        {
            //does not work on .NET 2.0
            //return string.Join(ZeroWidthJoiner.AsString(), emoji);
            return string.Join(ZeroWidthJoiner.AsString(), emoji.Select(e => e.ToString()).ToArray());
        }

        /// <summary>
        /// Combines multiple emoji with a zero-width-joiner to (potentially) create a new symbol
        /// </summary>
        /// <param name="emoji"></param>
        /// <returns></returns>
        public static string Combine(params SingleEmoji[] emoji)
        {
            return Combine((IEnumerable<SingleEmoji>)emoji);
        }

        /// <summary>
        /// Determines whether a string is comprised solely of emoji, optionally with a maximum number of drawn symbols.
        /// Can be used to determine whether a message consists of ≦ x emoji for purposes such as displaying at a larger size. Since one emoji symbol can be formed
        /// from many separate emoji "characters" combined with zero-width joiners or even non-emoji characters followed by a "use emoji representation" marker, this
        /// cannot be determined solely from the codepoints.
        /// </summary>
        /// <param name="message"></param>
        /// <param name="maxSymbolCount"></param>
        /// <returns></returns>
        public static bool IsEmoji(string message, int maxSymbolCount = int.MaxValue)
        {
            IEnumerable<Codepoint> codepoints = message.Codepoints();

            bool nextMustBeVS = false;
            string zwj = ZeroWidthJoiner.AsString();
            string variationSelector = VariationSelector.AsString();
            bool ignoreNext = false;
            int count = 0;
            foreach (Codepoint cp in codepoints)
            {
                //we used to have message = message.trim() previously. This avoids the extra allocation, hepful in case of long messages.
                //this was not premature optimization, it came out of necessity.
                if (cp == "\n" || cp == "\r" || cp == "\t" || cp == " ")
                {
                    continue;
                }

                if (nextMustBeVS)
                {
                    nextMustBeVS = false;
                    if (cp != variationSelector)
                    {
                        //a non-emoji codepoint was found and this (the codepoint after it) is not the variation selector
                        return false;
                    }
                }
                if (cp.In(SkinTones.All))
                {
                    //don't count as part of the length
                    continue;
                }

                if (cp == zwj)
                {
                    ignoreNext = true;
                    continue;
                }

                if (cp == variationSelector)
                {
                    continue;
                }

                if (cp == Codepoints.ObjectReplacementCharacter)
                {
                    //this is explicitly blacklisted for UI purposes
                    return false;
                }

                if (cp == Keycap)
                {
                    //this is not in the UTR, but is used to box symbols in an icon
                    //do not consider it part of the length
                    continue;
                }

                if (!ignoreNext)
                {
                    ++count;
                    if (count > maxSymbolCount)
                    {
                        return false;
                    }
                    //by default, the UTR lists numbers, the asterisk, and the number sign as emoji, but we won't consider them as such unless they are followed by a VS
                    if (Languages.ArabicNumerals.Contains(cp) || cp == "#" || cp == "*")
                    {
                        nextMustBeVS = true;
                        continue;
                    }
                    else if (Languages.Emoji.Contains(cp))
                    {
                        continue;
                    }
                    else
                    {
                        //we've either encountered a non-emoji character OR a non-emoji codepoint that should be treated as an emoji if followed by the variation selector codepoint
                        nextMustBeVS = true;
                        continue;
                    }
                }
                ignoreNext = false;
            }

            if (nextMustBeVS)
            {
                return false;
            }

            return count > 0 && count <= maxSymbolCount;
        }
    }
}
