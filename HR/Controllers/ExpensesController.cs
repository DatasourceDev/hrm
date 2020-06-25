using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.IO;
using System.Web.Configuration;
using System.Drawing;
using HR.Models;
using HR.Common;
using SBSModel.Models;
using SBSModel.Common;
using System.Threading.Tasks;
using SBSWorkFlowAPI.ModelsAndService;
using System.Data.Entity.SqlServer;
using OfficeOpenXml;
using SBSResourceAPI;
using SBSWorkFlowAPI.Constants;
using SBSWorkFlowAPI.Models;
using iTextSharp.text.html.simpleparser;
using iTextSharp.text;
using iTextSharp.text.pdf;
using System.Diagnostics;


namespace HR.Controllers
{
   [Authorize]
   [AllowAuthorized]
   public class ExpensesController : ControllerBase
   {
      [HttpGet]
      public ActionResult DashBoard()
      {
         var model = new ExpensesDashBoardViewModel();

         var userlogin = UserSession.getUser(HttpContext);
         if (userlogin == null)
            return errorPage(ERROR_CODE.ERROR_401_UNAUTHORIZED);

         //Validate Page Right
         var rightResult = base.validatePageRight(UserSession.RIGHT_A, "/Expenses/Application");
         if (rightResult.action != null) return rightResult.action;
         model.rights = rightResult.rights;

         var comService = new CompanyService();
         var eService = new ExpenseService();
         //model.ExpensesList = eService.getExpenseApplications(userlogin.Company_ID, userlogin.Profile_ID);

         var criteriaPending = new ExpenseCriteria()
         {
            Company_ID = userlogin.Company_ID,
            Profile_ID = userlogin.Profile_ID,
         };
         var presult = eService.LstExpenses(criteriaPending);
         if (presult.Object != null)
            model.ExpensesList = (List<Expenses_Application>)presult.Object;

         model.ExpensesBalanceList = new List<ExpensesBalanceViewModel>();
         model.ExpensesApplieList = new List<ExpensesApplieViewModel>();

         var com = comService.GetCompany(userlogin.Company_ID);
         if (com == null)
            return errorPage(ERROR_CODE.ERROR_401_UNAUTHORIZED);

         var eTypes = eService.getExpenseTypes(userlogin.Company_ID);

         foreach (var row in eTypes)
         {

            var docs = model.ExpensesList.Where(w => w.Expenses_Application_Document.Where(w2 => w2.Expenses_Config_ID == row.Expenses_Config_ID).Count() > 0);
            var amount = docs.Sum(s => s.Expenses_Application_Document.Where(w2 => w2.Expenses_Config_ID == row.Expenses_Config_ID).Select(s2 => (s2.Amount_Claiming.HasValue ? s2.Amount_Claiming.Value : 0)).Sum());

            var totalamount = row.Expenses_Config_Detail.Select(s => (s.Amount_Per_Year.HasValue ? s.Amount_Per_Year.Value : 0)).Sum();
            var PerMonth = row.Expenses_Config_Detail.Select(s => s.Select_Per_Month.Value).FirstOrDefault();
            if (PerMonth == true)
               totalamount = row.Expenses_Config_Detail.Select(s => (s.Amount_Per_Month.HasValue ? s.Amount_Per_Month.Value : 0)).Sum();

            model.ExpensesBalanceList.Add(new ExpensesBalanceViewModel()
            {
               Expenses_Type_Name = row.Expenses_Name,
               Amount = amount,
               Total_Amount = totalamount
            });

            model.ExpensesApplieList.Add(new ExpensesApplieViewModel()
            {
               Expenses_Type_Name = row.Expenses_Name,
               Amount = amount
            });
         }

         if (com.Currency != null) model.Currency_Code = com.Currency.Currency_Code;

         return View(model);
      }

      [HttpGet]
      public ActionResult Configuration(int[] Expenses, int[] ExpenseApprs, ServiceResult result, ExpensesConfigurationViewModel model, string apply, string tabAction = "")
      {

         var currentdate = StoredProcedure.GetCurrentDate();
         var userlogin = UserSession.getUser(HttpContext);
         if (userlogin == null)
            return errorPage(ERROR_CODE.ERROR_401_UNAUTHORIZED);

         //Validate Page Right
         var rightResult = base.validatePageRight(UserSession.RIGHT_A);
         if (rightResult.action != null) return rightResult.action;
         model.rights = rightResult.rights;
         model.result = result;
         model.tabAction = tabAction;

         var cbService = new ComboService();
         var eService = new ExpenseService();
         var aService = new SBSWorkFlowAPI.Service();

         model.departmentList = cbService.LstDepartment(userlogin.Company_ID, true);

         var criteria = new ExpensesCategoryCriteria() { Company_ID = userlogin.Company_ID };
         model.ExpensesCategoryList = eService.LstExpensesCategory(criteria);
         model.ExpensesTypeList = eService.getExpenseTypes(userlogin.Company_ID, model.search_Expenses_Type_Department, null, model.search_Expenses_Type);

         //edit By sun 29-06-2016
         if (tabAction == "category")
         {
            if (Expenses != null)
            {
               //Check use 
               foreach (var Expense in Expenses)
               {
                  var excat = eService.GetExpenseCategory(Expense);
                  if (excat != null)
                  {
                     if (apply == RecordStatus.Active | apply == RecordStatus.Inactive)
                        excat.Record_Status = apply;

                     else if (apply == UserSession.RIGHT_D)
                        excat.Record_Status = RecordStatus.Delete;

                     excat.Update_By = userlogin.User_Authentication.Email_Address;
                     excat.Update_On = currentdate;
                     model.result = eService.UpdateExpensesCategory(excat);
                  }
               }
               if (model.result.Code == ERROR_CODE.SUCCESS)
               {
                  if (apply == RecordStatus.Active | apply == RecordStatus.Inactive)
                     return RedirectToAction("Configuration", new { Code = model.result.Code, Msg = model.result.Msg, Field = model.result.Field, tabAction = "category" });
                  else if (apply == UserSession.RIGHT_D)
                     return RedirectToAction("Configuration", new { Code = ERROR_CODE.SUCCESS, Msg = new Success().getSuccess(ERROR_CODE.SUCCESS_DELETE), Field = Resource.Expenses_Category, tabAction = "category" });
               }
            }
         }

         //Added by Nay on 14-Jul-2015 
         //Purpose : to delete multiple Expense Types
         else if (tabAction == "expenses")
         {
            if (apply == UserSession.RIGHT_D)
            {
               apply = RecordStatus.Delete;
               if (Expenses != null)
               {
                  var chkRefHas = false;
                  foreach (var expense in Expenses)
                  {
                     if (eService.chkExpenseAppDocumentUsed(expense))
                     {
                        chkRefHas = true;
                        break;
                     }
                  }
                  if (chkRefHas)
                     return RedirectToAction("Configuration", new { Code = ERROR_CODE.ERROR_505_DELETE_ERROR, Msg = new Error().getError(ERROR_CODE.ERROR_505_DELETE_ERROR), Field = model.result.Field, tabAction = "expenses" });
                  else
                  {
                     model.result = eService.UpdateMultipleDeleteExpensesTypeStatus(Expenses, apply, userlogin.User_Authentication.Email_Address);
                     //  model.result = eService.MultipleDeleteExpenses_Type(Expenses);
                     return RedirectToAction("Configuration", new { Code = model.result.Code, Msg = model.result.Msg, Field = model.result.Field, tabAction = "expenses" });
                  }
               }
            }
         }

         //Added by Nay on 15-Jul-2015 
         //Purpose : to delete multiple Expense Approval
         else if (tabAction == "approval")
         {
            if (apply == UserSession.RIGHT_D)
            {
               if (ExpenseApprs != null)
               {
                  var chkProblem = false;
                  //check first is there any wrong records before delete!
                  foreach (var approval in ExpenseApprs)
                  {
                     var rwflow = aService.GetWorkflow(approval);
                     if (rwflow.Item2.IsSuccess && rwflow.Item1 != null)
                     {
                        if (rwflow.Item1.Requests.Count() > 0)
                        {
                           return RedirectToAction("Configuration", new { Code = ERROR_CODE.ERROR_505_DELETE_ERROR, Msg = new Error().getError(ERROR_CODE.ERROR_505_DELETE_ERROR), Field = Resource.Approval, tabAction = "approval" });
                        }
                     }
                  }
                  foreach (var approval in ExpenseApprs)
                  {
                     //var r = aService.DeleteWorkFlow(approval, userlogin.Profile_ID);
                     var r = aService.UpdateDeleteWorkFlowStatus(approval, userlogin.Profile_ID, apply);
                     if (!r.IsSuccess)
                     {
                        chkProblem = true;
                        break;
                     }
                  }
                  if (chkProblem)
                     return RedirectToAction("Configuration", new { Code = ERROR_CODE.ERROR_505_DELETE_ERROR, Msg = new Error().getError(ERROR_CODE.ERROR_505_DELETE_ERROR), Field = Resource.Approval, tabAction = "approval" });
                  else
                     return RedirectToAction("Configuration", new { Code = ERROR_CODE.SUCCESS, Msg = new Success().getSuccess(ERROR_CODE.SUCCESS_DELETE), Field = Resource.Approval, tabAction = "approval" });
               }
            }
         }
         //var aService = new SBSWorkFlowAPI.Service();
         if (model.search_Approval_Department.HasValue)
         {
            var r = aService.GetWorkflowByDepartment(userlogin.Company_ID.Value, model.search_Approval_Department.Value, ModuleCode.HR, ApprovalType.Expense);
            if (r.Item2.IsSuccess && r.Item1 != null)
            {
               model.ApprovalList = r.Item1;
            }
         }
         else
         {
            var r = aService.GetWorkflowByCompany(userlogin.Company_ID.Value, ModuleCode.HR, ApprovalType.Expense);
            if (r.Item2.IsSuccess && r.Item1 != null)
            {
               model.ApprovalList = r.Item1;
            }
         }

         return View(model);
      }

      public ActionResult AddNewExpensesDetail(int pIndex, string pDesignation, Nullable<int> pYear, Nullable<decimal> pAmountPerYear, bool pSelectAmount, Nullable<decimal> pAmount, Nullable<decimal> pPercentage, bool pSelectPerMonth, Nullable<decimal> pAmountPerMonth)
      {
         var userlogin = UserSession.getUser(HttpContext);
         var cbService = new ComboService();
         var companyService = new CompanyService();
         var model = new ExpensesTypeDetailViewModel()
         {
            Index = pIndex,
            Year_Service = pYear,
            Amount_Per_Year = pAmountPerYear,
            Select_Amount = pSelectAmount,
            Select_Percentage = !pSelectAmount,
            Amount = pAmount,
            Pecentage = pPercentage,
            Select_Per_Month = pSelectPerMonth,
            Amount_Per_Month = pAmountPerMonth,
         };

         pDesignation = Request.Params.Get("pDesignation[]");

         if (!string.IsNullOrEmpty(pDesignation))
         {
            var desinations = pDesignation.Split(',').Select(x => Int32.Parse(x)).ToArray();
            model.Designations = desinations;
         }
         model.designationList = cbService.LstDesignation(userlogin.Company_ID, true);

         return PartialView("ExpensesTypeDetailRow", model);
      }

      #region Expenses Category
      [HttpGet]
      public ActionResult ExpensesCategory(string eCid, string operation = "")
      {

         var currentdate = StoredProcedure.GetCurrentDate();
         var userlogin = UserSession.getUser(HttpContext);
         if (userlogin == null)
            return errorPage(ERROR_CODE.ERROR_401_UNAUTHORIZED);

         var model = new ExpensesCategoryViewModel();
         var eService = new ExpenseService();
         var cbService = new ComboService();
         model.operation = EncryptUtil.Decrypt(operation);
         model.ExpensesCategory_ID = NumUtil.ParseInteger(EncryptUtil.Decrypt(eCid));

         //Validate Page Right
         var rightResult = base.validatePageRight(model.operation, "/Expenses/Configuration");
         if (rightResult.action != null) return rightResult.action;
         model.rights = rightResult.rights;

         if (model.operation == UserSession.RIGHT_C)
         {

         }
         else if (model.operation == UserSession.RIGHT_U)
         {
            Expenses_Category excat = eService.GetExpenseCategory(model.ExpensesCategory_ID);
            if (excat == null)
               return errorPage(ERROR_CODE.ERROR_511_DATA_NOT_FOUND);

            model.ExpensesCategory_ID = excat.Expenses_Category_ID;
            model.Category_Name = excat.Category_Name;
            model.Category_Description = excat.Category_Description;
            model.Record_Status = excat.Record_Status;
         }
         else if (model.operation == UserSession.RIGHT_D)
         {

            Expenses_Category excat = eService.GetExpenseCategory(model.ExpensesCategory_ID);
            if (excat == null)
               return errorPage(ERROR_CODE.ERROR_511_DATA_NOT_FOUND);

            excat.Record_Status = RecordStatus.Delete;
            excat.Update_By = userlogin.User_Authentication.Email_Address;
            excat.Update_On = currentdate;

            model.result = eService.UpdateExpensesCategory(excat);
            if (model.result.Code == ERROR_CODE.SUCCESS)
               return RedirectToAction("Configuration", new { Code = ERROR_CODE.SUCCESS, Msg = new Success().getSuccess(ERROR_CODE.SUCCESS_DELETE), Field = Resource.Expenses_Category });
         }

         model.statusList = cbService.LstStatus();

         return View(model);
      }

      [HttpPost]
      [AllowAuthorized]
      public ActionResult ExpensesCategory(ExpensesCategoryViewModel model)
      {

         var currentdate = StoredProcedure.GetCurrentDate();
         var userlogin = UserSession.getUser(HttpContext);
         if (userlogin == null)
            return errorPage(ERROR_CODE.ERROR_401_UNAUTHORIZED);

         //Validate Page Right
         var rightResult = base.validatePageRight(model.operation, "/Expenses/Configuration");
         if (rightResult.action != null) return rightResult.action;
         model.rights = rightResult.rights;

         //-------data------------
         var eService = new ExpenseService();
         var cbService = new ComboService();

         var criteria = new ExpensesCategoryCriteria() { Company_ID = userlogin.Company_ID, Category_Name = model.Category_Name };
         var dupExCat = eService.LstExpensesCategory(criteria).FirstOrDefault(); ;
         if (dupExCat != null)
         {
            if (model.operation == UserSession.RIGHT_C)
               ModelState.AddModelError("Category_Name", Resource.Message_Is_Duplicated);
            else if (model.operation == UserSession.RIGHT_U)
            {
               if (dupExCat.Expenses_Category_ID != model.ExpensesCategory_ID)
                  ModelState.AddModelError("Category_Name", Resource.Message_Is_Duplicated);
            }
         }

         if (ModelState.IsValid)
         {
            var exCategory = new Expenses_Category();
            if (model.operation == UserSession.RIGHT_U)
            {
               exCategory = eService.GetExpenseCategory(model.ExpensesCategory_ID);
               if (exCategory == null)
                  return errorPage(ERROR_CODE.ERROR_511_DATA_NOT_FOUND);
            }
            exCategory.Category_Name = model.Category_Name;
            exCategory.Category_Description = model.Category_Description;
            exCategory.Record_Status = model.Record_Status;
            exCategory.Company_ID = userlogin.Company_ID;
            exCategory.Update_By = userlogin.User_Authentication.Email_Address;
            exCategory.Update_On = currentdate;

            if (model.operation == UserSession.RIGHT_C)
            {
               exCategory.Create_By = userlogin.User_Authentication.Email_Address;
               exCategory.Create_On = currentdate;
               model.result = eService.InsertExpensesCategory(exCategory);
            }
            else if (model.operation == UserSession.RIGHT_U)
            {
               model.result = eService.UpdateExpensesCategory(exCategory);
            }

            if (model.result.Code == ERROR_CODE.SUCCESS)
               return RedirectToAction("Configuration", new { Code = model.result.Code, Msg = model.result.Msg, Field = model.result.Field });
         }

         model.statusList = cbService.LstStatus();

         return View(model);
      }
      #endregion

