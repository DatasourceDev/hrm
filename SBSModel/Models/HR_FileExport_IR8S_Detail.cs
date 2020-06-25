using System;
using System.Collections.Generic;

namespace SBSModel.Models
{
    public partial class HR_FileExport_IR8S_Detail
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
        public string MTH_OW_OEplrCPF_OEmpCPF_AW_AEplrCPF_AEmpCPF { get; set; }
        public Nullable<decimal> TTL_Emloyer_CPF { get; set; }
        public Nullable<decimal> TTL_Employee_CPF { get; set; }
        public Nullable<decimal> AW_1 { get; set; }
        public Nullable<decimal> RefundToEmployer_1 { get; set; }
        public Nullable<decimal> InterestToEmployer_1 { get; set; }
        public Nullable<decimal> RefundToEmployee_1 { get; set; }
        public Nullable<decimal> InterestToEmployee_1 { get; set; }
        public Nullable<decimal> AW_2 { get; set; }
        public Nullable<decimal> RefundToEmployer_2 { get; set; }
        public Nullable<decimal> InterestToEmployer_2 { get; set; }
        public Nullable<decimal> RefundToEmployee_2 { get; set; }
        public Nullable<decimal> InterestToEmployee_2 { get; set; }
        public Nullable<decimal> AW_3 { get; set; }
        public Nullable<decimal> RefundToEmployer_3 { get; set; }
        public Nullable<decimal> InterestToEmployer_3 { get; set; }
        public Nullable<decimal> RefundToEmployee_3 { get; set; }
        public Nullable<decimal> InterestToEmployee_3 { get; set; }
        public string Create_By { get; set; }
        public Nullable<System.DateTime> Create_On { get; set; }
        public Nullable<decimal> Excess_Emloyer_CPF { get; set; }
        public Nullable<decimal> Excess_Employee_CPF { get; set; }
        public virtual HR_FileExport_History HR_FileExport_History { get; set; }
    }
}
