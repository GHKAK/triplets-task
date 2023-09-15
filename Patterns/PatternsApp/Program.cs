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
var result = await new PatternsCounter(filePath,4,10).GetTopPatterns();
foreach (var pair in result) {
    Console.WriteLine($"{pair.Key}  --- {pair.Value} times");
}
Console.WriteLine($"Elapsed time {sw.ElapsedMilliseconds} ms");