      #region ExpensesType
      [HttpGet]
      public ActionResult ExpensesType(string eid, string operation = "")
      {

         var userlogin = UserSession.getUser(HttpContext);
         if (userlogin == null)
            return errorPage(ERROR_CODE.ERROR_401_UNAUTHORIZED);

         var model = new ExpensesTypeViewModel();
         model.operation = EncryptUtil.Decrypt(operation);
         var expensesTypeID = NumUtil.ParseInteger(EncryptUtil.Decrypt(eid));

         //Validate Page Right
         var rightResult = base.validatePageRight(model.operation, "/Expenses/Configuration");
         if (rightResult.action != null) return rightResult.action;
         model.rights = rightResult.rights;

         var eService = new ExpenseService();
         var cbService = new ComboService();
         var dpService = new DepartmentService();

         //Added By sun 25-08-2015
         model.ExpensesCategoryList = cbService.LstExpensesCategory(userlogin.Company_ID, true);
         model.departmentList = cbService.LstDepartment(userlogin.Company_ID);
         model.designationList = cbService.LstDesignation(userlogin.Company_ID, true);
         model.claimtypeList = cbService.LstPer();
         model.uomList = cbService.LstLookup(ComboType.UOM, userlogin.Company_ID);

         var com = new CompanyService().GetCompany(userlogin.Company_ID);
         if (com != null)
         {
            if (com.Currency != null)
               model.Company_Currency = com.Currency.Currency_Code;
         }

         if (model.operation == UserSession.RIGHT_C)
         {

         }
         else if (model.operation == UserSession.RIGHT_U)
         {
            Expenses_Config expense = eService.GetExpenseType(expensesTypeID);
            if (expense == null)
               return errorPage(ERROR_CODE.ERROR_511_DATA_NOT_FOUND);

            if (expense != null)
            {
               model.eid = expensesTypeID;
               model.Expenses_Name = expense.Expenses_Name;
               model.Expenses_Description = expense.Expenses_Description;
               model.Claimable_Type = expense.Claimable_Type;
               model.Allowed_Probation = expense.Allowed_Probation.HasValue ? expense.Allowed_Probation.Value : false;
               model.Allowed_Over_Amount_Per_Year = expense.Allowed_Over_Amount_Per_Year.HasValue ? expense.Allowed_Over_Amount_Per_Year.Value : false;
               model.Is_MileAge = expense.Is_MileAge;
               model.UOM_ID = expense.UOM_ID;
               model.Amount_Per_UOM = expense.Amount_Per_UOM;
               model.Is_Accumulative = expense.Is_Accumulative.HasValue ? expense.Is_Accumulative.Value : false;
               //Added By sun 25-08-2015
               model.Expenses_Category_ID = expense.Expenses_Category_ID;
               model.Department_ID = expense.Department_ID;

               if (expense.Department_ID.HasValue && expense.Department_ID.Value > 0)
                  model.isDepartment = 1;
               else
                  model.isDepartment = 0;

               var eTypeDetails = new List<ExpensesTypeDetailViewModel>();
               var currGroup = 0;
               foreach (var row in expense.Expenses_Config_Detail.OrderBy(o => o.Group_ID))
               {
                  if (row.Group_ID.HasValue)
                  {
                     if (currGroup != row.Group_ID)
                     {
                        currGroup = row.Group_ID.Value;
                        var detailsrows = expense.Expenses_Config_Detail.Where(w => w.Group_ID == currGroup);
                        var etypeDetail = new ExpensesTypeDetailViewModel()
                        {
                           Designations = detailsrows.Select(s => s.Designation_ID.HasValue ? s.Designation_ID.Value : 0).ToArray(),
                           Group_ID = row.Group_ID,
                           Expenses_Config_ID = row.Expenses_Config_ID,
                           Year_Service = row.Year_Service
                        };

                        if (row.Select_Amount.HasValue && row.Select_Amount.Value)
                        {
                           etypeDetail.Select_Amount = true;
                           etypeDetail.Select_Percentage = false;
                           etypeDetail.Amount = row.Amount;
                        }
                        else
                        {
                           etypeDetail.Select_Amount = false;
                           etypeDetail.Select_Percentage = false;
                           etypeDetail.Pecentage = row.Pecentage;
                        }

                        if (row.Select_Per_Month.HasValue && row.Select_Per_Month.Value)
                        {
                           etypeDetail.Select_Per_Month = true;
                           etypeDetail.Amount_Per_Month = row.Amount_Per_Month;
                        }
                        else
                        {
                           etypeDetail.Select_Per_Month = false;
                           etypeDetail.Amount_Per_Year = row.Amount_Per_Year;
                        }

                        etypeDetail.Row_Type = RowType.EDIT;
                        eTypeDetails.Add(etypeDetail);
                     }
                  }
               }
               model.Detail_Rows = eTypeDetails.ToArray();
            }
         }
         else if (model.operation == UserSession.RIGHT_D)
         {
            var apply = RecordStatus.Delete;
            model.result = eService.UpdateDeleteExpenseTypeStatus(expensesTypeID, apply, userlogin.User_Authentication.Email_Address);
            if (model.result.Code == ERROR_CODE.SUCCESS)
            {
               return RedirectToAction("Configuration", new { Code = model.result.Code, Msg = model.result.Msg, Field = model.result.Field, tabAction = "expenses" });
            }
         }

         return View(model);
      }

      [HttpPost]
      [ValidateAntiForgeryToken]
      public ActionResult ExpensesType(ExpensesTypeViewModel model)
      {

         var userlogin = UserSession.getUser(HttpContext);
         if (userlogin == null)
            return errorPage(ERROR_CODE.ERROR_401_UNAUTHORIZED);

         //Validate Page Right
         var rightResult = base.validatePageRight(model.operation, "/Expenses/Configuration");
         if (rightResult.action != null) return rightResult.action;
         model.rights = rightResult.rights;

         ExpenseService eService = new ExpenseService();

         if (string.IsNullOrEmpty(model.Expenses_Name))
            ModelState.AddModelError("Expenses_Name", Resource.Message_Is_Required);

         if (model.isDepartment == 1)
         {
            if (!model.Department_ID.HasValue || model.Department_ID.Value == 0)
               ModelState.AddModelError("Department_ID", Resource.Message_Is_Required);
         }

         if (model.Is_MileAge.HasValue && model.Is_MileAge.Value)
         {
            if (!model.UOM_ID.HasValue || model.UOM_ID.Value <= 0)
               ModelState.AddModelError("UOM_ID", Resource.Message_Is_Required);

            if (!model.Amount_Per_UOM.HasValue || model.Amount_Per_UOM.Value <= 0)
               ModelState.AddModelError("Amount_Per_UOM", Resource.Message_Is_Required);
         }
         else
         {
            model.UOM_ID = null;
            model.Amount_Per_UOM = null;
         }

         if (model.Detail_Rows != null && model.Detail_Rows.Length > 0)
         {
            for (var i = 0; i < model.Detail_Rows.Length; i++)
            {
               var row = model.Detail_Rows[i];
               var type = model.Detail_Rows[i].Row_Type;
               if (type != RowType.DELETE)
               {
                  if (!row.Year_Service.HasValue || row.Year_Service.Value < 0)
                  {
                     ModelState.AddModelError("Detail_Rows[" + i + "].Year_Service", Resource.Message_Is_Required);
                  }
                  if (row.Select_Per_Month)
                  {
                     if (!row.Amount_Per_Month.HasValue || row.Amount_Per_Month.Value < 0)
                     {
                        ModelState.AddModelError("Detail_Rows[" + i + "].Amount_Per_Month", Resource.Message_Is_Required);
                     }
                     row.Amount_Per_Year = 0;
                  }
                  else
                  {
                     if (!row.Amount_Per_Year.HasValue || row.Amount_Per_Year.Value < 0)
                     {
                        ModelState.AddModelError("Detail_Rows[" + i + "].Amount_Per_Year", Resource.Message_Is_Required);
                     }
                     row.Amount_Per_Month = 0;
                  }

                  if (row.Designations == null || row.Designations.Count() == 0)
                  {
                     ModelState.AddModelError("Detail_Rows[" + i + "].Designations", Resource.Message_Is_Required);
                  }
                  else
                  {
                     foreach (var drow in row.Designations)
                     {
                        var duprows = model.Detail_Rows.Where(w => w.Year_Service == row.Year_Service & w.Group_ID != row.Group_ID & w.Row_Type != RowType.DELETE);
                        if (drow == 0)
                        {
                           // All
                           var dup = duprows.FirstOrDefault();
                           if (dup != null)
                           {
                              ModelState.AddModelError("Detail_Rows[" + i + "].Designations", Resource.The + " " + Resource.Designation + " " + Resource.Field + " " + Resource.And + " " + Resource.Year_Service + " " + Resource.Field + " " + Resource.Is_Duplicated_Lower);
                           }
                        }
                        else
                        {
                           var dup = duprows.Where(w => (w.Designations != null ? w.Designations.Contains(drow) : false)).FirstOrDefault();
                           if (dup != null)
                           {
                              ModelState.AddModelError("Detail_Rows[" + i + "].Designations", Resource.The + " " + Resource.Designation + " " + Resource.Field + " " + Resource.And + " " + Resource.Year_Service + " " + Resource.Field + " " + Resource.Is_Duplicated_Lower);
                           }
                        }
                     }
                  }
               }
            }
         }

         var dupex = eService.getExpenseTypes(userlogin.Company_ID, null, null, model.Expenses_Name).FirstOrDefault();
         if (dupex != null)
         {
            if (model.operation == UserSession.RIGHT_C)
               ModelState.AddModelError("Expenses_Name", Resource.Message_Is_Duplicated);
            else if (model.operation == UserSession.RIGHT_U)
            {
               if (dupex.Expenses_Config_ID != model.eid)
                  ModelState.AddModelError("Expenses_Name", Resource.Message_Is_Duplicated);
            }
         }

         var str = GetErrorModelState();
         if (ModelState.IsValid)
         {
            var currentdate = StoredProcedure.GetCurrentDate();
            var details = new List<Expenses_Type_Detail>();
            if (model.Detail_Rows != null)
            {
               foreach (var row in model.Detail_Rows)
               {
                  details.Add(new Expenses_Type_Detail()
                  {
                     Amount = row.Amount,
                     Select_Per_Month = row.Select_Per_Month,
                     Amount_Per_Month = row.Amount_Per_Month,
                     Amount_Per_Year = row.Amount_Per_Year,
                     Designations = row.Designations,
                     Expenses_Config_ID = row.Expenses_Config_ID,
                     Group_ID = row.Group_ID,
                     Pecentage = row.Pecentage,
                     Row_Type = row.Row_Type,
                     Select_Amount = row.Select_Amount,
                     Select_Percentage = row.Select_Percentage,
                     Year_Service = row.Year_Service
                  });
               }
            }

            Expenses_Config expense = new Expenses_Config();
            if (model.operation == UserSession.RIGHT_U)
            {
               expense = eService.GetExpenseType(model.eid);
               if (expense == null)
                  return errorPage(ERROR_CODE.ERROR_511_DATA_NOT_FOUND);

               if (expense.Company_ID != userlogin.Company_ID)
                  return errorPage(ERROR_CODE.ERROR_401_UNAUTHORIZED);
            }

            expense.Company_ID = userlogin.Company_ID;
            expense.Expenses_Name = model.Expenses_Name;
            expense.Expenses_Description = model.Expenses_Description;
            expense.Claimable_Type = model.Claimable_Type;
            expense.Allowed_Probation = model.Allowed_Probation;
            expense.Allowed_Over_Amount_Per_Year = model.Allowed_Over_Amount_Per_Year;
            expense.Is_MileAge = model.Is_MileAge;
            expense.UOM_ID = model.UOM_ID;
            expense.Amount_Per_UOM = model.Amount_Per_UOM;
            expense.Is_Accumulative = model.Is_Accumulative;
            expense.Expenses_Category_ID = model.Expenses_Category_ID;
            expense.Update_By = userlogin.User_Authentication.Email_Address;
            expense.Update_On = currentdate;

            if (model.isDepartment == 1)
               expense.Department_ID = model.Department_ID;
            else
               expense.Department_ID = null;

            if (model.operation == UserSession.RIGHT_C)
            {
               expense.Create_By = userlogin.User_Authentication.Email_Address;
               expense.Create_On = currentdate;
               model.result = eService.InsertExpenseType(expense, details.ToArray());
               if (model.result.Code == ERROR_CODE.SUCCESS)
               {
                  return RedirectToAction("Configuration", new { Code = model.result.Code, Msg = model.result.Msg, Field = model.result.Field, tabAction = "expenses" });
               }
            }
            else if (model.operation == UserSession.RIGHT_U)
            {
               model.result = eService.UpdateExpenseType(expense, details.ToArray());
               if (model.result.Code == ERROR_CODE.SUCCESS)
               {
                  return RedirectToAction("Configuration", new { Code = model.result.Code, Msg = model.result.Msg, Field = model.result.Field, tabAction = "expenses" });
               }
            }
         }

         var cbService = new ComboService();
         model.departmentList = cbService.LstDepartment(userlogin.Company_ID);
         model.designationList = cbService.LstDesignation(userlogin.Company_ID, true);
         model.claimtypeList = cbService.LstPer();
         model.ExpensesCategoryList = cbService.LstExpensesCategory(userlogin.Company_ID, true);
         model.uomList = cbService.LstLookup(ComboType.UOM, userlogin.Company_ID);

         return View(model);
      }
      #endregion

