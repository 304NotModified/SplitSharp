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
        [InlineData(@"a;b;c\", ';', Backslash, @"a,b,c\")]
        [InlineData(@"a;b;c\;", ';', Backslash, @"a,b,c;")]
        [InlineData(@"a;b;c\;;", ';', Backslash, @"a,b,c;,")]
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
            if (splitChar == escapeChar)
            {
                //also test SplitWithSelfEscape
                SplitWithSelfEscape2(input, splitChar, output);
            }

            SplitStringWithEscape2(input, splitChar, escapeChar, output);
        }

        private static void SplitWithSelfEscape2(string input, char splitChar, string output)
        {
            var strings = StringSplitter.SplitWithSelfEscape(input, splitChar).ToArray();
            var result = string.Join(",", strings);
            Assert.Equal(output, result);
        }

        private static void SplitStringWithEscape2(string input, char splitChar, char escapeChar, string output)
        {
            var strings = StringSplitter.SplitWithEscape(input, splitChar, escapeChar).ToArray();
            var result = string.Join(",", strings);
            Assert.Equal(output, result);
        }

        /// <summary>
        /// Tests with ; as separator, quoted and escaped with '
        /// </summary>
        [Theory]
        [InlineData(@";", @",")] 
        [InlineData(@";;", @",,")]
        [InlineData(@"a;", @"a,")]
        [InlineData(@"a;''b;c", "a,'b,c")]
        [InlineData(@"a;''b;c'", "a,'b,c'")]
        [InlineData(@"abc", "abc")]
        [InlineData(@"abc'", "abc'")]
        [InlineData(@"''abc'", "'abc'")]
        [InlineData(@"'abc'", "abc")]
        [InlineData(@"'ab;c'", "ab;c")]
        [InlineData(@"'ab\c'", @"ab\c")]
        [InlineData(@"'", @"'")]
        [InlineData(@"'a", @"'a")]
        [InlineData(@"a'", @"a'")]
        [InlineData(@"\", @"\")]
        [InlineData(@"a\", @"a\")]
        [InlineData(@"\b", @"\b")]

        private void SplitStringWithQuotes_selfQuoted(string input, string output)
        {
            SplitStringWithQuotes(input, ';', SingleQuote, SingleQuote, output);
        }

        /// <summary>
        /// Tests with ; as separator, quoted with single quote and escaped with backslash
        /// </summary>
        [Theory]
        [InlineData(null, "")]
        [InlineData(@"\", @"\")]
        [InlineData(@"'", @"'")]
        [InlineData(@"' ", @"' ")]
        [InlineData(@"a'", "a'")]
        [InlineData(@" ' ", @" ' ")]
        [InlineData(@" ; ", @" , ")]
        [InlineData(@";", @",")]
        [InlineData(@";;", @",,")]
        [InlineData(@"abc", "abc")]
        [InlineData(@"abc;", "abc,")]
        [InlineData(@"abc'", "abc'")]
        [InlineData(@"abc\", @"abc\")]
        [InlineData(@"abc\\", @"abc\\")]
        [InlineData(@"abc\b", @"abc\b")]
        [InlineData(@"a;b;c", "a,b,c")]
        [InlineData(@"a;'b;c'", "a,b;c")]
        [InlineData(@"a;'b;c", "a,'b;c")]
        [InlineData(@"a;b'c;d", "a,b'c,d")]
        [InlineData(@"a;\'b;c", "a,'b,c")]
        [InlineData(@"\b", @"\b")]
        [InlineData(@"'\'", @"'\'")]
        [InlineData(@"'\''", @"'\'")] //todo check case

        private void SplitStringWithQuotes_escaped(string input, string output)
        {
            SplitStringWithQuotes(input, ';', SingleQuote, Backslash, output);
        }




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
