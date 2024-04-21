using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace d9.ktv;
public class GoogleCalendarEventInfo(DateTime startTime, DateTime endTime, string? id = null)
{
    public DateTime StartTime { get; private set; } = startTime;
    public DateTime EndTime { get; set; } = endTime;
    public string? Id { get; set; } = id;
}
