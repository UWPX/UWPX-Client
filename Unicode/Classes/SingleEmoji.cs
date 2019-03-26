using System;
using System.Linq;
using System.Text;

namespace NeoSmart.Unicode
{
    //We hereby declare emoji to be a zero plural marker noun (in short, "emoji" is both the singular and the plural form)
    //this class refers to emoji in the singular
    public struct SingleEmoji : IComparable<SingleEmoji>, IEquatable<SingleEmoji>
    {
        private static readonly string[] NoTerms = new string[] { };
        public readonly UnicodeSequence Sequence;
        public readonly string Name;
        public readonly string[] SearchTerms;
        public readonly int SortOrder;
        public readonly SkinTone[] SkinTones;
        public static readonly SkinTone[] NoSkinTones = new SkinTone[] { SkinTone.NONE };
        public readonly Group Group;
        public readonly string Subgroup;
        public readonly bool HasGlyph;

        public SingleEmoji(UnicodeSequence sequence, string name = "", string[] searchTerms = null, SkinTone[] skinTones = null, Group group = Group.SMILEYS_AND_EMOTION, string subgroup = "", bool hasGlyph = false, int sortOrder = -1)
        {
            Sequence = sequence;
            Name = name;
            SearchTerms = searchTerms;
            SkinTones = skinTones ?? NoSkinTones;
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