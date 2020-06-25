using System;
using System.Collections.Generic;

namespace SBSModel.Models
{
    public partial class Default_Expense_Type
    {
        public int Default_Expense_Type_ID { get; set; }
        public int Expense_Category_ID { get; set; }
        public string Expense_Type_Name { get; set; }
        public string Expense_Type_Desc { get; set; }
        public virtual Global_Lookup_Data Global_Lookup_Data { get; set; }
    }
}
