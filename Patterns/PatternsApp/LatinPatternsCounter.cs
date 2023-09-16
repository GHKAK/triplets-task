using System.Collections.Concurrent;
using System.Net;
using System.Text;

namespace Patterns;

public class LatinPatternsCounter : PatternsCounter<byte> {
    public LatinPatternsCounter(string filePath, int patternLength, int topSize) : base(filePath, patternLength,
        topSize) {
    }

    public override async Task<Dictionary<string, int>> GetTopPatterns() {
        int bytesRead;
        using (FileStream fs = new FileStream(_filePath, FileMode.Open, FileAccess.Read, FileShare.None, chunkSize)) {
            while ((bytesRead = await fs.ReadAsync(_buffer)) > 0) {
                var tasks = StartTasks(bytesRead);
                await Task.WhenAll(tasks);
            }
        }


        return TopPatterns();
    }
    
    private protected override bool IsLetter(byte check) {
        return (check >= 65 && check <= 90) || (check >= 97 && check <= 122);
    }

    private protected override byte GetDataItem(int index) {
        return _buffer.Span[index];
    }

    private protected override KMPMatcher<byte> CreateKMPMatcher() {
        return new BytesKMPMatcher();
    }

    private protected override ReadOnlySpan<byte> GetTextDataSlice(int startIndex, int endIndex) {
        return _buffer.Span.Slice(startIndex, endIndex - startIndex);
    }
    private protected override string SpanToString(ReadOnlySpan<byte> span) {
        return Encoding.UTF8.GetString(span);
    }
}