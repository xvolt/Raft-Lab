using System;
using System.Collections.Generic;
using System.Text;

namespace NodeConsole
{
    class Configuration
    {
        public Node CurrentNode { get; set; }
        public Node[] Nodes { get; set; }
    }
    public class Node
    {
        public string BaseUrl { get; set; }
        public uint Id { get; set; }
    }
}
