// This class is a part of the fork from neosmarts unicode.net (https://github.com/neosmart/unicode.net)
// Source: https://github.com/UWPX/unicode.net
// Original license:
// MIT License:
// https://github.com/UWPX/unicode.net/blob/master/LICENSE

using System;

namespace NeoSmart.Unicode
{
    public class UnsupportedCodepointException: Exception
    { }

    public class InvalidRangeException: Exception
    { }

    public class InvalidEncodingException: Exception
    { }
}
