using d9.utl.compat;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace d9.ktv;
internal class KtvConfigDef2
{
    public required ActivityConfig Activities { get; set; }
}