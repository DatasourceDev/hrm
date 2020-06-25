using System;
using System.Collections.Generic;

namespace SBSModel.Models
{
    public partial class Company
    {
        public Company()
        {
            this.Bank_Details = new List<Bank_Details>();
            this.Branches = new List<Branch>();
            this.Expenses_Application = new List<Expenses_Application>();
            this.TsEXes = new List<TsEX>();
            this.Company_Details = new List<Company_Details>();
            this.Company_Details1 = new List<Company_Details>();
            this.Company_Logo = new List<Company_Logo>();
            this.Global_Lookup_Data = new List<Global_Lookup_Data>();
            this.Customers = new List<Customer>();
            this.Departments = new List<Department>();
            this.Designations = new List<Designation>();
            this.Donation_Formula = new List<Donation_Formula>();
            this.Donation_Type = new List<Donation_Type>();
            this.Email_Notification = new List<Email_Notification>();
            this.Employee_No_Pattern = new List<Employee_No_Pattern>();
            this.ETIRA8 = new List<ETIRA8>();
            this.Expenses_Category = new List<Expenses_Category>();
            this.Expenses_Config = new List<Expenses_Config>();
            this.Expenses_No_Pattern = new List<Expenses_No_Pattern>();
            this.Holiday_Config = new List<Holiday_Config>();
            this.HR_FileExport_History = new List<HR_FileExport_History>();
            this.Invoice_Header = new List<Invoice_Header>();
            this.Job_Cost = new List<Job_Cost>();
            this.Leave_Adjustment = new List<Leave_Adjustment>();
            this.Leave_Config = new List<Leave_Config>();
            this.Notification_Scheduler = new List<Notification_Scheduler>();
            this.PRCs = new List<PRC>();
            this.PRGs = new List<PRG>();
            this.Selected_CPF_Formula = new List<Selected_CPF_Formula>();
            this.Selected_Donation_Formula = new List<Selected_Donation_Formula>();
            this.Selected_OT_Formula = new List<Selected_OT_Formula>();
            this.Storage_Upgrade = new List<Storage_Upgrade>();
            this.Subscriptions = new List<Subscription>();
            this.User_Authentication = new List<User_Authentication>();
            this.User_Profile = new List<User_Profile>();
            this.User_Transactions = new List<User_Transactions>();
            this.Working_Days = new List<Working_Days>();
        }

        public int Company_ID { get; set; }
        public string Create_By { get; set; }
        public Nullable<System.DateTime> Create_On { get; set; }
        public string Update_By { get; set; }
        public Nullable<System.DateTime> Update_On { get; set; }
        public virtual ICollection<Bank_Details> Bank_Details { get; set; }
        public virtual ICollection<Branch> Branches { get; set; }
        public virtual ICollection<Expenses_Application> Expenses_Application { get; set; }
        public virtual ICollection<TsEX> TsEXes { get; set; }
        public virtual ICollection<Company_Details> Company_Details { get; set; }
        public virtual ICollection<Company_Details> Company_Details1 { get; set; }
        public virtual ICollection<Company_Logo> Company_Logo { get; set; }
        public virtual ICollection<Global_Lookup_Data> Global_Lookup_Data { get; set; }
        public virtual ICollection<Customer> Customers { get; set; }
        public virtual ICollection<Department> Departments { get; set; }
        public virtual ICollection<Designation> Designations { get; set; }
        public virtual ICollection<Donation_Formula> Donation_Formula { get; set; }
        public virtual ICollection<Donation_Type> Donation_Type { get; set; }
        public virtual ICollection<Email_Notification> Email_Notification { get; set; }
        public virtual ICollection<Employee_No_Pattern> Employee_No_Pattern { get; set; }
        public virtual ICollection<ETIRA8> ETIRA8 { get; set; }
        public virtual ICollection<Expenses_Category> Expenses_Category { get; set; }
        public virtual ICollection<Expenses_Config> Expenses_Config { get; set; }
        public virtual ICollection<Expenses_No_Pattern> Expenses_No_Pattern { get; set; }
        public virtual ICollection<Holiday_Config> Holiday_Config { get; set; }
        public virtual ICollection<HR_FileExport_History> HR_FileExport_History { get; set; }
        public virtual ICollection<Invoice_Header> Invoice_Header { get; set; }
        public virtual ICollection<Job_Cost> Job_Cost { get; set; }
        public virtual ICollection<Leave_Adjustment> Leave_Adjustment { get; set; }
        public virtual ICollection<Leave_Config> Leave_Config { get; set; }
        public virtual ICollection<Notification_Scheduler> Notification_Scheduler { get; set; }
        public virtual ICollection<PRC> PRCs { get; set; }
        public virtual ICollection<PRG> PRGs { get; set; }
        public virtual ICollection<Selected_CPF_Formula> Selected_CPF_Formula { get; set; }
        public virtual ICollection<Selected_Donation_Formula> Selected_Donation_Formula { get; set; }
        public virtual ICollection<Selected_OT_Formula> Selected_OT_Formula { get; set; }
        public virtual ICollection<Storage_Upgrade> Storage_Upgrade { get; set; }
        public virtual ICollection<Subscription> Subscriptions { get; set; }
        public virtual ICollection<User_Authentication> User_Authentication { get; set; }
        public virtual ICollection<User_Profile> User_Profile { get; set; }
        public virtual ICollection<User_Transactions> User_Transactions { get; set; }
        public virtual ICollection<Working_Days> Working_Days { get; set; }
    }
}
