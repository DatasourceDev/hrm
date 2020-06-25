using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using HR.Common;
using System.Data.Entity;
using HR.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Threading.Tasks;
using System.Data;
using SBSModel.Models;
using SBSModel.Common;

//Added By sun 24-08-2015
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Collections.Specialized;
using System.IO;
using System.Web.Mvc;
using SBSResourceAPI;


namespace HR.Models
{

   //public class EmployeeService
   //{
   //    public List<User_Role> LstUserRole()
   //    {
   //        var currentdate = StoredProcedure.GetCurrentDate();
   //        using (var db = new SBS2DBContext())
   //        {
   //            return db.User_Role.ToList();
   //        }
   //    }
   //    public List<User_Assign_Role> LstUserAssignRole(Nullable<int> pUserAuthenID)
   //    {
   //        var currentdate = StoredProcedure.GetCurrentDate();
   //        using (var db = new SBS2DBContext())
   //        {
   //            return db.User_Assign_Role.Where(w => w.User_Authentication_ID == pUserAuthenID).ToList();
   //        }
   //    }

   //    public List<Employee_Profile> LstEmployeeProfile(Nullable<int> pCompanyID, Nullable<int> pBranch = null, Nullable<int> pDepartmentID = null)
   //    {
   //        var currentdate = StoredProcedure.GetCurrentDate();
   //        using (var db = new SBS2DBContext())
   //        {
   //            var emps = db.Employee_Profile
   //                .Include(i => i.Employment_History)
   //                .Include(i => i.User_Profile)
   //                .Include(i => i.User_Profile.User_Authentication)
   //                .Include(i => i.Employment_History.Select(s => s.Branch))
   //                .Include(i => i.Employment_History.Select(s => s.Designation))
   //                .Include(i => i.Employment_History.Select(s => s.Department))
   //                .Where(w => w.User_Profile.Company_ID == pCompanyID);

   //            if (pBranch.HasValue)
   //            {
   //                emps = emps.Where(w => w.Employment_History.Where(we => we.Branch_ID == pBranch).FirstOrDefault() != null);
   //            }

   //            if (pDepartmentID.HasValue)
   //            {
   //                emps = emps.Where(w => w.Employment_History.Where(we => we.Department_ID == pDepartmentID).FirstOrDefault() != null);
   //            }

   //            return emps.ToList();
   //        }

   //    }

   //    public Employee_Profile GetEmployeeProfile(Nullable<int> pEmpID)
   //    {
   //        if (pEmpID.HasValue)
   //        {
   //            using (var db = new SBS2DBContext())
   //            {
   //                return db.Employee_Profile
   //                        .Include(i => i.Employment_History)
   //                        .Include(i => i.Employment_History.Select(s => s.Currency))
   //                        .Include(i => i.Employment_History.Select(s => s.Branch))
   //                        .Include(i => i.Employment_History.Select(s => s.Designation))
   //                        .Include(i => i.Employment_History.Select(s => s.Department))
   //                        .Include(i => i.Employment_History.Select(s => s.Employment_History_Allowance))
   //                        .Include(i => i.User_Profile)
   //                        .Include(i => i.User_Profile.User_Profile_Photo)
   //                        .Include(i => i.User_Profile.User_Authentication)
   //                        .Include(i => i.Nationality)
   //                        .Include(i => i.Banking_Info)
   //                        .Include(i => i.Employee_Emergency_Contact)
   //                        .Include(i => i.Relationships)
   //                        .Include(i => i.Relationships.Select(s => s.Nationality))
   //                        .Include(i => i.Relationships.Select(s => s.Global_Lookup_Data1))
   //                        .Include(i => i.Employee_Attachment)
   //                        .Include(i => i.Employee_Attachment.Select(s => s.Global_Lookup_Data))
   //                        .Where(i => i.Employee_Profile_ID == pEmpID)
   //                     .SingleOrDefault();

   //            }
   //        }
   //        return null;
   //    }

   //    public Employee_Profile GetEmployeeProfileByProfileID(Nullable<int> pProfileID)
   //    {
   //        using (var db = new SBS2DBContext())
   //        {
   //            return db.Employee_Profile
   //                  .Include(i => i.Employment_History)
   //                        .Include(i => i.Employment_History.Select(s => s.Currency))
   //                        .Include(i => i.Employment_History.Select(s => s.Branch))
   //                        .Include(i => i.Employment_History.Select(s => s.Designation))
   //                        .Include(i => i.Employment_History.Select(s => s.Department))
   //                        .Include(i => i.Employment_History.Select(s => s.Employment_History_Allowance))
   //                        .Include(i => i.User_Profile)
   //                        .Include(i => i.User_Profile.User_Profile_Photo)
   //                        .Include(i => i.User_Profile.User_Authentication)
   //                        .Include(i => i.Nationality)
   //                        .Include(i => i.Banking_Info)
   //                        .Include(i => i.Employee_Emergency_Contact)
   //                        .Include(i => i.Relationships)
   //                        .Include(i => i.Relationships.Select(s => s.Nationality))
   //                        .Include(i => i.Relationships.Select(s => s.Global_Lookup_Data1))
   //                        .Include(i => i.Employee_Attachment)
   //                        .Include(i => i.Employee_Attachment.Select(s => s.Global_Lookup_Data))
   //                    .Where(i => i.Profile_ID == pProfileID)
   //                 .FirstOrDefault();

   //        }
   //    }

   //    public bool IsHistoryRelated(Nullable<int> pHistoryID)
   //    {
   //        using (var db = new SBS2DBContext())
   //        {
   //            var hist = (from a in db.Employment_History where a.History_ID == pHistoryID select a).FirstOrDefault();
   //            if (hist != null)
   //            {
   //                foreach (var histAllow in hist.Employment_History_Allowance)
   //               {
   //                   if (histAllow != null)
   //                   {
   //                       if (histAllow.PRDs.Count() > 0)
   //                       {
   //                           return true;
   //                       }
   //                   }
   //               }
   //            }
   //        }
   //        return false;
   //    }


   //    public PRM GetCurrentPRM(Nullable<int> pEmpID)
   //    {
   //        if (pEmpID.HasValue)
   //        {
   //            var currentdate = StoredProcedure.GetCurrentDate();
   //            using (var db = new SBS2DBContext())
   //            {
   //                return db.PRMs
   //                     .Where(i => i.Employee_Profile_ID == pEmpID && i.Process_Month == currentdate.Month && i.Process_Year == currentdate.Year)
   //                     .FirstOrDefault();

   //            }
   //        }
   //        return null;
   //    }

   //    public string GetEmployeeNo(Nullable<int> pEmpID)
   //    {
   //        if (pEmpID.HasValue)
   //        {
   //            var currentdate = StoredProcedure.GetCurrentDate();
   //            using (var db = new SBS2DBContext())
   //            {
   //                var emp = db.Employee_Profile.Where(w => w.Employee_Profile_ID == pEmpID).FirstOrDefault();
   //                if (emp != null)
   //                {
   //                    var pattern = db.Employee_No_Pattern.Where(w => w.Company_ID == emp.User_Profile.Company_ID).FirstOrDefault();
   //                    if (pattern != null)
   //                    {
   //                        var empNo = GetEmployeeNo(pattern, currentdate, null, emp.Nationality_ID);
   //                        pattern.Current_Running_Number = pattern.Current_Running_Number + 1;
   //                        db.SaveChanges();
   //                        return empNo;
   //                    }
   //                }

   //            }
   //        }
   //        return "";
   //    }
   //    private string GetEmployeeNo(Employee_No_Pattern pattern, DateTime currentdate, Nullable<DateTime> Hired_Date, Nullable<int> Nationality_ID)
   //    {
   //        DateTime pattenDate;
   //        var year = "";
   //        var nationality = "";
   //        var branchcode = "";
   //        if (Hired_Date.HasValue)
   //        {
   //            pattenDate = Hired_Date.Value;
   //        }
   //        else
   //        {
   //            pattenDate = currentdate;
   //        }

   //        if (pattern.Year_2_Digit)
   //        {
   //            year = pattenDate.Year.ToString().Substring(2, 2) + "-";
   //        }
   //        else if (pattern.Year_4_Digit)
   //        {
   //            year = pattenDate.Year.ToString() + "-";
   //        }

   //        if (pattern.Select_Nationality)
   //        {
   //            using (var db = new SBS2DBContext())
   //            {
   //                var nal = (from a in db.Nationalities where a.Nationality_ID == Nationality_ID select a).FirstOrDefault();
   //                if (nal != null)
   //                {
   //                    nationality = nal.Name + "-";
   //                }
   //            }
   //        }
   //        if (pattern.Select_Branch_Code.HasValue && pattern.Select_Branch_Code.Value && pattern.Branch != null)
   //        {
   //            branchcode = pattern.Branch.Branch_Code;
   //        }
   //        return year + nationality + branchcode + pattern.Current_Running_Number.Value.ToString("000000");
   //    }

   //    public ServiceResult SaveEmployeeProfile(Employee_Profile pEmpProfile, Relationship[] pRelationshipList, string[] pRelationRowType, Banking_Info[] pBankInfoList, string[] pBankRowType, User_Profile user, Nullable<int> pCompanyID)
   //    {
   //        try
   //        {
   //            var currentdate = StoredProcedure.GetCurrentDate();
   //            using (var db = new SBS2DBContext())
   //            {
   //                if (pEmpProfile == null || !pCompanyID.HasValue)
   //                {
   //                    return new ServiceResult { Code = ERROR_CODE.ERROR_511_DATA_NOT_FOUND, Msg = new Error().getError(ERROR_CODE.ERROR_511_DATA_NOT_FOUND) };
   //                }
   //                Employee_Profile current = (from a in db.Employee_Profile where a.Profile_ID == pEmpProfile.Profile_ID select a).FirstOrDefault();
   //                if (current == null)
   //                {
   //                    //Insert
   //                    //---pattern----

   //                    var pattern = (from a in db.Employee_No_Pattern where a.Company_ID == pCompanyID.Value select a).SingleOrDefault();
   //                    if (pattern == null)
   //                    {
   //                        return new ServiceResult { Code = ERROR_CODE.ERROR_511_DATA_NOT_FOUND, Msg = new Error().getError(ERROR_CODE.ERROR_511_DATA_NOT_FOUND) };
   //                    }

   //                    pEmpProfile.Employee_No = GetEmployeeNo(pattern, currentdate, pEmpProfile.Hired_Date, pEmpProfile.Nationality_ID);

   //                    pattern.Current_Running_Number = pattern.Current_Running_Number + 1;

   //                    //---Relationship---
   //                    if (pRelationRowType != null)
   //                    {
   //                        for (var i = 0; i < pRelationRowType.Length; i++)
   //                        {
   //                            if (pRelationRowType[i] == RowType.ADD)
   //                            {
   //                                var relation = pRelationshipList[i];
   //                                relation.Employee_Profile_ID = pEmpProfile.Employee_Profile_ID;
   //                                relation.Profile_ID = pEmpProfile.Profile_ID;
   //                                relation.Create_By = user.Name;
   //                                relation.Create_On = currentdate;
   //                                relation.Update_By = user.Name;
   //                                relation.Update_On = currentdate;
   //                                pEmpProfile.Relationships.Add(relation);
   //                            }
   //                        }

