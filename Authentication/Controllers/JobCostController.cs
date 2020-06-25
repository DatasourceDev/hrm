using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Authentication.Models;
using Authentication.Common;
using SBSModel.Models;
using SBSModel.Common;
using SBSResourceAPI;
using SBSWorkFlowAPI.Constants;
using System.Web.Routing;
using System.IO;
using System.Text;
using iTextSharp.text;
using iTextSharp.text.html.simpleparser;
using iTextSharp.text.pdf;
using System.Web.UI.WebControls;
using System.Data;
using System.Web.UI;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System.Drawing;
using System.Net;
using Newtonsoft.Json.Linq;
using System.Text.RegularExpressions;

namespace Authentication.Controllers
{
   [Authorize]
   public class JobCostController : ControllerBase
   {
      [HttpGet]
      [AllowAuthorized]
      public ActionResult JobCost(ServiceResult result, JobCostViewModel model, string apply, int[] JobCostIDs = null)
      {

         var currentdate = StoredProcedure.GetCurrentDate();
         User_Profile userlogin = UserSession.getUser(HttpContext);
         if (userlogin == null)
            return errorPage(ERROR_CODE.ERROR_401_UNAUTHORIZED);

         ////Validate Page Right
         RightResult rightResult = base.validatePageRight(UserSession.RIGHT_A);
         if (rightResult.action != null) return rightResult.action;
         model.rights = rightResult.rights;
         model.result = result;

         var jobcostService = new JobCostService();
         var custService = new Customer2Service();
         var cbService = new ComboService();

         var criteria = new JobCostCriteria()
           {
              Company_ID = userlogin.Company_ID,
              Customer_ID = model.search_Customer_ID,
           };

         model.JobCostList = jobcostService.LstJobCost(criteria);
         model.cCustomerlst = cbService.LstCustomer(userlogin.Company_ID);
         if (JobCostIDs != null)
         {
            //Check use in Time Sheet
            foreach (var Job_Cost_ID in JobCostIDs)
            {
               var job = jobcostService.GetJobCost(Job_Cost_ID);

               if (job != null)
               {
                  if (apply == RecordStatus.Active | apply == RecordStatus.Inactive)
                     job.Record_Status = apply;

                  else if (apply == UserSession.RIGHT_D)
                     job.Record_Status = RecordStatus.Delete;

                  job.Update_By = userlogin.User_Authentication.Email_Address;
                  job.Update_On = currentdate;
                  model.result = jobcostService.UpdateJobCost(job);
               }
            }
            if (model.result.Code == ERROR_CODE.SUCCESS)
            {
               if (apply == RecordStatus.Active | apply == RecordStatus.Inactive)
                  return RedirectToAction("JobCost", new RouteValueDictionary(model.result));
               else if (apply == UserSession.RIGHT_D)
                  return RedirectToAction("JobCost", new { Code = ERROR_CODE.SUCCESS, Msg = new Success().getSuccess(ERROR_CODE.SUCCESS_DELETE), Field = Resource.Job_Cost });
            }
         }

         return View(model);
      }

      [HttpGet]
      [AllowAuthorized]
      public ActionResult JobCostInfo(ServiceResult result, string pJobCostID, string operation)
      {
         var currentdate = StoredProcedure.GetCurrentDate();
         User_Profile userlogin = UserSession.getUser(HttpContext);
         if (userlogin == null)
            return errorPage(ERROR_CODE.ERROR_401_UNAUTHORIZED);

         var model = new JobCostInfoViewModel();
         model.Job_Cost_ID = NumUtil.ParseInteger(EncryptUtil.Decrypt(pJobCostID));
         model.operation = EncryptUtil.Decrypt(operation);
         model.Company_ID = userlogin.Company_ID;

         //Validate Page Right
         RightResult rightResult = base.validatePageRight(model.operation, "/JobCost/JobCost");
         if (rightResult.action != null) return rightResult.action;
         model.rights = rightResult.rights;
         model.result = result;

         var jobcostService = new JobCostService();
         var custService = new Customer2Service();
         var cbService = new ComboService();
         var exService = new ExpenseService();

         if (model.operation == UserSession.RIGHT_C)
         {
            model.Costing = 0.00M;
         }
         else if (model.operation == UserSession.RIGHT_U)
         {
            var job = jobcostService.GetJobCost(model.Job_Cost_ID);
            if (job == null)
               return errorPage(ERROR_CODE.ERROR_511_DATA_NOT_FOUND);

            model.Indent_No = job.Indent_No;
            model.Indent_Name = job.Indent_Name;
            model.Customer_ID = job.Customer_ID;
            model.Date_Of_Date = DateUtil.ToDisplayDate(job.Date_Of_Date);
            model.Sell_Price = job.Sell_Price;
            model.Delivery_Date = DateUtil.ToDisplayDate(job.Delivery_Date);
            model.Term_Of_Deliver = job.Term_Of_Deliver;
            model.Warranty_Term = job.Warranty_Term;
            model.Costing = job.Costing;
            model.Record_Status = job.Record_Status;
            model.Supervisor = job.Supervisor;

            var JobCostPaymentTerm = new List<JobCostPaymentTermViewModel>();
            if (job.Job_Cost_Payment_Term != null && job.Job_Cost_Payment_Term.Count() > 0)
            {
               var i = 0;
               foreach (var row in job.Job_Cost_Payment_Term)
               {
                  JobCostPaymentTerm.Add(new JobCostPaymentTermViewModel()
                  {
                     Job_Cost_PayMent_Term_ID = row.Job_Cost_PayMent_Term_ID,
                     Job_Cost_ID = model.Job_Cost_ID,
                     Payment = row.Payment,
                     Payment_Type = row.Payment_Type,
                     Invoice_No = row.Invoice_No,
                     Invoice_Date = DateUtil.ToDisplayDate(row.Invoice_Date),
                     Note = row.Note,
                     Actual_Price = row.Actual_Price,
                     Row_Type = RowType.EDIT,
                     Index = i,
                  });

                  i++;
               }
            }
            model.JobCostPaymentTerm_Rows = JobCostPaymentTerm.ToArray();

            var JobCostExBudgets = new List<JobCostExBudgetViewModel>();
            if (job.Expenses_Config_Budget != null && job.Expenses_Config_Budget.Count() > 0)
            {
               var i = 0;
               foreach (var row in job.Expenses_Config_Budget)
               {
                  JobCostExBudgets.Add(new JobCostExBudgetViewModel()
                  {
                     Budget_ID = row.Budget_ID,
                     Job_Cost_ID = model.Job_Cost_ID,
                     Expenses_Config_ID = row.Expenses_Config_ID,
                     Budget = row.Budget,
                     Row_Type = RowType.EDIT,
                     Index = i,
                  });
                  i++;
               }
            }
            model.JobCostExBudget_Rows = JobCostExBudgets.ToArray();

         }
         else if (model.operation == UserSession.RIGHT_D)
         {
            var job = jobcostService.GetJobCost(model.Job_Cost_ID);
            if (job == null)
               return errorPage(ERROR_CODE.ERROR_511_DATA_NOT_FOUND);

            job.Record_Status = RecordStatus.Delete;
            job.Update_By = userlogin.User_Authentication.Email_Address;
            job.Update_On = currentdate;

            model.result = jobcostService.UpdateJobCost(job);
            if (model.result.Code == ERROR_CODE.SUCCESS)
               return RedirectToAction("JobCost", new { Code = ERROR_CODE.SUCCESS, Msg = new Success().getSuccess(ERROR_CODE.SUCCESS_DELETE), Field = Resource.Job_Cost });

         }

         model.cPaymentPeriodlst = cbService.LstPaymentPeriod(false);
         model.cCustomerlst = cbService.LstCustomer(userlogin.Company_ID);
         model.cExpensesTypelist = cbService.LstExpensesType(userlogin.Company_ID);
         model.cSupervisorlst = cbService.LstEmployee(userlogin.Company_ID, true);

         return View(model);

      }

