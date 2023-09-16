This console app makes search of top 10 triplets(three letters in a row) in text.
Text path in Program.cs with name filePath.
UniversalPatternCounter operates with ReadOnlySpan<char> consumes many alphabets.
LatinPatternsCounter operates with ReadOnlySpan<byte> consumes only files with ASCII symbols.
For search of pattern in text KMP algorithm is used.
UniversalPatternCounter patternLength and top size may be edited.