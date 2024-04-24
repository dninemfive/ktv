# ktv - Command-Line A<ins>ct</ins>i<ins>v</ins>ity Tracker
A program which runs in the background keeping track of what processes are running productivity purposes. In this branch, i am merging it with [slp](https://github.com/dninemfive/slp), which similarly runs in the background and shuts down programs when i need to sleep. This ended up being a total rewrite, drastically improving ktv's performance by getting rid of the weird spooling loop i used in my first version, as well as implementing a scheduling logic which is honestly pretty cool.

## Todo:
- [x] TaskScheduler class to generalize the concept of running things later
- [x] ktv features - logging programs
  - [x] log raw data to a file
  - [x] periodically load data from a file
  - [x] generalize program details to merge them
  - [x] convert generalized details to Google Calendar events
- [~] slp features - closing programs
  - [x] program matching
  - [ ] some kind of expression syntax?
  - [ ] close programs based on how long they've been running in a given period of time
  - [x] close programs based on their ktv categorization
- [x] structure config files for json use
- [ ] pin the program to the taskbar notification section and have a UI which can be used to manage it
