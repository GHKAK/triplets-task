using System.Text;

namespace Patterns;

public class UniversalPatternCounter : PatternsCounter<char> {
    private string _text;

    public UniversalPatternCounter(string filePath, int patternLength, int topSize) : base(filePath,
        patternLength, topSize) {
    }

    public override async Task<Dictionary<string, int>> GetTopPatterns() {
        int bytesRead;
        List<char> gapsSkipped = new();
        using (FileStream fs = new FileStream(FilePath, FileMode.Open, FileAccess.Read, FileShare.None, chunkSize)) {
            while ((bytesRead = await fs.ReadAsync(_buffer)) > 0) {
                _text = Encoding.UTF8.GetString(_buffer.Span.Slice(0, bytesRead)).ToLower();
                var tasks = StartTasks(gapsSkipped, _text.Length);
                await Task.WhenAll(tasks);
            }
        }
        return TopPatterns();
    }
    
    
    private protected override bool IsLetter(char check) {
        return char.IsLetter(check);
    }

    private protected override char GetDataItem(int index) {
        return _text[index];
    }
    private protected override KMPMatcher<char> CreateKMPMatcher() {
        return new CharKMPMatcher();
    }
    private protected override ReadOnlySpan<char> GetTextDataSlice(int startIndex, int endIndex) {
        return _text.AsSpan().Slice(startIndex, endIndex - startIndex);
    }
    public override string SpanToString(ReadOnlySpan<char> span) {
        return span.ToString();
    }
}