   //                    }

   //                    //---Bank info----
   //                    if (pBankRowType != null)
   //                    {
   //                        for (var i = 0; i < pBankRowType.Length; i++)
   //                        {
   //                            if (pBankRowType[i] == RowType.ADD)
   //                            {
   //                                var bank = pBankInfoList[i];
   //                                bank.Employee_Profile_ID = pEmpProfile.Employee_Profile_ID;
   //                                bank.Profile_ID = pEmpProfile.Profile_ID;
   //                                bank.Create_By = user.Name;
   //                                bank.Create_On = currentdate;
   //                                bank.Update_By = user.Name;
   //                                bank.Update_On = currentdate;
   //                                pEmpProfile.Banking_Info.Add(bank);
   //                            }
   //                        }

   //                    }

   //                    pEmpProfile.Banking_Info = pBankInfoList;
   //                    pEmpProfile.Employee_Profile_Action.Add(new Employee_Profile_Action() { Effective_Date = pEmpProfile.Hired_Date, Action = "Hire" });
   //                    db.Employee_Profile.Add(pEmpProfile);
   //                    db.SaveChanges();
   //                    db.Entry(pEmpProfile).GetDatabaseValues();
   //                    return new ServiceResult { Code = ERROR_CODE.SUCCESS_CREATE, Msg = new Success().getSuccess(ERROR_CODE.SUCCESS_CREATE) };
   //                }
   //                else
   //                {
   //                    // Update

   //                    //---Relationship
   //                    if (pRelationRowType != null)
   //                    {
   //                        for (var i = 0; i < pRelationRowType.Length; i++)
   //                        {
   //                            var relation = pRelationshipList[i];
   //                            relation.Employee_Profile_ID = pEmpProfile.Employee_Profile_ID;
   //                            relation.Profile_ID = pEmpProfile.Profile_ID;
   //                            relation.Update_By = user.Name;
   //                            relation.Update_On = currentdate;
   //                            if (pRelationRowType[i] == RowType.ADD)
   //                            {
   //                                db.Relationships.Add(relation);
   //                                relation.Create_By = user.Name;
   //                                relation.Create_On = currentdate;
   //                            }
   //                            else if (pRelationRowType[i] == RowType.EDIT)
   //                            {
   //                                var currentrelation = (from a in db.Relationships where a.Relationship_ID == relation.Relationship_ID select a).FirstOrDefault();
   //                                relation.Create_By = currentrelation.Create_By;
   //                                relation.Create_On = currentrelation.Create_On;
   //                                db.Entry(currentrelation).CurrentValues.SetValues(relation);
   //                            }
   //                            else if (pRelationRowType[i] == RowType.DELETE)
   //                            {
   //                                if (relation.Relationship_ID > 0)
   //                                {
   //                                    var currentrelation = (from a in db.Relationships where a.Relationship_ID == relation.Relationship_ID select a).FirstOrDefault();
   //                                    db.Relationships.Remove(currentrelation);
   //                                }
   //                            }
   //                        }
   //                    }

   //                    //---Bank Info -----
   //                    if (pBankRowType != null)
   //                    {
   //                        for (var i = 0; i < pBankRowType.Length; i++)
   //                        {
   //                            var bank = pBankInfoList[i];
   //                            bank.Employee_Profile_ID = pEmpProfile.Employee_Profile_ID;
   //                            bank.Profile_ID = pEmpProfile.Profile_ID;
   //                            bank.Update_By = user.Name;
   //                            bank.Update_On = currentdate;
   //                            if (pBankRowType[i] == RowType.ADD)
   //                            {
   //                                db.Banking_Info.Add(bank);
   //                                bank.Create_By = user.Name;
   //                                bank.Create_On = currentdate;
   //                            }
   //                            else if (pBankRowType[i] == RowType.EDIT)
   //                            {
   //                                var currentbankinfo = (from a in db.Banking_Info where a.Banking_Info_ID == bank.Banking_Info_ID select a).FirstOrDefault();
   //                                bank.Create_By = currentbankinfo.Create_By;
   //                                bank.Create_On = currentbankinfo.Create_On;
   //                                db.Entry(currentbankinfo).CurrentValues.SetValues(bank);
   //                            }
   //                            else if (pBankRowType[i] == RowType.DELETE)
   //                            {
   //                                if (bank.Banking_Info_ID > 0)
   //                                {
   //                                    var currentbankinfo = (from a in db.Banking_Info where a.Banking_Info_ID == bank.Banking_Info_ID select a).FirstOrDefault();
   //                                    db.Banking_Info.Remove(currentbankinfo);
   //                                }
   //                            }
   //                        }
   //                    }
   //                    db.Entry(current).CurrentValues.SetValues(pEmpProfile);
   //                    db.SaveChanges();
   //                    return new ServiceResult { Code = ERROR_CODE.SUCCESS_EDIT, Msg = new Success().getSuccess(ERROR_CODE.SUCCESS_EDIT) };
   //                }
   //            }
   //        }
   //        catch
   //        {
   //            return new ServiceResult { Code = ERROR_CODE.ERROR_500_DB, Msg = new Error().getError(ERROR_CODE.ERROR_500_DB) };
   //        }
   //    }

   //    public List<Employee_Profile_Action> GetEmployeeProfileAction(Nullable<int> pEmpID)
   //    {
   //        if (pEmpID.HasValue)
   //        {
   //            using (var db = new SBS2DBContext())
   //            {
   //                return db.Employee_Profile_Action
   //                     .Where(i => i.Employee_Profile_ID == pEmpID).ToList();

   //            }
   //        }
   //        return null;
   //    }

   //    public ServiceResult SaveEmployeeProfileAction(List<Employee_Profile_Action> pActions, int pEmpID)
   //    {
   //        try
   //        {
   //            using (var db = new SBS2DBContext())
   //            {
   //                var currentactions = db.Employee_Profile_Action.Where(w => w.Employee_Profile_ID == pEmpID);
   //                db.Employee_Profile_Action.RemoveRange(currentactions);

   //                //Insert    
   //                if (pActions.Count > 0)
   //                {
   //                    var minhiredate = pActions[0].Effective_Date;
   //                    var emp = db.Employee_Profile.Where(w => w.Employee_Profile_ID == pEmpID).FirstOrDefault();
   //                    if (emp != null)
   //                    {
   //                        emp.Hired_Date = minhiredate;
   //                    }
   //                    db.Employee_Profile_Action.AddRange(pActions);
   //                    db.SaveChanges();
   //                }
   //                return new ServiceResult { Code = ERROR_CODE.SUCCESS, Msg = new Success().getSuccess(ERROR_CODE.SUCCESS_EDIT) };
   //            }
   //        }
   //        catch
   //        {
   //            return new ServiceResult { Code = ERROR_CODE.ERROR_500_DB, Msg = new Error().getError(ERROR_CODE.ERROR_500_DB) };
   //        }

   //    }

   //    public List<string> LstEmail(Nullable<int> pCompanyID)
   //    {
   //        using (var db = new SBS2DBContext())
   //        {
   //            return db.User_Profile.Where(w => w.Company_ID == pCompanyID).Select(s => s.User_Authentication.Email_Address).ToList();

   //        }
   //    }
   //    public static int LINK_TIME_LIMIT = 120;
   //    private  Random _rng = new Random();
   //    public const string _chars = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmopqrstuvwxyz";
   //    public string randomString(int size)
   //    {

   //        char[] buffer = new char[size];

   //        for (int i = 0; i < size; i++)
   //        {
   //            buffer[i] = _chars[_rng.Next(_chars.Length)];
   //        }
   //        return new string(buffer);
   //    }
   //    public Boolean validateActivationCode(SBS2DBContext db, String code)
   //    {
   //        Activation_Link u = (from a in db.Activation_Link where a.Activation_Code.Equals(code) select a).FirstOrDefault();
   //        if (u != null)
   //            return false;
   //        else
   //            return true;
   //    }


   //    public ServiceResult ImportEmployee(Employee_Profile_[] emps, HttpRequestBase Request)
   //    {
   //        DateTime currentdate = StoredProcedure.GetCurrentDate();
   //        using (var db = new SBS2DBContext())
   //        {
   //            try
   //            {
   //                foreach (var emp in emps)
   //                {

   //                    if (db.Users.Where(w => w.UserName == emp.Email_Address.ToLower()).FirstOrDefault() != null)
   //                        return new ServiceResult { Code = ERROR_CODE.ERROR_510_DATA_DUPLICATE, Msg = new Error().getError(ERROR_CODE.ERROR_510_DATA_DUPLICATE), Field = Resources.ResourceEmployee.Email };

   //                    var com = (from a in db.Company_Details
   //                               where (a.Effective_Date <= currentdate & a.Company_ID == emp.Company_ID)
   //                               orderby a.Effective_Date descending
   //                               select a).FirstOrDefault();

   //                    if (com == null)
   //                        return new ServiceResult { Code = ERROR_CODE.ERROR_511_DATA_NOT_FOUND, Msg = new Error().getError(ERROR_CODE.ERROR_511_DATA_NOT_FOUND), Field = Resources.ResourceCompany.Company };

   //                    var guid = Guid.NewGuid().ToString();
   //                    while (db.Users.Where(w => w.Id == guid).FirstOrDefault() != null)
   //                    {
   //                        guid = Guid.NewGuid().ToString();
   //                    }

   //                    db.Users.Add(new ApplicationUser() { Id = guid, UserName = emp.Email_Address.ToLower() });

   //                    emp.ApplicationUser_Id = guid;

   //                    User_Authentication authen = new User_Authentication()
   //                    {
   //                        Email_Address = emp.Email_Address,
   //                        ApplicationUser_Id = emp.ApplicationUser_Id
   //                    };

   //                    User_Profile user = new User_Profile()
   //                    {
   //                        Name = emp.Name,
   //                        User_Status = UserStatus.Active,
   //                        Registration_Date = currentdate,
   //                        Company_ID = com.Company_ID,
   //                        User_Authentication = authen
   //                    };

   //                    //GENERATE ACTIVATION CODE
   //                    String code;
   //                    do
   //                    {
   //                        code = "A" + randomString(40);
   //                    }
   //                    while (!validateActivationCode(db, code));

   //                    //SET User_Assign_Role to ROLE_CUSTOMER_USER
   //                    User_Assign_Role role = new User_Assign_Role()
   //                    {
   //                        User_Role_ID = UserRole.ROLE_CUSTOMER_USER
   //                    };

   //                    authen.User_Assign_Role.Add(role);

   //                    Activation_Link activation_link = new Activation_Link()
   //                    {
   //                        Activation_Code = code,
   //                        //SET Time_Limit to activate within LINK_TIME_LIMIT
   //                        Time_Limit = currentdate.AddHours(LINK_TIME_LIMIT),
   //                    };

   //                    authen.Activation_Link.Add(activation_link);

