using System;
using System.Collections.Generic;

namespace SBSModel.Models
{
    public partial class Job_Cost
    {
        public Job_Cost()
        {
            this.Expenses_Application_Document = new List<Expenses_Application_Document>();
            this.Expenses_Config_Budget = new List<Expenses_Config_Budget>();
            this.Job_Cost_Payment_Term = new List<Job_Cost_Payment_Term>();
            this.Time_Sheet_Dtl = new List<Time_Sheet_Dtl>();
        }

        public int Job_Cost_ID { get; set; }
        public Nullable<int> Company_ID { get; set; }
        public string Indent_No { get; set; }
        public string Indent_Name { get; set; }
        public Nullable<int> Customer_ID { get; set; }
        public Nullable<System.DateTime> Date_Of_Date { get; set; }
        public Nullable<decimal> Sell_Price { get; set; }
        public Nullable<System.DateTime> Delivery_Date { get; set; }
        public Nullable<decimal> Term_Of_Deliver { get; set; }
        public Nullable<decimal> Warranty_Term { get; set; }
        public Nullable<decimal> Costing { get; set; }
        public string Record_Status { get; set; }
        public string Create_By { get; set; }
        public Nullable<System.DateTime> Create_On { get; set; }
        public string Update_By { get; set; }
        public Nullable<System.DateTime> Update_On { get; set; }
        public Nullable<bool> Using { get; set; }
        public Nullable<int> Supervisor { get; set; }
        public virtual Company Company { get; set; }
        public virtual Customer Customer { get; set; }
        public virtual Employee_Profile Employee_Profile { get; set; }
        public virtual ICollection<Expenses_Application_Document> Expenses_Application_Document { get; set; }
        public virtual ICollection<Expenses_Config_Budget> Expenses_Config_Budget { get; set; }
        public virtual ICollection<Job_Cost_Payment_Term> Job_Cost_Payment_Term { get; set; }
        public virtual ICollection<Time_Sheet_Dtl> Time_Sheet_Dtl { get; set; }
    }
}
