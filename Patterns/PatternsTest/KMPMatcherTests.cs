using Patterns;

namespace PatternsTest;

public class KMPMatcherTests {
    private CharKMPMatcher _matcher;

    [SetUp]
    public void Setup() {
        _matcher = new();
    }

    [TestCase("aaa", "aaaa", ExpectedResult = 2, TestName = "KMPSearch_WhenLatinHasPattern_ReturnsExpected")]
    [TestCase("aaa", "AAAA", ExpectedResult = 0, TestName = "KMPSearch_WhenLatinHasPatternButDiffCase_ReturnsExpected")]
    [TestCase("ffa", "ffafsdf sad", ExpectedResult = 1, TestName = "KMPSearch_WhenLatinHasPattern_ReturnsExpected")]
    [TestCase("0", "ffa", ExpectedResult = 0, TestName = "KMPSearch_WhenLatinHasDigit_ReturnsExpected")]
    [TestCase("рус", "русрусрус", ExpectedResult = 3, TestName = "KMPSearch_WhenKirillicCycle_ReturnsExpected")]
    [TestCase("absolutely", "absolutely", ExpectedResult = 1, TestName = "KMPSearch_WhenLatinLong_ReturnsExpected")]
    [TestCase("a", "a a a a a ", ExpectedResult = 5, TestName = "KMPSearch_WhenLatinWithSpaces_ReturnsExpected")]

    public int KMPSearchTest(string pattern, string text) {
        return _matcher.KMPSearch(text.AsSpan(), pattern.AsSpan());
    }
}