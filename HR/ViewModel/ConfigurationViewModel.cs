using System;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using HR.Common;
using System.Data.Entity.SqlServer;
using SBSModel.Models;
using SBSModel.Common;
using SBSResourceAPI;


namespace HR.Models
{

   public class ConfigulationViewModel : ModelBase
   {
      public string tabAction { get; set; }

      //********** tab company **********
      public List<ComboViewModel> countryList;

      public List<ComboViewModel> stateList;

      public List<ComboViewModel> stateBillingList;
      public List<ComboViewModel> statusList { get; set; }
      public List<Subscription> SubscriptionList { get; set; }
      public Nullable<int> User_Count { get; set; }
      public Nullable<int> Company_ID { get; set; }

      [LocalizedDisplayName("No_Of_Employees", typeof(Resource))]
      public Nullable<int> No_Of_Employees { get; set; }

      [LocalizedValidMaxLength(150)]
      [LocalizedDisplayName("Company_Name", typeof(Resource))]
      public string Company_Name { get; set; }

      [LocalizedDisplayName("Date_Of_Registration", typeof(Resource))]
      public string Effective_Date { get; set; }

      [LocalizedValidMaxLength(500)]
      [LocalizedDisplayName("Address", typeof(Resource))]
      public string Address { get; set; }

      [LocalizedDisplayName("Country", typeof(Resource))]
      public Nullable<int> Country_ID { get; set; }

      [LocalizedDisplayName("State_Or_Province", typeof(Resource))]
      public Nullable<int> State_ID { get; set; }

      [LocalizedValidMaxLength(10)]
      [DataType(DataType.PostalCode)]
      [LocalizedDisplayName("Zip_Or_Postal_Code", typeof(Resource))]
      public String Zip_Code { get; set; }

      [LocalizedValidMaxLength(500)]
      [LocalizedDisplayName("Billing_Address", typeof(Resource))]
      public string Billing_Address { get; set; }

      [LocalizedDisplayName("Country", typeof(Resource))]
      public Nullable<int> Billing_Country_ID { get; set; }

      [LocalizedDisplayName("State_Or_Province", typeof(Resource))]
      public Nullable<int> Billing_State_ID { get; set; }

      [LocalizedValidMaxLength(10)]
      [DataType(DataType.PostalCode)]
      [LocalizedDisplayName("Zip_Or_Postal_Code", typeof(Resource))]
      public String Billing_Zip_Code { get; set; }

      /********** PayPal ***********/
      [LocalizedDisplayName("API_Username", typeof(Resource))]
      public string APIUsername { get; set; }

      [LocalizedDisplayName("API_Password", typeof(Resource))]
      public string APIPassword { get; set; }

      [LocalizedDisplayName("API_Signature", typeof(Resource))]
      public string APISignature { get; set; }

      [LocalizedDisplayName("Currency", typeof(Resource))]
      public Nullable<int> Currency_ID { get; set; }

      public bool Is_Sandbox { get; set; }

      public List<ComboViewModel> Currency_List;
      public string Company_Level { get; set; }

      [LocalizedValidMaxLength(100)]
      [LocalizedDisplayName("Office_Phone", typeof(Resource))]
      public string Phone { get; set; }

      [LocalizedValidMaxLength(100)]
      [LocalizedDisplayName("Fax", typeof(Resource))]
      public string Fax { get; set; }
      public byte[] Company_Logo { get; set; }
      public Nullable<System.Guid> Company_Logo_ID { get; set; }

      [LocalizedDisplayName("CPF_Sub_No", typeof(Resource))]
      public string CPF_Submission_No { get; set; }
      public string Business_Type { get; set; }
      public List<ComboViewModel> LstCompanylevel { get; set; }
      public string Company_Levelg { get; set; }

