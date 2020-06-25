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

         using (var db = new SBS2DBContext())
         {
            return db.Time_Sheet.Include(i => i.Time_Sheet_Dtl).Where(w => w.Time_Sheet_ID == pTimeSheetID).FirstOrDefault();
         }

      }

      public List<Time_Sheet> LstTimeSheet(TimeSheetCriteria criteria)
      {
         using (var db = new SBS2DBContext())
         {

            var timeSheet = db.Time_Sheet
               .Include(i => i.Time_Sheet_Dtl)
               .Include(i => i.TsEXes)
              .Where(w => w.Company_ID == criteria.Company_ID);

            if (criteria != null)
            {
               if (criteria.Time_Sheet_ID.HasValue && criteria.Time_Sheet_ID.Value > 0)
                  timeSheet = timeSheet.Where(w => w.Time_Sheet_ID == criteria.Time_Sheet_ID.Value);
               if (criteria.Employee_Profile_ID.HasValue && criteria.Employee_Profile_ID.Value > 0)
                  timeSheet = timeSheet.Where(w => w.Employee_Profile_ID == criteria.Employee_Profile_ID);
               //if (!string.IsNullOrEmpty(criteria.Date_Of_Date))
               //{
               //   var tempdate = DateUtil.ToDate(criteria.Date_Of_Date);
               //   if (tempdate != null)
               //      timeSheet = timeSheet.Where(w => w.Time_Sheet_Dtl.Date_Of_Date.Value == tempdate);
               //}
               //if (!string.IsNullOrEmpty(criteria.Date_From))
               //{
               //   var tempdate = DateUtil.ToDate(criteria.Date_From);
               //   if (tempdate != null)
               //      timeSheet = timeSheet.Where(w => w.Date_Of_Date >= tempdate);
               //}
               //if (!string.IsNullOrEmpty(criteria.Date_To))
               //{
               //   var tempdate = DateUtil.ToDate(criteria.Date_To);
               //   if (tempdate != null)
               //      timeSheet = timeSheet.Where(w => w.Date_Of_Date <= tempdate);
               //}
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

               //if (criteria.Job_Cost_ID.HasValue && criteria.Job_Cost_ID.Value > 0)
               //   timeSheet = timeSheet.Where(w => w.Job_Cost_ID == criteria.Job_Cost_ID);

               if (criteria.Month.HasValue && criteria.Month.Value > 0)
                  timeSheet = timeSheet.Where(w => w.TsEXes.Where(w2 => w2.Month == criteria.Month).FirstOrDefault() != null);

               if (criteria.Year.HasValue && criteria.Year.Value > 0)
                  timeSheet = timeSheet.Where(w => w.TsEXes.Where(w2 => w2.Year == criteria.Year).FirstOrDefault() != null);

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
            return timeSheet.OrderBy(o => o.Employee_Name).OrderByDescending(o => o.TsEXes.FirstOrDefault().Year).OrderByDescending(o => o.TsEXes.FirstOrDefault().Month).ToList();
         }
      }

      public List<Time_Sheet_Dtl> LstTimeSheetDtl(TimeSheetCriteria criteria)
      {
         using (var db = new SBS2DBContext())
         {

            var timeSheetDtl = db.Time_Sheet_Dtl
               .Include(i => i.Time_Sheet)
              .Where(w => w.Time_Sheet.Company_ID == criteria.Company_ID);

            if (criteria != null)
            {
               if (criteria.Job_Cost_ID.HasValue && criteria.Job_Cost_ID.Value > 0)
                  timeSheetDtl = timeSheetDtl.Where(w => w.Job_Cost_ID == criteria.Job_Cost_ID);

               if (criteria.Closed_Status)
               {
                  timeSheetDtl = timeSheetDtl.Where(w =>
                     w.Time_Sheet.Overall_Status == SBSWorkFlowAPI.Constants.WorkflowStatus.Closed &&
                     w.Time_Sheet.Cancel_Status != SBSWorkFlowAPI.Constants.WorkflowStatus.Cancelled);
               }
            }
            return timeSheetDtl.ToList();
         }
      }
   }
}
