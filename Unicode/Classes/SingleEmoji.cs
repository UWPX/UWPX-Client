// This class is a part of the fork from neosmarts unicode.net (https://github.com/neosmart/unicode.net)
// Source: https://github.com/UWPX/unicode.net
// Original license:
// MIT License:
// https://github.com/UWPX/unicode.net/blob/master/LICENSE

using System;
using System.Linq;
using System.Text;

namespace NeoSmart.Unicode
{
    //We hereby declare emoji to be a zero plural marker noun (in short, "emoji" is both the singular and the plural form)
    //this class refers to emoji in the singular
    public struct SingleEmoji: IComparable<SingleEmoji>, IEquatable<SingleEmoji>
    {
        private static readonly string[] NoTerms = new string[] { };
        public readonly UnicodeSequence Sequence;
        public readonly string Name;
        public readonly string[] SearchTerms;
        public readonly int SortOrder;
        public readonly Codepoint[] SkinTones;
        public static readonly Codepoint[] NoSkinTones = new Codepoint[] { };
        /// <summary>
        /// The version the emoji got added to the standard.
        /// Reference: http://unicode.org/reports/tr51/tr51-17.html
        /// </summary>
        public readonly double ENumber;
        public readonly Group Group;
        public readonly string Subgroup;
        public readonly bool HasGlyph;

        public SingleEmoji(UnicodeSequence sequence, string name = "", string[] searchTerms = null, Codepoint[] skinTones = null, double eNumber = 0.0, Group group = Group.SMILEYS_AND_EMOTION, string subgroup = "", bool hasGlyph = false, int sortOrder = -1)
        {
            Sequence = sequence;
            Name = name;
            SearchTerms = searchTerms;
            SkinTones = skinTones ?? NoSkinTones;
            ENumber = eNumber;
            Group = group;
            Subgroup = subgroup;
            HasGlyph = hasGlyph;
            SortOrder = sortOrder;
        }

        public int CompareTo(SingleEmoji other)
        {
            return SortOrder.CompareTo(other.SortOrder);
        }

        public static bool operator ==(SingleEmoji a, SingleEmoji b)
        {
            return a.Sequence == b.Sequence;
        }

        public static bool operator !=(SingleEmoji a, SingleEmoji b)
        {
            return !(a == b);
        }

        public override bool Equals(object obj)
        {
            if (obj is SingleEmoji emoji)
            {
                return Equals(emoji);
            }
            return base.Equals(obj);
        }

        public override int GetHashCode()
        {
            return Sequence.GetHashCode();
        }

        public override string ToString()
        {
            return Encoding.Unicode.GetString(Sequence.AsUtf16Bytes().ToArray());
        }

        public bool Equals(SingleEmoji other)
        {
            return Sequence.Equals(other.Sequence);
        }
    }
}
