// This class is a part of the fork from neosmarts unicode.net (https://github.com/neosmart/unicode.net)
// Source: https://github.com/UWPX/unicode.net
// Original license:
// MIT License:
// https://github.com/UWPX/unicode.net/blob/master/LICENSE

using System.Collections.Generic;
using System.Linq;

namespace NeoSmart.Unicode
{
    public class MultiRange
    {
        private List<Range> _ranges = new List<Range>();
        public IEnumerable<Range> Ranges => _ranges;

        public MultiRange(params string[] ranges)
        {
            _ranges.AddRange(ranges.Select(r => new Range(r)));
        }

        public MultiRange(params Range[] ranges)
        {
            _ranges.AddRange(ranges);
        }

        public MultiRange(IEnumerable<Range> ranges)
        {
            _ranges.AddRange(ranges);
        }

        public bool Contains(Codepoint codepoint)
        {
            return _ranges.Any(r => r.Contains(codepoint));
        }
    }
}