   //                    var pattern = (from a in db.Employee_No_Pattern where a.Company_ID == emp.Company_ID.Value select a).SingleOrDefault();
   //                    if (pattern == null)
   //                        return new ServiceResult { Code = ERROR_CODE.ERROR_511_DATA_NOT_FOUND, Msg = new Error().getError(ERROR_CODE.ERROR_511_DATA_NOT_FOUND), Field = Resources.ResourceConfiguration.Pattern };


   //                    var hireddate = DateUtil.ToDate(emp.Hired_Date);
   //                    Employee_Profile e = new Employee_Profile()
   //                    {
   //                        Employee_No = GetEmployeeNo(pattern, currentdate, hireddate, emp.Nationality_ID),
   //                        Emp_Status = emp.Emp_Status,
   //                        Gender = emp.Gender,
   //                        Hired_Date = hireddate,
   //                        Marital_Status = emp.Marital_Status,
   //                        Mobile_No = emp.Mobile_No,
   //                        Nationality_ID = emp.Nationality_ID,
   //                        NRIC = emp.NRIC,
   //                        PR_End_Date = DateUtil.ToDate(emp.PR_End_Date),
   //                        PR_Start_Date = DateUtil.ToDate(emp.PR_Start_Date),
   //                        Religion = emp.Religion,
   //                        Residential_Status = emp.Residential_Status,
   //                        WP_Class = emp.WP_Class,
   //                        WP_End_Date = DateUtil.ToDate(emp.WP_End_Date),
   //                        WP_No = emp.WP_No,
   //                        WP_Start_Date = DateUtil.ToDate(emp.WP_Start_Date)
   //                    };

   //                    pattern.Current_Running_Number = pattern.Current_Running_Number + 1;

   //                    e.Employee_Profile_Action.Add(new Employee_Profile_Action() { Effective_Date = hireddate, Action = "Hire" });

   //                    user.Employee_Profile.Add(e);
   //                    db.User_Profile.Add(user);

   //                    try
   //                    {
   //                        //SEND EMAIL
   //                        //EmailTemplete.sendUserActivateEmail(emp.Email_Address, code, emp.Name, com.Name, com.Phone, com.Name, Request);
   //                    }
   //                    catch
   //                    {

   //                    }
   //                }
   //                db.SaveChanges();
   //            }
   //            catch
   //            {
   //                return new ServiceResult { Code = ERROR_CODE.ERROR_500_DB, Msg = new Error().getError(ERROR_CODE.ERROR_500_DB) };
   //            }

   //        }

   //        return new ServiceResult { Code = ERROR_CODE.SUCCESS, Msg = new Success().getSuccess(ERROR_CODE.SUCCESS) };
   //    }

   //    public ServiceResult InsertEmployee(User_Profile pUserProfile, int[] pUserAssignRole, Employee_Profile pEmp,
   //        EmployeeEmergencyContactViewModel[] pEmer, RelationshipViewModels[] pRelation,
   //        BankInfoViewModel[] pBank, HistoryViewModel[] pHist)
   //    {
   //        try
   //        {
   //            var currentdate = StoredProcedure.GetCurrentDate();

   //            using (var db = new SBS2DBContext())
   //            {
   //                if (db.Users.Where(w => w.UserName == pUserProfile.Email.ToLower()).FirstOrDefault() != null)
   //                    return new ServiceResult { Code = ERROR_CODE.ERROR_510_DATA_DUPLICATE, Msg = "duplicate " + Resources.ResourceEmployee.Email, Field = Resources.ResourceEmployee.Employee };

   //                var com = (from a in db.Company_Details
   //                           where (a.Effective_Date <= currentdate & a.Company_ID == pUserProfile.Company_ID)
   //                           orderby a.Effective_Date descending
   //                           select a).FirstOrDefault();

   //                if (com == null)
   //                    return new ServiceResult { Code = ERROR_CODE.ERROR_511_DATA_NOT_FOUND, Msg = Resources.ResourceCompany.Company + " not found.", Field = Resources.ResourceEmployee.Employee };

   //                var pattern = (from a in db.Employee_No_Pattern where a.Company_ID == pUserProfile.Company_ID.Value select a).SingleOrDefault();
   //                if (pattern == null)
   //                    return new ServiceResult { Code = ERROR_CODE.ERROR_511_DATA_NOT_FOUND, Msg = Resources.ResourceConfiguration.Pattern + " not found.", Field = Resources.ResourceEmployee.Employee };


   //                var guid = Guid.NewGuid().ToString();
   //                while (db.Users.Where(w => w.Id == guid).FirstOrDefault() != null)
   //                {
   //                    guid = Guid.NewGuid().ToString();
   //                }

   //                db.Users.Add(new ApplicationUser() { Id = guid, UserName = pUserProfile.Email.ToLower() });

   //                var ApplicationUser_Id = guid;

   //                User_Authentication authen = new User_Authentication()
   //                {
   //                    Email_Address = pUserProfile.Email,
   //                    ApplicationUser_Id = ApplicationUser_Id
   //                };

   //                User_Profile user = new User_Profile()
   //                {
   //                    First_Name = pUserProfile.First_Name,
   //                    Middle_Name = pUserProfile.Middle_Name,
   //                    Last_Name = pUserProfile.Last_Name,
   //                    User_Status = pUserProfile.User_Status,
   //                    Registration_Date = currentdate,
   //                    Company_ID = com.Company_ID,
   //                    Phone = pUserProfile.Phone,
   //                    User_Authentication = authen,
   //                    User_Profile_Photo = pUserProfile.User_Profile_Photo
   //                };

   //                //GENERATE ACTIVATION CODE
   //                String code;
   //                do
   //                {
   //                    code = "A" + randomString(40);
   //                }
   //                while (!validateActivationCode(db, code));

   //                //SET User_Assign_Role to ROLE_CUSTOMER_USER
   //                if (pUserAssignRole != null)
   //                {
   //                    foreach (var r in pUserAssignRole)
   //                    {
   //                        User_Assign_Role role = new User_Assign_Role()
   //                        {
   //                            User_Role_ID = r
   //                        };

   //                        authen.User_Assign_Role.Add(role);
   //                    }
   //                }


   //                Activation_Link activation_link = new Activation_Link()
   //                {
   //                    Activation_Code = code,
   //                    //SET Time_Limit to activate within LINK_TIME_LIMIT
   //                    Time_Limit = currentdate.AddHours(LINK_TIME_LIMIT),
   //                };

   //                authen.Activation_Link.Add(activation_link);



   //                //var hireddate =pEmp.Hired_Date;

   //                pEmp.Employee_No = GetEmployeeNo(pattern, currentdate, null, pEmp.Nationality_ID);



   //                pattern.Current_Running_Number = pattern.Current_Running_Number + 1;

   //                //pEmp.Employee_Profile_Action.Add(new Employee_Profile_Action() { Effective_Date = hireddate, Action = "Hire" });



   //                if (pEmer != null)
   //                {
   //                    foreach (var row in pEmer)
   //                    {
   //                        if (row.Row_Type == RowType.ADD)
   //                        {
   //                            var emer = new Employee_Emergency_Contact()
   //                            {
   //                                Contact_No = row.Contact_No,
   //                                Name = row.Name,
   //                                Relationship = row.Relationship
   //                            };
   //                            pEmp.Employee_Emergency_Contact.Add(emer);
   //                        }
   //                    }
   //                }

   //                if (pRelation != null)
   //                {
   //                    foreach (var row in pRelation)
   //                    {
   //                        if (row.Row_Type == RowType.ADD)
   //                        {
   //                            var relationship = new Relationship()
   //                            {
   //                                Company_Name = row.Company_Name,
   //                                Company_Position = row.Company_Position,
   //                                DOB = DateUtil.ToDate(row.DOB),
   //                                Gender = row.Gender,
   //                                Name = row.Name,
   //                                Nationality_ID = row.Nationality_ID,
   //                                NRIC = row.NRIC,
   //                                Passport = row.Passport,
   //                                Relationship1 = row.Relationship,
   //                                Working = row.Working
   //                            };
   //                            pEmp.Relationships.Add(relationship);
   //                        }
   //                    }
   //                }

   //                if (pBank != null)
   //                {
   //                    foreach (var row in pBank)
   //                    {
   //                        if (row.Row_Type == RowType.ADD)
   //                        {
   //                            var bank = new Banking_Info()
   //                            {
   //                                Payment_Type = row.Payment_Type,
   //                                Bank_Name = row.Bank_Name,
   //                                Bank_Account = row.Bank_Account,
   //                                Effective_Date = DateUtil.ToDate(row.Effective_Date)
   //                            };
   //                            pEmp.Banking_Info.Add(bank);
   //                        }
   //                    }
   //                }

   //                if (pHist != null)
   //                {
   //                    foreach (var row in pHist)
   //                    {
   //                        if (row.Row_Type == RowType.ADD)
   //                        {
   //                            var hist = new Employment_History()
   //                            {
   //                                Basic_Salary = EncryptUtil.Encrypt(row.Basic_Salary),
   //                                Branch_ID = row.Branch_ID,
   //                                Confirm_Date = DateUtil.ToDate(row.Confirm_Date),
   //                                Currency_ID = row.Currency_ID,
   //                                Department_ID = row.Department_ID,
   //                                Designation_ID = row.Designation_ID,
   //                                Effective_Date = DateUtil.ToDate(row.Effective_Date),
   //                                Employee_Type = row.Employee_Type,
   //                                Payment_Type = row.Payment_Type,
   //                                Supervisor = (row.Supervisor > 0 ? row.Supervisor : null),
   //                                Terminate_Date = DateUtil.ToDate(row.Terminate_Date),
   //                                ST_Sun_Time = DateUtil.ToTime(row.ST_Sun_Time),
   //                                ST_Mon_Time = DateUtil.ToTime(row.ST_Mon_Time),
   //                                ST_Tue_Time = DateUtil.ToTime(row.ST_Tue_Time),
   //                                ST_Wed_Time = DateUtil.ToTime(row.ST_Wed_Time),
   //                                ST_Thu_Time = DateUtil.ToTime(row.ST_Thu_Time),
   //                                ST_Fri_Time = DateUtil.ToTime(row.ST_Fri_Time),
   //                                ST_Sat_Time = DateUtil.ToTime(row.ST_Sat_Time),
   //                                ET_Sun_Time = DateUtil.ToTime(row.ET_Sun_Time),
   //                                ET_Mon_Time = DateUtil.ToTime(row.ET_Mon_Time),
   //                                ET_Tue_Time = DateUtil.ToTime(row.ET_Tue_Time),
   //                                ET_Wed_Time = DateUtil.ToTime(row.ET_Wed_Time),
   //                                ET_Thu_Time = DateUtil.ToTime(row.ET_Thu_Time),
   //                                ET_Fri_Time = DateUtil.ToTime(row.ET_Fri_Time),
   //                                ET_Sat_Time = DateUtil.ToTime(row.ET_Sat_Time),
   //                                CL_Sun = row.CL_Sun,
   //                                CL_Mon = row.CL_Mon,
   //                                CL_Tue = row.CL_Tue,
   //                                CL_Wed = row.CL_Wed,
   //                                CL_Thu = row.CL_Thu,
   //                                CL_Fri = row.CL_Fri,
   //                                CL_Sat = row.CL_Sat,
   //                            };

