// This class is a part of the fork from neosmarts unicode.net (https://github.com/neosmart/unicode.net)
// Source: https://github.com/UWPX/unicode.net
// Original license:
// MIT License:
// https://github.com/UWPX/unicode.net/blob/master/LICENSE

using System.Collections.Generic;

namespace NeoSmart.Unicode
{
    public static class StringExtension
    {
        public static IEnumerable<Codepoint> Codepoints(this string s)
        {
            for (int i = 0; i < s.Length; ++i)
            {
                if (char.IsHighSurrogate(s[i]))
                {
                    if (s.Length < i + 2)
                    {
                        throw new InvalidEncodingException();
                    }
                    if (!char.IsLowSurrogate(s[i + 1]))
                    {
                        throw new InvalidEncodingException();
                    }
                    yield return new Codepoint(char.ConvertToUtf32(s[i], s[++i]));
                }
                else
                {
                    yield return new Codepoint((int)s[i]);
                }
            }
        }

        public static IEnumerable<string> Letters(this string s)
        {
            for (int i = 0; i < s.Length; ++i)
            {
                if (char.IsHighSurrogate(s[i]))
                {
                    if (s.Length < i + 2)
                    {
                        throw new InvalidEncodingException();
                    }
                    if (!char.IsLowSurrogate(s[i + 1]))
                    {
                        throw new InvalidEncodingException();
                    }
                    yield return string.Format("{0}{1}", s[i], s[++i]);
                }
                else
                {
                    yield return string.Format("{0}", s[i]);
                }
            }
        }

        public static UnicodeSequence AsUnicodeSequence(this string s)
        {
            return new UnicodeSequence(s.Codepoints());
        }
    }
}
