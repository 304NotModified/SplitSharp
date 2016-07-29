// 
// Copyright (c) 2016 Julian Verdurmen.
// 

#region

using System;
using System.Collections.Generic;
using System.Text;

#endregion

namespace SplitSharp
{
    /// <summary>
    /// Split a string 
    /// </summary>
    public static class StringSplitter
    {
        /// <summary>
        /// Split string with escape. The escape char is the same as the splitchar
        /// </summary>
        /// <param name="text"></param>
        /// <param name="splitChar">split char. escaped also with this char</param>
        /// <returns></returns>
        public static IEnumerable<string> SplitWithSelfEscape(this string text, char splitChar)
        {
            return SplitWithSelfEscape2(text, splitChar);
        }

        /// <summary>
        /// Split string with escape
        /// </summary>
        /// <param name="text"></param>
        /// <param name="splitChar"></param>
        /// <param name="escapeChar"></param>
        /// <returns></returns>
        public static IEnumerable<string> SplitWithEscape(this string text, char splitChar, char escapeChar)
        {
            if (splitChar == escapeChar)
            {
                return SplitWithSelfEscape2(text, splitChar);
            }

            return SplitWithEscape2(text, splitChar, escapeChar);
        }


        private static IEnumerable<string> SplitWithEscape2(string text, char splitChar, char escapeChar)
        {
            if (!string.IsNullOrEmpty(text))
            {
                var prevWasEscape = false;
                int i;
                var sb = new StringBuilder();
                for (i = 0; i < text.Length; i++)
                {
                    var c = text[i];

                    //prev not escaped, then check splitchar
                    var isSplitChar = c == splitChar;
                    if (prevWasEscape)
                    {
                        if (isSplitChar)
                        {
                            //overwrite escapechar
                            if (sb.Length > 0)
                                sb.Length--;
                            sb.Append(c);
                            //if splitchar ==escapechar, then in this case it's used as split
                            prevWasEscape = false;
                        }
                        else
                        {
                            sb.Append(c);
                            prevWasEscape = c == escapeChar;
                        }
                    }
                    else
                    {
                        if (isSplitChar)
                        {
                            var part = sb.ToString();
                            //reset
                            sb.Length = 0;
                            yield return part;
                            if (text.Length - 1 == i)
                            {
                                //done
                                yield return string.Empty;
                                break;
                            }

                        }
                        else
                        {
                            sb.Append(c);
                            prevWasEscape = c == escapeChar;
                        }
                    }
                }
                var lastPart = GetLastPart(sb);
                if (lastPart != null)
                {
                    yield return lastPart;
                }
            }

        }

        private static IEnumerable<string> SplitWithSelfEscape2(string text, char splitChar)
        {
            if (!string.IsNullOrEmpty(text))
            {
                var prevWasEscape = false;
                int i;
                var sb = new StringBuilder();
                //if same, handle different
                for (i = 0; i < text.Length; i++)
                {
                    var c = text[i];
                    var isSplitChar = c == splitChar;
                    if (prevWasEscape)
                    {
                        if (isSplitChar)
                        {
                            prevWasEscape = false;
                        }
                        else
                        {
                            if (sb.Length > 0) sb.Length--;
                            var part = sb.ToString();
                            //reset
                            sb.Length = 0;
                            prevWasEscape = false;
                            sb.Append(c);
                            yield return part;

                            if (text.Length - 1 == i)
                            {
                                //done
                                yield return string.Empty;
                                break;
                            }
                        }
                    }
                    else
                    {
                        sb.Append(c);
                        if (isSplitChar)
                        {
                            prevWasEscape = true;
                        }
                    }
                }
                var lastPart = GetLastPart(sb);
                if (lastPart != null)
                {
                    yield return lastPart;
                }
            }
        }


        /// <summary>
        /// Split a string, optional quoted value
        /// </summary>
        /// <param name="text">Text to split</param>
        /// <param name="splitChar">Character to split the <paramref name="text"/></param>
        /// <param name="quoteChar">Quote character</param>
        /// <param name="escapeChar">Escape for the <paramref name="quoteChar"/>, not escape for the <paramref name="splitChar"/>, use quotes for that.</param>
        /// <returns></returns>
        public static IEnumerable<string> SplitQuoted(this string text, char splitChar, char quoteChar, char escapeChar)
        {
            if (!string.IsNullOrEmpty(text))
            {

                if (splitChar == quoteChar)
                {
                    throw new NotSupportedException("Quote character should different from split character");
                }


                if (splitChar == escapeChar)
                {
                    throw new NotSupportedException("Escape character should different from split character");
                }

                if (quoteChar == escapeChar)
                {
                    return SplitSelfQuoted2(text, splitChar, quoteChar);
                }


                return SplitQuoted2(text, splitChar, quoteChar, escapeChar);



            }
            return new List<string>();

        }

