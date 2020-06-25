using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using SBSModel.Common;
using System.Data.Entity;
using SBSModel.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Threading.Tasks;
using SBSResourceAPI;
using System.Data.Entity.Validation;

namespace SBSModel.Models
{
   //Added by sun 02-02-2016
   public class ImportEmployeeProfile_ : ModelBase
   {
      public Nullable<int> Company_ID { get; set; }
      public bool Validate_Emp { get; set; }
      public string ErrMsg_Emp { get; set; }

      public string Email_Address { get; set; }
      public string First_Name { get; set; }
      public Nullable<int> Nationality_ID { get; set; }
      public string Nationality_ { get; set; }
      public string Mobile_No { get; set; }
      public Nullable<int> Gender { get; set; }
      public string Gender_ { get; set; }
      public string NRIC { get; set; }
      public Nullable<int> Marital_Status { get; set; }
      public string Marital_Status_ { get; set; }
      public Nullable<int> Race { get; set; }
      public string Race_ { get; set; }
      public string Residential_Status { get; set; }
      public string Residential_Status_ { get; set; }
      public Nullable<int> Religion { get; set; }
      public string Religion_ { get; set; }
      public string Employee_No { get; set; }
      public string Middle_Name { get; set; }
      public string Last_Name { get; set; }
      public Nullable<int> Payment_Type { get; set; }
      public string Payment_Type_ { get; set; }
      public string DOB { get; set; }
      public string Date_Of_Issue { get; set; }
      public string Date_Of_Expire { get; set; }
      public string Address1 { get; set; }
      public string Address1_Postal_Code { get; set; }
      public Nullable<int> Address1_Country { get; set; }
      public string Address1_Country_ { get; set; }
      public string Address2 { get; set; }
      public string Address2_Postal_Code { get; set; }
      public Nullable<int> Address2_Country { get; set; }
      public string Address2_Country_ { get; set; }
      public Nullable<int> Bank_Name { get; set; }
      public string Bank_Name_ { get; set; }
      public string Bank_Account { get; set; }
      public string Effective_Date { get; set; }
      public string Passport { get; set; }


   }
   public class ImportHistory_ : ModelBase
   {
      public Nullable<int> Company_ID { get; set; }
      public bool Validate_His { get; set; }
      public string ErrMsg_His { get; set; }

      public string Employee_No { get; set; }
      public Nullable<int> Department_ID { get; set; }
      public string Department_ { get; set; }
      public Nullable<int> Designation_ID { get; set; }
      public string Designation_ { get; set; }
      public Nullable<int> Branch_ID { get; set; }
      public string Branch_ { get; set; }
      public string Effective_Date { get; set; }
      public string Confirm_Date { get; set; }
      public Nullable<int> Currency_ID { get; set; }
      public string Currency_ { get; set; }
      public string Basic_Salary { get; set; }
      public Nullable<int> Employee_Type { get; set; }
      public string Employee_Type_ { get; set; }
      public string Basic_Salary_Unit { get; set; }

   }
   public class ImportEmergencyContact_ : ModelBase
   {
      public Nullable<int> Company_ID { get; set; }
      public bool Validate_Emergency { get; set; }
      public string ErrMsg_Emergency { get; set; }

      public string Employee_No { get; set; }
      public string Name { get; set; }
      public string Contact_No { get; set; }
      public Nullable<int> Relationship_ID { get; set; }
      public string Relationship_ { get; set; }

   }
   public class ImportRelationship_ : ModelBase
   {
      public Nullable<int> Company_ID { get; set; }
      public bool Validate_Relation { get; set; }
      public string ErrMsg_Relation { get; set; }

      public string Employee_No { get; set; }
      public string Name { get; set; }
      public Nullable<int> Relationship_ID { get; set; }
      public string Relationship_ { get; set; }
      public string DOB { get; set; }
      public Nullable<int> Nationality_ID { get; set; }
      public string Nationality_ { get; set; }
      public string NRIC { get; set; }

   }


   public class EmployeeCriteria : CriteriaBase
   {
      public int Employee_Profile_ID { get; set; }
      public Nullable<int> Profile_ID { get; set; }
      public Nullable<int> Nationality_ID { get; set; }
      public Nullable<int> Branch_ID { get; set; }
      public Nullable<int> Department_ID { get; set; }
      public Nullable<int> Employee_Type { get; set; }
      public string Employee_No { get; set; }
      public string Date_From { get; set; }
      public string Date_To { get; set; }

   }

   public class EmployeeService
   {
      public List<User_Role> LstUserRole(int[] moduleDetailsID = null)
      {
         var currentdate = StoredProcedure.GetCurrentDate();
         using (var db = new SBS2DBContext())
         {
            if (moduleDetailsID != null)
            {
               return db.User_Role
                   .Include(i => i.Page_Role.Select(s => s.Page.SBS_Module_Detail))
                   .Where(w => w.Page_Role.Any(a => moduleDetailsID.Contains(a.Page.Module_Detail_ID.Value) | a.Page.Module_Detail_ID == null))
                   .ToList();
            }
            else
            {
               return db.User_Role.Include(i => i.Page_Role.Select(s => s.Page.SBS_Module_Detail)).ToList();
            }

         }
      }

      public List<User_Assign_Role> LstUserAssignRole(Nullable<int> pUserAuthenID)
      {
         var currentdate = StoredProcedure.GetCurrentDate();
         using (var db = new SBS2DBContext())
         {
            return db.User_Assign_Role.Where(w => w.User_Authentication_ID == pUserAuthenID).ToList();
         }
      }

      public List<Employee_Profile> LstEmployeeProfile(Nullable<int> pCompanyID, Nullable<int> pBranch = null, Nullable<int> pDepartmentID = null, Nullable<int> pEmpType = null, string pEmpNo = "")
      {
         var currentdate = StoredProcedure.GetCurrentDate();
         using (var db = new SBS2DBContext())
         {
            var emps = db.Employee_Profile
                .Include(i => i.Employment_History)
                .Include(i => i.User_Profile)
                .Include(i => i.User_Profile.User_Authentication)
                .Include(i => i.Employment_History.Select(s => s.Branch))
                .Include(i => i.Employment_History.Select(s => s.Designation))
                .Include(i => i.Employment_History.Select(s => s.Department))
                .Where(w => w.User_Profile.Company_ID == pCompanyID && w.User_Profile.User_Status != RecordStatus.Delete);

            if (pBranch.HasValue)
            {
               emps = emps.Where(w => w.Employment_History.Where(we => we.Branch_ID == pBranch).FirstOrDefault() != null);
            }

            if (pDepartmentID.HasValue)
            {
               emps = emps.Where(w => w.Employment_History.Where(we => we.Department_ID == pDepartmentID).FirstOrDefault() != null);
            }

            if (pEmpType.HasValue)
            {
               emps = emps.Where(w => w.Employment_History.Where(we => we.Employee_Type == pEmpType).FirstOrDefault() != null);
            }


            if (!string.IsNullOrEmpty(pEmpNo))
            {
               emps = emps.Where(w => w.Employee_No == pEmpNo);
            }
            return emps.ToList();
         }

      }

      //Added by Moet on 8-Aug-2016 to validate the employee who has acces right to Payroll module to be displayed
      public List<Employee_Profile> LstEmployeeProfileForPayrollAuth(Nullable<int> pCompanyID)
      {
         var currentdate = StoredProcedure.GetCurrentDate();
         using (var db = new SBS2DBContext())
         {
            var emps = db.Employee_Profile
                .Include(i => i.Employment_History)
                .Include(i => i.User_Profile)
                .Include(i => i.User_Profile.User_Authentication)
                .Include(i => i.User_Profile.User_Assign_Module.Select(s => s.Subscription))
                .Include(i => i.User_Profile.User_Assign_Module.Select(s => s.Subscription.SBS_Module_Detail))
                .Include(i => i.Employment_History.Select(s => s.Branch))
                .Include(i => i.Employment_History.Select(s => s.Designation))
                .Include(i => i.Employment_History.Select(s => s.Department))
                .Where(w => w.User_Profile.Company_ID == pCompanyID && w.User_Profile.User_Status != RecordStatus.Delete);

            emps = emps.Where(w => w.User_Profile.User_Assign_Module.Where(we => we.Subscription.SBS_Module_Detail.Module_Detail_Name == "Payroll").FirstOrDefault() != null);

            return emps.ToList();
         }

      }
      //Added by sun 22-12-2015
      public ServiceObjectResult LstEmployeeProfile(EmployeeCriteria criteria)
      {
         var result = new ServiceObjectResult();
         result.Object = new List<Employee_Profile>();
         using (var db = new SBS2DBContext())
         {
            var emps = db.Employee_Profile
               .Include(i => i.Employment_History)
               .Include(i => i.User_Profile)
               .Include(i => i.User_Profile.User_Authentication)
               .Include(i => i.Employment_History.Select(s => s.Branch))
               .Include(i => i.Employment_History.Select(s => s.Designation))
               .Include(i => i.Employment_History.Select(s => s.Department))
               .Where(w => w.User_Profile.Company_ID == criteria.Company_ID && w.User_Profile.User_Status != RecordStatus.Delete);

            if (criteria.Branch_ID.HasValue && criteria.Branch_ID.Value > 0)
            {
               emps = emps.Where(w => w.Employment_History.Where(we => we.Branch_ID == criteria.Branch_ID).FirstOrDefault() != null);
            }
            if (criteria.Department_ID.HasValue && criteria.Department_ID.Value > 0)
            {
               emps = emps.Where(w => w.Employment_History.Where(we => we.Department_ID == criteria.Department_ID).FirstOrDefault() != null);
            }
            if (criteria.Employee_Type.HasValue && criteria.Employee_Type.Value > 0)
            {
               emps = emps.Where(w => w.Employment_History.Where(we => we.Employee_Type == criteria.Employee_Type).FirstOrDefault() != null);
            }
            if (!string.IsNullOrEmpty(criteria.Employee_No))
            {
               emps = emps.Where(w => w.Employee_No == criteria.Employee_No);
            }
            if (!string.IsNullOrEmpty(criteria.Date_From))
            {
               var d = DateUtil.ToDate(criteria.Date_From);
               if (d != null && d.HasValue)
               {
                  emps = emps.Where(w => w.Hired_Date <= d | w.DOB <= d | w.PR_End_Date <= d);
               }
            }
            if (!string.IsNullOrEmpty(criteria.Date_To))
            {
               var d = DateUtil.ToDate(criteria.Date_To);
               if (d != null && d.HasValue)
               {
                  emps = emps.Where(w => w.Hired_Date >= d | w.DOB >= d | w.PR_End_Date >= d);
               }
            }

            var obj = new List<Employee_Profile>();
            obj = emps.ToList();
            result.Object = obj;
            return result;
         }
      }

      public Employee_Profile GetEmployeeProfile(Nullable<int> pEmpID)
      {
         if (pEmpID.HasValue)
         {
            using (var db = new SBS2DBContext())
            {
               return db.Employee_Profile
                       .Include(i => i.Employment_History)
                       .Include(i => i.Employment_History.Select(s => s.Currency))
                       .Include(i => i.Employment_History.Select(s => s.Branch))
                       .Include(i => i.Employment_History.Select(s => s.Designation))
                       .Include(i => i.Employment_History.Select(s => s.Department))
                       .Include(i => i.Employment_History.Select(s => s.Employment_History_Allowance))
                       .Include(i => i.User_Profile.User_Authentication)
                       .Include(i => i.User_Profile)
                       .Include(i => i.User_Profile.User_Assign_Module)
                       .Include(i => i.User_Profile.User_Profile_Photo)
                       .Include(i => i.User_Profile.User_Authentication)
                       .Include(i => i.Nationality)
                       .Include(i => i.Banking_Info)
                       .Include(i => i.Employee_Emergency_Contact)
                       .Include(i => i.Relationships)
                       .Include(i => i.Relationships.Select(s => s.Nationality))
                       .Include(i => i.Relationships.Select(s => s.Global_Lookup_Data1))
                       .Include(i => i.Employee_Attachment)
                       .Include(i => i.Employee_Attachment.Select(s => s.Global_Lookup_Data))
                       .Where(i => i.Employee_Profile_ID == pEmpID)
                    .SingleOrDefault();

            }
         }
         return null;
      }

      public Employee_Profile GetEmployeeProfile2(Nullable<int> pEmpID)
      {
         /*few Include*/
         using (var db = new SBS2DBContext())
         {
            return db.Employee_Profile
                    .Include(i => i.Nationality)
                    .Include(i => i.User_Profile)
                    .Include(i => i.User_Profile.User_Authentication)
                    .Where(i => i.Employee_Profile_ID == pEmpID)
                 .FirstOrDefault();
         }
      }

