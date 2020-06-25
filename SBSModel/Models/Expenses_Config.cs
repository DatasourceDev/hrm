using System;
using System.Collections.Generic;

namespace SBSModel.Models
{
    public partial class Expenses_Config
    {
        public Expenses_Config()
        {
            this.Expenses_Application_Document = new List<Expenses_Application_Document>();
            this.Expenses_Calculation = new List<Expenses_Calculation>();
            this.Expenses_Config_Budget = new List<Expenses_Config_Budget>();
            this.Expenses_Config_Detail = new List<Expenses_Config_Detail>();
        }

        public int Expenses_Config_ID { get; set; }
        public Nullable<int> Department_ID { get; set; }
        public Nullable<int> Company_ID { get; set; }
        public string Expenses_Name { get; set; }
        public string Expenses_Description { get; set; }
        public string Claimable_Type { get; set; }
        public Nullable<bool> Allowed_Probation { get; set; }
        public Nullable<bool> Allowed_Over_Amount_Per_Year { get; set; }
        public string Create_By { get; set; }
        public Nullable<System.DateTime> Create_On { get; set; }
        public string Update_By { get; set; }
        public Nullable<System.DateTime> Update_On { get; set; }
        public Nullable<bool> Is_MileAge { get; set; }
        public Nullable<int> UOM_ID { get; set; }
        public Nullable<decimal> Amount_Per_UOM { get; set; }
        public Nullable<int> Expenses_Category_ID { get; set; }
        public string Record_Status { get; set; }
        public Nullable<bool> Is_Accumulative { get; set; }
        public virtual Company Company { get; set; }
        public virtual Department Department { get; set; }
        public virtual ICollection<Expenses_Application_Document> Expenses_Application_Document { get; set; }
        public virtual ICollection<Expenses_Calculation> Expenses_Calculation { get; set; }
        public virtual Expenses_Category Expenses_Category { get; set; }
        public virtual ICollection<Expenses_Config_Budget> Expenses_Config_Budget { get; set; }
        public virtual ICollection<Expenses_Config_Detail> Expenses_Config_Detail { get; set; }
        public virtual Global_Lookup_Data Global_Lookup_Data { get; set; }
    }
}
