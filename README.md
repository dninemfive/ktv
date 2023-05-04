# ktv - Command-Line A<ins>ct</ins>i<ins>v</ins>ity Tracker
A (relatively) simple script to track what program i'm using in various time blocks, to try and shame myself into actually doing work.

Usage (using [utl](https://github.com/dninemfive/utl)'s command-line arg parsing):

```
./ktv.exe [--LogInterval <double>] [--AggregationInterval <double>]
```

`LogInterval`: the amount of time, in minutes, the program waits between logging your current active window title. The default is `0.5`, or 30 seconds.

`AggregationInterval`: the amount of time, in minutes, the program waits between aggregating the logged window titles, to better summarize your activity. The default is `15` minutes.

Once started, the program waits five seconds before logging anything. It aggregates each time the current time is evenly divisible by the aggregation interval, starting at the earliest such time.

For example, if started at 1:37pm, the first aggregation time with the default settings would be 1:45pm.

---

i would like to make this into a GUI program at some point, but that'd probably be even more procrastination than this script has been.