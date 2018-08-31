using System;
using System.Collections.Generic;
using System.Text;

namespace csharp_example
{
    class OutgoingSMS
    {
        public List<string> Numbers { get; set; }
        public List<int> Contacts { get; set; }
        public List<int> Groups { get; set; }
        public bool Email { get; set; }
        public int Prio { get; set; }
        public string From { get; set; }
        public string Message { get; set; }
    }
}
