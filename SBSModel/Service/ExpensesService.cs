using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Web;
using SBSModel.Models;
using SBSModel.Common;
using System.Data.Entity.Validation;
using SBSResourceAPI;
using SBSWorkFlowAPI.Constants;
using System.Data.Entity.SqlServer;

namespace SBSModel.Models
{

   public class ExpenseCriteria : CriteriaBase
   {
      public Nullable<int> Expenses_Config_ID { get; set; }
      public Nullable<int> Department_ID { get; set; }
      public Nullable<int> Year { get; set; }
      public string Date_From { get; set; }
      public string Date_To { get; set; }
      public List<int> Expenses_Type_Sel { get; set; }
      public int? Employee_Profile_ID { get; set; }

      public bool Closed_Status { get; set; }
      public string Date_Applied { get; set; }
      public string Overall_Status { get; set; }
      public Nullable<int> Expenses_Category_ID { get; set; }
      public Nullable<int> Job_Cost_ID { get; set; }


      public Nullable<int> Month { get; set; }
      public bool Include_Draft { get; set; }

      public Nullable<int> Supervisor { get; set; }


      public bool Include_Extra { get; set; }
      public bool Tab_Processed { get; set; }
      public bool Tab_Pending { get; set; }

      public Nullable<int> Request_Profile_ID { get; set; }






   }

   public class ExpensesCategoryCriteria : CriteriaBase
   {
      public Nullable<int> Expenses_Category_ID { get; set; }
      public string Category_Name { get; set; }

   }

   public class ExpensesTypeCriteria : CriteriaBase
   {
      public Nullable<int> Expenses_Config_ID { get; set; }
      public Nullable<int> Expenses_Category_ID { get; set; }

   }

   public class ExpenseService
   {

      #region Expenses Category // Edit By sun 29-06-2016
      public ServiceResult InsertExpensesCategory(Expenses_Category ExpensesCategory)
      {
         try
         {
            using (var db = new SBS2DBContext())
            {
               db.Expenses_Category.Add(ExpensesCategory);
               db.SaveChanges();
               return new ServiceResult() { Code = ERROR_CODE.SUCCESS, Msg = new Success().getSuccess(ERROR_CODE.SUCCESS_CREATE), Field = Resource.Expenses_Category };
            }

         }
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
            return new ServiceResult() { Code = ERROR_CODE.ERROR_503_INSERT_ERROR, Msg = new Error().getError(ERROR_CODE.ERROR_503_INSERT_ERROR), Field = Resource.Expenses_Category };
         }


      }

      public ServiceResult UpdateExpensesCategory(Expenses_Category pExCategory)
      {
         try
         {
            using (var db = new SBS2DBContext())
            {
               var current = db.Expenses_Category.Where(w => w.Expenses_Category_ID == pExCategory.Expenses_Category_ID).FirstOrDefault();
               if (current == null)
                  return new ServiceResult { Code = ERROR_CODE.ERROR_511_DATA_NOT_FOUND, Msg = Resource.Expenses_Category + " " + Resource.Not_Found_Msg, Field = Resource.Expenses_Category };

               db.Entry(current).CurrentValues.SetValues(pExCategory);
               db.SaveChanges();
               return new ServiceResult() { Code = ERROR_CODE.SUCCESS, Msg = new Success().getSuccess(ERROR_CODE.SUCCESS_EDIT), Field = Resource.Expenses_Category };
            }
         }
         catch
         {
            return new ServiceResult() { Code = ERROR_CODE.ERROR_504_UPDATE_ERROR, Msg = new Error().getError(ERROR_CODE.ERROR_504_UPDATE_ERROR), Field = Resource.Expenses_Category };
         }
      }

      public List<Expenses_Category> LstExpensesCategory(ExpensesCategoryCriteria criteria)
      {
         using (var db = new SBS2DBContext())
         {
            var exc = db.Expenses_Category.Where(w => w.Company_ID == criteria.Company_ID && w.Record_Status != RecordStatus.Delete);
            if (criteria != null)
            {
               if (criteria.Expenses_Category_ID.HasValue && criteria.Expenses_Category_ID.Value > 0)
                  exc = exc.Where(w => w.Expenses_Category_ID == criteria.Expenses_Category_ID);

               if (!string.IsNullOrEmpty(criteria.Category_Name))
                  exc = exc.Where(w => w.Category_Name == criteria.Category_Name);
            }
            return exc.OrderBy(o => o.Category_Name).ToList();
         }
      }

      public Expenses_Category GetExpenseCategory(Nullable<int> pExCategoryID)
      {
         Expenses_Category expenseCategory = null;
         try
         {
            using (var db = new SBS2DBContext())
            {
               expenseCategory = db.Expenses_Category
                   .Include(i => i.Expenses_Config)
                   .Where(w => w.Expenses_Category_ID == pExCategoryID && w.Record_Status != RecordStatus.Delete).FirstOrDefault();
            }
         }
         catch
         {
         }
         return expenseCategory;
      }
      #endregion

      #region Expenses Config
      public List<Expenses_Config> LstExpenseTypes(ExpensesTypeCriteria criteria)
      {
         using (var db = new SBS2DBContext())
         {
            var exc = db.Expenses_Config.Where(w => w.Company_ID == criteria.Company_ID && w.Record_Status != RecordStatus.Delete);
            if (criteria != null)
            {
               if (criteria.Expenses_Category_ID.HasValue)
                  exc = exc.Where(w => w.Expenses_Category_ID == criteria.Expenses_Category_ID);
               if (criteria.Expenses_Config_ID.HasValue)
                  exc = exc.Where(w => w.Expenses_Config_ID == criteria.Expenses_Config_ID);
            }
            return exc.OrderBy(o => o.Expenses_Name).ToList();
         }
      }

      public List<Expenses_Config> getExpenseTypes(Nullable<int> pCompanyID, Nullable<int> pDepartmentID = null, Nullable<int> pDesignationID = null, string pExpenseName = "", Nullable<int> pYearService = null, Nullable<int> pExpensesCategoryID = null)
      {
         List<Expenses_Config> expense = null;
         try
         {
            using (var db = new SBS2DBContext())
            {
               expense = db.Expenses_Config
                   .Include(i => i.Department)
                   .Include(i => i.Expenses_Config_Detail)
                   .Include(i => i.Global_Lookup_Data)
                   .Where(w => w.Company_ID == pCompanyID && w.Record_Status != RecordStatus.Delete).ToList();

               if (expense != null)
               {
                  //Added By sun 26-08-2015
                  if (pExpensesCategoryID.HasValue && pExpensesCategoryID.Value > 0)
                  {
                     expense = expense.Where(w => w.Expenses_Category_ID == pExpensesCategoryID).ToList();
                  }

                  if (pDepartmentID.HasValue && pDepartmentID.Value > 0)
                  {
                     expense = expense.Where(w => w.Department_ID == pDepartmentID || w.Department_ID == null).ToList();
                  }

                  if (pDesignationID.HasValue && pDesignationID.Value > 0)
                  {
                     expense = expense.Where(w => w.Expenses_Config_Detail.Any(x => x.Designation_ID == pDesignationID || x.Designation_ID == null) || w.Expenses_Config_Detail.Count() == 0).ToList();
                  }

                  //Edit By sun 27-08-2015
                  //if (pYearService.HasValue)
                  if (pYearService.HasValue && pYearService.Value > 0)
                  {
                     expense = expense.Where(w => w.Expenses_Config_Detail.Any(x => x.Year_Service <= pYearService) || w.Expenses_Config_Detail.Count() == 0).ToList();
                  }
                  if (!string.IsNullOrEmpty(pExpenseName))
                  {
                     expense = expense.Where(w => w.Expenses_Name.Contains(pExpenseName)).ToList();
                  }

                  return expense;
               }
            }
         }
         catch
         {
         }

         return expense;
      }

