using d9.utl.compat;

namespace d9.ktv.tests;
[TestClass]
public class Tests_Extensions
{
    [TestMethod]
    public void Test_IsInt()
    {
        Assert.IsTrue(2.0.IsInt());
    }
    [TestMethod]
    public void Test_IsInt_IsNotInt()
    {
        Assert.IsFalse(2.5.IsInt());
    }
    [TestMethod]
    public void Test_ToColorId()
    {
        foreach(int i in Enum.GetValuesAsUnderlyingType<GoogleUtils.EventColor>())
            Assert.AreEqual(i.ToString(), ((GoogleUtils.EventColor)i).ToColorId());
    }
    [TestMethod]
    public void Test_toCamelCase_0LengthString()
    {
        Assert.AreEqual("", "".toCamelCase());
    }
    [TestMethod]
    public void Test_toCamelCase_1LengthString()
    {
        Assert.AreEqual("a", "a".toCamelCase());
        Assert.AreEqual("a", "A".toCamelCase());
    }
    [TestMethod]
    public void Test_toCamelCase()
    {
        Assert.AreEqual("anExampleString", "AnExampleString".toCamelCase());
        Assert.AreEqual("anexamplestring", "anexamplestring".toCamelCase());
        Assert.AreEqual("anExampleString", "anExampleString".toCamelCase());
    }
    [TestMethod]
    public void Test_MultilineListWithAlignedTitle()
    {
        Assert.AreEqual("list:   test\n" +
                        "        item2", Extensions.MultilineListWithAlignedTitle(["test", "item2"], "list:"));
    }
}
