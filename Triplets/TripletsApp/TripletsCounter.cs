using System.Collections.Concurrent;
using System.Text;

namespace Triplets;

public class TripletsCounter {
    private ConcurrentDictionary<string, int> _triplets = new();
    private const int chunkSize = 1000000;
    private const int parrallelLimit = 4;
    private Memory<byte> _buffer = new Memory<byte>(new byte[chunkSize]);

    public async Task<Dictionary<string, int>> CountTriplets(string filePath) {
        int bytesRead;
        using (FileStream fs = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.None, chunkSize)) {
            while ((bytesRead = await fs.ReadAsync(_buffer)) > 0) {
                for (int i = 0; i < bytesRead; i++) {
                    Console.WriteLine($"{_buffer.Span[i]} {Encoding.UTF8.GetString(new byte[] { _buffer.Span[i] })}");
                }

                Parallel.For(0, parrallelLimit,
                    (int chunkNumber) => {
                        ProcessChunk(bytesRead * chunkNumber / parrallelLimit,
                            bytesRead * (chunkNumber + 1) / parrallelLimit + 2);
                    });
            }
        }

        return GetTopTriplets(10);
    }

    private void ProcessChunk(int startIndex, int endIndex) {
        HashSet<string> usedTriplets = new HashSet<string>();
        int index = startIndex;
        int matchCount = 0;
        endIndex = endIndex > chunkSize ? chunkSize - 1 : endIndex;
        while (index < endIndex) {
            int patternStart = PickTriplet(index, usedTriplets);
            if (patternStart == endIndex - 1) {
                break;
            }
            ReadOnlySpan<byte> triplet = _buffer.Span.Slice(patternStart, 3);
            string tripletString = Encoding.UTF8.GetString(triplet).ToLower();
            matchCount = KMPSearch(triplet);
            usedTriplets.Add(tripletString);
            _triplets.AddOrUpdate(tripletString, matchCount, (k, v) => v + matchCount);
            index = patternStart + 1;
        }
    }

    private int PickTriplet(int startSearchIndex,HashSet<string> usedTriplets) {
        throw new NotImplementedException();
    }

    private int KMPSearch(ReadOnlySpan<byte> pattern) {
        throw new NotImplementedException();

    }

    public Dictionary<string, int> GetTopTriplets(int topSize) {
        Dictionary<string, int> topTriplets = new Dictionary<string, int>(topSize);
        return topTriplets;
    }
}