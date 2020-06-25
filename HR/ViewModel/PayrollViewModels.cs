using HR.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Data.Entity;
using System.ComponentModel.DataAnnotations;
using SBSModel.Models;
using SBSModel.Common;
using Renci.SshNet;
using System.IO;
using System.Configuration;
using System.Web.Configuration;
using SBSResourceAPI;
using System.Data.Entity.Core.Objects;

namespace HR.Models
{


    //Added by sun 18-03-2016
    public class leave_By_Payroll
    {
        public int leave_ID { get; set; }
        public string leave_Name { get; set; }
        public decimal leave_Amount { get; set; }
        public bool leave_Allowed_Probation { get; set; }
    }

    public class PRDViewModel
    {
        public Nullable<int> Payroll_Detail_ID { get; set; }
        public string Type { get; set; }

        public Nullable<int> PRM_ID { get; set; }

        [LocalizedRequired]
        [LocalizedDisplayName("Type", typeof(Resource))]
        public Nullable<int> PRT_ID { get; set; }

        [LocalizedDisplayName("Description", typeof(Resource))]
        public Nullable<int> PRC_ID { get; set; }

        public Nullable<int> Currency_ID { get; set; }

        [LocalizedRequired]
        [LocalizedDisplayName("Amount", typeof(Resource))]
        public Nullable<decimal> Amount { get; set; }

        public string Description { get; set; }
        public Nullable<decimal> Hours_Worked { get; set; }
        public string Row_Type { get; set; }
        public Nullable<int> History_Allowance_ID { get; set; }
        public bool CPF_Deduction { get; set; }

    }

    public class CPFGenerateViewModels : ModelBase
    {
       public List<ComboViewModel> processDateList { get; set; }
       //Added by Nay on 14-Aug-2015
       public int CPF_ProcessMonth { get; set; }
       public int CPF_ProcessYear { get; set; }

       public List<int> CPF_YearList { get; set; }
       //Added by Nay on 08-Sept-2015
       public List<HR_FileExport_History> generatedCPF_File { get; set; }
       public List<HR_FileExport_History_Detail> generatedCPF_FileDetail { get; set; }

       public int CPF_Department { get; set; }
       public int CPF_Designation { get; set; }
       public int CPF_Race { get; set; }
       public string CPF_Residential { get; set; }
       public int Generated_ID { get; set; }

       public List<Department> departments { get; set; }
       public List<Designation> designations { get; set; }

       public List<HR_FileExport_History> generatedIRA8A_File { get; set; }
       public List<HR_FileExport_History> generatedIR8B_File { get; set; }
       public List<HR_FileExport_History> generatedIR8A_File { get; set; }
       public List<HR_FileExport_History> generatedIR8S_File { get; set; }
       public List<Global_Lookup_Data> races { get; set; }
       public List<ComboViewModel> residentialStatusList { get; set; }

       public List<int> IR8A_YearList { get; set; }
       public int IR8A_Year { get; set; }
       public List<HR_FileExport_IR8A_Detail> generatedIR8A_FileDetail { get; set; }

       public List<int> IR8S_YearList { get; set; }
       public int IR8S_Year { get; set; }
       public List<HR_FileExport_IR8S_Detail> generatedIR8S_FileDetail { get; set; }

       public List<int> IR8B_YearList { get; set; }
       public int IR8B_Year { get; set; }
       public List<HR_FileExport_IR8B_Detail> generatedIR8B_FileDetail { get; set; }

       public List<int> IRA8A_YearList { get; set; }
       public int IRA8A_Year { get; set; }
       public List<HR_FileExport_IRA8A_Detail> generatedIRA8A_FileDetail { get; set; }
    }
    public class PayrollViewModels : ModelBase
    {
        //Added by sun 18-03-2016
        public string tabAction { get; set; }
        public List<ComboViewModel> ExpensesLst { get; set; }
        public List<ComboViewModel> LeaveLst { get; set; }

        public List<leave_By_Payroll> LeaveLstTempItem { get; set; }



        //Added By sun
        public bool E_S { get; set; }