      #region Application
      [HttpGet]
      public ActionResult Application(ServiceResult result, string operation, string eID, string pAppr = "", string pYear = null)
      {
         var currentdate = StoredProcedure.GetCurrentDate();
         var userlogin = UserSession.getUser(HttpContext);
         if (userlogin == null)
            return errorPage(ERROR_CODE.ERROR_401_UNAUTHORIZED);

         ExpensesViewModel model = new ExpensesViewModel();
         model.operation = EncryptUtil.Decrypt(operation);
         if (string.IsNullOrEmpty(model.operation))
            model.operation = Operation.C;

         var expenseID = NumUtil.ParseInteger(EncryptUtil.Decrypt(eID));
         var ApprStatus = EncryptUtil.Decrypt(pAppr);

         //Validate Page Right
         var rightResult = base.validatePageRight(UserSession.RIGHT_A);
         if (rightResult.action != null) return rightResult.action;
         model.rights = rightResult.rights;
         model.result = result;

         var empService = new EmployeeService();
         var emp = empService.GetEmployeeProfileByProfileID(userlogin.Profile_ID);
         if (emp == null)
            return errorPage(ERROR_CODE.ERROR_16_NO_EMPLOYEE_PROFILE);

         var com = new CompanyService().GetCompany(userlogin.Company_ID.Value);
         if (com != null && !com.Currency_ID.HasValue)
            return errorPage(ERROR_CODE.ERROR_13_NO_DEFAULT_CURRENCY);

         var eService = new ExpenseService();
         var hService = new EmploymentHistoryService();
         var cbService = new ComboService();

         var userhist = hService.GetCurrentEmploymentHistory(emp.Employee_Profile_ID);
         if (userhist == null)
            return errorPage(ERROR_CODE.ERROR_14_NO_EMPLOYEE_HIST);

         var yearSerivce = empService.GetYearService(emp.Employee_Profile_ID);

         //Edit  by sun 12-02-2016
         var year = DateUtil.ToDate(pYear);
         if (year == null)
            model.CkYear = currentdate.Year.ToString();
         else
            model.CkYear = year.Value.Year.ToString();

         model.currencyList = cbService.LstCurrencyByCompany(userlogin.Company_ID.Value, model.CkYear);
         model.EmployeeUnderMeList = cbService.LstEmpUnderMe(userlogin.Profile_ID);
         model.expensesConfigList = eService.getExpenseTypes(userlogin.Company_ID.Value, userhist.Department_ID, userhist.Designation_ID, null, yearSerivce, null);
         //********  Smart Dev  ********//
         model.JobCostlst = cbService.LstJobCost(userlogin.Company_ID, true);
         model.TaxTypelst = cbService.LstTaxType(false);
         model.AmountTypelst = cbService.LstAmountType();

         model.Default_Currency_ID = com.Currency_ID;
         model.Default_Currency_Code = com.Currency != null ? com.Currency.Currency_Code : "";
         model.Employee_Profile_ID = emp.Employee_Profile_ID;
         model.ApprStatus = ApprStatus;
         model.Default_Date = DateUtil.ToDisplayDate(currentdate);
         model.Date_Applied = DateUtil.ToDisplayDate(currentdate);
         model.isRejectPopUp = false;

         if (expenseID > 0 & model.operation == UserSession.RIGHT_U)
         {
            var expense = eService.getExpenseApplication(expenseID);
            //Chk right on Expenses_Application_Document (of the USER)
            if (expense == null)
               return errorPage(ERROR_CODE.ERROR_511_DATA_NOT_FOUND);

            model.Name = UserSession.GetUserName(expense.Employee_Profile.User_Profile);
            model.Email = expense.Employee_Profile.User_Profile.User_Authentication.Email_Address;
            model.Expenses_ID = expenseID;
            model.Expenses_No = expense.Expenses_No;
            model.Expenses_Title = expense.Expenses_Title;
            model.Date_Applied = DateUtil.ToDisplayDate(expense.Date_Applied);
            model.Overall_Status = expense.Overall_Status;
            model.Approval_Status_1st = expense.Approval_Status_1st;
            model.Approval_Status_2st = expense.Approval_Status_2st;
            model.Approval_Cancel_Status = expense.Approval_Cancel_Status;
            model.Cancel_Status = expense.Cancel_Status;
            model.Request_Cancel_ID = expense.Request_Cancel_ID;
            model.Request_ID = expense.Request_ID;
            model.OnBehalf_Employee_Profile_ID = expense.Employee_Profile_ID;
            model.OnBehalf_Profile_ID = expense.Employee_Profile.Profile_ID;

            //******** Start Workflow Draft  ********//
            if (model.Overall_Status == WorkflowStatus.Draft || (model.ApprStatus != "Manage" && model.Overall_Status == WorkflowStatus.Rejected))
               model.ApprStatus = WorkflowStatus.Draft;
            //******** End Workflow Draft  ********//

            if (expense.Request_ID.HasValue)
            {
               var aService = new SBSWorkFlowAPI.Service();
               var r = aService.GetMyRequests(userlogin.Company_ID, null, null, ModuleCode.HR, ApprovalType.Expense, expense.Expenses_Application_ID);
               if (r.Item2.IsSuccess && r.Item1 != null)
                  model.Expenses_Request = r.Item1;
            }
            //Edit by Jane 27-04-2016
            if (expense.Supervisor.HasValue)
            {
               model.Supervisor = expense.Supervisor;
               var sup = empService.GetEmployeeProfile2(expense.Supervisor);
               if (sup != null)
                  model.Supervisor_Name = AppConst.GetUserName(sup.User_Profile);
            }

            var details = new List<ExpensesDetailViewModel>();
            foreach (var row in expense.Expenses_Application_Document.OrderBy(o => o.Expenses_Date).ToList())
            {
               var detail = new ExpensesDetailViewModel()
               {
                  Amount_Claiming = row.Amount_Claiming,
                  Expenses_Application_Document_ID = row.Expenses_Application_Document_ID,
                  Expenses_Config_ID = row.Expenses_Config_ID,
                  Expenses_Date = DateUtil.ToDisplayDate(row.Expenses_Date),
                  Doc_No = row.Doc_No,
                  Expenses_Type_Name = row.Expenses_Config_ID.HasValue ? row.Expenses_Config.Expenses_Name : "",
                  Expenses_Type_Desc = row.Expenses_Config_ID.HasValue ? row.Expenses_Config.Expenses_Description : "",
                  Notes = row.Reasons,
                  Row_Type = UserSession.RIGHT_U,
                  Selected_Currency = row.Selected_Currency,
                  Tax = row.Tax,
                  Mileage = row.Mileage,
                  Total_Amount = row.Total_Amount,
                  Department_ID = row.Department_ID,
                  //********  Smart Dev  ********//
                  Job_Cost_ID = row.Job_Cost_ID,
                  Withholding_Tax = row.Withholding_Tax,
                  Tax_Type = row.Tax_Type,
                  Withholding_Tax_Amount = row.Withholding_Tax_Amount,
                  Tax_Amount = row.Tax_Amount,
                  Tax_Amount_Type = row.Tax_Amount_Type,
                  Withholding_Tax_Type = row.Withholding_Tax_Type,
                  Job_Cost_Name = (row.Job_Cost_ID.HasValue && row.Job_Cost != null) ? row.Job_Cost.Indent_Name : "",
               };

               if (row.Upload_Receipt != null && row.Upload_Receipt.Count > 0)
               {
                  var uploadfile = row.Upload_Receipt.FirstOrDefault();
                  if (uploadfile != null)
                  {
                     detail.Upload_Receipt_Name = uploadfile.File_Name;
                     detail.Upload_Receipt_ID = uploadfile.Upload_Receipt_ID;
                     if (uploadfile.Receipt != null)
                        detail.Upload_Receipt = Convert.ToBase64String(uploadfile.Receipt);
                  }
               }
               details.Add(detail);
            }
            model.Detail_Rows = details.ToArray();
         }
         else
         {
            //******** Start Workflow Draft  ********//
            model.ApprStatus = WorkflowStatus.Draft;
            //******** End Workflow Draft  ********//
         }
         return View(model);
      }

      [HttpPost]
      [ValidateAntiForgeryToken]
      public ActionResult Application(ExpensesViewModel model, HttpPostedFileBase file, string pStatus)
      {
         if (model.ApprStatus == "Manage")
            return ApplicationMngt(model, pStatus);
         else
            return ApplicationNew(model, pStatus);
      }