      public Expenses_Config GetExpenseType(Nullable<int> pExConfigID)
      {
         Expenses_Config expense = null;
         try
         {
            using (var db = new SBS2DBContext())
            {
               expense = db.Expenses_Config
                   .Include(i => i.Department)
                   .Include(i => i.Global_Lookup_Data) // UOM
                   .Include(i => i.Expenses_Config_Detail)
                   .Where(w => w.Expenses_Config_ID == pExConfigID && w.Record_Status != RecordStatus.Delete).FirstOrDefault();
            }
         }
         catch
         {
         }
         return expense;
      }

      public ServiceResult InsertExpenseType(Expenses_Config expense, Expenses_Type_Detail[] details)
      {

         try
         {
            using (var db = new SBS2DBContext())
            {
               if (details != null)
               {
                  var i = 0;
                  var gcnt = 1;
                  foreach (var row in details)
                  {
                     if (row.Row_Type == RowType.ADD)
                     {
                        foreach (var ldrow in row.Designations)
                        {
                           Nullable<int> designationID = ldrow;
                           if (ldrow == 0)
                              designationID = null;

                           var eConfigDetail = new Expenses_Config_Detail()
                           {
                              Amount_Per_Year = row.Amount_Per_Year,
                              Designation_ID = designationID,
                              Group_ID = gcnt,
                              Year_Service = row.Year_Service
                           };

                           if (row.Select_Amount)
                           {
                              eConfigDetail.Select_Amount = true;
                              eConfigDetail.Select_Pecentage = false;
                              eConfigDetail.Amount = row.Amount;
                           }
                           else
                           {
                              eConfigDetail.Select_Amount = false;
                              eConfigDetail.Select_Pecentage = true;
                              eConfigDetail.Pecentage = row.Pecentage;
                           }

                           expense.Expenses_Config_Detail.Add(eConfigDetail);


                        }


                     }
                     i++;
                     gcnt++;
                  }
               }

               db.Entry(expense).State = EntityState.Added;
               db.SaveChanges();
               return new ServiceResult() { Code = ERROR_CODE.SUCCESS, Msg = new Success().getSuccess(ERROR_CODE.SUCCESS_CREATE), Field = Resource.Expenses_Type };
            }
         }
         catch
         {
            return new ServiceResult() { Code = ERROR_CODE.ERROR_503_INSERT_ERROR, Msg = new Error().getError(ERROR_CODE.ERROR_503_INSERT_ERROR), Field = Resource.Expenses_Type };
         }
      }

      public ServiceResult UpdateExpenseType(Expenses_Config expense, Expenses_Type_Detail[] details)
      {

         try
         {
            using (var db = new SBS2DBContext())
            {
               var current = db.Expenses_Config.Where(w => w.Expenses_Config_ID == expense.Expenses_Config_ID).FirstOrDefault();
               if (current != null)
               {
                  if (details != null)
                  {
                     var i = 0;
                     var gcnt = 1;
                     foreach (var row in details)
                     {
                        row.Expenses_Config_ID = expense.Expenses_Config_ID;
                        if (row.Row_Type == RowType.ADD)
                        {

                           foreach (var ldrow in row.Designations)
                           {
                              Nullable<int> designationID = ldrow;
                              if (ldrow == 0)
                                 designationID = null;

                              var eConfigDetail = new Expenses_Config_Detail()
                              {
                                 Select_Per_Month = row.Select_Per_Month,
                                 Amount_Per_Month = row.Amount_Per_Month,
                                 Amount_Per_Year = row.Amount_Per_Year,
                                 Designation_ID = designationID,
                                 Group_ID = gcnt,
                                 Expenses_Config_ID = row.Expenses_Config_ID,
                                 Year_Service = row.Year_Service
                              };

                              if (row.Select_Amount)
                              {
                                 eConfigDetail.Select_Amount = true;
                                 eConfigDetail.Select_Pecentage = false;
                                 eConfigDetail.Amount = row.Amount;
                                 eConfigDetail.Pecentage = 0;
                              }
                              else
                              {
                                 eConfigDetail.Select_Amount = false;
                                 eConfigDetail.Select_Pecentage = true;
                                 eConfigDetail.Pecentage = row.Pecentage;
                                 eConfigDetail.Amount = 0;
                              }

                              db.Expenses_Config_Detail.Add(eConfigDetail);
                           }
                           gcnt++;
                        }
                        else if (row.Row_Type == RowType.EDIT)
                        {
                           if (row.Designations != null)
                           {
                              foreach (var ldrow in row.Designations)
                              {

                                 Nullable<int> designationID = ldrow;
                                 if (ldrow == 0)
                                 {
                                    designationID = null;
                                 }

                                 var currentdetail = (from a in db.Expenses_Config_Detail
                                                      where a.Group_ID == row.Group_ID
                                                      && a.Expenses_Config_ID == row.Expenses_Config_ID
                                                      && a.Designation_ID == designationID
                                                      select a).FirstOrDefault();

                                 if (currentdetail == null)
                                 {

                                    var eConfigDetail = new Expenses_Config_Detail()
                                    {
                                       Select_Per_Month = row.Select_Per_Month,
                                       Amount_Per_Month = row.Amount_Per_Month,
                                       Amount_Per_Year = row.Amount_Per_Year,
                                       Designation_ID = designationID,
                                       Group_ID = gcnt,
                                       Expenses_Config_ID = row.Expenses_Config_ID,
                                       Year_Service = row.Year_Service
                                    };

                                    if (row.Select_Amount)
                                    {
                                       eConfigDetail.Select_Amount = true;
                                       eConfigDetail.Select_Pecentage = false;
                                       eConfigDetail.Amount = row.Amount;
                                       eConfigDetail.Pecentage = 0;
                                    }
                                    else
                                    {
                                       eConfigDetail.Select_Amount = false;
                                       eConfigDetail.Select_Pecentage = true;
                                       eConfigDetail.Pecentage = row.Pecentage;
                                       eConfigDetail.Amount = 0;
                                    }

                                    db.Expenses_Config_Detail.Add(eConfigDetail);

                                 }
                                 else
                                 {
                                    currentdetail.Select_Per_Month = row.Select_Per_Month;
                                    currentdetail.Amount_Per_Month = row.Amount_Per_Month;
                                    currentdetail.Amount_Per_Year = row.Amount_Per_Year;
                                    currentdetail.Year_Service = row.Year_Service;
                                    currentdetail.Designation_ID = designationID;
                                    currentdetail.Group_ID = gcnt;
                                    if (row.Select_Amount)
                                    {
                                       currentdetail.Amount = row.Amount;
                                       currentdetail.Select_Amount = true;
                                       currentdetail.Select_Pecentage = false;
                                       currentdetail.Pecentage = 0;
                                    }
                                    else
                                    {
                                       currentdetail.Pecentage = row.Pecentage;
                                       currentdetail.Select_Amount = false;
                                       currentdetail.Select_Pecentage = true;
                                       currentdetail.Amount = 0;
                                    }

                                 }
                              }

                              var notUseRows = (from a in db.Expenses_Config_Detail
                                                where a.Group_ID == row.Group_ID && a.Expenses_Config_ID == row.Expenses_Config_ID
                                                && !row.Designations.Contains(a.Designation_ID.HasValue ? a.Designation_ID.Value : 0)
                                                select a);
                              db.Expenses_Config_Detail.RemoveRange(notUseRows);



                           }
                           gcnt++;

                        }
                        else if (row.Row_Type == RowType.DELETE)
                        {
                           if (row.Designations != null)
                           {
                              foreach (var ldrow in row.Designations)
                              {
                                 Nullable<int> designationID = ldrow;
                                 if (ldrow == 0)
                                    designationID = null;

                                 var currentdetail = (from a in db.Expenses_Config_Detail
                                                      where a.Group_ID == row.Group_ID
                                                      && a.Expenses_Config_ID == row.Expenses_Config_ID
                                                      && a.Designation_ID == designationID
                                                      select a).FirstOrDefault();
                                 if (currentdetail != null)
                                 {
                                    db.Expenses_Config_Detail.Remove(currentdetail);
                                 }
                              }
                           }


                        }
                        i++;
                     }
                  }

                  db.Entry(current).CurrentValues.SetValues(expense);
                  db.SaveChanges();
               }

               return new ServiceResult() { Code = ERROR_CODE.SUCCESS, Msg = new Success().getSuccess(ERROR_CODE.SUCCESS_EDIT), Field = Resource.Expenses_Type };
            }
         }
         catch
         {
            return new ServiceResult() { Code = ERROR_CODE.ERROR_504_UPDATE_ERROR, Msg = new Error().getError(ERROR_CODE.ERROR_504_UPDATE_ERROR), Field = Resource.Expenses_Type };
         }
      }

