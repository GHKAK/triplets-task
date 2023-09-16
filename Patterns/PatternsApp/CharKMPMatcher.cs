namespace Patterns;

public class CharKMPMatcher : KMPMatcher<char> {
    //Case-sensitive comparison of chars, first text in lowered
    private protected  override bool AreLettersEquals(char char1, char char2) {
        return char1.CompareTo(char2) == 0;
    }
}