using System;
using System.Collections.Generic;

namespace SBSModel.Models
{
    public partial class Expenses_Category
    {
        public Expenses_Category()
        {
            this.Expenses_Config = new List<Expenses_Config>();
        }

        public int Expenses_Category_ID { get; set; }
        public string Category_Name { get; set; }
        public string Category_Description { get; set; }
        public string Record_Status { get; set; }
        public Nullable<int> Company_ID { get; set; }
        public string Create_By { get; set; }
        public Nullable<System.DateTime> Create_On { get; set; }
        public string Update_By { get; set; }
        public Nullable<System.DateTime> Update_On { get; set; }
        public virtual Company Company { get; set; }
        public virtual ICollection<Expenses_Config> Expenses_Config { get; set; }
    }
}