        public List<Employee_Profile> payrollList { get; set; }
        public List<ComboViewModel> departmentList { get; set; }
        public List<ComboViewModel> processDateList { get; set; }
        public List<ComboViewModel> processStatusList { get; set; }
        public List<ComboViewModel> prtallowanceList { get; set; }
        public List<ComboViewModel> prcOvertimeList { get; set; }
        public List<ComboViewModel> prcCommissionList { get; set; }
        public List<ComboViewModel> prcDonationList { get; set; }
        //public List<ComboViewModel> extradonationList { get; set; }
        public List<ComboViewModel> paymentTypeList { get; set; }

        public List<Expenses_Application_Document> expensesList { get; set; }
        public List<Leave_Application_Document> leaveList { get; set; }
        public List<decimal> leaveAmountList { get; set; }

        public int[] empIds { get; set; }

        // ----- search feilds -----
        [LocalizedDisplayName("Department", typeof(Resource))]
        public Nullable<int> sDepartment { get; set; }


        [LocalizedRequired]
        [LocalizedDisplayName("ProcessDate", typeof(Resource))]
        public int Process_Month { get; set; }

        [LocalizedRequired]
        public int Process_Year { get; set; }

        public string File_Type { get; set; }

        [LocalizedDisplayName("Process", typeof(Resource))]
        public string sProcess { get; set; }


        //----- db fields -------
        public Nullable<int> PRM_ID { get; set; }

        public int Employee_Profile_ID { get; set; }
        public Nullable<bool> Opt_Out { get; set; }

        public Nullable<int> History_ID { get; set; }
        public string Department { get; set; }

        public string Process_Status { get; set; }

        [LocalizedDisplayName("EmployeeNo", typeof(Resource))]
        public string Employee_No { get; set; }

        [LocalizedDisplayName("Name", typeof(Resource))]
        public string Name { get; set; }

        public string Company_Name { get; set; }
        public Nullable<int> Company_ID { get; set; }

        [LocalizedDisplayName()]
        public decimal Basic_Salary { get; set; }

        [LocalizedDisplayName()]
        public decimal Hourly_Rate { get; set; }

        [LocalizedDisplayName("Basic_Salary", typeof(Resource))]
        public decimal Raw_Basic_Salary { get; set; }

        public decimal Basic_Salary_Per_Day { get; set; }

        public string Basic_Salary_Unit { get; set; }

        [LocalizedDisplayName("HoursWorked", typeof(Resource))]
        public decimal No_Of_Hours { get; set; }

        [LocalizedDisplayName("RunDate", typeof(Resource))]
        public string Run_Date { get; set; }

        [LocalizedRequired]
        [LocalizedDisplayName("PaymentType", typeof(Resource))]
        public Nullable<int> Payment_Type { get; set; }
        public string Payment_Type_Name { get; set; }

        [LocalizedValidMaxLength(150)]
        [LocalizedDisplayName("ChequeNo", typeof(Resource))]
        public string Cheque_No { get; set; }


        [LocalizedDisplayName("TotalWorkDays", typeof(Resource))]
        public Nullable<decimal> Total_Work_Days { get; set; }
        public decimal Total_Work_Days_All { get; set; }

        [LocalizedDisplayName("LeavePeriodFrom", typeof(Resource))]
        public string Leave_Period_From { get; set; }

        [DataType(DataType.Date)]
        [LocalizedDisplayName("LeavePeriodFrom", typeof(Resource))]
        public string sLeave_Period_From { get; set; }

        [LocalizedDisplayName("LeavePeriodTo", typeof(Resource))]
        public string Leave_Period_To { get; set; }

        [DataType(DataType.Date)]
        [LocalizedDisplayName("LeavePeriodTo", typeof(Resource))]
        public string sLeave_Period_To { get; set; }


        public List<int> Working_Day { get; set; }

        public List<DateTime> holidays { get; set; }

        [LocalizedDisplayName("ExpensesPeriodFrom", typeof(Resource))]
        public string Expenses_Period_From { get; set; }

