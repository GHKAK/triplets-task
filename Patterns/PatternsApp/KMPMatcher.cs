namespace Patterns; 

public abstract class KMPMatcher<T> {
    public int KMPSearch(ReadOnlySpan<T> text, ReadOnlySpan<T> pattern) {
        int t = 0; // position of character in text
        int p = 0; // position of character in pattern

        int n = text.Length;
        int m = pattern.Length;

        int matches = 0;
        int[] prefix = CalcPrefix(pattern);
        while (t < n) {
            if (AreLettersEquals(pattern[p], text[t])) {
                p++;
                t++;

                if (p == m) {
                    matches++;
                    p = prefix[p];
                }
            } else {
                p = prefix[p];
                if (p < 0) {
                    p++;
                    t++;
                }
            }
        }

        return matches;
    }

    private   int[] CalcPrefix(ReadOnlySpan<T> pattern) {
        int M = pattern.Length;
        int[] prefix = new int[M+1];
        prefix[0] = -1;
        prefix[1] = 0;

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
                if (len > 0) {
                    len = prefix[len];
                }
                else
                {
                    i++;
                    prefix[i] = len;
                }
            }
        }

        return prefix;
    }
     private protected  abstract bool AreLettersEquals(T byte1, T byte2);
}