      public Employee_Profile GetEmployeeProfileByProfileID(Nullable<int> pProfileID)
      {
         using (var db = new SBS2DBContext())
         {
            return db.Employee_Profile
                  .Include(i => i.Employment_History)
                        .Include(i => i.Employment_History.Select(s => s.Currency))
                        .Include(i => i.Employment_History.Select(s => s.Branch))
                        .Include(i => i.Employment_History.Select(s => s.Designation))
                        .Include(i => i.Employment_History.Select(s => s.Department))
                        .Include(i => i.Employment_History.Select(s => s.Employment_History_Allowance))
                        .Include(i => i.User_Profile)
                        .Include(i => i.User_Profile.User_Assign_Module)
                        .Include(i => i.User_Profile.User_Profile_Photo)
                        .Include(i => i.User_Profile.User_Authentication)
                        .Include(i => i.Nationality)
                        .Include(i => i.Banking_Info)
                        .Include(i => i.Employee_Emergency_Contact)
                        .Include(i => i.Relationships)
                        .Include(i => i.Relationships.Select(s => s.Nationality))
                        .Include(i => i.Relationships.Select(s => s.Global_Lookup_Data1))
                        .Include(i => i.Employee_Attachment)
                        .Include(i => i.Employee_Attachment.Select(s => s.Global_Lookup_Data))
                    .Where(i => i.Profile_ID == pProfileID)
                 .FirstOrDefault();

         }
      }

      public Employee_Profile GetEmployeeProfileByProfileID2(Nullable<int> pProfileID)
      {
         using (var db = new SBS2DBContext())
         {
            return db.Employee_Profile
                    .Where(i => i.Profile_ID == pProfileID)
                 .FirstOrDefault();

         }
      }

      /*Added by Jane 02/02/2016*/
      public Employee_Attachment GetEmpAttachment(System.Guid pAttID)
      {
         using (var db = new SBS2DBContext())
         {
            return db.Employee_Attachment.Where(w => w.Attachment_ID == pAttID).FirstOrDefault();
         }

      }

      //Added By sun 31-08-2015
      public Nationality GetNationalByID(Nullable<int> NationalId)
      {
         if (NationalId.HasValue)
         {
            using (var db = new SBS2DBContext())
            {
               return (from a in db.Nationalities
                       where a.Nationality_ID == NationalId.Value
                       select a).SingleOrDefault();
            }
         }
         return null;
      }

      public int GetYearService(Nullable<int> pEmpID)
      {
         var currentdate = StoredProcedure.GetCurrentDate();
         if (pEmpID.HasValue)
         {
            using (var db = new SBS2DBContext())
            {
               var firsthist = db.Employment_History
                   .Where(w => w.Employee_Profile_ID == pEmpID)
                   .OrderBy(o => o.Effective_Date)
                   .FirstOrDefault();

               Nullable<DateTime> hiredDate = currentdate;
               if (firsthist != null && firsthist.Effective_Date.HasValue)
               {
                  hiredDate = firsthist.Effective_Date.Value;
               }
               var yearservice = (int)Math.Floor(((currentdate.Date - hiredDate.Value.Date).TotalDays + 1) / 365);
               return yearservice;
            }
         }
         return 0;
      }

      public bool IsHistoryRelated(Nullable<int> pHistoryID)
      {
         using (var db = new SBS2DBContext())
         {
            var hist = (from a in db.Employment_History where a.History_ID == pHistoryID select a).FirstOrDefault();
            if (hist != null)
            {
               foreach (var histAllow in hist.Employment_History_Allowance)
               {
                  if (histAllow != null)
                  {
                     if (histAllow.PRDs.Count() > 0)
                     {
                        return true;
                     }
                  }
               }
            }
         }
         return false;
      }

      public PRM GetCurrentPRM(Nullable<int> pEmpID)
      {
         if (pEmpID.HasValue)
         {
            var currentdate = StoredProcedure.GetCurrentDate();
            using (var db = new SBS2DBContext())
            {
               return db.PRMs
                    .Where(i => i.Employee_Profile_ID == pEmpID && i.Process_Month == currentdate.Month && i.Process_Year == currentdate.Year)
                    .FirstOrDefault();

            }
         }
         return null;
      }

      public string GetEmployeeNo(Nullable<int> pEmpID, bool dosave = true)
      {
         if (pEmpID.HasValue)
         {
            var currentdate = StoredProcedure.GetCurrentDate();
            using (var db = new SBS2DBContext())
            {
               var emp = db.Employee_Profile.Where(w => w.Employee_Profile_ID == pEmpID).FirstOrDefault();
               if (emp != null)
               {
                  var pattern = db.Employee_No_Pattern.Where(w => w.Company_ID == emp.User_Profile.Company_ID).FirstOrDefault();
                  if (pattern != null)
                  {
                     var empNo = GetEmployeeNo(pattern, currentdate, null, emp.Nationality_ID);

                     var number = empNo.Substring(empNo.Length - 6, 6);
                     while (db.Employee_Profile.Where(w => w.User_Profile.Company_ID == emp.User_Profile.Company_ID && w.Employee_No.Substring(w.Employee_No.Length - 6, 6) == number).FirstOrDefault() != null)
                     {
                        pattern.Current_Running_Number = pattern.Current_Running_Number + 1;
                        empNo = GetEmployeeNo(pattern, currentdate, null, emp.Nationality_ID);
                        number = empNo.Substring(empNo.Length - 6, 6);
                     }
                     emp.Employee_No = empNo;
                     if (dosave)
                        db.SaveChanges();
                     return empNo;
                  }
               }

            }
         }
         return "";
      }
     
      public bool ExsistEmployeeNo(int? pCompany, int? pEmpID, string pEmpNo)
      {

         using (var db = new SBS2DBContext())
         {
            var number = pEmpNo.Substring(pEmpNo.Length - 6, 6);
            var dupNo = db.Employee_Profile.Where(w => w.User_Profile.Company_ID == pCompany && w.Employee_Profile_ID != pEmpID && w.Employee_No.Substring(w.Employee_No.Length - 6, 6) == number).FirstOrDefault();
            if (dupNo != null)
               return true;

            return false;
         }
      }

      private string RegenEmployeeNo(Employee_No_Pattern pattern, DateTime currentdate, Nullable<int> Employee_Profile_ID, string Emp_No, Nullable<int> Nationality_ID)
      {
         var empNos = Emp_No.Split(new string[] { "-" }, StringSplitOptions.RemoveEmptyEntries);
         if (empNos.Length > 0)
         {
            var empNo = "";
            var year = empNos[0];
            var number = empNos[empNos.Length - 1];

            empNo += year + "-";
            var nal = "XX";
            var branch = "XXXXXXXXXX";
            if (pattern.Select_Nationality)
            {
               using (var db = new SBS2DBContext())
               {
                  var nationality = (from a in db.Nationalities where a.Nationality_ID == Nationality_ID select a).FirstOrDefault();
                  if (nationality != null)
                     nal = nationality.Name;
               }

               empNo += nal + "-";
            }
            if (pattern.Select_Branch_Code.HasValue && pattern.Select_Branch_Code.Value)
            {
               using (var db = new SBS2DBContext())
               {
                  var hist = db.Employment_History.Where(w => w.Effective_Date <= currentdate & w.Employee_Profile_ID == Employee_Profile_ID).OrderByDescending(o => o.Effective_Date).FirstOrDefault();
                  if (hist != null && hist.Branch != null)
                     branch = hist.Branch.Branch_Code;
                  else if (pattern.Branch != null)
                     branch = pattern.Branch.Branch_Code;
               }

               empNo += branch + "-";
            }
            empNo += number;
            return empNo;
         }
         return "";
      }
     
      private string GetEmployeeNo(Employee_No_Pattern pattern, DateTime currentdate, Nullable<DateTime> Hired_Date, Nullable<int> Nationality_ID)
      {
         DateTime pattenDate;
         var year = "";
         var nationality = "";
         var branchcode = "";
         if (Hired_Date.HasValue)
            pattenDate = Hired_Date.Value;
         else
            pattenDate = currentdate;

         if (pattern.Year_2_Digit)
            year = pattenDate.Year.ToString().Substring(2, 2) + "-";
         else if (pattern.Year_4_Digit)
            year = pattenDate.Year.ToString() + "-";

         if (pattern.Select_Nationality)
         {
            using (var db = new SBS2DBContext())
            {
               var nal = (from a in db.Nationalities where a.Nationality_ID == Nationality_ID select a).FirstOrDefault();
               if (nal != null)
                  nationality = nal.Name + "-";
               else
                  nationality = "XX" + "-";
            }
         }
         if (pattern.Select_Branch_Code.HasValue && pattern.Select_Branch_Code.Value && pattern.Branch != null)
            branchcode = pattern.Branch.Branch_Code + "-";

         return year + nationality + branchcode + pattern.Current_Running_Number.Value.ToString("000000");
      }

      public ServiceResult SaveEmployeeProfile(Employee_Profile pEmpProfile, Relationship[] pRelationshipList, string[] pRelationRowType, Banking_Info[] pBankInfoList, string[] pBankRowType, User_Profile user, Nullable<int> pCompanyID)
      {
         try
         {
            var currentdate = StoredProcedure.GetCurrentDate();
            using (var db = new SBS2DBContext())
            {
               if (pEmpProfile == null || !pCompanyID.HasValue)
               {
                  return new ServiceResult { Code = ERROR_CODE.ERROR_511_DATA_NOT_FOUND, Msg = new Error().getError(ERROR_CODE.ERROR_511_DATA_NOT_FOUND) };
               }
               Employee_Profile current = (from a in db.Employee_Profile where a.Profile_ID == pEmpProfile.Profile_ID select a).FirstOrDefault();
               if (current == null)
               {
                  //Insert
                  //---pattern----

                  var pattern = (from a in db.Employee_No_Pattern where a.Company_ID == pCompanyID.Value select a).SingleOrDefault();
                  if (pattern == null)
                     return new ServiceResult { Code = ERROR_CODE.ERROR_511_DATA_NOT_FOUND, Msg = new Error().getError(ERROR_CODE.ERROR_511_DATA_NOT_FOUND) };

                  pEmpProfile.Employee_No = GetEmployeeNo(pattern, currentdate, pEmpProfile.Hired_Date, pEmpProfile.Nationality_ID);
                  var number = pEmpProfile.Employee_No.Substring(pEmpProfile.Employee_No.Length - 6, 6);
                  while (db.Employee_Profile.Where(w => w.User_Profile.Company_ID == user.Company_ID && w.Employee_No.Substring(w.Employee_No.Length - 6, 6) == number).FirstOrDefault() != null)
                  {
                     pattern.Current_Running_Number = pattern.Current_Running_Number + 1;
                     pEmpProfile.Employee_No = GetEmployeeNo(pattern, currentdate, pEmpProfile.Hired_Date, pEmpProfile.Nationality_ID);
                     number = pEmpProfile.Employee_No.Substring(pEmpProfile.Employee_No.Length - 6, 6);
                  }

                  //---Relationship---
                  if (pRelationRowType != null)
                  {
                     for (var i = 0; i < pRelationRowType.Length; i++)
                     {
                        if (pRelationRowType[i] == RowType.ADD)
                        {
                           var relation = pRelationshipList[i];
                           relation.Employee_Profile_ID = pEmpProfile.Employee_Profile_ID;
                           relation.Profile_ID = pEmpProfile.Profile_ID;
                           relation.Create_By = user.Name;
                           relation.Create_On = currentdate;
                           relation.Update_By = user.Name;
                           relation.Update_On = currentdate;
                           pEmpProfile.Relationships.Add(relation);
                        }
                     }

                  }

                  //---Bank info----
                  if (pBankRowType != null)
                  {
                     for (var i = 0; i < pBankRowType.Length; i++)
                     {
                        if (pBankRowType[i] == RowType.ADD)
                        {
                           var bank = pBankInfoList[i];
                           bank.Employee_Profile_ID = pEmpProfile.Employee_Profile_ID;
                           bank.Profile_ID = pEmpProfile.Profile_ID;
                           bank.Create_By = user.Name;
                           bank.Create_On = currentdate;
                           bank.Update_By = user.Name;
                           bank.Update_On = currentdate;
                           pEmpProfile.Banking_Info.Add(bank);
                        }
                     }

                  }

                  pEmpProfile.Banking_Info = pBankInfoList;
                  db.Employee_Profile.Add(pEmpProfile);
                  db.SaveChanges();
                  db.Entry(pEmpProfile).GetDatabaseValues();
                  return new ServiceResult { Code = ERROR_CODE.SUCCESS_CREATE, Msg = new Success().getSuccess(ERROR_CODE.SUCCESS_CREATE) };
               }
               else
               {
                  // Update

                  //---Relationship
                  if (pRelationRowType != null)
                  {
                     for (var i = 0; i < pRelationRowType.Length; i++)
                     {
                        var relation = pRelationshipList[i];
                        relation.Employee_Profile_ID = pEmpProfile.Employee_Profile_ID;
                        relation.Profile_ID = pEmpProfile.Profile_ID;
                        relation.Update_By = user.Name;
                        relation.Update_On = currentdate;
                        if (pRelationRowType[i] == RowType.ADD)
                        {
                           db.Relationships.Add(relation);
                           relation.Create_By = user.Name;
                           relation.Create_On = currentdate;
                        }
                        else if (pRelationRowType[i] == RowType.EDIT)
                        {
                           var currentrelation = (from a in db.Relationships where a.Relationship_ID == relation.Relationship_ID select a).FirstOrDefault();
                           relation.Create_By = currentrelation.Create_By;
                           relation.Create_On = currentrelation.Create_On;
                           db.Entry(currentrelation).CurrentValues.SetValues(relation);
                        }
                        else if (pRelationRowType[i] == RowType.DELETE)
                        {
                           if (relation.Relationship_ID > 0)
                           {
                              var currentrelation = (from a in db.Relationships where a.Relationship_ID == relation.Relationship_ID select a).FirstOrDefault();
                              db.Relationships.Remove(currentrelation);
                           }
                        }
                     }
                  }

                  //---Bank Info -----
                  if (pBankRowType != null)
                  {
                     for (var i = 0; i < pBankRowType.Length; i++)
                     {
                        var bank = pBankInfoList[i];
                        bank.Employee_Profile_ID = pEmpProfile.Employee_Profile_ID;
                        bank.Profile_ID = pEmpProfile.Profile_ID;
                        bank.Update_By = user.Name;
                        bank.Update_On = currentdate;
                        if (pBankRowType[i] == RowType.ADD)
                        {
                           db.Banking_Info.Add(bank);
                           bank.Create_By = user.Name;
                           bank.Create_On = currentdate;
                        }
                        else if (pBankRowType[i] == RowType.EDIT)
                        {
                           var currentbankinfo = (from a in db.Banking_Info where a.Banking_Info_ID == bank.Banking_Info_ID select a).FirstOrDefault();
                           bank.Create_By = currentbankinfo.Create_By;
                           bank.Create_On = currentbankinfo.Create_On;
                           db.Entry(currentbankinfo).CurrentValues.SetValues(bank);
                        }
                        else if (pBankRowType[i] == RowType.DELETE)
                        {
                           if (bank.Banking_Info_ID > 0)
                           {
                              var currentbankinfo = (from a in db.Banking_Info where a.Banking_Info_ID == bank.Banking_Info_ID select a).FirstOrDefault();
                              db.Banking_Info.Remove(currentbankinfo);
                           }
                        }
                     }
                  }
                  db.Entry(current).CurrentValues.SetValues(pEmpProfile);
                  db.SaveChanges();
                  return new ServiceResult { Code = ERROR_CODE.SUCCESS_EDIT, Msg = new Success().getSuccess(ERROR_CODE.SUCCESS_EDIT) };
               }
            }
         }
         catch
         {
            return new ServiceResult { Code = ERROR_CODE.ERROR_500_DB, Msg = new Error().getError(ERROR_CODE.ERROR_500_DB) };
         }
      }

