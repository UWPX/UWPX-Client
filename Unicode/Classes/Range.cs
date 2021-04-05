// This class is a part of the fork from neosmarts unicode.net (https://github.com/neosmart/unicode.net)
// Source: https://github.com/UWPX/unicode.net
// Original license:
// MIT License:
// https://github.com/UWPX/unicode.net/blob/master/LICENSE

using System;
using System.Collections.Generic;

namespace NeoSmart.Unicode
{
    public class Range
    {
        public readonly Codepoint Begin;
        public readonly Codepoint End;

        public Range(Codepoint begin, Codepoint end)
        {
            Begin = begin;
            End = end;
        }

        public Range(Codepoint value)
        {
            Begin = value;
            End = value;
        }

        public bool Contains(Codepoint codepoint)
        {
            return codepoint >= Begin && codepoint <= End;
        }

        //either a single hex codepoint or two separated by a hyphen
        public Range(string range)
        {
            string[] values = range.Split(new[] { "-", "–", "—", ".." }, StringSplitOptions.RemoveEmptyEntries); //these are all different hyphens used on Wikipedia and in the UTR
            Begin = uint.Parse(values[0], System.Globalization.NumberStyles.HexNumber);

            if (values.Length == 1)
            {
                End = Begin;
            }
            else if (values.Length == 2)
            {
                End = uint.Parse(values[1], System.Globalization.NumberStyles.HexNumber);
            }
            else
            {
                throw new InvalidRangeException();
            }
        }

        public IEnumerable<uint> AsUtf32Sequence
        {
            get
            {
                for (uint i = 0; Begin + i <= End; ++i)
                {
                    yield return new Codepoint(Begin + i).AsUtf32();
                }
            }
        }

        public IEnumerable<ushort> AsUtf16Sequence
        {
            get
            {
                for (int i = 0; Begin + i <= End; ++i)
                {
                    foreach (ushort utf16 in new Codepoint(Begin + i).AsUtf16())
                    {
                        yield return utf16;
                    }
                }
            }
        }

        public IEnumerable<byte> AsUtf8Sequence
        {
            get
            {
                for (int i = 0; Begin + i <= End; ++i)
                {
                    foreach (byte utf8 in new Codepoint(Begin + i).AsUtf8())
                    {
                        yield return utf8;
                    }
                }
            }
        }
    }
}
