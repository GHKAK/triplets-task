using System.Text;
using System.Xml.Serialization;

namespace Triplets; 

public class TextGenerator {
    private Random _random = new Random();

    private string[] _words = {
        "I'm", "skill", "plus", "minus", "man", "algorithm", "computer", "character", "general", "form", "performing",
        "woman", "our", "logarithm", "aaabbbb", "bbbaa", "cccaabbaacc"
    };
    string[] _punctuation = { ".", ",", "!", "?", ";", ":","\n", "\t" };

    public void GenerateRandomText(string filePath, int wordsCount) {
        using (StreamWriter writer = new StreamWriter(filePath)) {
            for (int i = 0; i < wordsCount; i++) {
                writer.Write(_words[_random.Next(_words.Length)]);
                writer.Write(_punctuation[_random.Next(_punctuation.Length)]);
                writer.Write(" ");
            }
        }
    }
}