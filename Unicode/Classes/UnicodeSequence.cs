// This class is a part of the fork from neosmarts unicode.net (https://github.com/neosmart/unicode.net)
// Source: https://github.com/UWPX/unicode.net
// Original license:
// MIT License:
// https://github.com/UWPX/unicode.net/blob/master/LICENSE

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NeoSmart.Unicode
{
    /// <summary>
    /// A UnicodeSequence is a combination of one or more codepoints.
    /// </summary>
    public class UnicodeSequence: IComparable<UnicodeSequence>, IEquatable<UnicodeSequence>, IEquatable<string>, IEqualityComparer<UnicodeSequence>
    {
        private readonly Codepoint[] _codepoints;
        public IEnumerable<Codepoint> Codepoints => _codepoints;

        private UnicodeSequence()
        {

        }

        public UnicodeSequence(string sequence)
        {
            if (sequence.Contains("-"))
            {
                string[] values = sequence.Split('-');

                if (values.Length == 2)
                {
                    Codepoint begin = new Codepoint(values[0]);
                    Codepoint end = new Codepoint(values[1]);
                    if (end.Value < begin.Value)
                    {
                        throw new InvalidRangeException();
                    }
                    _codepoints = new Codepoint[end.Value - begin.Value + 1];
                    for (int i = 0; begin.Value + i <= end.Value; ++i)
                    {
                        _codepoints[i] = new Codepoint(begin.Value + i);
                    }
                }
                else
                {
                    throw new InvalidRangeException();
                }
            }
            else
            {
                string[] values = sequence.Split(new[] { ',', ' ' }, StringSplitOptions.RemoveEmptyEntries);
                _codepoints = values.Select(x => new Codepoint(x)).ToArray();
            }
        }

        public UnicodeSequence(int[] sequences)
        {
            _codepoints = sequences.Select(x => new Codepoint(x)).ToArray();
        }

        public UnicodeSequence(IEnumerable<Codepoint> codepoints)
        {
            _codepoints = codepoints.ToArray();
        }

        public bool Contains(Codepoint codepoint)
        {
            return codepoint.In(_codepoints);
        }

        public IEnumerable<uint> AsUtf32()
        {
            foreach (Codepoint cp in _codepoints)
            {
                yield return cp.AsUtf32();
            }
        }

        public IEnumerable<byte> AsUtf32Bytes()
        {
            foreach (uint u32 in AsUtf32())
            {
                //little endian byte order
                yield return (byte)(u32 & 0xFF);
                yield return (byte)((u32 >> 8) & 0xFF);
                yield return (byte)((u32 >> 16) & 0xFF);
                yield return (byte)(u32 >> 24);
            }
        }

        public IEnumerable<ushort> AsUtf16()
        {
            foreach (Codepoint cp in _codepoints)
            {
                foreach (ushort us in cp.AsUtf16())
                {
                    yield return us;
                }
            }
        }

        public IEnumerable<byte> AsUtf16Bytes()
        {
            foreach (ushort us in AsUtf16())
            {
                //little endian byte order
                yield return (byte)(us & 0xFF);
                yield return (byte)(us >> 8);
            }
        }

        public IEnumerable<byte> AsUtf8()
        {
            foreach (Codepoint cp in _codepoints)
            {
                foreach (byte b in cp.AsUtf8())
                {
                    yield return b;
                }
            }
        }

        public string AsString => Encoding.Unicode.GetString(AsUtf16Bytes().ToArray());

        public int CompareTo(UnicodeSequence other)
        {
            for (int i = 0; i < Math.Min(_codepoints.Length, other._codepoints.Length); ++i)
            {
                if (_codepoints[i] != other._codepoints[i])
                {
                    return _codepoints[i].CompareTo(other._codepoints[i]);
                }
            }
            if (_codepoints.Length == other._codepoints.Length)
            {
                //same codepoint sequence
                return 0;
            }
            if (_codepoints.Length < other._codepoints.Length)
            {
                return -(int)other._codepoints[_codepoints.Length].AsUtf32();
            }
            return (int)_codepoints[other._codepoints.Length].AsUtf32();
        }

        public bool Equals(UnicodeSequence other)
        {
            if (base.Equals(other))
            {
                return true;
            }

            if (other is null || _codepoints.Length != other._codepoints.Length)
            {
                return false;
            }

            for (int i = 0; i < _codepoints.Length; ++i)
            {
                if (_codepoints[i] != other._codepoints[i])
                {
                    return false;
                }
            }

            return true;
        }

        public static bool operator ==(UnicodeSequence a, UnicodeSequence b)
        {
            return (a is null && b is null) || (!(a is null || b is null) && a.Equals(b));
        }

        public static bool operator !=(UnicodeSequence a, UnicodeSequence b)
        {
            return !(a == b);
        }

        public override bool Equals(object b)
        {
            if (b is UnicodeSequence)
            {
                return Equals(b as UnicodeSequence);
            }
            return base.Equals(b);
        }

        public bool Equals(UnicodeSequence a, UnicodeSequence b)
        {
            return (a is null && b is null) || (!(a is null || b is null) && a.Equals(b));
        }

        public override int GetHashCode()
        {
            return _codepoints.GetHashCode();
        }

        public bool Equals(string other)
        {
            return !(other is null) && other.Codepoints().SequenceEqual(_codepoints);
        }

        public int GetHashCode(UnicodeSequence obj)
        {
            return obj.GetHashCode();
        }
    }
}
