// This class is a part of the fork from neosmarts unicode.net (https://github.com/neosmart/unicode.net)
// Source: https://github.com/UWPX/unicode.net
// Original license:
// MIT License:
// https://github.com/UWPX/unicode.net/blob/master/LICENSE

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace NeoSmart.Unicode
{
    public struct Codepoint: IComparable<Codepoint>, IComparable<UInt32>, IEquatable<Codepoint>,
        IEquatable<string>, IComparable<string>, IEquatable<char>
    {
        public readonly UInt32 Value;

        public Codepoint(UInt32 value)
        {
            Value = value;
        }

        public Codepoint(long value)
            : this((UInt32)value)
        { }

        /// <summary>
        /// Create a unicode codepoint from hexadecimal representation, supporting U+xxxx and 0xYYYY notation.
        /// </summary>
        /// <param name="hexValue"></param>
        public Codepoint(string hexValue)
        {
            if (hexValue.StartsWith("0x") || hexValue.StartsWith("U+") || hexValue.StartsWith("u+"))
            {
                hexValue = hexValue.Substring(2);
            }
            if (!UInt32.TryParse(hexValue, NumberStyles.HexNumber, CultureInfo.CurrentCulture.NumberFormat, out Value))
            {
                throw new UnsupportedCodepointException();
            }
        }

        public UInt32 AsUtf32()
        {
            return Value;
        }

        /// <summary>
        /// Returns an iterator that will enumerate over the big endian bytes in the UTF32 encoding of this codepoint.
        /// </summary>
        public IEnumerable<byte> AsUtf32Bytes()
        {
            //from highest to lowest
            uint utf32 = AsUtf32();
            byte b1 = (byte)(utf32 >> 24);
            yield return b1;
            byte b2 = (byte)((utf32 & 0x00FFFFFF) >> 16);
            yield return b2;
            byte b3 = (byte)(((UInt16)utf32) >> 8);
            yield return b3;
            byte b4 = (byte)utf32;
            yield return b4;
        }

        //https://en.wikipedia.org/wiki/UTF-16
        public IEnumerable<UInt16> AsUtf16()
        {
            //U+0000 to U+D7FF and U+E000 to U+FFFF
            if (Value <= 0xFFFF)
            {
                yield return (UInt16)Value;
            }
            //U+10000 to U+10FFFF
            else if (Value >= 0x10000 && Value <= 0x10FFFF)
            {
                UInt32 newVal = Value - 0x010000; //leaving 20 bits
                UInt16 high = (UInt16)((newVal >> 10) + 0xD800);
                System.Diagnostics.Debug.Assert(high <= 0xDBFF && high >= 0xD800);
                yield return high;

                UInt16 low = (UInt16)((newVal & 0x03FF) + 0xDC00);
                System.Diagnostics.Debug.Assert(low <= 0xDFFF && low >= 0xDC00);
                yield return low;
            }
            else
            {
                throw new UnsupportedCodepointException();
            }
        }

        /// <summary>
        /// Returns an iterator that will enumerate over the big endian bytes in the UTF16 encoding of this codepoint.
        /// </summary>
        public IEnumerable<byte> AsUtf16Bytes()
        {
            IEnumerable<ushort> utf16 = AsUtf16();
            foreach (ushort u16 in utf16)
            {
                byte high = (byte)(u16 >> 8);
                yield return high;
                byte low = (byte)u16;
                yield return low;
            }
        }

        //https://en.wikipedia.org/wiki/UTF-8
        public IEnumerable<byte> AsUtf8()
        {
            //up to 7 bits
            if (Value <= 0x007F)
            {
                yield return (byte)Value;
                yield break;
            }

            //up to 11 bits
            if (Value <= 0x07FF)
            {
                yield return (byte)(0b11000000 | (0b00011111 & (Value >> 6))); //tag + upper 5 bits
                yield return (byte)(0b10000000 | (0b00111111 & Value)); //tag + lower 6 bits
                yield break;
            }

            //up to 16 bits
            if (Value <= 0x0FFFF)
            {
                yield return (byte)(0b11100000 | (0b00001111 & (Value >> 12))); //tag + upper 4 bits
                yield return (byte)(0b10000000 | (0b00111111 & (Value >> 6))); //tag + next 6 bits
                yield return (byte)(0b10000000 | (0b00111111 & Value)); //tag + last 6 bits
                yield break;
            }

            //up to 21 bits
            if (Value <= 0x1FFFFF)
            {
                yield return (byte)(0b11110000 | (0b00000111 & (Value >> 18))); //tag + upper 3 bits
                yield return (byte)(0b10000000 | (0b00111111 & (Value >> 12))); //tag + next 6 bits
                yield return (byte)(0b10000000 | (0b00111111 & (Value >> 6))); //tag + next 6 bits
                yield return (byte)(0b10000000 | (0b00111111 & Value)); //tag + last 6 bits
                yield break;
            }

            throw new UnsupportedCodepointException();
        }

        public int CompareTo(Codepoint other)
        {
            return Value.CompareTo(other.Value);
        }

        public int CompareTo(UInt32 other)
        {
            return Value.CompareTo(other);
        }

        public bool Equals(Codepoint other)
        {
            return Value == other.Value;
        }

        public override bool Equals(object obj)
        {
            if (obj is Codepoint)
            {
                return Value == ((Codepoint)obj).Value;
            }
            return base.Equals(obj);
        }

        public override int GetHashCode()
        {
            return Value.GetHashCode();
        }

        public static bool operator ==(Codepoint a, Codepoint b)
        {
            return a.Value == b.Value;
        }

        public static bool operator !=(Codepoint a, Codepoint b)
        {
            return a.Value != b.Value;
        }

        public static bool operator <(Codepoint a, Codepoint b)
        {
            return a.Value < b.Value;
        }

        public static bool operator >(Codepoint a, Codepoint b)
        {
            return a.Value > b.Value;
        }

        public static bool operator >=(Codepoint a, Codepoint b)
        {
            return a.Value >= b.Value;
        }

        public static bool operator <=(Codepoint a, Codepoint b)
        {
            return a.Value <= b.Value;
        }

        public static implicit operator UInt32(Codepoint codepoint)
        {
            return codepoint.Value;
        }

        public static implicit operator Codepoint(UInt32 value)
        {
            return new Codepoint(value);
        }

        public override string ToString()
        {
            return $"U+{Value.ToString("X")}";
        }

        public string AsString()
        {
            return Encoding.UTF8.GetString(AsUtf8().ToArray());
        }

        public bool IsIn(Range range)
        {
            return range.Contains(this);
        }

        public bool IsIn(MultiRange multirange)
        {
            return multirange.Contains(this);
        }

        public bool Equals(string other)
        {
            return AsString() == other;
        }

        public int CompareTo(string other)
        {
            return AsString().CompareTo(other);
        }

        public bool Equals(char other)
        {
            string s = AsString();
            return s.Count() == 1 && s[0] == other;
        }

        public static bool operator ==(Codepoint a, string b)
        {
            return a.Equals(b);
        }

        public static bool operator !=(Codepoint a, string b)
        {
            return !a.Equals(b);
        }

        public static bool operator ==(Codepoint a, char b)
        {
            return a.Equals(b);
        }

        public static bool operator !=(Codepoint a, char b)
        {
            return !a.Equals(b);
        }
    }
}