      /********** pattern ***********/
      public List<Branch> BranchList { get; set; }
      public Nullable<int> Employee_No_Pattern_ID { get; set; }
      public bool Select_Nationality { get; set; }
      public Nullable<bool> Select_Year { get; set; }
      public bool Year_2_Digit { get; set; }
      public bool Year_4_Digit { get; set; }
      public bool Select_Company_code { get; set; }

      [LocalizedValidMaxLength(10)]
      public String Company_Code { get; set; }
      public Nullable<bool> Select_Branch_Code { get; set; }
      public Nullable<int> Pattern_Branch_ID { get; set; }
      public Nullable<bool> Initiated { get; set; }
      public String Pattern { get; set; }

      [LocalizedDisplayName("Select_Default_Emp_No_Next_Start", typeof(Resource))]
      public bool Is_Default_Emp_No_Next_Start { get; set; }
      public String Default_Emp_No_Next_Start { get; set; }

      [LocalizedDisplayName("Select_Emp_No_Next_Start", typeof(Resource))]
      public bool Is_Emp_No_Next_Start { get; set; }
      public Nullable<int> Current_Running_Number { get; set; }

      /********** branch ***********/

      public Nullable<int> Branch_ID { get; set; }

      [LocalizedValidMaxLength(10)]
      [LocalizedDisplayName("Branch_Code", typeof(Resource))]
      public String Branch_Code { get; set; }

      [LocalizedValidMaxLength(300)]
      [LocalizedDisplayName("Branch_Name", typeof(Resource))]
      public String Branch_Name { get; set; }

      [LocalizedValidMaxLength(500)]
      [LocalizedDisplayName("Branch_Desc", typeof(Resource))]
      public String Branch_Desc { get; set; }

      [LocalizedDisplayName("Status", typeof(Resource))]
      public String Branch_Status { get; set; }

      public List<ComboViewModel> businessCatList { get; set; }
      /********** department ***********/
      public List<Department> DepartmentList;
      public Nullable<int> Department_ID { get; set; }

      [LocalizedValidMaxLength(150)]
      [LocalizedDisplayName("Department_Name", typeof(Resource))]
      public String Department_Name { get; set; }

      [LocalizedDisplayName("Status", typeof(Resource))]
      public String Department_Status { get; set; }

      /********** designation ***********/
      public List<Designation> DesignationList;
      public Nullable<int> Designation_ID { get; set; }
      public List<ComboViewModel> GradingComboList { get; set; }

      [LocalizedValidMaxLength(150)]
      [LocalizedDisplayName("Designation_Name", typeof(Resource))]
      public String Designation_Name { get; set; }

      [LocalizedDisplayName("Status", typeof(Resource))]
      public String Designation_Status { get; set; }

      [LocalizedDisplayName("Grade", typeof(Resource))]
      public Nullable<int> Employee_Grading_ID { get; set; }

      /********** grading ***********/
      //public List<Employee_Grading> GradingList;

      [MaxLength(150)]
      [LocalizedDisplayName("Grade", typeof(Resource))]
      public String Grading_Name { get; set; }

      [LocalizedValidMaxLength(500)]
      [LocalizedDisplayName("Description", typeof(Resource))]
      public String Grading_Description { get; set; }

      [LocalizedDisplayName("Status", typeof(Resource))]
      public String Grading_Status { get; set; }
      public Nullable<int> Expenses_Reviewer_ID { get; set; }
      public string Expenses_Email_address { get; set; }
      public Nullable<int> Leave_Reviewer_ID { get; set; }
      public string Leave_Email_address { get; set; }

      [LocalizedDisplayName("Fiscal_Year", typeof(Resource))]
      public Nullable<bool> Default_Fiscal_Year { get; set; }
      public string Custom_Fiscal_Year { get; set; }