   //                            if (row.History_Allowance_Rows != null)
   //                            {
   //                                foreach (var arow in row.History_Allowance_Rows)
   //                                {
   //                                    if (arow.Row_Type == RowType.ADD)
   //                                    {
   //                                        var allowance = new Employment_History_Allowance()
   //                                        {
   //                                            Amount = arow.Amount,
   //                                            PRC_ID = arow.PRC_ID,
   //                                            PRT_ID = arow.PRT_ID
   //                                        };
   //                                        if (arow.PRC_ID == 0)
   //                                            allowance.PRC_ID = null;

   //                                        hist.Employment_History_Allowance.Add(allowance);
   //                                    }

   //                                }
   //                            }
   //                            pEmp.Employment_History.Add(hist);
   //                        }
   //                    }
   //                }


   //                user.Employee_Profile.Add(pEmp);
   //                db.User_Profile.Add(user);


   //                db.SaveChanges();
   //                return new ServiceResult() { Code = ERROR_CODE.SUCCESS, Msg = new Success().getSuccess(ERROR_CODE.SUCCESS_CREATE), Field = Resources.ResourceEmployee.Employee, Object = code };
   //            }
   //        }
   //        catch
   //        {
   //            return new ServiceResult() { Code = ERROR_CODE.ERROR_503_INSERT_ERROR, Msg = new Error().getError(ERROR_CODE.ERROR_503_INSERT_ERROR), Field = Resources.ResourceEmployee.Employee };
   //        }
   //    }

   //    public ServiceResult UpdateEmployee(User_Profile pUserProfile, int[] pUserAssignRole, Employee_Profile pEmp,
   //        EmployeeEmergencyContactViewModel[] pEmer, RelationshipViewModels[] pRelation,
   //        BankInfoViewModel[] pBank, HistoryViewModel[] pHist)
   //    {
   //        try
   //        {
   //            var currentdate = StoredProcedure.GetCurrentDate();

   //            using (var db = new SBS2DBContext())
   //            {
   //                var currProfile = db.User_Profile.Where(w => w.Profile_ID == pUserProfile.Profile_ID).FirstOrDefault();

   //                if (currProfile == null)
   //                    return new ServiceResult { Code = ERROR_CODE.ERROR_511_DATA_NOT_FOUND, Msg = Resources.ResourceAccount.User + " not found.", Field = Resources.ResourceEmployee.Employee };

   //                if (db.Users.Where(w => w.UserName == pUserProfile.Email.ToLower() && w.Id != currProfile.User_Authentication.ApplicationUser_Id).FirstOrDefault() != null)
   //                    return new ServiceResult { Code = ERROR_CODE.ERROR_510_DATA_DUPLICATE, Msg = "duplicate " + Resources.ResourceEmployee.Email, Field = Resources.ResourceEmployee.Employee };

   //                var com = (from a in db.Company_Details
   //                           where (a.Effective_Date <= currentdate & a.Company_ID == pUserProfile.Company_ID)
   //                           orderby a.Effective_Date descending
   //                           select a).FirstOrDefault();

   //                if (com == null)
   //                    return new ServiceResult { Code = ERROR_CODE.ERROR_511_DATA_NOT_FOUND, Msg = Resources.ResourceCompany.Company + " not found.", Field = Resources.ResourceEmployee.Employee };


   //                var currEmp = db.Employee_Profile.Where(w => w.Employee_Profile_ID == pEmp.Employee_Profile_ID).FirstOrDefault();

   //                if (currEmp == null)
   //                    return new ServiceResult { Code = ERROR_CODE.ERROR_511_DATA_NOT_FOUND, Msg = Resources.ResourceEmployee.Employee + " not found.", Field = Resources.ResourceEmployee.Employee };

   //                if (pUserAssignRole != null)
   //                {

   //                    //Delete unchecked  
   //                    foreach (User_Assign_Role urole in currProfile.User_Authentication.User_Assign_Role.ToList())
   //                    {
   //                        if (!pUserAssignRole.Contains(urole.User_Role_ID.Value))
   //                        {
   //                            db.User_Assign_Role.Remove(urole);
   //                        }
   //                    }

   //                    foreach (int role in pUserAssignRole)
   //                    {
   //                        User_Assign_Role dbarole = currProfile.User_Authentication.User_Assign_Role.Where(s => s.User_Role_ID == role).SingleOrDefault();
   //                        if (dbarole == null)
   //                        {
   //                            User_Assign_Role arole = new User_Assign_Role()
   //                            {
   //                                User_Authentication_ID = currProfile.User_Authentication_ID,
   //                                User_Role_ID = role
   //                            };

   //                            db.User_Assign_Role.Add(arole);
   //                        }
   //                    }
   //                }
   //                else
   //                {
   //                    foreach (User_Assign_Role dbarole in currProfile.User_Authentication.User_Assign_Role.ToList())
   //                    {
   //                        db.User_Assign_Role.Remove(dbarole);
   //                    }
   //                }


   //                if (pEmer != null)
   //                {
   //                    foreach (var row in pEmer)
   //                    {
   //                        if (row.Row_Type == RowType.ADD)
   //                        {
   //                            var emer = new Employee_Emergency_Contact()
   //                            {
   //                                Contact_No = row.Contact_No,
   //                                Name = row.Name,
   //                                Relationship = (row.Relationship.HasValue ? (row.Relationship.Value > 0 ? row.Relationship : null) : null),
   //                                Employee_Profile_ID = pEmp.Employee_Profile_ID
   //                            };
   //                            db.Employee_Emergency_Contact.Add(emer);

   //                        }
   //                        else if (row.Row_Type == RowType.EDIT)
   //                        {
   //                            var currEmer = db.Employee_Emergency_Contact.Where(w => w.Employee_Emergency_Contact_ID == row.Employee_Emergency_Contact_ID).FirstOrDefault();
   //                            if (currEmer != null)
   //                            {
   //                                currEmer.Contact_No = row.Contact_No;
   //                                currEmer.Name = row.Name;
   //                                currEmer.Relationship = (row.Relationship.HasValue ? (row.Relationship.Value > 0 ? row.Relationship : null) : null);
   //                            }
   //                        }
   //                        else if (row.Row_Type == RowType.DELETE)
   //                        {
   //                            var currEmer = db.Employee_Emergency_Contact.Where(w => w.Employee_Emergency_Contact_ID == row.Employee_Emergency_Contact_ID).FirstOrDefault();
   //                            if (currEmer != null)
   //                            {
   //                                db.Employee_Emergency_Contact.Remove(currEmer);
   //                            }
   //                        }
   //                    }
   //                }
   //                if (pRelation != null)
   //                {
   //                    foreach (var row in pRelation)
   //                    {
   //                        if (row.Row_Type == RowType.ADD)
   //                        {
   //                            var relationship = new Relationship()
   //                            {
   //                                Company_Name = row.Company_Name,
   //                                Company_Position = row.Company_Position,
   //                                DOB = DateUtil.ToDate(row.DOB),
   //                                Gender = row.Gender,
   //                                Name = row.Name,
   //                                Nationality_ID = row.Nationality_ID,
   //                                NRIC = row.NRIC,
   //                                Passport = row.Passport,
   //                                Relationship1 = row.Relationship,
   //                                Working = row.Working,
   //                                Employee_Profile_ID = pEmp.Employee_Profile_ID
   //                            };
   //                            db.Relationships.Add(relationship);

   //                        }
   //                        else if (row.Row_Type == RowType.EDIT)
   //                        {
   //                            var currRelation = db.Relationships.Where(w => w.Relationship_ID == row.Relationship_ID).FirstOrDefault();
   //                            if (currRelation != null)
   //                            {
   //                                currRelation.Company_Name = row.Company_Name;
   //                                currRelation.Company_Position = row.Company_Position;
   //                                currRelation.DOB = DateUtil.ToDate(row.DOB);
   //                                currRelation.Gender = row.Gender;
   //                                currRelation.Name = row.Name;
   //                                currRelation.Nationality_ID = row.Nationality_ID;
   //                                currRelation.NRIC = row.NRIC;
   //                                currRelation.Passport = row.Passport;
   //                                currRelation.Relationship1 = row.Relationship;
   //                                currRelation.Working = row.Working;
   //                            }
   //                        }
   //                        else if (row.Row_Type == RowType.DELETE)
   //                        {
   //                            var currRelation = db.Relationships.Where(w => w.Relationship_ID == row.Relationship_ID).FirstOrDefault();
   //                            if (currRelation != null)
   //                            {
   //                                db.Relationships.Remove(currRelation);
   //                            }
   //                        }
   //                    }
   //                }

   //                if (pBank != null)
   //                {
   //                    foreach (var row in pBank)
   //                    {
   //                        if (row.Row_Type == RowType.ADD)
   //                        {
   //                            var bank = new Banking_Info()
   //                            {
   //                                Payment_Type = row.Payment_Type,
   //                                Bank_Name = row.Bank_Name,
   //                                Bank_Account = row.Bank_Account,
   //                                Effective_Date = DateUtil.ToDate(row.Effective_Date),
   //                                Employee_Profile_ID = pEmp.Employee_Profile_ID
   //                            };
   //                            db.Banking_Info.Add(bank);

   //                        }
   //                        else if (row.Row_Type == RowType.EDIT)
   //                        {
   //                            var currBank = db.Banking_Info.Where(w => w.Banking_Info_ID == row.Banking_Info_ID).FirstOrDefault();
   //                            if (currBank != null)
   //                            {
   //                                currBank.Payment_Type = row.Payment_Type;
   //                                currBank.Bank_Name = row.Bank_Name;
   //                                currBank.Bank_Account = row.Bank_Account;
   //                                currBank.Effective_Date = DateUtil.ToDate(row.Effective_Date);
   //                            }
   //                        }
   //                        else if (row.Row_Type == RowType.DELETE)
   //                        {
   //                            var currBank = db.Banking_Info.Where(w => w.Banking_Info_ID == row.Banking_Info_ID).FirstOrDefault();
   //                            if (currBank != null)
   //                            {
   //                                db.Banking_Info.Remove(currBank);
   //                            }
   //                        }
   //                    }
   //                }