      #endregion

      //Added by Nay on 14-Jul-2015 
      //Purpose : to check is there any refercens or not
      public bool chkExpenseAppDocumentUsed(Nullable<int> pExpenseID)
      {
         var chkProblem = false;
         try
         {
            using (var db = new SBS2DBContext())
            {
               var expense = (from a in db.Expenses_Application_Document where a.Expenses_Config_ID == pExpenseID select a).ToList();

               if (expense.Count > 0)
                  chkProblem = true;
            }
            return chkProblem;
         }
         catch
         {
            return true;
         }
      }

      public Expenses_Application_Document getExpenseDtl(Nullable<int> pDtlID)
      {
         using (var db = new SBS2DBContext())
         {
            return db.Expenses_Application_Document
                      .Include(i => i.Job_Cost)
                      .Include(i => i.Upload_Receipt)
                      .Include(i => i.Expenses_Config)
                       .Include(i => i.Expenses_Application)
                      .Where(w => w.Expenses_Application_Document_ID == pDtlID).FirstOrDefault();
         }
      }

      public List<Expenses_Application_Document> getDepartmentExpenseApplications(Nullable<int> Department_ID)
      {
         List<Expenses_Application_Document> expense = new List<Expenses_Application_Document>();
         try
         {
            using (var db = new SBS2DBContext())
            {
               expense = db.Expenses_Application_Document
                   .Include(i => i.Employee_Profile)
                   .Include(i => i.Employee_Profile.User_Profile)
                   .Include(i => i.Expenses_Config)
                    .Include(i => i.Expenses_Application)
                   .Where(w => w.Department_ID == Department_ID && w.Overall_Status != RecordStatus.Delete)
                   .ToList();
            }
         }
         catch
         {
         }
         return expense;
      }

      public Expenses_Application getExpenseApplication(Nullable<int> Expenses_Application_ID)
      {
         var expense = new Expenses_Application();
         try
         {
            using (var db = new SBS2DBContext())
            {
               expense = db.Expenses_Application
                   .Include(i => i.Employee_Profile)
                   .Include(i => i.Employee_Profile.User_Profile)
                   .Include(i => i.Employee_Profile.User_Profile.User_Authentication)
                   .Include(i => i.Expenses_Application_Document)
                   .Include(i => i.Expenses_Application_Document.Select(s => s.Expenses_Config))
                   .Include(i => i.Expenses_Application_Document.Select(s => s.Upload_Receipt))
                   .Include(i => i.Expenses_Application_Document.Select(s => s.Job_Cost))
                   .Where(w => w.Expenses_Application_ID == Expenses_Application_ID && w.Cancel_Status != RecordStatus.Delete).FirstOrDefault();
            }
         }
         catch
         {
         }
         return expense;
      }

      //public List<Expenses_Application> getExpenseApplications(Nullable<int> pCompanyID, Nullable<int> pProfileID = null, string pDateApplied = "", string pStatus = "", Nullable<bool> IncludeDraft = false, Nullable<int> pEmpID = null, int? pMonth = null, int? pYear = null)
      //{
      //   try
      //   {
      //      using (var db = new SBS2DBContext())
      //      {
      //         var expenses = db.Expenses_Application
      //                 .Include(i => i.TsEXes)
      //                 .Include(i => i.Employee_Profile)
      //                 .Include(i => i.Employee_Profile.User_Profile)
      //                 .Include(i => i.Expenses_Application_Document)
      //                 .Include(i => i.Expenses_Application_Document.Select(s => s.Expenses_Config))
      //                 .Where(w => w.Employee_Profile.User_Profile.Company_ID == pCompanyID && w.Overall_Status != RecordStatus.Delete);


      //         if (pMonth.HasValue)
      //         {
      //            expenses = expenses.Where(w => w.Expenses_Application_Document.Where(w2 => w2.Expenses_Date.Value.Month == pMonth).FirstOrDefault() != null);
      //         }
      //         if (pYear.HasValue)
      //         {
      //            expenses = expenses.Where(w => w.Expenses_Application_Document.Where(w2 => w2.Expenses_Date.Value.Year == pYear).FirstOrDefault() != null);
      //         }
      //         if (!string.IsNullOrEmpty(pDateApplied))
      //         {
      //            var d = DateUtil.ToDate(pDateApplied);
      //            if (d != null)
      //            {
      //               expenses = expenses.Where(w => w.Date_Applied.Value.Day == d.Value.Day & w.Date_Applied.Value.Month == d.Value.Month & w.Date_Applied.Value.Year == d.Value.Year);
      //            }

      //         }
      //         if (pProfileID.HasValue)
      //         {
      //            expenses = expenses.Where(w => w.Employee_Profile.Profile_ID == pProfileID);
      //         }
      //         if (pEmpID.HasValue)
      //         {
      //            expenses = expenses.Where(w => w.Employee_Profile_ID == pEmpID);
      //         }
      //         if (!string.IsNullOrEmpty(pStatus))
      //         {
      //            if (pStatus == "Pending")
      //            {
      //               expenses = expenses.Where(w => w.Overall_Status == SBSWorkFlowAPI.Constants.WorkflowStatus.Pending | w.Overall_Status == SBSWorkFlowAPI.Constants.WorkflowStatus.Approved);
      //            }
      //            else
      //            {
      //               expenses = expenses.Where(w => w.Overall_Status == pStatus);
      //            }
      //         }

      //         #region Start Funtion Draft
      //         if (IncludeDraft.HasValue && IncludeDraft.Value)
      //         {

      //         }
      //         else
      //         {
      //            expenses = expenses.Where(w => w.Overall_Status != WorkflowStatus.Draft);
      //         }
      //         #endregion
      //         return expenses.OrderBy(o => o.Employee_Profile_ID).OrderByDescending(o => o.Date_Applied).ThenByDescending(o => o.Expenses_Application_ID).ToList();
      //      }
      //   }
      //   catch
      //   {
      //      return new List<Expenses_Application>();
      //   }
      //}

      public List<Expenses_Application_Document> getExpenseApplicationDocs(Nullable<int> pProfileID, string pDateApplied = "", string pStatus = "")
      {
         try
         {
            using (var db = new SBS2DBContext())
            {
               var expenses = db.Expenses_Application_Document
                       .Include(i => i.Employee_Profile)
                       .Include(i => i.Employee_Profile.User_Profile)
                       .Include(i => i.Currency)
                       .Include(i => i.Expenses_Config)
                       .Include(i => i.Expenses_Application)
                                            .Where(w => w.Employee_Profile.Profile_ID == pProfileID && w.Overall_Status != RecordStatus.Delete);

               if (!string.IsNullOrEmpty(pDateApplied))
               {
                  var d = DateUtil.ToDate(pDateApplied);
                  if (d != null)
                  {
                     expenses = expenses.Where(w => w.Expenses_Application.Date_Applied.Value.Day == d.Value.Day & w.Expenses_Application.Date_Applied.Value.Month == d.Value.Month & w.Expenses_Application.Date_Applied.Value.Year == d.Value.Year);
                  }

               }
               if (!string.IsNullOrEmpty(pStatus))
               {
                  expenses = expenses.Where(w => w.Expenses_Application.Overall_Status == pStatus);
               }
               return expenses.ToList();
            }
         }
         catch
         {
            return new List<Expenses_Application_Document>();
         }
      }

