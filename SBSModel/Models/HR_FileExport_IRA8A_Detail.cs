using System;
using System.Collections.Generic;

namespace SBSModel.Models
{
    public partial class HR_FileExport_IRA8A_Detail
    {
        public int Detail_ID { get; set; }
        public int Generated_ID { get; set; }
        public int Process_Year { get; set; }
        public int Employee_Profile_ID { get; set; }
        public string Employee_No { get; set; }
        public string NRIC { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public Nullable<System.DateTime> Period_From { get; set; }
        public Nullable<System.DateTime> Period_To { get; set; }
        public Nullable<int> Days { get; set; }
        public Nullable<int> Shared_Employees { get; set; }
        public Nullable<decimal> Annual_Value { get; set; }
        public Nullable<decimal> Furniture_Value { get; set; }
        public Nullable<decimal> Rent_Paid_To_Landlord { get; set; }
        public Nullable<decimal> Taxable_AV { get; set; }
        public Nullable<decimal> TTL_Rent_PaidBy_Emp { get; set; }
        public Nullable<decimal> TTL_Taxalbe_AV { get; set; }
        public Nullable<decimal> Utilities { get; set; }
        public Nullable<decimal> Driver { get; set; }
        public Nullable<decimal> Servant { get; set; }
        public Nullable<decimal> Taxable_Utilities { get; set; }
        public Nullable<decimal> Hotel { get; set; }
        public Nullable<decimal> Amt_PaidBy_Emp { get; set; }
        public Nullable<decimal> Taxable_Hotel { get; set; }
        public Nullable<decimal> Others { get; set; }
        public Nullable<decimal> TTL_Benefit_In_Kind { get; set; }
        public string Create_By { get; set; }
        public Nullable<System.DateTime> Create_On { get; set; }
    }
}