      public List<string> LstEmail(Nullable<int> pCompanyID)
      {
         using (var db = new SBS2DBContext())
         {
            return db.User_Profile.Where(w => w.Company_ID == pCompanyID).Select(s => s.User_Authentication.Email_Address).ToList();

         }
      }

      public static int LINK_TIME_LIMIT = 120;

      private readonly Random _rng = new Random();

      public const string _chars = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmopqrstuvwxyz";

      public string randomString(int size)
      {

         char[] buffer = new char[size];

         for (int i = 0; i < size; i++)
         {
            buffer[i] = _chars[_rng.Next(_chars.Length)];
         }
         return new string(buffer);
      }

      public Boolean validateActivationCode(SBS2DBContext db, String code)
      {
         Activation_Link u = (from a in db.Activation_Link where a.Activation_Code.Equals(code) select a).FirstOrDefault();
         if (u != null)
            return false;
         else
            return true;
      }

      public ServiceResult InsertEmployee(User_Profile pUserProfile, int[] pUserAssignRole, int[] pUserAssignModule, Employee_Profile pEmp)
      {
         try
         {
            var currentdate = StoredProcedure.GetCurrentDate();
            using (var db = new SBS2DBContext())
            {
               if (pUserProfile.Is_Email.HasValue && pUserProfile.Is_Email.Value)
               {
                  if (!string.IsNullOrEmpty(pUserProfile.Email))
                  {
                     if (db.Users.Where(w => w.UserName.ToLower() == pUserProfile.Email.ToLower()).FirstOrDefault() != null)
                        return new ServiceResult { Code = ERROR_CODE.ERROR_510_DATA_DUPLICATE, Msg = Resource.Email + " " + Resource.Is_Duplicated_Lower, Field = Resource.Employee };
                  }
               }
               else
               {
                  if (!string.IsNullOrEmpty(pUserProfile.User_Name))
                  {
                     if (db.User_Profile.Where(w => w.User_Name.ToLower() == pUserProfile.User_Name.ToLower()).FirstOrDefault() != null)
                        return new ServiceResult { Code = ERROR_CODE.ERROR_510_DATA_DUPLICATE, Msg = Resource.UserName + " " + Resource.Is_Duplicated_Lower, Field = Resource.Employee };
                  }
               }

               var com = (from a in db.Company_Details
                          where (a.Effective_Date <= currentdate & a.Company_ID == pUserProfile.Company_ID)
                          orderby a.Effective_Date descending
                          select a).FirstOrDefault();

               if (com == null)
                  return new ServiceResult { Code = ERROR_CODE.ERROR_511_DATA_NOT_FOUND, Msg = Resource.Company + " " + Resource.Not_Found_Msg, Field = Resource.Employee };

               var pattern = (from a in db.Employee_No_Pattern where a.Company_ID == pUserProfile.Company_ID.Value select a).FirstOrDefault();
               if (pattern == null)
                  return new ServiceResult { Code = ERROR_CODE.ERROR_511_DATA_NOT_FOUND, Msg = Resource.Company + " " + Resource.Not_Found_Msg, Field = Resource.Pattern };

               var guid = Guid.NewGuid().ToString();
               while (db.Users.Where(w => w.Id == guid).FirstOrDefault() != null)
               {
                  guid = Guid.NewGuid().ToString();
               }

               if (pUserProfile.Is_Email == false)
                   db.Users.Add(new ApplicationUser() { Id = guid, UserName = pUserProfile.User_Name.ToLower() });
               else
                  db.Users.Add(new ApplicationUser() { Id = guid, UserName = pUserProfile.Email.ToLower() });

               var ApplicationUser_Id = guid;

               #region User Authentication
               User_Authentication authen = new User_Authentication()
               {
                  Email_Address = pUserProfile.Email,
                  Company_ID = com.Company_ID,
                  ApplicationUser_Id = ApplicationUser_Id,
                  Create_By = pUserProfile.Create_By,
                  Create_On = pUserProfile.Create_On,
                  Update_By = pUserProfile.Update_By,
                  Update_On = pUserProfile.Update_On,
                  Is_Email = pUserProfile.Is_Email,
               };
               #endregion

               #region User Profile
               User_Profile user = new User_Profile()
               {
                  First_Name = pUserProfile.First_Name,
                  Middle_Name = pUserProfile.Middle_Name,
                  Last_Name = pUserProfile.Last_Name,
                  User_Status = pUserProfile.User_Status,
                  Phone = pUserProfile.Phone,
                  Registration_Date = currentdate,
                  Company_ID = com.Company_ID,
                  Is_Email = pUserProfile.Is_Email,
                  User_Authentication = authen,
                  User_Transactions = pUserProfile.User_Transactions,
                  Create_By = pUserProfile.Create_By,
                  Create_On = pUserProfile.Create_On,
                  Update_By = pUserProfile.Update_By,
                  Update_On = pUserProfile.Update_On,
               };
               #endregion

               if (pUserProfile.Is_Email.HasValue && pUserProfile.Is_Email.Value)
               {
                  authen.Email_Address = pUserProfile.Email;
                  user.Email = pUserProfile.Email;
               }
               else
               {
                  authen.User_Name = pUserProfile.User_Name;
                  user.User_Name = pUserProfile.User_Name;
               }

               #region User Profile Photo
               if (pUserProfile.User_Profile_Photo != null)
               {
                  var photo = pUserProfile.User_Profile_Photo.FirstOrDefault();
                  if (photo != null)
                  {
                     var photo_guid = Guid.NewGuid();
                     while (db.User_Profile_Photo.Where(w => w.User_Profile_Photo_ID == photo_guid).FirstOrDefault() != null)
                        photo_guid = Guid.NewGuid();

                     var profile_photo = new User_Profile_Photo()
                     {
                        User_Profile_Photo_ID = photo_guid,
                        Photo = photo.Photo,
                        Profile_ID = photo.Profile_ID,
                        Create_By = photo.Create_By,
                        Create_On = photo.Create_On,
                        Update_By = photo.Update_By,
                        Update_On = photo.Update_On
                     };
                     user.User_Profile_Photo.Add(profile_photo);
                  }
               }
               #endregion
               //GENERATE ACTIVATION CODE
               String code;
               do
               {
                  code = "A" + randomString(40);
               }
               while (!validateActivationCode(db, code));

               #region User Assign Role
               //SET User_Assign_Role to ROLE_CUSTOMER_USER
               if (pUserAssignRole != null)
               {
                  foreach (var r in pUserAssignRole)
                  {
                     User_Assign_Role role = new User_Assign_Role()
                     {
                        User_Role_ID = r,
                        Create_By = pUserProfile.Create_By,
                        Create_On = pUserProfile.Create_On,
                        Update_By = pUserProfile.Update_By,
                        Update_On = pUserProfile.Update_On
                     };
                     authen.User_Assign_Role.Add(role);
                  }
               }
               #endregion

               #region User Assign Module
               if (pUserAssignModule != null)
               {
                  foreach (var s in pUserAssignModule)
                  {
                     var sub = db.Subscriptions.Where(w => w.Subscription_ID == s).FirstOrDefault();
                     if (sub != null)
                     {
                        if (sub.No_Of_Users.Value - sub.User_Assign_Module.Count() > 0)
                        {
                           var um = new User_Assign_Module()
                           {
                              Subscription_ID = s
                           };
                           user.User_Assign_Module.Add(um);
                        }
                     }
                  }
               }
               else
               {
                  var AssignModules = db.Subscriptions.Where(w => w.Company_ID == com.Company_ID).ToArray();
                  foreach (var s in AssignModules)
                  {
                     var um = new User_Assign_Module()
                     {
                        Subscription_ID = s.Subscription_ID
                     };
                     user.User_Assign_Module.Add(um);
                  }
               }
               #endregion

               #region Activation Link
               Activation_Link activation_link = new Activation_Link()
               {
                  Activation_Code = code,
                  Time_Limit = currentdate.AddHours(LINK_TIME_LIMIT),
                  Create_By = pUserProfile.Create_By,
                  Create_On = pUserProfile.Create_On,
                  Update_By = pUserProfile.Update_By,
                  Update_On = pUserProfile.Update_On
               };
               authen.Activation_Link.Add(activation_link);
               #endregion
               //var hireddate =pEmp.Hired_Date;

               pEmp.Employee_No = GetEmployeeNo(pattern, currentdate, null, pEmp.Nationality_ID);
               var number = pEmp.Employee_No.Substring(pEmp.Employee_No.Length - 6, 6);
               while (db.Employee_Profile.Where(w => w.User_Profile.Company_ID == user.Company_ID && w.Employee_No.Substring(w.Employee_No.Length - 6, 6) == number).FirstOrDefault() != null)
               {
                  pattern.Current_Running_Number = pattern.Current_Running_Number + 1;
                  pEmp.Employee_No = GetEmployeeNo(pattern, currentdate, null, pEmp.Nationality_ID);
                  number = pEmp.Employee_No.Substring(pEmp.Employee_No.Length - 6, 6);
               }

               #region Employment History
               if (pEmp.Employment_History != null)
                   {
                  foreach (var row in pEmp.Employment_History)
                  {
                     if (!row.Branch_ID.HasValue && !string.IsNullOrEmpty(row.Other_Branch))
                     {
                       var bService = new BranchService();
                        var branch = bService.GetBranch(pUserProfile.Company_ID.Value, row.Other_Branch.Trim());
                        if (branch == null)
                       {
                           var b = new Branch()
                           {
                              Company_ID = pUserProfile.Company_ID.Value,
                              Branch_Code = row.Other_Branch,
                              Branch_Name = row.Other_Branch,
                               Create_On = currentdate,
                              Create_By = "System",
                              Update_On = currentdate,
                              Update_By = "System",
                               Record_Status = Resource.Active
                           };
                           db.Branches.Add(b);
                           row.Branch_ID = b.Branch_ID;
                       }                  
                        else
                           row.Branch_ID = branch.Branch_ID;
                   }
                     else
                     {
                        row.Branch_ID = row.Branch_ID;
                     }

                     if (!row.Department_ID.HasValue && !string.IsNullOrEmpty(row.Other_Department))
               {
                   var dpService = new DepartmentService();
                        var department = dpService.GetDepartment(pUserProfile.Company_ID.Value, row.Other_Department.Trim());
                        if (department == null)
                   {
                       //insert
                       var dp = new Department()
                       {
                              Name = row.Other_Department,
                           Record_Status = Resource.Active,
                              Company_ID = pUserProfile.Company_ID.Value,
                           Create_On = currentdate,
                              Create_By = "System",
                              Update_On = currentdate,
                              Update_By = "System",
                       };
                           db.Departments.Add(dp);
                           row.Department_ID = dp.Department_ID;
                   }                  
                        else
                           row.Department_ID = department.Department_ID;
               }
                     else
                     {
                        row.Department_ID = row.Department_ID;
                     }

                     if (!row.Designation_ID.HasValue && !string.IsNullOrEmpty(row.Other_Designation))
               {
                   var dService = new DesignationService();
                        var designation = dService.GetDesignation(pUserProfile.Company_ID.Value, row.Other_Designation.Trim());
                        if (designation == null)
                   {
                       //insert
                       var ds = new Designation()
                       {
                              Name = row.Other_Designation,
                           Record_Status = Resource.Active,
                              Company_ID = pUserProfile.Company_ID.Value,
                           Create_On = currentdate,
                              Create_By = "System",
                              Update_On = currentdate,
                              Update_By = "System",
                       };
                           db.Designations.Add(ds);
                           row.Designation_ID = ds.Designation_ID;
                   }
                        else
                           row.Designation_ID = designation.Designation_ID;
               }
                     else
                     {
                        row.Designation_ID = row.Designation_ID;
                     }
                  }
               }
               #endregion

               user.Employee_Profile.Add(pEmp);
               db.User_Profile.Add(user);
               db.SaveChanges();
               db.Entry(user).GetDatabaseValues();
               return new ServiceResult() { Code = ERROR_CODE.SUCCESS, Msg = new Success().getSuccess(ERROR_CODE.SUCCESS_CREATE), Field = Resource.Employee, Object = code };
            }
         }
         catch
         {
            return new ServiceResult() { Code = ERROR_CODE.ERROR_503_INSERT_ERROR, Msg = new Error().getError(ERROR_CODE.ERROR_503_INSERT_ERROR), Field = Resource.Employee };
         }
      }

