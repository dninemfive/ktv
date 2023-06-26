using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace d9.ktv
{
    internal static class CalendarManager
    {
        internal static IReadOnlyDictionary<string, string> ActiveEventIds => _activeEventIds;
        private static readonly Dictionary<string, string> _activeEventIds = new();
        /// <summary>
        /// Adds (or updates, if already present) the activities in the ActivityRecord specified 
        /// to the calendar, if possible.
        /// </summary>
        /// <param name="activityRecord">
        /// The ActivityRecord with which to update the calendar.<br/><br/>
        /// <b>NOTE:</b> there are no checks that the activity is actually contiguous with the 
        /// previous ones; that's the caller's problem.
        /// </param>
        internal static void Update(ActivityRecord activityRecord)
        {
            
        }
    }
}
