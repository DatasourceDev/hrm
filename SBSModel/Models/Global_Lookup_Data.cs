using System;
using System.Collections.Generic;

namespace SBSModel.Models
{
    public partial class Global_Lookup_Data
    {
        public Global_Lookup_Data()
        {
            this.Banking_Info = new List<Banking_Info>();
            this.Default_Expense_Type = new List<Default_Expense_Type>();
            this.Donation_Formula = new List<Donation_Formula>();
            this.Employee_Attachment = new List<Employee_Attachment>();
            this.Employee_Emergency_Contact = new List<Employee_Emergency_Contact>();
            this.Employee_Profile = new List<Employee_Profile>();
            this.Employee_Profile1 = new List<Employee_Profile>();
            this.Employee_Profile2 = new List<Employee_Profile>();
            this.Employee_Profile3 = new List<Employee_Profile>();
            this.Employee_Profile4 = new List<Employee_Profile>();
            this.Employee_Profile5 = new List<Employee_Profile>();
            this.Employee_Profile6 = new List<Employee_Profile>();
            this.Employment_History = new List<Employment_History>();
            this.Expenses_Config = new List<Expenses_Config>();
            this.Holiday_Config = new List<Holiday_Config>();
            this.HR_FileExport_History_Detail = new List<HR_FileExport_History_Detail>();
            this.Leave_Config_Condition = new List<Leave_Config_Condition>();
            this.Leave_Default_Condition = new List<Leave_Default_Condition>();
            this.PRMs = new List<PRM>();
            this.Relationships = new List<Relationship>();
            this.Relationships1 = new List<Relationship>();
        }

        public int Lookup_Data_ID { get; set; }
        public int Def_ID { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Record_Status { get; set; }
        public Nullable<int> Company_ID { get; set; }
        public string Colour_Config { get; set; }
        public string Create_By { get; set; }
        public Nullable<System.DateTime> Create_On { get; set; }
        public string Update_By { get; set; }
        public Nullable<System.DateTime> Update_On { get; set; }
        public virtual ICollection<Banking_Info> Banking_Info { get; set; }
        public virtual Company Company { get; set; }
        public virtual ICollection<Default_Expense_Type> Default_Expense_Type { get; set; }
        public virtual ICollection<Donation_Formula> Donation_Formula { get; set; }
        public virtual ICollection<Employee_Attachment> Employee_Attachment { get; set; }
        public virtual ICollection<Employee_Emergency_Contact> Employee_Emergency_Contact { get; set; }
        public virtual ICollection<Employee_Profile> Employee_Profile { get; set; }
        public virtual ICollection<Employee_Profile> Employee_Profile1 { get; set; }
        public virtual ICollection<Employee_Profile> Employee_Profile2 { get; set; }
        public virtual ICollection<Employee_Profile> Employee_Profile3 { get; set; }
        public virtual ICollection<Employee_Profile> Employee_Profile4 { get; set; }
        public virtual ICollection<Employee_Profile> Employee_Profile5 { get; set; }
        public virtual ICollection<Employee_Profile> Employee_Profile6 { get; set; }
        public virtual ICollection<Employment_History> Employment_History { get; set; }
        public virtual ICollection<Expenses_Config> Expenses_Config { get; set; }
        public virtual Global_Lookup_Def Global_Lookup_Def { get; set; }
        public virtual ICollection<Holiday_Config> Holiday_Config { get; set; }
        public virtual ICollection<HR_FileExport_History_Detail> HR_FileExport_History_Detail { get; set; }
        public virtual ICollection<Leave_Config_Condition> Leave_Config_Condition { get; set; }
        public virtual ICollection<Leave_Default_Condition> Leave_Default_Condition { get; set; }
        public virtual ICollection<PRM> PRMs { get; set; }
        public virtual ICollection<Relationship> Relationships { get; set; }
        public virtual ICollection<Relationship> Relationships1 { get; set; }
    }
}
