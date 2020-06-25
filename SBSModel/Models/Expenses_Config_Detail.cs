using System;
using System.Collections.Generic;

namespace SBSModel.Models
{
    public partial class Expenses_Config_Detail
    {
        public int Expenses_Config_Detail_ID { get; set; }
        public Nullable<int> Expenses_Config_ID { get; set; }
        public Nullable<int> Designation_ID { get; set; }
        public Nullable<decimal> Amount_Per_Year { get; set; }
        public Nullable<bool> Select_Pecentage { get; set; }
        public Nullable<bool> Select_Amount { get; set; }
        public Nullable<decimal> Amount { get; set; }
        public Nullable<decimal> Pecentage { get; set; }
        public Nullable<int> Year_Service { get; set; }
        public Nullable<int> Group_ID { get; set; }
        public string Create_By { get; set; }
        public Nullable<System.DateTime> Create_On { get; set; }
        public string Update_By { get; set; }
        public Nullable<System.DateTime> Update_On { get; set; }
        public Nullable<decimal> Amount_Per_Month { get; set; }
        public Nullable<bool> Select_Per_Month { get; set; }
        public virtual Designation Designation { get; set; }
        public virtual Expenses_Config Expenses_Config { get; set; }
    }
}
