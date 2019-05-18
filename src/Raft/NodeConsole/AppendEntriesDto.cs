using RaftCore.Components;
using System;
using System.Collections.Generic;
using System.Text;

namespace NodeConsole
{
    public class AppendEntriesDto
    {
        public int term { get; set; }
        public uint leaderId { get; set; }
        public int prevLogIndex { get; set; }
        public int prevLogTerm { get; set; }
        public int leaderCommit { get; set; }
        public List<LogEntry> entries { get; set; }
    }
}