      private ActionResult ApplicationMngt(ExpensesViewModel model, string pStatus)
      {
         var userlogin = UserSession.getUser(HttpContext);
         if (userlogin == null)
            return errorPage(ERROR_CODE.ERROR_401_UNAUTHORIZED);

         var eService = new ExpenseService();
         var cbService = new ComboService();
         var empService = new EmployeeService();
         var hService = new EmploymentHistoryService();
         var uService = new UserService();
         var currentdate = StoredProcedure.GetCurrentDate();
         var cpService = new CompanyService();

         var com = cpService.GetCompany(userlogin.Company_ID);
         if (com == null)
            return errorPage(ERROR_CODE.ERROR_511_DATA_NOT_FOUND, Resource.Company);

         var ex = eService.getExpenseApplication(model.Expenses_ID);
         if (ex == null)
            return errorPage(ERROR_CODE.ERROR_511_DATA_NOT_FOUND, Resource.Expenses);

         if (ex.Cancel_Status == WorkflowStatus.Cancelled)
            return errorPage(ERROR_CODE.ERROR_511_DATA_NOT_FOUND, Resource.Expenses);

         if (ex.Overall_Status == WorkflowStatus.Rejected)
            return errorPage(ERROR_CODE.ERROR_511_DATA_NOT_FOUND, Resource.Expenses);

         var hist = hService.GetCurrentEmploymentHistory(ex.Employee_Profile_ID);
         if (hist == null)
            return errorPage(ERROR_CODE.ERROR_511_DATA_NOT_FOUND, Resource.Employment_History);

         var user = uService.getUser(hist.Employee_Profile.Profile_ID, false);
         if (user == null)
            return errorPage(ERROR_CODE.ERROR_511_DATA_NOT_FOUND, Resource.User);

         var aService = new SBSWorkFlowAPI.Service();
         var status = pStatus;

         var Ac_Code = "E" + ex.Expenses_Application_ID + userlogin.Profile_ID + "_";
         ex.Update_By = userlogin.User_Authentication.Email_Address;
         ex.Update_On = currentdate;
         if (string.IsNullOrEmpty(ex.Cancel_Status))
         {
            if (model.Request_ID.HasValue)
            {
               #region Workflow
               var action = new ActionItem();
               action.Actioner_Profile_ID = userlogin.Profile_ID;
               action.Email = userlogin.User_Authentication.Email_Address;
               action.Name = UserSession.GetUserName(userlogin);
               action.Request_ID = model.Request_ID.Value;
               if (pStatus == WorkflowStatus.Approved)
               {
                  action.IsApprove = true;
                  action.Action = WorkflowAction.Approve;
               }
               else
               {
                  if (string.IsNullOrEmpty(model.Remark_Rej))
                  {
                     ModelState.AddModelError("Remark_Rej", Resource.The + " " + Resource.Remark + " " + Resource.Field + " " + Resource.Is_Rrequired_Lower);
                     model.isRejectPopUp = true;
                  }
                  else
                     model.isRejectPopUp = false;

                  action.IsApprove = false;
                  action.Remarks = model.Remark_Rej;
                  action.Action = WorkflowAction.Reject;
               }
               if (ModelState.IsValid)
               {
                  var r = aService.SubmitRequestAction(action);
                  if (r.IsSuccess)
                  {
                     ex.Approver = getApprovalStrIDs(ex.Approver, userlogin.Profile_ID.ToString());
                     if (action.Status == WorkflowStatus.Closed)
                     {
                        ex.Next_Approver = null;
                        ex.Overall_Status = WorkflowStatus.Closed;
                        model.result = eService.updateExpenseApplication(ex);
                        if (model.result.Code == ERROR_CODE.SUCCESS)
                        {
                           uService.ExpireActivationByPrefix(Ac_Code);
                           sendProceedEmail(ex, com, user, userlogin, hist, action.Status, null);
                           return RedirectToAction("ExpensesManagement", new { Code = ERROR_CODE.SUCCESS, Msg = new Success().getSuccess(ERROR_CODE.SUCCESS_APPROVE), Field = Resource.Expense });
                        }
                     }
                     else if (action.Status == WorkflowStatus.Rejected)
                     {
                        ex.Next_Approver = null;
                        ex.Overall_Status = WorkflowStatus.Rejected;
                        model.result = eService.updateExpenseApplication(ex);
                        if (model.result.Code == ERROR_CODE.SUCCESS)
                        {
                           uService.ExpireActivationByPrefix(Ac_Code);
                           sendProceedEmail(ex, com, user, userlogin, hist, action.Status, null);
                           return RedirectToAction("ExpensesManagement", new { Code = ERROR_CODE.SUCCESS, Msg = new Success().getSuccess(ERROR_CODE.SUCCESS_REJECT), Field = Resource.Expense });
                        }
                     }
                     else
                     {
                        var mstr = "";
                        if (action.NextApprover == null)
                        {
                           #region Indent flow
                           var haveSendRequestEmail = false;
                           if (action.IndentValue != null && action.IndentValue.IsIndent && action.IndentValue.SendRequest)
                              haveSendRequestEmail = true;

                           if (haveSendRequestEmail)
                           {
                              ex.Next_Approver = null;
                              List<IndentItem> IndentItems = getIndentSupervisor(ex.Expenses_Application_ID);
                              if (IndentItems != null && IndentItems.Count > 0)
                              {
                                 foreach (var row in IndentItems)
                                 {
                                    if (action.Actioner_Profile_ID == row.Requestor_Profile_ID)
                                       continue;

                                    var param = new Dictionary<string, object>();
                                    param.Add("expID", ex.Expenses_Application_ID);
                                    param.Add("appID", row.Requestor_Profile_ID);
                                    param.Add("empID", ex.Employee_Profile_ID);
                                    param.Add("reqID", ex.Request_ID);
                                    param.Add("status", WorkflowStatus.Approved);
                                    param.Add("code", uService.GenActivateCode("E" + ex.Expenses_Application_ID + row.Requestor_Profile_ID + "_", userlogin.User_Authentication.User_Authentication_ID));

                                    var linkApp = GenerateActionLink("Approval", "ProcessWorkflow", param);
                                    param["status"] = WorkflowStatus.Rejected;
                                    var linkRej = GenerateActionLink("Approval", "ProcessWorkflow", param);

                                    var appr = uService.getUser(row.Requestor_Profile_ID, false);
                                    if (appr != null)
                                       sendRequestEmail(ex, com, appr, user, hist, WorkflowStatus.Submitted, null, linkApp, linkRej);

                                    mstr = getApprovalStrIDs(mstr, row.Requestor_Profile_ID.ToString());
                                 }
                              }
                           }
                           else
                           {
                              var str = getApprovalStrIDs(null, userlogin.Profile_ID.ToString());
                              if (!string.IsNullOrEmpty(str))
                                 mstr = ex.Next_Approver.Replace(str, "|");
                           }
                           #endregion
                        }
                        else
                        {
                           #region Normal flow
                           var param = new Dictionary<string, object>();
                           param.Add("expID", ex.Expenses_Application_ID);
                           param.Add("appID", action.NextApprover.Profile_ID);
                           param.Add("empID", ex.Employee_Profile_ID);
                           param.Add("reqID", ex.Request_ID);
                           param.Add("status", WorkflowStatus.Approved);
                           param.Add("code", uService.GenActivateCode("E" + ex.Expenses_Application_ID + action.NextApprover.Profile_ID + "_", userlogin.User_Authentication.User_Authentication_ID));

                           var linkApp = GenerateActionLink("Approval", "ProcessWorkflow", param);
                           param["status"] = WorkflowStatus.Rejected;
                           var linkRej = GenerateActionLink("Approval", "ProcessWorkflow", param);

                           var appr = uService.getUser(action.NextApprover.Profile_ID, false);
                           if (appr != null)
                              sendRequestEmail(ex, com, appr, user, hist, WorkflowStatus.Submitted, null, linkApp, linkRej);

                           if (action.NextApprover.Profile_ID != userlogin.Profile_ID)
                              mstr = getApprovalStrIDs(mstr, action.NextApprover.Profile_ID.ToString());
                           #endregion
                        }

                        ex.Next_Approver = null;
                        if (!string.IsNullOrEmpty(mstr))
                           ex.Next_Approver = mstr;


                        model.result = eService.updateExpenseApplication(ex);
                        uService.ExpireActivationByPrefix(Ac_Code);
                        return RedirectToAction("ExpensesManagement", new { Code = ERROR_CODE.SUCCESS, Msg = new Success().getSuccess(ERROR_CODE.SUCCESS_APPROVE), Field = Resource.Expense });
                     }

                  }
               }
               #endregion
            }
            else if (hist.Supervisor.HasValue)
            {
               #region Supervisor
               /*approval by supervisor*/
               if (pStatus == WorkflowStatus.Approved)
                  ex.Overall_Status = WorkflowStatus.Closed;
               else
                  ex.Overall_Status = WorkflowStatus.Rejected;

               ex.Next_Approver = null;
               ex.Approver = getApprovalStrIDs(null, userlogin.Profile_ID.ToString());
               model.result = eService.updateExpenseApplication(ex);
               if (model.result.Code == ERROR_CODE.SUCCESS)
               {
                  uService.ExpireActivationByPrefix(Ac_Code);
                  sendProceedEmail(ex, com, user, userlogin, hist, ex.Overall_Status, null);
                  if (pStatus == WorkflowStatus.Approved)
                     return RedirectToAction("ExpensesManagement", new { Code = ERROR_CODE.SUCCESS, Msg = new Success().getSuccess(ERROR_CODE.SUCCESS_APPROVE), Field = Resource.Expense });
                  else
                     return RedirectToAction("ExpensesManagement", new { Code = ERROR_CODE.SUCCESS, Msg = new Success().getSuccess(ERROR_CODE.SUCCESS_REJECT), Field = Resource.Expense });
               }
               #endregion
            }
         }
         else
         {
            /* approve canncel workflow*/
            if (model.Request_Cancel_ID.HasValue && model.Request_Cancel_ID.Value > 0)
            {
               #region Workflow
               var action = new ActionItem();
               action.Actioner_Profile_ID = userlogin.Profile_ID;
               action.Email = userlogin.User_Authentication.Email_Address;
               action.Name = UserSession.GetUserName(userlogin);
               action.Request_ID = model.Request_Cancel_ID.Value;
               if (pStatus == WorkflowStatus.Approved)
               {
                  action.IsApprove = true;
                  action.Action = WorkflowAction.Approve;
               }
               else
               {
                  if (string.IsNullOrEmpty(model.Remark_Rej))
                  {
                     ModelState.AddModelError("Remark_Rej", Resource.The + " " + Resource.Remark + " " + Resource.Field + " " + Resource.Is_Rrequired_Lower);
                     model.isRejectPopUp = true;
                  }
                  else
                     model.isRejectPopUp = false;

                  action.IsApprove = false;
                  action.Remarks = model.Remark_Rej;
                  action.Action = WorkflowAction.Reject;
               }

               if (ModelState.IsValid)
               {
                  var r = aService.SubmitRequestAction(action);
                  if (r.IsSuccess)
                  {
                     ex.Approver = getApprovalStrIDs(ex.Approver, userlogin.Profile_ID.ToString());
                     if (action.Status == WorkflowStatus.Closed)
                     {
                        ex.Next_Approver = null;
                        ex.Cancel_Status = WorkflowStatus.Cancelled;
                        model.result = eService.updateExpenseApplication(ex);
                        if (model.result.Code == ERROR_CODE.SUCCESS)
                        {
                           uService.ExpireActivationByPrefix(Ac_Code);
                           sendProceedEmail(ex, com, user, userlogin, hist, WorkflowStatus.Cancelled, null);
                           return RedirectToAction("ExpensesManagement", new { Code = ERROR_CODE.SUCCESS, Msg = new Success().getSuccess(ERROR_CODE.SUCCESS_APPROVE), Field = Resource.Expense });
                        }
                     }
                     else if (action.Status == WorkflowStatus.Rejected)
                     {
                        ex.Next_Approver = null;
                        ex.Cancel_Status = WorkflowStatus.Cancellation_Rejected;
                        model.result = eService.updateExpenseApplication(ex);
                        if (model.result.Code == ERROR_CODE.SUCCESS)
                        {
                           uService.ExpireActivationByPrefix(Ac_Code);
                           sendProceedEmail(ex, com, user, userlogin, hist, WorkflowStatus.Cancellation_Rejected, null);
                           return RedirectToAction("ExpensesManagement", new { Code = ERROR_CODE.SUCCESS, Msg = new Success().getSuccess(ERROR_CODE.SUCCESS_CANCEL_REJECT), Field = Resource.Expense });
                        }
                     }
                     else
                     {
                        var nextappstr = "";
                        if (action.NextApprover == null)
                        {
                           #region Indent flow
                           var haveSendRequestEmail = false;
                           if (action.IndentValue != null && action.IndentValue.IsIndent && action.IndentValue.SendRequest)
                              haveSendRequestEmail = true;

                           if (haveSendRequestEmail)
                           {
                              ex.Next_Approver = null;
                              List<IndentItem> IndentItems = getIndentSupervisor(ex.Expenses_Application_ID);
                              if (IndentItems != null && IndentItems.Count > 0)
                              {
                                 foreach (var row in IndentItems)
                                 {

                                    if (action.Actioner_Profile_ID == row.Requestor_Profile_ID)
                                       continue;

                                    var param = new Dictionary<string, object>();
                                    param.Add("expID", ex.Expenses_Application_ID);
                                    param.Add("appID", row.Requestor_Profile_ID);
                                    param.Add("empID", ex.Employee_Profile_ID);
                                    param.Add("reqcancelID", action.Request_ID);
                                    param.Add("cancelStatus", WorkflowStatus.Cancelled);
                                    param.Add("code", uService.GenActivateCode("E" + ex.Expenses_Application_ID + row.Requestor_Profile_ID + "_", userlogin.User_Authentication.User_Authentication_ID));
                                    var linkApp = GenerateActionLink("Approval", "ProcessWorkflow", param);
                                    param["cancelStatus"] = WorkflowStatus.Rejected;
                                    var linkRej = GenerateActionLink("Approval", "ProcessWorkflow", param);

                                    var appr = uService.getUser(row.Requestor_Profile_ID, false);
                                    if (appr != null)
                                       sendRequestEmail(ex, com, appr, user, hist, ex.Cancel_Status, null, linkApp, linkRej);

                                    nextappstr = getApprovalStrIDs(nextappstr, row.Requestor_Profile_ID.ToString());
                                 }
                              }
                           }
                           else
                           {
                              var str = getApprovalStrIDs(null, userlogin.Profile_ID.ToString());
                              if (!string.IsNullOrEmpty(str))
                                 nextappstr = ex.Next_Approver.Replace(str, "|");
                           }
                           #endregion
                        }
                        else
                        {
                           #region Normal flow
                           var param = new Dictionary<string, object>();
                           param.Add("expID", ex.Expenses_Application_ID);
                           param.Add("appID", action.NextApprover.Profile_ID);
                           param.Add("empID", ex.Employee_Profile_ID);
                           param.Add("reqcancelID", action.Request_ID);
                           param.Add("cancelStatus", WorkflowStatus.Cancelled);
                           param.Add("code", uService.GenActivateCode("E" + ex.Expenses_Application_ID + action.NextApprover.Profile_ID + "_", userlogin.User_Authentication.User_Authentication_ID));
                           var linkApp = GenerateActionLink("Approval", "ProcessWorkflow", param);
                           param["cancelStatus"] = WorkflowStatus.Rejected;
                           var linkRej = GenerateActionLink("Approval", "ProcessWorkflow", param);

                           var appr = uService.getUser(action.NextApprover.Profile_ID, false);
                           if (appr != null)
                              sendRequestEmail(ex, com, appr, user, hist, ex.Cancel_Status, null, linkApp, linkRej);

                           if (action.NextApprover.Profile_ID != userlogin.Profile_ID)
                              nextappstr = getApprovalStrIDs(nextappstr, action.NextApprover.Profile_ID.ToString());

                           #endregion
                        }

                        if (!string.IsNullOrEmpty(nextappstr))
                           ex.Next_Approver = nextappstr;

                        model.result = eService.updateExpenseApplication(ex);
                        uService.ExpireActivationByPrefix(Ac_Code);
                        return RedirectToAction("ExpensesManagement", new { Code = ERROR_CODE.SUCCESS, Msg = new Success().getSuccess(ERROR_CODE.SUCCESS_APPROVE), Field = Resource.Expense });
                     }
                  }
               }
               #endregion
            }
            else if (hist.Supervisor.HasValue)
            {
               #region Supervisor
               /*cancel approval by supervisor*/
               if (pStatus == WorkflowStatus.Approved)
                  ex.Cancel_Status = WorkflowStatus.Cancelled;
               else
                  ex.Cancel_Status = WorkflowStatus.Cancellation_Rejected;

               ex.Next_Approver = null;
               ex.Approver = getApprovalStrIDs(null, userlogin.Profile_ID.ToString());

               model.result = eService.updateExpenseApplication(ex);
               if (model.result.Code == ERROR_CODE.SUCCESS)
               {
                  uService.ExpireActivationByPrefix(Ac_Code);
                  sendProceedEmail(ex, com, user, userlogin, hist, ex.Cancel_Status, null);
                  if (pStatus == WorkflowStatus.Approved)
                     return RedirectToAction("ExpensesManagement", new { Code = ERROR_CODE.SUCCESS, Msg = new Success().getSuccess(ERROR_CODE.SUCCESS_CANCEL), Field = Resource.Expense });
                  else
                     return RedirectToAction("ExpensesManagement", new { Code = ERROR_CODE.SUCCESS, Msg = new Success().getSuccess(ERROR_CODE.SUCCESS_CANCEL_REJECT), Field = Resource.Expense });
               }
               #endregion
            }
         }
         //-------rights------------
         var rightResult = base.validatePageRight(UserSession.RIGHT_A);
         if (rightResult.action != null)
            return rightResult.action;
         model.rights = rightResult.rights;

         var yearSerivce = empService.GetYearService(hist.Employee_Profile_ID);

         model.currencyList = cbService.LstCurrencyByCompany(userlogin.Company_ID.Value, currentdate.Year.ToString());
         model.expensesConfigList = eService.getExpenseTypes(userlogin.Company_ID.Value, hist.Department_ID, hist.Designation_ID, null, yearSerivce);
         model.EmployeeUnderMeList = cbService.LstEmpUnderMe(userlogin.Profile_ID);

         //********  Smart Dev  ********//
         model.JobCostlst = cbService.LstJobCost(userlogin.Company_ID, true);
         model.TaxTypelst = cbService.LstTaxType(false);
         model.AmountTypelst = cbService.LstAmountType();

         if (model.Request_ID.HasValue)
         {
            var r = aService.GetMyRequests(userlogin.Company_ID, null, null, ModuleCode.HR, ApprovalType.Expense, model.Expenses_ID);
            if (r.Item2.IsSuccess && r.Item1 != null)
               model.Expenses_Request = r.Item1;
         }

         return View(model);
      }

      [HttpGet]
      public ActionResult Record(ServiceResult result, ExpensesViewModel model)
      {
         var userlogin = UserSession.getUser(HttpContext);
         if (userlogin == null)
            return errorPage(ERROR_CODE.ERROR_401_UNAUTHORIZED);

         //Validate Page Right
         var rightResult = base.validatePageRight(UserSession.RIGHT_A, "/Expenses/Application");
         if (rightResult.action != null) return rightResult.action;
         model.rights = rightResult.rights;
         model.result = result;

         var eService = new ExpenseService();
         var cbService = new ComboService();

         if (userlogin.Employee_Profile.Count == 0)
            return errorPage(ERROR_CODE.ERROR_16_NO_EMPLOYEE_PROFILE);

         var com = new CompanyService().GetCompany(userlogin.Company_ID.Value);
         if (com != null && !com.Currency_ID.HasValue)
            return errorPage(ERROR_CODE.ERROR_13_NO_DEFAULT_CURRENCY);

         model.eStatuslist = cbService.LstApprovalStatus(true);

         var criteria = new ExpenseCriteria()
         {
            Company_ID = userlogin.Company_ID,
            Profile_ID = userlogin.Profile_ID,

            Include_Extra = false,
            Include_Draft = true,

            Overall_Status = model.Overall_Status,
            Date_Applied = model.Process_Date_Applied,
         };
         var presult = eService.LstExpenses(criteria);
         if (presult.Object != null)
            model.ExpensesProcessedLst = (List<Expenses_Application>)presult.Object;

         return View(model);
      }

      [HttpGet]
      public ActionResult ExpensesManagement(ServiceResult result, ExpensesViewModel model, int pno = 1, int plen = 10)
      {
         var userlogin = UserSession.getUser(HttpContext);
         if (userlogin == null)
            return errorPage(ERROR_CODE.ERROR_401_UNAUTHORIZED);

         var eService = new ExpenseService();
         var lService = new LeaveService();

         //-------rights------------
         var rightResult = base.validatePageRight(UserSession.RIGHT_A, "/Expenses/ExpensesManagement");
         if (rightResult.action != null)
            return rightResult.action;
         model.rights = rightResult.rights;
         model.result = result;

         var hService = new EmploymentHistoryService();
         var hist = hService.GetCurrentEmploymentHistoryByProfile(userlogin.Profile_ID);
         if (hist == null)
            return errorPage(ERROR_CODE.ERROR_511_DATA_NOT_FOUND, Resource.Employment_History);

         //-------list data------------
         model.employeeList = lService.getEmployeeList(userlogin.Company_ID.Value);

         //----------------------- Pending
         var criteriaPending = new ExpenseCriteria()
         {
            Company_ID = userlogin.Company_ID,
            Profile_ID = userlogin.Profile_ID,
            Employee_Profile_ID = hist.Employee_Profile_ID,
            Department_ID = hist.Department_ID,
            Include_Extra = true,
            Tab_Pending = true,

            //Page_Size = plen,
            //Page_No = pno,

            Request_Profile_ID = model.Pending_Profile_ID,
            Date_Applied = model.Pending_Date_Applied,
         };
         var presultPending = eService.LstExpenses(criteriaPending);
         if (presultPending.Object != null)
            model.ExpensesPendingLst = (List<Expenses_Application>)presultPending.Object;

         //----------------------- Processed
         var criteriaProcessed = new ExpenseCriteria()
         {
            Company_ID = userlogin.Company_ID,
            Profile_ID = userlogin.Profile_ID,
            Employee_Profile_ID = hist.Employee_Profile_ID,
            Department_ID = hist.Department_ID,

            Include_Extra = true,
            Tab_Processed = true,

            Page_Size = plen,
            Page_No = pno,

            Request_Profile_ID = model.Process_Profile_ID,
            Date_Applied = model.Process_Date_Applied,
         };
         var presultProcessed = eService.LstExpenses(criteriaProcessed);
         if (presultProcessed.Object != null)
            model.ExpensesProcessedLst = (List<Expenses_Application>)presultProcessed.Object;

         model.Record_Count = presultProcessed.Record_Count;
         model.Page_No = pno;
         model.Page_Length = plen;

         return View(model);
      }