        [LocalizedDisplayName("ExpensesPeriodTo", typeof(Resource))]
        public string Expenses_Period_To { get; set; }

        [DataType(DataType.Date)]
        [LocalizedDisplayName("ExpensesPeriodFrom", typeof(Resource))]
        public string sExpenses_Period_From { get; set; }

        [DataType(DataType.Date)]
        [LocalizedDisplayName("ExpensesPeriodTo", typeof(Resource))]
        public string sExpenses_Period_To { get; set; }

        public int Company_Currency_ID { get; set; }
        public string Company_Currency_Code { get; set; }
        public string Company_Currency_Name { get; set; }

        //----- Expenses Row -------
        public int[] Expenses_Rows { get; set; }

        //----- Leave Row -------
        public int[] Leave_Rows { get; set; }

        //----- Allowance Row -----
        public PRDViewModel[] Allowance_Rows { get; set; }


        //----- Extra Donation Row -----
        public PRDViewModel[] Extra_Donation_Rows { get; set; }

        //----- Overtime Row -----
        public PRDViewModel[] Overtime_Rows { get; set; }


        // Summary / Calculation
        [LocalizedDisplayName("LeaveAmount", typeof(Resource))]
        public decimal Leave_Amount { get; set; }


        [LocalizedDisplayName("ExpensesAmount", typeof(Resource))]
        public decimal Expenses_Amount { get; set; }


        [LocalizedDisplayName("Allowance_", typeof(Resource))]
        public decimal Allowance { get; set; }

        [LocalizedDisplayName("ExtraDonation", typeof(Resource))]
        public decimal Extra_Donation { get; set; }

        [LocalizedDisplayName("Overtime", typeof(Resource))]
        public decimal Overtime { get; set; }


        [LocalizedDisplayName("Commission", typeof(Resource))]
        public decimal Commission { get; set; }


        [LocalizedDisplayName("Deductions", typeof(Resource))]
        public decimal Deductions { get; set; }


        [LocalizedDisplayName("TotalAllowance", typeof(Resource))]
        public decimal Total_Allowance { get; set; }


        [LocalizedDisplayName("TotalDeduction", typeof(Resource))]
        public decimal Total_Deduction { get; set; }


        [LocalizedDisplayName("EmployeeContribution", typeof(Resource))]
        public decimal Employee_Contribution { get; set; }


        [LocalizedDisplayName("EmployerContribution", typeof(Resource))]
        public decimal Employer_Contribution { get; set; }

        [LocalizedDisplayName("NetSalary", typeof(Resource))]
        public decimal Net_Salary { get; set; }

        [LocalizedDisplayName("GrossSalary", typeof(Resource))]
        public decimal Gross_Salary { get; set; }

        public decimal Deduction_Adhoc { get; set; }

        public decimal Deduction_Donation { get; set; }

        public decimal Allowance_Adhoc { get; set; }

        public decimal Adjustment_Addition { get; set; }

        public decimal Adjustment_Deduction { get; set; }


        public Nullable<int> Selected_CPF_Formula_ID { get; set; }
        public Nullable<int> Selected_OT_Formula_ID { get; set; }
        public Nullable<int> Selected_Donation_Formula_ID { get; set; }

        public string Formula_Employee_Contribution { get; set; }

        public string Formula_Employer_Contribution { get; set; }

        [LocalizedDisplayName("TotalBonus", typeof(Resource))]
        public decimal Total_Bonus { get; set; }

        [LocalizedDisplayName("BonusAmount", typeof(Resource))]
        public decimal Bonus_Amount { get; set; }

        [LocalizedDisplayName("BonusIssue", typeof(Resource))]
        public bool Bonus_Issue { get; set; }

        [LocalizedDisplayName("TotalDirectorFee", typeof(Resource))]
        public decimal Total_Director_Fee { get; set; }

        [LocalizedDisplayName("FeeAmount", typeof(Resource))]
        public decimal Director_Fee_Amount { get; set; }

        [LocalizedDisplayName("IssueDirectorFee", typeof(Resource))]
        public bool Director_Fee_Issue { get; set; }