      /********** working days ***********/
      public Nullable<int> Working_Days_ID { get; set; }
      public Nullable<decimal> Days { get; set; }
      public string ST_Sun_Time { get; set; }
      public string ST_Mon_Time { get; set; }
      public string ST_Tue_Time { get; set; }
      public string ST_Wed_Time { get; set; }
      public string ST_Thu_Time { get; set; }
      public string ST_Fri_Time { get; set; }
      public string ST_Sat_Time { get; set; }
      public string ET_Sun_Time { get; set; }
      public string ET_Mon_Time { get; set; }
      public string ET_Tue_Time { get; set; }
      public string ET_Wed_Time { get; set; }
      public string ET_Thu_Time { get; set; }
      public string ET_Fri_Time { get; set; }
      public string ET_Sat_Time { get; set; }
      public Nullable<bool> CL_Sun { get; set; }
      public Nullable<bool> CL_Mon { get; set; }
      public Nullable<bool> CL_Tue { get; set; }
      public Nullable<bool> CL_Wed { get; set; }
      public Nullable<bool> CL_Thu { get; set; }
      public Nullable<bool> CL_Fri { get; set; }
      public Nullable<bool> CL_Sat { get; set; }
     
      public string ST_Lunch_Time { get; set; }
      public string ET_Lunch_Time { get; set; }
      public Nullable<bool> CL_Lunch { get; set; }

      /********** exchange ***********/
      public List<Exchange> ExchangeList;
      public Nullable<int> Exchange_ID { get; set; }



   }
   public class DesignationViewModel : ModelBase
   {
      public Nullable<int> Designation_ID { get; set; }
      public int Company_ID { get; set; }

      public List<ComboViewModel> StatusComboList { get; set; }
      public List<ComboViewModel> GradingComboList { get; set; }

      [LocalizedRequired]
      [LocalizedValidMaxLength(150)]
      [LocalizedDisplayName("Designation_Name", typeof(Resource))]
      public String Name { get; set; }

      [LocalizedDisplayName("Status", typeof(Resource))]
      public String Record_Status { get; set; }

      [LocalizedDisplayName("Grade", typeof(Resource))]
      public Nullable<int> Employee_Grading_ID { get; set; }
   }
   public class DepartmentViewModel : ModelBase
   {
      public Nullable<int> Department_ID { get; set; }
      public int Company_ID { get; set; }

      public List<ComboViewModel> StatusComboList { get; set; }

      [LocalizedRequired]
      [LocalizedValidMaxLength(150)]
      [LocalizedDisplayName("Department_Name", typeof(Resource))]
      public String Name { get; set; }

      [LocalizedDisplayName("Status", typeof(Resource))]
      public String Record_Status { get; set; }
   }

   //public class LeaveService
   //{




   //    //Added by Nay on 13-Jul-2015
   //    //Purpose : to delete multiple records of holiday
   //    public ServiceResult MultipleDeleteHoliday(int[] holidayList)
   //    {
   //        try
   //        {
   //            using (var db = new SBS2DBContext())
   //            {
   //                var holidays = db.Holiday_Config.Where(w => holidayList.Contains(w.Holiday_ID));
   //                if (holidays != null)
   //                {
   //                    foreach (var h in holidays)
   //                    {
   //                        db.Holiday_Config.Remove(h);
   //                    }
   //                    db.SaveChanges();
   //                }
   //                return new ServiceResult() { Code = ERROR_CODE.SUCCESS, Msg = new Success().getSuccess(ERROR_CODE.SUCCESS_DELETE), Field = Resource.Holiday };
   //            }
   //        }
   //        catch
   //        {
   //            //Log
   //            return new ServiceResult() { Code = ERROR_CODE.ERROR_505_DELETE_ERROR, Msg = new Error().getError(ERROR_CODE.ERROR_505_DELETE_ERROR), Field = Resource.Holiday };
   //        }
   //    }





   //    //Added by Nay on 14-Ju-2015 
   //    //Purpose : to check current Leave Type are still using in Leave Application or not.

