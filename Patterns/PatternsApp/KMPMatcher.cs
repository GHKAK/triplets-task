namespace Patterns; 

public abstract class KMPMatcher<T> {
    public int KMPSearch(ReadOnlySpan<T> text, ReadOnlySpan<T> pattern) {
        int t = 0; // position of character in text
        int p = 0; // position of character in pattern

        int n = text.Length;
        int m = pattern.Length;

        int matches = 0;
        int[] prefix = CalcPrefix(pattern);
        while ((n - t) >= (m - p)) {
            if (AreLettersEquals(pattern[p], text[t])) {
                p++;
                t++;
            } 
            else if (t < n) {
                if (p != 0)
                    p = prefix[p - 1];
                else
                    t += 1;
            }
            if (p == m) {
                matches++;
                p = prefix[p - 1];
            }
        }
        return matches;
    }

    private  int[] CalcPrefix(ReadOnlySpan<T> pattern) {
        int M = pattern.Length;
        int[] prefix = new int[M];
        prefix[0] = 0;

        int len = 0;
        int i = 1;
        while (i < M) {
            if (AreLettersEquals(pattern[i] , pattern[len])) {
                len++;
                prefix[i] = len;
                i++;
            }
            else 
            {
                if (len != 0) {
                    len = prefix[len - 1];
                }
                else 
                {
                    prefix[i] = len;
                    i++;
                }
            }
        }
        return prefix;
    }
     private protected  abstract bool AreLettersEquals(T byte1, T byte2);
}