        [LocalizedDisplayName("Donation", typeof(Resource))]
        public decimal Donation { get; set; }

        public string Donation_Label { get; set; }

        public Nullable<int> Department_ID { get; set; }

    

     
        public List<Department> departments { get; set; }
        public List<Designation> designations { get; set; }
        public List<Global_Lookup_Data> races { get; set; }
        public List<ComboViewModel> residentialStatusList { get; set; }
      


        public Nullable<int> Revision_No { get; set; }

        [LocalizedRequired]
        [LocalizedDisplayName()]
        public string Process_Date_From { get; set; }

        [LocalizedRequired]
        [LocalizedDisplayName()]
        public string Process_Date_To { get; set; }



       //Added by sun 09-02-2017
        public decimal Total_Allowance_Basic_Salary { get; set; }
    }

    //Added By sun 21-08-2015
    public class PayslipViewModels : ModelBase
    {

        public List<PRM> prmlist { get; set; }
        public string First_Name { get; set; }
        public string Middle_Name { get; set; }
        public string Last_Name { get; set; }


        public Nullable<int> Search_Month { get; set; }
        public Nullable<int> Search_Year { get; set; }
        public List<ComboViewModel> processDateList { get; set; }
        public List<int> YearList { get; set; }



    }

    public class PayrollDetailViewModel : ModelBase
    {
        public List<ComboViewModel> prtList { get; set; }
        public List<ComboViewModel> prcOvertimeList { get; set; }
        public List<ComboViewModel> prcCommissionList { get; set; }
        public List<ComboViewModel> prcDonationList { get; set; }

        public int Index { get; set; }

        public Nullable<int> Payroll_Detail_ID { get; set; }

        public Nullable<int> PRM_ID { get; set; }

        [LocalizedDisplayName("Type", typeof(Resource))]
        public Nullable<int> PRT_ID { get; set; }

        public string Description { get; set; }
        public Nullable<int> PRC_ID { get; set; }

        public int Company_Currency_ID { get; set; }
        public string Company_Currency_Code { get; set; }
        public Nullable<decimal> Amount { get; set; }
        public string Row_Type { get; set; }
        public Nullable<decimal> History_Allowance_ID { get; set; }
        public Nullable<decimal> Hours_Worked { get; set; }
        public bool CPF_Deduction { get; set; }

    }

    public class IR8AViewModel : ModelBase
    {
        public List<ETIRA8> etira8 { get; set; }
        public List<User_Profile> employeeList { get; set; }

        public int iid { get; set; }
        public Nullable<int> Employee_Profile_ID { get; set; }
        public Employee_Profile Employee_Profile { get; set; }


        public Company_Details company { get; set; }
        public Employment_History employment_History { get; set; }

        public string Designation_Name { get; set; }
        public string P_YEAR { get; set; }
        public string Date_of_Cessation { get; set; }
        public decimal Gross_Salary { get; set; }
        public decimal Bonus { get; set; }

        public decimal Director_Fee { get; set; }
        public Nullable<int> Director_Fee_Day { get; set; }
        public Nullable<int> Director_Fee_Month { get; set; }

