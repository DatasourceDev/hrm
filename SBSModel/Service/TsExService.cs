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
using System.Web.Routing;
using System.Net;
using System.Data;
using Newtonsoft.Json.Linq;
using System.Configuration;
using System.IO;
using System.Text;
using System.Web.Configuration;
using SBSResourceAPI;
using SBSWorkFlowAPI.Constants;

namespace SBSModel.Models
{
   public class ExApprover
   {
      public string Approver { get; set; }
      public string Next_Approver { get; set; }
   }

   public class TsExCriteria : CriteriaBase
   {
      public Nullable<int> Employee_Profile_ID { get; set; }

      public Nullable<int> Month { get; set; }
      public Nullable<int> Year { get; set; }
      public bool Include_Draft { get; set; }

      public bool Include_Extra { get; set; }
      public bool Tab_Processed { get; set; }
      public bool Tab_Pending { get; set; }

      public Nullable<int> Request_Profile_ID { get; set; }
      public Nullable<int> Department_ID { get; set; }

      public Nullable<int> Job_Cost_ID { get; set; }

   }

   public class TsExService
   {
      public void Round(Nullable<int> pComID)
      {
         var curdate = StoredProcedure.GetCurrentDate();
         using (var db = new SBS2DBContext())
         {
            foreach (var e in db.Expenses_Application.Where(w => w.Company_ID == pComID))
            {
               foreach (var ex in e.Expenses_Application_Document)
               {
                  var _taxAmount = 0M;
                  var _WithholdingTaxAmount = 0M;

                  if (ex.Tax_Type == TaxType.Exclusive)
                  {
                     if (ex.Total_Amount.HasValue && ex.Total_Amount.Value > 0)
                     {
                        if (ex.Tax_Amount_Type.Equals("%"))
                           _taxAmount = NumUtil.Round(ex.Total_Amount.Value * (ex.Tax.Value / 100));
                        else
                           _taxAmount = ex.Tax.Value;

                     }
                  }
                  else
                  {
                     if (ex.Tax_Amount_Type.Equals("%"))
                        _taxAmount = NumUtil.Round(ex.Total_Amount.Value * ex.Tax.Value / (ex.Tax.Value + 100));
                     else
                        _taxAmount = ex.Tax.Value;
                  }
                  if (_taxAmount > 0 && ex.Tax_Amount != _taxAmount)
                  {
                     ex.Tax_Amount = _taxAmount;
                     ex.Update_On = curdate;
                     ex.Update_By = "system";
                  }
                     
                  if (ex.Withholding_Tax.HasValue && ex.Withholding_Tax.Value > 0)
                  {
                     if (ex.Withholding_Tax_Type.Equals("%"))
                        _WithholdingTaxAmount = NumUtil.Round(ex.Total_Amount * (ex.Withholding_Tax.Value / 100));
                     else
                        _WithholdingTaxAmount = ex.Withholding_Tax.Value;
                  }
                  if (_WithholdingTaxAmount > 0 && ex.Withholding_Tax_Amount != _WithholdingTaxAmount)
                  {
                     ex.Withholding_Tax_Amount = _WithholdingTaxAmount;
                     ex.Update_On = curdate;
                     ex.Update_By = "system";
                  }
               
               }
            }
            db.SaveChanges();
         }
      }
      public TsEX GetTsEx(Nullable<int> pID)
      {
         using (var db = new SBS2DBContext())
         {
            return db.TsEXes
               .Include(i => i.Time_Sheet)
               .Include(i => i.Time_Sheet.Time_Sheet_Dtl)
               .Include(i => i.Expenses_Application)
               .Include(i => i.Expenses_Application.Expenses_Application_Document)
               .Include(i => i.Expenses_Application.Expenses_Application_Document.Select(s => s.Expenses_Config))
               .Include(i => i.Expenses_Application.Expenses_Application_Document.Select(s => s.Job_Cost))
               .Include(i => i.Expenses_Application.Expenses_Application_Document.Select(s => s.Upload_Receipt))
               .Where(w => w.TsEx_ID == pID).FirstOrDefault();
         }
      }
      public TsEX GetTsExByMonthYear(int? comID, int? eID, int month, int year)
      {
         using (var db = new SBS2DBContext())
         {
            return db.TsEXes
               .Where(w => w.Month == month & w.Year == year & w.Company_ID == comID & w.Employee_Profile_ID == eID).FirstOrDefault();
         }
      }
      public List<TsEX> LstTsEx(Nullable<int> pCID, int? pID, int? pmonth, int? pyear)
      {
         using (var db = new SBS2DBContext())
         {
            var tsexs = db.TsEXes
               .Include(i => i.Expenses_Application)
               .Include(i => i.Expenses_Application.Expenses_Application_Document)
               .Include(i => i.Time_Sheet)
               //.Include(i => i.Time_Sheet.Time_Sheet_Dtl)
               .Where(w => w.Company_ID == pCID && w.Employee_Profile.Profile_ID == pID);
            if (pmonth.HasValue)
               tsexs = tsexs.Where(w => w.Month == pmonth);
            if (pyear.HasValue)
               tsexs = tsexs.Where(w => w.Year == pyear);
            return tsexs.ToList();
         }
      }

