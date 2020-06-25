using System;
using System.Collections.Generic;

namespace SBSModel.Models
{
    public partial class HR_FileExport_IR8B_Detail
    {
        public int Detail_ID { get; set; }
        public int Generated_ID { get; set; }
        public int Process_Year { get; set; }
        public int Employee_Profile_ID { get; set; }
        public string Employee_No { get; set; }
        public string NRIC { get; set; }
        public string Name { get; set; }
        public Nullable<int> Department_ID { get; set; }
        public Nullable<int> Designation_ID { get; set; }
        public string Sex { get; set; }
        public Nullable<decimal> A_Gross_Amt_NotQualify { get; set; }
        public Nullable<decimal> A_Gross_Amt_Gained { get; set; }
        public Nullable<decimal> B_Gross_Amt_NotQualify { get; set; }
        public Nullable<decimal> B_Gross_Amt_Gained { get; set; }
        public Nullable<decimal> C_Gross_Amt_NotQualify { get; set; }
        public Nullable<decimal> C_Gross_Amt_Gained { get; set; }
        public Nullable<decimal> D_Gross_Amt_NotQualify { get; set; }
        public Nullable<decimal> D_Gross_Amt_Gained { get; set; }
        public string Create_By { get; set; }
        public Nullable<System.DateTime> Create_On { get; set; }
    }
}