   //    //Added by Nay on 14-Jul-2015
   //    //Purpose : to delete multiple records of Leave type
   //    public ServiceResult MultipleDeleteLeaveType(int[] LeaveTypeList)
   //    {
   //        try
   //        {
   //            using (var db = new SBS2DBContext())
   //            {
   //                var levList = db.Leave_Config.Where(w => LeaveTypeList.Contains(w.Leave_Config_ID));

   //                if (levList != null)
   //                {
   //                    foreach (var leave in levList)
   //                    {

   //                        //Added By sun 10-09-2015
   //                        var CurrentDoc = db.Leave_Application_Document.Where(w => w.Leave_Config_ID == leave.Leave_Config_ID).FirstOrDefault();
   //                        var CurrentAdj = db.Leave_Adjustment.Where(w => w.Leave_Config_ID == leave.Leave_Config_ID).FirstOrDefault();
   //                        if (CurrentDoc != null || CurrentAdj != null)
   //                        {
   //                            continue;
   //                        }

   //                        var leaveAdjust = (from a in db.Leave_Adjustment where a.Leave_Config_ID == leave.Leave_Config_ID select a);
   //                        db.Leave_Adjustment.RemoveRange(leaveAdjust);

   //                        var leavetypedetail = db.Leave_Config_Detail.Where(w => w.Leave_Config_ID == leave.Leave_Config_ID);
   //                        db.Leave_Config_Detail.RemoveRange(leavetypedetail);

   //                        var leaveCal = (from a in db.Leave_Calculation where a.Leave_Config_ID == leave.Leave_Config_ID select a);
   //                        db.Leave_Calculation.RemoveRange(leaveCal);

   //                        var docs = db.Leave_Application_Document.Where(w => w.Leave_Config_ID == leave.Leave_Config_ID);
   //                        db.Leave_Application_Document.RemoveRange(docs);

   //                        var lcon = db.Leave_Config_Condition.Where(w => w.Leave_Config_ID == leave.Leave_Config_ID);
   //                        db.Leave_Config_Condition.RemoveRange(lcon);

   //                        var l = db.Leave_Config.Where(w => w.Leave_Config_ID == leave.Leave_Config_ID).FirstOrDefault();
   //                        db.Leave_Config.Remove(l);
   //                    }
   //                    db.SaveChanges();
   //                }
   //                return new ServiceResult() { Code = ERROR_CODE.SUCCESS, Msg = new Success().getSuccess(ERROR_CODE.SUCCESS_DELETE), Field = Resource.Leave_Type };
   //            }
   //        }
   //        catch
   //        {
   //            return new ServiceResult() { Code = ERROR_CODE.ERROR_505_DELETE_ERROR, Msg = new Error().getError(ERROR_CODE.ERROR_505_DELETE_ERROR), Field = Resource.Leave_Type };
   //        }
   //    }





   //    //Added by Nay on 14-Jul-2015 
   //    //Purpose : to delete multiple records of Leave Adjustment 
   //    public ServiceResult MultipleDeleteLeaveAdjustment(int[] levAdjust)
   //    {
   //        try
   //        {
   //            using (var db = new SBS2DBContext())
   //            {
   //                var leaveAdjustmentList = db.Leave_Adjustment.Where(w => levAdjust.Contains(w.Adjustment_ID));
   //                if (leaveAdjustmentList != null)
   //                {
   //                    foreach (var leave in leaveAdjustmentList)
   //                    {
   //                        if (leave.Employee_Profile_ID == 0 || !leave.Employee_Profile_ID.HasValue)
   //                        {
   //                            //select All employee
   //                            var emps = (from a in db.Employee_Profile where a.User_Profile.Company_ID == leave.Company_ID select a);

   //                            foreach (var emp in emps)
   //                            {

   //                                var cal = (from a in db.Leave_Calculation
   //                                           where a.Employee_Profile_ID == emp.Employee_Profile_ID &
   //                                           a.Leave_Config_ID == leave.Leave_Config_ID &
   //                                           a.Year_Assigned == SqlFunctions.StringConvert((double)leave.Year_2).Trim()
   //                                           orderby a.Start_Date descending
   //                                           select a).FirstOrDefault();