      public Upload_Receipt GetReceiptUpload(Nullable<Guid> pUploadID)
      {
         using (var db = new SBS2DBContext())
         {
            return db.Upload_Receipt.Where(w => w.Upload_Receipt_ID == pUploadID).FirstOrDefault();
         }

      }

      public ServiceResult insertExpenseApplication(Expenses_Application expense, List<Expenses_Detail> details)
      {
         try
         {
            var currentdate = StoredProcedure.GetCurrentDate();
            using (var db = new SBS2DBContext())
            {
               var emp = db.Employee_Profile.Where(w => w.Employee_Profile_ID == expense.Employee_Profile_ID).FirstOrDefault();
               if (emp == null)
                  return new ServiceResult() { Code = ERROR_CODE.ERROR_511_DATA_NOT_FOUND, Msg = new Error().getError(ERROR_CODE.ERROR_511_DATA_NOT_FOUND), Field = Resource.Employee };

               var userhist = db.Employment_History
                 .Include(i => i.Department)
                 .Include(i => i.Designation)
                 .Include(i => i.Employee_Profile)
                 .Where(w => w.Employee_Profile_ID == emp.Employee_Profile_ID & w.Effective_Date <= currentdate)
                 .OrderByDescending(o => o.Effective_Date)
                 .FirstOrDefault();

               if (userhist == null)
                  return new ServiceResult() { Code = ERROR_CODE.ERROR_511_DATA_NOT_FOUND, Msg = new Error().getError(ERROR_CODE.ERROR_511_DATA_NOT_FOUND), Field = Resource.Employment_History };

               if (details != null)
               {
                  foreach (var row in details)
                  {
                     if (row.Row_Type == RowType.ADD)
                     {
                        var expensesDetail = new Expenses_Application_Document()
                        {
                           Amount_Claiming = row.Amount_Claiming,
                           Employee_Profile_ID = expense.Employee_Profile_ID,
                           Expenses_Config_ID = row.Expenses_Config_ID,
                           Expenses_Date = DateUtil.ToDate(row.Expenses_Date),
                           Doc_No = row.Doc_No,
                           Reasons = row.Notes,
                           Selected_Currency = row.Selected_Currency,
                           Tax = row.Tax,
                           Total_Amount = row.Total_Amount,
                           Department_ID = userhist.Department_ID,
                           Date_Applied = expense.Date_Applied,
                           Mileage = row.Mileage,
                           Create_By = expense.Create_By,
                           Create_On = currentdate,
                           Update_By = expense.Create_By,
                           Update_On = currentdate,
                           //********  Smart Dev  ********//
                           Job_Cost_ID = row.Job_Cost_ID,
                           Withholding_Tax = row.Withholding_Tax,
                           Tax_Type = row.Tax_Type,
                           Withholding_Tax_Amount = row.Withholding_Tax_Amount,
                           Tax_Amount = row.Tax_Amount,
                           Tax_Amount_Type = row.Tax_Amount_Type,
                           Withholding_Tax_Type = row.Withholding_Tax_Type,
                        };

                        if (!string.IsNullOrEmpty(row.Upload_Receipt))
                        {
                           var guid = Guid.NewGuid();
                           while (db.Upload_Receipt.Where(w => w.Upload_Receipt_ID == guid).FirstOrDefault() != null)
                              guid = Guid.NewGuid();

                           expensesDetail.Upload_Receipt.Add(new Upload_Receipt()
                           {
                              File_Name = row.Upload_Receipt_Name,
                              Receipt = Convert.FromBase64String(row.Upload_Receipt),
                              Create_By = expensesDetail.Create_By,
                              Create_On = currentdate,
                              Update_By = expensesDetail.Update_By,
                              Update_On = currentdate,
                              Upload_Receipt_ID = guid,
                           });
                        }
                        expense.Expenses_Application_Document.Add(expensesDetail);
                     }
                  }
               }

               var eNo = "";
               var pattern = db.Expenses_No_Pattern.Where(w => w.Company_ID == emp.User_Profile.Company_ID).FirstOrDefault();
               if (pattern == null)
               {
                  var newpattern = new Expenses_No_Pattern()
                  {
                     Company_ID = emp.User_Profile.Company_ID,
                     Current_Running_Number = 1
                  };
                  db.Expenses_No_Pattern.Add(newpattern);
                  eNo = "EX-" + currentdate.Year + "-" + 1.ToString("0000");
               }
               else
               {
                  pattern.Current_Running_Number = pattern.Current_Running_Number + 1;
                  eNo = "EX-" + currentdate.Year + "-" + pattern.Current_Running_Number.Value.ToString("0000");
               }
               expense.Expenses_No = eNo;

               db.Entry(expense).State = EntityState.Added;
               db.SaveChanges();
               db.Entry(expense).GetDatabaseValues();

               return new ServiceResult() { Code = ERROR_CODE.SUCCESS, Msg = new Success().getSuccess(ERROR_CODE.SUCCESS_CREATE), Field = Resource.Expenses };
            }
         }
         catch
         {
         }
         return new ServiceResult() { Code = ERROR_CODE.ERROR_503_INSERT_ERROR, Msg = new Error().getError(ERROR_CODE.ERROR_503_INSERT_ERROR), Field = Resource.Expenses };
      }

      public ServiceResult updateExpenseApplication(Expenses_Application pExpense)
      {
         try
         {
            using (var db = new SBS2DBContext())
            {
               var current = db.Expenses_Application.Where(w => w.Expenses_Application_ID == pExpense.Expenses_Application_ID).FirstOrDefault();
               if (current != null)
               {
                  //********  Start Smart Dev  ********//
                  if (pExpense.Overall_Status == WorkflowStatus.Closed && (pExpense.Cancel_Status == WorkflowStatus.Cancelled || pExpense.Cancel_Status == null))
                  {
                     //********  Calulate Job Cost  ********//
                     calulateJobCostByExpenseApplication(db, pExpense);
                  }
                  //********  End Smart Dev  ********//
                  db.Entry(current).CurrentValues.SetValues(pExpense);
                  //db.Entry(expense).State = EntityState.Modified;
                  db.SaveChanges();
               }
               return new ServiceResult() { Code = ERROR_CODE.SUCCESS, Msg = new Success().getSuccess(ERROR_CODE.SUCCESS_EDIT), Field = Resource.Expenses };
            }
         }
         catch
         {
            return new ServiceResult() { Code = ERROR_CODE.ERROR_506_SAVE_ERROR, Msg = new Error().getError(ERROR_CODE.ERROR_506_SAVE_ERROR), Field = Resource.Expenses };
         }
      }