        public decimal Allowance_Transport { get; set; }
        public decimal Allowance_Entertain { get; set; }
        public decimal Allowance_Others { get; set; }
        public string Commission_Start { get; set; }
        public string Commission_End { get; set; }
        public decimal Commission_Amount { get; set; }
        public decimal Commission_Type { get; set; }
        public decimal Pension { get; set; }
        public decimal Gratuity { get; set; }
        public decimal Notice_Pay { get; set; }
        public decimal Ex_Gratia { get; set; }
        public decimal Lump_Sum_Others { get; set; }
        [LocalizedValidMaxLength(200)]
        public string Nature { get; set; }
        public decimal Compensation_Loss { get; set; }
        public bool Approval_IRAS { get; set; }
        public string Date_Approval { get; set; }
        [LocalizedValidMaxLength(300)]
        public string Reason_Payment { get; set; }
        public decimal Length_Service { get; set; }
        [LocalizedValidMaxLength(300)]
        public string Basis_Payment { get; set; }
        public decimal Total_Lump_Sum { get; set; }
        [LocalizedValidMaxLength(300)]
        public string Retirement_Pension { get; set; }
        public decimal Amount_Accured_1992 { get; set; }
        public decimal Amount_Accured_1993 { get; set; }
        public decimal Contribution_Out_Singapore { get; set; }
        public decimal Excess_Employer { get; set; }
        public decimal Excess_Less { get; set; }
        public decimal Gain_Profit { get; set; }
        public decimal Value_Benefit { get; set; }
        public bool Income_Tax_Employer { get; set; }
        public decimal Tax_Partially { get; set; }
        public decimal Tax_Fixed { get; set; }
        public decimal Employee_CPF { get; set; }
        [LocalizedValidMaxLength(300)]
        public string Name_CPF { get; set; }
        public decimal Donation { get; set; }
        public decimal Contribution_Mosque { get; set; }
        public decimal Life_Insurance { get; set; }
        public string Hired_Date { get; set; }
        public string Run_Date { get; set; }

        public string Yayasan_Mendaki_Fund { get; set; }
        public string Community_chest_of_Singapore { get; set; }
        public string SINDA { get; set; }
        public string CDAC { get; set; }
        public string ECF { get; set; }
        public string Other_tax_exempt_donations { get; set; }

        public List<ComboViewModel> genderList { get; set; }
        public List<ComboViewModel> nationalityList { get; set; }

        public int sYear { get; set; }
        public List<int> sYearList { get; set; }

        public List<ComboViewModel> departmentList { get; set; }

        public Nullable<int> sDepartment { get; set; }

        public Employee_Profile employer { get; set; }

        public string employer_Desination_Name { get; set; }

        [LocalizedValidMaxLength(300)]
        public string Contribution_With_Name { get; set; }
        public Nullable<bool> Contribution_With_Madatory { get; set; }
        public Nullable<bool> Contribution_With_Establishment { get; set; }
        public decimal Contribution_With_Singapore { get; set; }
        public decimal Remission_Income { get; set; }
        public decimal Non_Tax_Income { get; set; }

        public string Bank_Name { get; set; }
    }


    public class ImportPayrollViewModels : ModelBase
    {
        public bool validated_Main { get; set; }
        public ImportPRM_PRD_[] prm_prd { get; set; }
        public List<string> errMsg { get; set; }

    }
    public class ImportPRM_PRD_ : ModelBase
    {
        public Nullable<int> Company_ID { get; set; }
        public bool Validate { get; set; }
        public string ErrMsg { get; set; }

        public Nullable<int> Employee_Profile_ID { get; set; }
        public string Employee_No { get; set; }
        //PRM
        public string Process_Year { get; set; }
        public Nullable<int> Process_Month { get; set; }
        public string Process_Month_ { get; set; }
        public string Run_Date { get; set; }
        public decimal Total_Work_Days { get; set; }
        public Nullable<int> Payment_Type { get; set; }
        public string Payment_Type_ { get; set; }

        //Overtime
        public Nullable<int> PRT_ID_Overtime { get; set; }
        public Nullable<int> PRC_ID_Overtime { get; set; }
        public string PRC_ID_Overtime_ { get; set; }
        public decimal OT_Rate { get; set; }
        public decimal OT_Hours { get; set; }
        public decimal Overtime_Type_Amount { get; set; }

        //Description
        public Nullable<int> PRT_ID_Allowance_Type { get; set; }
        public string PRT_ID_Allowance_Type_ { get; set; }
        public Nullable<int> PRC_ID_Description { get; set; }
        public string PRC_ID_Description_ { get; set; }
        public decimal Description_Type_Amount { get; set; }

        //Donatio
        public Nullable<int> PRT_ID_Donation { get; set; }
        public Nullable<int> PRC_ID_Donation { get; set; }
        public string PRC_ID_Donation_ { get; set; }
        public decimal Donation_Type_Amount { get; set; }
    }

   
}