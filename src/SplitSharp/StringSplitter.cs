using System;
using System.Collections.Generic;
using System.Text;
using static System.Diagnostics.Debug;

namespace SplitSharp
{
    public class StringSplitter
    {
        public static IEnumerable<string> SplitQuoted(string template, char delimiter, char quote, char escape)
        {
            if (template == null)
                throw new ArgumentNullException(nameof(template));
            var parser = new StringSplitter(template, delimiter, quote, escape);
            return parser.Parse();
        }

        private char _delimiter; // = ',';
        private char _escape; // = ',';
        /// <summary>
        /// Escape for Quote
        /// </summary>
        private char _quote;// = '\'';
        private string _template;
        private int _templateLength;
        private int _currentPos;
        private int _prevPos;

        private StringSplitter(string template, char delimiter, char quote, char escape)
        {
            _template = template;
            _delimiter = delimiter;
            _quote = quote;
            _escape = escape;
            _templateLength = template.Length;
        }


        private IEnumerable<string> Parse()
        {

            while (!IsDone())
            {
                if (Peek() == _quote && PrevPeek() != _escape)
                {
                    yield return ReadQuoted();
                }
                else
                {
                    yield return ReadUnQuoted();
                }


            }
            if (_currentPos != _prevPos)
            {
                yield return GetSubString();
            }

            //if (_currentPos == _templateLength - 1)
            //{
            //    //get last past
            //    _currentPos++;
            //    yield return GetSubString();
            //}

            //return null;

        }

        private string ReadQuoted()
        {
            SkipFirstChar(_quote);
            //read quoted
            while (!IsDone())
            {
                var found = ReadUntil(_quote);
                if (!found)
                {
                    //started with quote, but not ended. Restore start _quote
                    var text = _quote + GetSubString();
                    //_currentPos = _templateLength;//done
                    return text;
                }
                //find until quote with isn't escaped
                if (PrevPeek() != _escape)
                {
                    var text = GetSubString();
                    SkipFirstChar(_quote);
                    return text;
                }
                //skip
                SkipPrevChar(_escape);
                _currentPos++; //read 
            }
            //end of template
            return GetSubString();
        }

        /// <summary>
        /// Read unoquted, skip last delimiter
        /// </summary>
        /// <returns></returns>
        private string ReadUnQuoted()
        {
            var found = ReadUntil(_delimiter);
            var text = GetSubString();
            //skip delimiter
            if (found)
            {
                SkipFirstChar(_delimiter);
            }
            return text;
        }

        private List<int> _skipCharIndexes;

        private void SkipCurrentChar()
        {
            _skipCharIndexes = _skipCharIndexes ?? new List<int>();
            _skipCharIndexes.Add(_currentPos);
        }

        private void SkipFirstChar(char c)
        {
            var pos = _currentPos;
            Assert(_template[pos] == c);
            if (_prevPos == _currentPos)
            {
                _currentPos++;
            }
            _prevPos++;
        }

        private void SkipPrevChar(char c)
        {
            if (_currentPos > 0)
            {
                _skipCharIndexes = _skipCharIndexes ?? new List<int>();
                var prevPos = _currentPos - 1;
                Assert(_template[prevPos] == c);
                _skipCharIndexes.Add(prevPos);
            }
        }


        private bool IsDone() => _currentPos >= _templateLength;

        private char? PrevPeek()
        {
            char? nullChar = null;
            // ReSharper disable once ExpressionIsAlwaysNull
            return _currentPos == 0 ? nullChar : _template[_currentPos - 1];
        }

        private char Peek() => _template[_currentPos];

        private char Read() => _template[_currentPos++];

        // ReSharper disable once UnusedParameter.Local
        private void Skip(char c)
        {
            // Can be out of bounds, but never in correct use (expects a required char).
            Assert(_template[_currentPos] == c);
            _prevPos++;
        }

        private void SkipUntil(char search)
        {
            int i = _template.IndexOf(search, _currentPos);
            if (i == -1)
            {
                _prevPos = i;
            }
            else
            {

            }
        }

        /// <summary>
        /// Read until search or to end
        /// </summary>
        /// <param name="search"></param>
        /// <returns>found <paramref name="search"/>?</returns>
        private bool ReadUntil(char search)
        {
            int i = _template.IndexOf(search, _currentPos);
            if (i > -1)
            {
                _currentPos = i;
                return true;
            }
            //read to end
            _currentPos = _templateLength;
            return false;
        }



        private string GetSubString()
        {
            var length = _currentPos - _prevPos;
            if (length <= 0)
            {
                return string.Empty;
            }
            var substring = _template.Substring(_prevPos, length);
            _prevPos = _currentPos;
            if (_skipCharIndexes != null)
            {
                var sb = new StringBuilder(substring);
                foreach (var skipCharIndex in _skipCharIndexes)
                {
                    sb.Remove(skipCharIndex, 1);

                }

                _skipCharIndexes = null;
                return sb.ToString();
            }
            return substring;
        }

        public static IEnumerable<string> SplitWithSelfEscape(string input, char splitCharAndEscapeChar)
        {
            throw new NotImplementedException();
        }

        public static IEnumerable<string> SplitWithEscape(string input, char splitChar, char escapeChar)
        {
            throw new NotImplementedException();
        }
    }
}
