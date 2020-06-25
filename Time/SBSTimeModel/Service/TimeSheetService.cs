using SBSModel.Common;
using SBSResourceAPI;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Core.Objects;
using System.Data.Entity.SqlServer;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SBSTimeModel.Models;
using SBSWorkFlowAPI.Constants;

namespace SBSModel.Models
{
   public class TimeSheetCriteria : CriteriaBase
   {
      public Nullable<int> Time_Sheet_ID { get; set; }
      public Nullable<int> Employee_Profile_ID { get; set; }
      public string Overall_Status { get; set; }
      public string Date_From { get; set; }
      public string Date_To { get; set; }
      public bool ValidateDup { get; set; }
      public string Date_Of_Date { get; set; }

      //use in action Delete, Active, Inactive (Job Cost list)
      public Nullable<int> Job_Cost_ID { get; set; }
      public bool Closed_Status { get; set; }

      public Nullable<int> Month { get; set; }
      public Nullable<int> Year { get; set; }
      public Nullable<bool> IncludeDraft { get; set; }


   }
   public class TimeSheetService : ServiceBase
   {
      public TimeSheetService()
      {

      }

      public TimeSheetService(User_Profile userlogin)
         : base(userlogin)
      {

      }

      public Time_Sheet GetTimeSheet(Nullable<int> pTimeSheetID)
      {

         using (var db = new SBS2TimeDBContext())
         {
            return db.Time_Sheet.Where(w => w.Time_Sheet_ID == pTimeSheetID).FirstOrDefault();
         }

      }

      public List<Time_Sheet> LstTimeSheet(TimeSheetCriteria criteria)
      {
         using (var db = new SBS2TimeDBContext())
         {

            var timeSheet = db.Time_Sheet
              .Where(w => w.Company_ID == criteria.Company_ID && w.Record_Status != RecordStatus.Delete);

            if (criteria != null)
            {
               if (criteria.Time_Sheet_ID.HasValue && criteria.Time_Sheet_ID.Value > 0)
                  timeSheet = timeSheet.Where(w => w.Time_Sheet_ID == criteria.Time_Sheet_ID.Value);
               if (criteria.Employee_Profile_ID.HasValue && criteria.Employee_Profile_ID.Value > 0)
                  timeSheet = timeSheet.Where(w => w.Employee_Profile_ID == criteria.Employee_Profile_ID);
               if (!string.IsNullOrEmpty(criteria.Date_Of_Date))
               {
                  var tempdate = DateUtil.ToDate(criteria.Date_Of_Date);
                  if (tempdate != null)
                     timeSheet = timeSheet.Where(w => w.Date_Of_Date.Value == tempdate);
               }
               if (!string.IsNullOrEmpty(criteria.Date_From))
               {
                  var tempdate = DateUtil.ToDate(criteria.Date_From);
                  if (tempdate != null)
                     timeSheet = timeSheet.Where(w => w.Date_Of_Date >= tempdate);
               }
               if (!string.IsNullOrEmpty(criteria.Date_To))
               {
                  var tempdate = DateUtil.ToDate(criteria.Date_To);
                  if (tempdate != null)
                     timeSheet = timeSheet.Where(w => w.Date_Of_Date <= tempdate);
               }
               if (!string.IsNullOrEmpty(criteria.Overall_Status))
               {
                  if (criteria.Overall_Status == SBSWorkFlowAPI.Constants.WorkflowStatus.Pending)
                     timeSheet = timeSheet.Where(w => w.Overall_Status == SBSWorkFlowAPI.Constants.WorkflowStatus.Pending | w.Overall_Status == SBSWorkFlowAPI.Constants.WorkflowStatus.Approved);
                  else
                     timeSheet = timeSheet.Where(w => w.Overall_Status == criteria.Overall_Status);
               }

               if (criteria.ValidateDup)
               {
                  //var t = timeSheet.ToList().Count();
                  //กรณีอนุมัติเเล้วเเละยกเลิกเเล้ว จะไม่นำมาใช้ในการหาค่าซ้ำ
                  timeSheet = timeSheet.Where(w => (w.Overall_Status == SBSWorkFlowAPI.Constants.WorkflowStatus.Closed && w.Cancel_Status != SBSWorkFlowAPI.Constants.WorkflowStatus.Cancelled) || (w.Overall_Status == SBSWorkFlowAPI.Constants.WorkflowStatus.Pending && w.Cancel_Status != SBSWorkFlowAPI.Constants.WorkflowStatus.Cancelled));
                  //t = timeSheet.ToList().Count();
               }

               if (criteria.Closed_Status)
               {
                  timeSheet = timeSheet.Where(w =>
                     w.Overall_Status == SBSWorkFlowAPI.Constants.WorkflowStatus.Closed &&
                     w.Cancel_Status != SBSWorkFlowAPI.Constants.WorkflowStatus.Cancelled);
               }

               if (criteria.Job_Cost_ID.HasValue && criteria.Job_Cost_ID.Value > 0)
                  timeSheet = timeSheet.Where(w => w.Job_Cost_ID == criteria.Job_Cost_ID);

               if (criteria.Month.HasValue && criteria.Month.Value > 0)
                  timeSheet = timeSheet.Where(w => w.Date_Of_Date.Value.Month == criteria.Month);

               if (criteria.Year.HasValue && criteria.Year.Value > 0)
                  timeSheet = timeSheet.Where(w => w.Date_Of_Date.Value.Year == criteria.Year);

               #region Start Funtion Draft
               if (criteria.IncludeDraft.HasValue && criteria.IncludeDraft.Value)
               {

               }
               else
               {
                  timeSheet = timeSheet.Where(w => w.Overall_Status != WorkflowStatus.Draft);
               }
               #endregion
            }
            return timeSheet.OrderBy(o => o.Employee_Name).OrderByDescending(l => l.Date_Of_Date).ToList();
         }
      }


