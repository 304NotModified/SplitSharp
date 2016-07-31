# SplitSharp

[![Build status](https://ci.appveyor.com/api/projects/status/2c7ola6p5e3t6nlh?svg=true)](https://ci.appveyor.com/project/304NotModified/splitsharp)
[![codecov](https://codecov.io/gh/304NotModified/SplitSharp/branch/master/graph/badge.svg)](https://codecov.io/gh/304NotModified/SplitSharp)


SplitSharp, Easy split string with escapes or quotes

## Usage

```c#
//split on comma, escape backslash
"a,b,c\,d".SplitWithEscape(',', '\\'); // ["a", "b", "c,d"]

//split and escape on quote
"a'b'c''d".SplitWithSelfEscape('\''); // ["a", "b", "c'd"]

//split quoted values: split on comma, use single quotes (and optional escape on single quotes)
"a,b,'c,d'".SplitQuoted(',', '\'', '\''); // ["a", "b", "c,d"]

//no need to escape quote if not after separator 
"a,b,c'd".SplitQuoted(',', '\'', '\''); // ["a", "b", "c'd"]

//quote after separator could be escaped
"a,b,''c,d".SplitQuoted(',', '\'', '\''); // ["a", "b", "'c,d"]
```
