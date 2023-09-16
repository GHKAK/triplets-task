namespace Patterns; 

public  class BytesKMPMatcher: KMPMatcher<byte> {
    //Case-insensitive comparison of bytes that actually latin alphabet letter in ASCII 
    private protected override bool AreLettersEquals(byte byte1, byte byte2) {
        if (byte1 == byte2) {
            return true;
        }

        if ((byte1 >= 65 && byte2 == byte1 + 32) || (byte1 >= 97 && byte2 == byte1 - 32)) {
            return true;
        }

        return false;
    }
}