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


namespace SBSModel.Models
{
   public class CompanyCriteria : CriteriaBase
   {
      public Nullable<int> Company_ID { get; set; }
      public string Company_Name { get; set; }
      public string A7_Group_ID { get; set; }
      public string Email { get; set; }
   }


   public class CompanyService
   {
      public List<Company_Details> LstCompany(CompanyCriteria cri)
      {
         using (var db = new SBS2DBContext())
         {
            var coms = db.Company_Details.Where(w => 1 == 1);
            if (cri != null)
            {
               if (!string.IsNullOrEmpty(cri.Company_Name))
                  coms = coms.Where(w => w.Name == cri.Company_Name);

               if (!string.IsNullOrEmpty(cri.A7_Group_ID))
                  coms = coms.Where(w => w.A7_Group_ID == cri.A7_Group_ID);

               if (!string.IsNullOrEmpty(cri.Email))
                  coms = coms.Where(w => w.Email == cri.Email);

               if (cri.Company_ID.HasValue)
                  coms = coms.Where(w => w.Company_ID == cri.Company_ID);
            }
            return coms.OrderBy(o => o.Name).ToList();
         }
      }

      public List<Company_Details> LstCompany(Nullable<int> pCompanyID, Nullable<int> pCountryID = null, string pRegistrationDate = "", string pStatus = "")
      {
         List<Company_Details> companys = new List<Company_Details>();
         using (var db = new SBS2DBContext())
         {
            var bluecube = db.Company_Details.Where(w => w.Company_ID == pCompanyID & w.Belong_To_ID == null).Include(i => i.Country).FirstOrDefault();
            if (bluecube != null)
            {
               // is blue cube
               companys = db.Company_Details.Include(i => i.Country).ToList();
               if (pCountryID.HasValue)
               {
                  companys = companys.Where(w => w.Country_ID == pCountryID).ToList();
               }
               if (!string.IsNullOrEmpty(pRegistrationDate))
               {
                  var d = DateUtil.ToDate(pRegistrationDate);
                  companys = companys
                      .Where(w => w.Effective_Date.Day == d.Value.Day
                      & w.Effective_Date.Month == d.Value.Month
                      & w.Effective_Date.Year == d.Value.Year)
                      .ToList();
               }
               if (!string.IsNullOrEmpty(pStatus))
               {
               }
            }
            else
            {
               companys.AddRange(getBelongToCompany(pCompanyID, pCountryID, pStatus));
               companys.OrderBy(o => o.Company_ID);
            }
         }


         return companys.OrderBy(o => o.Company_ID).ToList();
      }

      public List<Company_Details> getBelongToCompany(Nullable<int> pCompanyID, Nullable<int> pCountryID = null, string pRegistrationDate = "", string pStatus = "")
      {
         using (var db = new SBS2DBContext())
         {
            //Get users in company
            List<Company_Details> companys = new List<Company_Details>();

            //Get users of company that belongto this company
            var belongs = db.Company_Details
                .Include(i => i.Country)
                .Where(w => w.Belong_To_ID == pCompanyID)
                .Distinct();

            if (pCountryID.HasValue)
            {
               belongs = belongs.Where(w => w.Country_ID == pCountryID);
            }
            if (!string.IsNullOrEmpty(pRegistrationDate))
            {
               var d = DateUtil.ToDate(pRegistrationDate);
               companys = companys.Where(w => w.Effective_Date.Day == d.Value.Day
                       & w.Effective_Date.Month == d.Value.Month
                       & w.Effective_Date.Year == d.Value.Year).ToList();
            }
            if (!string.IsNullOrEmpty(pStatus))
            {
            }

            companys.AddRange(belongs);

            if (belongs != null && belongs.Count() > 0)
            {
               foreach (Company_Details belong in belongs)
               {
                  if (belong.Company_ID != pCompanyID)
                  {
                     companys.AddRange(getBelongToCompany(belong.Company_ID));
                  }
               }
            }
            return companys;
         }
      }

      public Company_Details GetCompany(Nullable<int> pCompanyId)
      {
         var db = new SBS2DBContext();
         return (from a in db.Company_Details
                 where a.Company_ID.Equals(pCompanyId.Value)
                 orderby a.Effective_Date descending
                 select a).FirstOrDefault();
      }

