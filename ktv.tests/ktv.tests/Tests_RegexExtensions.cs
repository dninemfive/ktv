namespace d9.ktv.tests;

[TestClass]
public class Tests_RegexExtensions
{
    [TestMethod]
    public void Test_NullValue()
    {
        Assert.AreEqual("asdf", "asdf".RegexReplace(("test", null, ".+")));
    }
    [TestMethod]
    public void Test_NullRegex()
    {
        Assert.AreEqual("asdf", "asdf".RegexReplace(("test", "jkl;", null)));
    }
    [TestMethod]
    public void Test_NoInterpolation()
    {
        Assert.AreEqual("asdf", "asdf".RegexReplace(("test", "jkl;", ".+")));
    }
    [TestMethod]
    public void Test_OneMatch()
    {
        Assert.AreEqual("asdfqwerty", "asdf{test}".RegexReplace(("test", "qwerty", "qwer(.+)")));
        Assert.AreEqual("asdfqwerty", "asdf{test:0}".RegexReplace(("test", "qwerty", "qwer(.+)")));
        Assert.AreEqual("asdfqwerty", "asdf{test:0,0}".RegexReplace(("test", "qwerty", "qwer(.+)")));
    }
    [TestMethod]
    public void Test_OneMatchOneGroup()
    {
        Assert.AreEqual("asdfty", "asdf{test:0,1}".RegexReplace(("test", "qwerty", "qwer(.+)")));
    }
    [TestMethod]
    public void Test_MultipleMatches()
    {
        Assert.AreEqual($"Aqwertyupty", $"A{{test:0,0}}y".RegexReplace(("test", "asdfqwertyuptyqwersy3", "qwer(.{2})?(.{3})?")));
        Assert.AreEqual($"tyohu", $"{{test:0,1}}ohu".RegexReplace(("test", "asdfqwertyuptyqwersy3", "qwer(.{2})?(.{3})?")));
        Assert.AreEqual($"uptdrigo", $"{{test:0,2}}drigo".RegexReplace(("test", "asdfqwertyuptyqwersy3", "qwer(.{2})?(.{3})?")));
        Assert.AreEqual($"qwersy", $"{{test:1,0}}".RegexReplace(("test", "asdfqwertyuptyqwersy3", "qwer(.{2})?(.{3})?")));
        Assert.AreEqual($"Asy", $"A{{test:1,1}}".RegexReplace(("test", "asdfqwertyuptyqwersy3", "qwer(.{2})?(.{3})?")));
    }
}