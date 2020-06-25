using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.Net.Mail;
using System.Security.Cryptography;
using System.Text;
using System.Data.Entity;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Threading.Tasks;
using SBSModel.Common;
using SBSWorkFlowAPI.Models;
using System.Data.Entity.Core.Objects;
using SBSResourceAPI;
using System.Diagnostics;
using SBSModel.Models;

namespace SBSModel.Models
{

   public class _Time_Sheet
   {
      public int Time_Sheet_ID { get; set; }
      public Nullable<int> Employee_Profile_ID { get; set; }
      public string Date_Of_Date { get; set; }
      public Nullable<int> Job_Cost_ID { get; set; }
      public string Clock_In { get; set; }
      public string Clock_Out { get; set; }
      public string Note { get; set; }
      public Nullable<decimal> Hour_Rate { get; set; }
      public string Launch_Duration { get; set; }
      public string Duration { get; set; }
      public Nullable<decimal> Total_Amount { get; set; }
   }

   public class JobCostCriteria : CriteriaBase
   {
      public Nullable<int> Job_Cost_ID { get; set; }
      public string Indent_No { get; set; }
      public string Indent_Name { get; set; }
      public string Indent_No_Dup { get; set; }

      //use in action Delete, Active, Inactive
      public Nullable<int> Customer_ID { get; set; }

   }
   public class JobCostService
   {
      public Job_Cost GetJobCost(Nullable<int> pJobCostID)
      {
         using (var db = new SBS2DBContext())
         {
            return db.Job_Cost
               .Include(w => w.Job_Cost_Payment_Term)
               .Include(w => w.Expenses_Config_Budget)
               .Include(w => w.Expenses_Config_Budget.Select(s => s.Expenses_Config))
               .Include(w => w.Customer)
               .Where(w => w.Job_Cost_ID == pJobCostID && w.Record_Status != RecordStatus.Delete).FirstOrDefault();
         }
      }

      public decimal GetCosting(Nullable<int> pJobCostID)
      {
         using (var db = new SBS2DBContext())
         {
            decimal excosting = 0;
            var exs = db.Expenses_Application_Document.Where(w => w.Job_Cost_ID == pJobCostID && w.Expenses_Application.Overall_Status == SBSWorkFlowAPI.Constants.WorkflowStatus.Closed);
            if (exs.Count() > 0)
               excosting = exs.Select(s => ((s.Amount_Claiming.HasValue ? s.Amount_Claiming.Value : 0) + (s.Withholding_Tax_Amount.HasValue ? s.Withholding_Tax_Amount.Value : 0)) - (s.Tax_Amount.HasValue ? s.Tax_Amount.Value : 0)).Sum();

            decimal tscosting = 0;
            var tss = db.Time_Sheet_Dtl.Where(w => w.Job_Cost_ID == pJobCostID && w.Time_Sheet.Overall_Status == SBSWorkFlowAPI.Constants.WorkflowStatus.Closed);
            if (tss.Count() > 0)
               tscosting = tss.Select(s => s.Total_Amount.HasValue ? s.Total_Amount.Value : 0).Sum();


            return excosting + tscosting;
         }
      }


      public List<Job_Cost> LstJobCost(JobCostCriteria criteria)
      {
         using (var db = new SBS2DBContext())
         {
            var Job = db.Job_Cost
               .Include(w => w.Employee_Profile)
               .Include(w => w.Employee_Profile.User_Profile)
               .Include(w => w.Customer)
               .Include(w => w.Job_Cost_Payment_Term)
               .Include(w => w.Expenses_Application_Document)
               .Include(w => w.Expenses_Application_Document.Select(s => s.Expenses_Application))
               .Include(w => w.Time_Sheet_Dtl)
               .Include(w => w.Time_Sheet_Dtl.Select(s => s.Time_Sheet))
               .Where(w => w.Company_ID == criteria.Company_ID && w.Record_Status != RecordStatus.Delete);

            if (criteria != null)
            {
               if (criteria.Job_Cost_ID.HasValue && criteria.Job_Cost_ID.Value > 0)
                  Job = Job.Where(w => w.Job_Cost_ID == criteria.Job_Cost_ID);
               if (!string.IsNullOrEmpty(criteria.Indent_No))
                  Job = Job.Where(w => w.Indent_No.Contains(criteria.Indent_No));
               if (!string.IsNullOrEmpty(criteria.Indent_Name))
                  Job = Job.Where(w => w.Indent_Name.Contains(criteria.Indent_Name));
               if (!string.IsNullOrEmpty(criteria.Indent_No_Dup))
                  Job = Job.Where(w => w.Indent_No == criteria.Indent_No_Dup);
               if (criteria.Customer_ID.HasValue && criteria.Customer_ID.Value > 0)
                  Job = Job.Where(w => w.Customer_ID == criteria.Customer_ID);
               if (!string.IsNullOrEmpty(criteria.Record_Status))
                  Job = Job.Where(w => w.Record_Status == criteria.Record_Status);
            }

            return Job.OrderBy(o => o.Indent_No).ToList();
         }
      }

