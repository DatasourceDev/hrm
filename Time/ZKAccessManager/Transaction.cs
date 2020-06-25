using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZKAccessManager
{
    public class Transaction
    {
        public uint CardID { get; set; } // Card ID
        public DateTime DateAndTime { get; set; } // Transactio Date and Time
        public int ID { get; set; } // Transaction ID
        public short JobCode { get; set; } // In-Out
        public string JobCodeDesc { get; set; } // Transaction Type
        public int Pin { get; set; } // User Pin
        public short Type { get; set; } // Transaction Type
        public string TypeName { get; set; } // Transaction Type
        public User objUser { get; set; }
    }
}