      public ServiceResult updateExpenseApplicationDoc(Expenses_Application expense)
      {
         try
         {
            var currentdate = StoredProcedure.GetCurrentDate();
            using (var db = new SBS2DBContext())
            {
               var current = db.Expenses_Application.Where(w => w.Expenses_Application_ID == expense.Expenses_Application_ID).FirstOrDefault();
               if (current != null)
               {
                  if (expense.Expenses_Application_Document == null)
                  {
                     foreach (var app_doc in current.Expenses_Application_Document)
                     {
                        var current_app_doc = db.Expenses_Application_Document.Where(w => w.Expenses_Application_Document_ID == app_doc.Expenses_Application_Document_ID).FirstOrDefault();
                        if (current_app_doc != null)
                        {
                           var current_upload_receipt = db.Upload_Receipt.Where(w => w.Expenses_Application_Document_ID == current_app_doc.Expenses_Application_Document_ID);
                           if (current_upload_receipt != null && current_upload_receipt.Count() > 0)
                              db.Upload_Receipt.RemoveRange(current_upload_receipt);

                           db.Expenses_Application_Document.Remove(current_app_doc);
                        }
                     }
                  }
                  else
                  {
                     var current_app_doc_ids = current.Expenses_Application_Document.Select(s => s.Expenses_Application_Document_ID).ToArray();
                     foreach (var current_app_doc_id in current_app_doc_ids)
                     {
                        if (!expense.Expenses_Application_Document.Select(s => s.Expenses_Application_Document_ID).Contains(current_app_doc_id))
                        {
                           var current_app_doc = db.Expenses_Application_Document.Where(w => w.Expenses_Application_Document_ID == current_app_doc_id).FirstOrDefault();
                           if (current_app_doc != null)
                           {
                              var upload_receipt = db.Upload_Receipt.Where(w => w.Expenses_Application_Document_ID == current_app_doc.Expenses_Application_Document_ID);
                              if (upload_receipt != null && upload_receipt.Count() > 0)
                                 db.Upload_Receipt.RemoveRange(upload_receipt);

                              db.Expenses_Application_Document.Remove(current_app_doc);
                           }
                        }
                     }
                     foreach (var app_doc in expense.Expenses_Application_Document)
                     {
                        if (app_doc.Expenses_Application_Document_ID == 0)
                        {
                           if (app_doc.Upload_Receipt != null && app_doc.Upload_Receipt.FirstOrDefault() != null)
                           {
                              var upload_receipt = app_doc.Upload_Receipt.FirstOrDefault();
                              var guid = Guid.NewGuid();
                              while (db.Upload_Receipt.Where(w => w.Upload_Receipt_ID == guid).FirstOrDefault() != null)
                                 guid = Guid.NewGuid();

                              upload_receipt.Upload_Receipt_ID = guid;
                              db.Upload_Receipt.Add(upload_receipt);
                           }
                           db.Expenses_Application_Document.Add(app_doc);
                        }
                        else
                        {
                           var current_app_doc = db.Expenses_Application_Document.Where(w => w.Expenses_Application_Document_ID == app_doc.Expenses_Application_Document_ID).FirstOrDefault();
                           if (current_app_doc != null)
                           {
                              app_doc.Create_By = current_app_doc.Create_By;
                              app_doc.Create_On = current_app_doc.Create_On;
                              foreach (var upload_receipt in app_doc.Upload_Receipt)
                              {
                                 if (current_app_doc.Upload_Receipt == null || current_app_doc.Upload_Receipt.Count() == 0)
                                 {
                                    if (upload_receipt != null)
                                    {
                                       var guid = Guid.NewGuid();
                                       while (db.Upload_Receipt.Where(w => w.Upload_Receipt_ID == guid).FirstOrDefault() != null)
                                          guid = Guid.NewGuid();

                                       upload_receipt.Upload_Receipt_ID = guid;
                                       db.Upload_Receipt.Add(upload_receipt);
                                    }
                                 }
                                 else
                                 {
                                    var current_upload_receipt = db.Upload_Receipt.Where(w => w.Expenses_Application_Document_ID == app_doc.Expenses_Application_Document_ID).FirstOrDefault();
                                    if (current_upload_receipt != null)
                                    {
                                       current_upload_receipt.File_Name = upload_receipt.File_Name;
                                       current_upload_receipt.Receipt = upload_receipt.Receipt;
                                       current_upload_receipt.Update_By = upload_receipt.Update_By;
                                       current_upload_receipt.Update_On = upload_receipt.Update_On;
                                    }
                                    // db.Entry(current_upload_receipt).CurrentValues.SetValues(upload_receipt);
                                 }
                              }
                           }
                           db.Entry(current_app_doc).CurrentValues.SetValues(app_doc);
                        }
                     }
                  }

                  //********  Start Smart Dev  ********//
                  if (expense.Overall_Status == WorkflowStatus.Closed && (expense.Cancel_Status == WorkflowStatus.Cancelled || expense.Cancel_Status == null))
                  {
                     //********  Calulate Job Cost  ********//
                     calulateJobCostByExpenseApplication(db, expense);
                  }
                  //********  End Smart Dev  ********//
                  db.Entry(current).CurrentValues.SetValues(expense);
                  db.SaveChanges();
                  db.Entry(current).GetDatabaseValues();
               }
               return new ServiceResult() { Code = ERROR_CODE.SUCCESS, Msg = new Success().getSuccess(ERROR_CODE.SUCCESS_EDIT), Field = Resource.Expenses };
            }
         }
         catch
         {
            return new ServiceResult() { Code = ERROR_CODE.ERROR_504_UPDATE_ERROR, Msg = new Error().getError(ERROR_CODE.ERROR_504_UPDATE_ERROR), Field = Resource.Expenses };
         }
      }

      public Expenses_Config_Detail GetExpensesConfigDetail(Nullable<int> pExpensesConfigID, Nullable<int> pProfileID)
      {
         try
         {
            var currentdate = StoredProcedure.GetCurrentDate();
            using (var db = new SBS2DBContext())
            {
               var emp = db.Employee_Profile.Where(w => w.Profile_ID == pProfileID).FirstOrDefault();
               if (emp == null) { return null; }

               var userhist = db.Employment_History
                .Include(i => i.Department)
                .Include(i => i.Designation)
                .Include(i => i.Employee_Profile)
                .Where(w => w.Employee_Profile_ID == emp.Employee_Profile_ID & w.Effective_Date <= currentdate)
                .OrderByDescending(o => o.Effective_Date)
                .FirstOrDefault();
               if (userhist == null) { return null; }

               var yearservice = 0;
               var firsthist = db.Employment_History.Where(w => w.Employee_Profile_ID == emp.Employee_Profile_ID).OrderBy(o => o.Effective_Date).FirstOrDefault();
               if (firsthist != null && firsthist.Effective_Date.HasValue)
               {
                  var hiredDate = firsthist.Effective_Date.Value;
                  var workspan = (currentdate.Date - hiredDate.Date);
                  yearservice = NumUtil.ParseInteger(Math.Floor((workspan.TotalDays + 1) / 365));
               }

               return db.Expenses_Config_Detail
                   .Where(w => w.Expenses_Config_ID == pExpensesConfigID & (w.Designation_ID == userhist.Designation_ID | w.Designation_ID == null) & w.Year_Service <= yearservice)
                   .OrderByDescending(o => o.Year_Service)
                   .FirstOrDefault();
            }
         }
         catch
         {
            return null;
         }
      }

