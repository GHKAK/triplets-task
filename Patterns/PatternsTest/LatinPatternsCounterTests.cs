using System.Reflection;
using System.Text;
using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using Patterns;

namespace PatternsTest;

[TestFixture]
public class LatinPatternsCounterTests {
    private LatinPatternsCounter _myPatternsCounter;

    [SetUp]
    public void Setup() {
        _myPatternsCounter = new("", 3, 10);
        string myString = "Xx9- ";
        byte[] byteArray = Encoding.UTF8.GetBytes(myString);
        Memory<byte> buffer = new Memory<byte>(byteArray);
        var bufferFieldInfo =
            typeof(LatinPatternsCounter).GetField("_buffer", BindingFlags.NonPublic | BindingFlags.Instance);
        bufferFieldInfo.SetValue(_myPatternsCounter, buffer);
    }

    [TestCase(65, ExpectedResult = true, TestName = "IsLetter_WhenInputIsUppercaseLetter_ReturnsTrue")]
    [TestCase(97, ExpectedResult = true, TestName = "IsLetter_WhenInputIsLowercaseLetter_ReturnsTrue")]
    [TestCase(48, ExpectedResult = false, TestName = "IsLetter_WhenInputIsDigit_ReturnsFalse")]
    [TestCase(1, ExpectedResult = false, TestName = "IsLetter_WhenInputIsNotLetter_ReturnsFalse")]
    public bool TestIsLetter(byte check) {
        MethodInfo methodInfo =
            typeof(LatinPatternsCounter).GetMethod("IsLetter", BindingFlags.NonPublic | BindingFlags.Instance);

        bool result = (bool)methodInfo.Invoke(_myPatternsCounter, new object[] { check });

        return result;
    }

    [TestCase(0, ExpectedResult = "X", TestName = "GetDataItem_WhenInputIsInBuffer_ReturnsCorrectByte")]
    [TestCase(1, ExpectedResult = "x", TestName = "GetDataItem_WhenInputIsInBuffer_ReturnsCorrectCaseByte")]
    public string TestGetDataItem(int index) {
        MethodInfo methodInfo =
            typeof(LatinPatternsCounter).GetMethod("GetDataItem", BindingFlags.NonPublic | BindingFlags.Instance);
        var result = (byte)methodInfo.Invoke(_myPatternsCounter, new object[] { index });
        return Encoding.UTF8.GetString(new byte[] { result });
    }

    [TestCase("Xxx", ExpectedResult = "Xxx", TestName = "SpanToString_WhenInputLetters_ReturnsCorrectString")]
    [TestCase("", ExpectedResult = "", TestName = "SpanToString_WhenInputEmpty_ReturnsEmpty")]
    [TestCase("94561Adfsываыпы", ExpectedResult = "94561Adfsываыпы",
        TestName = "SpanToString_WhenInputNotASCII_ReturnsCorrectString")]
    public string TestGetTextDataSlice(string str) {
        byte[] byteArray = Encoding.UTF8.GetBytes(str);
        ReadOnlySpan<byte> span = new ReadOnlySpan<byte>(byteArray);
        var result = _myPatternsCounter.SpanToString(span);
        return result;
    }

    [TestCase("aaaa", "aaa", 2, TestName = "GetTopPatterns_WhenLettersCaseSame_TrueAssert")]
    [TestCase("AAAAaa", "aaa", 4, TestName = "GetTopPatterns_WhenLettersCaseDifferent_TrueAssert")] 
    [TestCase("XXX", "xxx", 1, TestName = "GetTopPatterns_WhenOnlyPatternCaseSame_TrueAssert")]
    [TestCase("XxX", "xxx", 1, TestName = "GetTopPatterns_WhenOnlyPatternCaseLadder_TrueAssert")]

    public async Task TestGetTopPatterns(string fileText, string expectedTop1Pattern, int expecteTop1dCount) {
        string filePath = Path.Combine(Environment.CurrentDirectory, "text.txt");
        RemakeFile(filePath, fileText);

        _myPatternsCounter.FilePath = filePath;
        var bufferFieldInfo =
            typeof(LatinPatternsCounter).GetField("_buffer", BindingFlags.NonPublic | BindingFlags.Instance);
        
        var result = await _myPatternsCounter.GetTopPatterns();
        
        var kvPair = result.ToList()[0];
        Assert.IsTrue(expectedTop1Pattern == kvPair.Key && expecteTop1dCount == kvPair.Value);
    }
    [TestCase("aaaa", ExpectedResult = 1, TestName = "GetTopPatterns_WhenRepeatedPattern_ReturnValid")]
    [TestCase("aabaa", ExpectedResult = 3, TestName = "GetTopPatterns_WhenPatternDifferent_ReturnValid")]
    [TestCase("aaaAAAA", ExpectedResult = 1, TestName = "GetTopPatterns_WhenRepeatedPatternCasesDiff_ReturnValid")]
    [TestCase("12345646479841 42t5435t 34?//// 435re e s da fa   rg ", ExpectedResult = 0, 
        TestName = "GetTopPatterns_WhenNoLetterPattern_ReturnValid")]

    public async Task<int> TestGetTopPatternsResultSize(string fileText) {
        string filePath = Path.Combine(Environment.CurrentDirectory, "text.txt");
        RemakeFile(filePath, fileText);

        _myPatternsCounter.FilePath = filePath;
        
        var result = await _myPatternsCounter.GetTopPatterns();
        return result.Count;
    }
    public static void RemakeFile(string filePath, string fileText) {
        if (File.Exists(filePath)) {
            File.Delete(filePath);
        }
        using (StreamWriter writer = new StreamWriter(filePath)) {
            writer.Write(fileText);
        }
    }
}