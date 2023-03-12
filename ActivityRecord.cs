using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ktv
{
    internal class ActivityRecord
    {
        public DateTime StartedAt { get; private set; }
        public DateTime EndedAt { get; private set; }
        public string Activity { get; private set; }
        public ActivityRecord(string activity)
        {

            EndedAt = DateTime.Now;
            Activity = activity;
        }
    }
}