   //                if (pHist != null)
   //                {
   //                    foreach (var row in pHist)
   //                    {
   //                        if (row.Row_Type == RowType.ADD)
   //                        {
   //                            var hist = new Employment_History()
   //                            {
   //                                Basic_Salary = EncryptUtil.Encrypt(row.Basic_Salary),
   //                                Branch_ID = row.Branch_ID,
   //                                Confirm_Date = DateUtil.ToDate(row.Confirm_Date),
   //                                Currency_ID = row.Currency_ID,
   //                                Department_ID = row.Department_ID,
   //                                Designation_ID = row.Designation_ID,
   //                                Effective_Date = DateUtil.ToDate(row.Effective_Date),
   //                                Employee_Type = row.Employee_Type,
   //                                Payment_Type = row.Payment_Type,
   //                                Supervisor = (row.Supervisor > 0 ? row.Supervisor : null),
   //                                Terminate_Date = DateUtil.ToDate(row.Terminate_Date),
   //                                Employee_Profile_ID = pEmp.Employee_Profile_ID,
   //                                ST_Sun_Time = DateUtil.ToTime(row.ST_Sun_Time),
   //                                ST_Mon_Time = DateUtil.ToTime(row.ST_Mon_Time),
   //                                ST_Tue_Time = DateUtil.ToTime(row.ST_Tue_Time),
   //                                ST_Wed_Time = DateUtil.ToTime(row.ST_Wed_Time),
   //                                ST_Thu_Time = DateUtil.ToTime(row.ST_Thu_Time),
   //                                ST_Fri_Time = DateUtil.ToTime(row.ST_Fri_Time),
   //                                ST_Sat_Time = DateUtil.ToTime(row.ST_Sat_Time),
   //                                ET_Sun_Time = DateUtil.ToTime(row.ET_Sun_Time),
   //                                ET_Mon_Time = DateUtil.ToTime(row.ET_Mon_Time),
   //                                ET_Tue_Time = DateUtil.ToTime(row.ET_Tue_Time),
   //                                ET_Wed_Time = DateUtil.ToTime(row.ET_Wed_Time),
   //                                ET_Thu_Time = DateUtil.ToTime(row.ET_Thu_Time),
   //                                ET_Fri_Time = DateUtil.ToTime(row.ET_Fri_Time),
   //                                ET_Sat_Time = DateUtil.ToTime(row.ET_Sat_Time),
   //                                CL_Sun = row.CL_Sun,
   //                                CL_Mon = row.CL_Mon,
   //                                CL_Tue = row.CL_Tue,
   //                                CL_Wed = row.CL_Wed,
   //                                CL_Thu = row.CL_Thu,
   //                                CL_Fri = row.CL_Fri,
   //                                CL_Sat = row.CL_Sat,
   //                            };
   //                            if (row.History_Allowance_Rows != null)
   //                            {
   //                                foreach (var arow in row.History_Allowance_Rows)
   //                                {
   //                                    if (arow.Row_Type == RowType.ADD)
   //                                    {
   //                                        var allowance = new Employment_History_Allowance()
   //                                        {
   //                                            Amount = arow.Amount,
   //                                            PRC_ID = arow.PRC_ID,
   //                                            PRT_ID = arow.PRT_ID
   //                                        };

   //                                        if (arow.PRC_ID == 0)
   //                                            allowance.PRC_ID = null;

   //                                        hist.Employment_History_Allowance.Add(allowance);
   //                                    }

   //                                }
   //                            }
   //                            db.Employment_History.Add(hist);

   //                        }
   //                        else if (row.Row_Type == RowType.EDIT)
   //                        {
   //                            var currHist = db.Employment_History.Where(w => w.History_ID == row.History_ID).FirstOrDefault();
   //                            if (currHist != null)
   //                            {
   //                                currHist.Basic_Salary = EncryptUtil.Encrypt(row.Basic_Salary);
   //                                currHist.Branch_ID = row.Branch_ID;
   //                                currHist.Confirm_Date = DateUtil.ToDate(row.Confirm_Date);
   //                                currHist.Currency_ID = row.Currency_ID;
   //                                currHist.Department_ID = row.Department_ID;
   //                                currHist.Designation_ID = row.Designation_ID;
   //                                currHist.Effective_Date = DateUtil.ToDate(row.Effective_Date);
   //                                currHist.Employee_Type = row.Employee_Type;
   //                                currHist.Payment_Type = row.Payment_Type;
   //                                currHist.Supervisor = (row.Supervisor > 0 ? row.Supervisor : null);
   //                                currHist.Terminate_Date = DateUtil.ToDate(row.Terminate_Date);
   //                                currHist.ST_Sun_Time = DateUtil.ToTime(row.ST_Sun_Time);
   //                                currHist.ST_Mon_Time = DateUtil.ToTime(row.ST_Mon_Time);
   //                                currHist.ST_Tue_Time = DateUtil.ToTime(row.ST_Tue_Time);
   //                                currHist.ST_Wed_Time = DateUtil.ToTime(row.ST_Wed_Time);
   //                                currHist.ST_Thu_Time = DateUtil.ToTime(row.ST_Thu_Time);
   //                                currHist.ST_Fri_Time = DateUtil.ToTime(row.ST_Fri_Time);
   //                                currHist.ST_Sat_Time = DateUtil.ToTime(row.ST_Sat_Time);
   //                                currHist.ET_Sun_Time = DateUtil.ToTime(row.ET_Sun_Time);
   //                                currHist.ET_Mon_Time = DateUtil.ToTime(row.ET_Mon_Time);
   //                                currHist.ET_Tue_Time = DateUtil.ToTime(row.ET_Tue_Time);
   //                                currHist.ET_Wed_Time = DateUtil.ToTime(row.ET_Wed_Time);
   //                                currHist.ET_Thu_Time = DateUtil.ToTime(row.ET_Thu_Time);
   //                                currHist.ET_Fri_Time = DateUtil.ToTime(row.ET_Fri_Time);
   //                                currHist.ET_Sat_Time = DateUtil.ToTime(row.ET_Sat_Time);
   //                                currHist.CL_Sun = row.CL_Sun;
   //                                currHist.CL_Mon = row.CL_Mon;
   //                                currHist.CL_Tue = row.CL_Tue;
   //                                currHist.CL_Wed = row.CL_Wed;
   //                                currHist.CL_Thu = row.CL_Thu;
   //                                currHist.CL_Fri = row.CL_Fri;
   //                                currHist.CL_Sat = row.CL_Sat;

   //                                if (row.History_Allowance_Rows != null)
   //                                {
   //                                    foreach (var arow in row.History_Allowance_Rows)
   //                                    {
   //                                        if (arow.Row_Type == RowType.ADD)
   //                                        {
   //                                            var allowance = new Employment_History_Allowance()
   //                                            {
   //                                                Amount = arow.Amount,
   //                                                PRC_ID = arow.PRC_ID,
   //                                                PRT_ID = arow.PRT_ID,
   //                                                History_ID = row.History_ID
   //                                            };

   //                                            if (arow.PRC_ID == 0)
   //                                                allowance.PRC_ID = null;

   //                                            db.Employment_History_Allowance.Add(allowance);
   //                                        }
   //                                        else if (arow.Row_Type == RowType.EDIT)
   //                                        {
   //                                            var currAll = db.Employment_History_Allowance.Where(w => w.Employment_History_Allowance_ID == arow.Employment_History_Allowance_ID).FirstOrDefault();
   //                                            if (currAll != null)
   //                                            {
   //                                                currAll.Amount = arow.Amount;
   //                                                currAll.PRC_ID = arow.PRC_ID;
   //                                                currAll.PRT_ID = arow.PRT_ID;

   //                                                if (arow.PRC_ID == 0) currAll.PRC_ID = null;
   //                                            }
   //                                        }
   //                                        else if (arow.Row_Type == RowType.DELETE)
   //                                        {
   //                                            var currAll = db.Employment_History_Allowance.Where(w => w.Employment_History_Allowance_ID == arow.Employment_History_Allowance_ID).FirstOrDefault();
   //                                            if (currAll != null)
   //                                            {
   //                                                db.Employment_History_Allowance.Remove(currAll);
   //                                            }
   //                                        }
   //                                    }
   //                                }
   //                            }
   //                        }
   //                        else if (row.Row_Type == RowType.DELETE)
   //                        {
   //                            var currHist = db.Employment_History.Where(w => w.History_ID == row.History_ID).FirstOrDefault();
   //                            if (currHist != null)
   //                            {
   //                                var currAll = db.Employment_History_Allowance.Where(w => w.History_ID == row.History_ID);
   //                                db.Employment_History_Allowance.RemoveRange(currAll);
   //                                db.Employment_History.Remove(currHist);
   //                            }
   //                        }
   //                    }
   //                }


   //                db.Entry(currEmp).CurrentValues.SetValues(pEmp);
   //                db.Entry(currProfile).CurrentValues.SetValues(pUserProfile);
   //                db.SaveChanges();

   //                return new ServiceResult() { Code = ERROR_CODE.SUCCESS, Msg = new Success().getSuccess(ERROR_CODE.SUCCESS_EDIT), Field = Resources.ResourceEmployee.Employee };
   //            }
   //        }
   //        catch
   //        {
   //            return new ServiceResult() { Code = ERROR_CODE.ERROR_504_UPDATE_ERROR, Msg = new Error().getError(ERROR_CODE.ERROR_504_UPDATE_ERROR), Field = Resources.ResourceEmployee.Employee };
   //        }
   //    }

   //    public ServiceResult DeleteEmployee(Nullable<int> pEmpID)
   //    {
   //        try
   //        {
   //            using (var db = new SBS2DBContext())
   //            {
   //                var emp = db.Employee_Profile.Where(w => w.Employee_Profile_ID == pEmpID).FirstOrDefault();
   //                if (emp != null)
   //                {
   //                    var user = db.User_Profile.Where(w => w.Profile_ID == emp.Profile_ID).FirstOrDefault();
   //                    if (user != null)
   //                    {
   //                        var photo = db.User_Profile_Photo.Where(w => w.User_Profile_Photo_ID == user.User_Profile_Photo_ID);
   //                        db.User_Profile_Photo.RemoveRange(photo);

   //                        var role = db.User_Assign_Role.Where(w => w.User_Authentication_ID == user.User_Authentication_ID);
   //                        db.User_Assign_Role.RemoveRange(role);

   //                        var alink = db.Activation_Link.Where(w => w.User_Authentication_ID == user.User_Authentication_ID);
   //                        db.Activation_Link.RemoveRange(alink);

   //                        var aspUser = db.Users.Where(w => w.Id.Equals(user.User_Authentication.ApplicationUser_Id)).FirstOrDefault();
   //                        db.Users.Remove(aspUser);

   //                        var authen = db.User_Authentication.Where(w => w.User_Authentication_ID == user.User_Authentication_ID).FirstOrDefault();
   //                        db.User_Authentication.Remove(authen);

   //                        var ir = db.IRs.Where(w => w.Profile_ID == user.Profile_ID);
   //                        db.IRs.RemoveRange(ir);

   //                        var ia = db.IAs.Where(w => w.Profile_ID == user.Profile_ID);
   //                        db.IAs.RemoveRange(ia);

   //                        db.User_Profile.Remove(user);
   //                    }

   //                    var contact = db.Employee_Emergency_Contact.Where(w => w.Employee_Profile_ID == emp.Employee_Profile_ID);
   //                    db.Employee_Emergency_Contact.RemoveRange(contact);

   //                    var bank = db.Banking_Info.Where(w => w.Employee_Profile_ID == emp.Employee_Profile_ID);
   //                    db.Banking_Info.RemoveRange(bank);

   //                    var rs = db.Relationships.Where(w => w.Employee_Profile_ID == emp.Employee_Profile_ID);
   //                    db.Relationships.RemoveRange(rs);

   //                    var empHistAllowance = db.Employment_History_Allowance.Where(w => w.Employment_History.Employee_Profile_ID == emp.Employee_Profile_ID);
   //                    db.Employment_History_Allowance.RemoveRange(empHistAllowance);

