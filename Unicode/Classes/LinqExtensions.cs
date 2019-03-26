// This class is a part of the fork from neosmarts unicode.net (https://github.com/neosmart/unicode.net)
// Source: https://github.com/UWPX/unicode.net
// Original license:
// MIT License:
// https://github.com/UWPX/unicode.net/blob/master/LICENSE

using System.Collections.Generic;
using System.Linq;

namespace NeoSmart.Unicode
{
    internal static class LinqExtensions
    {
        public static bool In<T>(this T t, IEnumerable<T> collection)
        {
            return collection.Contains(t);
        }
    }
}
