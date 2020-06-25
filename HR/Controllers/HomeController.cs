using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using HR.Models;
using SBSModel.Models;
using SBSModel.Common;
using SBSResourceAPI;
using System.Diagnostics;
using System.IO;
using iTextSharp.text.pdf;
using iTextSharp.text;
using SBSWorkFlowAPI.Constants;
using SBSWorkFlowAPI.Models;
using SBSWorkFlowAPI.ModelsAndService;
using iTextSharp.text.html.simpleparser;
using System.Text;


namespace HR.Controllers
{
    [Authorize]
    [AllowAuthorized]
    public class HomeController : ControllerBase
    {

        #region Landing
        [HttpGet]
        public ActionResult Landing_New()
        {

            var userlogin = UserSession.getUser(HttpContext);
            if (userlogin == null)
                return errorPage(ERROR_CODE.ERROR_401_UNAUTHORIZED);

            var model = new LandingViewModel();
            ////-------rights------------
            //RightResult rightResult = base.validatePageRight(UserSession.RIGHT_A);
            //if (rightResult.action != null)
            //    return rightResult.action;  
            //model.rights = rightResult.rights;

            var userService = new UserService();
            var comService = new CompanyService();
            var cbService = new ComboService();

            model.countryList = cbService.LstCountry(true);
            model.stateList = new List<ComboViewModel>();
            model.stateBillingList = new List<ComboViewModel>();

            var com = comService.GetCompany(userlogin.Company_ID);
            if (com == null)
                return errorPage(ERROR_CODE.ERROR_511_DATA_NOT_FOUND, Resource.Company);

            if (com != null)
            {
                var users = userService.getUsers(com.Company_ID);
                model.User_Count = users.Count();
                model.Company_Levelg = com.Company_Level;

                model.LstCompanylevel = cbService.LstCompanylevel(com.Company_Level, true);
                model.Company_ID = com.Company_ID;
                model.No_Of_Employees = com.No_Of_Employees;
                model.Company_Name = com.Name;
                model.Effective_Date = DateUtil.ToDisplayDate(com.Effective_Date);
                model.Address = com.Address;
                model.Country_ID = com.Country_ID;
                model.State_ID = com.State_ID;
                model.Zip_Code = com.Zip_Code;
                model.Billing_Address = com.Billing_Address;
                model.Billing_Country_ID = com.Billing_Country_ID;
                model.Billing_State_ID = com.Billing_State_ID;
                model.Billing_Zip_Code = com.Billing_Zip_Code;
                model.Business_Type = com.Business_Type;
                model.patUser_ID = com.patUser_ID;
                model.patPassword = com.patPassword;
                model.Fax = com.Fax;
                model.Phone = com.Phone;

                if (model.Country_ID.HasValue)
                    model.stateList = cbService.LstState(model.Country_ID.Value.ToString(), true);
                else if (model.countryList.Count() > 0)
                    model.stateList = cbService.LstState(model.countryList[0].Value, true);

                if (model.Billing_Country_ID.HasValue)
                    model.stateBillingList = cbService.LstState(model.Billing_Country_ID.Value.ToString(), true);
                else if (model.countryList.Count() > 0)
                    model.stateBillingList = cbService.LstState(model.countryList[0].Value, true);

                model.genderList = cbService.LstLookup(ComboType.Gender, com.Company_ID, false);
                model.maritalStatusList = cbService.LstLookup(ComboType.Marital_Status, com.Company_ID, false);
                model.raceList = cbService.LstLookup(ComboType.Race, com.Company_ID, false);
                model.nationalityList = cbService.LstNationality(false);
                model.residentialStatusList = cbService.LstResidentialStatus();
                model.branchList = cbService.LstBranch(com.Company_ID, true);
                model.departmentList = cbService.LstDepartment(com.Company_ID, true);
                model.desingnationList = cbService.LstDesignation(com.Company_ID, true);
                model.empTypeList = cbService.LstLookup(ComboType.Employee_Type, com.Company_ID, false);
                model.currencyList = cbService.LstCurrency(false);
                model.periodList = cbService.LstPeriod();

                model.businessCatList = cbService.LstLookup(ComboType.Business_Category, null, false);
                model.Business_Type = com.Business_Type == null ? "" : com.Business_Type;
                model.Country_ID = com.Country_ID;
                model.countryList = cbService.LstCountry(true);
                model.Company_Currency_Code = com.Currency.Currency_Code;
                model.termList = cbService.LstTerm();
                model.Basic_Salary_Unit = Term.Monthly;
            }

            //ModelState.Clear();
            return View(model);
        }
        [HttpPost]
        [AllowAuthorized]
        public ActionResult Landing_New(LandingViewModel model)
        {
            var userlogin = UserSession.getUser(HttpContext);
            if (userlogin == null)
                return errorPage(ERROR_CODE.ERROR_401_UNAUTHORIZED);

            var currentdate = StoredProcedure.GetCurrentDate();
            var comService = new CompanyService();
            var bService = new BranchService();
            var dpService = new DepartmentService();
            var cEService = new ConfigService();
            var cbService = new ComboService();

            //Company Information
            var com = comService.GetCompany(model.Company_ID);
            if (com == null)
                return errorPage(ERROR_CODE.ERROR_511_DATA_NOT_FOUND);

            if (ModelState.IsValid)
            {
                com.Name = model.Company_Name;
                com.No_Of_Employees = model.No_Of_Employees;
                com.Business_Type = model.Business_Type;

                com.Address = model.Address;
                com.Country_ID = model.Country_ID;
                com.State_ID = model.State_ID;
                com.Zip_Code = model.Zip_Code;

                com.Billing_Address = model.Billing_Address;
                com.Billing_Country_ID = model.Billing_Country_ID;
                com.Billing_State_ID = model.Billing_State_ID;
                com.Billing_Zip_Code = model.Billing_Zip_Code;

                com.Phone = model.Phone;
                com.Fax = model.Fax;
                com.Business_Type = model.Business_Type;

                com.Update_By = userlogin.User_Authentication.Email_Address;
                com.Update_On = currentdate;

                model.result = comService.UpdateCompany(com);
                if (model.result.Code == ERROR_CODE.SUCCESS)
                {
                    //Employee Info Update
                    var empService = new EmployeeService();
                    var emp = new Employee_Profile();
                    emp = empService.GetEmployeeProfileByProfileID(userlogin.Profile_ID);
                    if (emp == null)
                        return errorPage(ERROR_CODE.ERROR_16_NO_EMPLOYEE_PROFILE);

                    emp.Employee_Profile_ID = emp.Employee_Profile_ID;
                    emp.Gender = model.Gender;
                    emp.Marital_Status = model.Marital_Status;
                    emp.DOB = DateUtil.ToDate(model.DOB);
                    emp.Race = model.Race;
                    emp.Nationality_ID = model.Nationality_ID;
                    emp.Residential_Status = model.Residential_Status;
                    emp.NRIC = model.NRIC;
                    emp.Update_On = currentdate;
                    emp.Update_By = userlogin.User_Authentication.Email_Address;

                    //Employment History
                    var hist = new Employment_History();
                    if (emp.Employment_History.Count > 0)
                        hist.History_ID = emp.Employment_History.Select(w => w.History_ID).FirstOrDefault();

                    hist.Branch_ID = model.Branch_ID;
                    hist.Department_ID = model.Department_ID;
                    hist.Designation_ID = model.Designation_ID;

                    hist.Effective_Date = DateUtil.ToDate(model.Effective_Date);
                    hist.Confirm_Date = DateUtil.ToDate(model.Confirm_Date);
                    hist.Employee_Type = NumUtil.ParseInteger(model.Employee_Type);

                    hist.Currency_ID = model.Currency_ID;
                    hist.Basic_Salary = EncryptUtil.Encrypt(model.Basic_Salary);
                    hist.Basic_Salary_Unit = model.Basic_Salary_Unit;
                    hist.Days = 5;

                    hist.Update_On = currentdate;
                    hist.Update_By = userlogin.User_Authentication.Email_Address;
                    emp.Employment_History.Add(hist);

                    model.result = empService.Landing_UpdateEmployee(userlogin, emp);
                    if (model.result.Code == ERROR_CODE.SUCCESS)
                    {
                        return RedirectToAction("Index", "Home");
                    }
                }
            }

            model.Company_Currency_Code = com.Currency.Currency_Code;
            model.businessCatList = cbService.LstLookup(ComboType.Business_Category, null, false);

            if (model.Country_ID.HasValue)
                model.stateList = cbService.LstState(model.Country_ID.Value.ToString(), true);
            else if (model.countryList.Count() > 0)
                model.stateList = cbService.LstState(model.countryList[0].Value, true);

            if (model.Billing_Country_ID.HasValue)
                model.stateBillingList = cbService.LstState(model.Billing_Country_ID.Value.ToString(), true);
            else if (model.countryList.Count() > 0)
                model.stateBillingList = cbService.LstState(model.countryList[0].Value, true);

            model.genderList = cbService.LstLookup(ComboType.Gender, userlogin.Company_ID, false);
            model.maritalStatusList = cbService.LstLookup(ComboType.Marital_Status, userlogin.Company_ID, false);
            model.raceList = cbService.LstLookup(ComboType.Race, userlogin.Company_ID, false);
            model.nationalityList = cbService.LstNationality(false);
            model.residentialStatusList = cbService.LstResidentialStatus();
            model.branchList = cbService.LstBranch(userlogin.Company_ID, true);
            model.departmentList = cbService.LstDepartment(userlogin.Company_ID, true);
            model.desingnationList = cbService.LstDesignation(userlogin.Company_ID, true);
            model.empTypeList = cbService.LstLookup(ComboType.Employee_Type, userlogin.Company_ID, false);
            model.currencyList = cbService.LstCurrency(false);
            model.termList = cbService.LstTerm();
            model.countryList = cbService.LstCountry(true);
            return View(model);
        }
        #endregion