   //                    var empHist = db.Employment_History.Where(w => w.Employee_Profile_ID == emp.Employee_Profile_ID);
   //                    db.Employment_History.RemoveRange(empHist);

   //                    var action = db.Employee_Profile_Action.Where(w => w.Employee_Profile_ID == emp.Employee_Profile_ID);
   //                    db.Employee_Profile_Action.RemoveRange(action);

   //                    var empAtt = db.Employee_Attachment.Where(w => w.Employee_Profile_ID == emp.Employee_Profile_ID);
   //                    db.Employee_Attachment.RemoveRange(empAtt);

   //                    db.Employee_Profile.Remove(emp);
   //                    db.SaveChanges();
   //                }

   //                return new ServiceResult() { Code = ERROR_CODE.SUCCESS, Msg = new Success().getSuccess(ERROR_CODE.SUCCESS_DELETE), Field = Resources.ResourceEmployee.Employee };
   //            }
   //        }
   //        catch
   //        {
   //            return new ServiceResult() { Code = ERROR_CODE.ERROR_505_DELETE_ERROR, Msg = new Error().getError(ERROR_CODE.ERROR_505_DELETE_ERROR), Field = Resources.ResourceEmployee.Employee };
   //        }
   //    }

   //    public int DeleteAttachment(Nullable<System.Guid> pAttachmentID)
   //    {
   //        try
   //        {
   //            using (var db = new SBS2DBContext())
   //            {
   //                var attach = db.Employee_Attachment.Where(w => w.Attachment_ID == pAttachmentID).FirstOrDefault();
   //                if (attach == null)
   //                {
   //                    return -500;
   //                }
   //                db.Employee_Attachment.Remove(attach);

   //                db.SaveChanges();
   //                return 1;
   //            }
   //        }
   //        catch (Exception e)
   //        {
   //            //Log
   //            return -500;
   //        }
   //    }

   //    public int InsertAttachment(Employee_Attachment pAttach)
   //    {
   //        try
   //        {
   //            using (var db = new SBS2DBContext())
   //            {
   //                db.Employee_Attachment.Add(pAttach);
   //                db.SaveChanges();

   //                return 1;
   //            }
   //        }
   //        catch (Exception e)
   //        {
   //            //Log
   //            return -500;
   //        }
   //    }

   //    public ServiceResult UpdateEmpStatus(int[] pEmp, string pStatus)
   //    {
   //        try
   //        {
   //            using (var db = new SBS2DBContext())
   //            {
   //                if (pStatus == UserStatus.Active | pStatus == UserStatus.Inactive)
   //                {
   //                    var emps = db.Employee_Profile.Where(w => pEmp.Contains(w.Employee_Profile_ID));
   //                    foreach (var emp in emps)
   //                    {
   //                        emp.User_Profile.User_Status = pStatus;
   //                    }
   //                    db.SaveChanges();
   //                }
   //                return new ServiceResult() { Code = ERROR_CODE.SUCCESS, Msg = new Success().getSuccess(ERROR_CODE.SUCCESS_EDIT), Field = Resources.ResourceEmployee.Employee };
   //            }
   //        }
   //        catch (Exception e)
   //        {
   //            //Log
   //            return new ServiceResult() { Code = ERROR_CODE.ERROR_504_UPDATE_ERROR, Msg = new Error().getError(ERROR_CODE.ERROR_504_UPDATE_ERROR), Field = Resources.ResourceEmployee.Employee };
   //        }
   //    }
   //}


   public class EmployeeViewModels : ModelBase
   {
      public List<string> adminRights { get; set; }
      public List<string> hrRights { get; set; }
      public string pageAction { get; set; }
      public string tabAction2 { get; set; }
      public string step { get; set; }

      public List<ComboViewModel> nationalityList { get; set; }
      public List<ComboViewModel> empStatusList { get; set; }
      public List<ComboViewModel> residentialStatusList { get; set; }
      public List<ComboViewModel> religionList { get; set; }
      public List<ComboViewModel> genderList { get; set; }
      public List<ComboViewModel> raceList { get; set; }
      public List<ComboViewModel> wpClassList { get; set; }
      public List<ComboViewModel> relationshipList { get; set; }
      public List<ComboViewModel> childTypeList { get; set; }
      public List<ComboViewModel> maritalStatusList { get; set; }
      public List<ComboViewModel> branchList { get; set; }
      public List<ComboViewModel> departmentList { get; set; }
      public List<ComboViewModel> statusList { get; set; }
      public List<ComboViewModel> paymentTypeList { get; set; }
      public List<ComboViewModel> empTypeList { get; set; }
      public List<ComboViewModel> supervisorList { get; set; }
      public List<ComboViewModel> desingnationList { get; set; }
      public List<ComboViewModel> currencyList { get; set; }
      public List<ComboViewModel> prtList { get; set; }
      public List<ComboViewModel> prcList { get; set; }
      public List<ComboViewModel> attachmentTypeList { get; set; }
      public List<ComboViewModel> workPassTypeList { get; set; }
      public List<ComboViewModel> periodList { get; set; }
      public List<ComboViewModel> termList { get; set; }
      public Nullable<int> search_Branch { get; set; }
      public Nullable<int> search_Department { get; set; }
      public Nullable<int> search_empTypeList { get; set; }

      public List<Employee_Profile> EmpList;

      public List<User_Role> UserRoleList;

      public List<Subscription> SubscriptionList;

      public int[] User_Assign_Module;

      public int[] Users_Assign_Role;

      public Nullable<int> Employee_Profile_ID { get; set; }
      public Nullable<int> User_Login_Profile_ID { get; set; }
      public Nullable<int> Profile_ID { get; set; }
      public Nullable<int> User_Authentication_ID { get; set; }
      public bool Activated { get; set; }
      public Nullable<int> Company_ID { get; set; }
      public string Company_Level { get; set; }
      public List<PRM> prmlist { get; set; }
      public byte[] User_Photo { get; set; }
      public Nullable<System.Guid> User_Profile_Photo_ID { get; set; }

      [LocalizedDisplayName("Employee_No", typeof(Resource))]
      public string Employee_No { get; set; }

      //public string Employee_No_Old { get; set; }

      //Add By sun 31-08-2015
      public string Employee_No_Running { get; set; }
      public string Company_Currency_Code { get; set; }

      //Not found in db field
      [LocalizedRequired]
      [LocalizedValidMaxLength(150)]
      [LocalizedDisplayName("First_Name", typeof(Resource))]
      public string First_Name { get; set; }

      [LocalizedValidMaxLength(150)]
      [LocalizedDisplayName("Middle_Name", typeof(Resource))]
      public string Middle_Name { get; set; }

      [LocalizedRequired]
      [LocalizedValidMaxLength(150)]
      [LocalizedDisplayName("Last_Name", typeof(Resource))]
      public string Last_Name { get; set; }

      [LocalizedValidMaxLength(150)]
      [LocalizedDisplayName("Phone", typeof(Resource))]
      public string Phone { get; set; }


      [LocalizedValidMaxLength(50)]
      [LocalizedValidationEmail]
      [DataType(DataType.EmailAddress)]
      [LocalizedDisplayName("Email", typeof(Resource))]
      public string Email { get; set; }

      [LocalizedValidMaxLength(10)]
      [LocalizedValidationUserName]
      [LocalizedDisplayName("User_Name", typeof(Resource))]
      public string User_Name { get; set; }

      //Added by sun 09-09-2016
      [LocalizedRequired]
      [LocalizedDisplayName("Preferred_User_Id_Method", typeof(Resource))]
      public bool Is_Email { get; set; }

      public string Chk_No_Email { get; set; }

      [LocalizedRequired]
      [LocalizedDisplayName("Status", typeof(Resource))]
      public string User_Status { get; set; }

      [LocalizedValidMaxLength(150)]
      [LocalizedDisplayName("Mobile_Phone", typeof(Resource))]
      public string Mobile_No { get; set; }

      [LocalizedRequired]
      [LocalizedDisplayName("Gender", typeof(Resource))]
      public Nullable<int> Gender { get; set; }

      [LocalizedRequired]
      [LocalizedDisplayName("Marital_Status", typeof(Resource))]
      public Nullable<int> Marital_Status { get; set; }

      [LocalizedRequired]
      [LocalizedValidDate]
      [DataType(DataType.Date)]
      [LocalizedDisplayName("DOB", typeof(Resource))]
      public String DOB { get; set; }

      [LocalizedDisplayName("Nationality", typeof(Resource))]
      public Nullable<int> Nationality_ID { get; set; }

      [LocalizedDisplayName("Residential_Status", typeof(Resource))]
      public string Residential_Status { get; set; }

      [LocalizedDisplayName("Work_Pass_Type", typeof(Resource))]
      public Nullable<int> Work_Pass_Type { get; set; }

      [LocalizedRequired]
      [LocalizedValidMaxLength(30)]
      [LocalizedDisplayName("NRIC", typeof(Resource))]
      public string NRIC { get; set; }

      [LocalizedValidMaxLength(300)]
      [LocalizedDisplayName("Passport_No", typeof(Resource))]
      public string Passport { get; set; }

      [LocalizedValidMaxLength(150)]
      [LocalizedDisplayName("PR_No", typeof(Resource))]
      public string PR_No { get; set; }

      [DataType(DataType.Date)]
      [LocalizedDisplayName("PR_Date", typeof(Resource))]
      public String PR_Start_Date { get; set; }

      [DataType(DataType.Date)]
      public String PR_End_Date { get; set; }

      //Added By sun 24-08-2015
      [AllowHtml]
      [LocalizedValidMaxLength(500)]
      [LocalizedDisplayName("Remarks", typeof(Resource))]
      public string Remark { get; set; }

      [DataType(DataType.Date)]
      [LocalizedDisplayName("Hired_Date", typeof(Resource))]
      public String Hired_Date { get; set; }

      [DataType(DataType.Date)]
      [LocalizedDisplayName("Confirm_Date", typeof(Resource))]
      public String Confirm_Date { get; set; }

      [LocalizedValidMaxLength(150)]
      [LocalizedDisplayName("Residential_Phone", typeof(Resource))]
      public string Residential_No { get; set; }

      [LocalizedDisplayName("Status", typeof(Resource))]
      public string Emp_Status { get; set; }

      [LocalizedDisplayName("Race", typeof(Resource))]
      public Nullable<int> Race { get; set; }

      [LocalizedDisplayName("Religion", typeof(Resource))]
      public Nullable<int> Religion { get; set; }

      [LocalizedValidMaxLength(150)]
      [LocalizedDisplayName("Immigration_No", typeof(Resource))]
      public string Immigration_No { get; set; }

      [DataType(DataType.Date)]
      [LocalizedDisplayName("Expiry_Date", typeof(Resource))]
      public String Expiry_Date { get; set; }

      [LocalizedDisplayName("WP_Class", typeof(Resource))]
      public Nullable<int> WP_Class { get; set; }

      [LocalizedValidMaxLength(150)]
      public string WP_No { get; set; }

      [DataType(DataType.Date)]
      [LocalizedDisplayName("WP_Date", typeof(Resource))]
      public String WP_Start_Date { get; set; }