   //                                if (cal != null)
   //                                {
   //                                    cal.Adjustment = cal.Adjustment - leave.Adjustment_Amount;
   //                                }
   //                            }

   //                            leave.Employee_Profile_ID = null;
   //                        }
   //                        else
   //                        {
   //                            var cal = (from a in db.Leave_Calculation
   //                                       where a.Employee_Profile_ID == leave.Employee_Profile_ID &
   //                                       a.Leave_Config_ID == leave.Leave_Config_ID &
   //                                       a.Year_Assigned == SqlFunctions.StringConvert((double)leave.Year_2).Trim()
   //                                       orderby a.Start_Date descending
   //                                       select a).FirstOrDefault();

   //                            if (cal != null)
   //                            {
   //                                cal.Adjustment = cal.Adjustment - leave.Adjustment_Amount;
   //                            }
   //                        }

   //                        db.Entry(leave).State = EntityState.Deleted;
   //                    }
   //                    db.SaveChanges();
   //                }
   //                return new ServiceResult() { Code = ERROR_CODE.SUCCESS, Msg = new Success().getSuccess(ERROR_CODE.SUCCESS_DELETE), Field = Resource.Leave_Adjustment };
   //            }
   //        }
   //        catch
   //        {
   //            return new ServiceResult() { Code = ERROR_CODE.ERROR_505_DELETE_ERROR, Msg = new Error().getError(ERROR_CODE.ERROR_505_DELETE_ERROR), Field = Resource.Leave_Adjustment };
   //        }
   //    }



   //    //Added By sun 
   //    public List<User_Profile> getEmployeeListAll(Nullable<int> pCompanyID, Nullable<int> pDepartmentID = null)
   //    {
   //        List<User_Profile> employee = new List<User_Profile>();
   //        var currentdate = StoredProcedure.GetCurrentDate();
   //        try
   //        {
   //            using (var db = new SBS2DBContext())
   //            {
   //                var users = db.User_Profile
   //                    .Include(i => i.Employee_Profile.Select(s => s.Employment_History))
   //                    .Include(i => i.Employee_Profile.Select(s => s.Nationality))
   //                    .Where(w => w.Company_ID == pCompanyID && w.User_Status != RecordStatus.Delete);

   //                employee = users.OrderBy(o => o.Name).ToList();

   //            }
   //        }
   //        catch
   //        {

   //        }

   //        return employee;

   //    }

   //}
   public class LeaveTypeChildViewModel : ModelBase
   {
      public List<ComboViewModel> monthList { get; set; }
      public List<ComboViewModel> genderList { get; set; }
      public List<ComboViewModel> maritalList { get; set; }
      public List<ComboViewModel> residentalStatusList { get; set; }
      public List<ComboViewModel> periodList { get; set; }

      public int[] Condition_Rows { get; set; }
      public int lid { get; set; }

      [LocalizedRequired]
      [LocalizedValidMaxLength(150)]
      [LocalizedDisplayName("Leave_Type", typeof(Resource))]
      public string Leave_Name { get; set; }

      [LocalizedValidMaxLength(300)]
      [LocalizedDisplayName("Leave_Description", typeof(Resource))]
      public String Leave_Description { get; set; }

      public bool Flexibly { get; set; }

      public bool Continuously { get; set; }

      [LocalizedDisplayName("Valid_Period", typeof(Resource))]
      public int Valid_Period { get; set; }

      public Leave_Config_Child_Detail[] Detail_Rows { get; set; }

      public String Type { get; set; }