        #region DashBoard
        [HttpGet]
        [AllowAuthorized]
        public ActionResult Index()
        {
            var model = new DashBoardViewModel();
            model = InitDashboard();
            var cbService = new ComboService();
            if (model.ErrorCode != 0)
                return errorPage(model.ErrorCode);
            if (model.redirectURL != null)
                return Redirect(model.redirectURL);
            return View(model);
        }

        private DashBoardViewModel InitDashboard()
        {
            var model = new DashBoardViewModel();
            Debug.WriteLine("'***********APP DEBUG***********' " + DateTime.Now + "-Start Controller Home Index");
            var userlogin = UserSession.getUser(HttpContext);
            if (userlogin == null)
            {
                model.ErrorCode = ERROR_CODE.ERROR_401_UNAUTHORIZED;
                return model;
            }
            if (userlogin.User_Authentication == null)
            {
                model.ErrorCode = ERROR_CODE.ERROR_401_UNAUTHORIZED;
                return model;
            }
            model.userProfile = userlogin;

            var currentdate = StoredProcedure.GetCurrentDate();
            var comService = new CompanyService();
            var lService = new LeaveService();
            var eService = new ExpenseService();
            var empService = new EmployeeService();
            var pService = new PayrollService();
            var hService = new EmploymentHistoryService();
            var uService = new UserService();
            var sService = new SubscriptionService();

            var com = comService.GetCompany(userlogin.Company_ID);
            if (com == null)
            {
                model.ErrorCode = ERROR_CODE.ERROR_401_UNAUTHORIZED;
                return model;
            }

            model.Emp = empService.GetEmployeeProfileByProfileID(userlogin.Profile_ID);
            if (model.Emp == null)
            {
                model.ErrorCode = ERROR_CODE.ERROR_511_DATA_NOT_FOUND;
                return model;
            }

            if (com.Currency != null)
            {
                model.Currency_ID = com.Currency.Currency_ID;
                model.Currency_Code = com.Currency.Currency_Code;
            }


            #region Billing Report
            //To validate for the overdue
            //Added by Moet on 4 Sep
            bool isAdmin = false;
            if (uService.IsOverdue(com.Company_ID) == true)
            {
                foreach (User_Assign_Role urole in userlogin.User_Authentication.User_Assign_Role.ToList())
                {
                    int AuthUserRole = 4;
                    if (urole.User_Role_ID.Value == AuthUserRole)
                    {
                        //return RedirectToAction("BillingReport", "Subscription");                        
                        //return Redirect(AppSetting.SERVER_NAME + "Authensbs2/Subscription/BillingReport");
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
                        if (AppSetting.IsDemo == "true")
                        {
                            stringURL = AppSetting.SERVER_NAME + "Authensbs2/Subscription/BillingReport";
                        }
                        if (AppSetting.IsLocal == "true")
                        {
                            stringURL = AppSetting.SERVER_NAME + "Subscription/BillingReport";
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
            #endregion

            #region  Validate Page
            var page = new List<string>();
            page.Add("/Employee/Employee");
            page.Add("/Leave/Application");
            page.Add("/Expenses/Application");
            page.Add("/Payroll/Payroll");
            page.Add("/Employee/EmployeeInfoAdmin");
            page.Add("/Employee/EmployeeInfoHR");

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

            //-------rights------------
            if (right.ContainsKey("/Employee/EmployeeInfoAdmin")) model.adminRights = right["/Employee/EmployeeInfoAdmin"];
            if (right.ContainsKey("/Employee/EmployeeInfoHR")) model.hrRights = right["/Employee/EmployeeInfoHR"];

            var hrview = false;
            var admin = model.adminRights != null ? model.adminRights.Contains("A") : false;
            var hr = model.hrRights != null ? model.hrRights.Contains("A") : false;
            if (admin == true || hr == true)
            {
                hrview = true;
            }
            #endregion

            var hist = hService.GetCurrentEmploymentHistory(model.Emp.Employee_Profile_ID);
            if (hist == null)
            {
                model.Display_Leave = false;
                model.Display_Expenses = false;
                model.Display_Payroll = false;
            }
            model.Emp_Hist = hist;


            #region Leave
            if (model.Display_Leave)
            {
                Debug.WriteLine("'***********APP DEBUG***********' " + DateTime.Now + "-Start Get Leave Data");
                if (hist != null)
                {
                    var child = model.Emp.Relationships.Where(w => w.Child_Type == ChildType.OwnChild | w.Child_Type == ChildType.AdoptedChild).OrderByDescending(o => o.DOB).FirstOrDefault();
                    if (child != null)
                    {
                        model.Relationship_ID = child.Relationship_ID;
                        model.Relationship_Name = child.Name;
                    }

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
                    model.LeaveList = lService.LstLeaveApplicationDocument(com.Company_ID, pProfileID: userlogin.Profile_ID)
                       .Where(w =>
                          w.Overall_Status != SBSWorkFlowAPI.Constants.WorkflowStatus.Rejected
                          &&
                          w.Cancel_Status != SBSWorkFlowAPI.Constants.WorkflowStatus.Cancelled
                          ).ToList();

                    Debug.WriteLine("'***********APP DEBUG***********' " + DateTime.Now + "-End Lst Leave Doc");

                    var maincri = new LeaveTypeCriteria()
                    {
                        Company_ID = com.Company_ID,
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
                        var cri = new LeaveCalIsExistCriteria() { Employee_Profile_ID = model.Emp.Employee_Profile_ID };
                        var calisexsist = lService.LeaveCalIsExist(cri);
                        if (!calisexsist)
                            maincri.Ignore_Generate = false;
                    }
                    else
                        maincri.Ignore_Generate = false;

                    Debug.WriteLine("'***********APP DEBUG***********' " + DateTime.Now + "-Start Leave Calculate");
                    model.LeaveBalanceList = lService.LstAndCalulateLeaveType(maincri);
                    if (model.LeaveBalanceList != null && model.LeaveBalanceList.Count() > 0)
                    {
                        if (!model.Leave_Config_ID.HasValue)
                        {
                            var l = model.LeaveBalanceList.FirstOrDefault();
                            if (l != null && l.Leave_Config != null)
                                model.Leave_Config_ID = l.Leave_Config.Leave_Config_ID;
                        }
                    }
                    Debug.WriteLine("'***********APP DEBUG***********' " + DateTime.Now + "-End Leave Calculate");
                }
                var cbService = new ComboService();
                model.periodList = cbService.LstDatePeriod(true);
                Debug.WriteLine("'***********APP DEBUG***********' " + DateTime.Now + "-End Get Leave Data");


                var bankHolidays = new List<Holidays>();
                Debug.WriteLine("'***********APP DEBUG***********' " + DateTime.Now + "-Start Get Holidays Data");
                var holidaylist = lService.getHolidays(com.Company_ID);
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

                model.LeaveTeamCalendarList = lService.LstLeaveApplicationDocument(userlogin.Company_ID, WorkflowStatus.Closed);
                if (model.LeaveTeamCalendarList != null && model.LeaveTeamCalendarList.Count > 0)
                {
                    var empCount = model.LeaveTeamCalendarList.Select(s => s.Employee_Profile_ID).Distinct().ToList();
                    var color = new List<string>();
                    for (int i = 0; i <= empCount.Count; i++)
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
            #endregion

            #region Expenses Temp
            //if (model.Display_Expenses)
            //{
            //    Debug.WriteLine("'***********APP DEBUG***********' " + DateTime.Now + "-Start Get Expenses Data");
            //    model.ExpensesBalanceList = new List<ExpensesBalanceViewModel>();
            //    model.ExpensesList = eService.getExpenseApplications(userlogin.Company_ID, userlogin.Profile_ID);

            //    var eTypes = eService.getExpenseTypes(userlogin.Company_ID);
            //    if (eTypes != null)
            //    {
            //        foreach (var row in eTypes)
            //        {
            //            var docs = model.ExpensesList.Where(w => w.Expenses_Application_Document.Where(w2 => w2.Expenses_Config_ID == row.Expenses_Config_ID).Count() > 0);
            //            var amount = docs.Sum(s => s.Expenses_Application_Document.Where(w2 => w2.Expenses_Config_ID == row.Expenses_Config_ID).Select(s2 => (s2.Amount_Claiming.HasValue ? s2.Amount_Claiming.Value : 0)).Sum());
            //            var totalamount = row.Expenses_Config_Detail.Select(s => (s.Amount_Per_Year.HasValue ? s.Amount_Per_Year.Value : 0)).Sum();

            //            model.ExpensesBalanceList.Add(new ExpensesBalanceViewModel()
            //            {
            //                Expenses_Type_Name = row.Expenses_Name,
            //                Amount = totalamount - amount
            //            });
            //        }
            //    }
            //    Debug.WriteLine("'***********APP DEBUG***********' " + DateTime.Now + "-End Get Expenses Data");
            //}
            #endregion

            #region Payroll
            if (hrview)
            {
                var lstEmp = empService.LstEmployeeProfile(com.Company_ID);
                var cnt = 0;
                decimal T_Payroll = 0;
                foreach (var e in lstEmp)
                {
                    if (e.User_Profile.User_Status == RecordStatus.Active)
                    {
                        cnt++;
                        hist = e.Employment_History.OrderByDescending(o => o.History_ID).FirstOrDefault();
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
            }
            #endregion

            #region Expenses
            if (model.Display_Expenses)
            {
                if (model.Emp_Hist != null)
                {
                    var yearSerivce = empService.GetYearService(model.Emp_Hist.Employee_Profile_ID);
                    var expConfigList = eService.getExpenseTypes(com.Company_ID, model.Emp_Hist.Department_ID, model.Emp_Hist.Designation_ID, null, yearSerivce);
                    model.expensesConfigList = expConfigList;
                    model.Expenses_Date = DateUtil.ToDisplayDate(currentdate);
                    if (model.expensesConfigList.Count > 0)
                    {
                        var expense_Type = expConfigList[0];
                        var expense_Type_Detail = expConfigList[0].Expenses_Config_Detail.FirstOrDefault();
                        model.Balance = eService.calulateBalance(expense_Type, expense_Type_Detail, model.Emp_Hist.Employee_Profile_ID, currentdate);
                    }
                }
            }

            if (hrview)
            {
                Debug.WriteLine("'***********APP DEBUG***********' " + DateTime.Now + "-Start Get Expenses");

                DateTime sDateOfThisMonth = DateUtil.ToDate("01/" + currentdate.Month + "/" + currentdate.Year, "/").Value;
                model.Total_Expenses_Amt = 0;
                var expList = new List<Expenses_Application_Document>();
                var criteria = new ExpenseCriteria() { Company_ID = userlogin.Company_ID, Closed_Status = true };
                var pResult = eService.LstExpenseApplications(criteria);
                if (pResult.Object != null) expList = (List<Expenses_Application_Document>)pResult.Object;

                if (expList != null && expList.Count() > 0)
                {
                    var sexp = expList.Where(e => e.Date_Applied >= sDateOfThisMonth && e.Date_Applied <= currentdate).ToList();
                    if (sexp != null && sexp.Count() > 0)
                    {
                        model.Total_Expenses_Amt = sexp.Sum(s => s.Amount_Claiming.HasValue ? s.Amount_Claiming.Value : 0);
                    }
                }
                Debug.WriteLine("'***********APP DEBUG***********' " + DateTime.Now + "-End Get Expenses");
            }
            #endregion

            #region Diskspace
            Debug.WriteLine("'***********APP DEBUG***********' " + DateTime.Now + "-Start Get Disk space");
            model.Total_Diskspace = sService.Get_Total_Storage_ByCompany(com.Company_ID); // It returns MB

            decimal dsize = sService.Get_DataSize_ByCompany(com.Company_ID);
            dsize = Math.Round(dsize / 1000, 2); // 1 MB = 1000 KB

            decimal[] Usage = sService.Get_Dtl_DataSize_ByCompany(com.Company_ID);
            model.Company_Usage = Math.Round(Usage[0] / 1000, 2);
            model.Emp_Usage = Math.Round(Usage[1] / 1000, 2);
            model.Leave_Usage = Math.Round(Usage[2] / 1000, 2);
            model.Exp_Usage = Math.Round(Usage[3] / 1000, 2);

            model.Diskspace_Usage = dsize;
            model.Avail_Diskspace = model.Total_Diskspace - dsize;
            if (model.Avail_Diskspace < 0) model.Avail_Diskspace = 0;
            Debug.WriteLine("'***********APP DEBUG***********' " + DateTime.Now + "-End Get Disk space");

            if (hrview)
            {
                if (com.Is_PostPaid == true)
                {
                    Debug.WriteLine("'***********APP DEBUG***********' " + DateTime.Now + "-Start Get Total Billing");
                    decimal dBillAmt = sService.Get_BillingAmount_ByCompany(com.Company_ID);
                    if (dBillAmt > 0)
                    {
                        dBillAmt = Math.Round(dBillAmt, 2);
                    }
                    else
                        model.Is_Trial = true;
                    model.Total_Billing_Amt = dBillAmt;
                    Debug.WriteLine("'***********APP DEBUG***********' " + DateTime.Now + "-End Get Total Billing");
                }
            }
            #endregion

            #region Outstanding Bill
            if (hrview)
            {
                Debug.WriteLine("'***********APP DEBUG***********' " + DateTime.Now + "-Start Get Total Outstanding");
                decimal dOutAmt = sService.Get_OutstandingBill_ByCompany(com.Company_ID);
                dOutAmt = Math.Round(dOutAmt, 2);
                model.Total_Outstanding_Amt = dOutAmt;
                Debug.WriteLine("'***********APP DEBUG***********' " + DateTime.Now + "-Start End Total Outstanding");
            }
            #endregion

            Debug.WriteLine("'***********APP DEBUG***********' " + DateTime.Now + "-End Controller Home Index");
            var err = GetErrorModelState();

            return model;
        }

        [HttpPost]
        [AllowAuthorized]
        public ActionResult Index(DashBoardViewModel model, string pageAction)
        {
            var Initmodel = InitDashboard();
            if (Initmodel != null && Initmodel.userProfile != null)
            {
                if (pageAction == "btn500" || pageAction == "btn1gb" || pageAction == "btn2gb")
                {
                    #region Storage Upgrade
                    var uModel = new Storage_Upgrade();
                    uModel.Company_ID = Initmodel.userProfile.Company_ID.Value;
                    uModel.Upgrade_By = Initmodel.userProfile.User_Authentication.Email_Address;
                    switch (pageAction)
                    {
                        case "btn500":
                            uModel.Upgrade_Space = 500;
                            uModel.Price = 5;
                            break;
                        case "btn1gb":
                            uModel.Upgrade_Space = 1000;
                            uModel.Price = 10;
                            break;
                        case "btn2gb":
                            uModel.Upgrade_Space = 2000;
                            uModel.Price = 15;
                            break;
                    }
                    var sService = new SubscriptionService();
                    var result = sService.Insert_Storage(uModel);
                    var svr = new ServiceResult();
                    if (result == true)
                    {
                        //relode view after update storage
                        Initmodel = InitDashboard();
                        svr.Msg = "Your storage has been increased.";
                        svr.Code = 1;
                        svr.Field = "Upgrage Storage";
                    }
                    else
                    {
                        Error e = new Error();
                        svr.Msg = "Upgrading storage is not success.";
                        svr.Code = -1;
                        svr.Field = "Upgrage Storage";
                    }
                    Initmodel.result = svr;
                    #endregion
                }
                else if (pageAction == "Leave")
                {
                    #region Leave
                    if (string.IsNullOrEmpty(model.Start_Date))
                        ModelState.AddModelError("Start_Date", Resource.Message_Is_Required);

                    if (string.IsNullOrEmpty(model.End_Date))
                        ModelState.AddModelError("End_Date", Resource.Message_Is_Required);

                    if (DateUtil.ToDate(model.Start_Date) > DateUtil.ToDate(model.End_Date))
                        ModelState.AddModelError("Start_Date", Resource.Message_Is_Invalid);

                    var LeaveIsValid = true;
                    if (ModelState.IsValid)
                    {
                        decimal LeaveLeft = 0;
                        string LeaveType = "";
                        if (Initmodel.LeaveBalanceList != null)
                        {
                            foreach (var l in Initmodel.LeaveBalanceList)
                            {
                                if (l.Leave_Config.Leave_Config_ID == model.Leave_Config_ID)
                                {
                                    LeaveLeft = l.Leave_Left.Entitle - l.Leave_Left.Leave_Used;
                                    LeaveType = l.Leave_Config.Type;
                                    break;
                                }
                            }
                        }
                        var lvModel = new LeaveViewModel();
                        lvModel.Leave_Config_ID = model.Leave_Config_ID;
                        lvModel.Start_Date = model.Start_Date;
                        lvModel.End_Date = model.End_Date;
                        lvModel.Start_Date_Period = model.Start_Date_Period;
                        lvModel.End_Date_Period = model.End_Date_Period;
                        lvModel.Days_Taken = "0";
                        lvModel.Leave_Left = LeaveLeft;
                        lvModel.Type = LeaveType;
                        lvModel.OnBehalf_Employee_Profile_ID = 0;
                        lvModel.OnBehalf_Profile_ID = 0;
                        ApplicationNew(lvModel, Request.Files[0], "Quick");
                        if (lvModel != null && lvModel.result != null)
                        {

                            if (lvModel.result.Code == ERROR_CODE.SUCCESS)
                            {
                                lvModel.result = new ServiceResult() { Code = lvModel.result.Code, Msg = new Success().getSuccess(ERROR_CODE.SUCCESS), Field = lvModel.result.Field };
                                //relode view after save Leave pass
                                Initmodel = InitDashboard();
                                Initmodel.result = lvModel.result;
                                Initmodel.Start_Date = String.Empty;
                                Initmodel.End_Date = String.Empty;
                                LeaveIsValid = false;
                            }
                        }
                    }
                    if (LeaveIsValid)
                    {
                        Initmodel.Leave_Config_ID = model.Leave_Config_ID;
                        Initmodel.Start_Date = model.Start_Date;
                        Initmodel.End_Date = model.End_Date;
                        Initmodel.Start_Date_Period = model.Start_Date_Period;
                        Initmodel.End_Date_Period = model.End_Date_Period;
                    }
                    #endregion
                }
                else if (pageAction == "Expense")
                {
                    #region Expense
                    //validation
                    if (!model.Balance.HasValue)
                        model.Balance = 0;

                    if (string.IsNullOrEmpty(model.Expenses_Title))
                        ModelState.AddModelError("Expenses_Title", Resource.Message_Is_Invalid);

                    if (string.IsNullOrEmpty(model.Expenses_Date))
                        ModelState.AddModelError("Expenses_Date", Resource.Message_Is_Invalid);

                    if (!model.Amount_Claiming.HasValue || model.Amount_Claiming == 0)
                        ModelState.AddModelError("Amount_Claiming", Resource.Message_Is_Invalid);
                    else
                    {
                        if (model.Amount_Claiming > model.Balance)
                            ModelState.AddModelError("Amount_Claiming", "Your balance is " + model.Balance + " and your claimed amount should not over the balance.");
                    }

                    if (ModelState.IsValid)
                    {
                        HttpPostedFileBase file = Request.Files[0];
                        if (file != null && file.ContentLength > 0)
                        {
                            MemoryStream target = new MemoryStream();
                            file.InputStream.CopyTo(target);
                            byte[] data = target.ToArray();

                            string pFile = Convert.ToBase64String(data);
                            if (!string.IsNullOrEmpty(pFile))
                            {
                                string trimmedData = pFile;
                                var prefixindex = trimmedData.IndexOf(",");
                                trimmedData = trimmedData.Substring(prefixindex + 1, trimmedData.Length - (prefixindex + 1));

                                var filebyte = Convert.FromBase64String(trimmedData);
                                if (filebyte != null)
                                {
                                    model.Upload_Receipt = trimmedData;
                                    model.Upload_Receipt_Name = file.FileName;
                                }
                            }
                        }

                        var exModel = new ExpensesViewModel();
                        exModel.Expenses_Config_ID = model.Expenses_Config_ID;
                        exModel.Expenses_Title = model.Expenses_Title;
                        exModel.Employee_Profile_ID = Initmodel.Emp.Employee_Profile_ID;
                        exModel.Email = Initmodel.userProfile.User_Authentication.Email_Address;
                        exModel.Date_Applied = model.Expenses_Date;
                        exModel.OnBehalf_Employee_Profile_ID = 0;
                        exModel.OnBehalf_Profile_ID = 0;
                        exModel.operation = UserSession.RIGHT_C;
                        var details = new List<ExpensesDetailViewModel>();
                        details.Add(new ExpensesDetailViewModel()
                        {
                            Expenses_Application_Document_ID = 0,
                            Amount_Claiming = model.Amount_Claiming,
                            Balance = model.Balance,
                            Date_Applied = model.Expenses_Date,
                            Expenses_Config_ID = model.Expenses_Config_ID,
                            Expenses_Date = model.Expenses_Date,
                            Total_Amount = model.Amount_Claiming,
                            Upload_Receipt = model.Upload_Receipt,
                            Upload_Receipt_Name = model.Upload_Receipt_Name,
                            UOM_ID = model.UOM_ID,
                            UOM_Name = model.UOM_Name,
                            Mileage = model.Mileage,
                            Amount_Per_UOM = model.Amount_Per_UOM,
                            Tax_Type = TaxType.Exclusive,
                            Tax = 0,
                            Tax_Amount = 0,
                            Tax_Amount_Type = "%",
                            Withholding_Tax = 0,
                            Withholding_Tax_Amount = 0,
                            Withholding_Tax_Type = "%",
                            Row_Type = RowType.ADD,
                            Selected_Currency = model.Currency_ID,
                        });

                        exModel.Detail_Rows = details.ToArray();
                        var ExpensesIsValid = true;
                        ApplicationNew(exModel, "Quick");
                        if (exModel != null && exModel.result != null)
                        {
                            if (exModel.result.Code == ERROR_CODE.SUCCESS)
                            {
                                exModel.result = new ServiceResult() { Code = exModel.result.Code, Msg = new Success().getSuccess(ERROR_CODE.SUCCESS), Field = exModel.result.Field };
                                //relode view after save expenses pass
                                Initmodel = InitDashboard();
                                Initmodel.result = exModel.result;
                                Initmodel.Expenses_Title = String.Empty;
                                Initmodel.Amount_Claiming = 0;
                                ExpensesIsValid = false;
                            }
                        }
                        if (ExpensesIsValid)
                        {
                            Initmodel.Expenses_Date = model.Expenses_Date;
                            Initmodel.Expenses_Config_ID = model.Expenses_Config_ID;
                            Initmodel.Amount_Claiming = model.Amount_Claiming;
                            Initmodel.Expenses_Title = model.Expenses_Title;
                        }
                        Response.Clear();
                        Response.ClearContent();
                        Response.ClearHeaders();
                    }
                    #endregion
                }
            }

            return View(Initmodel);
        }

        #region Ajax
        public ActionResult CheckUploadDoc(Nullable<int> pLeaveConfigID)
        {
            var userlogin = UserSession.getUser(HttpContext);
            if (userlogin == null)
                return errorPage(ERROR_CODE.ERROR_401_UNAUTHORIZED);

            var leaveService = new LeaveService();
            var lService = new LeaveService();

            var uploaddoc = false;
            if (pLeaveConfigID.HasValue)
            {
                var leaveConfig = leaveService.GetLeaveConfig(pLeaveConfigID);
                if (leaveConfig != null)
                    uploaddoc = leaveConfig.Upload_Document.HasValue ? leaveConfig.Upload_Document.Value : false;

            }
            return Json(new { IsUploadDoc = uploaddoc }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public String QuickApplicationConfig(Nullable<int> Expenses_Config_ID, Nullable<decimal> Total_Amount, string pExpenses_Date, Nullable<int> pProfileID)
        {
            var model = new ExpensesViewModel();
            var exCurService = new ExchangeCurrencyConfigService();
            var eService = new ExpenseService();
            var cbService = new ComboService();
            var comService = new CompanyService();
            var histService = new EmploymentHistoryService();
            var userlogin = UserSession.getUser(HttpContext);
            var currentdate = StoredProcedure.GetCurrentDate();
            var emp = new EmployeeService(); //Added by Moet

            if (userlogin.Employee_Profile.Count == 0)
                return "";

            var expense_Type = eService.GetExpenseType(Expenses_Config_ID);
            if (expense_Type == null)
                return "";

            var Emp_Profile_ID = userlogin.Employee_Profile.Select(w => w.Employee_Profile_ID).FirstOrDefault();
            if (Emp_Profile_ID == 0)
                return "";

            //Added by Moet
            if (pProfileID == null)
            {
                pProfileID = userlogin.Profile_ID;
            }
            else
            {
                var e = emp.GetEmployeeProfileByProfileID(pProfileID);
                Emp_Profile_ID = e.Employee_Profile_ID;
            }

            var expense_Type_Detail = eService.GetExpensesConfigDetail(Expenses_Config_ID, pProfileID);
            if (expense_Type_Detail == null)
                return "";

            var balance = 0M;
            var amountClaiming = 0M;
            var amount = 1;
            var isPerDepartment = false;


            if (expense_Type.Claimable_Type == "Per Department")
                isPerDepartment = true;

            balance = eService.calulateBalance(expense_Type, expense_Type_Detail, pProfileID, currentdate);

            if (expense_Type.Is_Accumulative.HasValue && expense_Type.Is_Accumulative.Value)
            {
                int longdate = 0;


                var firstHist = histService.GetFirstEmploymentHistory(Emp_Profile_ID);

                Nullable<DateTime> StartDate = new DateTime(currentdate.Year, 1, 1);
                if (firstHist != null && firstHist.Effective_Date.HasValue && firstHist.Effective_Date.Value.Year == currentdate.Year)
                    StartDate = firstHist.Effective_Date.Value;

                TimeSpan span = currentdate.Date.Subtract(StartDate.Value.Date);
                longdate = (int)span.TotalDays;
                balance = ((balance * (longdate + 1)) / 365);
            }


            decimal? totalclaimed = 0;
            var cri = new ExpenseCriteria();
            cri.Company_ID = userlogin.Company_ID;
            cri.Employee_Profile_ID = Emp_Profile_ID;
            cri.Year = currentdate.Year;
            cri.Closed_Status = true;
            var closedEx = eService.LstExpenseApplications(cri);
            if (closedEx.Object != null)
            {
                var exs = closedEx.Object as List<Expenses_Application_Document>;
                foreach (var ex in exs)
                {
                    totalclaimed += ex.Amount_Claiming;
                }
            }
            balance = balance - totalclaimed.Value;

            var pday = DateUtil.ToDate(pExpenses_Date);
            var day = currentdate;
            var year = currentdate.Year.ToString();
            var month = currentdate.Month.ToString();
            if (pday != null)
            {
                day = pday.Value;
                year = pday.Value.Year.ToString();
                month = pday.Value.Month.ToString();
            }

            model.currencyList = cbService.LstCurrencyByCompany(userlogin.Company_ID.Value, year.ToString());

            var comcurr = comService.GetCompany(userlogin.Company_ID).Currency;
            if (comcurr == null) return "";

            if (expense_Type_Detail.Select_Pecentage.HasValue && expense_Type_Detail.Select_Pecentage.Value)
            {
                var percent = expense_Type_Detail.Pecentage.HasValue ? expense_Type_Detail.Pecentage.Value : 0;
                amountClaiming = ((Total_Amount.HasValue ? Total_Amount.Value : 0) * percent / 100) * amount;
            }
            else
            {
                if (expense_Type_Detail.Amount >= Total_Amount * amount)
                {
                    amountClaiming = (Total_Amount.HasValue ? Total_Amount.Value : 0) * amount;
                }
                else
                {
                    amountClaiming = expense_Type_Detail.Amount.HasValue ? expense_Type_Detail.Amount.Value : 0;
                }
            }

            if (!isPerDepartment)
            {
                if (balance <= 0)
                {
                    amountClaiming = 0;
                }
                else if (balance < amountClaiming)
                {
                    amountClaiming = balance;
                }
            }

            string str = "";
            str = "<script type=\"text/javascript\"> \n\n $(function () {" +
                    "$('#Amount_Claiming').val(" + amountClaiming + "); ";

            if (expense_Type.UOM_ID.HasValue && expense_Type.Global_Lookup_Data != null)
            {
                str += "$('#UOM_Name').val('" + expense_Type.Global_Lookup_Data.Description + "');";
                str += "$('#UOM_Name2').val('" + expense_Type.Global_Lookup_Data.Description + "');";
                str += "$('#UOM_ID').val('" + expense_Type.UOM_ID + "');";
                str += "$('#Amount_Per_UOM').val('" + expense_Type.Amount_Per_UOM + "');";
                str += "$('#UOM_ID').trigger('chosen:updated');";
            }
            else
            {
                str += "$('#UOM_Name').val('');";
                str += "$('#UOM_Name2').val('');";
                str += "$('#UOM_ID').val('');";
                str += "$('#Amount_Per_UOM').val('');";
            }

            str += "$('#Max_Amount_Claiming').val(" + amountClaiming + ");";
            str += "$('#Balance_Lable').val('" + balance.ToString("n2") + "');";
            str += "$('#Balance').val('" + balance + "');";
            str += "});\n\n</script>";

            return str;
        }

        #endregion

        //public ActionResult QuickExpenseApplicationNew(ExpensesViewModel model, string pStatus)
        //{
        //   var userlogin = UserSession.getUser(HttpContext);
        //   if (userlogin == null)
        //      return errorPage(ERROR_CODE.ERROR_401_UNAUTHORIZED);

        //   var currentdate = StoredProcedure.GetCurrentDate();
        //   var hService = new EmploymentHistoryService();
        //   var eService = new ExpenseService();
        //   var cpService = new CompanyService();
        //   var empService = new EmployeeService();
        //   var uService = new UserService();
        //   var aService = new SBSWorkFlowAPI.Service();

        //   var emp = userlogin.Employee_Profile.FirstOrDefault();
        //   if (emp == null)
        //      return errorPage(ERROR_CODE.ERROR_511_DATA_NOT_FOUND, Resource.Employee);

        //   var hist = hService.GetCurrentEmploymentHistory(emp.Employee_Profile_ID);
        //   if (hist == null)
        //      return errorPage(ERROR_CODE.ERROR_14_NO_EMPLOYEE_HIST);

        //   var com = cpService.GetCompany(userlogin.Company_ID);
        //   if (com == null)
        //      return errorPage(ERROR_CODE.ERROR_511_DATA_NOT_FOUND, Resource.Company);

        //   //{5-Jul-2016} - Moet : modified this part to enhance that the supervisor should able to apply the leave onbehalf        
        //   var _EmpProfileID = 0;
        //   var _ProfileID = userlogin.Profile_ID;
        //   //{5-Jul-2016} - Moet : This is to take the employee profile id of the person who wants to take leave
        //   if (model.OnBehalf_Profile_ID.Value > 0)
        //   {
        //      _ProfileID = model.OnBehalf_Profile_ID.Value;
        //      var empProf = empService.GetEmployeeProfileByProfileID(model.OnBehalf_Profile_ID);
        //      if (empProf != null)
        //      {
        //         model.OnBehalf_Employee_Profile_ID = empProf.Employee_Profile_ID;
        //         _EmpProfileID = empProf.Employee_Profile_ID;
        //      }
        //   }
        //   else
        //   {
        //      model.OnBehalf_Employee_Profile_ID = 0;
        //      model.OnBehalf_Profile_ID = 0;

        //      var empProf = empService.GetEmployeeProfileByProfileID(userlogin.Profile_ID);
        //      if (empProf != null)
        //      {
        //         model.Employee_Profile_ID = empProf.Employee_Profile_ID;
        //         _EmpProfileID = model.Employee_Profile_ID;
        //      }
        //   }

        //   if (pStatus == "Quick")
        //   {
        //      /* create new expenses by employee*/
        //      model.PageStatus = null;
        //      var edoc = new Expenses_Application()
        //      {
        //         Expenses_Application_ID = model.Expenses_ID.HasValue ? model.Expenses_ID.Value : 0,
        //         Employee_Profile_ID = _EmpProfileID,
        //         Expenses_No = model.Expenses_No,
        //         Date_Applied = DateUtil.ToDate(model.Date_Applied),
        //         Expenses_Title = model.Expenses_Title,
        //         Update_By = userlogin.User_Authentication.Email_Address,
        //         Update_On = currentdate,
        //         Supervisor = hist.Supervisor,
        //         Overall_Status = WorkflowStatus.Pending,
        //      };

        //      var details = new List<Expenses_Detail>();
        //      if (model.Detail_Rows != null)
        //      {
        //         foreach (var row in model.Detail_Rows)
        //         {
        //            details.Add(new Expenses_Detail()
        //            {
        //               Expenses_Application_Document_ID = row.Expenses_Application_Document_ID,
        //               Amount_Claiming = row.Amount_Claiming,
        //               Balance = row.Balance,
        //               Claimable_Type = row.Claimable_Type,
        //               Date_Applied = row.Date_Applied,
        //               Department_ID = row.Department_ID,
        //               Expenses_Config_ID = row.Expenses_Config_ID,
        //               Expenses_Date = row.Expenses_Date,
        //               Doc_No = row.Doc_No,
        //               Expenses_Type_Desc = row.Expenses_Type_Desc,
        //               Expenses_Type_Name = row.Expenses_Type_Name,
        //               Notes = row.Notes,
        //               Row_Type = RowType.ADD,
        //               Selected_Currency = com.Currency_ID,
        //               Mileage = row.Mileage,
        //               Total_Amount = row.Total_Amount,
        //               Upload_Receipt = row.Upload_Receipt,
        //               Upload_Receipt_Name = row.Upload_Receipt_Name,
        //               Tax = row.Tax,
        //               Tax_Type = row.Tax_Type,
        //               Tax_Amount = row.Tax_Amount,
        //               Tax_Amount_Type = row.Tax_Amount_Type,
        //               Withholding_Tax = row.Withholding_Tax,
        //               Withholding_Tax_Amount = row.Withholding_Tax_Amount,
        //               Withholding_Tax_Type = row.Withholding_Tax_Type,
        //               //********  Smart Dev  ********//
        //               Job_Cost_ID = ((row.Job_Cost_ID.HasValue && row.Job_Cost_ID.Value != 0) ? row.Job_Cost_ID : null),
        //            });
        //         }
        //      }

        //      var haveApprover = true;
        //      var rworkflow = aService.GetWorkflowByEmployee(userlogin.Company_ID.Value, userlogin.Profile_ID, ModuleCode.HR, ApprovalType.Expense, hist.Department_ID);
        //      if (!rworkflow.Item2.IsSuccess || rworkflow.Item1 == null || rworkflow.Item1.Count == 0)
        //         haveApprover = false;

        //      // Create Expense Application
        //      edoc.Create_By = userlogin.User_Authentication.Email_Address;
        //      edoc.Create_On = currentdate;
        //      model.result = eService.insertExpenseApplication(edoc, details);
        //      if (model.result.Code == ERROR_CODE.SUCCESS)
        //      {
        //         var ex = eService.getExpenseApplication(edoc.Expenses_Application_ID);
        //         if (ex != null)
        //         {
        //            if (haveApprover)
        //            {
        //               //Use WF
        //               var request = new RequestItem();
        //               request.Doc_ID = edoc.Expenses_Application_ID;
        //               request.Approval_Type = ApprovalType.Expense;
        //               request.Company_ID = userlogin.Company_ID.Value;
        //               request.Department_ID = hist.Department_ID.HasValue ? hist.Department_ID.Value : 0;
        //               request.Module = ModuleCode.HR;
        //               request.Requestor_Email = userlogin.User_Authentication.Email_Address;
        //               request.Requestor_Name = UserSession.GetUserName(userlogin);
        //               request.Requestor_Profile_ID = userlogin.Profile_ID;
        //               var r = aService.SubmitRequest(request);
        //               if (r.IsSuccess)
        //               {
        //                  ex.Request_ID = request.Request_ID;
        //                  ex.Overall_Status = request.Status;
        //                  ex.Update_By = userlogin.User_Authentication.Email_Address;
        //                  ex.Update_On = currentdate;
        //                  model.result = eService.updateExpenseApplication(ex);
        //                  if (model.result.Code == ERROR_CODE.SUCCESS)
        //                  {

        //                     if (request.Status == WorkflowStatus.Closed)
        //                        sendProceedEmail(ex, com, userlogin, userlogin, hist, request.Status, request.Reviewers,
        //                                  model.OnBehalf_Employee_Profile_ID.Value, model.OnBehalf_Profile_ID.Value);
        //                     //sendProceedEmail(ex, com, userlogin, userlogin, hist, request.Status, request.Reviewers, null,
        //                     //    model.OnBehalf_Employee_Profile_ID.Value, model.OnBehalf_Profile_ID.Value);
        //                     else
        //                     {
        //                        var param = new Dictionary<string, object>();
        //                        param.Add("expID", ex.Expenses_Application_ID);
        //                        param.Add("appID", request.NextApprover.Profile_ID);
        //                        param.Add("empID", ex.Employee_Profile_ID);
        //                        param.Add("reqID", ex.Request_ID);
        //                        param.Add("status", WorkflowStatus.Approved);
        //                        param.Add("code", uService.GenActivateCode("E" + ex.Expenses_Application_ID + request.NextApprover.Profile_ID + "_", userlogin.User_Authentication.User_Authentication_ID));

        //                        var linkApp = GenerateActionLink("Approval", "ProcessWorkflow", param);
        //                        param["status"] = WorkflowStatus.Rejected;
        //                        var linkRej = GenerateActionLink("Approval", "ProcessWorkflow", param);

        //                        //sendProceedEmail(ex, com, userlogin, userlogin, hist, WorkflowStatus.Submitted, request.Reviewers, SmartDevPdfProceed);
        //                        //sendProceedEmail(ex, null, userlogin, userlogin, hist, WorkflowStatus.Submitted, request.Reviewers, null,
        //                        //         model.OnBehalf_Employee_Profile_ID.Value, model.OnBehalf_Profile_ID.Value);

        //                        sendProceedEmail(ex, null, userlogin, userlogin, hist, WorkflowStatus.Submitted, request.Reviewers,
        //   model.OnBehalf_Employee_Profile_ID.Value, model.OnBehalf_Profile_ID.Value);

        //                        var appr = uService.getUser(request.NextApprover.Profile_ID, false);
        //                        if (appr != null)
        //                           sendRequestEmail(ex, com, appr, userlogin, hist, request.Status, request.Reviewers, linkApp, linkRej);

        //                        //sendRequestEmail(ex, com, appr, userlogin, hist, request.Status, request.Reviewers, linkApp, linkRej, null);
        //                     }
        //                  }
        //                  return View(model);
        //               }
        //            }
        //            else
        //            {
        //               if (hist.Supervisor.HasValue)
        //               {
        //                  var sup = empService.GetEmployeeProfile2(hist.Supervisor);
        //                  if (sup != null)
        //                  {
        //                     var param = new Dictionary<string, object>();
        //                     param.Add("expID", ex.Expenses_Application_ID);
        //                     param.Add("appID", sup.Profile_ID);
        //                     param.Add("empID", ex.Employee_Profile_ID);
        //                     param.Add("status", WorkflowStatus.Approved);
        //                     param.Add("code", uService.GenActivateCode("E" + ex.Expenses_Application_ID + sup.Profile_ID + "_", userlogin.User_Authentication.User_Authentication_ID));

        //                     var linkApp = GenerateActionLink("Approval", "ProcessWorkflow", param);
        //                     param["status"] = WorkflowStatus.Rejected;
        //                     var linkRej = GenerateActionLink("Approval", "ProcessWorkflow", param);

        //                     //sendProceedEmail(ex, com, userlogin, userlogin, hist, WorkflowStatus.Submitted, null, null,
        //                     //           model.OnBehalf_Employee_Profile_ID.Value, model.OnBehalf_Profile_ID.Value);

        //                     sendProceedEmail(ex, com, userlogin, userlogin, hist, WorkflowStatus.Submitted, null,
        //                                       model.OnBehalf_Employee_Profile_ID.Value, model.OnBehalf_Profile_ID.Value);

        //                     //sendRequestEmail(ex, com, sup.User_Profile, userlogin, hist, ex.Overall_Status, null, linkApp, linkRej, null);

        //                     sendRequestEmail(ex, com, sup.User_Profile, userlogin, hist, ex.Overall_Status, null, linkApp, linkRej);
        //                     return View(model);
        //                  }
        //               }
        //               else
        //               {
        //                  //no WF
        //                  ex.Overall_Status = WorkflowStatus.Closed;
        //                  ex.Update_By = userlogin.User_Authentication.Email_Address;
        //                  ex.Update_On = currentdate;
        //                  model.result = eService.updateExpenseApplication(ex);
        //                  if (model.result.Code == ERROR_CODE.SUCCESS)
        //                  {
        //                     //sendProceedEmail(ex, com, userlogin, userlogin, hist, ex.Overall_Status, null, null,
        //                     //          model.OnBehalf_Employee_Profile_ID.Value, model.OnBehalf_Profile_ID.Value);

        //                     sendProceedEmail(ex, com, userlogin, userlogin, hist, ex.Overall_Status, null,
        //                                        model.OnBehalf_Employee_Profile_ID.Value, model.OnBehalf_Profile_ID.Value);
        //                  }
        //                  return View(model);
        //               }
        //            }
        //         }
        //      }
        //   }

        //   return View(model);
        //}
        //#endregion

        //#region Send Email
        //private void sendRequestEmail(Expenses_Application ex, Company_Details com, User_Profile sentto, User_Profile receivedfrom, Employment_History receivedhist, string Overall_Status, List<Reviewer> Reviewers, string linkApp, string linkRej)
        //{
        //   var ecode = "[RE" + ex.Expenses_Application_ID + "_";
        //   if (ex.Request_Cancel_ID.HasValue)
        //      ecode += "RQC" + ex.Request_Cancel_ID;
        //   else if (ex.Request_ID.HasValue)
        //      ecode += "RQ" + ex.Request_ID;

        //   ecode += "S" + receivedfrom.User_Authentication.User_Authentication_ID + "R" + sentto.User_Authentication.User_Authentication_ID + "]";

        //   //Fixed by sun 04-01-2016
        //   var Ex_Employee_Profile_ID = ex.Employee_Profile_ID.HasValue ? ex.Employee_Profile_ID.Value : 0;
        //   var Ex_Profile_ID = 0;
        //   if (ex.Employee_Profile != null && ex.Employee_Profile.Profile_ID.HasValue)
        //      Ex_Profile_ID = ex.Employee_Profile.Profile_ID.Value;

        //   //******** Start Smart Dev  ********//
        //   FileAttach SmartDevPdfRequest = getFileExpenseSmartDevPdf(EncryptUtil.Encrypt(ex.Expenses_Application_ID), EncryptUtil.Encrypt(UserSession.RIGHT_U), Ex_Employee_Profile_ID, Ex_Profile_ID);
        //   //******** End Smart Dev  ********//

        //   var eitem = new EmailItem()
        //   {
        //      LogoLink = GenerateLogoLink(),
        //      Company = com,
        //      Send_To_Email = sentto.User_Authentication.Email_Address,
        //      Send_To_Name = AppConst.GetUserName(sentto),
        //      Received_From_Email = receivedfrom.User_Authentication.Email_Address,
        //      Received_From_Name = AppConst.GetUserName(receivedfrom),
        //      Received_From_Department = receivedhist.Department != null ? receivedhist.Department.Name : "",
        //      Module = ModuleCode.HR,
        //      Approval_Type = ApprovalType.Expense,
        //      Expenses = ex,
        //      Status = Overall_Status,
        //      Reviewer = Reviewers,
        //      Link = linkApp,
        //      Link2 = linkRej,
        //      Url = Request.Url.AbsoluteUri,
        //      ECode = ecode,
        //      Attachment_SmartDev = SmartDevPdfRequest
        //   };
        //   EmailTemplete.sendRequestEmail(eitem);
        //}

        //private void sendProceedEmail(Expenses_Application ex, Company_Details com, User_Profile sentto, User_Profile receivedfrom, Employment_History receivedhist, string Overall_Status, List<Reviewer> Reviewers,
        //    int OnBehalf_Employee_Profile_ID = 0, int OnBehalf_Profile_ID = 0)
        //{
        //   var ecode = "[PE" + ex.Expenses_Application_ID + "_";
        //   if (ex.Request_Cancel_ID.HasValue)
        //      ecode += "RQC" + ex.Request_Cancel_ID;
        //   else if (ex.Request_ID.HasValue)
        //      ecode += "RQ" + ex.Request_ID;

        //   ecode += "S" + receivedfrom.User_Authentication.User_Authentication_ID + "R" + sentto.User_Authentication.User_Authentication_ID + "]";

        //   //Fixed by sun 04-01-2016
        //   var Ex_Employee_Profile_ID = ex.Employee_Profile_ID.HasValue ? ex.Employee_Profile_ID.Value : 0;
        //   var Ex_Profile_ID = 0;
        //   if (ex.Employee_Profile != null && ex.Employee_Profile.Profile_ID.HasValue)
        //      Ex_Profile_ID = ex.Employee_Profile.Profile_ID.Value;

        //   //******** Start Smart Dev  ********//
        //   FileAttach SmartDevPdfRequest = getFileExpenseSmartDevPdf(EncryptUtil.Encrypt(ex.Expenses_Application_ID), EncryptUtil.Encrypt(UserSession.RIGHT_U), Ex_Employee_Profile_ID, Ex_Profile_ID);
        //   //******** End Smart Dev  ********//

        //   var eitem = new EmailItem()
        //   {
        //      LogoLink = GenerateLogoLink(),
        //      Company = com,
        //      Send_To_Email = sentto.User_Authentication.Email_Address,
        //      Send_To_Name = AppConst.GetUserName(sentto),
        //      Received_From_Email = receivedfrom.User_Authentication.Email_Address,
        //      Received_From_Name = AppConst.GetUserName(receivedfrom),
        //      Received_From_Department = receivedhist.Department != null ? receivedhist.Department.Name : "",
        //      Module = ModuleCode.HR,
        //      Approval_Type = ApprovalType.Expense,
        //      Expenses = ex,
        //      Status = Overall_Status,
        //      Reviewer = Reviewers,
        //      Url = Request.Url.AbsoluteUri,
        //      ECode = ecode,
        //      Attachment_SmartDev = SmartDevPdfRequest
        //   };

        //   //EmailTemplete.sendProceedEmail(eitem);
        //   if (OnBehalf_Profile_ID > 0 && OnBehalf_Profile_ID != receivedfrom.Profile_ID)
        //   {
        //      var empService = new EmployeeService();
        //      var emp = empService.GetEmployeeProfile(OnBehalf_Employee_Profile_ID);
        //      var onBehalfName = "";
        //      if (OnBehalf_Employee_Profile_ID != null)
        //      {
        //         onBehalfName = AppConst.GetUserName(emp.User_Profile);
        //      }
        //      var r = new Reviewer();
        //      eitem.Send_To_Email = emp.User_Profile.Email; // Requester's email
        //      r.Email = sentto.User_Authentication.Email_Address; // Submitter's email

        //      if (eitem.Reviewer != null)
        //         eitem.Reviewer.Add(r);
        //      else
        //      {
        //         List<SBSWorkFlowAPI.Models.Reviewer> lstR = new List<SBSWorkFlowAPI.Models.Reviewer>();
        //         lstR.Add(r);
        //         eitem.Reviewer = lstR;
        //      }
        //      EmailTemplete.sendProceedEmail(eitem, onBehalfName);
        //   }
        //   else
        //   {
        //      EmailTemplete.sendProceedEmail(eitem);
        //   }

        //}
        #endregion


    }
}