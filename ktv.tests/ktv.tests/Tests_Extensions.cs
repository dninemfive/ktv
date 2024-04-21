using d9.ktv;

namespace d9.ktv.tests;

[TestClass]
public class Tests_Extensions
{
    [TestMethod]
    public void Test_RegexReplace_NullValue()
    {
        Assert.AreEqual("asdf", "asdf".RegexReplace("test", null, ".+"));
    }
    [TestMethod]
    public void Test_RegexReplace_NullRegex()
    {
        Assert.AreEqual("asdf", "asdf".RegexReplace("test", "jkl;", null));
    }
    [TestMethod]
    public void Test_RegexReplace_NoInterpolation()
    {
        Assert.AreEqual("asdf", "asdf".RegexReplace("test", "jkl;", ".+"));
    }
    [TestMethod]
    public void Test_RegexReplace_OneMatch()
    {
        Assert.AreEqual("asdfty", "asdf{test:1}".RegexReplace("test", "qwerty", "qwer(.+)"));
    }
}