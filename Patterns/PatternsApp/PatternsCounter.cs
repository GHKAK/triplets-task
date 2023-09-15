using System.Collections.Concurrent;
using System.Text;

namespace Patterns;

public class PatternsCounter {
    private ConcurrentDictionary<string, int> _triplets = new();
    private const int chunkSize = 1000000;
    private const int parrallelLimit = 24;
    private Memory<byte> _buffer = new Memory<byte>(new byte[chunkSize]);
    private readonly string _filePath;
    private readonly int _patternLength;
    private readonly int _topSize;
    
    public PatternsCounter(string filePath, int patternLength, int topSize) {
        _filePath = filePath;
        _patternLength = patternLength;
        _topSize = topSize;
    }
    public async Task<Dictionary<string, int>> GetTopPatterns() {
        int bytesRead;
        using (FileStream fs = new FileStream(_filePath, FileMode.Open, FileAccess.Read, FileShare.None, chunkSize)) {
            while ((bytesRead = await fs.ReadAsync(_buffer)) > 0) {
                Task[] tasks = new Task[parrallelLimit];
                 for (int chunkNumber = 0; chunkNumber < parrallelLimit; chunkNumber++)
                 {
                     int start = bytesRead * chunkNumber / parrallelLimit;
                     int end = bytesRead * (chunkNumber + 1) / parrallelLimit + _patternLength-1;
                     end = end > chunkSize ? chunkSize : end;
                     tasks[chunkNumber] = Task.Run(() => ProcessChunk(start, end));
                 }
                await Task.WhenAll(tasks);
                
            }
        }
        Console.WriteLine("finished");

        return TopPatterns();
    }

    private void ProcessChunk(int startIndex, int endIndex) {
        ReadOnlySpan<byte> text = _buffer.Span.Slice(startIndex, endIndex-startIndex);
        string strtext = Encoding.UTF8.GetString(text);

        HashSet<string> usedTriplets = new HashSet<string>();
        int index = startIndex;
        int matchCount = 0;
        endIndex = endIndex > chunkSize ? chunkSize - 1 : endIndex;
        while (index < endIndex) {
            if (!TryPickTriplet(index, endIndex, out int patternStart)) {
                break;
            }

            ReadOnlySpan<byte> triplet = _buffer.Span.Slice(patternStart, _patternLength);
            string tripletString = Encoding.UTF8.GetString(triplet).ToLower();

            index = patternStart + 1;

            if (usedTriplets.TryGetValue(tripletString, out _)) {
                continue;
            }

            matchCount = KMPMatcher.KMPSearch(text, triplet);
            usedTriplets.Add(tripletString);
            _triplets.AddOrUpdate(tripletString, matchCount, (_, v) => v + matchCount);
        }
    }
    private bool TryPickTriplet(int startSearchIndex, int endIndex, out int patternStart) {
        int patternLength = 0;
        patternStart = 0;
        for (int i = startSearchIndex; i < endIndex - patternLength; i++) {
            byte currentByte = _buffer.Span[i];
            if (IsByteLetter(currentByte)) {
                patternLength++;
            } else {
                patternLength = 0;
            }

            if (patternLength == _patternLength) {
                patternStart = i - _patternLength+1;
                return true;
            }
        }

        return false;
    }
    private  bool IsByteLetter(byte checkByte) {
        return (checkByte >= 65 && checkByte <= 90) || (checkByte >= 97 && checkByte <= 122);
    }
    
    private Dictionary<string, int> TopPatterns() {
        var sortedList = _triplets.ToList();

        sortedList.Sort((x, y) =>
        {
            int valueComparison = y.Value.CompareTo(x.Value);
            if (valueComparison != 0)
            {
                return valueComparison;
            }
            return x.Key.CompareTo(y.Key); 
        });

        return sortedList.Take(_topSize).ToDictionary(item => item.Key, item => item.Value);
    }
}