using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZKAccessManager
{
    public class User
    {
        public bool Enabled { get; set; }
        public byte FAR { get; set; }
        public string Name { get; set; }
        public byte Level { get; set; }
        public int Pin { get; set; }
        public byte Type { get; set; }
    }
}