      public ServiceResult UpdateJobCost(Job_Cost pJobCost)
      {
         try
         {
            using (var db = new SBS2DBContext())
            {
               var current = db.Job_Cost.Where(w => w.Job_Cost_ID == pJobCost.Job_Cost_ID).FirstOrDefault();
               if (current == null)
                  return new ServiceResult { Code = ERROR_CODE.ERROR_511_DATA_NOT_FOUND, Msg = Resource.Job_Cost + " " + Resource.Not_Found_Msg, Field = Resource.Job_Cost };

               //Check Funtion Delete 
               if (pJobCost.Record_Status != RecordStatus.Delete)
               {
                  var JobCostPaymentTermRemove = new List<Job_Cost_Payment_Term>();
                  foreach (var row in current.Job_Cost_Payment_Term)
                  {
                     if (pJobCost.Job_Cost_Payment_Term == null || !pJobCost.Job_Cost_Payment_Term.Select(s => s.Job_Cost_PayMent_Term_ID).Contains(row.Job_Cost_PayMent_Term_ID))
                     {
                        JobCostPaymentTermRemove.Add(row);
                     }
                  }
                  if (JobCostPaymentTermRemove.Count > 0)
                  {
                     db.Job_Cost_Payment_Term.RemoveRange(JobCostPaymentTermRemove);
                  }
                  if (pJobCost.Job_Cost_Payment_Term != null)
                  {
                     foreach (var row in pJobCost.Job_Cost_Payment_Term)
                     {
                        if (row.Job_Cost_PayMent_Term_ID == 0 || !pJobCost.Job_Cost_Payment_Term.Select(s => s.Job_Cost_PayMent_Term_ID).Contains(row.Job_Cost_PayMent_Term_ID))
                        {
                           // add
                           db.Job_Cost_Payment_Term.Add(row);
                        }
                        else
                        {
                           var currPT = db.Job_Cost_Payment_Term.Where(w => w.Job_Cost_PayMent_Term_ID == row.Job_Cost_PayMent_Term_ID).FirstOrDefault();
                           if (currPT != null)
                           {
                              //update
                              row.Create_By = currPT.Create_By;
                              row.Create_On = currPT.Create_On;
                              db.Entry(currPT).CurrentValues.SetValues(row);
                           }
                        }
                     }
                  }


                  var bgRemoves = new List<Expenses_Config_Budget>();
                  foreach (var row in current.Expenses_Config_Budget)
                  {
                     if (pJobCost.Expenses_Config_Budget == null || !pJobCost.Expenses_Config_Budget.Select(s => s.Budget_ID).Contains(row.Budget_ID))
                        bgRemoves.Add(row);
                  }
                  if (bgRemoves.Count > 0)
                  {
                     db.Expenses_Config_Budget.RemoveRange(bgRemoves);
                  }
                  if (pJobCost.Expenses_Config_Budget != null)
                  {
                     foreach (var row in pJobCost.Expenses_Config_Budget)
                     {
                        if (row.Budget_ID == 0 || !pJobCost.Expenses_Config_Budget.Select(s => s.Budget_ID).Contains(row.Budget_ID))
                        {
                           // add
                           db.Expenses_Config_Budget.Add(row);
                        }
                        else
                        {
                           var currBG = db.Expenses_Config_Budget.Where(w => w.Budget_ID == row.Budget_ID).FirstOrDefault();
                           if (currBG != null)
                           {
                              //update
                              db.Entry(currBG).CurrentValues.SetValues(row);
                           }
                        }
                     }
                  }
               }
               db.Entry(current).CurrentValues.SetValues(pJobCost);
               db.SaveChanges();
               return new ServiceResult() { Code = ERROR_CODE.SUCCESS, Msg = new Success().getSuccess(ERROR_CODE.SUCCESS_EDIT), Field = Resource.Job_Cost };
            }
         }
         catch
         {
            return new ServiceResult() { Code = ERROR_CODE.ERROR_504_UPDATE_ERROR, Msg = new Error().getError(ERROR_CODE.ERROR_504_UPDATE_ERROR), Field = Resource.Job_Cost };
         }
      }

      public ServiceResult InsertJobCost(Job_Cost pJobCost)
      {
         try
         {
            using (var db = new SBS2DBContext())
            {
               db.Job_Cost.Add(pJobCost);
               db.SaveChanges();
               return new ServiceResult() { Code = ERROR_CODE.SUCCESS, Msg = new Success().getSuccess(ERROR_CODE.SUCCESS_CREATE), Field = Resource.Job_Cost };
            }
         }
         catch
         {
            return new ServiceResult() { Code = ERROR_CODE.ERROR_503_INSERT_ERROR, Msg = new Error().getError(ERROR_CODE.ERROR_503_INSERT_ERROR), Field = Resource.Job_Cost };
         }
      }

      public ServiceResult CalCosting(Nullable<int> pJobID, decimal? cost)
      {
         try
         {
            var currentdate = StoredProcedure.GetCurrentDate();
            using (var db = new SBS2DBContext())
            {
               var jobcost = db.Job_Cost.Where(w => w.Job_Cost_ID == pJobID).FirstOrDefault();
               if (jobcost != null)
               {
                  var total = (jobcost.Costing.HasValue ? jobcost.Costing.Value : 0);
                  total += (cost.HasValue ? cost.Value : 0);
                  jobcost.Costing = total;
                  db.SaveChanges();
               }
               return new ServiceResult() { Code = ERROR_CODE.SUCCESS, Msg = new Success().getSuccess(ERROR_CODE.SUCCESS_EDIT), Field = Resource.Job_Cost };
            }
         }
         catch
         {
            return new ServiceResult() { Code = ERROR_CODE.ERROR_506_SAVE_ERROR, Msg = new Error().getError(ERROR_CODE.ERROR_506_SAVE_ERROR), Field = Resource.Job_Cost };
         }
      }
   }
}
