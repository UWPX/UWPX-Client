// This class is a part of the fork from neosmarts unicode.net (https://github.com/neosmart/unicode.net)
// Source: https://github.com/UWPX/unicode.net
// Original license:
// MIT License:
// https://github.com/UWPX/unicode.net/blob/master/LICENSE

namespace NeoSmart.Unicode
{
    /// <summary>
    /// Helpful definitions for commonly- and not-so-commonly-used codepoints.
    /// </summary>
    public static class Codepoints
    {
        /// <summary>
        /// The right-to-left mark
        /// </summary>
        public static readonly Codepoint RLM = new Codepoint("U+200F");

        /// <summary>
        /// The left-to-right mark
        /// </summary>
        public static readonly Codepoint LRM = new Codepoint("U+200E");

        /// <summary>
        /// ZWJ is used to combine multiple emoji codepoints into a single emoji symbol.
        /// </summary>
        public static readonly Codepoint ZWJ = new Codepoint("U+200D");

        /// <summary>
        /// ORC is used as a placeholder to indicate an object should replace this codepoint in the string.
        /// </summary>
        public static readonly Codepoint ObjectReplacementCharacter = new Codepoint("U+FFFC");

        public static readonly Codepoint ORC = ObjectReplacementCharacter;

        /// <summary>
        /// The "combined enclosing keycap" is used by emoji to box icons
        /// </summary>
        public static readonly Codepoint Keycap = new Codepoint("U+20E3");

        /// <summary>
        /// Variation selectors come after a unicode codepoint to indicate that it should be represented in a particular format.
        /// </summary>
        public static class VariationSelectors
        {
            public static readonly Codepoint VS15 = new Codepoint("U+FE0E");
            public static readonly Codepoint TextSymbol = VS15;

            public static readonly Codepoint VS16 = new Codepoint("U+FE0F");
            public static readonly Codepoint EmojiSymbol = VS16;
        }
    }
}
