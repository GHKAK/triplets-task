namespace Patterns; 

public static class KMPMatcher {
    public static int KMPSearch(ReadOnlySpan<byte> text, ReadOnlySpan<byte> pattern) {
        int t = 0; // position of character in text
        int p = 0; // position of character in pattern

        int n = text.Length;
        int m = pattern.Length;

        int matches = 0;
        int[] prefix = CalcPrefix(pattern);
        while (t < n) {
            if (AreLettersInBytesEquals(pattern[p], text[t])) {
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

    private static int[] CalcPrefix(ReadOnlySpan<byte> pattern) {
        int M = pattern.Length;
        int[] prefix = new int[M+1];
        prefix[0] = -1;
        prefix[1] = 0;

        int len = 0;
        int i = 1;
        while (i < M) {
            if (AreLettersInBytesEquals(pattern[i] , pattern[len])) {
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
    
    //Case-insensitive comparison of bytes that actually latin alphabet letter in ASCII code
    private static bool AreLettersInBytesEquals(byte byte1, byte byte2) {
        if (byte1 == byte2) {
            return true;
        }

        if ((byte1 >= 65 && byte2 == byte1 + 32) || (byte1 >= 97 && byte2 == byte1 - 32)) {
            return true;
        }

        return false;
    }
}