      public decimal calulateBalance(Expenses_Config expenseType, Expenses_Config_Detail expenseTypeDetail, Nullable<int> profileID, DateTime appliedDate)
      {
         var currentdate = StoredProcedure.GetCurrentDate();
         decimal balance = 0;

         try
         {
            using (var db = new SBS2DBContext())
            {


               var emp = db.Employee_Profile.Where(w => w.Profile_ID == profileID).FirstOrDefault();
               if (emp == null) return 0;

               var userhist = db.Employment_History
               .Include(i => i.Department)
               .Include(i => i.Designation)
               .Include(i => i.Employee_Profile)
               .Where(w => w.Employee_Profile_ID == emp.Employee_Profile_ID & w.Effective_Date <= appliedDate)
               .OrderByDescending(o => o.Effective_Date)
               .FirstOrDefault();
               if (userhist == null) { return 0; }


               //Expense Type - Per Department / Per Employee 
               bool isPerDepartment = false;
               if (expenseType.Claimable_Type == "Per Department") isPerDepartment = true;


               if (expenseTypeDetail == null) { return 0; }

               DateTime startDate, endDate;
               var com = db.Company_Details.Where(w => w.Company_ID == emp.User_Profile.Company_ID & w.Effective_Date <= currentdate).OrderByDescending(o => o.Effective_Date).FirstOrDefault();
               if (com == null) return 0;

               if (expenseTypeDetail.Select_Per_Month.HasValue && expenseTypeDetail.Select_Per_Month.Value)
               {
                  balance = expenseTypeDetail.Amount_Per_Month.HasValue ? expenseTypeDetail.Amount_Per_Month.Value : 0;
                  startDate = DateUtil.ToDate(1, currentdate.Month, appliedDate.Year).Value;
                  endDate = DateUtil.ToDate(DateTime.DaysInMonth(appliedDate.Year, currentdate.Month), currentdate.Month, appliedDate.Year).Value;
               }
               else
               {
                  balance = expenseTypeDetail.Amount_Per_Year.HasValue ? expenseTypeDetail.Amount_Per_Year.Value : 0;
                  if (com.Default_Fiscal_Year.HasValue && com.Default_Fiscal_Year.Value)
                  {
                     startDate = DateUtil.ToDate(1, 1, appliedDate.Year).Value;
                     endDate = DateUtil.ToDate(DateTime.DaysInMonth(appliedDate.Year, appliedDate.Month), 12, appliedDate.Year).Value;
                  }
                  else
                  {
                     startDate = DateUtil.ToDate(com.Custom_Fiscal_Year.Value.Day, com.Custom_Fiscal_Year.Value.Month, (appliedDate.Year - 1)).Value;
                     endDate = DateUtil.ToDate(com.Custom_Fiscal_Year.Value.Day, com.Custom_Fiscal_Year.Value.Month, appliedDate.Year).Value;

                  }
               }


               var expenses = new List<Expenses_Application_Document>();
               if (isPerDepartment)
               {
                  expenses = getDepartmentExpenseApplications(expenseType.Department_ID);

               }
               else
               {
                  //Get all expense applications of Employee
                  expenses = getExpenseApplicationDocs(emp.Employee_Profile_ID);

               }

               //Filter with specific Expense Type between startDate and endDate
               expenses = expenses.Where(w => w.Expenses_Config_ID == expenseType.Expenses_Config_ID).Where(w => w.Expenses_Application.Date_Applied >= startDate).Where(w => w.Expenses_Application.Date_Applied <= endDate).ToList();

               //Calucate Balance
               if (expenses != null && expenses.Count > 0)
               {
                  foreach (Expenses_Application_Document expense in expenses)
                  {
                     if (expense.Expenses_Application.Overall_Status == SBSWorkFlowAPI.Constants.WorkflowStatus.Closed)
                     {
                        balance -= (expense.Amount_Claiming.HasValue ? expense.Amount_Claiming.Value : 0);
                     }
                  }
               }
            }
         }
         catch
         {
         }

         return balance;
      }

      public ServiceResult UpdateMultipleDeleteExpensesTypeStatus(int[] pExConfigID, string pStatus, string pUpdateBy)
      {
         try
         {
            var currentdate = StoredProcedure.GetCurrentDate();
            using (var db = new SBS2DBContext())
            {
               var current = db.Expenses_Config.Where(w => pExConfigID.Contains(w.Expenses_Config_ID));
               if (current != null)
               {
                  foreach (var ec in current)
                  {
                     ec.Update_On = currentdate;
                     ec.Update_By = pUpdateBy;
                     ec.Record_Status = pStatus;
                  }
                  db.SaveChanges();
               }
               return new ServiceResult() { Code = ERROR_CODE.SUCCESS, Msg = new Success().getSuccess(ERROR_CODE.SUCCESS_DELETE), Field = Resource.Expenses_Type };
            }
         }
         catch
         {
            return new ServiceResult() { Code = ERROR_CODE.ERROR_505_DELETE_ERROR, Msg = new Error().getError(ERROR_CODE.ERROR_505_DELETE_ERROR), Field = Resource.Expenses_Type };
         }
      }

      public ServiceResult UpdateDeleteExpenseTypeStatus(Nullable<int> pExConfigID, string pStatus, string pUpdateBy)
      {

         try
         {
            var currentdate = StoredProcedure.GetCurrentDate();
            using (var db = new SBS2DBContext())
            {
               var expense = db.Expenses_Config.Where(w => w.Expenses_Config_ID == pExConfigID).FirstOrDefault();
               if (expense == null)
                  return new ServiceResult { Code = ERROR_CODE.ERROR_511_DATA_NOT_FOUND, Msg = Resource.Expenses_Type + " " + Resource.Not_Found_Msg, Field = Resource.Expenses_Type };

               expense.Record_Status = pStatus;
               expense.Update_By = pUpdateBy;
               expense.Update_On = currentdate;

               db.SaveChanges();
               return new ServiceResult() { Code = ERROR_CODE.SUCCESS, Msg = new Success().getSuccess(ERROR_CODE.SUCCESS_DELETE), Field = Resource.Expenses_Type };
            }
         }
         catch
         {
            return new ServiceResult() { Code = ERROR_CODE.ERROR_505_DELETE_ERROR, Msg = new Error().getError(ERROR_CODE.ERROR_505_DELETE_ERROR), Field = Resource.Expenses_Type };
         }
      }

      //Added by sun 05-02-2016
      public ServiceResult InsertExpensesApplication(Expenses_Application[] ExpensesApp)
      {
         try
         {
            using (var db = new SBS2DBContext())
            {
               db.Expenses_Application.AddRange(ExpensesApp);
               db.SaveChanges();
               return new ServiceResult() { Code = ERROR_CODE.SUCCESS, Msg = new Success().getSuccess(ERROR_CODE.SUCCESS_CREATE), Field = Resource.Expenses };
            }
         }
         catch
         {
            return new ServiceResult() { Code = ERROR_CODE.ERROR_503_INSERT_ERROR, Msg = new Error().getError(ERROR_CODE.ERROR_503_INSERT_ERROR), Field = Resource.Expenses };
         }
      }

      public Expenses_No_Pattern getExpensesNoPattern(Nullable<int> pCompanyID)
      {
         Expenses_No_Pattern exNoPattern = null;

         try
         {
            using (var db = new SBS2DBContext())
            {
               exNoPattern = db.Expenses_No_Pattern
                   .Where(w => w.Company_ID == pCompanyID).FirstOrDefault();
            }
         }
         catch
         {
         }

         return exNoPattern;
      }

      public ServiceResult InsertExpensesNoPattern(Expenses_No_Pattern pExpensesPattern)
      {
         try
         {
            using (var db = new SBS2DBContext())
            {
               db.Expenses_No_Pattern.Add(pExpensesPattern);
               db.SaveChanges();
               return new ServiceResult() { Code = ERROR_CODE.SUCCESS, Msg = new Success().getSuccess(ERROR_CODE.SUCCESS_CREATE), Field = Resource.Expenses_Pattern };
            }
         }
         catch
         {
            return new ServiceResult() { Code = ERROR_CODE.ERROR_503_INSERT_ERROR, Msg = new Error().getError(ERROR_CODE.ERROR_503_INSERT_ERROR), Field = Resource.Expenses_Pattern };
         }
      }

      public ServiceResult UpdateExpensesRunningNumber(Nullable<int> pExpensesPatternID, Nullable<int> RunningNumber, string pUpdateBy)
      {
         try
         {
            var currentdate = StoredProcedure.GetCurrentDate();
            using (var db = new SBS2DBContext())
            {
               var expensePattern = db.Expenses_No_Pattern.Where(w => w.Expenses_No_Pattern_ID == pExpensesPatternID).FirstOrDefault();
               if (expensePattern == null)
                  return new ServiceResult { Code = ERROR_CODE.ERROR_511_DATA_NOT_FOUND, Msg = Resource.Expenses_Pattern + " " + Resource.Not_Found_Msg, Field = Resource.Expenses_Pattern };

               expensePattern.Update_On = currentdate;
               expensePattern.Update_By = pUpdateBy;
               expensePattern.Current_Running_Number = RunningNumber;
               db.SaveChanges();
               return new ServiceResult() { Code = ERROR_CODE.SUCCESS, Msg = new Success().getSuccess(ERROR_CODE.SUCCESS_DELETE), Field = Resource.Expenses_Pattern };
            }
         }
         catch
         {
            return new ServiceResult() { Code = ERROR_CODE.ERROR_505_DELETE_ERROR, Msg = new Error().getError(ERROR_CODE.ERROR_505_DELETE_ERROR), Field = Resource.Expenses_Pattern };
         }
      }


