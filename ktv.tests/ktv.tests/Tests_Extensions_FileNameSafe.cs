using d9.ktv;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ktv.tests;
[TestClass]
public class Tests_Extensions_FileNameSafe
{
    [TestMethod]
    public void Test_EmptyString()
    {
        Assert.ThrowsException<ArgumentException>(() => "".FileNameSafe());
        Assert.ThrowsException<ArgumentException>(() => "/".FileNameSafe());
        Assert.ThrowsException<ArgumentException>(() => " ".FileNameSafe());
    }
    [TestMethod]
    public void Test_NoReplacements()
    {
        Assert.AreEqual("expected", "expected".FileNameSafe());
    }
    [TestMethod]
    public void Test_Trim()
    {
        Assert.AreEqual("expected", "  expected ".FileNameSafe());
        Assert.AreEqual("exp e   cted", "exp e   cted".FileNameSafe());
        Assert.AreEqual("bracket", "< bracket >".FileNameSafe());
    }
    [TestMethod]
    public void Test_Replacements()
    {
        Assert.AreEqual("expected", @"<expected\/?*".FileNameSafe());
        Assert.AreEqual("expected_result_here", "expected|result?here".FileNameSafe("_"));
    }
}