      [DataType(DataType.Date)]
      public String WP_End_Date { get; set; }

      [LocalizedValidMaxLength(300)]
      [LocalizedDisplayName("Address_1", typeof(Resource))]
      public string Residential_Address_1 { get; set; }

      [LocalizedValidMaxLength(300)]
      [LocalizedDisplayName("Address_2", typeof(Resource))]
      public string Residential_Address_2 { get; set; }

      [LocalizedValidMaxLength(20)]
      [LocalizedDisplayName("Zip_Or_Postal_Code", typeof(Resource))]
      public string Postal_Code_1 { get; set; }

      [LocalizedValidMaxLength(20)]
      [LocalizedDisplayName("Zip_Or_Postal_Code", typeof(Resource))]
      public string Postal_Code_2 { get; set; }

      //Added by sun 01-10-2015
      public List<ComboViewModel> countryList;

      [LocalizedDisplayName("Country", typeof(Resource))]
      public Nullable<int> Residential_Country_1 { get; set; }

      [LocalizedDisplayName("Country", typeof(Resource))]
      public Nullable<int> Residential_Country_2 { get; set; }

      [LocalizedValidMaxLength(150)]
      [LocalizedDisplayName("Name", typeof(Resource))]
      public string Emergency_Name { get; set; }

      [LocalizedValidMaxLength(15)]
      [LocalizedDisplayName("Contact_No", typeof(Resource))]
      public string Emergency_Contact_No { get; set; }

      [LocalizedDisplayName("Relationship", typeof(Resource))]
      public Nullable<int> Emergency_Relationship { get; set; }


      [LocalizedDisplayName("Opt_Out", typeof(Resource))]
      public Nullable<bool> Opt_Out { get; set; }

      [DataType(DataType.Date)]
      [LocalizedDisplayName("Date_Of_Issue", typeof(Resource))]
      public string NRIC_FIN_Issue_Date { get; set; }

      [DataType(DataType.Date)]
      [LocalizedDisplayName("Date_Of_Expire", typeof(Resource))]
      public string NRIC_FIN_Expire_Date { get; set; }

      [DataType(DataType.Date)]
      [LocalizedDisplayName("Date_Of_Issue", typeof(Resource))]
      public string Passport_Issue_Date { get; set; }

      [DataType(DataType.Date)]
      [LocalizedDisplayName("Date_Of_Expire", typeof(Resource))]
      public string Passpor_Expire_Date { get; set; }

      public Nullable<bool> Contribute_Rate1 { get; set; }
      public Nullable<bool> Contribute_Rate2 { get; set; }
      public string Activation_Code { get; set; }


      //--------Employee Emergency Contact-----------    
      public EmployeeEmergencyContactViewModel[] Emer_Contact_Rows { get; set; }

      //--------Bank Info-----------    
      public BankInfoViewModel[] Bank_Info_Rows { get; set; }
      public Nullable<int> Bank_Info_Selected { get; set; }

      //-------- History Info-----------    
      public HistoryViewModel[] History_Rows { get; set; }

      public Nullable<int> History_ID { get; set; }
      public Nullable<int> History_Index { get; set; }
      
      [LocalizedDisplayName("Department", typeof(Resource))]
      public Nullable<int> History_Department_ID { get; set; }
      
      [LocalizedDisplayName("Designation", typeof(Resource))]
      public Nullable<int> History_Designation_ID { get; set; }

      [LocalizedRequired]
      [LocalizedDisplayName("Currency", typeof(Resource))]
      public Nullable<int> History_Currency_ID { get; set; }

      [LocalizedValidDate]
      //[LocalizedRequired]
      [DataType(DataType.Date)]
      [LocalizedDisplayName("Hire_Or_Effective_Date", typeof(Resource))]
      public string History_Effective_Date { get; set; }

      [LocalizedDisplayName("Supervisor", typeof(Resource))]
      public Nullable<int> History_Supervisor { get; set; }

      //Moet added on 5/Sep
      public Nullable<bool> History_No_Approval_WF { get; set; }
      public string History_Other_Branch { get; set; }
      public string History_Other_Designation { get; set; }
      public string History_Other_Department { get; set; }
      
      [LocalizedDisplayName("Branch", typeof(Resource))]
      public Nullable<int> History_Branch_ID { get; set; }


      [LocalizedValidDate]
      [LocalizedRequired]
      [DataType(DataType.Date)]
      [LocalizedDisplayName("Confirm_Date", typeof(Resource))]
      public string History_Confirm_Date { get; set; }

      [DataType(DataType.Date)]
      [LocalizedDisplayName("Terminated_Date", typeof(Resource))]
      public string History_Terminate_Date { get; set; }

      [LocalizedRequired]
      [LocalizedDisplayName("Basic_Salary", typeof(Resource))]
      public Nullable<decimal> History_Basic_Salary { get; set; }

      [LocalizedDisplayName("Hour_Rate", typeof(Resource))]
      public Nullable<decimal> History_Hour_Rate { get; set; }

      [LocalizedRequired]
      public string History_Basic_Salary_Unit { get; set; }


      [LocalizedRequired]
      [LocalizedDisplayName("Employment_Type", typeof(Resource))]
      public string History_Employee_Type { get; set; }

      public string History_Row_Type { get; set; }
      public string History_ST_Sun_Time { get; set; }
      public string History_ST_Mon_Time { get; set; }
      public string History_ST_Tue_Time { get; set; }
      public string History_ST_Wed_Time { get; set; }
      public string History_ST_Thu_Time { get; set; }
      public string History_ST_Fri_Time { get; set; }
      public string History_ST_Sat_Time { get; set; }
      public string History_ET_Sun_Time { get; set; }
      public string History_ET_Mon_Time { get; set; }
      public string History_ET_Tue_Time { get; set; }
      public string History_ET_Wed_Time { get; set; }
      public string History_ET_Thu_Time { get; set; }
      public string History_ET_Fri_Time { get; set; }
      public string History_ET_Sat_Time { get; set; }
      public Nullable<bool> History_CL_Sun { get; set; }
      public Nullable<bool> History_CL_Mon { get; set; }
      public Nullable<bool> History_CL_Tue { get; set; }
      public Nullable<bool> History_CL_Wed { get; set; }
      public Nullable<bool> History_CL_Thu { get; set; }
      public Nullable<bool> History_CL_Fri { get; set; }
      public Nullable<bool> History_CL_Sat { get; set; }

      //public string History_ST_Lunch_Time { get; set; }
      //public string History_ET_Lunch_Time { get; set; }
      //public Nullable<bool> History_CL_Lunch { get; set; }

      public Nullable<decimal> History_Days { get; set; }
      public Nullable<bool> Pattern_Nationality_Se { get; set; }


      [LocalizedDisplayName("Notice_Period", typeof(Resource))]
      public Nullable<decimal> History_Notice_Period_Amount { get; set; }

      public string History_Notice_Period_Unit { get; set; }
      public Nullable<bool> History_Contract_Staff { get; set; }

      [DataType(DataType.Date)]
      [LocalizedDisplayName("Contract_Start_Date", typeof(Resource))]
      public String History_Contract_Start_Date { get; set; }

      [DataType(DataType.Date)]
      [LocalizedDisplayName("Contract_End_Date", typeof(Resource))]
      public String History_Contract_End_Date { get; set; }

      public HistoryAllowanceViewModel[] History_Allowance_Rows { get; set; }

      //--------Relation Ship-----------    
      public RelationshipViewModels[] Relationship_Rows { get; set; }
      public Nullable<int> Relationship_ID { get; set; }
      public Nullable<int> Relationship_Index { get; set; }
      public bool Relationship_Leaved { get; set; }

      //[LocalizedRequired]
      [LocalizedValidMaxLength(150)]
      [LocalizedDisplayName("Name", typeof(Resource))]
      public string Relationship_Name { get; set; }

      [LocalizedDisplayName("Relationship", typeof(Resource))]
      public Nullable<int> Relationship_Relationship { get; set; }

      [LocalizedDisplayName("Child_Type", typeof(Resource))]
      public string Relationship_Child_Type { get; set; }

      public bool Relationship_Is_Maternity { get; set; }
      public bool Relationship_Is_Paternity { get; set; }
      public bool Relationship_Is_Maternity_Share_Father { get; set; }

      [LocalizedValidDate]
      //[LocalizedRequired]
      [DataType(DataType.Date)]
      [LocalizedDisplayName("DOB", typeof(Resource))]
      public String Relationship_DOB { get; set; }

      [LocalizedDisplayName("Gender", typeof(Resource))]
      public Nullable<int> Relationship_Gender { get; set; }

      [LocalizedValidMaxLength(30)]
      [LocalizedDisplayName("NRIC", typeof(Resource))]
      public string Relationship_NRIC { get; set; }

      [LocalizedValidMaxLength(100)]
      [LocalizedDisplayName("Passport_No", typeof(Resource))]
      public string Relationship_Passport { get; set; }

      [LocalizedDisplayName("Nationality", typeof(Resource))]
      public Nullable<int> Relationship_Nationality_ID { get; set; }

      [LocalizedDisplayName("Working", typeof(Resource))]
      public bool Relationship_Working { get; set; }

      [LocalizedValidMaxLength(150)]
      [LocalizedDisplayName("Company_Name", typeof(Resource))]
      public string Relationship_Company_Name { get; set; }

      [LocalizedValidMaxLength(150)]
      [LocalizedDisplayName("Company_Position", typeof(Resource))]
      public string Relationship_Company_Position { get; set; }

      public string Relationship_Row_Type { get; set; }

      //-------- Attachment file -----------    
      public AttachmentViewModel[] Attachment_Rows { get; set; }

      public int[] Emp;
   }

   public class EmployeeEmergencyContactViewModel
   {
      public List<ComboViewModel> relationshipList;
      public int Index { get; set; }
      public int Employee_Emergency_Contact_ID { get; set; }

      [LocalizedRequired]
      [LocalizedValidMaxLength(150)]
      [LocalizedDisplayName("Name", typeof(Resource))]
      public string Name { get; set; }

      [LocalizedRequired]
      [LocalizedValidMaxLength(50)]
      [LocalizedDisplayName("Contact_No", typeof(Resource))]
      public string Contact_No { get; set; }

      public Nullable<int> Relationship { get; set; }
      public string Row_Type { get; set; }
   }

   public class BankInfoViewModel
   {
      public List<ComboViewModel> paymentTypeList { get; set; }
      public Nullable<int> Banking_Info_ID { get; set; }
      public int Index { get; set; }

      [LocalizedRequired]
      [LocalizedValidMaxLength(150)]
      [LocalizedDisplayName("Bank_Name", typeof(Resource))]
      public string Bank_Name { get; set; }

      [LocalizedRequired]
      [LocalizedValidMaxLength(150)]
      [LocalizedDisplayName("Bank_Account", typeof(Resource))]
      public string Bank_Account { get; set; }

      [LocalizedDisplayName("Payment_Type", typeof(Resource))]
      public Nullable<int> Payment_Type { get; set; }

      [LocalizedValidDate]
      [LocalizedRequired]
      [LocalizedDisplayName("Effective_Date", typeof(Resource))]
      public string Effective_Date { get; set; }

