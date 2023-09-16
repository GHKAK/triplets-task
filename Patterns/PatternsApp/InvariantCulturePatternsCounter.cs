using System.Text;

namespace Patterns;

public class InvariantCulturePatternsCounter : PatternsCounter<char> {
    private string _text;

    public InvariantCulturePatternsCounter(string filePath, int patternLength, int topSize) : base(filePath,
        patternLength, topSize) {
    }

    public override async Task<Dictionary<string, int>> GetTopPatterns() {
        int bytesRead;
        using (FileStream fs = new FileStream(_filePath, FileMode.Open, FileAccess.Read, FileShare.None, chunkSize)) {
            while ((bytesRead = await fs.ReadAsync(_buffer)) > 0) {
                _text = Encoding.UTF8.GetString(_buffer.Span.Slice(0, bytesRead));
                var tasks = StartTasks(_text.Length);
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
    private protected override string SpanToString(ReadOnlySpan<char> span) {
        return span.ToString();
    }
}