      [HttpGet]
      public ActionResult ExpensesDelete(string operation, string eID)
      {
         var currentdate = StoredProcedure.GetCurrentDate();
         var userlogin = UserSession.getUser(HttpContext);
         if (userlogin == null)
            return errorPage(ERROR_CODE.ERROR_401_UNAUTHORIZED);

         var lService = new LeaveService();
         var model = new ExpensesViewModel();

         //-------rights------------
         var rightResult = base.validatePageRight(UserSession.RIGHT_A, "/Expenses/Application");
         if (rightResult.action != null)
            return rightResult.action;
         model.rights = rightResult.rights;

         ExpenseService eService = new ExpenseService();
         model.operation = EncryptUtil.Decrypt(operation);
         if (string.IsNullOrEmpty(model.operation))
            model.operation = Operation.C;

         if (model.operation == UserSession.RIGHT_D)
         {
            var expenseID = NumUtil.ParseInteger(EncryptUtil.Decrypt(eID));
            if (expenseID > 0)
            {
               var ex = eService.getExpenseApplication(expenseID);
               if (ex == null)
                  return errorPage(ERROR_CODE.ERROR_511_DATA_NOT_FOUND);
               //-------data------------
               ex.Update_On = currentdate;
               ex.Update_By = userlogin.User_Authentication.Email_Address;
               ex.Overall_Status = RecordStatus.Delete;
               model.result = eService.updateExpenseApplication(ex);
               if (model.result.Code == ERROR_CODE.SUCCESS)
                  return RedirectToAction("Record", new { Code = ERROR_CODE.SUCCESS, Msg = new Success().getSuccess(ERROR_CODE.SUCCESS_DELETE), Field = Resource.Expenses });
               else
                  return RedirectToAction("Record", new { Code = ERROR_CODE.ERROR_505_DELETE_ERROR, Msg = new Success().getSuccess(ERROR_CODE.ERROR_505_DELETE_ERROR), Field = Resource.Expenses });
            }
         }
         return RedirectToAction("Record");
      }
      #endregion

      # region Reports & Import
      [HttpGet]
      public ActionResult ExpenseReports(ServiceResult result, ExpenseReportViewModel model, string Expid, string lYear, string tabAction = "")
      {

         var userlogin = UserSession.getUser(HttpContext);
         if (userlogin == null)
            return errorPage(ERROR_CODE.ERROR_401_UNAUTHORIZED);

         //-------rights------------
         var rightResult = base.validatePageRight(UserSession.RIGHT_A, "/Expenses/ExpenseReport");
         if (rightResult.action != null)
            return rightResult.action;
         model.rights = rightResult.rights;

         var currentdate = StoredProcedure.GetCurrentDate();
         var ExpenService = new ExpenseService();
         var cbService = new ComboService();
         var EmpService = new EmployeeService();
         var deparService = new DepartmentService();
         var comService = new CompanyService();

         var year = NumUtil.ParseInteger(lYear);
         var Expenses_Type_ID = NumUtil.ParseInteger(EncryptUtil.Decrypt(Expid));
         if (Expenses_Type_ID != 0)
         {
            model.Expenses_Type_Sel = new List<int>(new int[] { Expenses_Type_ID });
            if (year != 0)
               model.Year = year;

            if (model.Year == 0)
               model.Year = currentdate.Year;
         }
         var tempYear = model.Year;
         if (!string.IsNullOrEmpty(model.sTo) || !string.IsNullOrEmpty(model.sFrom))
            tempYear = 0;

         //Apply filter
         model.employeeList = EmpService.LstEmployeeProfile(userlogin.Company_ID.Value);
         model.departmentlst = cbService.LstDepartment(userlogin.Company_ID, hasBlank: true);
         model.expensesTypeList = cbService.LstExpensesType(userlogin.Company_ID, hasBlank: false);
         if (model.expensesTypeList == null)
            return errorPage(ERROR_CODE.ERROR_511_DATA_NOT_FOUND, Resource.Expenses_Type);

         if (model.Expenses_Type_Sel == null)
         {
            model.Expenses_Type_Sel = new List<int>();
            foreach (var r in model.expensesTypeList)
               model.Expenses_Type_Sel.Add(NumUtil.ParseInteger(r.Value));
         }
         if (model.expensesTypeList != null && model.expensesTypeList.Count > 0)
            model.Expenses_Sel = Newtonsoft.Json.JsonConvert.SerializeObject(model.Expenses_Type_Sel);

         var Expentypelst = new List<Expenses_Application_Document>();
         var criteria = new ExpenseCriteria()
         {
            Company_ID = userlogin.Company_ID,
            Year = tempYear,
            Department_ID = model.Department_ID,
            Date_To = model.sTo,
            Date_From = model.sFrom,
            Closed_Status = true
         };
         var pResult = ExpenService.LstExpenseApplications(criteria);
         if (pResult.Object != null) Expentypelst = (List<Expenses_Application_Document>)pResult.Object;
         model.expenseList = Expentypelst;

         if (tabAction == "export")
         {
            string csv = "";
            var dep_name = "";
            if (model.Department_ID.HasValue)
            {
               var department = deparService.GetDepartment(model.Department_ID.Value);
               if (department != null) dep_name = department.Name;
            }
            decimal[] emp_total = new decimal[model.expensesTypeList.Count + 1];
            var fullName = UserSession.GetUserName(userlogin);

            //HEADER
            string compname = comService.GetCompany(userlogin.Company_ID).Name;
            csv += "<table><tr valign='top'><td valign='top'><b> " + compname + " </b></td><td>&nbsp;</td><td><b>" + Resource.Expenses_Report + "</b><br><b> " + Resource.From + " </b> " + model.sFrom + " - " + model.sTo + "</td></tr>";
            if (dep_name != "")
               csv += "<tr><td>&nbsp;</td><td>&nbsp;</td><td><b>" + Resource.Department + "</b> " + dep_name + " </td></tr>";

            csv += "<tr><td>&nbsp;</td><td>&nbsp;</td><td>&nbsp;</td></tr></table>";
            csv += "<table border=1 ><tr><td><b>" + Resource.Employee_No_SymbolDot + "</b></td>";
            csv += "<td><b>" + Resource.Employee_Name + "</b></td>";
            if (model.expensesTypeList != null && model.expensesTypeList.Count > 0)
            {
               emp_total[model.expensesTypeList.Count] = 0;
               int i = 0;
               foreach (ComboViewModel c in model.expensesTypeList)
               {
                  if (model.Expenses_Type_Sel.Contains(NumUtil.ParseInteger(c.Value)))
                  {
                     csv += "<td><b>" + c.Text + "</b></td>";
                     emp_total[i] = 0;
                     i++;
                  }
               }
            }
            csv += "<td><b>" + Resource.Total + "</b></td></tr>";
            if (model.employeeList != null && model.employeeList.Count > 0)
            {
               decimal total = 0;
               int employee = 0;
               foreach (var emp in model.employeeList)
               {
                  employee = 0;
                  total = 0;
                  Global_Lookup_Data emptype = null;
                  Employment_History emphist = emp.Employment_History.Where(w => w.Effective_Date <= currentdate).OrderByDescending(o => o.Effective_Date).FirstOrDefault();
                  if (emphist != null)
                  {
                     emptype = new ComboService().GetLookup(emphist.Employee_Type);
                     if (model.Department_ID.HasValue && emphist.Department_ID != model.Department_ID)
                     {
                        continue;
                     }
                  }
                  employee = emp.Employee_Profile_ID;
                  string emp_no = emp.Employee_No;
                  List<Expenses_Application_Document> expenses = model.expenseList.Where(w => w.Employee_Profile_ID == employee).ToList();
                  csv += "<tr><td>" + emp_no + "</td>";
                  csv += "<td>" + UserSession.GetUserName(emp.User_Profile) + "</td>";
                  if (model.expensesTypeList != null && model.expensesTypeList.Count > 0)
                  {
                     total = 0;
                     int i = 0;
                     foreach (ComboViewModel c in model.expensesTypeList)
                     {
                        if (model.Expenses_Type_Sel.Contains(NumUtil.ParseInteger(c.Value)))
                        {
                           decimal expV = 0;
                           if (expenses != null && expenses.Count > 0)
                           {
                              foreach (Expenses_Application_Document exp in expenses.Where(w => w.Expenses_Config.Expenses_Name.ToLower() == c.Text.ToLower()).ToList())
                              {
                                 expV += exp.Amount_Claiming.Value;
                                 total += exp.Amount_Claiming.Value;
                              }
                           }
                           emp_total[i] += expV;
                           csv += "<td>" + expV.ToString("n2") + "</td>";
                           i++;
                        }
                     }
                     emp_total[i] += total;
                  }
                  csv += "<td><b>" + total.ToString("n2") + "</b></td></tr>";
               }

               csv += "<tr><td></td>";
               csv += "<td><b> " + Resource.Total + " </b></td>";
               int j = 0;
               if (model.expensesTypeList != null && model.expensesTypeList.Count > 0)
               {
                  foreach (ComboViewModel c in model.expensesTypeList)
                  {
                     if (model.Expenses_Type_Sel.Contains(NumUtil.ParseInteger(c.Value)))
                     {
                        csv += "<td><b>" + emp_total[j].ToString("n2") + "</b></td>";
                        j++;
                     }
                  }
               }
               csv += "<td><b>" + emp_total[j].ToString("n2") + "</b></td></tr>";
            }
            csv += "</table>";
            csv += "<table><tr><td>&nbsp;</td></tr>";
            csv += "<tr><td><b> " + Resource.Printed_By + " </b> " + fullName + "</td></tr></table>";

            System.Web.UI.WebControls.GridView gv = new System.Web.UI.WebControls.GridView();
            //gv.DataSource = data;
            gv.DataBind();

            Response.ClearContent();
            Response.Buffer = true;
            Response.AddHeader("content-disposition", "attachment; filename=ExpenseReport.xls");
            Response.ContentType = "application/ms-excel";
            Response.Charset = "";
            StringWriter sw = new StringWriter();
            sw.Write(csv);
            System.Web.UI.HtmlTextWriter htw = new System.Web.UI.HtmlTextWriter(sw);
            gv.RenderControl(htw);
            Response.Output.Write(sw.ToString());
            Response.Flush();
            Response.End();
         }

         return View(model);
      }

      [HttpGet]
      public ActionResult ExpensesImport()
      {
         var model = new ImportExpensesViewModels();

         var userlogin = UserSession.getUser(HttpContext);
         if (userlogin == null)
            return errorPage(ERROR_CODE.ERROR_401_UNAUTHORIZED);

         //-------rights------------
         var rightResult = base.validatePageRight(UserSession.RIGHT_C);
         if (rightResult.action != null)
            return rightResult.action;
         model.rights = rightResult.rights;

         model.ExpensesAppDoc = new List<ImportExpensesApplicationViewModels>().ToArray();
         model.ErrMsg = new List<string>();
         model.Validated_Main = true;

         return View(model);

      }

