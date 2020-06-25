using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SBSModel.Models;
using SBSModel.Common;
using Authentication.Models;
using System.Diagnostics;
using SBSResourceAPI;
using System.IO;
using iTextSharp.text.pdf;
using iTextSharp.text;
using SBSWorkFlowAPI.Constants;
using SBSWorkFlowAPI.Models;
using SBSWorkFlowAPI.ModelsAndService;
using iTextSharp.text.html.simpleparser;

namespace Authentication.Controllers
{
   [Authorize]
   [AllowAuthorized]
   public class HomeController : ControllerBase
   {

      [HttpGet]
      [AllowAuthorized]
      public ActionResult Index()
      {
         var stringURL = string.Empty;
         if (Session["URL_CURRENT"] == null)
         {
            //Added by Moet to redirect to Landing page if they haven't set up Branch, Department and Emp no. Pattern
            var userlogin = UserSession.getUser(HttpContext);
            var eService = new EmployeeService();
            var criteria = new EmployeeCriteria() { Company_ID = userlogin.Company_ID };
            var empList = eService.LstEmployeeProfile(criteria);
            var loginAttempt = userlogin.User_Authentication.Login_Attempt;
            bool isAdmin = false;
            foreach (User_Assign_Role urole in userlogin.User_Authentication.User_Assign_Role.ToList())
            {
               int AuthUserRole = 4;
               if (urole.User_Role_ID.Value == AuthUserRole)
               {
                  isAdmin = true;
                  break;
               }
            }
            var routeURL = "Home/Index";
            if (empList.Record_Count == 1 && isAdmin == true && loginAttempt == 1)
            {
               routeURL = "Home/Landing_New";
            }

            if (AppSetting.IsLive == "true")
            {
               stringURL = AppSetting.SERVER_NAME + "hrsbs2/" + routeURL;
            }
            if (AppSetting.IsDemo == "true")
            {
               stringURL = AppSetting.SERVER_NAME + "hrsbs2/" + routeURL;
            }
            if (AppSetting.IsStaging == "true")
            {
               stringURL = AppSetting.SERVER_NAME + "hrsbs2-Staging/" + routeURL;
            }
            if (AppSetting.IsLocal == "true")
            {
               var model = new DashBoardViewModel();
               model = InitDashboard();
               if (model.ErrorCode != 0)
                  return errorPage(model.ErrorCode);
               if (model.redirectURL != null)
                  return Redirect(model.redirectURL);
               return View(model);
            }
         }
         else
         {
            //stringURL = Session["URL_CURRENT"].ToString();
            //fixed error (temporarily) by sun 05-11-2016
            var routeURL = "Home/Index";
            if (AppSetting.IsLive == "true" || AppSetting.IsDemo == "true")
            {
               stringURL = AppSetting.SERVER_NAME + "hrsbs2/" + routeURL;
            }
            else if (AppSetting.IsStaging == "true")
            {
               stringURL = AppSetting.SERVER_NAME + "hrsbs2-Staging/" + routeURL;
            }
            else if (AppSetting.IsLocal == "true")
            {
               var model = new DashBoardViewModel();
               model = InitDashboard();
               if (model.ErrorCode != 0)
                  return errorPage(model.ErrorCode);
               if (model.redirectURL != null)
                  return Redirect(model.redirectURL);
               return View(model);
            }
         }
         return Redirect(stringURL);
      }

