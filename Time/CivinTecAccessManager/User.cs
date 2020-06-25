using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CivinTecAccessManager
{
    public class User
    {
        public bool Enabled { get; set; }
        public byte FAR { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public byte Level { get; set; }
        public string MiddleName { get; set; }        
        public int Pin { get; set; }
        public byte Type { get; set; }
    }
}
