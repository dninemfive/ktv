using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using d9.utl.compat.google;

namespace d9.ktv.tests;
[TestClass]
public class Tests_ProcessMatchModeImplementation
{
    public ProcessMatchModeImplementation Pmmi = new(new KtvConfig()
    {
        ActivityTracker = new()
        {
            LogPeriodMinutes = 1,
            AggregationConfig = new()
            {
                DefaultCategoryName = "default",
                CategoryDefs = new()
                {
                    { "games", new()
                        {
                            ActivityDefs = new()
                            {
                                new()
                                {
                                    Patterns = new()
                                    {
                                        { ProcessPropertyTarget.FileName, @".+\\Steam\\steamapps\\common\\([^\\]+).+" }
                                    },
                                    Format = "{fileName:0,1}"
                                },
                                new()
                                {
                                    Patterns = new()
                                    {
                                        { ProcessPropertyTarget.MainWindowTitle, @"(Minecraft\*? \d+\.\d+)" }
                                    },
                                    Format = "{mainWindowTitle:0,1}"
                                }
                            },
                            EventColor = GoogleCalendar.EventColor.Banana
                        }
                    }
                },
                PeriodMinutes = 2
            }
        }
    });
    [TestMethod]
    public void Test_IsInCategory_Matcher()
    {
        Assert.IsTrue(Pmmi.IsInCategory("games", new ProcessSummary(@"C:\Users\dninemfive\Documents\mmc-stable-win32\MultiMC\instances\1.17.1\.minecraft",
                                                                     "Minecraft* 1.21",
                                                                     "javaw")));
        Assert.IsFalse(Pmmi.IsInCategory("games", new ProcessSummary(@"C:\Users\dninemfive\Music\_foobar2000\foobar2000.exe",
                                                                      "C418: Minecraft - Volume Alpha - Key [foobar2000]",
                                                                      "foobar2000")));
    }
}
