using System;
using System.Collections.Generic;

namespace SBSModel.Models
{
    public partial class HR_FileExport_IR8A_Detail
    {
        public int Detail_ID { get; set; }
        public Nullable<int> Generated_ID { get; set; }
        public Nullable<int> Process_Year { get; set; }
        public Nullable<int> Employee_Profile_ID { get; set; }
        public string Employee_No { get; set; }
        public string NRIC { get; set; }
        public string Name { get; set; }
        public Nullable<int> Department_ID { get; set; }
        public Nullable<int> Designation_ID { get; set; }
        public Nullable<decimal> Total_Amount { get; set; }
        public Nullable<decimal> Donation { get; set; }
        public Nullable<decimal> CPF_Desgn_Pension { get; set; }
        public Nullable<decimal> Insurance { get; set; }
        public Nullable<decimal> Salary { get; set; }
        public Nullable<decimal> Bonus { get; set; }
        public Nullable<decimal> Directors_Fee { get; set; }
        public Nullable<decimal> Others { get; set; }
        public Nullable<decimal> Gross_Commission { get; set; }
        public Nullable<decimal> Pension { get; set; }
        public string Create_By { get; set; }
        public Nullable<System.DateTime> Create_On { get; set; }
    }
}
