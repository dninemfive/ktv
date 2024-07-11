using d9.utl;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace d9.ktv;
public class ProcessMatchModeImplementation : EnumImplementation<ProcessMatchMode, ProcessMatcher>
{
    public static readonly ProcessMatchModeImplementation Instance = new();
}
