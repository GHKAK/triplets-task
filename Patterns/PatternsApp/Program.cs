using System.Diagnostics;
using Patterns;

int wordsCount = 1000000;
string projectFolderPath = Directory.GetParent(Directory.GetCurrentDirectory()).Parent.Parent.FullName;
string filePath = Path.Combine(projectFolderPath, "text.txt");
if (!File.Exists(filePath)) {
    new TextGenerator().GenerateRandomText(filePath, wordsCount);
}

Stopwatch sw = new();
sw.Start();
var result = await new LatinPatternsCounter(filePath,3,10).GetTopPatterns();
foreach (var pair in result) {
    Console.WriteLine($"{pair.Key}  --- {pair.Value} times");
}
Console.WriteLine($"Elapsed time {sw.ElapsedMilliseconds} ms");
sw.Restart();
var result2 = await new InvariantCulturePatternsCounter(filePath,3,10).GetTopPatterns();
foreach (var pair in result2) {
    Console.WriteLine($"{pair.Key}  --- {pair.Value} times");
}
Console.WriteLine($"Elapsed time {sw.ElapsedMilliseconds} ms");