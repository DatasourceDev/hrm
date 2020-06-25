using System;
using System.Collections.Generic;

namespace SBSModel.Models
{
    public partial class HR_FileExport_History_Detail
    {
        public int Detail_ID { get; set; }
        public Nullable<int> Generated_ID { get; set; }
        public Nullable<int> Process_Year { get; set; }
        public Nullable<int> Process_Month { get; set; }
        public Nullable<int> Employee_Profile_ID { get; set; }
        public string Employee_No { get; set; }
        public string NRIC { get; set; }
        public string Name { get; set; }
        public Nullable<int> Department_ID { get; set; }
        public Nullable<int> Designation_ID { get; set; }
        public Nullable<decimal> CPF_Contribution { get; set; }
        public Nullable<decimal> CPF_Employee { get; set; }
        public Nullable<decimal> Ordinary_Wages { get; set; }
        public Nullable<decimal> Additional_Wages { get; set; }
        public Nullable<int> Race { get; set; }
        public string Residential_Status { get; set; }
        public string Create_By { get; set; }
        public Nullable<System.DateTime> Create_On { get; set; }
        public string Update_By { get; set; }
        public Nullable<System.DateTime> Update_On { get; set; }
        public Nullable<decimal> SDL { get; set; }
        public Nullable<decimal> MBMF { get; set; }
        public Nullable<decimal> SIDA { get; set; }
        public Nullable<decimal> CDAC { get; set; }
        public Nullable<decimal> ECF { get; set; }
        public virtual Global_Lookup_Data Global_Lookup_Data { get; set; }
        public virtual HR_FileExport_History HR_FileExport_History { get; set; }
    }
}