      public ServiceResult UpdateEmployeeNo(Nullable<int> pEmpID, string pEmpNo)
      {
         var curdate = StoredProcedure.GetCurrentDate();
         try
         {
            using (var db = new SBS2DBContext())
            {
               var emp = db.Employee_Profile.Where(w => w.Employee_Profile_ID == pEmpID).FirstOrDefault();
               if (emp != null)
               {
                  emp.Employee_No = pEmpNo;


               }
               db.SaveChanges();
            }
            return new ServiceResult() { Code = ERROR_CODE.SUCCESS, Msg = new Success().getSuccess(ERROR_CODE.SUCCESS_EDIT), Field = Resource.Employee };

         }
         catch
         {
            return new ServiceResult() { Code = ERROR_CODE.ERROR_504_UPDATE_ERROR, Msg = new Error().getError(ERROR_CODE.ERROR_504_UPDATE_ERROR), Field = Resource.Employee };
         }
      }

      public ServiceResult UpdateEmployee(User_Profile pUserProfile, int[] pUserAssignRole, int[] pUserAssignModule, Employee_Profile pEmp)
      {
         try
         {
            var currentdate = StoredProcedure.GetCurrentDate();
            using (var db = new SBS2DBContext())
            {
               var currProfile = db.User_Profile.Where(w => w.Profile_ID == pUserProfile.Profile_ID).FirstOrDefault();
               if (currProfile == null)
                  return new ServiceResult { Code = ERROR_CODE.ERROR_511_DATA_NOT_FOUND, Msg = Resource.User + " " + Resource.Not_Found_Msg, Field = Resource.Employee };

               if (pUserProfile.Is_Email.HasValue && pUserProfile.Is_Email.Value)
               {
                  if (!string.IsNullOrEmpty(currProfile.Email) && !string.IsNullOrEmpty(pUserProfile.Email))
                  {
                     if (currProfile.Email.ToLower() != pUserProfile.Email.ToLower())
                     {
                        if (db.Users.Where(w => w.UserName.ToLower() == pUserProfile.Email.ToLower()).FirstOrDefault() != null)
                           return new ServiceResult { Code = ERROR_CODE.ERROR_510_DATA_DUPLICATE, Msg = Resource.Duplicate_Small + Resource.Email, Field = Resource.User };
                     }
                  }
                  else
                  {
                     if (!string.IsNullOrEmpty(pUserProfile.Email))
                     {
                        if (db.Users.Where(w => w.UserName.ToLower() == pUserProfile.Email.ToLower()).FirstOrDefault() != null)
                           return new ServiceResult { Code = ERROR_CODE.ERROR_510_DATA_DUPLICATE, Msg = Resource.Duplicate_Small + Resource.Email, Field = Resource.User };
                     }
                  }
               }
               else
               {
                  if (!string.IsNullOrEmpty(currProfile.User_Name) && !string.IsNullOrEmpty(pUserProfile.User_Name))
                  {
                     if (currProfile.User_Name.ToLower() != pUserProfile.User_Name.ToLower())
                     {
                        if (db.Users.Where(w => w.UserName.ToLower() == pUserProfile.User_Name.ToLower()).FirstOrDefault() != null)
                           return new ServiceResult { Code = ERROR_CODE.ERROR_510_DATA_DUPLICATE, Msg = Resource.Duplicate_Small + Resource.User_Name, Field = Resource.User };
                     }
                  }
                  else
                  {
                     if (!string.IsNullOrEmpty(pUserProfile.User_Name))
                     {
                        if (db.Users.Where(w => w.UserName.ToLower() == pUserProfile.User_Name.ToLower()).FirstOrDefault() != null)
                           return new ServiceResult { Code = ERROR_CODE.ERROR_510_DATA_DUPLICATE, Msg = Resource.Duplicate_Small + Resource.User_Name, Field = Resource.User };
                     }
                  }
               }

               var currasp = db.Users.Where(w => w.Id == pUserProfile.User_Authentication.ApplicationUser_Id).FirstOrDefault();
               if (currasp != null)
               {
                  if (pUserProfile.Is_Email.HasValue && pUserProfile.Is_Email.Value)
                  {
                     if (!string.IsNullOrEmpty(currasp.UserName) && !string.IsNullOrEmpty(pUserProfile.Email))
                     {
                        if (currasp.UserName.ToLower() != pUserProfile.Email.ToLower())
                        {
                           currasp.UserName = pUserProfile.Email.ToLower();
                        }
                     }
                  }
                  else
                  {
                     if (!string.IsNullOrEmpty(currasp.UserName) && !string.IsNullOrEmpty(pUserProfile.User_Name))
                     {
                        if (currasp.UserName.ToLower() != pUserProfile.User_Name.ToLower())
                        {
                           currasp.UserName = pUserProfile.User_Name.ToLower();
                        }
                     }
                  }
               }

               var com = (from a in db.Company_Details
                          where (a.Effective_Date <= currentdate & a.Company_ID == pUserProfile.Company_ID)
                          orderby a.Effective_Date descending
                          select a).FirstOrDefault();

               if (com == null)
                  return new ServiceResult { Code = ERROR_CODE.ERROR_511_DATA_NOT_FOUND, Msg = Resource.Company + " " + Resource.Not_Found_Msg, Field = Resource.Employee };

               var currEmp = db.Employee_Profile.Where(w => w.Employee_Profile_ID == pEmp.Employee_Profile_ID).FirstOrDefault();
               if (currEmp == null)
                  return new ServiceResult { Code = ERROR_CODE.ERROR_511_DATA_NOT_FOUND, Msg = Resource.Employee + " " + Resource.Not_Found_Msg, Field = Resource.Employee };

               var pattern = db.Employee_No_Pattern.Where(w => w.Company_ID == currProfile.Company_ID).FirstOrDefault();
               if (pattern == null)
                  return new ServiceResult { Code = ERROR_CODE.ERROR_511_DATA_NOT_FOUND, Msg = Resource.Employee_Pattern + " " + Resource.Not_Found_Msg, Field = Resource.Employee_Pattern };

               if (string.IsNullOrEmpty(pEmp.Employee_No))
                  pEmp.Employee_No = GetEmployeeNo(pattern, currentdate, null, pEmp.Nationality_ID);

               #region User Assign Role
               if (pUserAssignRole != null)
               {
                  //Delete unchecked  
                  foreach (User_Assign_Role urole in currProfile.User_Authentication.User_Assign_Role.ToList())
                  {
                     if (!pUserAssignRole.Contains(urole.User_Role_ID.Value))
                     {
                        db.User_Assign_Role.Remove(urole);
                     }
                  }

                  foreach (int role in pUserAssignRole)
                  {
                     var dbarole = currProfile.User_Authentication.User_Assign_Role.Where(s => s.User_Role_ID == role).FirstOrDefault();
                     if (dbarole == null)
                     {
                        User_Assign_Role arole = new User_Assign_Role()
                        {
                           User_Authentication_ID = currProfile.User_Authentication_ID,
                           User_Role_ID = role,
                           Create_On = pUserProfile.Create_On,
                           Create_By = pUserProfile.Create_By,
                           Update_By = pUserProfile.Update_By,
                           Update_On = pUserProfile.Update_On
                        };
                        db.User_Assign_Role.Add(arole);
                     }
                  }
               }
               else
               {
                  foreach (User_Assign_Role dbarole in currProfile.User_Authentication.User_Assign_Role.ToList())
                  {
                     db.User_Assign_Role.Remove(dbarole);
                  }
               }
               #endregion

               #region User Assign Module
               if (pUserAssignModule != null)
               {
                  //Delete unchecked  
                  foreach (var s in currProfile.User_Assign_Module.ToList())
                  {
                     if (!pUserAssignModule.Contains(s.Subscription_ID.Value))
                     {
                        db.User_Assign_Module.Remove(s);
                     }
                  }

                  foreach (int s in pUserAssignModule)
                  {
                     var dbsub = currProfile.User_Assign_Module.Where(w => w.Subscription_ID == s).FirstOrDefault();
                     if (dbsub == null)
                     {
                        var sub = db.Subscriptions.Where(w => w.Subscription_ID == s).FirstOrDefault();
                        if (sub != null)
                        {
                           if (sub.No_Of_Users.Value - sub.User_Assign_Module.Where(w => w.Profile_ID != pEmp.Profile_ID).Count() > 0)
                           {
                              var um = new User_Assign_Module()
                              {
                                 Subscription_ID = s,
                                 Profile_ID = pEmp.Profile_ID,
                              };
                              db.User_Assign_Module.Add(um);
                           }
                        }
                     }
                  }
               }
               else
               {
                  foreach (var dbsub in currProfile.User_Assign_Module.ToList())
                  {
                     db.User_Assign_Module.Remove(dbsub);
                  }
               }
               #endregion

               #region Employee Emergency Contact
               var emerRemove = new List<Employee_Emergency_Contact>();
               foreach (var row in currEmp.Employee_Emergency_Contact)
               {
                  if (pEmp.Employee_Emergency_Contact == null || !pEmp.Employee_Emergency_Contact.Select(s => s.Employee_Emergency_Contact_ID).Contains(row.Employee_Emergency_Contact_ID))
                     emerRemove.Add(row);
               }
               if (emerRemove.Count > 0)
                  db.Employee_Emergency_Contact.RemoveRange(emerRemove);

               if (pEmp.Employee_Emergency_Contact != null)
               {
                  foreach (var row in pEmp.Employee_Emergency_Contact)
                  {
                     if (row.Employee_Emergency_Contact_ID == 0 || !currEmp.Employee_Emergency_Contact.Select(s => s.Employee_Emergency_Contact_ID).Contains(row.Employee_Emergency_Contact_ID))
                     {
                        // add
                        db.Employee_Emergency_Contact.Add(row);
                     }
                     else
                     {
                        var currEmer = db.Employee_Emergency_Contact.Where(w => w.Employee_Emergency_Contact_ID == row.Employee_Emergency_Contact_ID).FirstOrDefault();
                        if (currEmer != null)
                        {
                           currEmer.Name = row.Name;
                           currEmer.Contact_No = row.Contact_No;
                           currEmer.Relationship = (row.Relationship.HasValue ? (row.Relationship.Value > 0 ? row.Relationship : null) : null);
                           currEmer.Update_By = pUserProfile.Update_By;
                           currEmer.Update_On = pUserProfile.Update_On;
                        }
                     }
                  }
               }
               #endregion

               #region Relationship
               var relationRemove = new List<Relationship>();
               foreach (var row in currEmp.Relationships)
               {
                  if (pEmp.Relationships == null || !pEmp.Relationships.Select(s => s.Relationship_ID).Contains(row.Relationship_ID))
                  {
                     relationRemove.Add(row);
                  }
               }
               if (relationRemove.Count > 0)
               {
                  db.Relationships.RemoveRange(relationRemove);
               }
               if (pEmp.Relationships != null)
               {
                  foreach (var row in pEmp.Relationships)
                  {
                     if (row.Relationship_ID == 0 || !currEmp.Relationships.Select(s => s.Relationship_ID).Contains(row.Relationship_ID))
                     {
                        // add
                        db.Relationships.Add(row);
                     }
                     else
                     {
                        var currRelation = db.Relationships.Where(w => w.Relationship_ID == row.Relationship_ID).FirstOrDefault();
                        if (currRelation != null)
                        {
                           currRelation.Company_Name = row.Company_Name;
                           currRelation.Company_Position = row.Company_Position;
                           currRelation.Passport = row.Passport;
                           currRelation.Child_Type = row.Child_Type;
                           currRelation.Is_Maternity_Share_Father = row.Is_Maternity_Share_Father;
                           currRelation.Working = row.Working;
                           currRelation.Name = row.Name;
                           currRelation.Relationship1 = row.Relationship1;
                           currRelation.DOB = row.DOB;
                           currRelation.Nationality_ID = row.Nationality_ID;
                           currRelation.NRIC = row.NRIC;
                           currRelation.Gender = row.Gender;
                           currRelation.Update_On = row.Update_On;
                           currRelation.Update_By = row.Update_By;
                        }
                     }
                  }
               }
               #endregion

               #region Banking Info
               var bankRemove = new List<Banking_Info>();
               foreach (var row in currEmp.Banking_Info)
               {
                  if (pEmp.Banking_Info == null || !pEmp.Banking_Info.Select(s => s.Banking_Info_ID).Contains(row.Banking_Info_ID))
                  {
                     bankRemove.Add(row);
                  }
               }
               if (bankRemove.Count > 0)
               {
                  db.Banking_Info.RemoveRange(bankRemove);
               }

               if (pEmp.Banking_Info != null)
               {
                  foreach (var row in pEmp.Banking_Info)
                  {
                     if (row.Banking_Info_ID == 0 || !currEmp.Banking_Info.Select(s => s.Banking_Info_ID).Contains(row.Banking_Info_ID))
                     {
                        // add
                        db.Banking_Info.Add(row);
                     }
                     else
                     {
                        var currBank = db.Banking_Info.Where(w => w.Banking_Info_ID == row.Banking_Info_ID).FirstOrDefault();
                        if (currBank != null)
                        {
                           currBank.Selected = row.Selected;
                           currBank.Bank_Name = row.Bank_Name;
                           currBank.Bank_Account = row.Bank_Account;
                           currBank.Payment_Type = row.Payment_Type;
                           currBank.Effective_Date = row.Effective_Date;
                           currBank.Update_By = row.Update_By;
                           currBank.Update_On = row.Update_On;
                        }
                     }
                  }
               }
               #endregion

               #region Employee Attachment
               var Attah = db.Employee_Attachment.Where(w => w.Employee_Profile_ID == pEmp.Employee_Profile_ID);
               db.Employee_Attachment.RemoveRange(Attah);

               if (pEmp.Employee_Attachment != null)
               {
                  foreach (var Arow in pEmp.Employee_Attachment)
                  {
                     var guid = Guid.NewGuid();
                     while (db.Employee_Attachment.Where(w => w.Attachment_ID == guid).FirstOrDefault() != null)
                        guid = Guid.NewGuid();

                     var Att = new Employee_Attachment()
                     {
                        Employee_Profile_ID = pEmp.Employee_Profile_ID,
                        Attachment_ID = guid,
                        Attachment_Type = Arow.Attachment_Type,
                        Attachment_File = Arow.Attachment_File,
                        File_Name = Arow.File_Name,
                        Create_By = Arow.Create_By,
                        Create_On = Arow.Create_On,
                        Uploaded_by = Arow.Uploaded_by,
                        Uploaded_On = Arow.Uploaded_On,
                     };
                     db.Employee_Attachment.Add(Att);
                  }
               }
               #endregion

               #region Employment History
               var histRemove = new List<Employment_History>();
               foreach (var row in currEmp.Employment_History)
               {
                  if (pEmp.Employment_History == null || !pEmp.Employment_History.Select(s => s.History_ID).Contains(row.History_ID))
                  {
                     histRemove.Add(row);
                  }
               }

               if (histRemove.Count > 0)
               {
                  foreach (var row in histRemove)
                  {
                     db.Employment_History_Allowance.RemoveRange(row.Employment_History_Allowance);
                  }
                  db.Employment_History.RemoveRange(histRemove);
               }


               if (pEmp.Employment_History != null)
               {
                  foreach (var row in pEmp.Employment_History)
                  {
                     if (!row.Branch_ID.HasValue && !string.IsNullOrEmpty(row.Other_Branch))
                     {
                              var bService = new BranchService();
                        var branch = bService.GetBranch(pUserProfile.Company_ID.Value, row.Other_Branch.Trim());
                        if (branch == null)
                              {
                                 var b = new Branch()
                                 {
                                    Company_ID = pUserProfile.Company_ID.Value,
                              Branch_Code = row.Other_Branch,
                              Branch_Name = row.Other_Branch,
                                    Create_On = currentdate,
                              Create_By = "System",
                              Update_On = currentdate,
                              Update_By = "System",
                                    Record_Status = Resource.Active
                                 };
                           db.Branches.Add(b);
                           row.Branch_ID = b.Branch_ID;
                              }
                              else
                           row.Branch_ID = branch.Branch_ID;
                           }
                           else
                           {
                        row.Branch_ID = row.Branch_ID;
                           }

                     if (!row.Department_ID.HasValue && !string.IsNullOrEmpty(row.Other_Department))
                           {
                              var dpService = new DepartmentService();
                        var department = dpService.GetDepartment(pUserProfile.Company_ID.Value, row.Other_Department.Trim());
                        if (department == null)
                              {
                                 //insert
                                 var dp = new Department()
                                 {
                              Name = row.Other_Department,
                                    Record_Status = Resource.Active,
                                    Company_ID = pUserProfile.Company_ID.Value,
                                    Create_On = currentdate,
                              Create_By = "System",
                              Update_On = currentdate,
                              Update_By = "System",
                                 };
                           db.Departments.Add(dp);
                           row.Department_ID = dp.Department_ID;
                              }
                              else
                           row.Department_ID = department.Department_ID;
                           }
                           else
                           {
                        row.Department_ID = row.Department_ID;
                           }

                     if (!row.Designation_ID.HasValue && !string.IsNullOrEmpty(row.Other_Designation))
                           {
                              var dService = new DesignationService();
                        var designation = dService.GetDesignation(pUserProfile.Company_ID.Value, row.Other_Designation.Trim());
                        if (designation == null)
                              {
                                 //insert
                                 var ds = new Designation()
                                 {
                              Name = row.Other_Designation,
                                    Record_Status = Resource.Active,
                                    Company_ID = pUserProfile.Company_ID.Value,
                                    Create_On = currentdate,
                              Create_By = "System",
                              Update_On = currentdate,
                              Update_By = "System",
                                 };
                           db.Designations.Add(ds);
                           row.Designation_ID = ds.Designation_ID;
                              }
                              else
                           row.Designation_ID = designation.Designation_ID;
                           }
                           else
                           {
                        row.Designation_ID = row.Designation_ID;
                           }

                     if (row.History_ID == 0 || !currEmp.Employment_History.Select(s => s.History_ID).Contains(row.History_ID))
                        db.Employment_History.Add(row);
                     else
                     {
                        var currHist = db.Employment_History.Where(w => w.History_ID == row.History_ID).FirstOrDefault();
                        if (currHist != null)
                        {
                           currHist.Branch_ID = row.Branch_ID;
                           currHist.Department_ID = row.Department_ID;
                           currHist.Designation_ID = row.Designation_ID;
                           currHist.Employee_Type = row.Employee_Type;
                           currHist.Supervisor = (row.Supervisor > 0 ? row.Supervisor : null);
                           currHist.No_Approval_WF = row.No_Approval_WF;
                           currHist.Currency_ID = row.Currency_ID;
                           currHist.Basic_Salary = EncryptUtil.Encrypt(row.Basic_Salary);
                           currHist.Basic_Salary_Unit = row.Basic_Salary_Unit;
                           currHist.Confirm_Date = row.Confirm_Date;
                           currHist.Effective_Date = row.Effective_Date;
                           currHist.Terminate_Date = row.Terminate_Date;
                           currHist.Notice_Period_Amount = row.Notice_Period_Amount;
                           currHist.Notice_Period_Unit = row.Notice_Period_Unit;
                           currHist.Contract_Staff = row.Contract_Staff;
                           currHist.Contract_Start_Date = row.Contract_Start_Date;
                           currHist.Contract_End_Date = row.Contract_End_Date;
                           currHist.Payment_Type = row.Payment_Type;
                           currHist.Days = row.Days;

                           currHist.Update_On = row.Update_On;
                           currHist.Update_By = row.Update_By;

                           var allowanceRemove = new List<Employment_History_Allowance>();
                           foreach (var arow in row.Employment_History_Allowance)
                           {
                              if (row.Employment_History_Allowance == null || !row.Employment_History_Allowance.Select(s => s.Employment_History_Allowance_ID).Contains(arow.Employment_History_Allowance_ID))
                              {
                                 allowanceRemove.Add(arow);
                              }
                           }
                           if (allowanceRemove.Count > 0)
                           {
                              db.Employment_History_Allowance.RemoveRange(allowanceRemove);
                           }

                           if (row.Employment_History_Allowance != null)
                           {
                              foreach (var arow in row.Employment_History_Allowance)
                              {
                                 if (arow.Employment_History_Allowance_ID == 0 || !row.Employment_History_Allowance.Select(s => s.Employment_History_Allowance_ID).Contains(arow.Employment_History_Allowance_ID))
                                 {
                                    // add
                                    db.Employment_History_Allowance.Add(arow);
                                 }
                                 else
                                 {
                                    var currAll = db.Employment_History_Allowance.Where(w => w.Employment_History_Allowance_ID == arow.Employment_History_Allowance_ID).FirstOrDefault();
                                    if (currAll != null)
                                    {
                                       currAll.Amount = arow.Amount;
                                       currAll.PRC_ID = arow.PRC_ID;
                                       currAll.PRT_ID = arow.PRT_ID;
                                       currAll.Update_On = arow.Update_On;
                                       currAll.Update_By = arow.Update_By;
                                       if (arow.PRC_ID == 0) currAll.PRC_ID = null;
                                    }
                                 }
                              }
                           }
                        }
                     }
                  }
               }
               #endregion

               currProfile.User_Authentication.Is_Email = pUserProfile.Is_Email;
               currProfile.Is_Email = pUserProfile.Is_Email;
               if (pUserProfile.Is_Email.HasValue && pUserProfile.Is_Email.Value)
               {
                  currProfile.User_Authentication.Email_Address = pUserProfile.Email;
                  currProfile.Email = pUserProfile.Email;
               }
               else
               {
                  currProfile.User_Authentication.User_Name = pUserProfile.User_Name;
                  currProfile.User_Name = pUserProfile.User_Name;
               }

               currProfile.User_Authentication.Update_By = pUserProfile.Create_By;
               currProfile.User_Authentication.Update_On = pUserProfile.Update_On;
               //Added by Moet
               db.Entry(currEmp).CurrentValues.SetValues(pEmp);
               db.Entry(currProfile).CurrentValues.SetValues(pUserProfile);
               db.SaveChanges();
               return new ServiceResult() { Code = ERROR_CODE.SUCCESS, Msg = new Success().getSuccess(ERROR_CODE.SUCCESS_EDIT), Field = Resource.Employee };
            }
         }
         //catch
         //{
         //   return new ServiceResult() { Code = ERROR_CODE.ERROR_504_UPDATE_ERROR, Msg = new Error().getError(ERROR_CODE.ERROR_504_UPDATE_ERROR), Field = Resource.Employee };
         //}
         catch (DbEntityValidationException e)
         {
             foreach (var eve in e.EntityValidationErrors)
             {
                 Console.WriteLine("Entity of type \"{0}\" in state \"{1}\" has the following validation errors:",
                     eve.Entry.Entity.GetType().Name, eve.Entry.State);
                 foreach (var ve in eve.ValidationErrors)
                 {
                     Console.WriteLine("- Property: \"{0}\", Error: \"{1}\"",
                         ve.PropertyName, ve.ErrorMessage);
                 }
             }
             throw;
         }
      }

