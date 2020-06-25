using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace CivinTecAccessManager
{
    public class Transaction
    {                
        public uint CardID { get; set; }
        public DateTime DateAndTime { get; set; }
        public int ID { get; set; }
        public short JobCode { get; set; }
        public string JobCodeDesc { get; set; }
        public int Pin { get; set; }
        public short Type { get; set; }
        public string TypeName { get; set; }
        public User objUser { get; set; }
    }
}