      public Company_Details GetCompany(Nullable<int> pCompanyId, DateTime pEffectiveDate)
      {
         if (pCompanyId.HasValue)
         {
            var db = new SBS2DBContext();

            var q = (from a in db.Company_Details
                     where a.Effective_Date == pEffectiveDate
                     & a.Company_ID == pCompanyId.Value
                     orderby a.Effective_Date descending
                     select a).FirstOrDefault();

            return q;
         }
         return null;
      }

      public Currency GetCurrency(Nullable<int> pCountryID)
      {

         using (var db = new SBS2DBContext())
         {
            return db.Currencies.Where(w => w.Country_ID == pCountryID).FirstOrDefault();
         }

      }

      public Currency GetCurrency(string pCurCode)
      {

         using (var db = new SBS2DBContext())
         {
            return db.Currencies.Where(w => w.Currency_Code == pCurCode).FirstOrDefault();
         }

      }

      public Country GetCountry(string pCounCode)
      {

         using (var db = new SBS2DBContext())
         {
            return db.Countries.Where(w => w.Name == pCounCode).FirstOrDefault();
         }

      }

      public Country GetCountry(Nullable<int> pCountryID)
      {

          using (var db = new SBS2DBContext())
          {
              return db.Countries.Where(w => w.Country_ID == pCountryID).FirstOrDefault();
          }

      }

      public Company_Details GetCompanyA7(string pA7groupID)
      {
         using (var db = new SBS2DBContext())
         {

            var com = db.Company_Details.Where(w => w.A7_Group_ID == pA7groupID).FirstOrDefault();
            if (com != null)
               return com;

            return null;
         }
      }