      public ActionResult ExpensesImport(ImportExpensesViewModels model, string pageAction)
      {

         User_Profile userlogin = UserSession.getUser(HttpContext);
         if (userlogin == null)
            return errorPage(ERROR_CODE.ERROR_401_UNAUTHORIZED);

         //Validate Page Right
         var rightResult = base.validatePageRight(UserSession.RIGHT_C);
         if (rightResult.action != null) return rightResult.action;
         model.rights = rightResult.rights;

         //-------data------------
         DateTime currentdate = StoredProcedure.GetCurrentDate();
         var leaveService = new LeaveService();
         var cbService = new ComboService();
         var comService = new CompanyService();
         var empService = new EmployeeService();
         var eService = new ExpenseService();

         if (pageAction == "import")
         {
            if (model.ExpensesAppDoc.Length > 0 && model.Validated_Main)
            {
               List<Expenses_Application> ExpensesApps = new List<Expenses_Application>();
               foreach (var row in model.ExpensesAppDoc)
               {
                  if (row.Validate && row.Employee_Profile_ID != null)
                  {
                     var eNo = "";
                     var pattern = eService.getExpensesNoPattern(row.Company_ID);
                     if (pattern == null)
                     {
                        var newpattern = new Expenses_No_Pattern()
                        {
                           Company_ID = row.Company_ID,
                           Current_Running_Number = 1,
                           Create_By = userlogin.User_Authentication.Email_Address,
                           Create_On = currentdate,
                           Update_By = userlogin.User_Authentication.Email_Address,
                           Update_On = currentdate
                        };
                        model.result = eService.InsertExpensesNoPattern(newpattern);
                        if (model.result.Code == ERROR_CODE.SUCCESS)
                        {
                           eNo = "EX-" + currentdate.Year + "-" + 1.ToString("0000");
                        }
                        else
                        {
                           ModelState.AddModelError("Import_Expenses", Resource.Message_Can_Not_Update_Expenses_Pattern_Running_Number);
                           return View(model);
                        }
                     }
                     else
                     {
                        pattern.Current_Running_Number = pattern.Current_Running_Number + 1;
                        model.result = eService.UpdateExpensesRunningNumber(pattern.Expenses_No_Pattern_ID, pattern.Current_Running_Number.Value, userlogin.User_Authentication.Email_Address);
                        if (model.result.Code == ERROR_CODE.SUCCESS)
                        {
                           eNo = "EX-" + currentdate.Year + "-" + pattern.Current_Running_Number.Value.ToString("0000");
                        }
                        else
                        {
                           ModelState.AddModelError("Import_Expenses", Resource.Message_Can_Not_Update_Expenses_Pattern_Running_Number);
                           return View(model);
                        }
                     }

                     var ExpensesApp = new Expenses_Application()
                     {
                        Employee_Profile_ID = row.Employee_Profile_ID,
                        Expenses_No = eNo,
                        Expenses_Title = row.Expenses_Title,
                        Date_Applied = DateUtil.ToDate(row.Date_Applied),

                        //Request_ID = row.Request_ID,
                        Overall_Status = WorkflowStatus.Closed,
                        Create_By = userlogin.User_Authentication.Email_Address,
                        Create_On = currentdate,
                        Update_By = userlogin.User_Authentication.Email_Address,
                        Update_On = currentdate
                     };

                     var ExpensesAppDoc = new Expenses_Application_Document()
                     {
                        Employee_Profile_ID = row.Employee_Profile_ID,
                        Expenses_Config_ID = row.Expenses_Config_ID,
                        Expenses_Date = DateUtil.ToDate(row.Expenses_Date),
                        Doc_No = row.Doc_No,
                        Date_Applied = DateUtil.ToDate(row.Date_Applied),
                        Total_Amount = row.Total_Amount,
                        Amount_Claiming = row.Amount_Claiming,
                        Tax = row.Tax,
                        Selected_Currency = row.Selected_Currency,
                        Reasons = row.Remarks,
                        Department_ID = row.Department_ID.HasValue ? row.Department_ID.Value : 0,
                        Create_By = userlogin.User_Authentication.Email_Address,
                        Create_On = currentdate,
                        Update_By = userlogin.User_Authentication.Email_Address,
                        Update_On = currentdate
                     };
                     ExpensesApp.Expenses_Application_Document.Add(ExpensesAppDoc);
                     ExpensesApps.Add(ExpensesApp);
                  }
               }
               if (ExpensesApps != null)
               {
                  model.result = eService.InsertExpensesApplication(ExpensesApps.ToArray());
                  if (model.result.Code == ERROR_CODE.SUCCESS)
                  {
                     return RedirectToAction("Record", new { Code = model.result.Code, Msg = model.result.Msg, Field = model.result.Field });
                  }
               }
            }
         }
         else
         {
            if (Request.Files.Count == 0)
            {
               ModelState.AddModelError("Import_Expenses", Resource.Message_Cannot_Found_Excel_Sheet);
               return View(model);
            }

            HttpPostedFileBase file = Request.Files[0];
            if (file != null)
            {
               var com = comService.GetCompany(userlogin.Company_ID);
               if (com == null)
                  return errorPage(ERROR_CODE.ERROR_511_DATA_NOT_FOUND);

               var expensesTypelst = eService.getExpenseTypes(userlogin.Company_ID.Value);
               var employeeprofilelst = empService.LstEmployeeProfile(userlogin.Company_ID);
               var currencylst = cbService.LstCurrencyCode(false);

               try
               {
                  using (var package = new ExcelPackage(file.InputStream))
                  {
                     List<string> chk_Emp_No = new List<string>();
                     model.Validated_Main = true;

                     ExcelWorksheet worksheet_1 = package.Workbook.Worksheets[1];
                     if (worksheet_1.Dimension != null)
                     {
                        int totalRows_1 = worksheet_1.Dimension.End.Row;
                        int totalCols_1 = worksheet_1.Dimension.End.Column;

                        if (totalCols_1 != 10)
                        {
                           ModelState.AddModelError("ExpensesAppDoc", Resource.Message_Column_Count_Is_Invalid + " " + Resource.Message_Please_Edit_Reupload);
                           model.Validated_Main = false;
                        }
                        if (totalRows_1 <= 1)
                        {
                           ModelState.AddModelError("ExpensesAppDoc", Resource.Message_Row_Count_Is_Invalid + " " + Resource.Message_Please_Edit_Reupload);
                           model.Validated_Main = false;
                        }

                        if (ModelState.IsValid)
                        {
                           if (totalRows_1 > 1)
                           {
                              var ExpensesAppDocs = new List<ImportExpensesApplicationViewModels>();
                              for (int i = 2; i <= totalRows_1; i++)
                              {
                                 var ExpensesAppDoc = new ImportExpensesApplicationViewModels();
                                 ExpensesAppDoc.Company_ID = userlogin.Company_ID;
                                 ExpensesAppDoc.Validate = true;
                                 var isempty = true;
                                 var err_ = new System.Text.StringBuilder();

                                 for (int j = 1; j <= totalCols_1; j++)
                                 {
                                    var columnName = worksheet_1.Cells[1, j].Value.ToString();
                                    isempty = false;
                                    if (worksheet_1.Cells[i, j].Value != null)
                                    {
                                       if (j == ExpensesDocImportColumn.Employee_No)
                                       {
                                          var emp_no = "";
                                          emp_no = worksheet_1.Cells[i, j].Value.ToString();
                                          ExpensesAppDoc.Employee_No = emp_no;

                                          var empprofil = employeeprofilelst.Where(w => w.Employee_No.ToString().Trim() == emp_no.ToString().Trim()).FirstOrDefault();
                                          if (empprofil != null)
                                          {
                                             ExpensesAppDoc.Employee_Profile_ID = empprofil.Employee_Profile_ID;
                                             ExpensesAppDoc.Department_ID = empprofil.Employment_History.Select(s => s.Department.Department_ID).FirstOrDefault();
                                          }
                                          else
                                          {
                                             model.Validated_Main = false;
                                             ExpensesAppDoc.Validate = false;
                                             err_.AppendLine(Resource.The + " " + columnName + " " + Resource.Field + " " + Resource.Is_Not_Found_Lower);
                                          }
                                       }
                                       else if (j == ExpensesDocImportColumn.Expenses_Config_Type)
                                       {
                                          var expensesType = expensesTypelst.Where(w => w.Expenses_Name.ToLower().Trim() == worksheet_1.Cells[i, j].Value.ToString().ToLower().Trim()).FirstOrDefault();
                                          if (expensesType == null)
                                          {
                                             model.Validated_Main = false;
                                             ExpensesAppDoc.Validate = false;
                                             err_.AppendLine(Resource.The + " " + columnName + " " + Resource.Field + " " + Resource.Is_Invalid_Lower);
                                          }
                                          else
                                          {
                                             ExpensesAppDoc.Expenses_Config_ID = NumUtil.ParseInteger(expensesType.Expenses_Config_ID);
                                             ExpensesAppDoc.Expenses_Config_Type = expensesType.Expenses_Name;
                                          }
                                       }
                                       else if (j == ExpensesDocImportColumn.Selected_Currency)
                                       {
                                          var currency = currencylst.Where(w => w.Text.ToLower().Trim() == worksheet_1.Cells[i, j].Value.ToString().ToLower().Trim()).FirstOrDefault();
                                          if (currency == null)
                                          {
                                             model.Validated_Main = false;
                                             ExpensesAppDoc.Validate = false;
                                             err_.AppendLine(Resource.The + " " + columnName + " " + Resource.Field + " " + Resource.Is_Invalid_Lower);
                                          }
                                          else
                                          {
                                             ExpensesAppDoc.Selected_Currency = NumUtil.ParseInteger(currency.Value);
                                             ExpensesAppDoc.Selected_Currency_ = currency.Text;
                                          }
                                       }
                                       else if (j == ExpensesDocImportColumn.Date_Applied || j == ExpensesDocImportColumn.Expenses_Date)
                                       {
                                          var strdate = "";
                                          try
                                          {
                                             var date = (DateTime)worksheet_1.Cells[i, j].Value;
                                             strdate = DateUtil.ToDisplayDate(date);
                                          }
                                          catch
                                          {
                                             model.Validated_Main = false;
                                             ExpensesAppDoc.Validate = false;
                                             strdate = worksheet_1.Cells[i, j].Value.ToString();
                                             err_.AppendLine(Resource.The + " " + columnName + " " + Resource.Field + " " + Resource.Is_Invalid_Lower);
                                          }

                                          if (j == ExpensesDocImportColumn.Date_Applied)
                                             ExpensesAppDoc.Date_Applied = strdate;
                                          else if (j == ExpensesDocImportColumn.Expenses_Date)
                                             ExpensesAppDoc.Expenses_Date = strdate;
                                       }
                                       else if (j == ExpensesDocImportColumn.Total_Amount
                                           || j == ExpensesDocImportColumn.Amount_Claiming
                                           || j == ExpensesDocImportColumn.Tax)
                                       {
                                          decimal daystaken = 0;
                                          try
                                          {
                                             daystaken = Convert.ToDecimal(worksheet_1.Cells[i, j].Value);
                                          }
                                          catch
                                          {
                                             model.Validated_Main = false;
                                             ExpensesAppDoc.Validate = false;
                                             daystaken = Convert.ToDecimal(worksheet_1.Cells[i, j].Value.ToString());
                                             err_.AppendLine(Resource.The + " " + columnName + " " + Resource.Field + " " + Resource.Is_Invalid_Lower);
                                          }
                                          if (j == ExpensesDocImportColumn.Total_Amount)
                                             ExpensesAppDoc.Total_Amount = daystaken;
                                          else if (j == ExpensesDocImportColumn.Amount_Claiming)
                                             ExpensesAppDoc.Amount_Claiming = daystaken;
                                          else if (j == ExpensesDocImportColumn.Tax)
                                             ExpensesAppDoc.Tax = daystaken;

                                       }
                                       else if (j == ExpensesDocImportColumn.Remarks)
                                       {
                                          ExpensesAppDoc.Remarks = worksheet_1.Cells[i, j].Value.ToString();
                                          if (ExpensesAppDoc.Remarks.Length > 300)
                                          {
                                             model.Validated_Main = false;
                                             ExpensesAppDoc.Validate = false;
                                             err_.AppendLine(Resource.The + " " + columnName + " " + Resource.Field + " " + Resource.Message_Maximum_Length + " '300'.");
                                          }
                                       }
                                       else if (j == ExpensesDocImportColumn.Expenses_Title)
                                       {
                                          ExpensesAppDoc.Expenses_Title = worksheet_1.Cells[i, j].Value.ToString();
                                          if (ExpensesAppDoc.Expenses_Title.Length > 300)
                                          {
                                             model.Validated_Main = false;
                                             ExpensesAppDoc.Validate = false;
                                             err_.AppendLine(Resource.The + " " + columnName + " " + Resource.Field + " " + Resource.Message_Maximum_Length + " '300'.");
                                          }
                                       }
                                    }
                                    else
                                    {
                                       // Validate require
                                       if (j == ExpensesDocImportColumn.Employee_No
                                           || j == ExpensesDocImportColumn.Expenses_Title
                                           || j == ExpensesDocImportColumn.Expenses_Config_Type
                                           || j == ExpensesDocImportColumn.Date_Applied
                                           || j == ExpensesDocImportColumn.Expenses_Date
                                           || j == ExpensesDocImportColumn.Total_Amount
                                           || j == ExpensesDocImportColumn.Selected_Currency
                                          //|| j == ExpensesDocImportColumn.Tax
                                           || j == ExpensesDocImportColumn.Amount_Claiming
                                           )
                                       {
                                          model.Validated_Main = false;
                                          ExpensesAppDoc.Validate = false;
                                          err_.AppendLine(Resource.The + " " + columnName + " " + Resource.Field + " " + Resource.Is_Rrequired_Lower);
                                       }
                                    }
                                 }
                                 if (isempty)
                                 {
                                    model.Validated_Main = false;
                                    ExpensesAppDoc.Validate = false;
                                    err_.AppendLine(Resource.Message_Empty_Row);
                                 }
                                 ExpensesAppDoc.ErrMsg = err_.ToString();
                                 ExpensesAppDocs.Add(ExpensesAppDoc);
                              }
                              model.ExpensesAppDoc = ExpensesAppDocs.ToArray();
                           }
                        }
                        else
                        {
                           model.ExpensesAppDoc = new List<ImportExpensesApplicationViewModels>().ToArray();
                        }
                     }
                  }
               }
               catch
               {
                  ModelState.AddModelError("Import_Employee", Resource.Message_Cannot_Found_Excel_Sheet + " " + Resource.Message_Please_Edit_Reupload);
                  model.ExpensesAppDoc = new List<ImportExpensesApplicationViewModels>().ToArray();
               }
               //var erjrors = GetErrorModelState();
            }
         }
         return View(model);
      }

      [HttpGet]
      public ActionResult ExpenseReport(ServiceResult result, ExpensesListViewModel model)
      {

         var userlogin = UserSession.getUser(HttpContext);
         if (userlogin == null)
            return errorPage(ERROR_CODE.ERROR_401_UNAUTHORIZED);

         //Validate Page Right
         var rightResult = base.validatePageRight(UserSession.RIGHT_A);
         if (rightResult.action != null) return rightResult.action;
         model.rights = rightResult.rights;

         var currentdate = StoredProcedure.GetCurrentDate();
         var ExpenService = new ExpenseService();
         var cbService = new ComboService();
         var EmpService = new EmployeeService();
         var deparService = new DepartmentService();
         var comService = new CompanyService();

         model.Yearlst = new List<int>();
         for (int i = 2014; i <= currentdate.Year; i++)
            model.Yearlst.Add(i);

         if (model.Year == 0)
            model.Year = currentdate.Year;

         var expensesTtypelist = cbService.LstExpensesType(userlogin.Company_ID, hasBlank: false);
         if (expensesTtypelist == null)
            return errorPage(ERROR_CODE.ERROR_511_DATA_NOT_FOUND, Resource.Expenses_Type);

         model.ExpensesTypelst = expensesTtypelist;
         model.departmentlst = cbService.LstDepartment(userlogin.Company_ID, hasBlank: true);

         var dup = new List<Expenses_Application_Document>();
         var criteria = new ExpenseCriteria() { Company_ID = userlogin.Company_ID, Year = model.Year, Department_ID = model.Department_ID, Closed_Status = true };
         var pResult = ExpenService.LstExpenseApplications(criteria);
         if (pResult.Object != null) dup = (List<Expenses_Application_Document>)pResult.Object;
         var expenseList = dup;

         var expensedetails = new List<Expenses_List>();
         var totalhash = new System.Collections.Hashtable();
         foreach (var lrow in expensesTtypelist)
         {
            totalhash.Add("d" + lrow.Value, (decimal)0);
         }
         if (expenseList != null)
         {
            foreach (var row in expenseList)
            {
               foreach (var lrow in expensesTtypelist)
               {
                  if (row.Expenses_Config_ID == NumUtil.ParseInteger(lrow.Value))
                  {
                     totalhash["d" + lrow.Value] = (decimal)totalhash["d" + lrow.Value] + row.Amount_Claiming.Value;
                  }
               }
            }
         }
         foreach (var lrow in expensesTtypelist)
         {
            if (NumUtil.ParseInteger(lrow.Value) != 0)
            {
               decimal dtotal = 0;
               if (totalhash.Contains("d" + lrow.Value))
               {
                  dtotal = (decimal)totalhash["d" + lrow.Value];
                  expensedetails.Add(new Expenses_List()
                  {
                     Expenses_ID = NumUtil.ParseInteger(lrow.Value),
                     Expenses_Type = lrow.Text,
                     Claimable_Amount = dtotal
                  });
               }
            }
         }
         model.ExpensesList = expensedetails;

         return View(model);
      }
      #endregion

      #region Ajex & Pdf & Receipt Upload