      public Nullable<bool> Is_Default { get; set; }     
   }
   public class LeaveTypeViewModel : ModelBase
   {
      //sun 28-09-2015
      /********** designation ***********/
      public Nullable<int> Year_Service { get; set; }
      public Nullable<decimal> Default_Leave_Amount { get; set; }
      public string Company_Currency { get; set; }
      public List<Leave_Config> leaveType { get; set; }
      public List<ComboViewModel> leaveTypeList { get; set; }
      public List<ComboViewModel> designationList { get; set; }
      public List<ComboViewModel> monthList { get; set; }
      public List<ComboViewModel> genderList { get; set; }
      public List<ComboViewModel> maritalList { get; set; }
      public List<ComboViewModel> empList { get; set; }
      public List<ComboViewModel> adjustmentTypeList { get; set; }
      public List<ComboViewModel> relatedtoList { get; set; }

      public LeaveConfigDetailViewModel[] Detail_Rows { get; set; }
      public LeaveConfigExtraViewModel[] Extra_Rows { get; set; }

      public int[] Condition_Rows { get; set; }

      [LocalizedDisplayName("Designation", typeof(Resource))]
      public int sDesignation { get; set; }

      [LocalizedDisplayName("Leave_Type", typeof(Resource))]
      public int sLeaveName { get; set; }

      public int lid { get; set; }

      [LocalizedRequired]
      [LocalizedValidMaxLength(150)]
      [LocalizedDisplayName("Leave_Type", typeof(Resource))]
      public string Leave_Name { get; set; }

      [LocalizedRequired]
      [LocalizedDisplayName("Deduct_In_Payroll", typeof(Resource))]
      public bool Deduct_In_Payroll { get; set; }



      [LocalizedRequired]
      [LocalizedDisplayName("Allowed_Probation", typeof(Resource))]
      public bool Allowed_Probation { get; set; }

      [LocalizedRequired]
      [LocalizedDisplayName("Allowed_Notice_Period", typeof(Resource))]
      public Nullable<bool> Allowed_Notice_Period { get; set; }

      [LocalizedDisplayName("Months_To_Expiry_Leave", typeof(Resource))]
      public int Months_To_Expiry { get; set; }

      [LocalizedRequired]
      [LocalizedDisplayName("Bring_Forward", typeof(Resource))]
      public bool Bring_Forward { get; set; }

      [LocalizedDisplayName("Bring_Forward_Percent", typeof(Resource))]
      public Nullable<decimal> Bring_Forward_Percent { get; set; }

      public Nullable<bool> Is_Default { get; set; }

      public bool Is_Bring_Forward_Days { get; set; }

      [LocalizedDisplayName("Days", typeof(Resource))]
      public decimal Bring_Forward_Days { get; set; }

      [LocalizedDisplayName("Fiscal_Year", typeof(Resource))]
      public bool Fiscal_Year { get; set; }

      [LocalizedDisplayName("Custom_Fiscal_Year", typeof(Resource))]
      public string Custom_Fiscal_Year { get; set; }

      [LocalizedRequired]
      [LocalizedDisplayName("Upload_Document", typeof(Resource))]
      public bool Upload_Document { get; set; }

      [LocalizedValidMaxLength(300)]
      [LocalizedDisplayName("Leave_Desc", typeof(Resource))]
      public String Leave_Description { get; set; }

      public String Type { get; set; }

      [LocalizedDisplayName("Related_To", typeof(Resource))]
      public Nullable<int> Leave_Config_Parent_ID { get; set; }

      [LocalizedDisplayName("Accumulative", typeof(Resource))]
      public bool Is_Accumulative { get; set; }
   }
   public class LeaveConfigDetailViewModel
   {
      public int Index { get; set; }
      public int[] Designations { get; set; }
      public Nullable<int> Leave_Config_ID { get; set; }

      [LocalizedRequired]
      [LocalizedDisplayName("Default_Leave_Amount", typeof(Resource))]
      public Nullable<decimal> Default_Leave_Amount { get; set; }

      [LocalizedRequired]
      [LocalizedDisplayName("Year_Service", typeof(Resource))]
      public Nullable<int> Year_Service { get; set; }
      public Nullable<int> Group_ID { get; set; }
      public string Row_Type { get; set; }
      public List<ComboViewModel> designationList { get; set; }
   }

