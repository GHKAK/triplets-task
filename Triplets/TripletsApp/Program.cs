using Triplets;

int wordsCount = 100000;
string projectFolderPath = Directory.GetParent(Directory.GetCurrentDirectory()).Parent.Parent.FullName;
string filePath = Path.Combine(projectFolderPath, "text.txt");
if (!File.Exists(filePath)) {
    new TextGenerator().GenerateRandomText(filePath, wordsCount);
}
await new TripletsCounter().CountTriplets(filePath);