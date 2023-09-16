using System.Collections.Concurrent;

namespace Patterns;

public abstract class PatternsCounter<T> {
    private protected ConcurrentDictionary<string, int> _triplets = new();
    private protected const int chunkSize = 1000000;
    private protected const int parrallelLimit = 24;
    private protected Memory<byte> _buffer = new Memory<byte>(new byte[chunkSize]);
    private protected readonly string _filePath;
    private protected readonly int _patternLength;
    private protected readonly int _topSize;

    public PatternsCounter(string filePath, int patternLength, int topSize) {
        _filePath = filePath;
        _patternLength = patternLength;
        _topSize = topSize;
    }

    public abstract Task<Dictionary<string, int>> GetTopPatterns(); 
    //private protected abstract void ProcessChunk(int startIndex, int endIndex);
    private protected abstract bool IsLetter(T check);
    private protected abstract T GetDataItem(int index);
    private protected abstract KMPMatcher<T> CreateKMPMatcher();
    private protected abstract ReadOnlySpan<T> GetTextDataSlice(int startIndex, int endIndex);
    private protected abstract string SpanToString(ReadOnlySpan<T> span);

    private protected Task[] StartTasks(int contentLength) {
        int parallelValid = contentLength > parrallelLimit * _patternLength ? parrallelLimit : 1;
        Task[] tasks = new Task[parallelValid];

        for (int chunkNumber = 0; chunkNumber < parallelValid; chunkNumber++) {
            int start = contentLength * chunkNumber / parallelValid;
            int end = contentLength * (chunkNumber + 1) / parallelValid + _patternLength - 1;
            end = end > contentLength ? contentLength : end;
            tasks[chunkNumber] = Task.Run(() => ProcessChunk(GetTextDataSlice(start, end ),start, end));
        }

        return tasks;
    }

    private protected  void ProcessChunk(ReadOnlySpan<T> span, int startIndex, int endIndex)
    {
        ReadOnlySpan<T> text = span;

        HashSet<string> usedTriplets = new HashSet<string>();
        int index = startIndex;
        int matchCount = 0;
        endIndex = endIndex > chunkSize ? chunkSize - 1 : endIndex;
        var matcher = CreateKMPMatcher();
        while (index < endIndex)
        {
            if (!TryPickTriplet(index, endIndex, out int patternStart))
            {
                break;
            }

            ReadOnlySpan<T> triplet = GetTextDataSlice(patternStart, patternStart + _patternLength);
            string tripletString = SpanToString(triplet).ToLower();

            index = patternStart + 1;

            if (usedTriplets.TryGetValue(tripletString, out _))
            {
                continue;
            }

            matchCount = matcher.KMPSearch(text, triplet);
            usedTriplets.Add(tripletString);
            _triplets.AddOrUpdate(tripletString, matchCount, (_, v) => v + matchCount);
        }
    }
    private protected bool TryPickTriplet(int startSearchIndex, int endIndex, out int patternStart) {
        int patternLength = 0;
        patternStart = 0;
        for (int i = startSearchIndex; i < endIndex - patternLength; i++) {
            T currentItem = GetDataItem(i);
            if (IsLetter(currentItem)) {
                patternLength++;
            } else {
                patternLength = 0;
            }

            if (patternLength == _patternLength) {
                patternStart = i - _patternLength + 1;
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

        return sortedList.Take(_topSize).ToDictionary(item => item.Key, item => item.Value);
    }
}