   public class LeaveConfigExtraViewModel
   {
      public int Index { get; set; }
      public Nullable<int> Leave_Config_ID { get; set; }
      public Nullable<int> Leave_Config_Extra_ID { get; set; }

      [LocalizedRequired]
      [LocalizedDisplayName("Name", typeof(Resource))]
      public Nullable<int> Employee_Profile_ID { get; set; }

      [LocalizedRequired]
      [LocalizedDisplayName("Adjustment_Type", typeof(Resource))]
      public string Adjustment_Type { get; set; }

      [LocalizedRequired]
      [LocalizedDisplayName("No_Of_Days", typeof(Resource))]
      public Nullable<decimal> No_Of_Days { get; set; }

      public string Row_Type { get; set; }
      public List<ComboViewModel> empList { get; set; }
      public List<ComboViewModel> adjustmentTypeList { get; set; }
   }

   public class LeaveAdjustmentViewModel : ModelBase
   {
      public List<Leave_Adjustment> leaveAdjustment { get; set; }
      public List<ComboViewModel> leaveTypeList { get; set; }
      public List<ComboViewModel> employeeList { get; set; }
      public List<ComboViewModel> departmentList { get; set; }

      [LocalizedDisplayName("Leave_Type", typeof(Resource))]
      public int sLeaveType { get; set; }

      [LocalizedDisplayName("Year", typeof(Resource))]
      public int sYear { get; set; }

      public int aid { get; set; }

      [LocalizedDisplayName("Employee", typeof(Resource))]
      public int Employee_Profile_ID { get; set; }

      public Nullable<int> Department_ID { get; set; }

      [LocalizedRequired]
      [LocalizedDisplayName("Leave_Type", typeof(Resource))]
      public int Leave_Config_ID { get; set; }

      public string Leave_Name { get; set; }

      [LocalizedRequired]
      [LocalizedDisplayName("Adjustment_Amount", typeof(Resource))]
      public decimal Adjustment_Amount { get; set; }

      [LocalizedValidMaxLength(500)]
      [LocalizedDisplayName("Reason", typeof(Resource))]
      public string Reason { get; set; }

      [LocalizedRequired]
      [LocalizedDisplayName("Year", typeof(Resource))]
      public int Year_2 { get; set; }
   }

   public class GradingViewModel : ModelBase
   {
      public Nullable<int> Employee_Grading_ID { get; set; }
      public int Company_ID { get; set; }
      public List<ComboViewModel> StatusComboList { get; set; }

      [LocalizedRequired]
      [LocalizedValidMaxLength(150)]
      [LocalizedDisplayName("Grade", typeof(Resource))]
      public String Name { get; set; }

      [LocalizedValidMaxLength(500)]
      [LocalizedDisplayName("Description", typeof(Resource))]
      public String Grading_Description { get; set; }

      [LocalizedDisplayName("Status", typeof(Resource))]
      public String Record_Status { get; set; }
   }

   //--------------------- Leave Defaul-------------------//
   //15-09-2015 Add by sun
   public class LeaveDefaultNormalViewModel : ModelBase
   {
      public Nullable<int> Year_Service { get; set; }
      public Nullable<decimal> Default_Leave_Amount { get; set; }
      public string Company_Currency { get; set; }
      public List<Leave_Config> leaveType { get; set; }
      public List<ComboViewModel> leaveTypeList { get; set; }
      public List<ComboViewModel> monthList { get; set; }
      public List<ComboViewModel> genderList { get; set; }
      public List<ComboViewModel> maritalList { get; set; }
      public LeaveDefaultDetailViewModel[] Detail_Rows { get; set; }
      public int[] Condition_Rows { get; set; }

      [LocalizedDisplayName("Leave_Type", typeof(Resource))]
      public int sLeaveName { get; set; }
      public int Did { get; set; }