      public ServiceResult InsertCompany(Company pCompany)
      {
         try
         {
            var currentdate = StoredProcedure.GetCurrentDate();
            using (var db = new SBS2DBContext())
            {
               if (pCompany != null)
               {
                  var user = pCompany.User_Profile.FirstOrDefault();
                  user.Is_Email = true;
                  if (db.Users.Where(w => w.UserName.ToLower() == user.Email.ToLower()).FirstOrDefault() != null)
                     return new ServiceResult { Code = ERROR_CODE.ERROR_510_DATA_DUPLICATE, Msg = Resource.Email + " " + Resource.Is_Duplicated_Lower, Field = Resource.Employee };

                  var guid = Guid.NewGuid().ToString();
                  while (db.Users.Where(w => w.Id == guid).FirstOrDefault() != null)
                  {
                     guid = Guid.NewGuid().ToString();
                  }

                  db.Users.Add(new ApplicationUser() { Id = guid, UserName = user.Email.ToLower() });

                  var ApplicationUser_Id = guid;
                  User_Authentication authen = new User_Authentication()
                  {                     
                     Create_By = user.Create_By,
                     Create_On = user.Create_On,
                     Update_By = user.Update_By,
                     Update_On = user.Update_On,
                     Email_Address = user.Email,
                     Is_Email = true,
                     ApplicationUser_Id = ApplicationUser_Id,
                     Company = pCompany
                  };

                  //GENERATE ACTIVATION CODE
                  String code;
                  do
                  {
                     code = "A" + randomString(40);
                  }
                  while (!validateActivationCode(db, code));

                  var detail = pCompany.Company_Details.FirstOrDefault();
                  if (detail.Company_Level == Companylevel.Mainmaster)
                  {
                     authen.User_Assign_Role.Add(new User_Assign_Role() { User_Role_ID = Role.ROLE_MAIN_MASTER_ADMIN });
                     authen.User_Assign_Role.Add(new User_Assign_Role() { User_Role_ID = Role.ROLE_FRANCHISE_ADMIN });
                     authen.User_Assign_Role.Add(new User_Assign_Role() { User_Role_ID = Role.ROLE_WHITE_LABEL_ADMIN });
                  }
                  else if (detail.Company_Level == Companylevel.Franchise)
                  {
                     authen.User_Assign_Role.Add(new User_Assign_Role() { User_Role_ID = Role.ROLE_FRANCHISE_ADMIN });
                     authen.User_Assign_Role.Add(new User_Assign_Role() { User_Role_ID = Role.ROLE_WHITE_LABEL_ADMIN });
                  }
                  else if (detail.Company_Level == Companylevel.Whitelabel)
                  {
                     authen.User_Assign_Role.Add(new User_Assign_Role() { User_Role_ID = Role.ROLE_WHITE_LABEL_ADMIN });
                  }
                  authen.User_Assign_Role.Add(new User_Assign_Role() { User_Role_ID = Role.ROLE_CUSTOMER_ADMIN });

                  Activation_Link activation_link = new Activation_Link()
                  {
                     Create_By = user.Create_By,
                     Create_On = user.Create_On,
                     Update_By = user.Update_By,
                     Update_On = user.Update_On,

                     Activation_Code = code,
                     //SET Time_Limit to activate within LINK_TIME_LIMIT
                     Time_Limit = currentdate.AddHours(LINK_TIME_LIMIT),
                  };

                    
                  var emp = new Employee_Profile();
                  var pattern = new Employee_No_Pattern();
                  pattern.Current_Running_Number = 1;
                  pattern.Select_Year = true;
                  pattern.Year_4_Digit = true;
                  emp.Employee_No = currentdate.Year.ToString() + "-" + pattern.Current_Running_Number.Value.ToString("000000");
                  pattern.Current_Running_Number = 2;

                  pCompany.Employee_No_Pattern.Add(pattern);                   
                  
                  if (pCompany.Subscriptions != null)
                  {
                     foreach (var row in pCompany.Subscriptions)
                     {
                        var pages = db.Pages.Where(w => w.Module_Detail_ID == row.Module_Detail_ID).Select(s => s.Page_ID);
                        var roles = db.Page_Role
                            .Where(w => pages.Contains(w.Page_ID.Value) &
                                w.User_Role_ID != Role.ROLE_MAIN_MASTER_ADMIN &
                                w.User_Role_ID != Role.ROLE_FRANCHISE_ADMIN &
                                w.User_Role_ID != Role.ROLE_WHITE_LABEL_ADMIN &
                                w.User_Role_ID != Role.ROLE_CUSTOMER_ADMIN)
                            .Select(s => s.User_Role_ID)
                            .Distinct();

                        foreach (var roleID in roles)
                        {
                           authen.User_Assign_Role.Add(new User_Assign_Role() { User_Role_ID = roleID });
                        }

                        var ua = new User_Assign_Module();
                        ua.Subscription = row;
                        user.User_Assign_Module.Add(ua);
                     }
                  }                  
                   
                  //-------------Add leave Defaul---------------//               
                  //Added by sun 18-08-2015
                  var lDefaults = db.Leave_Default;
                  foreach (var crow in lDefaults)
                  {
                     Leave_Config leaveConfig = new Leave_Config();
                     leaveConfig.Leave_Name = crow.Leave_Name;
                     leaveConfig.Leave_Description = crow.Leave_Description;
                     leaveConfig.Bring_Forward = crow.Bring_Forward;
                     leaveConfig.Bring_Forward_Percent = crow.Bring_Forward_Percent;
                     leaveConfig.Upload_Document = crow.Upload_Document;
                     leaveConfig.Deduct_In_Payroll = crow.Deduct_In_Payroll;
                     leaveConfig.Months_To_Expiry = crow.Months_To_Expiry;
                     leaveConfig.Allowed_Probation = crow.Allowed_Probation;
                     leaveConfig.Bring_Forward_Days = crow.Bring_Forward_Days;
                     leaveConfig.Is_Bring_Forward_Days = crow.Is_Bring_Forward_Days;
                     leaveConfig.Is_Default = crow.Is_Default;
                     leaveConfig.Flexibly = crow.Flexibly;
                     leaveConfig.Continuously = crow.Continuously;
                     leaveConfig.Valid_Period = crow.Valid_Period;
                     leaveConfig.Type = crow.Type;
                     leaveConfig.Create_By = user.Create_By;
                     leaveConfig.Create_On = user.Create_On;
                     leaveConfig.Update_By = user.Update_By;
                     leaveConfig.Update_On = user.Update_On;

                     //Added by sun 10-09-2016
                     leaveConfig.Is_Accumulative = crow.Is_Accumulative;

                     if (crow.Type != LeaveConfigType.Normal)
                     {
                        if (crow.Leave_Default_Child_Detail.Count != 0)
                        {
                           foreach (var lconchde in crow.Leave_Default_Child_Detail)
                           {
                              var lconchdes = new Leave_Config_Child_Detail()
                              {
                                 Residential_Status = lconchde.Residential_Status,
                                 Default_Leave_Amount = lconchde.Default_Leave_Amount,
                                 Period = lconchde.Period,

                              };
                              leaveConfig.Leave_Config_Child_Detail.Add(lconchdes);
                           }
                        }
                     }
                     else
                     {
                        if (crow.Leave_Default_Detail.Count != 0)
                        {
                           foreach (var lconde in crow.Leave_Default_Detail)
                           {
                              var lcondes = new Leave_Config_Detail()
                              {
                                 Default_Leave_Amount = lconde.Default_Leave_Amount,
                                 Year_Service = lconde.Year_Service,
                                 Bring_Forward_Days = lconde.Bring_Forward_Days,
                                 Is_Bring_Forward_Percent = lconde.Is_Bring_Forward_Percent,
                                 Group_ID = 1
                              };
                              leaveConfig.Leave_Config_Detail.Add(lcondes);
                           }
                        }
                     }

                     if (crow.Leave_Default_Condition.Count != 0)
                     {
                        foreach (var lconco in crow.Leave_Default_Condition)
                        {
                           var lconcos = new Leave_Config_Condition()
                           {
                              Lookup_Data_ID = lconco.Lookup_Data_ID,
                           };
                           leaveConfig.Leave_Config_Condition.Add(lconcos);
                        }
                     }
                     pCompany.Leave_Config.Add(leaveConfig);
                  }
                   //Added by Moet
                  var c = GetCountry(detail.Country_ID);
                  //var lstPH = CSIUtil.readCSIFile(c.Description, DateTime.Now.Year);
                  //if (lstPH.Count > 0)
                  // {
                  //     foreach (var h in lstPH)
                  //     {
                  //         //Console.WriteLine(h.Holiday_Name + ": " + h.Holiday_Date);
                  //         var ph = new Holiday_Config();
                  //         ph.Name = h.Holiday_Name;
                  //         ph.Start_Date = DateTime.Parse(h.Holiday_Date);
                  //         ph.End_Date = DateTime.Parse(h.Holiday_Date);
                  //         ph.Record_Status = Resource.Active;
                  //         ph.Create_By = user.Create_By;
                  //         ph.Create_On = user.Create_On;
                  //         pCompany.Holiday_Config.Add(ph);
                  //     }   
                  // }

                   //Add Default Branch                  
                    var branch = new Branch();                  
                    branch.Branch_Code = Resource.Default_Branch;
                    branch.Branch_Name = Resource.Default_Branch;
                    branch.Branch_Desc = "";
                    branch.Record_Status = Resource.Active;
                    branch.Create_By = user.Create_By;
                    branch.Create_On = user.Create_On;
                    pCompany.Branches.Add(branch);

                    var gService = new GlobalLookupService();
                   //Add attachment type
                    var eAtt = new Global_Lookup_Data();
                    eAtt.Def_ID = 30;
                    eAtt.Name = "Resume";
                    eAtt.Description = "Resume";
                    eAtt.Record_Status = Resource.Active;
                    eAtt.Create_By = "System";
                    eAtt.Create_On = user.Create_On;
                    pCompany.Global_Lookup_Data.Add(eAtt);

                   //Add Default Department                    
                    var gLookupDept = gService.LstLookUpData(SystemDefaultSetup.Department, RecordStatus.Default);
                    if (gLookupDept != null)
                    {
                        foreach (var d in gLookupDept)
                        {
                            var dp = new Department();
                            dp.Name = d.Name;                            
                            dp.Record_Status = Resource.Active;
                            dp.Create_By = user.Create_By;
                            dp.Create_On = user.Create_On;
                            pCompany.Departments.Add(dp);
                        }
                    }                    
                   //Add Default Designation
                    var gLookupDesignation = gService.LstLookUpData(SystemDefaultSetup.Designation,RecordStatus.Default);
                    if (gLookupDesignation != null)
                    {                        
                        foreach (var d in gLookupDesignation)
                        {
                            var ds = new Designation();
                            ds.Name = d.Name;
                            ds.Record_Status = Resource.Active;
                            ds.Create_By = user.Create_By;
                            ds.Create_On = user.Create_On;
                            pCompany.Designations.Add(ds);
                        }
                    }
                    // Add default working days
                    var working = new Working_Days();
                    working.Days = 5;
                    var startTime = "09:00";
                    var endTime = "18:00";
                   // working.ST_Sun_Time = DateUtil.ToTime(startTime);
                    working.ST_Mon_Time = DateUtil.ToTime(startTime);
                    working.ST_Tue_Time = DateUtil.ToTime(startTime);
                    working.ST_Wed_Time = DateUtil.ToTime(startTime);
                    working.ST_Thu_Time = DateUtil.ToTime(startTime);
                    working.ST_Fri_Time = DateUtil.ToTime(startTime);
                    //working.ST_Sat_Time = DateUtil.ToTime(startTime);
                    //working.ST_Lunch_Time = DateUtil.ToTime(model.ST_Lunch_Time);
                    //working.ET_Sun_Time = DateUtil.ToTime(endTime);
                    working.ET_Mon_Time = DateUtil.ToTime(endTime);
                    working.ET_Tue_Time = DateUtil.ToTime(endTime);
                    working.ET_Wed_Time = DateUtil.ToTime(endTime);
                    working.ET_Thu_Time = DateUtil.ToTime(endTime);
                    working.ET_Fri_Time = DateUtil.ToTime(endTime);
                    //working.ET_Sat_Time = DateUtil.ToTime(endTime);
                    //working.ET_Lunch_Time = DateUtil.ToTime(model.ET_Lunch_Time);
                    working.CL_Sun = false;
                    working.CL_Mon = true;
                    working.CL_Tue = true;
                    working.CL_Wed = true;
                    working.CL_Thu = true;
                    working.CL_Fri = true;
                    working.CL_Sat = false;
                    working.CL_Lunch = false;
                    working.Create_By = user.Create_By;
                    working.Create_On = user.Create_On;
                    pCompany.Working_Days.Add(working);                                        

                    // Add Default Expense Category
                    var gExpCat = gService.LstLookUpData(SystemDefaultSetup.Expense_Category, RecordStatus.Default);
                    if (gExpCat != null)
                    {
                        foreach (var cat in gExpCat)
                        {
                            var ec = new Expenses_Category();
                            ec.Category_Name = cat.Name;
                            ec.Category_Description = cat.Name;
                            ec.Record_Status = Resource.Active;
                            ec.Create_By = user.Create_By;
                            ec.Create_On = user.Create_On;
                            pCompany.Expenses_Category.Add(ec);

                            var LstType = gService.LstDefaultExpenseType(cat.Lookup_Data_ID);
                            foreach (var et in LstType)
                            {
                                var expType = new Expenses_Config();
                                expType.Expenses_Name = et.Expense_Type_Name;
                                expType.Expenses_Description = et.Expense_Type_Desc;
                                expType.Record_Status = Resource.Active;
                                expType.Create_By = user.Create_By;
                                expType.Create_On = user.Create_On;
                                pCompany.Expenses_Config.Add(expType);
                            }
                        }
                    }
                    
                  //--------------------------------//

                  //Added by sun 13-10-2015                      
                  emp.Create_By = user.Create_By;
                  emp.Create_On = user.Create_On;
                  emp.Update_By = user.Update_By;
                  emp.Update_On = user.Update_On;

                  // populate user transaction
                  var user_tran = new User_Transactions();
                  user_tran.Activate_On = user.Create_On;
                  user_tran.Activate_By = user.Create_By;
                  user.User_Transactions.Add(user_tran);

                  authen.Activation_Link.Add(activation_link);

                  user.User_Authentication = authen;
                  user.Employee_Profile.Add(emp);

                  db.Companies.Add(pCompany);
                  db.SaveChanges();
                    
                  //Payroll Arppoval work flow
                  var pService = new PayrollService();
                  var prg = new PRG();
                  prg.Company_ID = Convert.ToInt16(pCompany.Company_ID);
                  prg.Create_On = currentdate;
                  prg.Create_By = "Auto";
                  prg.Name = "Auto Group";
                  var result = pService.InsertPRG(prg);
                  if (result.Code == ERROR_CODE.SUCCESS)
                  {
                      var eService = new EmployeeService();
                      //var emp = eService.GetEmployeeProfileByProfileID(user.Profile_ID);

                      var prel = new PREL();
                      prel.Employee_Profile_ID = emp.Employee_Profile_ID;
                      prel.PRG_ID = prg.PRG_ID;
                      prel.Create_By = "Front";
                      prel.Create_On = StoredProcedure.GetCurrentDate();
                      result = pService.InsertPREL(prel);
                      if (result.Code == ERROR_CODE.SUCCESS)
                      {
                          var pral = new PRAL();
                          pral.Employee_Profile_ID = emp.Employee_Profile_ID;
                          pral.PRG_ID = prg.PRG_ID;
                          pral.Create_By = "Front";
                          pral.Create_On = StoredProcedure.GetCurrentDate();
                          result = pService.InsertPRAL(pral);
                      }
                  }

                   //CPF & Donations
                  int curYear = DateTime.Now.Year;
                  DateTime firstDay = new DateTime(curYear, 1, 1);
                  var p = pService.GetCPFFormulas(curYear);
                  if(p != null)
                  {
                      var pLatest = p.OrderByDescending(o => o.CPF_Formula_ID).FirstOrDefault();
                      if (pLatest == null)
                         pLatest =pService. GetLatestCPFFormulas();
                      if (pLatest != null)
                      {
                         Selected_CPF_Formula formula = new Selected_CPF_Formula();
                         formula.CPF_Formula_ID = pLatest.CPF_Formula_ID;
                         formula.Company_ID = Convert.ToInt16(pCompany.Company_ID);
                         formula.Effective_Date = firstDay;
                         formula.Create_By = "Auto";
                         formula.Create_On = currentdate;
                         result = pService.InsertSelectedCPFFormula(formula);   
                      }
                                         
                  }

                  var dFormula = pService.GetDonationFormulas();
                  if(dFormula != null)
                  {
                      foreach(var d in dFormula)
                      {
                          var formula = new Selected_Donation_Formula();
                          formula.Donation_Formula_ID = d.Donation_Formula_ID;
                          formula.Company_ID = Convert.ToInt16(pCompany.Company_ID);
                          formula.Effective_Date = firstDay;
                          formula.Create_By = "Auto";
                          formula.Create_On = currentdate;
                          result = pService.InsertSelectedDonationFormula(formula);
                      }
                  }
                  
                  return new ServiceResult() { Code = ERROR_CODE.SUCCESS, Msg = new Success().getSuccess(ERROR_CODE.SUCCESS_CREATE), Field = Resource.Company, Object = code };
               }
            }
         }
         catch
         {

         }

         return new ServiceResult() { Code = ERROR_CODE.ERROR_503_INSERT_ERROR, Msg = new Error().getError(ERROR_CODE.ERROR_503_INSERT_ERROR), Field = Resource.Company };
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

      public ServiceResult UpdateCompany(Company_Details pCompanyDetail)
      {
         try
         {
            using (var db = new SBS2DBContext())
            {

               if (pCompanyDetail != null)
               {
                  var currentCompany = (from a in db.Company_Details
                                        where a.Effective_Date == pCompanyDetail.Effective_Date
                                        & a.Company_ID == pCompanyDetail.Company_ID
                                        select a).FirstOrDefault();

                  if (currentCompany != null)
                  {
                     //Update
                     db.Entry(currentCompany).CurrentValues.SetValues(pCompanyDetail);
                  }
                  db.SaveChanges();
               }
               return new ServiceResult() { Code = ERROR_CODE.SUCCESS, Msg = new Success().getSuccess(ERROR_CODE.SUCCESS_EDIT), Field = Resource.Company };
            }
         }
         catch
         {
            return new ServiceResult() { Code = ERROR_CODE.ERROR_504_UPDATE_ERROR, Msg = new Error().getError(ERROR_CODE.ERROR_504_UPDATE_ERROR), Field = Resource.Company };
         }
      }


      #region Company Logo
      public Company_Logo GetLogo(Nullable<int> pCompanyId)
      {
         using (var db = new SBS2DBContext())
         {
            return (from a in db.Company_Logo
                    where a.Company_ID == pCompanyId
                    select a).FirstOrDefault();

         }

      }

      public bool InsertCompanyLogo(Company_Logo pCompanyLogo)
      {
         try
         {
            using (var db = new SBS2DBContext())
            {
               if (pCompanyLogo != null)
               {
                  var guid = Guid.NewGuid();
                  while (db.Company_Logo.Where(w => w.Company_Logo_ID == guid).FirstOrDefault() != null)
                  {
                     guid = Guid.NewGuid();
                  }
                  pCompanyLogo.Company_Logo_ID = guid;
                  db.Company_Logo.Add(pCompanyLogo);
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

      public bool UpdateCompanyLogo(Company_Logo pCompanyLogo)
      {
         try
         {
            using (var db = new SBS2DBContext())
            {

               if (pCompanyLogo != null)
               {
                  Company_Logo q = (from a in db.Company_Logo
                                    where (a.Company_Logo_ID == pCompanyLogo.Company_Logo_ID)
                                    select a).FirstOrDefault();

                  if (q != null)
                  {
                     //Update
                     db.Entry(q).CurrentValues.SetValues(pCompanyLogo);
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
      #endregion



      public bool InsertCompanyLogo(Company_Logo pCompanyLogo, int pCompanyDetailID)
      {
         try
         {
            using (var db = new SBS2DBContext())
            {
               if (pCompanyLogo != null)
               {
                  Company_Details q = (from a in db.Company_Details
                                       where (a.Company_Detail_ID == pCompanyDetailID)
                                       select a).FirstOrDefault();
                  //Insert
                  pCompanyLogo.Company_Logo_ID = Guid.NewGuid();
                  if (q != null)
                  {
                     pCompanyLogo.Company_ID = q.Company_ID;
                  }
                  db.Company_Logo.Add(pCompanyLogo);

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

      public List<Subscription> LstSubscription(Nullable<int> pCompanyId)
      {

         using (var db = new SBS2DBContext())
         {
            return db.Subscriptions.Where(w => w.Company_ID == pCompanyId)
                .Include(i => i.User_Assign_Module)
                .Include(i => i.SBS_Module_Detail)
                .Include(i => i.SBS_Module_Detail.SBS_Module)
                .OrderBy(o => o.SBS_Module_Detail.Module_Detail_ID)
                .ToList();
         }
      }

      public SBS_Module_Detail GetModule(Nullable<int> pModuleDetailID)
      {

         using (var db = new SBS2DBContext())
         {
            return db.SBS_Module_Detail
                .Include(i => i.SBS_Module)
                .Where(w => w.Module_Detail_ID == pModuleDetailID).FirstOrDefault();
         }
      }

      public List<SBS_Module_Detail> LstModule()
      {

         using (var db = new SBS2DBContext())
         {
            return db.SBS_Module_Detail
                .Include(i => i.SBS_Module)
                .OrderBy(o => o.Module_Detail_Name)
                .ToList();
         }
      }

      public Subscription GetSubscription(Nullable<int> pSubID)
      {
         using (var db = new SBS2DBContext())
         {
            return db.Subscriptions
                .Where(w => w.Subscription_ID == pSubID)
                .Include(i => i.SBS_Module_Detail)
                .Include(i => i.User_Assign_Module)
                .Include(i => i.User_Assign_Module.Select(s => s.User_Profile))
                .Include(i => i.User_Assign_Module.Select(s => s.User_Profile.User_Authentication))
                .OrderByDescending(o => o.Start_Date).FirstOrDefault();
         }
      }

      public ServiceResult UpdateUserAssign(Nullable<int> pSupscriptionID, int[] users)
      {
         try
         {
            using (var db = new SBS2DBContext())
            {

               var sub = db.Subscriptions.Where(w => w.Subscription_ID == pSupscriptionID).FirstOrDefault();
               if (sub != null)
               {
                  var currentUsers = sub.User_Assign_Module;
                  if (users != null)
                  {

                     var currentUserIDs = currentUsers.Select(s => s.Profile_ID);
                     var removeAssign = new List<User_Assign_Module>();
                     foreach (var row in currentUsers)
                     {
                        if (!users.Contains(row.Profile_ID.Value))
                        {
                           removeAssign.Add(row);
                        }
                     }
                     if (removeAssign.Count > 0)
                     {
                        db.User_Assign_Module.RemoveRange(removeAssign);
                     }

                     foreach (var row in users)
                     {
                        if (!currentUserIDs.Contains(row))
                        {
                           db.User_Assign_Module.Add(new User_Assign_Module() { Subscription_ID = pSupscriptionID, Profile_ID = row });
                        }
                     }
                  }
                  else
                  {
                     if (currentUsers != null)
                     {
                        db.User_Assign_Module.RemoveRange(currentUsers);
                     }

                  }
               }
               db.SaveChanges();


               return new ServiceResult() { Code = ERROR_CODE.SUCCESS, Msg = new Success().getSuccess(ERROR_CODE.SUCCESS_EDIT), Field = Resource.Assign_User };
            }
         }
         catch
         {
            return new ServiceResult() { Code = ERROR_CODE.ERROR_504_UPDATE_ERROR, Msg = new Error().getError(ERROR_CODE.ERROR_504_UPDATE_ERROR), Field = Resource.Assign_User };
         }
      }

      public ServiceResult InsertSubscription(Subscription[] pSupscriptions)
      {
         try
         {
            var currentdate = StoredProcedure.GetCurrentDate();
            using (var db = new SBS2DBContext())
            {
               if (pSupscriptions != null)
               {
                  foreach (var row in pSupscriptions)
                  {

                     //if (row.Company_ID != null)
                     //{

                     //    var ex = db.Subscriptions.Where(w => w.Company_ID == row.Company_ID && w.Module_Detail_ID == row.Module_Detail_ID).ToList();
                     //    if (ex != null)
                     //    {
                     //        //if (ex.da((w => w.No_Of_Users == row.No_Of_Users);
                     //        //{

                     //        //}
                     //    }
                     //    else
                     //    {
                     //        row.Start_Date = currentdate;
                     //        db.Subscriptions.Add(row);
                     //    }
                     //    continue;
                     //}


                     if (row.Module_Detail_ID.HasValue && row.Module_Detail_ID.Value > 0)
                     {
                        row.Create_On = currentdate;
                        row.Update_On = currentdate;
                        row.Start_Date = currentdate;
                        db.Subscriptions.Add(row);
                     }
                  }
                  db.SaveChanges();

               }
               return new ServiceResult() { Code = ERROR_CODE.SUCCESS, Msg = new Success().getSuccess(ERROR_CODE.SUCCESS_EDIT), Field = Resource.Subscription };
            }
         }
         catch
         {
            return new ServiceResult() { Code = ERROR_CODE.ERROR_504_UPDATE_ERROR, Msg = new Error().getError(ERROR_CODE.ERROR_504_UPDATE_ERROR), Field = Resource.Subscription };
         }
      }

      //Added by Nay on 08-Sept-2015
      //to check current company is using PAT service or not
      public string usePAT_FileGenerate(Nullable<int> compID)
      {
         using (var db = new SBS2DBContext())
         {
            var q = (from a in db.Subscriptions
                     join b in db.SBS_Module_Detail
                         on a.Module_Detail_ID equals b.Module_Detail_ID
                     where a.Company_ID == compID
                     select a).ToList();
            if (q != null && q.Count() > 0)
               return "Y";
            else
               return "N";
         }
      }

      public ServiceResult InsertCompanyMailConfig(Company_Mail_Config pMailConfig)
      {
          try
          {
              var currentdate = StoredProcedure.GetCurrentDate();

              using (var db = new SBS2DBContext())
              {

                  db.Entry(pMailConfig).State = EntityState.Added;
                  db.SaveChanges();

                  return new ServiceResult() { Code = ERROR_CODE.SUCCESS, Msg = "Mail Config successfully saved.", Field = "Mail Config" };
              }
          }
          catch
          {
              return new ServiceResult() { Code = ERROR_CODE.ERROR_503_INSERT_ERROR, Msg = new Error().getError(ERROR_CODE.ERROR_503_INSERT_ERROR), Field = "Message" };
          }
      }

      public ServiceResult UpdateCompanyMailConfig(Company_Mail_Config pMailConfig)
      {
          try
          {
              using (var db = new SBS2DBContext())
              {
                  var current = db.Company_Mail_Config
                      .FirstOrDefault(m => m.Config_ID == pMailConfig.Config_ID);

                  if (current != null)
                  {
                      current.SMTP_Server = pMailConfig.SMTP_Server;
                      current.SMTP_Port = pMailConfig.SMTP_Port;
                      current.Email = pMailConfig.Email;
                      current.Password = pMailConfig.Password;
                      current.SSL = pMailConfig.SSL;

                      db.Entry(current).State = EntityState.Modified;
                      db.SaveChanges();
                  }

                  return new ServiceResult() { Code = ERROR_CODE.SUCCESS, Msg = "Message Layout successfully updated.", Field = "Message" };
              }
          }
          catch
          {
              return new ServiceResult() { Code = ERROR_CODE.ERROR_503_INSERT_ERROR, Msg = new Error().getError(ERROR_CODE.ERROR_503_INSERT_ERROR), Field = "Message" };
          }
      }

      public Company_Mail_Config GetCompanyMailConfig(int pCompanyID)
      {
          using (var db = new SBS2DBContext())
          {
              return db.Company_Mail_Config
                  .Where(w => w.Company_ID == pCompanyID).FirstOrDefault();
          }
      }

    public List<Company_Details> getPostPaidCompanies()
          {
              using (var db = new SBS2DBContext())
              {
                  var cd = db.Company_Details.Where(w => w.Is_PostPaid == true).ToList();
                  return cd;
              }
          }
   }

}

