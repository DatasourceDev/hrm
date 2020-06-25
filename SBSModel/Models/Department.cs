using System;
using System.Collections.Generic;

namespace SBSModel.Models
{
    public partial class Department
    {
        public Department()
        {
            this.Employment_History = new List<Employment_History>();
            this.Expenses_Application_Document = new List<Expenses_Application_Document>();
            this.Expenses_Config = new List<Expenses_Config>();
            this.Leave_Adjustment = new List<Leave_Adjustment>();
            this.PRC_Department = new List<PRC_Department>();
            this.PREDLs = new List<PREDL>();
        }

        public int Department_ID { get; set; }
        public int Company_ID { get; set; }
        public string Name { get; set; }
        public string Record_Status { get; set; }
        public string Create_By { get; set; }
        public Nullable<System.DateTime> Create_On { get; set; }
        public string Update_By { get; set; }
        public Nullable<System.DateTime> Update_On { get; set; }
        public virtual Company Company { get; set; }
        public virtual ICollection<Employment_History> Employment_History { get; set; }
        public virtual ICollection<Expenses_Application_Document> Expenses_Application_Document { get; set; }
        public virtual ICollection<Expenses_Config> Expenses_Config { get; set; }
        public virtual ICollection<Leave_Adjustment> Leave_Adjustment { get; set; }
        public virtual ICollection<PRC_Department> PRC_Department { get; set; }
        public virtual ICollection<PREDL> PREDLs { get; set; }
    }
}