      public ActionResult AddNewPaymentTerm(int pIndex)
      {
         var userlogin = UserSession.getUser(HttpContext);
         var cbService = new ComboService();
         var model = new JobCostPaymentTermViewModel()
         {
            Index = pIndex,
            Row_Type = RowType.ADD,
         };
         model.cPaymentPeriodlst = cbService.LstPaymentPeriod(false);
         return PartialView("_JobCostInfoRow", model);
      }

      public ActionResult AddNewBudget(int pIndex)
      {
         var userlogin = UserSession.getUser(HttpContext);
         var cbService = new ComboService();
         var model = new JobCostExBudgetViewModel()
         {
            Index = pIndex,
            Row_Type = RowType.ADD,
         };
         model.cExpensesTypelist = cbService.LstExpensesType(userlogin.Company_ID);
         return PartialView("_JobCostBudgetRow", model);
      }
      [HttpPost]
      [AllowAuthorized]
      public ActionResult JobCostInfo(JobCostInfoViewModel model)
      {

         var userlogin = UserSession.getUser(HttpContext);
         if (userlogin == null)
            return errorPage(ERROR_CODE.ERROR_401_UNAUTHORIZED);

         var currentdate = StoredProcedure.GetCurrentDate();
         var jobcostService = new JobCostService();
         var custService = new Customer2Service();
         var cbService = new ComboService();

         var criteria = new JobCostCriteria() { Company_ID = userlogin.Company_ID, Indent_No_Dup = model.Indent_No, Record_Status = model.Record_Status };
         var dupJobCose = jobcostService.LstJobCost(criteria).FirstOrDefault(); ;
         if (dupJobCose != null)
         {
            if (model.operation == UserSession.RIGHT_C)
               ModelState.AddModelError("Indent_No", Resource.Message_Is_Duplicated);
            else if (model.operation == UserSession.RIGHT_U)
            {
               if (dupJobCose.Job_Cost_ID != model.Job_Cost_ID)
                  ModelState.AddModelError("Indent_No", Resource.Message_Is_Duplicated);
            }
         }

         if (model.JobCostPaymentTerm_Rows != null)
         {
            var i = 0;
            foreach (var row in model.JobCostPaymentTerm_Rows)
            {
               if (row.Row_Type == RowType.DELETE)
               {
                  DeleteModelStateError("JobCostPaymentTerm_Rows[" + i + "]");
               }
               else
               {
                  if (!row.Payment.HasValue || row.Payment == 0)
                  {
                     ModelState.AddModelError("JobCostPaymentTerm_Rows[" + i + "].Payment", Resource.Message_Is_Required);
                  }
                  else
                  {
                     if (row.Payment_Type == "P")
                     {
                        if (row.Payment.Value > 100)
                        {
                           ModelState.AddModelError("JobCostPaymentTerm_Rows[" + i + "].Payment", Resource.Payment + " " + Resource.Message_Percentage_Shouldn_T_Over + " 100%");
                        }
                     }
                  }
               }
               i++;
            }
         }

         if (model.JobCostExBudget_Rows != null)
         {
            var extypes = new List<int?>();
            var i = 0;
            foreach (var row in model.JobCostExBudget_Rows)
            {
               if (row.Row_Type == RowType.DELETE)
               {
                  DeleteModelStateError("JobCostExBudget_Rows[" + i + "]");
               }
               else
               {
                  if (!row.Budget.HasValue || row.Budget == 0)
                     ModelState.AddModelError("JobCostExBudget_Rows[" + i + "].Budget", Resource.Message_Is_Required);

                  if (!row.Expenses_Config_ID.HasValue)
                     ModelState.AddModelError("JobCostExBudget_Rows[" + i + "].Expenses_Config_ID", Resource.Message_Is_Required);
                  else if (extypes.Contains(row.Expenses_Config_ID))
                     ModelState.AddModelError("JobCostExBudget_Rows[" + i + "].Expenses_Config_ID", Resource.Duplicate_Small);

                  extypes.Add(row.Expenses_Config_ID);
               }
               i++;
            }
         }
         if (ModelState.IsValid)
         {
            var job = new Job_Cost();
            if (model.operation == UserSession.RIGHT_U)
            {
               job = jobcostService.GetJobCost(model.Job_Cost_ID);
               if (job == null)
                  return errorPage(ERROR_CODE.ERROR_511_DATA_NOT_FOUND);
            }
            job.Indent_No = model.Indent_No;
            job.Indent_Name = model.Indent_Name;
            job.Customer_ID = model.Customer_ID;
            job.Date_Of_Date = DateUtil.ToDate(model.Date_Of_Date);
            job.Sell_Price = model.Sell_Price;
            job.Delivery_Date = DateUtil.ToDate(model.Delivery_Date);
            job.Term_Of_Deliver = model.Term_Of_Deliver;
            job.Warranty_Term = model.Warranty_Term;
            job.Costing = model.Costing;
            job.Record_Status = model.Record_Status;
            job.Update_By = userlogin.User_Authentication.Email_Address;
            job.Update_On = currentdate;
            job.Company_ID = userlogin.Company_ID;
            job.Supervisor = model.Supervisor;

            if (model.JobCostPaymentTerm_Rows != null)
            {
               var JobCostPaymentTerms = new List<Job_Cost_Payment_Term>();
               foreach (var row in model.JobCostPaymentTerm_Rows)
               {
                  if (row.Row_Type != RowType.DELETE)
                  {
                     var JobCostPaymentTerm = new Job_Cost_Payment_Term()
                     {
                        Job_Cost_PayMent_Term_ID = row.Job_Cost_PayMent_Term_ID,
                        Job_Cost_ID = model.Job_Cost_ID,
                        Payment = row.Payment,
                        Payment_Type = row.Payment_Type,
                        Invoice_No = row.Invoice_No,
                        Invoice_Date = DateUtil.ToDate(row.Invoice_Date),
                        Note = row.Note,
                        Actual_Price = row.Actual_Price,
                        Update_By = userlogin.User_Authentication.Email_Address,
                        Update_On = currentdate
                     };
                     if (row.Row_Type == RowType.ADD)
                     {
                        JobCostPaymentTerm.Create_By = userlogin.User_Authentication.Email_Address;
                        JobCostPaymentTerm.Create_On = currentdate;
                     }
                     JobCostPaymentTerms.Add(JobCostPaymentTerm);
                  }
               }
               job.Job_Cost_Payment_Term = JobCostPaymentTerms.ToList();
            }

            if (model.JobCostExBudget_Rows != null)
            {
               var JobCostBudgets = new List<Expenses_Config_Budget>();
               foreach (var row in model.JobCostExBudget_Rows)
               {
                  if (row.Row_Type != RowType.DELETE)
                  {
                     var budget = new Expenses_Config_Budget()
                     {
                        Budget_ID = row.Budget_ID,
                        Job_Cost_ID = model.Job_Cost_ID,
                        Expenses_Config_ID = row.Expenses_Config_ID,
                        Budget = row.Budget,
                     };

                     JobCostBudgets.Add(budget);
                  }
               }
               job.Expenses_Config_Budget = JobCostBudgets.ToList();
            }
            if (model.operation == UserSession.RIGHT_C)
            {
               job.Record_Status = RecordStatus.Active;
               job.Create_By = userlogin.User_Authentication.Email_Address;
               job.Create_On = currentdate;
               model.result = jobcostService.InsertJobCost(job);
            }
            else if (model.operation == UserSession.RIGHT_U)
            {
               model.result = jobcostService.UpdateJobCost(job);
            }
            if (model.result.Code == ERROR_CODE.SUCCESS)
               return RedirectToAction("JobCost", new RouteValueDictionary(model.result));
         }

         //Validate Page Right
         RightResult rightResult = base.validatePageRight(model.operation, "/JobCost/JobCost");
         if (rightResult.action != null) return rightResult.action;
         model.rights = rightResult.rights;

         model.cPaymentPeriodlst = cbService.LstPaymentPeriod(false);
         model.cCustomerlst = cbService.LstCustomer(userlogin.Company_ID);
         model.cExpensesTypelist = cbService.LstExpensesType(userlogin.Company_ID);
         model.cSupervisorlst = cbService.LstEmployee(userlogin.Company_ID, true);

         return View(model);
      }