      public string Row_Type { get; set; }
      public Nullable<bool> Selected { get; set; }
   }

   public class RelationshipViewModels
   {
      public int Index { get; set; }
      public Nullable<int> Relationship_ID { get; set; }

      [LocalizedRequired]
      [LocalizedValidMaxLength(150)]
      [LocalizedDisplayName("Name", typeof(Resource))]
      public string Name { get; set; }

      [LocalizedDisplayName("Relationship", typeof(Resource))]
      public Nullable<int> Relationship { get; set; }

      [LocalizedDisplayName("Child_Type", typeof(Resource))]
      public string Child_Type { get; set; }
      public bool Is_Paternity { get; set; }
      public bool Is_Maternity { get; set; }
      public bool Is_Maternity_Share_Father { get; set; }
      public string Relationship_Name { get; set; }

      [LocalizedDisplayName("DOB", typeof(Resource))]
      public String DOB { get; set; }

      [LocalizedDisplayName("Gender", typeof(Resource))]
      public Nullable<int> Gender { get; set; }

      [LocalizedValidMaxLength(30)]
      [LocalizedDisplayName("NRIC", typeof(Resource))]
      public string NRIC { get; set; }

      [LocalizedValidMaxLength(100)]
      [LocalizedDisplayName("Passport_No", typeof(Resource))]
      public string Passport { get; set; }

      [LocalizedDisplayName("Nationality", typeof(Resource))]
      public Nullable<int> Nationality_ID { get; set; }
      public string Nationality_Name { get; set; }

      [LocalizedDisplayName("Working", typeof(Resource))]
      public bool Working { get; set; }

      [LocalizedValidMaxLength(150)]
      [LocalizedDisplayName("Company_Name", typeof(Resource))]
      public string Company_Name { get; set; }

      [LocalizedValidMaxLength(150)]
      [LocalizedDisplayName("Company_Position", typeof(Resource))]
      public string Company_Position { get; set; }
      public string Row_Type { get; set; }
      public bool Leaved { get; set; }
   }

   public class HistoryViewModel : ModelBase
   {
      public int History_ID { get; set; }

      [LocalizedRequired]
      [LocalizedDisplayName("Department", typeof(Resource))]
      public Nullable<int> Department_ID { get; set; }
      public string Department_Name { get; set; }

      [LocalizedRequired]
      [LocalizedDisplayName("Designation", typeof(Resource))]
      public Nullable<int> Designation_ID { get; set; }
      public string Designation_Name { get; set; }

      [LocalizedRequired]
      [LocalizedDisplayName("Currency", typeof(Resource))]
      public Nullable<int> Currency_ID { get; set; }

      [LocalizedValidDate]
      [LocalizedRequired]
      [LocalizedDisplayName("Effective_Date", typeof(Resource))]
      public string Effective_Date { get; set; }

      [LocalizedDisplayName("Supervisor", typeof(Resource))]
      public Nullable<int> Supervisor { get; set; }

      [LocalizedDisplayName("Branch", typeof(Resource))]
      public Nullable<int> Branch_ID { get; set; }
      public string Branch_Name { get; set; }

      [LocalizedDisplayName("Confirm_Date", typeof(Resource))]
      public string Confirm_Date { get; set; }

      [LocalizedDisplayName("Terminated_Date", typeof(Resource))]
      public string Terminate_Date { get; set; }

      [LocalizedRequired]
      [LocalizedDisplayName("Basic_Salary", typeof(Resource))]
      public Nullable<decimal> Basic_Salary { get; set; }

      [LocalizedDisplayName("Hour_Rate", typeof(Resource))]
      public Nullable<decimal> Hour_Rate { get; set; }

      [LocalizedRequired]
      public string Basic_Salary_Unit { get; set; }

      [LocalizedDisplayName("Payment_Type", typeof(Resource))]
      public Nullable<int> Payment_Type { get; set; }

      [LocalizedDisplayName("Employment_Type", typeof(Resource))]
      public string Employee_Type { get; set; }
      public string Employee_Type_Name { get; set; }
      public HistoryAllowanceViewModel[] History_Allowance_Rows { get; set; }

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
      public Nullable<decimal> Days { get; set; }

      public Nullable<decimal> Notice_Period_Amount { get; set; }
      public string Notice_Period_Unit { get; set; }
      public Nullable<bool> Contract_Staff { get; set; }
      public string Contract_Start_Date { get; set; }
      public string Contract_End_Date { get; set; }
      public string Row_Type { get; set; }
      //Moet added on 5/Sep
      public Nullable<bool> No_Approval_WF { get; set; }
      public string Other_Branch { get; set; }
      public string Other_Designation { get; set; }
      public string Other_Department { get; set; }

      //Added by sun 30-11-2016
      //public string ST_Lunch_Time { get; set; }
      //public string ET_Lunch_Time { get; set; }
      //public Nullable<bool> CL_Lunch { get; set; }
   }

   public class HistoryAllowanceViewModel : ModelBase
   {
      public List<ComboViewModel> prtList { get; set; }
      public List<ComboViewModel> prcList { get; set; }
      public int Index { get; set; }
      public Nullable<int> Employment_History_Allowance_ID { get; set; }

      [LocalizedRequired]
      [LocalizedDisplayName("Allowance_Type", typeof(Resource))]
      public Nullable<int> PRT_ID { get; set; }

      [LocalizedRequired]
      [LocalizedDisplayName("Description", typeof(Resource))]
      public Nullable<int> PRC_ID { get; set; }

      [LocalizedRequired]
      [LocalizedDisplayName("Amount", typeof(Resource))]
      public Nullable<decimal> Amount { get; set; }
      public string Company_Currency_Code { get; set; }
      public string Row_Type { get; set; }
   }

   public class AttachmentViewModel
   {
      public string Row_Type { get; set; }
      public byte[] Attachment_File { get; set; }
      public int Index { get; set; }
      public Nullable<System.Guid> Attachment_ID { get; set; }

      [LocalizedDisplayName("Uploaded_On", typeof(Resource))]
      public string Uploaded_On { get; set; }

      [LocalizedDisplayName("Uploaded_By", typeof(Resource))]
      public string Uploaded_By { get; set; }

      [LocalizedValidMaxLength(150)]
      public string File_Name { get; set; }

      //public string Upload_File { get; set; }

      [LocalizedDisplayName("Attachment_Type", typeof(Resource))]
      public Nullable<int> Attachment_Type { get; set; }
      public string Attachment_Type_Name { get; set; }
   }

   public class ImportEmployeeViewModels : ModelBase
   {
      public bool validated { get; set; }

      // Employee
      public ImportEmployeeProfile_[] emps { get; set; }
      public List<string> errMsg { get; set; }

      // History
      public ImportHistory_[] hiss { get; set; }
      public List<string> errMsg_Hit { get; set; }

      // Emergency Contact
      public ImportEmergencyContact_[] emergencys { get; set; }
      public List<string> errMsg_Eemc { get; set; }

      // Relationship
      public ImportRelationship_[] relations { get; set; }
      public List<string> errMsg_Relation { get; set; }
   }

   public partial class Employee_Profile_
   {
      public Nullable<int> Company_ID { get; set; }
      public bool Validate { get; set; }
      public string ErrMsg { get; set; }
      public string Email_Address { get; set; }
      public string Name { get; set; }
      public Nullable<int> Nationality_ID { get; set; }
      public string Nationality_ { get; set; }
      public string Mobile_No { get; set; }
      public Nullable<int> Gender { get; set; }
      public string Gender_ { get; set; }
      public string NRIC { get; set; }
      public Nullable<int> Marital_Status { get; set; }
      public string Marital_Status_ { get; set; }
      public string Emp_Status { get; set; }
      public string Emp_Status_ { get; set; }
      public Nullable<int> Race { get; set; }
      public string Race_ { get; set; }
      public string Residential_Status { get; set; }
      public string Residential_Status_ { get; set; }
      public string PR_No { get; set; }
      public string PR_Start_Date { get; set; }
      public string PR_End_Date { get; set; }
      public Nullable<int> WP_Class { get; set; }
      public string WP_Class_ { get; set; }
      public string WP_No { get; set; }
      public string WP_Start_Date { get; set; }
      public string WP_End_Date { get; set; }
      public Nullable<int> Religion { get; set; }
      public string Religion_ { get; set; }
      public string Hired_Date { get; set; }
      public string ApplicationUser_Id { get; set; }
      public string Employee_No { get; set; }
   }

   public class EmployeeActionViewModels : ModelBase
   {
      public Nullable<int> Employee_Profile_ID { get; set; }
      public Nullable<int> Profile_ID { get; set; }
      public int Index { get; set; }
      public string[] Rows_Type { get; set; }
      public Employee_Action[] empActionlist { get; set; }
   }

   public class Employee_Action
   {
      public string Hired_Date { get; set; }
      public string Terminated_Date { get; set; }
   }

   //Added By sun 24-08-2015
   public class Thumbmail
   {
      public static byte[] GenerateTumbnail(string image, double thumbWidth)
      {
         try
         {
            using (var originalImage = System.Drawing.Image.FromFile(image))
            {
               var oWidth = originalImage.Width;
               var oHeight = originalImage.Height;
               var thumbHeight = oWidth > thumbWidth ? (thumbWidth / oWidth) * oHeight : oHeight;
               thumbWidth = oWidth > thumbWidth ? thumbWidth : oWidth;
               using (var bmp = new Bitmap((int)thumbWidth, (int)thumbHeight))
               {
                  bmp.SetResolution(originalImage.HorizontalResolution, originalImage.VerticalResolution);
                  using (var graphic = Graphics.FromImage(bmp))
                  {
                     graphic.SmoothingMode = SmoothingMode.AntiAlias;
                     graphic.InterpolationMode = InterpolationMode.HighQualityBicubic;
                     graphic.PixelOffsetMode = PixelOffsetMode.HighQuality;

                     var rectangle = new Rectangle(0, 0, (int)thumbWidth, (int)thumbHeight);
                     graphic.DrawImage(originalImage, rectangle, 0, 0, oWidth, oHeight, GraphicsUnit.Pixel);
                     var ms = new MemoryStream();
                     bmp.Save(ms, originalImage.RawFormat);
                     return ms.GetBuffer();
                  }
               }
            }
         }
         catch (Exception ex)
         {
            throw (ex);
         }
      }
   }

   //Added by sun 21-12-2015
   public class EmployeeReportModel : ModelBase
   {
      public int sDepartment { get; set; }

      [LocalizedDisplayName("Employee", typeof(Resource))]
      public string sEmployee { get; set; }

      [DataType(DataType.Date)]
      [LocalizedDisplayName("From", typeof(Resource))]
      public string sFrom { get; set; }

      [DataType(DataType.Date)]
      [LocalizedDisplayName("To", typeof(Resource))]
      public string sTo { get; set; }

      public Nullable<int> Department { get; set; }
      public List<ComboViewModel> departmentList { get; set; }
      public List<Employee_Profile> employeeList { get; set; }
      public List<ComboViewModel> residentialStatusList { get; set; }
      public List<ComboViewModel> nationalityList { get; set; }
   }


}