      public ServiceResult Landing_UpdateEmployee(User_Profile pUserProfile, Employee_Profile pEmp)
      {
          try
          {
              var currentdate = StoredProcedure.GetCurrentDate();
              using (var db = new SBS2DBContext())
              {
                  var currProfile = db.User_Profile.Where(w => w.Profile_ID == pUserProfile.Profile_ID).FirstOrDefault();
                  if (currProfile == null)
                      return new ServiceResult { Code = ERROR_CODE.ERROR_511_DATA_NOT_FOUND, Msg = Resource.User + " " + Resource.Not_Found_Msg, Field = Resource.Employee };                  

                  var com = (from a in db.Company_Details
                             where (a.Effective_Date <= currentdate & a.Company_ID == pUserProfile.Company_ID)
                             orderby a.Effective_Date descending
                             select a).FirstOrDefault();

                  if (com == null)
                      return new ServiceResult { Code = ERROR_CODE.ERROR_511_DATA_NOT_FOUND, Msg = Resource.Company + " " + Resource.Not_Found_Msg, Field = Resource.Employee };

                  var currEmp = db.Employee_Profile.Where(w => w.Employee_Profile_ID == pEmp.Employee_Profile_ID).FirstOrDefault();
                  if (currEmp == null)
                      return new ServiceResult { Code = ERROR_CODE.ERROR_511_DATA_NOT_FOUND, Msg = Resource.Employee + " " + Resource.Not_Found_Msg, Field = Resource.Employee };

                  var pattern = db.Employee_No_Pattern.Where(w => w.Company_ID == currProfile.Company_ID).FirstOrDefault();
                  if (pattern == null)
                      return new ServiceResult { Code = ERROR_CODE.ERROR_511_DATA_NOT_FOUND, Msg = Resource.Employee_Pattern + " " + Resource.Not_Found_Msg, Field = Resource.Employee_Pattern };

                  if (!string.IsNullOrEmpty(pEmp.Employee_No))
                  {
                      pEmp.Employee_No = RegenEmployeeNo(pattern, currentdate, currEmp.Employee_Profile_ID, pEmp.Employee_No, pEmp.Nationality_ID);
                  }
                  else if (string.IsNullOrEmpty(pEmp.Employee_No))
                  {
                      pEmp.Employee_No = GetEmployeeNo(pattern, currentdate, null, pEmp.Nationality_ID);
                  }

                  if (pEmp.Employment_History != null)
                  {
                      foreach (var row in pEmp.Employment_History)
                      {
                          if (row.History_ID == 0 || !currEmp.Employment_History.Select(s => s.History_ID).Contains(row.History_ID))
                          {
                              // add
                              row.Employee_Profile_ID = pEmp.Employee_Profile_ID;
                              db.Employment_History.Add(row);
                          }
                          else
                          {
                              var currHist = db.Employment_History.Where(w => w.History_ID == row.History_ID).FirstOrDefault();
                              if (currHist != null)
                              {
                                  currHist.Branch_ID = row.Branch_ID;
                                  currHist.Department_ID = row.Department_ID;
                                  currHist.Designation_ID = row.Designation_ID;

                           currHist.Confirm_Date = row.Confirm_Date;
                                  currHist.Effective_Date = row.Effective_Date;
                           currHist.Terminate_Date = row.Terminate_Date;

                                  currHist.Employee_Type = row.Employee_Type;

                           currHist.Currency_ID = row.Currency_ID;
                           currHist.Basic_Salary = EncryptUtil.Encrypt(row.Basic_Salary);
                           currHist.Basic_Salary_Unit = row.Basic_Salary_Unit;

                           currHist.Days = row.Days;

                                  currHist.Update_On = row.Update_On;
                                  currHist.Update_By = row.Update_By;
                              }
                          }
                      }
                  }                  
                  db.Entry(currEmp).CurrentValues.SetValues(pEmp);                  
                  db.SaveChanges();
                  return new ServiceResult() { Code = ERROR_CODE.SUCCESS, Msg = new Success().getSuccess(ERROR_CODE.SUCCESS_EDIT), Field = Resource.Employee };
              }
          }
          catch
          {
              return new ServiceResult() { Code = ERROR_CODE.ERROR_504_UPDATE_ERROR, Msg = new Error().getError(ERROR_CODE.ERROR_504_UPDATE_ERROR), Field = Resource.Employee };
          }
      }

