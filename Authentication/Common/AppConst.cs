using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

public class Module
{
    public static string Authentication = "Authentication";
    public static string HR = "HR";
    public static string Inventory = "Inventory";
    public static string POS = "POS";
    public static string CRM = "CRM";
}


public class ModuleDomain
{
    public static string Authentication = "AuthenSBS2";
    public static string HR = "HRSBS2";
    public static string Inventory = "InvenSBS2";
    public static string POS = "POS";
    public static string CRM = "CRM";
}


public class Period
{
    public static string AM = "AM";
    public static string PM = "PM";
}

public class WorkingDays
{
    public static double Days5 = 5;
    public static double Days5_5 = 5.5;
    public static double Days6 = 6;
}
public class LeaveStatus
{
    public static string Pending = "P";
    public static string Rejected = "R";
    public static string Approved = "A";
    public static string Cancel = "C";
    public static string ApproveCancel = "O";
}

public class PayrollStatus
{
    public static string Pending = "Pending";
    public static string Process = "Process"; // when process payroll complete
    public static string Comfirm = "Comfirm"; // when confirm payroll complete
}

public class AllowanceStatus
{
    public static string Active = "A";
    public static string InActive = "I";
}

public class PayrollAllowanceType
{
    public static string Allowance_Deduction = "Allowance_Deduction";
    public static string Overtime = "Overtime";
    public static string Commission = "Commission";
}

public class PayrollFlag
{
    public static string Yes = "Y";
    public static string No = "N";
    public static string Partial = "P";
}
    
public class AppConst
{
    public static string Other = "Other";
}

public class FormulaVariable
{
    public static string Employee_Contribution = "[Employee Contribution]";
    public static string Total_CPF_Contribution = "[Total CPF Contribution]";
    public static string Basic_Salary = "[Basic Salary]";
    public static string Deduction_Ad_Hoc = "[Deduction (Ad Hoc)]";
    public static string Leave_Amount = "[Leave Amount]";
    public static string Allowance = "[Allowance]";
    public static string Adjustment_Allowance = "[Adjustment (Allowance)]";
    public static string Adjustment_Deductions = "[Adjustment (Deductions)]";
    public static string Commission = "[Commission]";
    public static string Overtime = "[Overtime]";
    public static string Employee_Residential_Status = "[Employee.Residential Status]";
    public static string Employee_Age = "[Employee.Age]";
    public static string PR_Years = "[PR.Years]";
    public static string Employee_Total_Wages = "[Employee.Total Wages]";
    public static string Current_Date = "[Current Date]";
    public static string Bonus = "[Bonus]";
    public static string Local = "[Local]";
    public static string PR = "[PR]";
    public static string Internship = "[Internship]";
    public static string Employee_Status = "[Employee.Status]";
}