using d9.ktv;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ktv.tests;
[TestClass]
public class Tests_ActiveWindowLogEntry
{
    public static ActiveWindowLogEntry TestAwle => new(new DateTime(2024, 4, 26), "processName", "mainWindowTitle", "fileName");
    [TestMethod]
    public void Test_Indexer()
    {
        Assert.AreEqual("processName", TestAwle[ProcessPropertyTarget.ProcessName]);
        Assert.AreEqual("mainWindowTitle", TestAwle[ProcessPropertyTarget.MainWindowTitle]);
        Assert.AreEqual("fileName", TestAwle[ProcessPropertyTarget.FileName]);
        Assert.ThrowsException<ArgumentOutOfRangeException>(() => TestAwle[(ProcessPropertyTarget)(-1)]);
    }
    [TestMethod]
    public void Test_AnyPropertyContains()
    {
        Assert.IsTrue(TestAwle.AnyPropertyContains("processName"));
        Assert.IsTrue(TestAwle.AnyPropertyContains("file"));
        Assert.IsTrue(TestAwle.AnyPropertyContains("e"));
        Assert.IsFalse(TestAwle.AnyPropertyContains("z"));
        Assert.IsFalse(TestAwle.AnyPropertyContains("mainProcessWindowTitle"));
    }
}