      public ServiceResult DeleteEmployee(Nullable<int> pEmpID)
      {
         try
         {
            using (var db = new SBS2DBContext())
            {
               var emp = db.Employee_Profile.Where(w => w.Employee_Profile_ID == pEmpID).FirstOrDefault();
               if (emp != null)
               {
                  var user = db.User_Profile.Where(w => w.Profile_ID == emp.Profile_ID).FirstOrDefault();
                  if (user != null)
                  {
                     user.User_Status = RecordStatus.Inactive;
                  }

                  emp.Emp_Status = RecordStatus.Inactive;
                  db.SaveChanges();
               }
               return new ServiceResult() { Code = ERROR_CODE.SUCCESS, Msg = new Success().getSuccess(ERROR_CODE.SUCCESS_DELETE), Field = Resource.Employee };
            }
         }
         catch
         {
            return new ServiceResult() { Code = ERROR_CODE.ERROR_505_DELETE_ERROR, Msg = new Error().getError(ERROR_CODE.ERROR_505_DELETE_ERROR), Field = Resource.Employee };
         }
      }

      public int DeleteAttachment(Nullable<System.Guid> pAttachmentID)
      {
         try
         {
            using (var db = new SBS2DBContext())
            {
               var attach = db.Employee_Attachment.Where(w => w.Attachment_ID == pAttachmentID).FirstOrDefault();
               if (attach == null)
               {
                  return -500;
               }
               db.Employee_Attachment.Remove(attach);

               db.SaveChanges();
               return 1;
            }
         }
         catch
         {
            //Log
            return -500;
         }
      }

      public int InsertAttachment(Employee_Attachment pAttach)
      {
         try
         {
            using (var db = new SBS2DBContext())
            {
               db.Employee_Attachment.Add(pAttach);
               db.SaveChanges();

               return 1;
            }
         }
         catch
         {
            //Log
            return -500;
         }
      }

      public ServiceResult UpdateEmpStatus(int[] pEmp, string pStatus, string pUpdateBy)
      {
         try
         {
            var currentdate = StoredProcedure.GetCurrentDate();
            using (var db = new SBS2DBContext())
            {
               var emps = db.Employee_Profile.Where(w => pEmp.Contains(w.Employee_Profile_ID));
               foreach (var emp in emps)
               {
                  if (emp != null)
                  {
                     emp.User_Profile.User_Status = pStatus;
                     emp.User_Profile.Update_By = pUpdateBy;
                     emp.User_Profile.Update_On = currentdate;
                  }
               }
               db.SaveChanges();

               return new ServiceResult() { Code = ERROR_CODE.SUCCESS, Msg = new Success().getSuccess(ERROR_CODE.SUCCESS_EDIT), Field = Resource.Employee };
            }
         }
         catch
         {
            //Log
            return new ServiceResult() { Code = ERROR_CODE.ERROR_504_UPDATE_ERROR, Msg = new Error().getError(ERROR_CODE.ERROR_504_UPDATE_ERROR), Field = Resource.Employee };
         }
      }

      //Added by Nay on 06-Jul-2015, by Ref existing SBS Inventory
      public Employee_Profile GetEmployeeProfileByUserProfile(Nullable<int> pProfileID)
      {
         if (pProfileID.HasValue)
         {
            using (var db = new SBS2DBContext())
            {
               return db.Employee_Profile
                    .Include(i => i.User_Profile)
                    .Include(i => i.Nationality)
                    .Where(i => i.Profile_ID == pProfileID)
                    .FirstOrDefault();

            }
         }
         return null;
      }

      public List<User_Assign_Module> LstUserAssignModule(Nullable<int> pProfileID)
      {

         using (var db = new SBS2DBContext())
         {
            return db.User_Assign_Module
                .Where(w => w.Profile_ID == pProfileID)
                .OrderBy(o => o.Subscription_ID)
                .ToList();
         }
      }

      //Add By sun 31-08-2015
      public ServiceResult InsertUserPageRole(Page_Role pProle)
      {
         try
         {
            using (var db = new SBS2DBContext())
            {

               return new ServiceResult() { Code = ERROR_CODE.SUCCESS, Msg = new Success().getSuccess(ERROR_CODE.SUCCESS_CREATE), Field = Resource.System_Configuration };
            }
         }
         catch
         {
            //Log
            return new ServiceResult() { Code = ERROR_CODE.ERROR_504_UPDATE_ERROR, Msg = new Error().getError(ERROR_CODE.ERROR_504_UPDATE_ERROR), Field = Resource.System_Configuration };
         }
      }

      //Added by sun 14-10-2015
      //Update Status function delete
      public ServiceResult UpdateDeleteEmployeeStatus(Nullable<int> pEmpID, string pStatus, string pUpdateBy)
      {
         try
         {
            var currentdate = StoredProcedure.GetCurrentDate();
            using (var db = new SBS2DBContext())
            {
               var emp = db.Employee_Profile.Where(w => w.Employee_Profile_ID == pEmpID).FirstOrDefault();
               if (emp != null)
               {
                  var user = db.User_Profile.Where(w => w.Profile_ID == emp.Profile_ID).FirstOrDefault();
                  if (user != null)
                  {
                     var photo = db.User_Profile_Photo.Where(w => w.Profile_ID == user.Profile_ID);
                     if (photo != null)
                     {
                        foreach (var row in photo)
                        {
                           row.Update_On = currentdate;
                           row.Update_By = pUpdateBy;
                           //row.Record_Status = pStatus;
                        }
                     }

                     var role = db.User_Assign_Role.Where(w => w.User_Authentication_ID == user.User_Authentication_ID);
                     if (role != null)
                     {
                        foreach (var row in role)
                        {
                           row.Update_On = currentdate;
                           row.Update_By = pUpdateBy;
                           //row.Record_Status = pStatus;
                        }
                     }

                     var alink = db.Activation_Link.Where(w => w.User_Authentication_ID == user.User_Authentication_ID);
                     if (alink != null)
                     {
                        foreach (var row in alink)
                        {
                           row.Update_On = currentdate;
                           row.Update_By = pUpdateBy;
                           //row.Record_Status = pStatus;
                        }
                     }

                     //var aspUser = db.Users.Where(w => w.Id.Equals(user.User_Authentication.ApplicationUser_Id)).FirstOrDefault();
                     //db.Users.Remove(aspUser);

                     var authen = db.User_Authentication.Where(w => w.User_Authentication_ID == user.User_Authentication_ID).FirstOrDefault();
                     if (authen != null)
                     {
                        authen.Update_On = currentdate;
                        authen.Update_By = pUpdateBy;
                        //authen.Record_Status = pStatus;                    
                     }

                     //var ir = db.IRs.Where(w => w.Profile_ID == user.Profile_ID);
                     //if (ir != null)
                     //{
                     //    foreach (var row in ir)
                     //    {
                     //        row.Update_On = currentdate;
                     //        row.Update_By = pUpdateBy;
                     //        //row.Record_Status = pStatus;
                     //    }
                     //}

                     //var ia = db.IAs.Where(w => w.Profile_ID == user.Profile_ID);
                     //if (ia != null)
                     //{
                     //    foreach (var row in ia)
                     //    {
                     //        row.Update_On = currentdate;
                     //        row.Update_By = pUpdateBy;
                     //        //row.Record_Status = pStatus;
                     //    }
                     //}

                     user.Update_On = currentdate;
                     user.Update_By = pUpdateBy;
                     user.User_Status = pStatus;
                  }

                  var contact = db.Employee_Emergency_Contact.Where(w => w.Employee_Profile_ID == emp.Employee_Profile_ID);
                  if (contact != null)
                  {
                     foreach (var row in contact)
                     {
                        row.Update_On = currentdate;
                        row.Update_By = pUpdateBy;
                        //row.Record_Status = pStatus;
                     }
                  }

                  var bank = db.Banking_Info.Where(w => w.Employee_Profile_ID == emp.Employee_Profile_ID);
                  if (bank != null)
                  {
                     foreach (var row in bank)
                     {
                        row.Update_On = currentdate;
                        row.Update_By = pUpdateBy;
                        //row.Record_Status = pStatus;
                     }
                  }

                  var rs = db.Relationships.Where(w => w.Employee_Profile_ID == emp.Employee_Profile_ID);
                  if (rs != null)
                  {
                     foreach (var row in rs)
                     {
                        row.Update_On = currentdate;
                        row.Update_By = pUpdateBy;
                        //row.Record_Status = pStatus;
                     }
                  }

                  //var empHistAllowance = db.Employment_History_Allowance.Where(w => w.Employment_History.Employee_Profile_ID == emp.Employee_Profile_ID);
                  //db.Employment_History_Allowance.RemoveRange(empHistAllowance);

                  var empHist = db.Employment_History.Where(w => w.Employee_Profile_ID == emp.Employee_Profile_ID);
                  if (empHist != null)
                  {
                     foreach (var row in empHist)
                     {
                        row.Update_On = currentdate;
                        row.Update_By = pUpdateBy;
                        row.Record_Status = pStatus;
                     }
                  }

                  //var empAtt = db.Employee_Attachment.Where(w => w.Employee_Profile_ID == emp.Employee_Profile_ID);
                  //db.Employee_Attachment.RemoveRange(empAtt);

                  emp.Update_By = pUpdateBy;
                  emp.Update_On = currentdate;
                  //emp.Record_Status = pStatus;
                  db.SaveChanges();
               }

               return new ServiceResult() { Code = ERROR_CODE.SUCCESS, Msg = new Success().getSuccess(ERROR_CODE.SUCCESS_DELETE), Field = Resource.Employee };
            }
         }
         catch
         {
            return new ServiceResult() { Code = ERROR_CODE.ERROR_505_DELETE_ERROR, Msg = new Error().getError(ERROR_CODE.ERROR_505_DELETE_ERROR), Field = Resource.Employee };
         }
      }