      public class ColProperty
      {
         public bool Bold { get; set; }
         public string Color { get; set; }
         public string Bg { get; set; }
         public string Align { get; set; }
      }


      //private string getExportRow(List<string> values, List<ExcelStyle> props)
      //{
      //   var csv = new StringBuilder();
      //   if (values != null)
      //   {
      //      csv.Append("<tr valign='top'>");
      //      for (var index = 0; index < values.Count; index++)
      //      {
      //         var value = values[index];
      //         var prop = props[index];
      //         csv.Append("<td");
      //         if (prop != null)
      //         {
      //            if (!string.IsNullOrEmpty(prop.Bg))
      //            {
      //               csv.Append(" bgcolor=");
      //               csv.Append(prop.Bg);
      //            }
      //            csv.Append(" style='");
      //            if (prop.Bold)
      //            {
      //               csv.Append(" font-weight:700;");
      //            }
      //            if (!string.IsNullOrEmpty(prop.Color))
      //            {
      //               csv.Append(" color:");
      //               csv.Append(prop.Color);
      //               csv.Append(";");
      //            }
      //            if (!string.IsNullOrEmpty(prop.Align))
      //            {
      //               csv.Append(" text-align:");
      //               csv.Append(prop.Align);
      //               csv.Append(";");
      //            }

      //            csv.Append("'");
      //         }
      //         csv.Append(">");
      //         csv.Append(values[index]);
      //         csv.Append("</td>");
      //      }
      //      csv.Append("</tr>");
      //   }

      //   return csv.ToString();
      //}

      public string RenderPartialViewAsString(string viewName, object model)
      {
         StringWriter stringWriter = new StringWriter();

         ViewEngineResult viewResult = ViewEngines.Engines.FindPartialView(ControllerContext, viewName);
         ViewContext viewContext = new ViewContext(
                 ControllerContext,
                 viewResult.View,
                 new ViewDataDictionary(model),
                 new TempDataDictionary(),
                 stringWriter
                 );

         viewResult.View.Render(viewContext, stringWriter);
         return stringWriter.ToString();
      }

      private const string PERCENT = "%";

      //public ActionResult JobCostInfoExport(string pJobCostID, string operation)
      //{


      //   var model = new JobCostExportViewModel();
      //   //model.csv = htmlToConvert;
      //   var filename = "123";
      //   //var htmlToConvert = RenderPartialViewAsString("JobCostInfoExport", model);
      //   var htmlToConvert = JobCostInfoExport2(pJobCostID,ref filename);
      //   Response.Clear();
      //   Response.Buffer = true;
      //   Response.AddHeader("content-disposition", "attachment;filename=" + filename + ".xls");
      //   Response.ContentEncoding = System.Text.Encoding.UTF8;
      //   Response.BinaryWrite(System.Text.Encoding.UTF8.GetPreamble());
      //   Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
      //   var gv = new GridView();
      //   gv.DataBind();
      //   using (StringWriter sw = new StringWriter())
      //   {
      //      sw.Write(htmlToConvert);
      //      HtmlTextWriter hw = new HtmlTextWriter(sw);
      //      //gv.RenderControl(hw);
      //      Response.Write("<meta http-equiv=\"Content-Type\" content=application/vnd.openxmlformats-officedocument.spreadsheetml.sheet; charset=utf-8 />");           
      //      Response.Output.Write(sw.ToString());
      //      Response.Flush();
      //      Response.Close();
      //      Response.End();
      //   }

      //   //System.Web.UI.WebControls.GridView gv = new System.Web.UI.WebControls.GridView();
      //   //gv.DataBind();
      //   //Response.Clear();
      //   //Response.ClearContent();
      //   //Response.ClearHeaders();
      //   //Response.Buffer = true;
      //   //Response.AddHeader("content-disposition", "attachment;filename=" + filename + ".xls");
      //   //Response.ContentType = "application/ms-excel";
      //   //Response.Charset = "";
      //   //Response.Write("<meta http-equiv=\"Content-Type\" content=\"text/html; charset=utf-8\" />");

      //   //using (StringWriter sw = new StringWriter())
      //   //{
      //   //   HtmlTextWriter hw = new HtmlTextWriter(sw);
      //   //   sw.Write(htmlToConvert);
      //   //   gv.RenderControl(hw);
      //   //   Response.Output.Write(sw.ToString());
      //   //   Response.Flush();
      //   //   Response.Close();
      //   //   Response.End();
      //   //}



      //   return View(model);
      //}

      public List<_Time_Sheet> LstTimesheet(Nullable<int> pComID, Nullable<int> pJobID)
      {
         List<_Time_Sheet> timesheets = new List<_Time_Sheet>();
         HttpWebResponse response = null;
         StreamReader readStream = null;
         var request = (HttpWebRequest)WebRequest.Create(string.Format(AppSetting.SERVER_NAME + ModuleDomain.Time + "/WServ/LstTimesheet?pComID=" + pComID + "&pJobID=" + pJobID));
         request.Method = "Get";
         try
         {
            response = (HttpWebResponse)request.GetResponse();
            Stream receiveStream = response.GetResponseStream();
            readStream = new StreamReader(receiveStream, Encoding.UTF8);
            var rawJson = readStream.ReadToEnd();
            var json = JObject.Parse(rawJson);
            timesheets = json["Timesheets"].ToObject<List<_Time_Sheet>>();
         }
         catch (Exception ex)
         {

         }
         finally
         {
            if (readStream != null)
               readStream.Close();
            if (response != null)
               response.Close();
         }

         return timesheets;
      }

