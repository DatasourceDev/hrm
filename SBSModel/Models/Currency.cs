using System;
using System.Collections.Generic;

namespace SBSModel.Models
{
    public partial class Currency
    {
        public Currency()
        {
            this.Company_Details = new List<Company_Details>();
            this.Employment_History = new List<Employment_History>();
            this.Expenses_Application_Document = new List<Expenses_Application_Document>();
            this.PRDs = new List<PRD>();
        }

        public int Currency_ID { get; set; }
        public Nullable<int> Country_ID { get; set; }
        public string Currency_Code { get; set; }
        public string Currency_Name { get; set; }
        public string Create_By { get; set; }
        public Nullable<System.DateTime> Create_On { get; set; }
        public string Update_By { get; set; }
        public Nullable<System.DateTime> Update_On { get; set; }
        public virtual ICollection<Company_Details> Company_Details { get; set; }
        public virtual Country Country { get; set; }
        public virtual ICollection<Employment_History> Employment_History { get; set; }
        public virtual ICollection<Expenses_Application_Document> Expenses_Application_Document { get; set; }
        public virtual ICollection<PRD> PRDs { get; set; }
    }
}
