// 
// Copyright (c) 2016 Julian Verdurmen.
// 

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SplitSharp;
using Xunit;
using Xunit.Extensions;

namespace SplitSharp.Tests
{
    public class StringSplitterTests
    {
        private const char SingleQuote = '\'';
        private const char Backslash = '\\';

        [Theory]
        [InlineData("abc", ';', Backslash, "abc")]
        [InlineData("abc", ';', ';', "abc")]
        [InlineData("  abc", ';', Backslash, "  abc")]
        [InlineData("  abc", ';', ';', "  abc")]
        [InlineData(null, ';', Backslash, "")]
        [InlineData(null, ';', ';', "")]
        [InlineData("", ';', Backslash, "")]
        [InlineData("", ';', ';', "")]
        [InlineData("   ", ';', Backslash, "   ")]
        [InlineData("   ", ';', ';', "   ")]
        [InlineData(@"abc", ';', ',', "abc")]
        [InlineData(@"a;b;c", ';', Backslash, "a,b,c")]
        [InlineData(@"a;b;c;", ';', Backslash, "a,b,c,")]
        [InlineData(@"a;", ';', Backslash, "a,")]
        [InlineData(@";", ';', Backslash, ",")]
        [InlineData(@"a;b;c;", ';', Backslash, "a,b,c,")]
        [InlineData(@";a;b;c", ';', Backslash, ",a,b,c")]
        [InlineData(@"a;;b;c;", ';', Backslash, "a,,b,c,")]
        [InlineData(@"a\b;c", ';', Backslash, @"a\b,c")]
        [InlineData(@"a\;b;c", ';', Backslash, @"a;b,c")]
        [InlineData(@"a\;b\;c", ';', Backslash, @"a;b;c")]
        [InlineData(@"a\;b\;c;d", ';', Backslash, @"a;b;c,d")]
        [InlineData(@"a\;b\;c;d", ';', Backslash, @"a;b;c,d")]
        [InlineData(@"a\;b;c\;d", ';', Backslash, @"a;b,c;d")]
        [InlineData(@"a;b;;c", ';', ';', @"a,b;c")]
        [InlineData(@"a;b;;;;c", ';', ';', @"a,b;;c")]
        [InlineData(@"a;;b", ';', ';', @"a;b")]
        [InlineData(@"abc", ';', ';', @"abc")]
        [InlineData(@"abc\;", ';', Backslash, @"abc;")]
        [InlineData(@"a'b'c''d", SingleQuote, SingleQuote, @"a,b,c'd")]

        void SplitStringWithEscape(string input, char splitChar, char escapeChar, string output)
        {
            var strings = StringSplitter.SplitWithEscape(input, splitChar, escapeChar).ToArray();
            var result = string.Join(",", strings);
            Assert.Equal(output, result);
        }

        [Theory]
        [InlineData(null, ';', SingleQuote, Backslash, "")]
        [InlineData(@"abc", ';', SingleQuote, Backslash, "abc")]
        [InlineData(@"abc;", ';', SingleQuote, Backslash, "abc,")]
        [InlineData(@"abc'", ';', SingleQuote, Backslash, "abc'")]
        [InlineData(@"abc\", ';', SingleQuote, Backslash, @"abc\")]
        [InlineData(@"abc\\", ';', SingleQuote, Backslash, @"abc\\")]
        [InlineData(@"abc\b", ';', SingleQuote, Backslash, @"abc\b")]
        [InlineData(@"a;b;c", ';', SingleQuote, Backslash, "a,b,c")]
        [InlineData(@"a;'b;c'", ';', SingleQuote, Backslash, "a,b;c")]
        [InlineData(@"a;'b;c", ';', SingleQuote, Backslash, "a,'b;c")]
        [InlineData(@"a;b'c;d", ';', SingleQuote, Backslash, "a,b'c,d")]
        [InlineData(@"a;\'b;c", ';', SingleQuote, Backslash, "a,'b,c")]
        [InlineData(@"a;''b;c", ';', SingleQuote, SingleQuote, "a,'b,c")]
        [InlineData(@"a;''b;c'", ';', SingleQuote, SingleQuote, "a,'b,c'")]
        [InlineData(@"abc", ';', SingleQuote, SingleQuote, "abc")]
        [InlineData(@"abc'", ';', SingleQuote, SingleQuote, "abc'")]
        [InlineData(@"''abc'", ';', SingleQuote, SingleQuote, "'abc'")]
        [InlineData(@"'abc'", ';', SingleQuote, SingleQuote, "abc")]
        [InlineData(@"'ab;c'", ';', SingleQuote, SingleQuote, "ab;c")]
        [InlineData(@"'ab\c'", ';', SingleQuote, SingleQuote, @"ab\c")]
        [InlineData(@"'", ';', SingleQuote, SingleQuote, @"'")]
        [InlineData(@"a'", ';', SingleQuote, SingleQuote, @"a'")]
        [InlineData(@"\", ';', SingleQuote, SingleQuote, @"\")]
        [InlineData(@"a\", ';', SingleQuote, SingleQuote, @"a\")]

        void SplitStringWithQuotes(string input, char splitChar, char quoteChar, char escapeChar, string output)
        {
            var strings = StringSplitter.SplitQuoted(input, splitChar, quoteChar, escapeChar).ToArray();
            var result = string.Join(",", strings);
            Assert.Equal(output, result);
        }

        [Theory]
        [InlineData(';', ';', Backslash)]
        [InlineData(';', Backslash, ';')]
        void SplitStringNotSupported(char splitChar, char quoteChar, char escapeChar)
        {
            Assert.Throws<NotSupportedException>(() => StringSplitter.SplitQuoted("dont care", splitChar, quoteChar, escapeChar));

        }
    }
}