        private static IEnumerable<string> SplitSelfQuoted2(string text, char splitChar, char quoteAndEscapeChar)
        {
            var inQuotedMode = false;
            int i;
            var sb = new StringBuilder();
            var isNewPart = true;

            for (i = 0; i < text.Length; i++)
            {
                var c = text[i];

                //prev not escaped, then check splitchar
                var isSplitChar = c == splitChar;
                var isQuoteAndEscapeChar = c == quoteAndEscapeChar;
                var isLastChar = i == text.Length - 1;

                if (isNewPart)
                {
                    if (isLastChar)
                    {
                        //done
                        sb.Append(c);
                        break;
                    }

                    //now only quote for quotemode accepted
                    isNewPart = false;
                    isQuoteAndEscapeChar = c == quoteAndEscapeChar;

                    if (isQuoteAndEscapeChar)
                    {
                        //escape of the quote, if the quote is after this.

                        i++;
                        if (text.Length == i)
                        {
                            //last char
                            sb.Append(quoteAndEscapeChar);
                            break;
                        }
                        else
                        {
                            c = text[i];
                            if (c == quoteAndEscapeChar)
                            {
                                sb.Append(quoteAndEscapeChar);
                            }
                            else
                            {
                                sb.Append(c);
                                inQuotedMode = true;
                            }
                        }
                    }


                    else
                    {
                        sb.Append(c);
                    }
                }

                else if (inQuotedMode)
                {
                    if (isQuoteAndEscapeChar)
                    {
                        //skip quoteChar
                        i++;
                        //    isInPart = false;
                        inQuotedMode = false;
                        var part = sb.ToString();
                        //reset
                        sb.Length = 0;
                        yield return part;
                    }
                    else
                    {
                        sb.Append(c);
                    }


                }
                else
                {

                    if (isSplitChar)
                    {


                        //end of part

                        var part = sb.ToString();
                        //reset
                        sb.Length = 0;
                        //  isInPart = false;
                        yield return part;

                        if (isLastChar)
                        {
                            //done
                            yield return string.Empty;
                            break;
                        }

                        isNewPart = true;


                    }
                    else
                    {
                        sb.Append(c);
                    }
                }
            }

            var lastPart = GetLastPart(sb);
            if (inQuotedMode)
            {
                //append quote back
                lastPart = quoteAndEscapeChar + lastPart;
            }

            if (lastPart != null)
            {
                yield return lastPart;
            }
        }

        private static IEnumerable<string> SplitQuoted2(string text, char splitChar, char quoteChar, char escapeChar)
        {
            var inQuotedMode = false;
            int i;
            var sb = new StringBuilder();
            var isNewPart = true;

            var prevIsEscape = false;
            for (i = 0; i < text.Length; i++)
            {
                var c = text[i];

                //prev not escaped, then check splitchar
                var isSplitChar = c == splitChar;
                var isQuoteChar = c == quoteChar;
                var isEscapeChar = c == escapeChar;
                var isLastChar = i == text.Length - 1;


                if (isNewPart)
                {
                    if (isLastChar)
                    {
                        //done
                        sb.Append(c);
                        break;
                    }

                    isNewPart = false;
                    isQuoteChar = c == quoteChar;
                    isEscapeChar = c == escapeChar;

                    if (isEscapeChar)
                    {
                        //escape of the quote, if the quote is after this.

                        i++;
                        if (text.Length == i)
                        {
                            //last char
                            sb.Append(escapeChar);
                            break;
                        }

                        c = text[i];
                        if (c == quoteChar)
                        {
                            sb.Append(quoteChar);
                        }
                        else
                        {
                            sb.Append(escapeChar);
                            sb.Append(quoteChar);
                        }
                    }

                    else if (isQuoteChar)
                    {
                        //skip quoteChar
                        if (sb.Length > 0)
                            sb.Length--;
                        //isInPart = true;
                        inQuotedMode = true;
                        //todo check escape quoteChar
                    }
                    else
                    {
                        sb.Append(c);
                    }
                }

                else if (inQuotedMode)
                {
                    if (isQuoteChar)
                    {
                        if (prevIsEscape)
                        {
                            sb.Append(c);
                            break;
                        }


                        //skip quoteChar
                        i++;
                        //    isInPart = false;
                        inQuotedMode = false;
                        var part = sb.ToString();
                        //reset
                        sb.Length = 0;
                        yield return part;
                    }

                    else
                    {
                        prevIsEscape = isEscapeChar;

                        sb.Append(c);
                    }
                }
                else
                {


                    if (isSplitChar)
                    {
                        //end of part

                        var part = sb.ToString();
                        //reset
                        sb.Length = 0;
                        yield return part;

                        if (isLastChar)
                        {
                            //done
                            yield return string.Empty;
                            break;
                        }

                        isNewPart = true;

                    }
                    else
                    {
                        sb.Append(c);
                    }
                }
            }

            var lastPart = GetLastPart(sb);
            if (inQuotedMode)
            {
                //append quote back
                lastPart = quoteChar + lastPart;
            }

            if (lastPart != null)
            {
                yield return lastPart;
            }
        }

        private static string GetLastPart(StringBuilder sb)
        {
            var length = sb.Length;
            if (length > 0)
            {
                var lastPart = sb.ToString();
                return lastPart;
            }
            return null;
        }
    }
}