      [HttpGet]
      public string JobCostInfoExport(string pJobCostID)
      {

         var currentdate = StoredProcedure.GetCurrentDate();
         var userlogin = UserSession.getUser(HttpContext);
         var jID = NumUtil.ParseInteger(EncryptUtil.Decrypt(pJobCostID));

         var jobcostService = new JobCostService();
         var custService = new Customer2Service();
         var cbService = new ComboService();
         var exService = new ExpenseService();
         var comService = new CompanyService();
         var expenService = new ExpenseService();
         var csv = new StringBuilder();

         var job = jobcostService.GetJobCost(jID);
         if (job != null)
         {
            var timesheets = LstTimesheet(userlogin.Company_ID, job.Job_Cost_ID);
            var filename = job.Indent_No + "_" + job.Indent_Name;
            var comname = "";
            var addr1 = "";
            var addr2 = "";
            var addr3 = "";
            var cusname = "";
            var com = comService.GetCompany(userlogin.Company_ID);
            if (com != null)
            {
               comname = com.Name;
               addr1 = com.Address;
               addr2 = (com.State_ID.HasValue ? com.State.Descrition : "") + " " + com.Zip_Code + " " + (com.Currency_ID.HasValue ? com.Country.Description : "");
               addr3 = Resource.Tel + ": " + com.Phone + "   " + Resource.Fax + ": " + com.Fax;
            }
            if (job.Customer != null)
               cusname = job.Customer.Customer_Name;

            //-----------------------------------------------------//

            var totalAmountActual = 0.00M;
            var totalAmountValue = 0.00M;
            var totalPercenActual = 0.00M;

            int minyear = currentdate.Year;
            int maxyear = currentdate.Year;
            var dates = new List<DateTime?>();
            if (job.Job_Cost_Payment_Term != null)
            {
               var invoicedates = job.Job_Cost_Payment_Term.Where(w => w.Invoice_Date.HasValue).OrderBy(o => o.Invoice_Date).Select(s => s.Invoice_Date).Distinct().ToList();
               if (invoicedates.Count() > 0)
                  dates.AddRange(invoicedates);
            }


            var expenses = new List<Expenses_Application_Document>();
            var ecri = new ExpenseCriteria();
            ecri.Company_ID = userlogin.Company_ID;
            ecri.Job_Cost_ID = job.Job_Cost_ID;
            ecri.Closed_Status = true;
            var eresult = expenService.LstExpenseApplications(ecri);
            if (eresult.Object != null)
            {
               expenses = eresult.Object as List<Expenses_Application_Document>;
            }

            if (expenses != null)
            {
               var exdates = expenses.Where(w => w.Expenses_Date.HasValue).OrderBy(o => o.Expenses_Date).Select(s => s.Expenses_Date).Distinct().ToList();
               if (exdates.Count() > 0)
                  dates.AddRange(exdates);
            }
            if (dates.Count() > 0)
            {
               dates = dates.OrderBy(s => s.Value).ToList();
               var min = dates[0].Value;
               minyear = min.Year;
               var max = dates[dates.Count() - 1].Value;
               maxyear = max.Year;
            }
            var ccri = new ExpensesCategoryCriteria();
            ccri.Company_ID = userlogin.Company_ID;
            var excates = expenService.LstExpensesCategory(ccri);

            var rightcolspan = 1;
            for (int year = minyear; year <= maxyear; year++)
            {
               for (int k = 1; k <= 12; k++)
                  rightcolspan++;
            }
            var headercolspan = 10;
            var leftcolspan = 10;
            var allcolspan = leftcolspan + rightcolspan;
            //------------------------- Start Job Cost----------------------------//

            var totalPermonths = new decimal[rightcolspan];

            ExcelPackage pck = new ExcelPackage();
            var ws = pck.Workbook.Worksheets.Add(filename);

            var row = 1;
            var col = 1;

            ws.Cells[row, col].Value = comname;
            ws.Cells[row, col, row, headercolspan].Merge = true;
            ws.Cells[row, col, row, headercolspan].Style.Font.Color.SetColor(Color.Green);
            ws.Cells[row, col, row, headercolspan].Style.Font.Bold = true;
            ws.Cells[row, col, row, headercolspan].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
            row++;

            ws.Cells[row, col].Value = addr1;
            ws.Cells[row, col, row, headercolspan].Merge = true;
            ws.Cells[row, col, row, headercolspan].Style.Font.Bold = true;
            ws.Cells[row, col, row, headercolspan].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
            row++;

            ws.Cells[row, col].Value = addr2;
            ws.Cells[row, col, row, headercolspan].Merge = true;
            ws.Cells[row, col, row, headercolspan].Style.Font.Bold = true;
            ws.Cells[row, col, row, headercolspan].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
            row++;

            ws.Cells[row, col].Value = addr3;
            ws.Cells[row, col, row, headercolspan].Merge = true;
            ws.Cells[row, col, row, headercolspan].Style.Font.Bold = true;
            ws.Cells[row, col, row, headercolspan].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
            row++;
            row++;

            ws.Cells[row, col].Value = Resource.Project + " / " + Resource.Job_Cost;
            ws.Cells[row, col, row, headercolspan].Style.Font.Size = (float)(ws.Cells[row, col, row, headercolspan].Style.Font.Size * 1.4);
            ws.Cells[row, col, row, headercolspan].Style.Font.Bold = true;
            ws.Cells[row, col, row, headercolspan].Merge = true;
            ws.Cells[row, col, row, headercolspan].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            row++;
            row++;

            ws.Cells[row, col].Value = Resource.SDS_Job_No + " : " + job.Indent_No;
            ws.Cells[row, (headercolspan / 2) + 1].Value = Resource.Date_Of_Date + " : ";
            ws.Cells[row, (headercolspan / 2) + 2].Value = DateUtil.ToDisplayDate(job.Date_Of_Date);
            row++;

            ws.Cells[row, col].Value = Resource.Client_Name + " : " + cusname;
            ws.Cells[row, (headercolspan / 2) + 1].Value = Resource.Delivery_Date + " : ";
            ws.Cells[row, (headercolspan / 2) + 2].Value = DateUtil.ToDisplayDate(job.Delivery_Date);
            row++;

            ws.Cells[row, col].Value = Resource.Project_Name + " : " + job.Indent_Name;
            ws.Cells[row, (headercolspan / 2) + 1].Value = Resource.Term_Of_Deliver + " : ";
            ws.Cells[row, (headercolspan / 2) + 2].Value = job.Term_Of_Deliver;
            row++;

            ws.Cells[row, col].Value = "";
            ws.Cells[row, (headercolspan / 2) + 1].Value = Resource.Warranty_Term + " : ";
            ws.Cells[row, (headercolspan / 2) + 2].Value = job.Warranty_Term;
            row++;
            row++;

            ws.Cells[row, col].Value = Resource.For_Sale_Project_Management;
            ws.Cells[row, col, row, leftcolspan].Style.Fill.PatternType = ExcelFillStyle.Solid;
            ws.Cells[row, col, row, leftcolspan].Style.Fill.BackgroundColor.SetColor(Color.Yellow);
            ws.Cells[row, col, row, leftcolspan].Merge = true;
            ws.Cells[row, col, row, leftcolspan].Style.Font.Bold = true;
            ws.Cells[row, col, row, leftcolspan].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

            ws.Cells[row, leftcolspan + 1].Value = "";
            ws.Cells[row, leftcolspan, row, allcolspan].Style.Fill.PatternType = ExcelFillStyle.Solid;
            ws.Cells[row, leftcolspan, row, allcolspan].Style.Fill.BackgroundColor.SetColor(Color.CornflowerBlue);
            ws.Cells[row, leftcolspan + 1, row, allcolspan].Merge = true;
            ws.Cells[row, leftcolspan + 1, row, allcolspan].Style.Font.Bold = true;
            ws.Cells[row, leftcolspan + 1, row, allcolspan].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            row++;

            /***** Header 1 *******/
            ws.Cells[row, col, row, allcolspan].Style.Font.Bold = true;
            ws.Cells[row, col, row, allcolspan].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            ws.Cells[row, col, row, allcolspan].Style.Fill.PatternType = ExcelFillStyle.Solid;
            ws.Cells[row, col, row, allcolspan].Style.Fill.BackgroundColor.SetColor(Color.LightGray);
            for (int c = 1; c <= allcolspan; c++)
               ws.Cells[row, c].Style.Border.BorderAround(ExcelBorderStyle.Thin);

            ws.Cells[row, 1].Value = Resource.Sell_Price;
            ws.Cells[row, 2].Value = Resource.Sell_Price;
            ws.Cells[row, 2, row, 3].Merge = true;
            ws.Cells[row, 4].Value = Resource.Project;
            ws.Cells[row, 4, row, 5].Merge = true;
            ws.Cells[row, 6].Value = Resource.Actual_Price;
            ws.Cells[row, 6, row, 7].Merge = true;
            ws.Cells[row, 8].Value = Resource.Customer;
            ws.Cells[row, 8, row, 10].Merge = true;
            ws.Cells[row, 11, row, allcolspan].Merge = true;
            row++;

            /***** Header 2 *******/
            ws.Cells[row, col, row, allcolspan].Style.Font.Bold = true;
            ws.Cells[row, col, row, allcolspan].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            ws.Cells[row, col, row, allcolspan].Style.Fill.PatternType = ExcelFillStyle.Solid;
            ws.Cells[row, col, row, allcolspan].Style.Fill.BackgroundColor.SetColor(Color.LightGray);
            for (int c = 1; c <= allcolspan; c++)
               ws.Cells[row, c].Style.Border.BorderAround(ExcelBorderStyle.Thin);

            ws.Cells[row, 1].Value = "";
            ws.Cells[row, 2].Value = Resource.Bath;
            ws.Cells[row, 3].Value = PERCENT;
            ws.Cells[row, 4].Value = Resource.Bath;
            ws.Cells[row, 5].Value = PERCENT;
            ws.Cells[row, 6].Value = Resource.Bath;
            ws.Cells[row, 7].Value = PERCENT;
            ws.Cells[row, 8].Value = Resource.Invoice_No_SymbolDot;
            ws.Cells[row, 9].Value = Resource.Invoiced_Date;
            ws.Cells[row, 10].Value = Resource.Value;

            int n = 1;
            for (int year = minyear; year <= maxyear; year++)
               for (int k = 1; k <= 12; k++)
               {
                  ws.Cells[row, leftcolspan + n].Value = DateUtil.GetFullMonth(k) + " " + year.ToString();
                  n++;
               }
            ws.Cells[row, allcolspan].Value = Resource.Total;
            ws.Cells[row, allcolspan].Style.Border.BorderAround(ExcelBorderStyle.Thin);

            row++;

            /***** Job Cost Detail 1 *******/
            ws.Cells[row, col, row, allcolspan].Style.Font.Bold = true;
            ws.Cells[row, col, row, allcolspan].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
            for (int c = 1; c <= allcolspan; c++)
               ws.Cells[row, c].Style.Border.BorderAround(ExcelBorderStyle.Thin);

            ws.Cells[row, 1].Value = Resource.Sell_Price;
            ws.Cells[row, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
            ws.Cells[row, 2].Value = NumUtil.FormatCurrencyExcel(job.Sell_Price);
            ws.Cells[row, 3].Value = NumUtil.FormatPercenExcel(100);
            ws.Cells[row, 4].Value = NumUtil.FormatCurrencyExcel(job.Sell_Price);
            ws.Cells[row, 5].Value = NumUtil.FormatPercenExcel(100);
            ws.Cells[row, 6].Value = NumUtil.FormatCurrencyExcel(job.Sell_Price);
            ws.Cells[row, 7].Value = NumUtil.FormatPercenExcel(100);
            ws.Cells[row, 8].Value = "";
            ws.Cells[row, 9].Value = "";
            ws.Cells[row, 10].Value = "";

            n = 1;
            for (int year = minyear; year <= maxyear; year++)
            {
               for (int k = 1; k <= 12; k++)
               {
                  ws.Cells[row, leftcolspan + n].Value = "";
                  n++;
               }
            }

            ws.Cells[row, allcolspan].Value = "";
            ws.Cells[row, allcolspan].Style.Border.BorderAround(ExcelBorderStyle.Thin);
            row++;

            /***** Job Cost Detail 2 *******/
            ws.Cells[row, col, row, allcolspan].Style.Font.Bold = true;
            ws.Cells[row, col, row, headercolspan].Style.Font.Color.SetColor(Color.CornflowerBlue);
            ws.Cells[row, col, row, allcolspan].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
            for (int c = 1; c <= allcolspan; c++)
               ws.Cells[row, c].Style.Border.BorderAround(ExcelBorderStyle.Thin);

            ws.Cells[row, 1].Value = Resource.Customer_Payment_term;
            row++;
            var monthindex = 0;
            if (job.Job_Cost_Payment_Term != null)
            {
               var tcnt = 1;
               foreach (var portion in job.Job_Cost_Payment_Term)
               {
                  var ActualPercent = 0.00M;
                  if (portion.Actual_Price.HasValue && job.Sell_Price.HasValue && portion.Actual_Price.Value > 0 && job.Sell_Price.Value > 0)
                     ActualPercent = ((portion.Actual_Price.Value * 100) / job.Sell_Price.Value);

                  /***** Job Term *******/
                  ws.Cells[row, col, row, allcolspan].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                  for (int c = 1; c <= allcolspan; c++)
                     ws.Cells[row, c].Style.Border.BorderAround(ExcelBorderStyle.Thin);

                  ws.Cells[row, 1].Value = Resource.Position + " " + tcnt + " " + (portion.Payment_Type == "P" ? (NumUtil.FormatPercenExcel(portion.Payment)) : (NumUtil.FormatCurrencyExcel(portion.Payment))) + " " + portion.Note;
                  ws.Cells[row, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                  ws.Cells[row, 2].Value = "";
                  ws.Cells[row, 3].Value = "";
                  ws.Cells[row, 4].Value = "";
                  ws.Cells[row, 5].Value = "";
                  ws.Cells[row, 6].Value = NumUtil.FormatCurrencyExcel(portion.Actual_Price);
                  ws.Cells[row, 7].Value = NumUtil.FormatPercenExcel(ActualPercent);
                  ws.Cells[row, 8].Value = portion.Invoice_No;
                  ws.Cells[row, 9].Value = DateUtil.ToDisplayDate(portion.Invoice_Date);
                  ws.Cells[row, 10].Value = NumUtil.FormatCurrencyExcel(portion.Actual_Price);
                  monthindex = 0;
                  n = 1;
                  for (int year = minyear; year <= maxyear; year++)
                  {
                     for (int k = 1; k <= 12; k++)
                     {
                        if (portion.Invoice_Date.HasValue && year == portion.Invoice_Date.Value.Year && k == portion.Invoice_Date.Value.Month)
                        {
                           ws.Cells[row, leftcolspan + n].Value = NumUtil.FormatCurrencyExcel(portion.Actual_Price);
                           totalPermonths[monthindex] += portion.Actual_Price.Value;
                        }
                        else
                           ws.Cells[row, leftcolspan + n].Value = "";
                        n++;
                        monthindex++;
                     }
                  }
                  ws.Cells[row, allcolspan].Value = NumUtil.FormatCurrencyExcel(portion.Actual_Price);
                  row++;

                  totalAmountActual += (portion.Actual_Price.HasValue ? portion.Actual_Price.Value : 0);
                  totalAmountValue += (portion.Actual_Price.HasValue ? portion.Actual_Price.Value : 0);
                  totalPercenActual += ActualPercent;
                  tcnt++;
               }
            }

            /***** Balance *******/
            var balanceAmt = 0M;
            var balancePercent = 0M;
            if (job.Sell_Price.HasValue)
            {
               balanceAmt = job.Sell_Price.Value - totalAmountActual;
               balancePercent = 100 - totalPercenActual;
            }

            ws.Cells[row, col, row, allcolspan].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
            ws.Cells[row, col, row, allcolspan].Style.Font.Bold = true;
            ws.Cells[row, col, row, headercolspan].Style.Font.Color.SetColor(Color.Red);
            for (int c = 1; c <= allcolspan; c++)
               ws.Cells[row, c].Style.Border.BorderAround(ExcelBorderStyle.Thin);

            ws.Cells[row, 1].Value = Resource.Balance;
            ws.Cells[row, 2].Value = "";
            ws.Cells[row, 3].Value = "";
            ws.Cells[row, 4].Value = "";
            ws.Cells[row, 5].Value = "";
            ws.Cells[row, 6].Value = NumUtil.FormatCurrencyExcel(balanceAmt);
            ws.Cells[row, 7].Value = NumUtil.FormatPercenExcel(balancePercent);
            ws.Cells[row, 8].Value = "";
            ws.Cells[row, 9].Value = "";
            ws.Cells[row, 10].Value = "";

            n = 1;
            for (int year = minyear; year <= maxyear; year++)
            {
               for (int k = 1; k <= 12; k++)
               {
                  ws.Cells[row, leftcolspan + n].Value = "";
                  n++;
               }
            }

            ws.Cells[row, allcolspan].Value = "";
            row++;

            /***** Total *******/
            var totalPercentActual = 0.00M;
            var totalPercentValue = 0.00M;

            if (totalAmountActual > 0 && (job.Sell_Price.HasValue))
               if (job.Sell_Price.Value > 0)
                  totalPercentActual = ((totalAmountActual * 100) / job.Sell_Price.Value);

            if (totalAmountValue > 0 && (job.Sell_Price.HasValue))
               if (job.Sell_Price.Value > 0)
                  totalPercentValue = ((totalAmountValue * 100) / job.Sell_Price.Value);

            ws.Cells[row, col, row, allcolspan].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
            ws.Cells[row, col, row, allcolspan].Style.Font.Bold = true;
            for (int c = 1; c <= allcolspan; c++)
               ws.Cells[row, c].Style.Border.BorderAround(ExcelBorderStyle.Thin);

            ws.Cells[row, 1].Value = Resource.Total_Receive;
            ws.Cells[row, 2].Value = NumUtil.FormatCurrencyExcel(job.Sell_Price);
            ws.Cells[row, 3].Value = NumUtil.FormatPercenExcel(100);
            ws.Cells[row, 4].Value = NumUtil.FormatCurrencyExcel(job.Sell_Price);
            ws.Cells[row, 5].Value = NumUtil.FormatPercenExcel(100);
            ws.Cells[row, 6].Value = NumUtil.FormatCurrencyExcel(totalAmountActual);
            ws.Cells[row, 7].Value = NumUtil.FormatPercenExcel(totalPercentActual);
            ws.Cells[row, 8].Value = "";
            ws.Cells[row, 9].Value = Resource.Total;
            ws.Cells[row, 10].Value = NumUtil.FormatCurrencyExcel(totalAmountValue);

            monthindex = 0;
            n = 1;
            for (int year = minyear; year <= maxyear; year++)
            {
               for (int k = 1; k <= 12; k++)
               {
                  ws.Cells[row, leftcolspan + n].Value = NumUtil.FormatCurrencyExcel(totalPermonths[monthindex]);
                  totalPermonths[monthindex] = 0; // clear total
                  monthindex++;
                  n++;
               }
            }

            ws.Cells[row, allcolspan].Value = NumUtil.FormatCurrencyExcel(totalAmountValue);
            row++;
            row++;
            //------------------------- End Job Cost----------------------------//

            //------------------------- Start Expense----------------------------//
            /***** Header 1 *******/

            ws.Cells[row, col, row, allcolspan].Style.Font.Bold = true;
            ws.Cells[row, col, row, allcolspan].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            ws.Cells[row, col, row, allcolspan].Style.Fill.PatternType = ExcelFillStyle.Solid;
            ws.Cells[row, col, row, allcolspan].Style.Fill.BackgroundColor.SetColor(Color.LightGray);
            for (int c = 1; c <= allcolspan; c++)
               ws.Cells[row, c].Style.Border.BorderAround(ExcelBorderStyle.Thin);

            ws.Cells[row, 1].Value = Resource.Cost_And_Account_Payment;
            ws.Cells[row, 2].Value = Resource.Budget;
            ws.Cells[row, 2, row, 3].Merge = true;
            ws.Cells[row, 4].Value = Resource.Budget;
            ws.Cells[row, 4, row, 5].Merge = true;
            ws.Cells[row, 6].Value = Resource.Actual_Price;
            ws.Cells[row, 6, row, 7].Merge = true;
            ws.Cells[row, 8].Value = Resource.Payment;
            ws.Cells[row, 8, row, 10].Merge = true;
            ws.Cells[row, 11, row, allcolspan].Merge = true;

            row++;

            /***** Header 2 *******/
            ws.Cells[row, col, row, allcolspan].Style.Font.Bold = true;
            ws.Cells[row, col, row, allcolspan].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            ws.Cells[row, col, row, allcolspan].Style.Fill.PatternType = ExcelFillStyle.Solid;
            ws.Cells[row, col, row, allcolspan].Style.Fill.BackgroundColor.SetColor(Color.LightGray);
            for (int c = 1; c <= allcolspan; c++)
               ws.Cells[row, c].Style.Border.BorderAround(ExcelBorderStyle.Thin);

            ws.Cells[row, 1].Value = "";
            ws.Cells[row, 2].Value = Resource.Bath;
            ws.Cells[row, 3].Value = PERCENT;
            ws.Cells[row, 4].Value = Resource.Bath;
            ws.Cells[row, 5].Value = PERCENT;
            ws.Cells[row, 6].Value = Resource.Bath;
            ws.Cells[row, 7].Value = PERCENT;
            ws.Cells[row, 8].Value = Resource.Invoice_No_SymbolDot;
            ws.Cells[row, 9].Value = Resource.Invoiced_Date;
            ws.Cells[row, 10].Value = Resource.Value;

            n = 1;
            for (int year = minyear; year <= maxyear; year++)
               for (int k = 1; k <= 12; k++)
               {
                  ws.Cells[row, leftcolspan + n].Value = DateUtil.GetFullMonth(k) + " " + year.ToString();
                  n++;
               }

            ws.Cells[row, allcolspan].Value = Resource.Total;
            ws.Cells[row, allcolspan].Style.Border.BorderAround(ExcelBorderStyle.Thin);
            row++;

            var totalAllBudget = 0M;
            var totalAllCosting = 0M;
            var totalAllBudgetPercen = 0M;
            var totalAllCostingPercen = 0M;

            foreach (var cate in excates)
            {
               decimal? totalbudget = 0M;
               decimal? totalbudgetpercen = 0M;
               decimal? totalclaim = 0M;
               decimal? totalclaimpercen = 0M;
               if (job.Expenses_Config_Budget != null)
               {
                  var jbds = job.Expenses_Config_Budget.Where(w => w.Expenses_Config.Expenses_Category_ID == cate.Expenses_Category_ID);
                  if (jbds != null && jbds.Count() > 0)
                     totalbudget = jbds.Select(s => s.Budget).Sum();

                  if (job.Sell_Price.HasValue && job.Sell_Price.Value > 0)
                     totalbudgetpercen = (totalbudget / job.Sell_Price) * 100;
               }

               var exps = expenses.Where(w => w.Expenses_Config.Expenses_Category_ID == cate.Expenses_Category_ID).ToList();
               totalclaim = exps.Select(s => (s.Amount_Claiming + s.Withholding_Tax_Amount) - s.Tax_Amount).Sum();
               if (job.Sell_Price.HasValue && job.Sell_Price.Value > 0)
                  totalclaimpercen = (totalclaim / job.Sell_Price) * 100;

               /***** Expenses Category Detail *******/
               ws.Cells[row, col, row, allcolspan].Style.Fill.PatternType = ExcelFillStyle.Solid;
               ws.Cells[row, col, row, allcolspan].Style.Fill.BackgroundColor.SetColor(Color.SkyBlue);
               ws.Cells[row, col, row, allcolspan].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
               ws.Cells[row, col, row, allcolspan].Style.Font.Bold = true;
               for (int c = 1; c <= allcolspan; c++)
                  ws.Cells[row, c].Style.Border.BorderAround(ExcelBorderStyle.Thin);

               ws.Cells[row, 1].Value = cate.Category_Name;
               ws.Cells[row, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
               ws.Cells[row, 2].Value = NumUtil.FormatCurrencyExcel(totalbudget);
               ws.Cells[row, 3].Value = NumUtil.FormatPercenExcel(totalbudgetpercen);
               ws.Cells[row, 4].Value = NumUtil.FormatCurrencyExcel(totalbudget);
               ws.Cells[row, 5].Value = NumUtil.FormatPercenExcel(totalbudgetpercen);
               ws.Cells[row, 6].Value = NumUtil.FormatCurrencyExcel(totalclaim);
               ws.Cells[row, 7].Value = NumUtil.FormatPercenExcel(totalclaimpercen);
               ws.Cells[row, 8].Value = "";
               ws.Cells[row, 9].Value = "";
               ws.Cells[row, 10].Value = "";

               n = 1;
               for (int year = minyear; year <= maxyear; year++)
                  for (int k = 1; k <= 12; k++)
                  {
                     ws.Cells[row, leftcolspan + n].Value = "";
                     n++;
                  }

               ws.Cells[row, allcolspan].Value = "";
               row++;

               var tcri = new ExpensesTypeCriteria();
               tcri.Company_ID = userlogin.Company_ID;
               tcri.Expenses_Category_ID = cate.Expenses_Category_ID;
               var etypes = expenService.LstExpenseTypes(tcri);
               foreach (var etype in etypes)
               {
                  var budget = 0M;
                  var budgetpercen = 0M;
                  if (job.Expenses_Config_Budget != null)
                  {
                     var jbd = job.Expenses_Config_Budget.Where(w => w.Expenses_Config_ID == etype.Expenses_Config_ID).FirstOrDefault();
                     if (jbd != null && jbd.Budget.HasValue)
                        budget = jbd.Budget.Value;
                     if (job.Sell_Price.HasValue && job.Sell_Price.Value > 0)
                        budgetpercen = (budget / job.Sell_Price.Value) * 100;
                  }

                  totalAllBudget += budget;
                  totalAllBudgetPercen += budgetpercen;
                  /***** Expenses Type Detail *******/
                  ws.Cells[row, col, row, allcolspan].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                  for (int c = 1; c <= allcolspan; c++)
                     ws.Cells[row, c].Style.Border.BorderAround(ExcelBorderStyle.Thin);

                  ws.Cells[row, 1].Value = "    " + etype.Expenses_Name;
                  ws.Cells[row, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                  ws.Cells[row, 2].Value = NumUtil.FormatCurrencyExcel(budget);
                  ws.Cells[row, 3].Value = NumUtil.FormatPercenExcel(budgetpercen);
                  ws.Cells[row, 4].Value = NumUtil.FormatCurrencyExcel(budget);
                  ws.Cells[row, 5].Value = NumUtil.FormatPercenExcel(budgetpercen);
                  ws.Cells[row, 6].Value = "";
                  ws.Cells[row, 7].Value = "";
                  ws.Cells[row, 8].Value = "";
                  ws.Cells[row, 9].Value = "";
                  ws.Cells[row, 10].Value = "";

                  n = 1;
                  for (int year = minyear; year <= maxyear; year++)
                     for (int k = 1; k <= 12; k++)
                     {
                        ws.Cells[row, leftcolspan + n].Value = "";
                        n++;
                     }

                  ws.Cells[row, allcolspan].Value = "";
                  row++;

                  foreach (var doc in exps.Where(w => w.Expenses_Config_ID == etype.Expenses_Config_ID))
                  {
                     decimal? amount = 0M;
                     amount = ((doc.Amount_Claiming.HasValue ? doc.Amount_Claiming.Value : 0) + (doc.Withholding_Tax_Amount.HasValue ? doc.Withholding_Tax_Amount.Value : 0)) - (doc.Tax_Amount.HasValue ? doc.Tax_Amount.Value : 0);

                     var claimpercen = (amount / job.Sell_Price) * 100;
                     totalAllCosting += (decimal)amount;
                     totalAllCostingPercen += claimpercen.HasValue ? claimpercen.Value : 0;

                     /***** Expenses document *******/
                     ws.Cells[row, col, row, allcolspan].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                     for (int c = 1; c <= allcolspan; c++)
                        ws.Cells[row, c].Style.Border.BorderAround(ExcelBorderStyle.Thin);

                     ws.Cells[row, 1].Value = "        " + doc.Expenses_Application.Expenses_Title;
                     ws.Cells[row, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                     ws.Cells[row, 2].Value = "";
                     ws.Cells[row, 3].Value = "";
                     ws.Cells[row, 4].Value = "";
                     ws.Cells[row, 5].Value = "";
                     ws.Cells[row, 6].Value = NumUtil.FormatCurrencyExcel(amount);
                     ws.Cells[row, 7].Value = NumUtil.FormatPercenExcel(claimpercen);
                     ws.Cells[row, 8].Value = doc.Doc_No;
                     ws.Cells[row, 9].Value = DateUtil.ToDisplayDate(doc.Expenses_Date);
                     ws.Cells[row, 10].Value = NumUtil.FormatCurrencyExcel(amount);

                     monthindex = 0;
                     n = 1;
                     for (int year = minyear; year <= maxyear; year++)
                     {
                        for (int k = 1; k <= 12; k++)
                        {
                           if (doc.Expenses_Date.HasValue && year == doc.Expenses_Date.Value.Year && k == doc.Expenses_Date.Value.Month)
                           {
                              ws.Cells[row, leftcolspan + n].Value = NumUtil.FormatCurrencyExcel(amount);
                              totalPermonths[monthindex] += (decimal)amount;
                           }
                           else
                              ws.Cells[row, leftcolspan + n].Value = "";

                           n++;
                           monthindex++;
                        }
                     }

                     ws.Cells[row, allcolspan].Value = NumUtil.FormatCurrencyExcel(amount);
                     row++;
                  }
               }
            }

            /***** Expenses Category Detail *******/
            ws.Cells[row, col, row, allcolspan].Style.Fill.PatternType = ExcelFillStyle.Solid;
            ws.Cells[row, col, row, allcolspan].Style.Fill.BackgroundColor.SetColor(Color.SkyBlue);
            ws.Cells[row, col, row, allcolspan].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
            ws.Cells[row, col, row, allcolspan].Style.Font.Bold = true;
            for (int c = 1; c <= allcolspan; c++)
               ws.Cells[row, c].Style.Border.BorderAround(ExcelBorderStyle.Thin);

            ws.Cells[row, 1].Value = Resource.Salary;
            ws.Cells[row, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
            ws.Cells[row, 2].Value = "";
            ws.Cells[row, 3].Value = "";
            ws.Cells[row, 4].Value = "";
            ws.Cells[row, 5].Value = "";
            ws.Cells[row, 6].Value = "";
            ws.Cells[row, 7].Value = "";
            ws.Cells[row, 8].Value = "";
            ws.Cells[row, 9].Value = "";
            ws.Cells[row, 10].Value = "";

            n = 1;
            for (int year = minyear; year <= maxyear; year++)
               for (int k = 1; k <= 12; k++)
               {
                  ws.Cells[row, leftcolspan + n].Value = "";
                  n++;
               }

            ws.Cells[row, allcolspan].Value = "";
            row++;
            var eService = new EmployeeService();
            var empIDs = timesheets.OrderBy(o => o.Employee_Profile_ID).Select(s => s.Employee_Profile_ID).Distinct();
            foreach (var empID in empIDs)
            {
               var emp = eService.GetEmployeeProfile2(empID);
               if (emp == null)
                  continue;

               ws.Cells[row, col, row, allcolspan].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
               for (int c = 1; c <= allcolspan; c++)
                  ws.Cells[row, c].Style.Border.BorderAround(ExcelBorderStyle.Thin);

               var emptimesheets = timesheets.Where(w => w.Employee_Profile_ID == empID);
               var totalamt = emptimesheets.Select(s => s.Total_Amount.HasValue ? s.Total_Amount.Value : 0).Sum();
               var claimpercen = (totalamt / job.Sell_Price) * 100;
               totalAllCosting += totalamt;
               totalAllCostingPercen += claimpercen.HasValue ? claimpercen.Value : 0;

               ws.Cells[row, 1].Value = "    " + AppConst.GetUserName(emp.User_Profile);
               ws.Cells[row, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
               ws.Cells[row, 2].Value = "";
               ws.Cells[row, 3].Value = "";
               ws.Cells[row, 4].Value = "";
               ws.Cells[row, 5].Value = "";
               ws.Cells[row, 6].Value = NumUtil.FormatCurrencyExcel(totalamt);
               ws.Cells[row, 7].Value = NumUtil.FormatPercenExcel(claimpercen);
               ws.Cells[row, 8].Value = "";
               ws.Cells[row, 9].Value = "";
               ws.Cells[row, 10].Value = NumUtil.FormatCurrencyExcel(totalamt);

               n = 1;
               monthindex = 0;
               for (int year = minyear; year <= maxyear; year++)
               {
                  for (int k = 1; k <= 12; k++)
                  {
                     var totalmonthamt = 0M;
                     foreach (var time in emptimesheets)
                     {
                        var date = DateUtil.ToDate(time.Date_Of_Date);
                        if (date.HasValue && year == date.Value.Year && k == date.Value.Month)
                        {
                           totalmonthamt += time.Total_Amount.HasValue ? time.Total_Amount.Value : 0;
                        }
                     }

                     if (totalmonthamt > 0)
                     {

                        ws.Cells[row, leftcolspan + n].Value = NumUtil.FormatCurrencyExcel(totalmonthamt);
                        totalPermonths[monthindex] += totalmonthamt;

                     }
                     else
                        ws.Cells[row, leftcolspan + n].Value = "";

                     n++;
                     monthindex++;
                  }
               }

               ws.Cells[row, allcolspan].Value = NumUtil.FormatCurrencyExcel(totalamt);
               row++;
            }
            //------------------------- End Expense----------------------------//


            //------------------------- Start Footer----------------------------//
            /***** Total Cost*******/
            ws.Cells[row, col, row, allcolspan].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
            ws.Cells[row, col, row, allcolspan].Style.Fill.PatternType = ExcelFillStyle.Solid;
            ws.Cells[row, col, row, allcolspan].Style.Fill.BackgroundColor.SetColor(Color.LightGray);
            ws.Cells[row, col, row, allcolspan].Style.Font.Bold = true;
            for (int c = 1; c <= allcolspan; c++)
               ws.Cells[row, c].Style.Border.BorderAround(ExcelBorderStyle.Thin);

            ws.Cells[row, 1].Value = Resource.Total_Cost;
            ws.Cells[row, 2].Value = NumUtil.FormatCurrencyExcel(totalAllBudget);
            ws.Cells[row, 3].Value = NumUtil.FormatPercenExcel(totalAllBudgetPercen);
            ws.Cells[row, 4].Value = NumUtil.FormatCurrencyExcel(totalAllBudget);
            ws.Cells[row, 5].Value = NumUtil.FormatPercenExcel(totalAllBudgetPercen);
            ws.Cells[row, 6].Value = NumUtil.FormatCurrencyExcel(totalAllCosting);
            ws.Cells[row, 7].Value = NumUtil.FormatPercenExcel(totalAllCostingPercen);
            ws.Cells[row, 8].Value = "";
            ws.Cells[row, 9].Value = Resource.Total;
            ws.Cells[row, 10].Value = NumUtil.FormatCurrencyExcel(totalAllCosting);

            var total = 0M;
            monthindex = 0;
            n = 1;
            for (int year = minyear; year <= maxyear; year++)
            {
               for (int k = 1; k <= 12; k++)
               {
                  ws.Cells[row, leftcolspan + n].Value = NumUtil.FormatCurrencyExcel(totalPermonths[monthindex]);
                  total += totalPermonths[monthindex];
                  monthindex++;
                  n++;
               }
            }


            ws.Cells[row, allcolspan].Value = NumUtil.FormatCurrencyExcel(total);
            row++;

            /***** Gross Profit *******/
            ws.Cells[row, col, row, allcolspan].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
            ws.Cells[row, col, row, allcolspan].Style.Fill.PatternType = ExcelFillStyle.Solid;
            ws.Cells[row, col, row, allcolspan].Style.Fill.BackgroundColor.SetColor(Color.LightGray);
            ws.Cells[row, col, row, allcolspan].Style.Font.Bold = true;
            for (int c = 1; c <= allcolspan; c++)
               ws.Cells[row, c].Style.Border.BorderAround(ExcelBorderStyle.Thin);

            ws.Cells[row, 1].Value = Resource.Gross_Profit;
            ws.Cells[row, 2].Value = NumUtil.FormatCurrencyExcel(job.Sell_Price - totalAllBudget);
            ws.Cells[row, 3].Value = NumUtil.FormatPercenExcel(100 - totalAllBudgetPercen);
            ws.Cells[row, 4].Value = NumUtil.FormatCurrencyExcel(job.Sell_Price - totalAllBudget);
            ws.Cells[row, 5].Value = NumUtil.FormatPercenExcel(100 - totalAllBudgetPercen);
            ws.Cells[row, 6].Value = NumUtil.FormatCurrencyExcel(totalAmountActual - totalAllCosting);
            ws.Cells[row, 7].Value = NumUtil.FormatPercenExcel(100 - totalAllCostingPercen);
            ws.Cells[row, 8].Value = "";
            ws.Cells[row, 9].Value = "";
            ws.Cells[row, 10].Value = NumUtil.FormatCurrencyExcel(totalAmountActual - totalAllCosting);

            n = 1;
            for (int year = minyear; year <= maxyear; year++)
               for (int k = 1; k <= 12; k++)
               {
                  ws.Cells[row, leftcolspan + n].Value = "";
                  n++;
               }

            ws.Cells[row, allcolspan].Value = NumUtil.FormatCurrencyExcel(total);
            row++;

            /***** Gross Profit Percen *******/
            ws.Cells[row, col, row, allcolspan].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
            ws.Cells[row, col, row, allcolspan].Style.Fill.PatternType = ExcelFillStyle.Solid;
            ws.Cells[row, col, row, allcolspan].Style.Fill.BackgroundColor.SetColor(Color.LightGray);
            ws.Cells[row, col, row, allcolspan].Style.Font.Color.SetColor(Color.Red);
            ws.Cells[row, col, row, allcolspan].Style.Font.Bold = true;
            for (int c = 1; c <= allcolspan; c++)
               ws.Cells[row, c].Style.Border.BorderAround(ExcelBorderStyle.Thin);

            ws.Cells[row, 1].Value = Resource.Gross_Profit_Percen;
            ws.Cells[row, 2].Value = "";
            ws.Cells[row, 3].Value = NumUtil.FormatPercenExcel(100 - totalAllBudgetPercen);
            ws.Cells[row, 4].Value = "";
            ws.Cells[row, 5].Value = NumUtil.FormatPercenExcel(100 - totalAllBudgetPercen);
            ws.Cells[row, 6].Value = "";
            ws.Cells[row, 7].Value = NumUtil.FormatPercenExcel(100 - totalAllCostingPercen);
            ws.Cells[row, 8].Value = "";
            ws.Cells[row, 9].Value = "";
            ws.Cells[row, 10].Value = "";

            n = 1;
            for (int year = minyear; year <= maxyear; year++)
               for (int k = 1; k <= 12; k++)
               {
                  ws.Cells[row, leftcolspan + n].Value = "";
                  n++;
               }

            ws.Cells[row, allcolspan].Value = NumUtil.FormatCurrencyExcel(0);
            row++;


            ws.Column(1).Width = 60;
            for (var c = 2; c <= allcolspan; c++)
               ws.Column(c).Width = 15;
            pck.SaveAs(Response.OutputStream);

            var regex = new Regex(Regex.Escape(","));
            filename = regex.Replace(filename, "_");
            Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            Response.AddHeader("content-disposition", "attachment;  filename=" + filename + ".xlsx");



         }
         return csv.ToString();
      }



   }
}