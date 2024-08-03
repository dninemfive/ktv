using d9.utl;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace d9.ktv;
public class ConfigReloader(KtvConfig config, Log log)
    : FixedPeriodTaskScheduler(TimeSpan.FromMinutes(config.ReloadPeriod), log)
{
}