      [LocalizedRequired]
      [LocalizedValidMaxLength(150)]
      [LocalizedDisplayName("Leave_Type", typeof(Resource))]
      public string Leave_Name { get; set; }

      [LocalizedRequired]
      [LocalizedDisplayName("Deduct_In_Payroll", typeof(Resource))]
      public bool Deduct_In_Payroll { get; set; }

      [LocalizedRequired]
      [LocalizedDisplayName("Allowed_Probation", typeof(Resource))]
      public bool Allowed_Probation { get; set; }

      [LocalizedDisplayName("Months_To_Expiry", typeof(Resource))]
      public int Months_To_Expiry { get; set; }

      [LocalizedRequired]
      [LocalizedDisplayName("Bring_Forward", typeof(Resource))]
      public bool Bring_Forward { get; set; }

      [LocalizedDisplayName("Bring_Forward_Percent", typeof(Resource))]
      public Nullable<decimal> Bring_Forward_Percent { get; set; }

      public Nullable<bool> Is_Default { get; set; }
      public bool Is_Bring_Forward_Days { get; set; }

      [LocalizedDisplayName("Days", typeof(Resource))]
      public decimal Bring_Forward_Days { get; set; }

      [LocalizedDisplayName("Fiscal_Year", typeof(Resource))]
      public bool Fiscal_Year { get; set; }

      [LocalizedDisplayName("Custom_Fiscal_Year", typeof(Resource))]
      public string Custom_Fiscal_Year { get; set; }

      [LocalizedRequired]
      [LocalizedDisplayName("Upload_Document", typeof(Resource))]
      public bool Upload_Document { get; set; }

      [LocalizedValidMaxLength(300)]
      [LocalizedDisplayName("Leave_Desc", typeof(Resource))]
      public String Leave_Description { get; set; }
      public String Type { get; set; }

      [LocalizedDisplayName("Accumulative", typeof(Resource))]
      public bool Is_Accumulative { get; set; }
   }

   public class LeaveDefaultDetailViewModel
   {
      public int Index { get; set; }
      public Nullable<int> Default_Detail_ID { get; set; }
      public Nullable<int> Default_ID { get; set; }

      [LocalizedRequired]
      [LocalizedDisplayName("Default_Leave_Amount", typeof(Resource))]
      public Nullable<decimal> Default_Leave_Amount { get; set; }

      [LocalizedRequired]
      [LocalizedDisplayName("Year_Service", typeof(Resource))]
      public Nullable<int> Year_Service { get; set; }
      public string Row_Type { get; set; }
   }

   public class LeaveDefaultViewModel : ModelBase
   {
      public List<Leave_Default> LeaveDefaultList;
      public String LeaveDefault { get; set; }
      public List<ComboViewModel> LeaveDefList { get; set; }
      public String LeaveDefaultType { get; set; }
   }

   public class LeaveDefaultChildViewModel : ModelBase
   {
      public List<ComboViewModel> monthList { get; set; }
      public List<ComboViewModel> genderList { get; set; }
      public List<ComboViewModel> maritalList { get; set; }
      public List<ComboViewModel> residentalStatusList { get; set; }
      public List<ComboViewModel> periodList { get; set; }

      public int[] Condition_Rows { get; set; }
      public int Did { get; set; }

      [LocalizedRequired]
      [LocalizedValidMaxLength(150)]
      [LocalizedDisplayName("Leave_Name", typeof(Resource))]
      public string Leave_Name { get; set; }

      [LocalizedValidMaxLength(300)]
      [LocalizedDisplayName("Leave_Desc", typeof(Resource))]
      public String Leave_Description { get; set; }
      public bool Flexibly { get; set; }
      public bool Continuously { get; set; }

      [LocalizedDisplayName("Valid_Period", typeof(Resource))]
      public int Valid_Period { get; set; }
      public Leave_Default_Child_Detail[] Detail_Rows { get; set; }
      public String Type { get; set; }
      public Nullable<bool> Is_Default { get; set; }
   }

}

