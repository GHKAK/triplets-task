namespace Patterns;

public class CharKMPMatcher : KMPMatcher<char> {
    //Case-sensitive comparison of chars can be replaced
    private protected  override bool AreLettersEquals(char char1, char char2) {
        return char1.CompareTo(char2) == 0;
        //return String.Equals(char1.ToString(), char2.ToString(), StringComparison.InvariantCultureIgnoreCase);;
    }
}