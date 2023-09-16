using System.Collections.Concurrent;

namespace Patterns;

public abstract class PatternsCounter<T> {
    private protected ConcurrentDictionary<string, int> _triplets = new();
    private protected const int chunkSize = 1000000;
    private protected const int parrallelLimit = 24;
    private protected Memory<byte> _buffer = new Memory<byte>(new byte[chunkSize]);
    public string FilePath { get; set; }

    public int PatternLength { get; set; }
    public int TopSize { get; set; }

    public PatternsCounter(string filePath, int patternLength, int topSize) {
        FilePath = filePath;
        PatternLength = patternLength;
        TopSize = topSize;
    }

    public abstract Task<Dictionary<string, int>> GetTopPatterns();
    private protected abstract bool IsLetter(T check);
    private protected abstract T GetDataItem(int index);
    private protected abstract KMPMatcher<T> CreateKMPMatcher();

    private protected abstract ReadOnlySpan<T> GetTextDataSlice(int startIndex, int endIndex);
    public abstract string SpanToString(ReadOnlySpan<T> span);

    private protected Task[] StartTasks(List<T> gapsSkipped, int contentLength) {
        int parallelValid = contentLength > parrallelLimit * PatternLength ? parrallelLimit : 1;
        Task[] tasks = new Task[parallelValid];

        for (int chunkNumber = 0; chunkNumber < parallelValid; chunkNumber++) {
            int start = contentLength * chunkNumber / parallelValid;
            int end = contentLength * (chunkNumber + 1) / parallelValid + PatternLength - 1;
            end = end > contentLength ? contentLength : end;
            tasks[chunkNumber] = Task.Run(() => ProcessChunk(GetTextDataSlice(start, end), start, end));
        }

        if (gapsSkipped.Count > 0) {
            int gapsEndLength = contentLength < PatternLength - 1 ? contentLength : PatternLength - 1;
            gapsSkipped.AddRange(GetTextDataSlice(0, gapsEndLength).ToArray());
            ProcessChunk(gapsSkipped.ToArray(), 0, gapsSkipped.Count);
        }

        gapsSkipped.Clear();
        if (contentLength > PatternLength + 1) {
            gapsSkipped.AddRange(GetTextDataSlice(contentLength - PatternLength + 1, contentLength).ToArray());
        }

        return tasks;
    }

    private protected void ProcessChunk(ReadOnlySpan<T> span, int startIndex, int endIndex) {
        ReadOnlySpan<T> text = span;

        HashSet<string> usedTriplets = new HashSet<string>();
        int index = startIndex;
        int matchCount = 0;
        var matcher = CreateKMPMatcher();
        while (index < endIndex) {
            if (!TryPickTriplet(index, endIndex, out int patternStart)) {
                break;
            }

            ReadOnlySpan<T> triplet = GetTextDataSlice(patternStart, patternStart + PatternLength);
            string tripletString = SpanToString(triplet).ToLower();

            index = patternStart + 1;

            if (usedTriplets.TryGetValue(tripletString, out _)) {
                continue;
            }

            matchCount = matcher.KMPSearch(text, triplet);
            usedTriplets.Add(tripletString);
            _triplets.AddOrUpdate(tripletString, matchCount, (_, v) => v + matchCount);
        }
    }

    private protected bool TryPickTriplet(int startSearchIndex, int endIndex, out int patternStart) {
        int currentSeries = 0;
        patternStart = 0;
        for (int i = startSearchIndex; i < endIndex; i++) {
            T currentItem = GetDataItem(i);
            if (IsLetter(currentItem)) {
                currentSeries++;
            } else {
                currentSeries = 0;
            }

            if (currentSeries == PatternLength) {
                patternStart = i - PatternLength + 1;
                return true;
            }
        }

        return false;
    }

    private protected Dictionary<string, int> TopPatterns() {
        var sortedList = _triplets.ToList();

        sortedList.Sort((x, y) => {
            int valueComparison = y.Value.CompareTo(x.Value);
            if (valueComparison != 0) {
                return valueComparison;
            }

            return x.Key.CompareTo(y.Key);
        });

        return sortedList.Take(TopSize).ToDictionary(item => item.Key, item => item.Value);
    }
}