      public ServiceResult InsertTsEs(TsEX pTsEx)
      {
         var currentdate = StoredProcedure.GetCurrentDate();
         try
         {
            using (var db = new SBS2DBContext())
            {
               var eNo = "";
               var pattern = db.Expenses_No_Pattern.Where(w => w.Company_ID == pTsEx.Company_ID).FirstOrDefault();
               if (pattern == null)
               {
                  var newpattern = new Expenses_No_Pattern()
                  {
                     Company_ID = pTsEx.Company_ID,
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
               pTsEx.Doc_No = eNo;
               pTsEx.Expenses_Application.Expenses_No = eNo;
               db.TsEXes.Add(pTsEx);
               db.SaveChanges();
               db.Entry(pTsEx).GetDatabaseValues();
               return new ServiceResult() { Code = ERROR_CODE.SUCCESS, Msg = new Success().getSuccess(ERROR_CODE.SUCCESS_CREATE), Field = Resource.Timesheet_Expenses };
            }
         }
         catch
         {
            return new ServiceResult() { Code = ERROR_CODE.ERROR_503_INSERT_ERROR, Msg = new Error().getError(ERROR_CODE.ERROR_503_INSERT_ERROR), Field = Resource.Timesheet_Expenses };
         }
      }
      public ServiceResult UpdateTsExStatus(int? pTsExID, string status = null, string cstatus = null, int? rID = null, int? cID = null, bool includeapprove = false, ExApprover exapprover = null)
      {
         try
         {
            using (var db = new SBS2DBContext())
            {
               var current = db.TsEXes.Where(w => w.TsEx_ID == pTsExID).FirstOrDefault();
               if (current != null)
               {
                  if (status == WorkflowStatus.Closed && current.Expenses_Application.Overall_Status != WorkflowStatus.Closed)
                  {
                     foreach (var row in current.Expenses_Application.Expenses_Application_Document)
                     {
                        decimal amount = ((row.Amount_Claiming.HasValue ? row.Amount_Claiming.Value : 0) + (row.Withholding_Tax_Amount.HasValue ? row.Withholding_Tax_Amount.Value : 0)) - (row.Tax_Amount.HasValue ? row.Tax_Amount.Value : 0);
                        if (row.Job_Cost_ID != null)
                        {
                           decimal total = 0;
                           var jobcost = db.Job_Cost.Where(w => w.Job_Cost_ID == row.Job_Cost_ID).FirstOrDefault();
                           if (jobcost != null)
                           {
                              total = (jobcost.Costing.HasValue ? jobcost.Costing.Value : 0);
                              total += amount;
                              jobcost.Costing = total;

                              if (!jobcost.Using.HasValue || !jobcost.Using.Value)
                                 jobcost.Using = true;
                           }
                        }
                     }

                     if (current.Time_Sheet != null)
                     {
                        foreach (var row2 in current.Time_Sheet.Time_Sheet_Dtl)
                        {
                           decimal amount2 = (row2.Total_Amount.HasValue ? row2.Total_Amount.Value : 0);
                           if (row2.Job_Cost_ID != null)
                           {
                              decimal total2 = 0;
                              var jobcost2 = db.Job_Cost.Where(w => w.Job_Cost_ID == row2.Job_Cost_ID).FirstOrDefault();
                              if (jobcost2 != null)
                              {
                                 total2 = (jobcost2.Costing.HasValue ? jobcost2.Costing.Value : 0);
                                 total2 += amount2;
                                 jobcost2.Costing = total2;

                                 if (!jobcost2.Using.HasValue || !jobcost2.Using.Value)
                                    jobcost2.Using = true;
                              }
                           }
                        }
                     }
                  }
                  if (!string.IsNullOrEmpty(status))
                  {
                     current.Expenses_Application.Overall_Status = status;
                     current.Time_Sheet.Overall_Status = status;
                  }
                  if (!string.IsNullOrEmpty(cstatus))
                  {
                     current.Expenses_Application.Cancel_Status = cstatus;
                     current.Time_Sheet.Cancel_Status = cstatus;
                  }
                  if (rID.HasValue)
                  {
                     current.Expenses_Application.Supervisor = null;
                     current.Expenses_Application.Request_ID = rID;
                     current.Time_Sheet.Supervisor = null;
                     current.Time_Sheet.Request_ID = rID;
                  }
                  if (cID.HasValue)
                  {
                     current.Expenses_Application.Supervisor = null;
                     current.Expenses_Application.Request_Cancel_ID = cID;
                     current.Time_Sheet.Supervisor = null;
                     current.Time_Sheet.Request_Cancel_ID = cID;
                  }

                  if (includeapprove)
                  {
                     current.Expenses_Application.Approver = exapprover.Approver;
                     current.Expenses_Application.Next_Approver = exapprover.Next_Approver;
                  }


                  db.SaveChanges();
                  return new ServiceResult() { Code = ERROR_CODE.SUCCESS, Msg = new Success().getSuccess(ERROR_CODE.SUCCESS_EDIT), Field = Resource.Timesheet_Expenses };
               }
            }
         }
         catch
         {
         }
         return new ServiceResult() { Code = ERROR_CODE.ERROR_504_UPDATE_ERROR, Msg = new Error().getError(ERROR_CODE.ERROR_504_UPDATE_ERROR), Field = Resource.Timesheet_Expenses };
      }
      public ServiceResult UpdateTsEx(TsEX pTsEx)
      {
         try
         {
            using (var db = new SBS2DBContext())
            {
               var newts = false;
               var newex = false;
               var current = db.TsEXes.Where(w => w.TsEx_ID == pTsEx.TsEx_ID).FirstOrDefault();
               if (current == null)
                  return new ServiceResult { Code = ERROR_CODE.ERROR_511_DATA_NOT_FOUND, Msg = Resource.Time_Sheet + " " + Resource.Not_Found_Msg, Field = Resource.Timesheet_Expenses };

               if (current.Time_Sheet == null)
               {
                  db.Time_Sheet.Add(pTsEx.Time_Sheet);
                  newts = true;
               }
               else 
               {
                  if (pTsEx.Time_Sheet.Time_Sheet_Dtl.Count > 0)
                  {
                     if (current.Time_Sheet.Time_Sheet_Dtl.Count() > 0)
                     {
                        var dIDs = pTsEx.Time_Sheet.Time_Sheet_Dtl.Where(w=>w.Dtl_ID> 0).Select(s => s.Dtl_ID);
                        var rms = current.Time_Sheet.Time_Sheet_Dtl.Where(w => !dIDs.Contains(w.Dtl_ID));
                        if (rms.Count() > 0)
                           db.Time_Sheet_Dtl.RemoveRange(rms);
                        
                     }
                     foreach (var row in pTsEx.Time_Sheet.Time_Sheet_Dtl)
                     {
                        if (row.Dtl_ID > 0)
                        {
                           var curdtl = db.Time_Sheet_Dtl.Where(w => w.Dtl_ID == row.Dtl_ID).FirstOrDefault();
                           if (curdtl != null)
                              db.Entry(curdtl).CurrentValues.SetValues(row);
                        }
                        else
                           db.Time_Sheet_Dtl.Add(row);
                     }
                  }
                  else
                  {
                     if (current.Time_Sheet.Time_Sheet_Dtl.Count() > 0)
                        db.Time_Sheet_Dtl.RemoveRange(current.Time_Sheet.Time_Sheet_Dtl);
                  }
                
                  db.Entry(current.Time_Sheet).CurrentValues.SetValues(pTsEx.Time_Sheet);
               }

               if (current.Expenses_Application == null)
               {
                  db.Expenses_Application.Add(pTsEx.Expenses_Application);
                  newex = true;
               }
               else
               {
                  if (pTsEx.Expenses_Application.Expenses_Application_Document.Count > 0)
                  {
                     if (current.Expenses_Application.Expenses_Application_Document.Count() > 0)
                     {
                        var dIDs = pTsEx.Expenses_Application.Expenses_Application_Document.Where(w => w.Expenses_Application_Document_ID > 0).Select(s => s.Expenses_Application_Document_ID);
                       
                           var rms = current.Expenses_Application.Expenses_Application_Document.Where(w => !dIDs.Contains(w.Expenses_Application_Document_ID));
                           if (rms.Count() > 0)
                           {
                              foreach (var curdtl in rms)
                              {
                                 if (curdtl.Upload_Receipt.Count() > 0)
                                    db.Upload_Receipt.RemoveRange(curdtl.Upload_Receipt);
                              }
                              db.Expenses_Application_Document.RemoveRange(rms);
                           }
                        
                     }
                     foreach (var row in pTsEx.Expenses_Application.Expenses_Application_Document)
                     {
                        if (row.Expenses_Application_Document_ID > 0)
                        {
                           var curdtl = db.Expenses_Application_Document.Where(w => w.Expenses_Application_Document_ID == row.Expenses_Application_Document_ID).FirstOrDefault();
                           if (curdtl != null)
                              db.Entry(curdtl).CurrentValues.SetValues(row);
                        }
                        else
                           db.Expenses_Application_Document.Add(row);
                     }
                  }
                  else
                  {
                     if (current.Expenses_Application.Expenses_Application_Document.Count() > 0)
                        db.Expenses_Application_Document.RemoveRange(current.Expenses_Application.Expenses_Application_Document);
                  }
                  
                  db.Entry(current.Expenses_Application).CurrentValues.SetValues(pTsEx.Expenses_Application);
               }
               db.Entry(current).CurrentValues.SetValues(pTsEx);
               db.SaveChanges();

               if (newts && pTsEx.Time_Sheet != null && pTsEx.Time_Sheet.Time_Sheet_ID > 0)
               {
                  current = db.TsEXes.Where(w => w.TsEx_ID == pTsEx.TsEx_ID).FirstOrDefault();
                  current.Time_Sheet_ID = pTsEx.Time_Sheet.Time_Sheet_ID;
                  db.SaveChanges();
               }
               if (newex &&  pTsEx.Expenses_Application != null && pTsEx.Expenses_Application.Expenses_Application_ID > 0)
               {
                  current = db.TsEXes.Where(w => w.TsEx_ID == pTsEx.TsEx_ID).FirstOrDefault();
                  current.Expenses_Application_ID = pTsEx.Expenses_Application.Expenses_Application_ID;
                  db.SaveChanges();
               }
               return new ServiceResult() { Code = ERROR_CODE.SUCCESS, Msg = new Success().getSuccess(ERROR_CODE.SUCCESS_EDIT), Field = Resource.Timesheet_Expenses };
            }

         }
         catch
         {
            return new ServiceResult() { Code = ERROR_CODE.ERROR_504_UPDATE_ERROR, Msg = new Error().getError(ERROR_CODE.ERROR_504_UPDATE_ERROR), Field = Resource.Timesheet_Expenses };
         }
      }


      public ServiceObjectResult LstTsEx(TsExCriteria criteria)
      {
         var result = new ServiceObjectResult();
         result.Object = new List<TsEX>();
         using (var db = new SBS2DBContext())
         {
            var tsexs = db.TsEXes
                       .Include(i => i.Employee_Profile)
                       .Include(i => i.Employee_Profile.User_Profile)
                       .Include(i => i.Expenses_Application)
                       .Include(i => i.Expenses_Application.Expenses_Application_Document)
                       .Include(i => i.Time_Sheet)
                       .Include(i => i.Time_Sheet.Time_Sheet_Dtl)
                       .Where(w => w.Employee_Profile.User_Profile.Company_ID == criteria.Company_ID && w.Expenses_Application.Overall_Status != RecordStatus.Delete);

            if (criteria.Year.HasValue && criteria.Year > 0)
               tsexs = tsexs.Where(w => w.Year == criteria.Year);

            if (criteria.Month.HasValue && criteria.Month > 0)
               tsexs = tsexs.Where(w => w.Month == criteria.Month);

            if (criteria.Request_Profile_ID.HasValue && criteria.Request_Profile_ID > 0)
               tsexs = tsexs.Where(w => w.Employee_Profile.Profile_ID == criteria.Request_Profile_ID);

            if (criteria.Include_Extra)
            {
               if (criteria.Profile_ID.HasValue && criteria.Employee_Profile_ID.HasValue)
               {
                  var ProfileIDStr = criteria.Profile_ID.Value.ToString();
                  if (criteria.Tab_Processed)
                  {
                     var tempResults = tsexs.Where(w => (!string.IsNullOrEmpty(w.Expenses_Application.Approver) && w.Expenses_Application.Approver.Contains("|" + ProfileIDStr + "|")));
                     var tempResultsOut = tempResults.Where(w => w.Expenses_Application.Next_Approver.Contains("|" + ProfileIDStr + "|"));
                     var exIdsOut = tempResultsOut.Select(s => s.Expenses_Application_ID).ToList();
                     var exIds = tempResults.Select(s => s.Expenses_Application_ID).ToList();
                     tsexs = tsexs.Where(w =>
                        (!w.Expenses_Application.Supervisor.HasValue && !w.Expenses_Application.Request_ID.HasValue && !w.Expenses_Application.Request_Cancel_ID.HasValue && w.Employee_Profile_ID == criteria.Employee_Profile_ID.Value ? true : false) ||
                        (exIds.Contains(w.Expenses_Application_ID) && !exIdsOut.Contains(w.Expenses_Application_ID) ? true : false));

                  }
                  else if (criteria.Tab_Pending)
                  {
                     tsexs = tsexs.Where(w =>
                        (string.IsNullOrEmpty(w.Expenses_Application.Cancel_Status) && (w.Expenses_Application.Overall_Status != WorkflowStatus.Closed || w.Expenses_Application.Overall_Status != WorkflowStatus.Rejected) ? true : false ||
                        !string.IsNullOrEmpty(w.Expenses_Application.Cancel_Status) && (w.Expenses_Application.Cancel_Status != WorkflowStatus.Cancellation_Rejected || w.Expenses_Application.Cancel_Status != WorkflowStatus.Cancelled) ? true : false)
                        ? true : false
                        );

                     var tempResults = tsexs.Where(w => (!string.IsNullOrEmpty(w.Expenses_Application.Next_Approver) && w.Expenses_Application.Next_Approver.Contains("|" + ProfileIDStr + "|")));
                     var exIds = tempResults.Select(s => s.Expenses_Application_ID).ToList();
                     tsexs = tsexs.Where(w => exIds.Contains(w.Expenses_Application_ID));
                  }
               }
            }
            else
            {
               if (criteria.Profile_ID.HasValue)
                  tsexs = tsexs.Where(w => w.Employee_Profile.Profile_ID == criteria.Profile_ID);

               if (criteria.Employee_Profile_ID.HasValue)
                  tsexs = tsexs.Where(w => w.Employee_Profile_ID == criteria.Employee_Profile_ID);

            }

            if (criteria.Include_Draft)
            {
            }
            else
               tsexs = tsexs.Where(w => w.Expenses_Application.Overall_Status != WorkflowStatus.Draft);

            tsexs = tsexs.OrderByDescending(o => o.Year).ThenBy(o => o.Month).ThenByDescending(o => o.Employee_Profile_ID);

            result.Record_Count = tsexs.Count();
            criteria.Record_Count = result.Record_Count;
            if (result.Record_Count > 300 && criteria.Start_Index == 0 && criteria.Page_Size == 0)
               criteria.Page_Size = 30;

            if (criteria.Top.HasValue)
               tsexs = tsexs.Take(criteria.Top.Value);

            else if (criteria.End_Index > 0)
               tsexs = tsexs.Skip(criteria.Start_Index).Take(criteria.End_Index);

            else if (criteria.Page_Size > 0)
            {
               if (criteria.Page_No > 1)
               {
                  var startindex = criteria.Page_Size * (criteria.Page_No - 1);
                  tsexs = tsexs.Skip(startindex).Take(criteria.Page_Size);
               }
               else
                  tsexs = tsexs.Skip(criteria.Start_Index).Take(criteria.Page_Size);
            }

            var obj = new List<TsEX>();
            obj = tsexs.ToList();

            result.Object = obj;
            result.Start_Index = criteria.Start_Index;
            result.Page_Size = criteria.Page_Size;

            return result;
         }
      }
   }

}