      #region Start Smart Dev
      public void calulateJobCostByExpenseApplication(SBS2DBContext db2, Expenses_Application ex)
      {
         if (ex != null)
         {
            var currentdate = StoredProcedure.GetCurrentDate();
            if (ex != null && ex.Expenses_Application_Document != null)
            {
               if (ex.Overall_Status == WorkflowStatus.Closed)
               {
                  foreach (var row in ex.Expenses_Application_Document)
                  {
                     decimal amount = 0M;
                     amount = ((row.Amount_Claiming.HasValue ? row.Amount_Claiming.Value : 0) + (row.Withholding_Tax_Amount.HasValue ? row.Withholding_Tax_Amount.Value : 0)) - (row.Tax_Amount.HasValue ? row.Tax_Amount.Value : 0);

                     if (row.Job_Cost_ID != null)
                     {
                        decimal total = 0;
                        var jobcost = db2.Job_Cost.Where(w => w.Job_Cost_ID == row.Job_Cost_ID).FirstOrDefault();
                        if (jobcost != null)
                        {
                           total = (jobcost.Costing.HasValue ? jobcost.Costing.Value : 0);

                           if (ex.Cancel_Status == WorkflowStatus.Cancelled)
                              total -= amount;
                           else
                              total += amount;

                           jobcost.Update_On = currentdate;
                           jobcost.Update_By = ex.Update_By;
                           jobcost.Costing = total;

                           if (!jobcost.Using.HasValue || !jobcost.Using.Value)
                              jobcost.Using = true;

                           //db.Entry(jobcost).State = EntityState.Modified;
                           //db2.SaveChanges();
                        }
                     }
                  }
               }
            }
         }
      }
      #endregion

      public ServiceObjectResult LstExpenseApplications(ExpenseCriteria criteria)
      {
         var result = new ServiceObjectResult();
         result.Object = new List<Expenses_Application_Document>();
         using (var db = new SBS2DBContext())
         {
            var expen = db.Expenses_Application_Document
                       .Include(i => i.Expenses_Application)
                       .Include(i => i.Employee_Profile)
                       .Include(i => i.Employee_Profile.User_Profile)
                       .Include(i => i.Currency)
                       .Include(i => i.Expenses_Config)
                       .Include(i => i.Department)
                       .Where(w => w.Employee_Profile.User_Profile.Company_ID == criteria.Company_ID && w.Expenses_Application.Cancel_Status != RecordStatus.Delete);

            if (criteria != null)
            {
               if (criteria.Expenses_Category_ID.HasValue)
                  expen = expen.Where(w => w.Expenses_Config.Expenses_Category_ID == criteria.Expenses_Category_ID);

               if (criteria.Job_Cost_ID.HasValue)
                  expen = expen.Where(w => w.Job_Cost_ID == criteria.Job_Cost_ID);

               if (criteria.Employee_Profile_ID.HasValue)
                  expen = expen.Where(w => w.Expenses_Application.Employee_Profile_ID == criteria.Employee_Profile_ID);

               if (criteria.Profile_ID.HasValue && criteria.Profile_ID.Value > 0)
                  expen = expen.Where(w => w.Employee_Profile.Profile_ID == criteria.Profile_ID);

               if (criteria.Expenses_Config_ID.HasValue && criteria.Expenses_Config_ID.Value > 0)
                  expen = expen.Where(w => w.Expenses_Config_ID == criteria.Expenses_Config_ID);

               if (criteria.Department_ID.HasValue && criteria.Department_ID.Value > 0)
                  expen = expen.Where(w => w.Department_ID == criteria.Department_ID);

               if (!string.IsNullOrEmpty(criteria.Date_From))
               {
                  var d = DateUtil.ToDate(criteria.Date_From);
                  if (d != null && d.HasValue)
                     expen = expen.Where(w => w.Expenses_Date >= d);
               }

               if (!string.IsNullOrEmpty(criteria.Date_To))
               {
                  var d = DateUtil.ToDate(criteria.Date_To);
                  if (d != null && d.HasValue)
                     expen = expen.Where(w => w.Expenses_Date <= d);
               }

               if (criteria.Year.HasValue)
               {
                  if (criteria.Year.Value > 0 && criteria.Year.ToString().Length == 4)
                     expen = expen.Where(w => w.Expenses_Date.Value.Year == criteria.Year);
               }

               if (criteria.Closed_Status)
               {
                  expen = expen.Where(w =>
                      w.Expenses_Application.Overall_Status == SBSWorkFlowAPI.Constants.WorkflowStatus.Closed
                      && w.Expenses_Application.Cancel_Status != WorkflowStatus.Cancelled);
               }

               if (!string.IsNullOrEmpty(criteria.Overall_Status))
                  expen = expen.Where(w => w.Expenses_Application.Overall_Status == criteria.Overall_Status);

               if (!string.IsNullOrEmpty(criteria.Date_Applied))
               {
                  var d = DateUtil.ToDate(criteria.Date_Applied);
                  if (d != null)
                     expen = expen.Where(w => w.Expenses_Application.Date_Applied.Value.Day == d.Value.Day & w.Expenses_Application.Date_Applied.Value.Month == d.Value.Month & w.Expenses_Application.Date_Applied.Value.Year == d.Value.Year);
               }
            }


            var obj = new List<Expenses_Application_Document>();
            obj = expen.ToList();
            result.Object = obj;
            return result;
         }
      }

