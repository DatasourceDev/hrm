using System;
using System.Collections.Generic;

namespace SBSModel.Models
{
    public partial class Designation
    {
        public Designation()
        {
            this.Employment_History = new List<Employment_History>();
            this.Expenses_Config_Detail = new List<Expenses_Config_Detail>();
            this.Leave_Calculation = new List<Leave_Calculation>();
            this.Leave_Config_Detail = new List<Leave_Config_Detail>();
        }

        public int Designation_ID { get; set; }
        public int Company_ID { get; set; }
        public string Name { get; set; }
        public string Record_Status { get; set; }
        public string Create_By { get; set; }
        public Nullable<System.DateTime> Create_On { get; set; }
        public string Update_By { get; set; }
        public Nullable<System.DateTime> Update_On { get; set; }
        public Nullable<int> Employee_Grading_ID { get; set; }
        public virtual Company Company { get; set; }
        public virtual ICollection<Employment_History> Employment_History { get; set; }
        public virtual ICollection<Expenses_Config_Detail> Expenses_Config_Detail { get; set; }
        public virtual ICollection<Leave_Calculation> Leave_Calculation { get; set; }
        public virtual ICollection<Leave_Config_Detail> Leave_Config_Detail { get; set; }
    }
}
