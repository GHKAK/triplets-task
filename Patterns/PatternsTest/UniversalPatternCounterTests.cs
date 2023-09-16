using System.Reflection;
using System.Text;
using Patterns;

namespace PatternsTest; 

public class UniversalPatternCounterTests {
    private UniversalPatternCounter _myPatternsCounter;

    [SetUp]
    public void Setup() {
        _myPatternsCounter = new("", 3, 10);
        string myString = "Xx9- ";
        var bufferFieldInfo =
            typeof(UniversalPatternCounter).GetField("_text", BindingFlags.NonPublic | BindingFlags.Instance);
        bufferFieldInfo.SetValue(_myPatternsCounter, myString);
    }
    
    [TestCase('x', ExpectedResult = true, TestName = "IsLetter_WhenInputIsUppercaseLetter_ReturnsTrue")]
    [TestCase('ы', ExpectedResult = true, TestName = "IsLetter_WhenInputIsLowercaseLetter_ReturnsTrue")]
    [TestCase('X', ExpectedResult = true, TestName = "IsLetter_WhenInputIsLetterASCIIUpper_ReturnsTrue")]
    [TestCase('8', ExpectedResult = false, TestName = "IsLetter_WhenInputIsDigit_ReturnsFalse")]
    [TestCase('\t', ExpectedResult = false, TestName = "IsLetter_WhenInputIsNotLetter_ReturnsFalse")]
    public bool TestIsLetter(char check) {
        MethodInfo methodInfo =
            typeof(UniversalPatternCounter).GetMethod("IsLetter", BindingFlags.NonPublic | BindingFlags.Instance);

        bool result = (bool)methodInfo.Invoke(_myPatternsCounter, new object[] { check });

        return result;
    }
    
    [TestCase(0, ExpectedResult = "X", TestName = "GetDataItem_WhenInputIsInBuffer_ReturnsCorrectByte")]
    [TestCase(1, ExpectedResult = "x", TestName = "GetDataItem_WhenInputIsInBuffer_ReturnsCorrectCaseByte")]
    [TestCase(2, ExpectedResult = "9", TestName = "GetDataItem_WhenInputIsInBuffer_ReturnsCorrectCaseByte")]

    public string TestGetDataItem(int index) {
        MethodInfo methodInfo =
            typeof(UniversalPatternCounter).GetMethod("GetDataItem", BindingFlags.NonPublic | BindingFlags.Instance);
        var result = (char)methodInfo.Invoke(_myPatternsCounter, new object[] { index });
        return result.ToString();
    }
    [TestCase("Xxx", ExpectedResult = "Xxx", TestName = "SpanToString_WhenInputLetters_ReturnsCorrectString")]
    [TestCase("", ExpectedResult = "", TestName = "SpanToString_WhenInputEmpty_ReturnsEmpty")]
    [TestCase("94561Adfsываыпы", ExpectedResult = "94561Adfsываыпы",
        TestName = "SpanToString_WhenInputNotASCII_ReturnsCorrectString")]
    
    public string TestGetTextDataSlice(string str) {
        ReadOnlySpan<char> span = str.AsSpan();
        var result = _myPatternsCounter.SpanToString(span);
        return result;
    }
    [TestCase("aaaa", "aaa", 2, TestName = "GetTopPatterns_WhenLettersCaseSame_TrueAssert")]
    [TestCase("AAAAaa", "aaa", 4, TestName = "GetTopPatterns_WhenLettersCaseDifferent_TrueAssert")] 
    [TestCase("XXX", "xxx", 1, TestName = "GetTopPatterns_WhenOnlyPatternCaseSame_TrueAssert")]
    [TestCase("XxX", "xxx", 1, TestName = "GetTopPatterns_WhenOnlyPatternCaseLadder_TrueAssert")]
    [TestCase("ББб бб ббб \t \r БББ", "ббб", 3, TestName = "GetTopPatterns_WhenPatternsWithSpacesKirillic_TrueAssert")]
    [TestCase("ББб бб aaA \t \r aaa", "aaa", 2, TestName = "GetTopPatterns_WhenPatternsWithDifferentKirillic_TrueAssert")]

    public async Task TestGetTopPatterns(string fileText, string expectedTop1Pattern, int expecteTop1dCount) {
        string filePath = Path.Combine(Environment.CurrentDirectory, "text.txt");
        LatinPatternsCounterTests.RemakeFile(filePath, fileText);

        _myPatternsCounter.FilePath = filePath;

        var result = await _myPatternsCounter.GetTopPatterns();
        
        var kvPair = result.ToList()[0];
        Assert.IsTrue(expectedTop1Pattern == kvPair.Key && expecteTop1dCount == kvPair.Value);
    }
    [TestCase("aaaa", ExpectedResult = 1, TestName = "GetTopPatterns_WhenRepeatedPattern_ReturnValid")]
    [TestCase("aabaa", ExpectedResult = 3, TestName = "GetTopPatterns_WhenPatternDifferent_ReturnValid")]
    [TestCase("aaaAAAA", ExpectedResult = 1, TestName = "GetTopPatterns_WhenRepeatedPatternCasesDiff_ReturnValid")]
    [TestCase("бббф", ExpectedResult = 2, TestName = "GetTopPatterns_WhenKirillicSameCase_ReturnValid")]
    [TestCase("бббАВ", ExpectedResult = 3, TestName = "GetTopPatterns_WhenKirillicDifferentCase_ReturnValid")]
    [TestCase("бб бА В \t\r", ExpectedResult = 0, TestName = "GetTopPatterns_WhenKirillicNotPatterns_ReturnValid")]
    [TestCase("ZSЫж \t\r", ExpectedResult = 2, TestName = "GetTopPatterns_WhenAlphabetsMess_ReturnValid")]
    [TestCase("12345646479841 42t5435t 34?//// 435re e s da fa   rg ", ExpectedResult = 0, 
        TestName = "GetTopPatterns_WhenNoLetterPattern_ReturnValid")]

    public async Task<int> TestGetTopPatternsResultSize(string fileText) {
        string filePath = Path.Combine(Environment.CurrentDirectory, "text.txt");
        LatinPatternsCounterTests.RemakeFile(filePath, fileText);

        _myPatternsCounter.FilePath = filePath;
        
        var result = await _myPatternsCounter.GetTopPatterns();
        return result.Count;
    }
    
}