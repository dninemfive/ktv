using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace d9.ktv.slp;
/*
 * Goal: allow writing conditions readably *and* parsably in JSON, e.g.
 * === Example 1 ===
 * {
 *      "time": {
 *          "or": "between
 *      }
 * }
 * === Example 2 ===
 * 
 */