      [HttpGet]
      public String ApplicationConfig(
         Nullable<int> pExpenses_Config_ID,
         Nullable<int> pSelected_Currency,
         Nullable<decimal> pTotal_Amount,
         string pExpenses_Date,
         Nullable<int> pProfile_ID,
         string pTax_Type,
         Nullable<decimal> pTax,
         string pTax_Amount_Type,
         Nullable<decimal> pWithholding_Tax,
         string pWithholding_Tax_Type
         )
      {
         Debug.WriteLine("'***********APP DEBUG***********' " + DateTime.Now + "-Start Controller Application Config");
         var model = new ExpensesViewModel();
         var exCurService = new ExchangeCurrencyConfigService();
         var eService = new ExpenseService();
         var cbService = new ComboService();
         var comService = new CompanyService();
         var histService = new EmploymentHistoryService();
         var userlogin = UserSession.getUser(HttpContext);
         var currentdate = StoredProcedure.GetCurrentDate();
         var emp = new EmployeeService(); //Added by Moet

         if (userlogin == null)
            return "";

         if (userlogin.Employee_Profile.Count == 0)
            return "";

         var expense_Type = eService.GetExpenseType(pExpenses_Config_ID);
         if (expense_Type == null)
            return "";

         var Emp_Profile_ID = userlogin.Employee_Profile.Select(w => w.Employee_Profile_ID).FirstOrDefault();
         if (Emp_Profile_ID == 0)
            return "";

         if (pProfile_ID.HasValue && pProfile_ID.Value > 0)
         {
            var e = emp.GetEmployeeProfileByProfileID(pProfile_ID);
            Emp_Profile_ID = e.Employee_Profile_ID;
         }
         else
            pProfile_ID = userlogin.Profile_ID;

         var expense_Type_Detail = eService.GetExpensesConfigDetail(pExpenses_Config_ID, pProfile_ID);
         if (expense_Type_Detail == null)
            return "";

         var balance = 0M;
         var amountClaiming = 0M;
         var amount = 1;
         var isPerDepartment = false;
         bool checkYear = false;
         bool isAlert = false;
         bool dataIsNull = false;
         var exchangeDetail = "";
         var amountConver = 2M;

         var taxAmount = 0M;
         var _taxAmount = 0M;
         var _WithholdingTaxAmount = 0M;

         if (pTax_Type == TaxType.Exclusive)
         {
            if (pTotal_Amount.HasValue && pTotal_Amount.Value > 0)
            {
               if (pTax_Amount_Type.Equals("%"))
                  _taxAmount = pTotal_Amount.Value * (pTax.Value / 100);
               else
                  _taxAmount = pTax.Value;

               taxAmount = _taxAmount;
            }
         }
         else
         {
            if (pTax_Amount_Type.Equals("%"))
               _taxAmount = (pTotal_Amount.Value * pTax.Value / (pTax.Value + 100));
            else
               _taxAmount = pTax.Value;
         }

         if (pWithholding_Tax.HasValue && pWithholding_Tax.Value > 0)
         {
            if (pWithholding_Tax_Type.Equals("%"))
               _WithholdingTaxAmount = pTotal_Amount.Value * (pWithholding_Tax.Value / 100);
            else
               _WithholdingTaxAmount = pWithholding_Tax.Value;
         }

         pTotal_Amount = pTotal_Amount + (taxAmount - _WithholdingTaxAmount);

         if (expense_Type.Claimable_Type == ClaimableType.Per_Department)
            isPerDepartment = true;

         balance = eService.calulateBalance(expense_Type, expense_Type_Detail, pProfile_ID, currentdate);

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
         cri.Expenses_Config_ID = pExpenses_Config_ID;
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
         checkYear = true;

         var company_Detail = comService.GetCompany(userlogin.Company_ID).Currency;
         if (company_Detail == null) return "";
         if (pSelected_Currency != company_Detail.Currency_ID && pday != null)
         {
            var currency_Detail = cbService.GetCurrency(pSelected_Currency);
            if (currency_Detail == null) return "";

            Exchange exchange_Detail = exCurService.GetExchangeRate(userlogin.Company_ID.Value, NumUtil.ParseInteger(year));
            if (exchange_Detail != null)
            {
               var ex_Currency = exchange_Detail.Exchange_Currency.Where(w => w.Currency_ID == pSelected_Currency.Value).FirstOrDefault();
               if (ex_Currency != null && ex_Currency.Exchange_Period == ExchangePeriod.ByDate)
               {
                  var ex_Rate = ex_Currency.Exchange_Rate.Where(w => w.Exchange_Period == ExchangePeriod.ByDate && w.Exchange_Date == day).FirstOrDefault();
                  if (ex_Rate != null)
                  {
                     exchangeDetail = "1 " + currency_Detail.Currency_Code + " = " + (ex_Rate.Rate.HasValue ? ex_Rate.Rate.Value : 0) + " " + company_Detail.Currency_Code;
                     amountConver = (((ex_Rate.Rate.HasValue ? ex_Rate.Rate.Value : 0) * (pTotal_Amount.HasValue ? pTotal_Amount.Value : 0)) / 100);

                     dataIsNull = true;
                  }
                  else
                  {
                     isAlert = true;
                  }
               }
               else if (ex_Currency != null && ex_Currency.Exchange_Period == ExchangePeriod.ByMonth)
               {
                  var ex_Rate = ex_Currency.Exchange_Rate.Where(w => w.Exchange_Period == ExchangePeriod.ByMonth && w.Exchange_Month.ToString() == month).FirstOrDefault();
                  if (ex_Rate != null)
                  {
                     exchangeDetail = "1 " + currency_Detail.Currency_Code + " = " + (ex_Rate.Rate.HasValue ? ex_Rate.Rate.Value : 0) + " " + company_Detail.Currency_Code;
                     amountConver = (((ex_Rate.Rate.HasValue ? ex_Rate.Rate.Value : 0) * (pTotal_Amount.HasValue ? pTotal_Amount.Value : 0)) / 100);
                     dataIsNull = true;
                  }
                  else
                  {
                     isAlert = true;
                  }
               }
            }
            pTotal_Amount = amountConver; //Added by Moet on 5-Aug-2016
         }

         if (expense_Type_Detail.Select_Pecentage.HasValue && expense_Type_Detail.Select_Pecentage.Value)
         {
            var percent = expense_Type_Detail.Pecentage.HasValue ? expense_Type_Detail.Pecentage.Value : 0;
            amountClaiming = ((pTotal_Amount.HasValue ? pTotal_Amount.Value : 0) * percent / 100) * amount;
         }
         else
         {
            if (expense_Type_Detail.Amount >= pTotal_Amount * amount)
            {
               amountClaiming = (pTotal_Amount.HasValue ? pTotal_Amount.Value : 0) * amount;
            }
            else
            {
               amountClaiming = expense_Type_Detail.Amount.HasValue ? expense_Type_Detail.Amount.Value : 0;
               //กรณีวงเงินเกิน จะทำการหา amount tax & WH tax จากวงเงินที่จ่ายจริง 
               //ข้อมูลในส่วนด้านล่างนี้จะถูกนำไปใช้ในส่วนของ Job cost
               var _amount = 0M;
               _taxAmount = 0;

               if (pWithholding_Tax.HasValue && pWithholding_Tax.Value > 0)
               {
                  if (pWithholding_Tax_Type.Equals("%"))
                     _WithholdingTaxAmount = amountClaiming * (pWithholding_Tax.Value / 100);
                  else
                     _WithholdingTaxAmount = pWithholding_Tax.Value;

                  _amount = amountClaiming + _WithholdingTaxAmount;
               }
               else
                  _amount = amountClaiming;

               if (pTax_Amount_Type.Equals("%"))
                  _taxAmount = (_amount * pTax.Value / (pTax.Value + 100));
               else
                  _taxAmount = pTax.Value;

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

         amountClaiming = decimal.Round(amountClaiming, 2);

         string str = "";
         str = "<script type=\"text/javascript\"> \n\n $(function () {" +
                 "$('#Amount_Claiming').val('" + amountClaiming + "'); ";

         str += "$('#Balance').get(0).setAttribute('readonly',true);";

         str += "$('#Tax_Amount').val('" + _taxAmount + "');";
         str += "$('#Withholding_Tax_Amount').val('" + _WithholdingTaxAmount + "');";

         if (isPerDepartment)
         {
            str += "$('#Balance').val('" + balance + "');";
            str += "$('#Balance_Amount').val('" + balance.ToString("n2") + "');";
         }
         else
         {
            if (balance <= 0)
            {
               str += "$('#form-action').css('display','none');";
            }
            else
            {
               str += "$('#form-action').css('display','block');";
            }
            str += "$('#Balance').val('" + balance + "');";
            str += "$('#Balance_Amount').val('" + balance.ToString("n2") + "');";
         }


         if (dataIsNull)
         {
            str += "$('#Exchange_Detail').val('" + exchangeDetail + "');";
            str += "$('#Exchange_Total_Amount').val('" + amountConver.ToString("n2") + "');";
         }
         else
         {
            str += "$('#Selected_Currency').val('" + company_Detail.Currency_ID + "');";
            str += "$('#Selected_Currency').trigger('chosen:updated');";
            str += "$('#Exchange_Detail').val('');";
            str += "$('#Exchange_Total_Amount').val('');";
            if (isAlert)
            {
               str += "alert('" + Resource.Exchange_Rate + " " + Resource.Is_Not_Found_Lower + " " + DateUtil.ToDisplayDate(day) + "  " + Resource.Message_Please_Change_Currency + "');";
            }
         }

         if (checkYear)
         {
            str += "$('#Selected_Currency').empty();";
            if (model.currencyList != null)
            {
               foreach (var itemData in model.currencyList)
               {
                  str += "$('#Selected_Currency').append($('<option></option>').val('" + itemData.Value + "').html('" + itemData.Text + "'));";
               }
            }
            str += "$('#Selected_Currency').val('" + pSelected_Currency + "');";
            str += "$('#Selected_Currency').trigger('chosen:updated');";
         }

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

         str += "$('#Expenses_Type_Desc').val('" + expense_Type.Expenses_Description + "');";
         str += "$('#Expenses_Type_Name').val('" + expense_Type.Expenses_Name + "');";
         str += "$('#Expenses_Config_ID').val('" + expense_Type.Expenses_Config_ID + "');";
         str += "$('#Expenses_Config_ID').trigger('chosen:updated');";
         str += "});\n\n</script>";

         Debug.WriteLine("'***********APP DEBUG***********' " + DateTime.Now + "-End Controller Application Config");
         return str;
      }

      public ActionResult ReloadExpensesType(Nullable<int> pProfileID)
      {
         Debug.WriteLine("'***********APP DEBUG***********' " + DateTime.Now + "-Start Controller Reload Expenses Type");
         var userlogin = UserSession.getUser(HttpContext);
         if (userlogin == null)
            return errorPage(ERROR_CODE.ERROR_401_UNAUTHORIZED);

         //Added by Moet
         if (pProfileID == null)
            pProfileID = userlogin.Profile_ID;

         var empService = new EmployeeService();
         var eService = new ExpenseService();
         var hService = new EmploymentHistoryService();

         var emp = empService.GetEmployeeProfileByProfileID(pProfileID);
         if (emp == null)
            return errorPage(ERROR_CODE.ERROR_16_NO_EMPLOYEE_PROFILE);

         var userhist = hService.GetCurrentEmploymentHistory(emp.Employee_Profile_ID);
         if (userhist == null)
            return errorPage(ERROR_CODE.ERROR_14_NO_EMPLOYEE_HIST);

         var yearSerivce = empService.GetYearService(emp.Employee_Profile_ID);

         var comboList = new List<ComboViewModel>();
         var extypes = eService.getExpenseTypes(userlogin.Company_ID.Value, userhist.Department_ID, userhist.Designation_ID, null, yearSerivce, null);
         if (extypes != null)
         {
            foreach (var extype in extypes)
            {
               comboList.Add(new ComboViewModel
               {
                  Value = extype.Expenses_Config_ID.ToString(),
                  Text = extype.Expenses_Name
               });
            }

         }
         Debug.WriteLine("'***********APP DEBUG***********' " + DateTime.Now + "-End Controller Reload Expenses Type");
         return Json(new { pExpensesConfigList = comboList, pProfileID = pProfileID }, JsonRequestBehavior.AllowGet);
      }

      public ActionResult JobCostConfig(Nullable<int> pJobCostID)
      {
         Debug.WriteLine("'***********APP DEBUG***********' " + DateTime.Now + "-Start Controller Job Cost Config");
         decimal? Job_Cost_Balance = 0M;
         var userlogin = UserSession.getUser(HttpContext);
         if (userlogin == null)
            return errorPage(ERROR_CODE.ERROR_401_UNAUTHORIZED);

         if (pJobCostID.HasValue && pJobCostID.Value > 0)
         {
            var jobcostService = new JobCostService();
            var job = jobcostService.GetJobCost(pJobCostID);
            if (job != null)
               Job_Cost_Balance = (job.Sell_Price - job.Costing);
         }
         Debug.WriteLine("'***********APP DEBUG***********' " + DateTime.Now + "-End Controller  Job Cost Config");
         return Json(new { Job_Cost_Balance = NumUtil.FormatCurrency(Job_Cost_Balance, 2) }, JsonRequestBehavior.AllowGet);
      }

      public ActionResult ApplicationDetailRow(
          Nullable<int> pIndex,
          Nullable<int> pDocID,
          Nullable<int> pExpensesConfigID,
          string pExpensesDate,
          Nullable<decimal> pBalance,
          Nullable<decimal> pTotalAmount,
          Nullable<decimal> pAmountClaiming,
          Nullable<int> pSelectedCurrency,
          Nullable<decimal> pTax,
          string pNotes,
          string pDesc,
          string pName,
          Nullable<int> pUomID,
          string pUomName,
          Nullable<decimal> pMileage,
          Nullable<decimal> pAmountPerUOM,
          string pFile,
          string pFileName,
          Nullable<int> pJobCostID,
          Nullable<decimal> pWithholdingTax,
          string pDocNo,
          string pTaxType,
          Nullable<decimal> pWithholdingTaxAmount,
          Nullable<decimal> pTaxAmount,
          string pTaxAmountType,
          string pWithholdingTaxType,
         string pJobCostName
          )
      {
         var userlogin = UserSession.getUser(HttpContext);
         if (userlogin == null)
            return errorPage(ERROR_CODE.ERROR_401_UNAUTHORIZED);

         var model = new ExpensesDetailViewModel()
         {
            Index = pIndex.Value,
            Amount_Claiming = pAmountClaiming,
            Balance = pBalance,
            Expenses_Application_Document_ID = pDocID,
            Expenses_Config_ID = pExpensesConfigID,
            Expenses_Date = pExpensesDate,
            Doc_No = pDocNo,
            Notes = pNotes,
            Selected_Currency = pSelectedCurrency,
            Tax = pTax,
            Total_Amount = pTotalAmount,
            Expenses_Type_Desc = pDesc,
            Expenses_Type_Name = pName,
            UOM_ID = pUomID,
            UOM_Name = pUomName,
            Mileage = pMileage,
            Amount_Per_UOM = pAmountPerUOM,
            Row_Type = RowType.ADD,

            //********  Start Smart Dev  ********//
            Job_Cost_ID = pJobCostID,
            Withholding_Tax = pWithholdingTax,
            Tax_Type = pTaxType,
            Withholding_Tax_Amount = pWithholdingTaxAmount,
            Tax_Amount = pTaxAmount,
            Tax_Amount_Type = pTaxAmountType,
            Withholding_Tax_Type = pWithholdingTaxType,
            Job_Cost_Name = pJobCostName,
            //******** End Edit Smart Dev  ********//
         };

         //Added by Jane 26-11-2015
         if (!string.IsNullOrEmpty(pFile) && pFile.Contains("data:"))
         {
            string trimmedData = pFile;
            var prefixindex = trimmedData.IndexOf(",");
            trimmedData = trimmedData.Substring(prefixindex + 1, trimmedData.Length - (prefixindex + 1));
            var filebyte = Convert.FromBase64String(trimmedData);
            if (filebyte != null)
            {
               model.Upload_Receipt = trimmedData;
               model.Upload_Receipt_Name = pFileName;
            }
         }
         return PartialView("ApplicationDetailRow", model);
      }


      [HttpGet]
      public void ApplicationFile(Nullable<Guid> pUploadID)
      {
         var eService = new ExpenseService();
         var file = eService.GetReceiptUpload(pUploadID);
         if (file != null && file.Receipt != null)
         {
            Response.ClearHeaders();
            Response.Clear();
            if (file.File_Name.Contains(".pdf"))
               Response.AddHeader("Content-Type", "application/pdf");
            else if (file.File_Name.Contains(".xls"))
               Response.AddHeader("Content-Type", "application/ms-excel");
            else
               Response.AddHeader("Content-Type", "text/plain");

            Response.AddHeader("Content-Length", file.Receipt.Length.ToString());
            Response.AddHeader("Content-Disposition", "inline; filename=\"" + file.File_Name + "\"");
            Response.BinaryWrite(file.Receipt);
            Response.Flush();
            Response.End();
         }
      }

      [HttpGet]
      public void ExpensesDocPrint(string eID = null, string operation = null)
      {
         var userlogin = UserSession.getUser(HttpContext);
         ExpensesDocPrintViewModel model = new ExpensesDocPrintViewModel();
         var exService = new ExpenseService();
         model.operation = EncryptUtil.Decrypt(operation);
         model.Expenses_ID = NumUtil.ParseInteger(EncryptUtil.Decrypt(eID));

         if (model.Expenses_ID.HasValue && model.Expenses_ID.Value > 0 && model.operation == UserSession.RIGHT_U)
         {
            var ex = exService.getExpenseApplication(model.Expenses_ID);
            if (ex != null && ex.Employee_Profile != null)
            {
               var SmartDevPdf = getFileExpenseSmartDevPdf(EncryptUtil.Encrypt(ex.Expenses_Application_ID), EncryptUtil.Encrypt(UserSession.RIGHT_U), ex.Employee_Profile_ID);
               if (SmartDevPdf != null)
               {
                  if (SmartDevPdf.File != null && SmartDevPdf.File_Name != null)
                  {
                     Response.ContentType = "application/pdf";
                     Response.AddHeader("content-disposition", "inline; filename=" + SmartDevPdf.File_Name);
                     Response.BinaryWrite(SmartDevPdf.File.ToArray());
                     Response.Flush();
                     Response.Close();
                  }
               }
            }
         }
      }

      #endregion

      [HttpGet]
      [AllowAuthorized]
      public ActionResult TransactionSummary(ExpensesSummaryViewModel model, int pno = 1, int plen = 50)
      {
         var userlogin = UserSession.getUser(HttpContext);
         if (userlogin == null)
            return errorPage(ERROR_CODE.ERROR_401_UNAUTHORIZED);

         //Validate Page Right
         var rightResult = base.validatePageRight(UserSession.RIGHT_A, "/Expenses/ExpenseReport");
         if (rightResult.action != null) return rightResult.action;
         model.rights = rightResult.rights;

         var eService = new ExpenseService();
         var cbService = new ComboService();
         var empService = new EmployeeService();
        

         if (userlogin.Employee_Profile.Count == 0)
            return errorPage(ERROR_CODE.ERROR_16_NO_EMPLOYEE_PROFILE);

         var com = new CompanyService().GetCompany(userlogin.Company_ID.Value);
         if (com != null && !com.Currency_ID.HasValue)
            return errorPage(ERROR_CODE.ERROR_13_NO_DEFAULT_CURRENCY);

         var currentdate = StoredProcedure.GetCurrentDate();
         model.Monthlst = cbService.LstMonth(true);
         model.Statuslst = cbService.LstApprovalStatus(true);
         model.JobCostList=    cbService.LstJobCost(com.Company_ID);

         var criteriaPending = new ExpenseCriteria()
         {
            Company_ID = userlogin.Company_ID,
            Employee_Profile_ID = model.search_Emp,
            Include_Draft = true,
            Month = model.Search_Month,
            Year = model.Search_Year,
            Job_Cost_ID = model.search_Jobcost,
            Page_Size = plen,
            Page_No = pno,
            Overall_Status = model.search_Status,
         };
         var presult = eService.LstExpenses(criteriaPending);
         if (presult.Object != null)
            model.expensesApplicationList = (List<Expenses_Application>)presult.Object;

         model.Record_Count = presult.Record_Count;
         model.Page_No = pno;
         model.Page_Length = plen;


         //filter
         var emp = new List<Employee_Profile>();
         var criteria2 = new EmployeeCriteria() { 
            Company_ID = userlogin.Company_ID 
         };
         var pResult2 = empService.LstEmployeeProfile(criteria2);
         if (pResult2.Object != null)
         {
            emp = (List<Employee_Profile>)pResult2.Object;
            model.EmployeeList = emp;
         }

         return View(model);
      }

      public ActionResult ApproverTools()
      {
         Debug.WriteLine("'***********APP DEBUG***********' " + DateTime.Now + "-Start Controller Approver Tools");
         var Output = "";
         //Migrate data tool
         var userlogin = UserSession.getUser(HttpContext);
         if (userlogin != null)
         {
            var selectEx = new List<Expenses_Application>();

            var cService = new CompanyService();
            var cri = new CompanyCriteria();
            //cri.Company_ID = 46;
            var comlst = cService.LstCompany(cri);
            if (comlst != null)
            {
               foreach (var comrow in comlst)
               {
                  var requests = new List<Request>();
                  var requestIDs = new List<int>();
                  var aService = new SBSWorkFlowAPI.Service();
                  var r = aService.GetMyTasks(comrow.Company_ID, null, null, ModuleCode.HR, ApprovalType.Expense, "");
                  if (r.Item2.IsSuccess && r.Item1 != null)
                  {
                     requests = r.Item1.ToList();
                     requestIDs = r.Item1.Select(s => s.Request_ID).ToList();
                  }

                  var ExpensesgLst = new List<Expenses_Application>();
                  var eService = new ExpenseService();
                  var uService = new UserService();
                  var criteriaPending = new ExpenseCriteria() { Company_ID = comrow.Company_ID, };
                  var presultPending = eService.LstExpenses(criteriaPending);
                  if (presultPending.Object != null)
                     ExpensesgLst = (List<Expenses_Application>)presultPending.Object;

                  if (ExpensesgLst != null)
                  {
                     Debug.WriteLine("'***********APP DEBUG***********' " + DateTime.Now + "-Info Controller Approver Tools, Company ID :" + comrow.Company_ID + " Total Expenses :" + ExpensesgLst.Count());
                     foreach (var row in ExpensesgLst)
                     {

                        //var selectIDs = new List<int>() { 1306, 1360, 1364, 1380 };
                        //if (selectIDs.Contains(row.Expenses_Application_ID))
                        //{

                        #region Approval
                        var approval = "";
                        if (string.IsNullOrEmpty(row.Cancel_Status))
                        {
                           if (!row.Request_ID.HasValue && !row.Supervisor.HasValue)
                           {
                              approval = null;
                           }
                           else if (requestIDs.Contains(row.Request_ID.HasValue ? row.Request_ID.Value : 0))
                           {
                              var request = requests.Where(w => w.Request_ID == row.Request_ID.Value).FirstOrDefault();
                              if (request != null && request.Task_Assignment != null)
                              {
                                 var tasks = request.Task_Assignment.Where(w => w.Record_Status == WfRecordStatus.InActive);
                                 if (tasks != null)
                                 {
                                    var taskArray = (string.Join("|", tasks.Select(s => s.Profile_ID).ToArray()));
                                    if (!string.IsNullOrEmpty(taskArray))
                                       approval = taskArray.ToString() + "|";
                                 }
                              }
                           }
                           else if (row.Supervisor.HasValue && row.Supervisor.Value > 0)
                           {
                              if (row.Overall_Status == WorkflowStatus.Closed || row.Overall_Status == WorkflowStatus.Rejected)
                              {
                                 var user = uService.getUserByEmployeeProfile(row.Supervisor);
                                 if (user != null)
                                    approval = user.Profile_ID.ToString() + "|";
                              }
                           }
                        }
                        else
                        {
                           if (!row.Request_Cancel_ID.HasValue && !row.Supervisor.HasValue)
                           {
                              approval = null;
                           }
                           else if (requestIDs.Contains(row.Request_Cancel_ID.HasValue ? row.Request_Cancel_ID.Value : 0))
                           {
                              var request = requests.Where(w => w.Request_ID == row.Request_Cancel_ID).FirstOrDefault();
                              if (request != null && request.Task_Assignment != null)
                              {
                                 var tasks = request.Task_Assignment.Where(w => w.Record_Status == WfRecordStatus.InActive);
                                 if (tasks != null)
                                 {
                                    var taskArray = (string.Join("|", tasks.Select(s => s.Profile_ID).ToArray()));
                                    if (!string.IsNullOrEmpty(taskArray))
                                       approval = taskArray.ToString() + "|";
                                 }
                              }
                           }
                           else if (requestIDs.Contains(row.Request_ID.HasValue ? row.Request_ID.Value : 0))
                           {
                              var request = requests.Where(w => w.Request_ID == row.Request_ID).FirstOrDefault();
                              if (request != null && request.Task_Assignment != null)
                              {
                                 var tasks = request.Task_Assignment.Where(w => w.Record_Status == WfRecordStatus.InActive);
                                 if (tasks != null)
                                 {
                                    var taskArray = (string.Join("|", tasks.Select(s => s.Profile_ID).ToArray()));
                                    if (!string.IsNullOrEmpty(taskArray))
                                       approval = taskArray.ToString() + "|";
                                 }
                              }
                           }
                           else if (row.Supervisor.HasValue && row.Supervisor.Value > 0)
                           {
                              if (row.Cancel_Status == WorkflowStatus.Cancelled || row.Cancel_Status == WorkflowStatus.Cancellation_Rejected)
                              {
                                 var user = uService.getUserByEmployeeProfile(row.Supervisor);
                                 if (user != null)
                                    approval = user.Profile_ID.ToString() + "|";
                              }
                           }
                        }
                        if (!string.IsNullOrEmpty(approval))
                           row.Approver = "|" + approval;
                        else
                           row.Approver = null;
                        #endregion
                        var IsData = true;
                        #region Next Approver
                        var nextapproval = "";
                        if (string.IsNullOrEmpty(row.Cancel_Status))
                        {
                           if (row.Overall_Status == WorkflowStatus.Closed)
                           {
                              row.Next_Approver = null;
                              IsData = false;
                           }
                           else if (row.Overall_Status == WorkflowStatus.Rejected)
                           {
                              row.Next_Approver = null;
                              IsData = false;
                           }
                        }
                        else
                        {
                           if (row.Cancel_Status == WorkflowStatus.Cancellation_Rejected)
                           {
                              row.Next_Approver = null;
                              IsData = false;
                           }
                           else if (row.Cancel_Status == WorkflowStatus.Cancelled)
                           {
                              row.Next_Approver = null;
                              IsData = false;
                           }
                        }

                        if (IsData)
                        {
                           if (string.IsNullOrEmpty(row.Cancel_Status))
                           {
                              //Normal
                              if (requestIDs.Contains(row.Request_ID.HasValue ? row.Request_ID.Value : 0))
                              {
                                 var request = requests.Where(w => w.Request_ID == row.Request_ID).FirstOrDefault();
                                 if (request != null && request.Task_Assignment != null)
                                 {
                                    if (request.Status == WorkflowStatus.Pending || request.Status == WorkflowStatus.Approved)
                                    {
                                       var tasks = request.Task_Assignment.Where(w => w.Record_Status == WfRecordStatus.Active).OrderBy(o => o.Approval_Level).ToList();
                                       if (tasks != null && tasks.Count > 0)
                                       {
                                          var task = tasks.FirstOrDefault();
                                          if (task != null)
                                          {
                                             if (task.Is_Indent != true)
                                             {
                                                nextapproval = task.Profile_ID.ToString() + "|";
                                             }
                                             else
                                             {
                                                if (task.Is_Indent.HasValue && task.Is_Indent.Value)
                                                {
                                                   var taskArray = (string.Join("|", tasks.Where(w => w.Is_Indent == true && w.Indent_Closed != true).Select(s => s.Profile_ID).ToArray()));
                                                   if (!string.IsNullOrEmpty(taskArray))
                                                      nextapproval = taskArray.ToString() + "|";
                                                }
                                             }
                                          }
                                       }
                                    }
                                 }
                              }
                              else if (row.Supervisor.HasValue && row.Supervisor > 0)
                              {
                                 var user = uService.getUserByEmployeeProfile(row.Supervisor);
                                 if (user != null)
                                    nextapproval = user.Profile_ID.ToString() + "|";
                              }
                           }
                           else
                           {
                              if (row.Cancel_Status == WorkflowStatus.Canceling)
                              {
                                 //Canceling
                                 if (requestIDs.Contains(row.Request_Cancel_ID.HasValue ? row.Request_Cancel_ID.Value : 0))
                                 {
                                    var request = requests.Where(w => w.Request_ID == row.Request_Cancel_ID).FirstOrDefault();
                                    if (request != null && request.Task_Assignment != null)
                                    {
                                       var tasks = request.Task_Assignment.Where(w => w.Record_Status == WfRecordStatus.Active).OrderBy(o => o.Approval_Level).ToList();
                                       if (tasks != null && tasks.Count > 0)
                                       {
                                          var task = tasks.FirstOrDefault();
                                          if (task != null)
                                          {
                                             if (task.Is_Indent != true)
                                             {
                                                nextapproval = task.Profile_ID.ToString() + "|";
                                             }
                                             else
                                             {
                                                if (task.Is_Indent.HasValue && task.Is_Indent.Value)
                                                {
                                                   var taskArray = (string.Join("|", tasks.Where(w => w.Is_Indent == true && w.Indent_Closed != true).Select(s => s.Profile_ID).ToArray()));
                                                   if (!string.IsNullOrEmpty(taskArray))
                                                      nextapproval = taskArray.ToString() + "|";
                                                }
                                             }
                                          }
                                       }
                                    }
                                 }
                                 else if (row.Supervisor.HasValue && row.Supervisor > 0)
                                 {
                                    var user = uService.getUserByEmployeeProfile(row.Supervisor);
                                    if (user != null)
                                       nextapproval = user.Profile_ID.ToString() + "|";
                                 }
                              }
                           }
                        }

                        if (!string.IsNullOrEmpty(nextapproval))
                           row.Next_Approver = "|" + nextapproval;
                        else
                           row.Next_Approver = null;
                        #endregion
                        selectEx.Add(row);
                        //}

                     }


                     var result = eService.toolsAp(ExpensesgLst);
                     if (result != null && result.Code == ERROR_CODE.SUCCESS)
                     {
                        Output = "Expenses Application " + result.Msg;
                     }
                  }


               }
               //var lst = selectEx.ToList();
            }
         }
         Debug.WriteLine("'***********APP DEBUG***********' " + DateTime.Now + "-End Controller Approver Tools");
         return Json(new { Result = Output }, JsonRequestBehavior.AllowGet);
      }
   }
}