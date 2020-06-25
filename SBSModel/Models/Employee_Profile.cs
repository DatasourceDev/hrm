using System;
using System.Collections.Generic;

namespace SBSModel.Models
{
    public partial class Employee_Profile
    {
        public Employee_Profile()
        {
            this.Banking_Info = new List<Banking_Info>();
            this.Employee_Attachment = new List<Employee_Attachment>();
            this.Employee_Emergency_Contact = new List<Employee_Emergency_Contact>();
            this.Time_Mobile_Map = new List<Time_Mobile_Map>();
            this.Time_Sheet = new List<Time_Sheet>();
            this.TsEXes = new List<TsEX>();
            this.Employment_History = new List<Employment_History>();
            this.Employment_History1 = new List<Employment_History>();
            this.ETIRA8 = new List<ETIRA8>();
            this.Expenses_Application_Document = new List<Expenses_Application_Document>();
            this.Expenses_Application = new List<Expenses_Application>();
            this.Expenses_Calculation = new List<Expenses_Calculation>();
            this.Job_Cost = new List<Job_Cost>();
            this.Leave_Adjustment = new List<Leave_Adjustment>();
            this.Leave_Application_Document = new List<Leave_Application_Document>();
            this.Leave_Calculation = new List<Leave_Calculation>();
            this.Leave_Config_Extra = new List<Leave_Config_Extra>();
            this.PRALs = new List<PRAL>();
            this.PRELs = new List<PREL>();
            this.PRMs = new List<PRM>();
            this.Relationships = new List<Relationship>();
        }

        public int Employee_Profile_ID { get; set; }
        public Nullable<int> Profile_ID { get; set; }
        public Nullable<int> Nationality_ID { get; set; }
        public string CVResume { get; set; }
        public string Position_Apply { get; set; }
        public string Mobile_No { get; set; }
        public Nullable<System.DateTime> DOB { get; set; }
        public Nullable<int> Gender { get; set; }
        public string Certification { get; set; }
        public string Passport { get; set; }
        public string Porsonal_Details { get; set; }
        public string NRIC { get; set; }
        public Nullable<int> Marital_Status { get; set; }
        public string Remark { get; set; }
        public string Emp_Status { get; set; }
        public Nullable<int> Race { get; set; }
        public string Emergency_Name { get; set; }
        public string Emergency_Contact_No { get; set; }
        public Nullable<int> Emergency_Relationship { get; set; }
        public string Residential_No { get; set; }
        public string Residential_Address_1 { get; set; }
        public string Residential_Address_2 { get; set; }
        public string Postal_Code_1 { get; set; }
        public string Postal_Code_2 { get; set; }
        public string FIN_No { get; set; }
        public string Residential_Status { get; set; }
        public string PR_No { get; set; }
        public Nullable<System.DateTime> PR_Start_Date { get; set; }
        public Nullable<System.DateTime> PR_End_Date { get; set; }
        public string Immigration_No { get; set; }
        public Nullable<int> WP_Class { get; set; }
        public string WP_No { get; set; }
        public Nullable<System.DateTime> WP_Start_Date { get; set; }
        public Nullable<System.DateTime> WP_End_Date { get; set; }
        public Nullable<System.DateTime> Contract_Start_Date { get; set; }
        public Nullable<System.DateTime> Contract_End_Date { get; set; }
        public string Employee_No { get; set; }
        public Nullable<int> Religion { get; set; }
        public Nullable<System.DateTime> Expiry_Date { get; set; }
        public Nullable<System.DateTime> Confirm_Date { get; set; }
        public Nullable<System.DateTime> Hired_Date { get; set; }
        public Nullable<bool> Opt_Out { get; set; }
        public Nullable<System.DateTime> NRIC_FIN_Issue_Date { get; set; }
        public Nullable<System.DateTime> NRIC_FIN_Expire_Date { get; set; }
        public Nullable<System.DateTime> Passport_Issue_Date { get; set; }
        public Nullable<System.DateTime> Passpor_Expire_Date { get; set; }
        public Nullable<int> Work_Pass_Type { get; set; }
        public Nullable<bool> Contribute_Rate1 { get; set; }
        public Nullable<bool> Contribute_Rate2 { get; set; }
        public string Create_By { get; set; }
        public Nullable<System.DateTime> Create_On { get; set; }
        public string Update_By { get; set; }
        public Nullable<System.DateTime> Update_On { get; set; }
        public Nullable<int> Residential_Country_1 { get; set; }
        public Nullable<int> Residential_Country_2 { get; set; }
        public virtual ICollection<Banking_Info> Banking_Info { get; set; }
        public virtual Country Country { get; set; }
        public virtual Country Country1 { get; set; }
        public virtual ICollection<Employee_Attachment> Employee_Attachment { get; set; }
        public virtual ICollection<Employee_Emergency_Contact> Employee_Emergency_Contact { get; set; }
        public virtual ICollection<Time_Mobile_Map> Time_Mobile_Map { get; set; }
        public virtual ICollection<Time_Sheet> Time_Sheet { get; set; }
        public virtual ICollection<TsEX> TsEXes { get; set; }
        public virtual Global_Lookup_Data Global_Lookup_Data { get; set; }
        public virtual Global_Lookup_Data Global_Lookup_Data1 { get; set; }
        public virtual Global_Lookup_Data Global_Lookup_Data2 { get; set; }
        public virtual Nationality Nationality { get; set; }
        public virtual Global_Lookup_Data Global_Lookup_Data3 { get; set; }
        public virtual Global_Lookup_Data Global_Lookup_Data4 { get; set; }
        public virtual User_Profile User_Profile { get; set; }
        public virtual Global_Lookup_Data Global_Lookup_Data5 { get; set; }
        public virtual Global_Lookup_Data Global_Lookup_Data6 { get; set; }
        public virtual ICollection<Employment_History> Employment_History { get; set; }
        public virtual ICollection<Employment_History> Employment_History1 { get; set; }
        public virtual ICollection<ETIRA8> ETIRA8 { get; set; }
        public virtual ICollection<Expenses_Application_Document> Expenses_Application_Document { get; set; }
        public virtual ICollection<Expenses_Application> Expenses_Application { get; set; }
        public virtual ICollection<Expenses_Calculation> Expenses_Calculation { get; set; }
        public virtual ICollection<Job_Cost> Job_Cost { get; set; }
        public virtual ICollection<Leave_Adjustment> Leave_Adjustment { get; set; }
        public virtual ICollection<Leave_Application_Document> Leave_Application_Document { get; set; }
        public virtual ICollection<Leave_Calculation> Leave_Calculation { get; set; }
        public virtual ICollection<Leave_Config_Extra> Leave_Config_Extra { get; set; }
        public virtual ICollection<PRAL> PRALs { get; set; }
        public virtual ICollection<PREL> PRELs { get; set; }
        public virtual ICollection<PRM> PRMs { get; set; }
        public virtual ICollection<Relationship> Relationships { get; set; }
    }
}
