using System.Diagnostics;
using Patterns;

string projectFolderPath = Directory.GetParent(Directory.GetCurrentDirectory()).Parent.Parent.FullName;
string filePath = Path.Combine(projectFolderPath, "text.txt");

if (!File.Exists(filePath)) {
    new TextGenerator().GenerateRandomText(filePath);
}

Stopwatch sw = new();
sw.Start();

var result = await new UniversalPatternCounter(filePath,3,10).GetTopPatterns();
//var result = await new LatinPatternsCounter(filePath,3,10).GetTopPatterns();
foreach (var pair in result) {
    Console.WriteLine($"{pair.Key}  --- {pair.Value} times");
}
Console.WriteLine($"Elapsed time {sw.ElapsedMilliseconds} ms");