      public ServiceResult UpdateTimeSheet(Time_Sheet pTimeSheet)
      {
         try
         {
            using (var db = new SBS2TimeDBContext())
            {
               var current = db.Time_Sheet.Where(w => w.Time_Sheet_ID == pTimeSheet.Time_Sheet_ID).FirstOrDefault();
               if (current == null)
                  return new ServiceResult { Code = ERROR_CODE.ERROR_511_DATA_NOT_FOUND, Msg = Resource.Time_Sheet + " " + Resource.Not_Found_Msg, Field = Resource.Time_Sheet };

               db.Entry(current).CurrentValues.SetValues(pTimeSheet);
               db.SaveChanges();
               return new ServiceResult() { Code = ERROR_CODE.SUCCESS, Msg = new Success().getSuccess(ERROR_CODE.SUCCESS_EDIT), Field = Resource.Time_Sheet };
            }

         }
         catch
         {
            return new ServiceResult() { Code = ERROR_CODE.ERROR_504_UPDATE_ERROR, Msg = new Error().getError(ERROR_CODE.ERROR_504_UPDATE_ERROR), Field = Resource.Time_Sheet };
         }
      }

      public ServiceResult InsertTimeSheet(Time_Sheet pTimeSheet)
      {
         try
         {
            using (var db = new SBS2TimeDBContext())
            {
               db.Time_Sheet.Add(pTimeSheet);
               db.SaveChanges();


               return new ServiceResult() { Code = ERROR_CODE.SUCCESS, Msg = new Success().getSuccess(ERROR_CODE.SUCCESS_CREATE), Field = Resource.Time_Sheet };
            }
         }
         catch
         {
            return new ServiceResult() { Code = ERROR_CODE.ERROR_503_INSERT_ERROR, Msg = new Error().getError(ERROR_CODE.ERROR_503_INSERT_ERROR), Field = Resource.Time_Sheet };
         }
      }

   }
}