      //Added by sun 29-01-2016
      public ServiceResult ImportEmployee(ImportEmployeeProfile_[] emps, ImportHistory_[] his, ImportEmergencyContact_[] emergency, ImportRelationship_[] relation)
      {
         DateTime currentdate = StoredProcedure.GetCurrentDate();
         using (var db = new SBS2DBContext())
         {
            try
            {
               foreach (var emp in emps)
               {

                  if (db.Users.Where(w => w.UserName.ToLower() == emp.Email_Address.ToLower()).FirstOrDefault() != null)
                     return new ServiceResult { Code = ERROR_CODE.ERROR_510_DATA_DUPLICATE, Msg = Resource.Email + " " + Resource.Is_Duplicated_Lower, Field = Resource.Employee };

                  var com = (from a in db.Company_Details
                             where (a.Effective_Date <= currentdate & a.Company_ID == emp.Company_ID)
                             orderby a.Effective_Date descending
                             select a).FirstOrDefault();

                  if (com == null)
                     return new ServiceResult { Code = ERROR_CODE.ERROR_511_DATA_NOT_FOUND, Msg = Resource.Company + " " + Resource.Not_Found_Msg, Field = Resource.Employee };

                  var pattern = (from a in db.Employee_No_Pattern where a.Company_ID == emp.Company_ID.Value select a).SingleOrDefault();
                  if (pattern == null)
                     return new ServiceResult { Code = ERROR_CODE.ERROR_511_DATA_NOT_FOUND, Msg = Resource.Pattern + " " + Resource.Not_Found_Msg, Field = Resource.Employee };

                  if (db.Users.Where(w => w.UserName.ToLower() == emp.Email_Address.ToLower()).FirstOrDefault() != null)
                     return new ServiceResult { Code = ERROR_CODE.ERROR_510_DATA_DUPLICATE, Msg = Resource.Email + " " + Resource.Is_Duplicated_Lower, Field = Resource.Employee };

                  if (!string.IsNullOrEmpty(emp.Employee_No))
                  {

                  }
                  else
                  {

                     emp.Employee_No = GetEmployeeNo(pattern, currentdate, null, emp.Nationality_ID);
                     var number = emp.Employee_No.Substring(emp.Employee_No.Length - 6, 6);
                     while (db.Employee_Profile.Where(w => w.User_Profile.Company_ID == com.Company_ID && w.Employee_No.Substring(w.Employee_No.Length - 6, 6) == number).FirstOrDefault() != null)
                     {
                        pattern.Current_Running_Number = pattern.Current_Running_Number + 1;
                        emp.Employee_No = GetEmployeeNo(pattern, currentdate, null, emp.Nationality_ID);
                        number = emp.Employee_No.Substring(emp.Employee_No.Length - 6, 6);
                     }
                  }

                  var guid = Guid.NewGuid().ToString();
                  while (db.Users.Where(w => w.Id == guid).FirstOrDefault() != null)
                  {
                     guid = Guid.NewGuid().ToString();
                  }

                  db.Users.Add(new ApplicationUser() { Id = guid, UserName = emp.Email_Address.ToLower() });

                  var ApplicationUser_Id = guid;

                  User_Authentication authen = new User_Authentication()
                  {
                     Email_Address = emp.Email_Address,
                     Company_ID = com.Company_ID,
                     ApplicationUser_Id = ApplicationUser_Id,
                     Create_By = emp.Create_By,
                     Create_On = emp.Create_On,
                     Update_By = emp.Create_By,
                     Update_On = emp.Create_On
                  };

                  User_Profile user = new User_Profile()
                  {
                     First_Name = emp.First_Name,
                     Middle_Name = emp.Middle_Name,
                     Last_Name = emp.Last_Name,
                     User_Status = RecordStatus.Active,
                     Registration_Date = currentdate,
                     Company_ID = com.Company_ID,
                     Phone = emp.Mobile_No,
                     User_Authentication = authen,
                     Email = emp.Email_Address,
                     Create_By = emp.Create_By,
                     Create_On = emp.Create_On,
                     Update_By = emp.Create_By,
                     Update_On = emp.Create_On
                  };

                  //GENERATE ACTIVATION CODE
                  String code;
                  do
                  {
                     code = "A" + randomString(40);
                  }
                  while (!validateActivationCode(db, code));

                  //SET User_Assign_Role to ROLE_CUSTOMER_USER
                  User_Assign_Role role = new User_Assign_Role()
                  {
                     User_Role_ID = UserRole.ROLE_CUSTOMER_USER,
                     Create_By = emp.Create_By,
                     Create_On = emp.Create_On,
                     Update_By = emp.Create_By,
                     Update_On = emp.Create_On
                  };

                  authen.User_Assign_Role.Add(role);

                  Activation_Link activation_link = new Activation_Link()
                  {
                     Activation_Code = code,
                     //SET Time_Limit to activate within LINK_TIME_LIMIT
                     Time_Limit = currentdate.AddHours(LINK_TIME_LIMIT),

                     Create_By = emp.Create_By,
                     Create_On = emp.Create_On,
                     Update_By = emp.Create_By,
                     Update_On = emp.Create_On
                  };

                  authen.Activation_Link.Add(activation_link);

                  var hireddate = DateUtil.ToDate(emp.Date_Of_Issue);
                  if (string.IsNullOrEmpty(emp.Employee_No))
                     return new ServiceResult { Code = ERROR_CODE.ERROR_511_DATA_NOT_FOUND, Msg = Resource.Employee_No + " " + Resource.Not_Found_Msg, Field = Resource.Employee };

                  //emp.Employee_No = GetEmployeeNo(pattern, currentdate, hireddate, emp.Nationality_ID);

                  Employee_Profile e = new Employee_Profile()
                  {
                     Employee_No = emp.Employee_No,
                     Gender = emp.Gender,
                     Marital_Status = emp.Marital_Status,
                     DOB = DateUtil.ToDate(emp.DOB),
                     Religion = emp.Religion,
                     Race = emp.Race,
                     Nationality_ID = emp.Nationality_ID,
                     Residential_Status = emp.Residential_Status,
                     NRIC = emp.NRIC,
                     Passport = emp.Passport,
                     Hired_Date = DateUtil.ToDate(emp.Date_Of_Issue),
                     Expiry_Date = DateUtil.ToDate(emp.Date_Of_Expire),
                     Residential_Address_1 = emp.Address1,
                     Postal_Code_1 = emp.Address1_Postal_Code,
                     Residential_Country_1 = emp.Address1_Country,
                     Residential_Address_2 = emp.Address2,
                     Postal_Code_2 = emp.Address2_Postal_Code,
                     Residential_Country_2 = emp.Address2_Country,
                     Create_By = emp.Create_By,
                     Create_On = emp.Create_On,
                     Update_By = emp.Create_By,
                     Update_On = emp.Create_On
                  };

                  Banking_Info b = new Banking_Info()
                  {
                     Bank_Name = emp.Bank_Name_,
                     Bank_Account = emp.Bank_Account,
                     Payment_Type = emp.Payment_Type,
                     Effective_Date = DateUtil.ToDate(emp.Effective_Date),
                     Create_By = emp.Create_By,
                     Create_On = emp.Create_On,
                     Update_By = emp.Create_By,
                     Update_On = emp.Create_On
                  };
                  e.Banking_Info.Add(b);

                  if (his != null)
                  {
                     var his_emp_no = his.Where(w => w.Employee_No == emp.Employee_No);
                     if (his_emp_no != null)
                     {
                        foreach (var row in his_emp_no)
                        {
                           if (row.Validate_His)
                           {
                              var hist = new Employment_History()
                             {
                                Employee_Type = row.Employee_Type,
                                Department_ID = row.Department_ID,
                                Designation_ID = row.Designation_ID,
                                Branch_ID = row.Branch_ID,
                                Effective_Date = DateUtil.ToDate(row.Effective_Date),
                                Confirm_Date = DateUtil.ToDate(row.Confirm_Date),
                                Currency_ID = row.Currency_ID,
                                Basic_Salary = EncryptUtil.Encrypt(row.Basic_Salary),
                                Basic_Salary_Unit = row.Basic_Salary_Unit
                             };
                              e.Employment_History.Add(hist);
                           }
                        }
                     }
                  }

                  if (emergency != null)
                  {
                     var emergency_emp_no = emergency.Where(w => w.Employee_No == emp.Employee_No);
                     if (emergency_emp_no != null)
                     {
                        foreach (var row in emergency_emp_no)
                        {
                           if (row.Validate_Emergency)
                           {
                              var emer = new Employee_Emergency_Contact()
                              {
                                 Contact_No = row.Contact_No,
                                 Name = row.Name,
                                 Relationship = (row.Relationship_ID.HasValue ? row.Relationship_ID.Value : 0),
                                 Create_By = emp.Create_By,
                                 Create_On = emp.Create_On,
                                 Update_By = emp.Create_By,
                                 Update_On = emp.Create_On
                              };
                              e.Employee_Emergency_Contact.Add(emer);
                           }
                        }
                     }
                  }

                  if (relation != null)
                  {
                     var relation_emp_no = relation.Where(w => w.Employee_No == emp.Employee_No);
                     if (relation_emp_no != null)
                     {
                        foreach (var row in relation_emp_no)
                        {
                           if (row.Validate_Relation)
                           {
                              var relationship = new Relationship()
                              {
                                 Name = row.Name,
                                 Relationship1 = (row.Relationship_ID.HasValue ? row.Relationship_ID.Value : 0),
                                 DOB = DateUtil.ToDate(row.DOB),
                                 Nationality_ID = row.Nationality_ID,
                                 NRIC = row.NRIC,
                                 Create_By = emp.Create_By,
                                 Create_On = emp.Create_On,
                                 Update_By = emp.Create_By,
                                 Update_On = emp.Create_On
                              };
                              e.Relationships.Add(relationship);
                           }
                        }
                     }
                  }


                  user.Employee_Profile.Add(e);
                  db.User_Profile.Add(user);

               }

               db.SaveChanges();
            }
            catch (Exception ex)
            {
               return new ServiceResult() { Code = ERROR_CODE.ERROR_503_INSERT_ERROR, Msg = new Error().getError(ERROR_CODE.ERROR_503_INSERT_ERROR), Field = Resource.Employee };
            }
         }
         return new ServiceResult() { Code = ERROR_CODE.SUCCESS, Msg = new Success().getSuccess(ERROR_CODE.SUCCESS_CREATE), Field = Resource.Employee };

      }

   }

   public class EmploymentHistoryService
   {
      public List<Employment_History> LstEmploymentHistory(Nullable<int> pEmpID)
      {
         if (pEmpID.HasValue)
         {
            var db = new SBS2DBContext();
            return (from a in db.Employment_History
                    where a.Employee_Profile_ID == pEmpID.Value
                    orderby a.Effective_Date descending
                    select a).ToList();
         }
         return null;
      }

      public List<Employment_History> LstEmploymentHistoryAll(Nullable<int> pEmpID)
      {
         var currentdate = StoredProcedure.GetCurrentDate();
         using (var db = new SBS2DBContext())
         {
            return db.Employment_History
                  .Include(i => i.Department)
                  .Include(i => i.Designation)
                  .Include(i => i.Employee_Profile)
                  .Where(w => w.Employee_Profile_ID == pEmpID & w.Effective_Date <= currentdate)
                  .OrderBy(o => o.History_ID)
                  .ToList();
         }
      }

      public Employment_History GetFirstEmploymentHistory(Nullable<int> pEmpID)
      {

         var currentdate = StoredProcedure.GetCurrentDate();
         using (var db = new SBS2DBContext())
         {
            return db.Employment_History
                  .Include(i => i.Department)
                  .Include(i => i.Designation)
                  .Include(i => i.Employee_Profile)
                  .Where(w => w.Employee_Profile_ID == pEmpID)
                  .OrderBy(o => o.Effective_Date)
                  .FirstOrDefault();
         }
      }

      public Employment_History GetCurrentEmploymentHistory(Nullable<int> pEmpID)
      {

         var currentdate = StoredProcedure.GetCurrentDate();
         using (var db = new SBS2DBContext())
         {
            return db.Employment_History
                  .Include(i => i.Department)
                  .Include(i => i.Designation)
                  .Include(i => i.Employee_Profile)
                  .Where(w => w.Employee_Profile_ID == pEmpID & w.Effective_Date <= currentdate)
                  .OrderByDescending(o => o.Effective_Date)
                  .FirstOrDefault();
         }
      }

      public Employment_History GetCurrentEmploymentHistoryByProfile(Nullable<int> pProfileID)
      {

         var currentdate = StoredProcedure.GetCurrentDate();
         using (var db = new SBS2DBContext())
         {
            return db.Employment_History
                  .Include(i => i.Department)
                  .Include(i => i.Designation)
                  .Include(i => i.Employee_Profile)
                  .Include(i => i.Employee_Profile.User_Profile)
                  .Where(w => w.Employee_Profile.Profile_ID == pProfileID & w.Effective_Date <= currentdate)
                  .OrderByDescending(o => o.Effective_Date)
                  .FirstOrDefault();
         }
      }