      private Authentication.Models.DashBoardViewModel InitDashboard()
      {
         var model = new DashBoardViewModel();
         Debug.WriteLine("'***********APP DEBUG***********' " + DateTime.Now + "-Start Controller Home Index");
         var userlogin = UserSession.getUser(HttpContext);
         if (userlogin == null)
         {
            model.ErrorCode = ERROR_CODE.ERROR_401_UNAUTHORIZED;
            return model;
         }
         model.userProfile = userlogin;
         //return errorPage(ERROR_CODE.ERROR_401_UNAUTHORIZED);   

         var comService = new CompanyService();
         var lService = new LeaveService();
         var eService = new ExpenseService();
         var empService = new EmployeeService();
         var pService = new PayrollService();
         var hService = new EmploymentHistoryService();
         var leaveService = new LeaveService();
         var uService = new UserService();
         var sService = new SubscriptionService();

         var com = comService.GetCompany(userlogin.Company_ID);
         if (com == null)
         {
            model.ErrorCode = ERROR_CODE.ERROR_401_UNAUTHORIZED;
            return model;
         }
         //return errorPage(ERROR_CODE.ERROR_401_UNAUTHORIZED);

         model.Emp = empService.GetEmployeeProfileByProfileID(userlogin.Profile_ID);
         if (model.Emp == null)
         {
            model.ErrorCode = ERROR_CODE.ERROR_511_DATA_NOT_FOUND;
            return model;
         }
         //return errorPage(ERROR_CODE.ERROR_511_DATA_NOT_FOUND);

         if (com.Currency != null)
            model.Currency_Code = com.Currency.Currency_Code;

         model.Emp_Hist = hService.GetCurrentEmploymentHistory(model.Emp.Employee_Profile_ID);

         var currentdate = StoredProcedure.GetCurrentDate();

         //To validate for the overdue
         //Added by Moet on 4 Sep
         bool isAdmin = false;
         if (uService.IsOverdue(userlogin.Company_ID.Value) == true)
         {
            foreach (User_Assign_Role urole in userlogin.User_Authentication.User_Assign_Role.ToList())
            {
               int AuthUserRole = 4;
               if (urole.User_Role_ID.Value == AuthUserRole)
               {
                  isAdmin = true;
                  var stringURL = string.Empty;
                  if (AppSetting.IsLive == "true")
                  {
                     stringURL = AppSetting.SERVER_NAME + "Authensbs2/Subscription/BillingReport";
                  }
                  if (AppSetting.IsStaging == "true")
                  {
                     stringURL = AppSetting.SERVER_NAME + "Authensbs2-Staging/Subscription/BillingReport";
                  }
                  if (AppSetting.IsLocal == "true")
                  {
                     stringURL = "http://localhost:55581/Authensbs2-Staging/Subscription/BillingReport";
                     //return RedirectToAction("BillingReport", "Subscription");          
                  }
                  model.redirectURL = stringURL;
                  return model;
               }
            }
            if (!isAdmin)
            {
               model.ErrorCode = ERROR_CODE.ERROR_702_OVERDUE_ERROR;
               return model;
            }
         }

         //Added by sun 21-004-2016
         var page = new List<string>();
         page.Add("/Employee/Employee");
         page.Add("/Leave/Application");
         page.Add("/Expenses/Application");
         page.Add("/Payroll/Payroll");
         var right = base.validatePageRight(page);
         foreach (var r in right)
         {
            if (r.Value.Contains(UserSession.RIGHT_A))
            {
               if (r.Key == "/Leave/Application")
                  model.Display_Leave = true;
               else if (r.Key == "/Expenses/Application")
                  model.Display_Expenses = true;
               else if (r.Key == "/Payroll/Payroll")
                  model.Display_Payroll = true;
            }
         }
         if (model.Display_Leave)
         {

            Nullable<int> relationshipID = null;
            var child = model.Emp.Relationships.Where(w => w.Child_Type == ChildType.OwnChild | w.Child_Type == ChildType.AdoptedChild).OrderByDescending(o => o.DOB).FirstOrDefault();
            if (child != null)
               relationshipID = child.Relationship_ID;

            Debug.WriteLine("'***********APP DEBUG***********' " + DateTime.Now + "-Start Get Leave Data");

            var hist = hService.GetCurrentEmploymentHistory(model.Emp.Employee_Profile_ID);
            if (hist != null)
            {
               var yearservice = 0;
               var hiredDate = currentdate;
               var firsthist = hService.GetFirstEmploymentHistory(model.Emp.Employee_Profile_ID);
               if (firsthist != null && firsthist.Effective_Date.HasValue)
               {
                  hiredDate = firsthist.Effective_Date.Value;
                  var workspan = (currentdate.Date - hiredDate.Date);
                  yearservice = NumUtil.ParseInteger(Math.Floor((workspan.TotalDays + 1) / 365));
               }

               Debug.WriteLine("'***********APP DEBUG***********' " + DateTime.Now + "-Start Lst Leave Doc");
               model.LeaveList = lService.LstLeaveApplicationDocument(userlogin.Company_ID, pProfileID: userlogin.Profile_ID);
               Debug.WriteLine("'***********APP DEBUG***********' " + DateTime.Now + "-End Lst Leave Doc");
               var maincri = new LeaveTypeCriteria()
               {
                  Company_ID = userlogin.Company_ID,
                  Profile_ID = userlogin.Profile_ID,
                  firsthist = firsthist,
                  Year_Service = yearservice,
                  Emp = model.Emp,
                  Ignore_Generate = true,
                  Year = currentdate.Year,
               };
               if (model.LeaveList.Count == 0)
               {
                  /*first time setting*/
                  var calisexsist = lService.LeaveCalIsExist(model.Emp.Employee_Profile_ID);
                  if (!calisexsist)
                     maincri.Ignore_Generate = false;
               }
               Debug.WriteLine("'***********APP DEBUG***********' " + DateTime.Now + "-Start Leave Calculate");
               model.LeaveBalanceList = lService.LstAndCalulateLeaveType(maincri);
               model.Working_Days = lService.GetWorkingDayOfWeek(userlogin.Company_ID, userlogin.Profile_ID);
               Debug.WriteLine("'***********APP DEBUG***********' " + DateTime.Now + "-End Leave Calculate");
            }
            Debug.WriteLine("'***********APP DEBUG***********' " + DateTime.Now + "-End Get Leave Data");

            //Added by sun 01-12-2015
            var bankHolidays = new List<Holidays>();
            Debug.WriteLine("'***********APP DEBUG***********' " + DateTime.Now + "-Start Get Holidays Data");
            var holidaylist = lService.getHolidays(userlogin.Company_ID.Value);
            if (holidaylist != null)
            {
               foreach (var h in holidaylist)
               {
                  if (h.End_Date.HasValue & h.Start_Date.HasValue)
                  {
                     for (var dt = h.Start_Date.Value; dt <= h.End_Date.Value; dt = dt.AddDays(1))
                     {
                        var holiday = new Holidays()
                        {
                           date = dt,
                           name = h.Name
                        };
                        bankHolidays.Add(holiday);
                     }
                  }
                  else
                  {
                     var holiday = new Holidays()
                     {
                        date = h.Start_Date.Value,
                        name = h.Name
                     };
                     bankHolidays.Add(holiday);
                  }
               }
            }

            model.HolidayList = bankHolidays.ToArray();
            Debug.WriteLine("'***********APP DEBUG***********' " + DateTime.Now + "-End Get Holidays Data");
            Debug.WriteLine("'***********APP DEBUG***********' " + DateTime.Now + "-Start Get Leave Color");
            if (model.LeaveList != null)
            {
               var color = new List<string>();
               for (int i = 0; i <= model.LeaveList.Count; i++)
               {
                  var Col = RandomColor.GetLstCode();
                  while (color.Contains(Col.Name))
                  {
                     Col = RandomColor.GetLstCode();
                  }

                  color.Add(Col.Name);
               }
               model.Collor = color.ToArray();
            }
            Debug.WriteLine("'***********APP DEBUG***********' " + DateTime.Now + "-End Get Leave Color");
         }

         if (model.Display_Expenses)
         {
            Debug.WriteLine("'***********APP DEBUG***********' " + DateTime.Now + "-Start Get Expenses Data");
            model.ExpensesBalanceList = new List<ExpensesBalanceViewModel>();
            model.ExpensesList = eService.getExpenseApplications(userlogin.Company_ID, userlogin.Profile_ID);

            var eTypes = eService.getExpenseTypes(userlogin.Company_ID);
            if (eTypes != null)
            {
               foreach (var row in eTypes)
               {
                  var docs = model.ExpensesList.Where(w => w.Expenses_Application_Document.Where(w2 => w2.Expenses_Config_ID == row.Expenses_Config_ID).Count() > 0);
                  var amount = docs.Sum(s => s.Expenses_Application_Document.Where(w2 => w2.Expenses_Config_ID == row.Expenses_Config_ID).Select(s2 => (s2.Amount_Claiming.HasValue ? s2.Amount_Claiming.Value : 0)).Sum());
                  var totalamount = row.Expenses_Config_Detail.Select(s => (s.Amount_Per_Year.HasValue ? s.Amount_Per_Year.Value : 0)).Sum();

                  model.ExpensesBalanceList.Add(new ExpensesBalanceViewModel()
                  {
                     Expenses_Type_Name = row.Expenses_Name,
                     Amount = totalamount - amount
                  });
               }
            }
            Debug.WriteLine("'***********APP DEBUG***********' " + DateTime.Now + "-End Get Expenses Data");
         }

         if (model.Display_Payroll)
         {
            if (model.Emp != null)
            {
               model.Emp_Hist = hService.GetCurrentEmploymentHistory(model.Emp.Employee_Profile_ID);
               model.payroll = pService.GetPayrollByEmployeeID(model.Emp.Employee_Profile_ID, currentdate.Month, currentdate.Year);
            }
         }
         //Added by Moet on 16-Sep-2016
         if (Session["NoOfEmp"] != null)
         {
            model.Total_Employees = Convert.ToInt32(Session["NoOfEmp"]);
            model.Total_Payroll_Amt = Convert.ToInt32(Session["SumPayroll"]);
         }
         else
         {
            var lstEmp = empService.LstEmployeeProfile(userlogin.Company_ID);
            var cnt = 0;
            decimal T_Payroll = 0;
            foreach (var e in lstEmp)
            {
               if (e.User_Profile.User_Status == RecordStatus.Active)
               {
                  cnt++;
                  var hist = e.Employment_History.OrderByDescending(o => o.History_ID).FirstOrDefault();
                  if (hist != null)
                  {
                     // Payroll
                     var sal = NumUtil.ParseDecimal(EncryptUtil.Decrypt(hist.Basic_Salary));
                     if (sal == 0)
                        sal = NumUtil.ParseDecimal(EncryptUtil.Decrypt(EncryptUtil.Decrypt(hist.Basic_Salary)));
                     T_Payroll += sal;
                  }

               }
            }
            model.Total_Employees = cnt;
            model.Total_Payroll_Amt = T_Payroll;
            Session["NoOfEmp"] = cnt;
            Session["SumPayroll"] = T_Payroll;
         }

         //Added by Moet on 16-Sep-2016              
         if (Session["DiskSpace"] != null)
         {
            model.Diskspace_Usage = NumUtil.ParseDecimal(Session["DiskSpace"].ToString());
         }
         else
         {
            decimal dsize = sService.Get_DataSize_ByCompany(userlogin.Company_ID.Value);
            dsize = Math.Round(dsize / 1000, 2); // 1 MB = 1000 KB

            Session["DiskSpace"] = dsize;
            model.Diskspace_Usage = dsize;

         }

         if (Session["BillingAmt"] != null)
         {
            model.Total_Billing_Amt = NumUtil.ParseDecimal(Session["BillingAmt"].ToString());
         }
         else
         {
            decimal dBillAmt = sService.Get_BillingAmount_ByCompany(userlogin.Company_ID.Value);
            dBillAmt = Math.Round(dBillAmt, 2);
            Session["BillingAmt"] = dBillAmt;

            model.Total_Billing_Amt = dBillAmt;
         }

         Debug.WriteLine("'***********APP DEBUG***********' " + DateTime.Now + "-End Controller Home Index");

         //var yearSerivce = empService.GetYearService(model.Emp_Hist.Employee_Profile_ID);
         //model.expensesConfigList = eService.getExpenseTypes(userlogin.Company_ID.Value, model.Emp_Hist.Department_ID, model.Emp_Hist.Designation_ID, null, yearSerivce);
         //model.Expenses_Date = DateUtil.ToDisplayDate(currentdate);
         //var bal = ApplicationConfig(model.expensesConfigList[0].Expenses_Config_ID, 0, model.Expenses_Date, userlogin.Profile_ID);
         //if (bal != "")
         //{
         //    model.Balance = Convert.ToDecimal(bal);
         //}
         return model;
      }
   }
}