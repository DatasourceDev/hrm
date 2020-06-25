using System;
using System.Collections.Generic;

namespace SBSModel.Models
{
    public partial class Company_Details
    {
        public int Company_Detail_ID { get; set; }
        public int Company_ID { get; set; }
        public string Name { get; set; }
        public string Phone { get; set; }
        public string Address { get; set; }
        public string Billing_Address { get; set; }
        public Nullable<int> Country_ID { get; set; }
        public string Email { get; set; }
        public string Website { get; set; }
        public string Fax { get; set; }
        public Nullable<int> State_ID { get; set; }
        public string Registry { get; set; }
        public string Zip_Code { get; set; }
        public Nullable<bool> Is_Billing_same { get; set; }
        public Nullable<int> Billing_Country_ID { get; set; }
        public Nullable<int> Billing_State_ID { get; set; }
        public string Billing_Zip_Code { get; set; }
        public string Billing_Tagline { get; set; }
        public string Report_Footer { get; set; }
        public Nullable<int> Currency_ID { get; set; }
        public System.DateTime Effective_Date { get; set; }
        public string Tax_No { get; set; }
        public Nullable<int> Schduler_Range_Days { get; set; }
        public Nullable<System.TimeSpan> Purchase_Lead_Time { get; set; }
        public Nullable<int> Security_Days { get; set; }
        public string Company_Status { get; set; }
        public string Belong_To { get; set; }
        public string Tagline { get; set; }
        public Nullable<System.DateTime> Registration_Date { get; set; }
        public Nullable<int> Belong_To_ID { get; set; }
        public string URL { get; set; }
        public string Business_Type { get; set; }
        public string GST_Registration { get; set; }
        public Nullable<bool> Default_Fiscal_Year { get; set; }
        public Nullable<System.DateTime> Custom_Fiscal_Year { get; set; }
        public string APIUsername { get; set; }
        public string APIPassword { get; set; }
        public string APISignature { get; set; }
        public Nullable<int> No_Of_Employees { get; set; }
        public string Company_Level { get; set; }
        public string CPF_Submission_No { get; set; }
        public string Create_By { get; set; }
        public Nullable<System.DateTime> Create_On { get; set; }
        public string Update_By { get; set; }
        public Nullable<System.DateTime> Update_On { get; set; }
        public string patUser_ID { get; set; }
        public string patPassword { get; set; }
        public string Company_Source { get; set; }
        public string PayerID_Type { get; set; }
        public string PayerID_No { get; set; }
        public Nullable<int> Branch_ID { get; set; }
        public Nullable<bool> Is_Sandbox { get; set; }
        public string A7_Group_ID { get; set; }
        public Nullable<bool> Is_PostPaid { get; set; }
        public Nullable<System.DateTime> Leave_Start_Date { get; set; }
        public Nullable<bool> Is_Indent { get; set; }
        public virtual Company Company { get; set; }
        public virtual Company Company1 { get; set; }
        public virtual Country Country { get; set; }
        public virtual State State { get; set; }
        public virtual Country Country1 { get; set; }
        public virtual Currency Currency { get; set; }
        public virtual State State1 { get; set; }
    }
}