      public Employment_History GetEmploymentHistory(Nullable<int> pHistoryID)
      {
         if (pHistoryID.HasValue)
         {
            using (var db = new SBS2DBContext())
            {
               return db.Employment_History
                     .Include(i => i.Department)
                     .Include(i => i.Designation)
                     .Include(i => i.Employee_Profile)
                     .Include(i => i.Currency)
                     .Where(w => w.History_ID == pHistoryID)
                     .FirstOrDefault();
            }

         }
         return null;
      }

      public Employment_History GetEmploymentHistory(int? pEmpID, DateTime? pEffectiveDate)
      {

         using (var db = new SBS2DBContext())
         {
            return db.Employment_History
                  .Include(i => i.Department)
                  .Include(i => i.Designation)
                  .Include(i => i.Employee_Profile)
                  .Where(w => w.Employee_Profile_ID == pEmpID & w.Effective_Date <= pEffectiveDate)
                  .OrderByDescending(o => o.Effective_Date)
                  .FirstOrDefault();
         }
      }

      public Employment_History GetPayrollEmploymentHistory(int pEmpID, DateTime pEffectiveDate)
      {

         using (var db = new SBS2DBContext())
         {
            return db.Employment_History
                  .Include(i => i.Department)
                  .Include(i => i.Designation)
                  .Include(i => i.Employee_Profile)
                  .Where(w => w.Employee_Profile_ID == pEmpID & (w.Effective_Date <= pEffectiveDate || (w.Effective_Date.Value.Month == pEffectiveDate.Month && w.Effective_Date.Value.Year == pEffectiveDate.Year)))
                  .OrderByDescending(o => o.Effective_Date)
                  .FirstOrDefault();
         }
      }

      public ServiceResult InsertEmploymentHistory(Employment_History pEmpHist, int pCompanyID)
      {
         try
         {
            using (var db = new SBS2DBContext())
            {
               var currentdate = StoredProcedure.GetCurrentDate();
               if (!pEmpHist.Branch_ID.HasValue && !string.IsNullOrEmpty(pEmpHist.Other_Branch))
               {
                  var bService = new BranchService();
                  var branch = bService.GetBranch(pCompanyID, pEmpHist.Other_Branch.Trim());
                  if (branch == null)
                  {
                     var b = new Branch()
                     {
                        Company_ID = pCompanyID,
                        Branch_Code = pEmpHist.Other_Branch,
                        Branch_Name = pEmpHist.Other_Branch,
                        Create_On = currentdate,
                        Create_By = "System",
                        Update_On = currentdate,
                        Update_By = "System",
                        Record_Status = Resource.Active
                     };
                     db.Branches.Add(b);
                     pEmpHist.Branch_ID = b.Branch_ID;
                  }
                  else
                     pEmpHist.Branch_ID = branch.Branch_ID;
               }

               if (!pEmpHist.Department_ID.HasValue && !string.IsNullOrEmpty(pEmpHist.Other_Department))
               {
                  var dpService = new DepartmentService();
                  var department = dpService.GetDepartment(pCompanyID, pEmpHist.Other_Department.Trim());
                  if (department == null)
                  {
                     //insert
                     var dp = new Department()
                     {
                        Name = pEmpHist.Other_Department,
                        Record_Status = Resource.Active,
                        Company_ID = pCompanyID,
                        Create_On = currentdate,
                        Create_By = "System",
                        Update_On = currentdate,
                        Update_By = "System",
                     };
                     db.Departments.Add(dp);
                     pEmpHist.Department_ID = dp.Department_ID;
                  }
                  else
                     pEmpHist.Department_ID = department.Department_ID;
               }

               if (!pEmpHist.Designation_ID.HasValue && !string.IsNullOrEmpty(pEmpHist.Other_Designation))
               {
                  var dService = new DesignationService();
                  var designation = dService.GetDesignation(pCompanyID, pEmpHist.Other_Designation.Trim());
                  if (designation == null)
                  {
                     var ds = new Designation()
                     {
                        Name = pEmpHist.Other_Designation,
                        Record_Status = Resource.Active,
                        Company_ID = pCompanyID,
                        Create_By = "Auto",
                        Create_On = currentdate,
                     };
                     db.Designations.Add(ds);
                     pEmpHist.Designation_ID = ds.Designation_ID;
                  }
                  else
                     pEmpHist.Designation_ID = designation.Designation_ID;
               }

               pEmpHist.Other_Designation = null;
               pEmpHist.Other_Department = null;
               pEmpHist.Other_Branch = null;

               db.Employment_History.Add(pEmpHist);
               db.SaveChanges();
               db.Entry(pEmpHist).GetDatabaseValues();
               return new ServiceResult { Code = ERROR_CODE.SUCCESS, Msg = new Success().getSuccess(ERROR_CODE.SUCCESS_CREATE), Field = Resource.Employee_History };
            }
         }
         catch
         {
            return new ServiceResult { Code = ERROR_CODE.ERROR_503_INSERT_ERROR, Msg = new Error().getError(ERROR_CODE.ERROR_503_INSERT_ERROR), Field = Resource.Employee_History };
         }
      }

      public ServiceResult UpdateEmploymentHistory(Employment_History pEmpHist, int pCompanyID)
      {
         try
         {
            using (var db = new SBS2DBContext())
            {
               var currentdate = StoredProcedure.GetCurrentDate();
               //Update
               var current = db.Employment_History.Where(w => w.History_ID == pEmpHist.History_ID).FirstOrDefault();
               if (current != null)
               {
                  if (!pEmpHist.Branch_ID.HasValue && !string.IsNullOrEmpty(pEmpHist.Other_Branch))
                  {
                     var bService = new BranchService();
                     var branch = bService.GetBranch(pCompanyID, pEmpHist.Other_Branch.Trim());
                     if (branch == null)
                     {
                        var b = new Branch()
                        {
                           Company_ID = pCompanyID,
                           Branch_Code = pEmpHist.Other_Branch,
                           Branch_Name = pEmpHist.Other_Branch,
                           Create_On = currentdate,
                           Create_By = "System",
                           Update_On = currentdate,
                           Update_By = "System",
                           Record_Status = Resource.Active
                        };
                        db.Branches.Add(b);
                        pEmpHist.Branch_ID = b.Branch_ID;
                     }
                     else
                        pEmpHist.Branch_ID = branch.Branch_ID;
                  }

                  if (!pEmpHist.Department_ID.HasValue && !string.IsNullOrEmpty(pEmpHist.Other_Department))
                  {
                     var dpService = new DepartmentService();
                     var department = dpService.GetDepartment(pCompanyID, pEmpHist.Other_Department.Trim());
                     if (department == null)
                     {
                        //insert
                        var dp = new Department()
                        {
                           Name = pEmpHist.Other_Department,
                           Record_Status = Resource.Active,
                           Company_ID = pCompanyID,
                           Create_On = currentdate,
                           Create_By = "System",
                           Update_On = currentdate,
                           Update_By = "System",
                        };
                        db.Departments.Add(dp);
                        pEmpHist.Department_ID = dp.Department_ID;
                     }
                     else
                        pEmpHist.Department_ID = department.Department_ID;
                  }

                  if (!pEmpHist.Designation_ID.HasValue && !string.IsNullOrEmpty(pEmpHist.Other_Designation))
                  {
                     var dService = new DesignationService();
                     var designation = dService.GetDesignation(pCompanyID, pEmpHist.Other_Designation.Trim());
                     if (designation == null)
                     {
                        var ds = new Designation()
                        {
                           Name = pEmpHist.Other_Designation,
                           Record_Status = Resource.Active,
                           Company_ID = pCompanyID,
                           Create_On = currentdate,
                           Create_By = "System",
                           Update_On = currentdate,
                           Update_By = "System",
                        };
                        db.Designations.Add(ds);
                        pEmpHist.Designation_ID = ds.Designation_ID;
                     }
                     else
                        pEmpHist.Designation_ID = designation.Designation_ID;
                  }

                  if (pEmpHist.Employment_History_Allowance == null)
                     db.Employment_History_Allowance.RemoveRange(current.Employment_History_Allowance);
                  else
                  {
                     var Allowances = current.Employment_History_Allowance.Select(s => s.Employment_History_Allowance_ID).ToArray();
                     foreach (var row in Allowances)
                     {
                        if (!pEmpHist.Employment_History_Allowance.Select(s => s.Employment_History_Allowance_ID).Contains(row))
                        {
                           var currAll = db.Employment_History_Allowance.Where(w => w.Employment_History_Allowance_ID == row).FirstOrDefault();
                           if (currAll != null)
                              db.Employment_History_Allowance.Remove(currAll);
                        }
                     }

                     foreach (var row in pEmpHist.Employment_History_Allowance)
                     {
                        if (row.Employment_History_Allowance_ID == 0)
                           db.Employment_History_Allowance.Add(row);
                        else
                        {
                           var currA = db.Employment_History_Allowance.Where(w => w.Employment_History_Allowance_ID == row.Employment_History_Allowance_ID).FirstOrDefault();
                           if (currA != null)
                              db.Entry(currA).CurrentValues.SetValues(row);
                        }
                     }
                  }
                  pEmpHist.Other_Designation = null;
                  pEmpHist.Other_Department = null;
                  pEmpHist.Other_Branch = null;

                  db.Entry(current).CurrentValues.SetValues(pEmpHist);
                  db.SaveChanges();
               }
               return new ServiceResult { Code = ERROR_CODE.SUCCESS, Msg = new Success().getSuccess(ERROR_CODE.SUCCESS_EDIT), Field = Resource.Employee_History };
            }
         }
         catch
         {
            return new ServiceResult { Code = ERROR_CODE.ERROR_504_UPDATE_ERROR, Msg = new Error().getError(ERROR_CODE.ERROR_504_UPDATE_ERROR), Field = Resource.Employee_History };
         }
      }

      public ServiceResult DeleteEmploymentHistory(Nullable<int> pHistID)
      {
         try
         {
            using (var db = new SBS2DBContext())
            {
               //Insert
               var current = db.Employment_History.Where(w => w.History_ID == pHistID).FirstOrDefault();
               if (current != null)
               {
                  db.Employment_History_Allowance.RemoveRange(db.Employment_History_Allowance);
                  db.Employment_History.Remove(current);
               }
               db.SaveChanges();
               return new ServiceResult { Code = ERROR_CODE.SUCCESS, Msg = new Success().getSuccess(ERROR_CODE.SUCCESS_DELETE), Field = Resource.Employee_History };
            }
         }
         catch
         {
            return new ServiceResult { Code = ERROR_CODE.ERROR_505_DELETE_ERROR, Msg = new Error().getError(ERROR_CODE.ERROR_505_DELETE_ERROR), Field = Resource.Employee_History };
         }
      }

   }

   public class BankDetailService
   {

      public List<Bank_Details> LstBanKDetail(Nullable<int> pCompanyID)
      {
         if (pCompanyID.HasValue)
         {
            var db = new SBS2DBContext();

            var q = (from a in db.Bank_Details
                     where a.Company_ID == pCompanyID.Value
                     orderby a.Company_ID, a.Bank_Account_Number
                     select a);

            return q.ToList();
         }
         return null;
      }

      public Bank_Details GetBanKDetail(Nullable<int> pBankDetailID)
      {
         if (pBankDetailID.HasValue)
         {
            var db = new SBS2DBContext();

            var q = (from a in db.Bank_Details
                     where a.Bank_Detail_ID == pBankDetailID
                     select a).FirstOrDefault();

            return q;
         }
         return null;
      }

      public bool UpdateBankDetail(Bank_Details pBankDetail)
      {
         try
         {
            using (var db = new SBS2DBContext())
            {
               if (pBankDetail != null && pBankDetail.Company_ID > 0 && pBankDetail.Bank_Detail_ID > 0)
               {
                  var current = (from a in db.Bank_Details
                                 where a.Bank_Detail_ID == pBankDetail.Bank_Detail_ID
                                 select a).FirstOrDefault();

                  if (current != null)
                  {
                     //Update
                     db.Entry(current).CurrentValues.SetValues(pBankDetail);
                  }
                  db.SaveChanges();
               }
               return true;
            }
         }
         catch
         {
            return false;
         }

      }

      public bool InsertBankDetail(Bank_Details pBankDetail)
      {
         try
         {
            using (var db = new SBS2DBContext())
            {
               if (pBankDetail != null)
               {
                  //Insert
                  db.Bank_Details.Add(pBankDetail);
                  db.SaveChanges();
               }
               return true;
            }
         }
         catch
         {
            return false;
         }

      }

      public bool DeleteBankDetail(int pBankDetailID)
      {
         try
         {
            using (var db = new SBS2DBContext())
            {
               if (pBankDetailID > 0)
               {
                  var current = (from a in db.Bank_Details where a.Bank_Detail_ID == pBankDetailID select a).FirstOrDefault();
                  db.Bank_Details.Remove(current);
                  db.SaveChanges();
               }
               return true;
            }
         }
         catch
         {
            return false;
         }

      }
   }
}