      public ServiceObjectResult LstExpenses(ExpenseCriteria criteria)
      {
         var result = new ServiceObjectResult();
         result.Object = new List<Expenses_Application>();
         using (var db = new SBS2DBContext())
         {
            var expenses = db.Expenses_Application
                       .Include(i => i.TsEXes)
                       .Include(i => i.Employee_Profile)
                       .Include(i => i.Employee_Profile.User_Profile)
                       .Include(i => i.Expenses_Application_Document)
                       .Include(i => i.Expenses_Application_Document.Select(s => s.Expenses_Config))
                       .Where(w => w.Company_ID == criteria.Company_ID && w.Overall_Status != RecordStatus.Delete);

            if (criteria.Year.HasValue && criteria.Year > 0)
               expenses = expenses.Where(w => w.Expenses_Application_Document.Where(w2 => w2.Expenses_Date.Value.Year == criteria.Year).FirstOrDefault() != null);

            if (criteria.Job_Cost_ID.HasValue )
               expenses = expenses.Where(w => w.Expenses_Application_Document.Where(w2 => w2.Job_Cost_ID == criteria.Job_Cost_ID).FirstOrDefault() != null);

            if (criteria.Month.HasValue && criteria.Month > 0)
               expenses = expenses.Where(w => w.Expenses_Application_Document.Where(w2 => w2.Expenses_Date.Value.Month == criteria.Month).FirstOrDefault() != null);

            if (criteria.Request_Profile_ID.HasValue && criteria.Request_Profile_ID > 0)
               expenses = expenses.Where(w => w.Employee_Profile.Profile_ID == criteria.Request_Profile_ID);

            if (!string.IsNullOrEmpty(criteria.Date_Applied))
            {
               var d = DateUtil.ToDate(criteria.Date_Applied);
               if (d != null)
                  expenses = expenses.Where(w => w.Date_Applied.Value.Day == d.Value.Day & w.Date_Applied.Value.Month == d.Value.Month & w.Date_Applied.Value.Year == d.Value.Year);
            }

            if (!string.IsNullOrEmpty(criteria.Overall_Status))
            {
               if (criteria.Overall_Status == SBSWorkFlowAPI.Constants.WorkflowStatus.Pending)
                  expenses = expenses.Where(w => w.Overall_Status == SBSWorkFlowAPI.Constants.WorkflowStatus.Pending | w.Overall_Status == SBSWorkFlowAPI.Constants.WorkflowStatus.Approved);
               else
                  expenses = expenses.Where(w => w.Overall_Status == criteria.Overall_Status);
            }

            if (criteria.Include_Extra)
            {
               if (criteria.Profile_ID.HasValue && criteria.Employee_Profile_ID.HasValue)
               {
                  var ProfileIDStr = criteria.Profile_ID.Value.ToString();
                  if (criteria.Tab_Processed)
                  {
                     var tempResults = expenses.Where(w => (!string.IsNullOrEmpty(w.Approver) && w.Approver.Contains("|" + ProfileIDStr + "|")));
                     var tempResultsOut = tempResults.Where(w => w.Next_Approver.Contains("|" + ProfileIDStr + "|"));
                     var exIdsOut = tempResultsOut.Select(s => s.Expenses_Application_ID).ToList();
                     var exIds = tempResults.Select(s => s.Expenses_Application_ID).ToList();
                     expenses = expenses.Where(w =>
                        (!w.Supervisor.HasValue && !w.Request_ID.HasValue && !w.Request_Cancel_ID.HasValue && w.Employee_Profile_ID == criteria.Employee_Profile_ID.Value ? true : false) ||
                        (exIds.Contains(w.Expenses_Application_ID) && !exIdsOut.Contains(w.Expenses_Application_ID) ? true : false));
                  }
                  else if (criteria.Tab_Pending)
                  {
                     expenses = expenses.Where(w =>
                        (string.IsNullOrEmpty(w.Cancel_Status) && (w.Overall_Status != WorkflowStatus.Closed || w.Overall_Status != WorkflowStatus.Rejected) ? true : false ||
                        !string.IsNullOrEmpty(w.Cancel_Status) && (w.Cancel_Status != WorkflowStatus.Cancellation_Rejected || w.Cancel_Status != WorkflowStatus.Cancelled) ? true : false)
                        ? true : false
                        );

                     var tempResults = expenses.Where(w => (!string.IsNullOrEmpty(w.Next_Approver) && w.Next_Approver.Contains("|" + ProfileIDStr + "|")));
                     var exIds = tempResults.Select(s => s.Expenses_Application_ID).ToList();
                     expenses = expenses.Where(w => exIds.Contains(w.Expenses_Application_ID));
                  }
               }
            }
            else
            {
               if (criteria.Profile_ID.HasValue)
                  expenses = expenses.Where(w => w.Employee_Profile.Profile_ID == criteria.Profile_ID);

               if (criteria.Employee_Profile_ID.HasValue)
                  expenses = expenses.Where(w => w.Employee_Profile_ID == criteria.Employee_Profile_ID);
            }

            if (criteria.Include_Draft)
            {
            }
            else
               expenses = expenses.Where(w => w.Overall_Status != WorkflowStatus.Draft);

            expenses = expenses.OrderByDescending(o => o.Expenses_Application_Document.Select(s => s.Date_Applied).FirstOrDefault()).ThenBy(o => o.Employee_Profile_ID);
         
            result.Record_Count = expenses.Count();
            criteria.Record_Count = result.Record_Count;
            if (result.Record_Count > 300 && criteria.Start_Index == 0 && criteria.Page_Size == 0)
               criteria.Page_Size = 30;

            if (criteria.Top.HasValue)
               expenses = expenses.Take(criteria.Top.Value);

            else if (criteria.End_Index > 0)
               expenses = expenses.Skip(criteria.Start_Index).Take(criteria.End_Index);

            else if (criteria.Page_Size > 0)
            {
               if (criteria.Page_No > 1)
               {
                  var startindex = criteria.Page_Size * (criteria.Page_No - 1);
                  expenses = expenses.Skip(startindex).Take(criteria.Page_Size);
               }
               else
                  expenses = expenses.Skip(criteria.Start_Index).Take(criteria.Page_Size);
            }

            var obj = new List<Expenses_Application>();
            obj = expenses.ToList();

            result.Object = obj;
            result.Start_Index = criteria.Start_Index;
            result.Page_Size = criteria.Page_Size;

            return result;
         }
      }

      public ServiceResult toolsAp(List<Expenses_Application> expenselst)
      {
         try
         {
            var currentdate = StoredProcedure.GetCurrentDate();
            using (var db = new SBS2DBContext())
            {
               foreach (var row in expenselst)
               {
                  var current = db.Expenses_Application.Where(w => w.Expenses_Application_ID == row.Expenses_Application_ID).FirstOrDefault();
                  if (current != null)
                  {
                     db.Entry(current).CurrentValues.SetValues(row);
                  }
               }
               db.SaveChanges();
               return new ServiceResult() { Code = ERROR_CODE.SUCCESS, Msg = new Success().getSuccess(ERROR_CODE.SUCCESS_EDIT), Field = Resource.Expenses };
            }
         }
         catch
         {
            return new ServiceResult() { Code = ERROR_CODE.ERROR_504_UPDATE_ERROR, Msg = new Error().getError(ERROR_CODE.ERROR_504_UPDATE_ERROR), Field = Resource.Expenses };
         }
      }
   }

   public class Expenses_Type_Detail
   {
      public int[] Designations { get; set; }
      public Nullable<int> Expenses_Config_ID { get; set; }
      public Nullable<decimal> Amount_Per_Year { get; set; }
      public Nullable<decimal> Amount_Per_Month { get; set; }
      public Nullable<int> Year_Service { get; set; }
      public Nullable<int> Group_ID { get; set; }
      public bool Select_Percentage { get; set; }
      public Nullable<decimal> Amount { get; set; }
      public bool Select_Per_Month { get; set; }
      public bool Select_Amount { get; set; }
      public Nullable<decimal> Pecentage { get; set; }
      public string Row_Type { get; set; }
   }

   public class Expenses_Detail
   {
      public Nullable<int> Expenses_Application_Document_ID { get; set; }
      public Nullable<int> Expenses_Config_ID { get; set; }
      public string Claimable_Type { get; set; }
      public Nullable<int> Department_ID { get; set; }
      public string Date_Applied { get; set; }
      public Nullable<decimal> Total_Amount { get; set; }
      public Nullable<decimal> Amount_Claiming { get; set; }
      public Nullable<decimal> Balance { get; set; }
      public Nullable<decimal> Tax { get; set; }
      public string Expenses_Type_Desc { get; set; }
      public string Expenses_Type_Name { get; set; }
      public string Notes { get; set; }
      public Nullable<System.Guid> Upload_Receipt_ID { get; set; }
      public Nullable<int> Selected_Currency { get; set; }
      public Nullable<decimal> Mileage { get; set; }
      public string Expenses_Date { get; set; }
      public string Upload_Receipt { get; set; }
      public string Upload_Receipt_Name { get; set; }
      public string Row_Type { get; set; }
      public string Doc_No { get; set; }
      //********  Smart Dev  ********//
      public Nullable<int> Job_Cost_ID { get; set; }
      public Nullable<decimal> Withholding_Tax { get; set; }
      public string Tax_Type { get; set; }

      public Nullable<decimal> Withholding_Tax_Amount { get; set; }
      public Nullable<decimal> Tax_Amount { get; set; }
      public string Tax_Amount_Type { get; set; }
      public string Withholding_Tax_Type { get; set; }
   }

}