using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Web;
using SBSModel.Models;
using SBSModel.Common;
using SBSWorkFlowAPI.Models;
using System.Diagnostics;
using System.Data.Entity.SqlServer;
using SBSResourceAPI;


namespace SBSModel.Models
{
   public class Leave_Left
   {
      public decimal Left { get; set; } /* Remianing leave*/
      public decimal Entitle { get; set; }/* Entile Per Workdays (accumulate)*/
      public decimal EntitleAll { get; set; }/* Default entitle from leave config */
      public decimal Leave_Used { get; set; }
      public decimal BringForward { get; set; }/* Entile Per Workdays (accumulate)*/

      /*only Maternity Leave*/
      public decimal Weeks_Left { get; set; }
      public bool Is_First_Period { get; set; }
   }

   public class LeaveTypeCriteria : CriteriaBase
   {
      public Nullable<int> Leave_Config_ID { get; set; }
      public Nullable<int> Year { get; set; }
      public Nullable<int> Relationship_ID { get; set; }


      public Employment_History firsthist { get; set; }
      public int Year_Service { get; set; }
      public Nullable<DateTime> Hired_Date { get; set; }
      public Employee_Profile Emp { get; set; }
      public Leave_Config Leave_Config { get; set; }

      public bool Ignore_Generate { get; set; }
      public List<int> Leave_Config_IDs { get; set; }
      public bool isNew { get; set; }

   }

   public class Leave_Type
   {
      public Leave_Config Leave_Config { get; set; }
      public Leave_Left Leave_Left { get; set; }
   }

   public class _Leave_Config
   {
      public int Index { get; set; }
      public int[] Designations { get; set; }
      public Nullable<int> Leave_Config_ID { get; set; }

      public Nullable<decimal> Default_Leave_Amount { get; set; }

      public Nullable<int> Year_Service { get; set; }
      public Nullable<int> Group_ID { get; set; }
      public string Row_Type { get; set; }
   }

   public class LeaveService
   {
      public List<User_Profile> getEmployeeList(Nullable<int> pCompanyID, Nullable<int> pDepartmentID = null)
      {
         List<User_Profile> employee = new List<User_Profile>();
         var currentdate = StoredProcedure.GetCurrentDate();
         try
         {
            using (var db = new SBS2DBContext())
            {
               var users = db.User_Profile
                   .Include(i => i.Employee_Profile.Select(s => s.Employment_History))
                   .Include(i => i.Employee_Profile.Select(s => s.Nationality))
                   .Where(w => w.Company_ID == pCompanyID && w.User_Status == "Active");

               employee = users.OrderBy(o => o.Name).ToList();

            }
         }
         catch
         {

         }

         return employee;

      }

      //Added By sun 
      public List<User_Profile> getEmployeeListAll(Nullable<int> pCompanyID, Nullable<int> pDepartmentID = null)
      {
         List<User_Profile> employee = new List<User_Profile>();
         var currentdate = StoredProcedure.GetCurrentDate();
         try
         {
            using (var db = new SBS2DBContext())
            {
               var users = db.User_Profile
                   .Include(i => i.Employee_Profile.Select(s => s.Employment_History))
                   .Include(i => i.Employee_Profile.Select(s => s.Nationality))
                   .Where(w => w.Company_ID == pCompanyID && w.User_Status != RecordStatus.Delete);

               employee = users.OrderBy(o => o.Name).ToList();

            }
         }
         catch
         {

         }
         return employee;
      }

      #region Holiday
      public List<Holiday_Config> getHolidays(Nullable<int> Company_ID, Nullable<int> year = null)
      {
         List<Holiday_Config> holiday = null;

         try
         {
            using (var db = new SBS2DBContext())
            {
               holiday = db.Holiday_Config.Where(w => w.Company_ID == Company_ID && w.Record_Status != RecordStatus.Delete).ToList();
               if (year.HasValue)
               {

                  holiday = holiday.Where(w => (w.Start_Date.HasValue ? w.Start_Date.Value.Year == year : true) | (w.End_Date.HasValue ? w.End_Date.Value.Year == year : true))
                      .OrderBy(o => o.Start_Date).ToList();
               }
            }
         }
         catch
         {

         }

         return holiday;
      }

      public Holiday_Config getHoliday(Nullable<int> pHolidayID, string pName = null, Nullable<int> year = null, Nullable<int> pCompanyID = null)
      {
         try
         {
            using (var db = new SBS2DBContext())
            {
               var holiday = db.Holiday_Config.Where(w => 1 == 1);
               if (pHolidayID.HasValue)
               {
                  holiday = holiday.Where(w => w.Holiday_ID == pHolidayID);
               }
               if (!string.IsNullOrEmpty(pName))
               {
                  holiday = holiday.Where(w => w.Name == pName);
               }
               if (year.HasValue)
               {
                  holiday = holiday.Where(w => (w.Start_Date.HasValue ? w.Start_Date.Value.Year == year : true) | (w.End_Date.HasValue ? w.End_Date.Value.Year == year : true));
               }
               if (pCompanyID.HasValue)
               {
                  holiday = holiday.Where(w => w.Company_ID == pCompanyID);
               }
               return holiday.FirstOrDefault();
            }
         }
         catch
         {

         }
         return null;
      }

      public ServiceResult InsertHoliday(Holiday_Config holiday)
      {
         try
         {
            using (var db = new SBS2DBContext())
            {
               //db.Holiday_Config.Add(holiday);
               db.Entry(holiday).State = EntityState.Added;
               db.SaveChanges();
               return new ServiceResult() { Code = ERROR_CODE.SUCCESS, Msg = new Success().getSuccess(ERROR_CODE.SUCCESS_CREATE), Field = Resource.Holiday };
            }
         }
         catch
         {
            return new ServiceResult() { Code = ERROR_CODE.ERROR_503_INSERT_ERROR, Msg = new Error().getError(ERROR_CODE.ERROR_503_INSERT_ERROR), Field = Resource.Holiday };
         }
      }

      public ServiceResult UpdateHoliday(Holiday_Config holiday)
      {
         try
         {
            using (var db = new SBS2DBContext())
            {
               db.Entry(holiday).State = EntityState.Modified;
               db.SaveChanges();
               return new ServiceResult() { Code = ERROR_CODE.SUCCESS, Msg = new Success().getSuccess(ERROR_CODE.SUCCESS_EDIT), Field = Resource.Holiday };

            }
         }
         catch
         {
            return new ServiceResult() { Code = ERROR_CODE.ERROR_504_UPDATE_ERROR, Msg = new Error().getError(ERROR_CODE.ERROR_504_UPDATE_ERROR), Field = Resource.Holiday };
         }
      }

      public ServiceResult DeleteHoliday(Nullable<int> Holiday_ID)
      {
         try
         {
            using (var db = new SBS2DBContext())
            {
               var current = db.Holiday_Config.Where(w => w.Holiday_ID == Holiday_ID).FirstOrDefault();
               if (current != null)
               {
                  db.Holiday_Config.Remove(current);
               }
               db.SaveChanges();
               return new ServiceResult() { Code = ERROR_CODE.SUCCESS, Msg = new Success().getSuccess(ERROR_CODE.SUCCESS_DELETE), Field = Resource.Holiday };
            }
         }
         catch
         {
            return new ServiceResult() { Code = ERROR_CODE.ERROR_505_DELETE_ERROR, Msg = new Error().getError(ERROR_CODE.ERROR_505_DELETE_ERROR), Field = Resource.Holiday };
         }
      }
      #endregion

      #region Leave Adjustment
      public List<Leave_Adjustment> getLeaveAdjustments(Nullable<int> Company_ID, Nullable<int> sYear = null, Nullable<int> sLeaveConfigID = null)
      {
         try
         {
            using (var db = new SBS2DBContext())
            {
               var leaveAdjustment = db.Leave_Adjustment
                    .Include(i => i.Employee_Profile)
                    .Include(i => i.Employee_Profile.User_Profile)
                    .Include(i => i.Leave_Config)
                    .Where(w => w.Company_ID == Company_ID && w.Record_Status != RecordStatus.Delete);

               if (sYear.HasValue && sYear.Value > 0)
               {
                  leaveAdjustment = leaveAdjustment.Where(w => w.Year_2 == sYear);
               }

               if (sLeaveConfigID.HasValue && sLeaveConfigID.Value > 0)
               {
                  leaveAdjustment = leaveAdjustment.Where(w => w.Leave_Config.Leave_Config_ID == sLeaveConfigID);
               }

               return leaveAdjustment.OrderBy(o => o.Year_2)
                   .ThenBy(o => o.Employee_Profile.User_Profile.Name)
                   .ThenBy(o => o.Leave_Config_ID).ToList();
            }

         }
         catch
         {

         }
         return new List<Leave_Adjustment>();

      }

      public Leave_Adjustment getLeaveAdjustment(int Adjustment_ID)
      {
         Leave_Adjustment leaveAdjustment = null;

         try
         {
            using (var db = new SBS2DBContext())
            {
               leaveAdjustment = db.Leave_Adjustment
                   .Include(i => i.Employee_Profile)
                   .Include(i => i.Employee_Profile.User_Profile)
                   .Include(i => i.Leave_Config)
                   .Where(w => w.Adjustment_ID == Adjustment_ID).FirstOrDefault();
            }
         }
         catch
         {

         }

         return leaveAdjustment;
      }

      public ServiceResult InsertLeaveAdjustment(Leave_Adjustment leave)
      {
         try
         {
            using (var db = new SBS2DBContext())
            {
               if (leave.Employee_Profile_ID == 0 || !leave.Employee_Profile_ID.HasValue)
                  leave.Employee_Profile_ID = null;

               var leaveconfig = db.Leave_Config.Where(w => w.Company_ID == leave.Company_ID & w.Leave_Config_ID == leave.Leave_Config_ID).FirstOrDefault();

               if (leaveconfig != null)
               {
                  db.Entry(leave).State = EntityState.Added;
                  db.SaveChanges();

               }
               return new ServiceResult() { Code = ERROR_CODE.SUCCESS, Msg = new Success().getSuccess(ERROR_CODE.SUCCESS_CREATE), Field = Resource.Leave_Adjustment };
            }
         }
         catch
         {
            return new ServiceResult() { Code = ERROR_CODE.ERROR_503_INSERT_ERROR, Msg = new Error().getError(ERROR_CODE.ERROR_503_INSERT_ERROR), Field = Resource.Leave_Adjustment };
         }
      }

      public ServiceResult UpdateLeaveAdjustment(Leave_Adjustment leave)
      {
         try
         {
            using (var db = new SBS2DBContext())
            {
               if (leave.Employee_Profile_ID == 0 || !leave.Employee_Profile_ID.HasValue)
                  leave.Employee_Profile_ID = null;

               var leaveconfig = db.Leave_Config.Where(w => w.Company_ID == leave.Company_ID & w.Leave_Config_ID == leave.Leave_Config_ID).FirstOrDefault();

               if (leaveconfig != null)
               {
                  db.Entry(leave).State = EntityState.Modified;
                  db.SaveChanges();
               }

               return new ServiceResult() { Code = ERROR_CODE.SUCCESS, Msg = new Success().getSuccess(ERROR_CODE.SUCCESS_EDIT), Field = Resource.Leave_Adjustment };
            }
         }
         catch
         {
            return new ServiceResult() { Code = ERROR_CODE.ERROR_504_UPDATE_ERROR, Msg = new Error().getError(ERROR_CODE.ERROR_504_UPDATE_ERROR), Field = Resource.Leave_Adjustment };
         }
      }

      public ServiceResult DeleteLeaveAdjustment(Nullable<int> aid)
      {
         try
         {
            using (var db = new SBS2DBContext())
            {
               var leave = db.Leave_Adjustment.Where(w => w.Adjustment_ID == aid).FirstOrDefault();
               if (leave != null)
               {
                  if (leave.Employee_Profile_ID == 0 || !leave.Employee_Profile_ID.HasValue)
                  {
                     //select All employee
                     var emps = (from a in db.Employee_Profile where a.User_Profile.Company_ID == leave.Company_ID select a);

                     foreach (var emp in emps)
                     {

                        var cal = (from a in db.Leave_Calculation
                                   where a.Employee_Profile_ID == emp.Employee_Profile_ID &
                                   a.Leave_Config_ID == leave.Leave_Config_ID &
                                   a.Year_Assigned == SqlFunctions.StringConvert((double)leave.Year_2).Trim()
                                   orderby a.Start_Date descending
                                   select a).FirstOrDefault();

                        if (cal != null)
                        {
                           cal.Adjustment = cal.Adjustment - leave.Adjustment_Amount;
                        }
                     }

                     leave.Employee_Profile_ID = null;
                  }
                  else
                  {
                     var cal = (from a in db.Leave_Calculation
                                where a.Employee_Profile_ID == leave.Employee_Profile_ID &
                                a.Leave_Config_ID == leave.Leave_Config_ID &
                                a.Year_Assigned == SqlFunctions.StringConvert((double)leave.Year_2).Trim()
                                orderby a.Start_Date descending
                                select a).FirstOrDefault();

                     if (cal != null)
                     {
                        cal.Adjustment = cal.Adjustment - leave.Adjustment_Amount;
                     }
                  }

                  db.Entry(leave).State = EntityState.Deleted;
                  db.SaveChanges();
               }
               return new ServiceResult() { Code = ERROR_CODE.SUCCESS, Msg = new Success().getSuccess(ERROR_CODE.SUCCESS_DELETE), Field = Resource.Leave_Adjustment };
            }
         }
         catch
         {
            return new ServiceResult() { Code = ERROR_CODE.ERROR_505_DELETE_ERROR, Msg = new Error().getError(ERROR_CODE.ERROR_505_DELETE_ERROR), Field = Resource.Leave_Adjustment };
         }
      }

      #endregion

      #region Leave Type
      public bool chkLeaveTypeApplied(Nullable<int> pLeave_Config_ID)
      {
         var chkValue = false;
         try
         {
            using (var db = new SBS2DBContext())
            {
               var appliedLeaveCnt = db.Leave_Application_Document.Where(w => w.Leave_Config_ID == pLeave_Config_ID).ToList();
               if (appliedLeaveCnt.Count > 0)
                  chkValue = true;
               return chkValue;
            }
         }
         catch
         {
            return false;
         }
      }
      public List<Leave_Config> LstLeaveType(Nullable<int> Company_ID, Nullable<int> pLeaveConfigID = null, string pLeaveName = "")
      {
         List<Leave_Config> leaveType = null;
         try
         {
            using (var db = new SBS2DBContext())
            {
               leaveType = db.Leave_Config
                   .Include(i => i.Leave_Config_Detail)
                   .Include(i => i.Leave_Config_Child_Detail)
                   .Include(i => i.Leave_Config_Condition)
                   .Include(i => i.Leave_Config_Condition.Select(s => s.Global_Lookup_Data))
                   .Where(w => w.Company_ID == Company_ID && w.Record_Status != RecordStatus.Delete)
                   .OrderBy(o => o.Leave_Name)
                   .ToList();

               if (pLeaveConfigID.HasValue)
                  leaveType = leaveType.Where(w => w.Leave_Config_ID == pLeaveConfigID).ToList();
               if (!string.IsNullOrEmpty(pLeaveName))
                  leaveType = leaveType.Where(w => w.Leave_Name == pLeaveName).ToList();
            }
         }
         catch
         {

         }
         return leaveType;
      }
      public Leave_Config getLeaveType(int Leave_Config_ID)
      {
         Leave_Config leaveType = null;

         try
         {
            using (var db = new SBS2DBContext())
            {
               leaveType = db.Leave_Config
                   .Where(w => w.Leave_Config_ID == Leave_Config_ID)
                   .Include(i => i.Leave_Config_Detail)
                   .Include(i => i.Leave_Config_Detail.Select(s => s.Designation))
                   .Include(i => i.Leave_Config_Extra)
                   .Include(i => i.Leave_Config_Condition)
                   .Include(i => i.Leave_Config_Condition.Select(s => s.Global_Lookup_Data))
                   .Include(i => i.Leave_Config_Child_Detail)
                   .FirstOrDefault();
            }
         }
         catch
         {

         }

         return leaveType;
      }
      public ServiceResult InsertLeaveType(Leave_Config leave, List<_Leave_Config> details, int[] conditions)
      {
         try
         {
            using (var db = new SBS2DBContext())
            {
               //---------allowance----------
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

                           var leaveConfigDetail = new Leave_Config_Detail()
                           {
                              Default_Leave_Amount = row.Default_Leave_Amount,
                              Designation_ID = designationID,
                              Group_ID = gcnt,
                              Year_Service = row.Year_Service
                           };
                           leave.Leave_Config_Detail.Add(leaveConfigDetail);


                        }

                        gcnt++;
                     }
                     i++;

                  }
               }

               if (conditions != null)
               {
                  foreach (var crow in conditions)
                  {
                     var lcon = new Leave_Config_Condition()
                     {
                        Lookup_Data_ID = crow
                     };
                     leave.Leave_Config_Condition.Add(lcon);
                  }
               }

               db.Entry(leave).State = EntityState.Added;
               db.SaveChanges();
               return new ServiceResult() { Code = ERROR_CODE.SUCCESS, Msg = new Success().getSuccess(ERROR_CODE.SUCCESS_CREATE), Field = Resource.Leave_Type };
            }
         }
         catch
         {
            return new ServiceResult() { Code = ERROR_CODE.ERROR_503_INSERT_ERROR, Msg = new Error().getError(ERROR_CODE.ERROR_503_INSERT_ERROR), Field = Resource.Leave_Type };
         }
      }
      public ServiceResult UpdateLeaveTypeDefault(Leave_Config leave)
      {
         try
         {
            using (var db = new SBS2DBContext())
            {
               var current = db.Leave_Config.Where(w => w.Leave_Config_ID == leave.Leave_Config_ID).FirstOrDefault();

               if (current == null)
                  return new ServiceResult() { Code = ERROR_CODE.ERROR_511_DATA_NOT_FOUND, Msg = new Error().getError(ERROR_CODE.ERROR_511_DATA_NOT_FOUND), Field = Resource.Leave_Type };

               var lcons = db.Leave_Config_Condition.Where(w => w.Leave_Config_ID == leave.Leave_Config_ID);
               db.Leave_Config_Condition.RemoveRange(lcons);

               db.Leave_Config_Condition.AddRange(leave.Leave_Config_Condition);

               if (leave.Leave_Config_Child_Detail != null)
               {
                  foreach (var detail in leave.Leave_Config_Child_Detail)
                  {
                     var dcurrent = db.Leave_Config_Child_Detail.Where(w => w.Leave_Config_Child_Detail_ID == detail.Leave_Config_Child_Detail_ID & w.Residential_Status == detail.Residential_Status).FirstOrDefault();
                     if (detail.Leave_Config_Child_Detail_ID > 0)
                     {
                        // update
                        db.Entry(dcurrent).CurrentValues.SetValues(detail);
                     }
                     else
                     {
                        //inesert
                        db.Leave_Config_Child_Detail.Add(detail);
                     }
                  }
               }


               db.Entry(current).CurrentValues.SetValues(leave);
               db.SaveChanges();
               return new ServiceResult() { Code = ERROR_CODE.SUCCESS, Msg = new Success().getSuccess(ERROR_CODE.SUCCESS_EDIT), Field = Resource.Leave_Type };
            }


         }
         catch
         {
            return new ServiceResult() { Code = ERROR_CODE.ERROR_504_UPDATE_ERROR, Msg = new Error().getError(ERROR_CODE.ERROR_504_UPDATE_ERROR), Field = Resource.Leave_Type };
         }
      }
      public ServiceResult UpdateLeaveType(Leave_Config leave, List<_Leave_Config> details, int[] conditions)
      {
         try
         {
            using (var db = new SBS2DBContext())
            {
               var current = db.Leave_Config.Where(w => w.Leave_Config_ID == leave.Leave_Config_ID).FirstOrDefault();
               if (current != null)
               {

                  if (details != null)
                  {
                     var i = 0;
                     var gcnt = 1;
                     foreach (var row in details)
                     {
                        row.Leave_Config_ID = leave.Leave_Config_ID;
                        if (row.Row_Type == RowType.ADD)
                        {

                           foreach (var ldrow in row.Designations)
                           {
                              Nullable<int> designationID = ldrow;
                              if (ldrow == 0)
                                 designationID = null;

                              var leaveConfigDetail = new Leave_Config_Detail()
                              {
                                 Default_Leave_Amount = row.Default_Leave_Amount,
                                 Designation_ID = designationID,
                                 Group_ID = gcnt,
                                 Leave_Config_ID = row.Leave_Config_ID,
                                 Year_Service = row.Year_Service
                              };
                              db.Leave_Config_Detail.Add(leaveConfigDetail);
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
                                    designationID = null;

                                 var currentdetail = (from a in db.Leave_Config_Detail
                                                      where a.Group_ID == row.Group_ID
                                                      && a.Leave_Config_ID == row.Leave_Config_ID
                                                      && a.Designation_ID == designationID
                                                      select a).FirstOrDefault();

                                 if (currentdetail == null)
                                 {

                                    var leaveConfigDetail = new Leave_Config_Detail()
                                    {
                                       Default_Leave_Amount = row.Default_Leave_Amount,
                                       Designation_ID = designationID,
                                       Group_ID = gcnt,
                                       Leave_Config_ID = row.Leave_Config_ID,
                                       Year_Service = row.Year_Service
                                    };
                                    db.Leave_Config_Detail.Add(leaveConfigDetail);

                                 }
                                 else
                                 {
                                    currentdetail.Default_Leave_Amount = row.Default_Leave_Amount;
                                    currentdetail.Year_Service = row.Year_Service;
                                    currentdetail.Designation_ID = designationID;
                                    currentdetail.Group_ID = gcnt;
                                 }
                              }

                              var notUseRows = (from a in db.Leave_Config_Detail
                                                where a.Group_ID == row.Group_ID && a.Leave_Config_ID == row.Leave_Config_ID
                                                && !row.Designations.Contains(a.Designation_ID.HasValue ? a.Designation_ID.Value : 0)
                                                select a);
                              db.Leave_Config_Detail.RemoveRange(notUseRows);
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

                                 var currentdetail = (from a in db.Leave_Config_Detail
                                                      where a.Group_ID == row.Group_ID
                                                      && a.Leave_Config_ID == row.Leave_Config_ID
                                                      && a.Designation_ID == designationID
                                                      select a).FirstOrDefault();
                                 if (currentdetail != null)
                                 {
                                    db.Leave_Config_Detail.Remove(currentdetail);
                                 }
                              }
                           }


                        }
                        i++;
                     }
                  }

                  var lcons = db.Leave_Config_Condition.Where(w => w.Leave_Config_ID == leave.Leave_Config_ID);
                  db.Leave_Config_Condition.RemoveRange(lcons);

                  if (conditions != null)
                  {
                     foreach (var crow in conditions)
                     {
                        var lcon = new Leave_Config_Condition()
                        {
                           Lookup_Data_ID = crow,
                           Leave_Config_ID = leave.Leave_Config_ID
                        };
                        db.Leave_Config_Condition.Add(lcon);
                     }
                  }


                  if (leave.Leave_Config_Extra == null)
                  {
                     db.Leave_Config_Extra.RemoveRange(current.Leave_Config_Extra);
                  }
                  else
                  {
                     var currentExtraIDs = current.Leave_Config_Extra.Select(s => s.Leave_Config_Extra_ID).ToArray();
                     foreach (var extraID in currentExtraIDs)
                     {
                        if (!leave.Leave_Config_Extra.Select(s => s.Leave_Config_Extra_ID).Contains(extraID))
                        {
                           var currentextra = db.Leave_Config_Extra.Where(w => w.Leave_Config_Extra_ID == extraID).FirstOrDefault();
                           db.Leave_Config_Extra.Remove(currentextra);
                        }
                     }
                     foreach (var extra in leave.Leave_Config_Extra)
                     {
                        if (extra.Leave_Config_Extra_ID == 0)
                        {
                           db.Leave_Config_Extra.Add(extra);
                        }
                        else
                        {
                           var currentextra = db.Leave_Config_Extra.Where(w => w.Leave_Config_Extra_ID == extra.Leave_Config_Extra_ID).FirstOrDefault();
                           db.Entry(currentextra).CurrentValues.SetValues(extra);
                        }
                     }
                  }
                  db.Entry(current).CurrentValues.SetValues(leave);
                  db.SaveChanges();
               }

               return new ServiceResult() { Code = ERROR_CODE.SUCCESS, Msg = new Success().getSuccess(ERROR_CODE.SUCCESS_EDIT), Field = Resource.Leave_Type };
            }
         }
         catch
         {
            return new ServiceResult() { Code = ERROR_CODE.ERROR_504_UPDATE_ERROR, Msg = new Error().getError(ERROR_CODE.ERROR_504_UPDATE_ERROR), Field = Resource.Leave_Type };
         }
      }
      public ServiceResult DeleteLeaveType(Nullable<int> Leave_Config_ID)
      {
         try
         {
            using (var db = new SBS2DBContext())
            {
               var leave = db.Leave_Config.Where(w => w.Leave_Config_ID == Leave_Config_ID).FirstOrDefault();

               if (leave != null)
               {
                  //Added By sun 10-09-2015
                  var CurrentDoc = db.Leave_Application_Document.Where(w => w.Leave_Config_ID == leave.Leave_Config_ID).FirstOrDefault();
                  var CurrentAdj = db.Leave_Adjustment.Where(w => w.Leave_Config_ID == leave.Leave_Config_ID).FirstOrDefault();

                  if (CurrentDoc != null || CurrentAdj != null)
                  {
                     return new ServiceResult() { Code = ERROR_CODE.ERROR_505_DELETE_ERROR, Msg = new Error().getError(ERROR_CODE.ERROR_505_DELETE_ERROR), Field = Resource.Leave_Type };
                  }

                  db.Leave_Adjustment.RemoveRange(leave.Leave_Adjustment);

                  db.Leave_Config_Detail.RemoveRange(leave.Leave_Config_Detail);

                  db.Leave_Calculation.RemoveRange(leave.Leave_Calculation);

                  db.Leave_Application_Document.RemoveRange(leave.Leave_Application_Document);

                  db.Leave_Config_Condition.RemoveRange(leave.Leave_Config_Condition);

                  db.Leave_Config_Child_Detail.RemoveRange(leave.Leave_Config_Child_Detail);

                  db.Leave_Config_Extra.RemoveRange(leave.Leave_Config_Extra);

                  db.Leave_Config.Remove(leave);

                  db.SaveChanges();
               }

               return new ServiceResult() { Code = ERROR_CODE.SUCCESS, Msg = new Success().getSuccess(ERROR_CODE.SUCCESS_DELETE), Field = Resource.Leave_Type };
            }
         }
         catch
         {
            return new ServiceResult() { Code = ERROR_CODE.ERROR_505_DELETE_ERROR, Msg = new Error().getError(ERROR_CODE.ERROR_505_DELETE_ERROR), Field = Resource.Leave_Type };
         }
      }
      public List<Leave_Config_Detail> LstConfigDetail(Nullable<int> pID = null)
      {
         using (var db = new SBS2DBContext())
         {
            return db.Leave_Config_Detail.Include(i => i.Designation).Where(w => w.Leave_Config_ID == pID).ToList();
         }
      }
      public List<Leave_Type> LstAndCalulateLeaveType(LeaveTypeCriteria cri)
      {
         // return leave type global data
         var currentdate = StoredProcedure.GetCurrentDate();
         var leavetypelist = new List<Leave_Type>();

         using (var db = new SBS2DBContext())
         {
            if (cri.Company_ID.HasValue)
            {
               if (cri.Profile_ID.HasValue)
               {
                  if (cri.Emp == null)
                     cri.Emp = db.Employee_Profile.Where(w => w.Profile_ID == cri.Profile_ID).FirstOrDefault();

                  var userhist = cri.Emp.Employment_History.FirstOrDefault();
                  if (cri.Emp == null && userhist == null)
                     return leavetypelist;

                  var gender = db.Global_Lookup_Data.Where(w => w.Lookup_Data_ID == cri.Emp.Gender).FirstOrDefault();
                  var mStatus = db.Global_Lookup_Data.Where(w => w.Lookup_Data_ID == cri.Emp.Marital_Status).FirstOrDefault();

                  if (gender == null || mStatus == null)
                     return leavetypelist;

                  if (cri.firsthist == null)
                     cri.firsthist = cri.Emp.Employment_History.Where(w => w.Employee_Profile.Profile_ID == cri.Profile_ID).OrderBy(o => o.Effective_Date).FirstOrDefault();

                  cri.Hired_Date = currentdate;
                  if (cri.firsthist != null && cri.firsthist.Effective_Date.HasValue)
                     cri.Hired_Date = cri.firsthist.Effective_Date.Value;

                  var workspan = (currentdate.Date - cri.Hired_Date.Value.Date);
                  cri.Year_Service = NumUtil.ParseInteger(Math.Floor((workspan.TotalDays + 1) / 365));

                  var leavetypes = db.Leave_Config
                      .Include(i => i.Leave_Config_Extra)
                      .Include(i => i.Leave_Config_Detail)
                      .Include(i => i.Leave_Config_Condition)
                      .Include(i => i.Leave_Config_Child_Detail)
                      .Where(w => w.Company_ID == cri.Company_ID && w.Record_Status != RecordStatus.Delete);
                  if (cri.Leave_Config_IDs != null && cri.Leave_Config_IDs.Count() > 0)
                  {
                     leavetypes = leavetypes.Where(w => cri.Leave_Config_IDs.Contains(w.Leave_Config_ID));
                  }
                  foreach (var ltype in leavetypes)
                  {
                     cri.Leave_Config_ID = ltype.Leave_Config_ID;
                     cri.Leave_Config = ltype;
                     if (!cri.Ignore_Generate)
                     {
                        var result = CalculateLeave(cri);
                        if (result.Code != ERROR_CODE.SUCCESS)
                           continue;
                     }

                     cri.Leave_Config = ltype;
                     var lname = ltype.Leave_Name;
                     var valid = false;
                     var havegender = ltype.Leave_Config_Condition.Where(w => w.Global_Lookup_Data.Name == gender.Name).FirstOrDefault();
                     var havemStatusr = ltype.Leave_Config_Condition.Where(w => w.Global_Lookup_Data.Name == mStatus.Name).FirstOrDefault();
                     if (havegender == null || havemStatusr == null)
                        continue;

                     if (ltype.Leave_Name == LeaveType.AdoptionLeave)
                     {
                        var child = cri.Emp.Relationships.Where(w => w.Employee_Profile_ID == cri.Emp.Employee_Profile_ID).OrderByDescending(o => o.DOB).FirstOrDefault();
                        if (child != null)
                        {
                           if (child.Child_Type == ChildType.AdoptedChild)
                              valid = true;
                        }

                        if (valid)
                           leavetypelist.Add(new Leave_Type { Leave_Config = ltype, Leave_Left = new Leave_Left() });
                     }
                     else if (ltype.Leave_Name == LeaveType.MaternityLeave)
                     {
                        var left = new Leave_Left();

                        var lconfigDetail = ltype.Leave_Config_Child_Detail.Where(w => cri.Emp.Residential_Status == w.Residential_Status).FirstOrDefault();
                        if (lconfigDetail == null)
                           continue;

                        var child = cri.Emp.Relationships.Where(w => w.Employee_Profile_ID == cri.Emp.Employee_Profile_ID).OrderByDescending(o => o.DOB).FirstOrDefault();
                        if (child == null)
                           continue;

                        if (child.Child_Type != ChildType.OwnChild)
                           continue;

                        var dob = child.DOB.Value;
                        var validperiod = ltype.Valid_Period.HasValue ? ltype.Valid_Period.Value : 0;
                        var cals = db.Leave_Calculation.Where(w => w.Employee_Profile_ID == cri.Emp.Employee_Profile_ID & w.Leave_Config_ID == ltype.Leave_Config_ID & w.Relationship_ID == child.Relationship_ID);
                        var used = 0M;
                        var entitle = lconfigDetail.Default_Leave_Amount;

                        if (child.Is_Maternity_Share_Father.HasValue && child.Is_Maternity_Share_Father.Value)
                           entitle -= 1;

                        foreach (var cal in cals)
                           used += cal.Leave_Used.HasValue ? cal.Leave_Used.Value : 0;

                        if (used > 0)
                        {
                           if (ltype.Flexibly.HasValue && ltype.Flexibly.Value)
                           {
                              // validate period after 1st application date (have only flexible and second period)
                              var leaves = db.Leave_Application_Document.Where(w => w.Employee_Profile_ID == cri.Emp.Employee_Profile_ID & w.Relationship_ID == child.Relationship_ID);
                              if (leaves.Count() == 1)
                              {
                                 var firstperiod = leaves.FirstOrDefault();
                                 var enddate = firstperiod.End_Date.Value;
                                 if (enddate.AddMonths(validperiod) >= currentdate)
                                 {
                                    if (used < entitle)
                                       valid = true;
                                 }
                              }
                           }
                           left.Is_First_Period = true;
                        }
                        else
                           valid = true;

                        left.Entitle = entitle.HasValue ? entitle.Value : 0;
                        left.Weeks_Left = left.Entitle - used;
                        left.EntitleAll = left.Entitle;

                        if (valid)
                           leavetypelist.Add(new Leave_Type { Leave_Config = ltype, Leave_Left = left });
                     }
                     else if (ltype.Leave_Name == LeaveType.PaternityLeave)
                     {
                        // share 1 week to father.
                        var left = new Leave_Left();

                        var lconfigDetail = ltype.Leave_Config_Child_Detail.Where(w => cri.Emp.Residential_Status == w.Residential_Status).FirstOrDefault();
                        if (lconfigDetail == null)
                           continue;

                        var child = cri.Emp.Relationships.Where(w => w.Employee_Profile_ID == cri.Emp.Employee_Profile_ID).OrderByDescending(o => o.DOB).FirstOrDefault();
                        if (child == null)
                           continue;

                        if (child.Is_Maternity_Share_Father.HasValue && child.Is_Maternity_Share_Father.Value)
                        {
                           var entitle = lconfigDetail.Default_Leave_Amount;
                           left.Entitle = entitle.HasValue ? entitle.Value : 0;
                           left.EntitleAll = left.Entitle;

                           var used = 0M;
                           var cals = db.Leave_Calculation.Where(w => w.Employee_Profile_ID == cri.Emp.Employee_Profile_ID & w.Leave_Config_ID == ltype.Leave_Config_ID & w.Relationship_ID == child.Relationship_ID);
                           foreach (var cal in cals)
                              used += cal.Leave_Used.HasValue ? cal.Leave_Used.Value : 0;

                           if (used == 0)
                           {
                              valid = true;
                              left.Weeks_Left = left.Entitle;
                           }
                        }

                        if (valid)
                           leavetypelist.Add(new Leave_Type { Leave_Config = ltype, Leave_Left = left });
                     }
                     #region Temp for delete comment by sun 08-03-2017
                     //else if (ltype.Leave_Name == LeaveType.ChildCareLeave)
                     //{
                     //   
                     //   var left = new Leave_Left();
                     //   var child = cri.Emp.Relationships.Where(w => w.Employee_Profile_ID == cri.Emp.Employee_Profile_ID).OrderByDescending(o => o.DOB).FirstOrDefault();

                     //   if (child != null)
                     //   {
                     //      if (child.Relationship1 != null)
                     //      {
                     //         var relationshipName = db.Global_Lookup_Data.Where(w => w.Lookup_Data_ID == child.Relationship1.Value).FirstOrDefault();
                     //         if (child.Child_Type == ChildType.OwnChild && (relationshipName.Description.ToUpper() == "SON" || relationshipName.Description.ToUpper() == "DAUGHTER"))
                     //         {
                     //            var tWorkExp = currentdate - userhist.Confirm_Date;
                     //            var workExp = Convert.ToDouble(tWorkExp.Value.TotalDays);
                     //            var t = currentdate - child.DOB;
                     //            var age = Convert.ToDouble(t.Value.TotalDays) / 365;
                     //            if ((age <= 12) && (child.Nationality.Name == "SG") && (workExp > 90))
                     //            {
                     //               var lconfigDetail = ltype.Leave_Config_Detail.Where(w => cri.Leave_Config_ID == w.Leave_Config_ID).FirstOrDefault();
                     //               var entitle = lconfigDetail.Default_Leave_Amount;
                     //               left.Entitle = entitle.HasValue ? entitle.Value : 0;
                     //               left.EntitleAll = left.Entitle;

                     //               if (age >= 7 && age <= 12)
                     //               {
                     //                  left.Entitle += 2;
                     //                  left.EntitleAll += 2;
                     //               }
                     //               var used = 0M;
                     //               var cals = db.Leave_Calculation.Where(w => w.Employee_Profile_ID == cri.Emp.Employee_Profile_ID & w.Leave_Config_ID == ltype.Leave_Config_ID & w.Relationship_ID == child.Relationship_ID);
                     //               foreach (var cal in cals)
                     //                  used += cal.Leave_Used.HasValue ? cal.Leave_Used.Value : 0;

                     //               if (used < left.Entitle)
                     //               {
                     //                  valid = true;
                     //                  left.Weeks_Left = left.Entitle;
                     //               }
                     //            }
                     //         }
                     //      }

                     //   }


                     //   if (valid)
                     //      leavetypelist.Add(new Leave_Type { Leave_Config = ltype, Leave_Left = left });
                     //}
                     #endregion
                     else
                     {
                        //Added by sun 08-03-2017
                        if (ltype.Leave_Name == LeaveType.ChildCareLeave)
                        {
                           Debug.WriteLine("'***********APP DEBUG***********' " + LeaveType.ChildCareLeave);
                           var filterOut = true;
                           var child = cri.Emp.Relationships.Where(w => w.Employee_Profile_ID == cri.Emp.Employee_Profile_ID).OrderByDescending(o => o.DOB).FirstOrDefault();
                           if (child != null)
                           {
                              if (child.Relationship1 != null)
                              {
                                 var relationshipName = db.Global_Lookup_Data.Where(w => w.Lookup_Data_ID == child.Relationship1.Value).FirstOrDefault();
                                 if (child.Child_Type == ChildType.OwnChild && (relationshipName.Description.ToUpper() == "SON" || relationshipName.Description.ToUpper() == "DAUGHTER"))
                                 {
                                    var tWorkExp = currentdate - userhist.Confirm_Date;
                                    var workExp = Convert.ToDouble(tWorkExp.Value.TotalDays);
                                    var t = currentdate - child.DOB;
                                    var age = Convert.ToDouble(t.Value.TotalDays) / 365;
                                    if ((age <= 12) && (child.Nationality.Name == "SG") && (workExp > 90))
                                    {
                                       filterOut = false;
                                    }
                                 }
                              }
                           }
                           if (filterOut)
                              continue;
                        }

                        var ldetails = ltype.Leave_Config_Detail.Where(w => w.Year_Service <= cri.Year_Service && (w.Designation_ID == null || w.Designation_ID == userhist.Designation_ID)).ToList();
                        if (ldetails.Count() == 0)
                           continue;

                        if (ltype.Allowed_Notice_Period.HasValue && ltype.Allowed_Notice_Period.Value)
                           valid = true;
                        else if (!userhist.Terminate_Date.HasValue)
                           valid = true;
                        else if (!userhist.Notice_Period_Amount.HasValue && userhist.Notice_Period_Amount.Value == 0)
                           valid = true;
                        else
                        {
                           int days = (int)userhist.Notice_Period_Amount.Value;
                           if (userhist.Notice_Period_Unit == TimePeriod.Weeks)
                              days = days * 7;
                           else if (userhist.Notice_Period_Unit == TimePeriod.Months)
                              days = days * 30;
                           else if (userhist.Notice_Period_Unit == TimePeriod.Years)
                              days = days * 365;

                           if (currentdate < userhist.Terminate_Date.Value.AddDays(-days))
                              valid = true;
                        }


                        if (valid)
                        {
                           if (ltype.Leave_Config_Parent_ID.HasValue)
                           {

                              var leaveleft = CalculateLeaveLeft(cri);
                              if (leaveleft != null)
                              {
                                 if (leaveleft.Left > 0.5M)
                                 {
                                    var parentcri = cri.Clone() as LeaveTypeCriteria;
                                    parentcri.Leave_Config_ID = ltype.Leave_Config_Parent_ID;
                                    parentcri.Leave_Config = null;
                                    var parentleaveleft = CalculateLeaveLeft(parentcri);
                                    if (parentleaveleft != null)
                                    {
                                       if (parentleaveleft.Left < 0.5M)
                                       {
                                          leaveleft.Left = leaveleft.Left - parentleaveleft.Leave_Used;
                                          leavetypelist.Add(new Leave_Type { Leave_Config = ltype, Leave_Left = leaveleft });
                                       }

                                    }
                                 }
                              }
                              else
                                 leavetypelist.Add(new Leave_Type { Leave_Config = ltype, Leave_Left = new Leave_Left() });
                           }
                           else
                           {
                              var leaveleft = CalculateLeaveLeft(cri);
                              if (leaveleft != null)
                              {
                                 if (leaveleft.Left > 0.5M | !cri.isNew)
                                    leavetypelist.Add(new Leave_Type { Leave_Config = ltype, Leave_Left = leaveleft });
                              }
                              else
                                 leavetypelist.Add(new Leave_Type { Leave_Config = ltype, Leave_Left = new Leave_Left() });
                           }

                        }

                     }


                  }
               }
            }
         }
         return leavetypelist;
      }
      public List<Leave_Config> LstLeaveConfig(Nullable<int> pCompanyID, Nullable<int> pDesignation = null, Nullable<int> pYearService = 0)
      {
         using (var db = new SBS2DBContext())
         {
            var lTypes = db.Leave_Config.Where(w => w.Company_ID == pCompanyID);
            if (pDesignation.HasValue)
               lTypes = lTypes.Where(w => w.Leave_Config_Detail.Where(w2 => w2.Designation_ID == pDesignation || w2.Designation_ID == null).Count() > 0);
            if (pYearService.HasValue)
               lTypes = lTypes.Where(w => w.Leave_Config_Detail.Where(w2 => w2.Year_Service <= pYearService).Count() > 0);
            return lTypes.Include(i => i.Leave_Config_Detail).Include(i => i.Leave_Config_Extra).ToList();
         }

      }
      public Leave_Default getLeaveDefaulType(int pDefaulID)
      {
         using (var db = new SBS2DBContext())
         {
            return db.Leave_Default
                .Where(w => w.Default_ID == pDefaulID)
                .Include(i => i.Leave_Default_Detail)
                .Include(i => i.Leave_Default_Condition)
                .Include(i => i.Leave_Default_Condition.Select(s => s.Global_Lookup_Data))
                .Include(i => i.Leave_Default_Child_Detail)
                .FirstOrDefault();
         }
      }

      //15-09-2015 Added by sun
      public List<Leave_Default> LstLeaveDefaul(string pLeaveDefault = "", string pLeaveDefaultType = "")
      {
         List<Leave_Default> leaveDefaulType = null;

         try
         {
            using (var db = new SBS2DBContext())
            {

               leaveDefaulType = db.Leave_Default
                      .Include(i => i.Leave_Default_Detail)
                      .Include(i => i.Leave_Default_Child_Detail)
                      .Include(i => i.Leave_Default_Condition)
                      .Include(i => i.Leave_Default_Condition.Select(s => s.Global_Lookup_Data))
                      .OrderBy(o => o.Leave_Name)
                      .Where(w => w.Record_Status != RecordStatus.Delete)
                      .ToList();

               if (!string.IsNullOrEmpty(pLeaveDefault))
               {
                  leaveDefaulType = leaveDefaulType.Where(w => w.Leave_Name.Contains(pLeaveDefault)).ToList();
               }
               if (!string.IsNullOrEmpty(pLeaveDefaultType))
               {
                  leaveDefaulType = leaveDefaulType.Where(w => w.Type == pLeaveDefaultType).ToList();
               }
            }
         }
         catch
         {

         }

         return leaveDefaulType;
      }

      //15-09-2015 Added by sun 
      public ServiceResult UpdateLeaveDefaultNormal(Leave_Default LeaveDefault)
      {
         try
         {
            using (var db = new SBS2DBContext())
            {
               List<Leave_Default_Detail> currLeaveDefaultDetail = new List<Leave_Default_Detail>();
               var lDDRemove = new List<Leave_Default_Detail>();

               if (LeaveDefault.Default_ID == 0)
                  return new ServiceResult { Code = ERROR_CODE.ERROR_511_DATA_NOT_FOUND, Msg = Resource.Leave_Default + " " + Resource.Not_Found_Msg, Field = Resource.Leave_Default };

               var current = db.Leave_Default.Where(w => w.Default_ID == LeaveDefault.Default_ID).FirstOrDefault();
               if (current == null)
                  return new ServiceResult { Code = ERROR_CODE.ERROR_511_DATA_NOT_FOUND, Msg = Resource.Leave_Default + " " + Resource.Not_Found_Msg, Field = Resource.Leave_Default };

               currLeaveDefaultDetail = (from a in db.Leave_Default_Detail where a.Default_ID == current.Default_ID select a).ToList();

               foreach (var row in currLeaveDefaultDetail)
               {
                  if (LeaveDefault.Leave_Default_Detail == null || !LeaveDefault.Leave_Default_Detail.Select(s => s.Default_Detail_ID).Contains(row.Default_Detail_ID))
                  {
                     lDDRemove.Add(row);
                     var lDD = db.Leave_Default_Detail.Where(w => w.Default_Detail_ID == row.Default_Detail_ID);
                     db.Leave_Default_Detail.RemoveRange(lDD);
                  }
               }

               if (lDDRemove.Count > 0)
                  db.Leave_Default_Detail.RemoveRange(lDDRemove);

               if (LeaveDefault.Leave_Default_Detail.Count != 0)
               {
                  foreach (var row in LeaveDefault.Leave_Default_Detail)
                  {
                     if (row.Default_Detail_ID == 0 || !currLeaveDefaultDetail.Select(s => s.Default_Detail_ID).Contains(row.Default_Detail_ID))
                        db.Leave_Default_Detail.Add(row);
                     else
                     {
                        var currLDD = db.Leave_Default_Detail.Where(w => w.Default_Detail_ID == row.Default_Detail_ID).FirstOrDefault();
                        if (currLDD != null)
                        {
                           currLDD.Default_Leave_Amount = row.Default_Leave_Amount;
                           currLDD.Default_ID = row.Default_ID;
                           currLDD.Year_Service = row.Year_Service;
                           db.Entry(currLDD).State = EntityState.Modified;
                        }
                     }
                  }
               }

               var lcons = db.Leave_Default_Condition.Where(w => w.Default_ID == LeaveDefault.Default_ID);
               db.Leave_Default_Condition.RemoveRange(lcons);

               if (LeaveDefault.Leave_Default_Condition != null)
               {
                  foreach (var crow in LeaveDefault.Leave_Default_Condition)
                  {
                     var lcon = new Leave_Default_Condition()
                     {
                        Lookup_Data_ID = crow.Lookup_Data_ID,
                        Default_ID = LeaveDefault.Default_ID
                     };
                     db.Leave_Default_Condition.Add(lcon);
                  }
               }

               db.Entry(current).CurrentValues.SetValues(LeaveDefault);
               db.SaveChanges();
               return new ServiceResult() { Code = ERROR_CODE.SUCCESS, Msg = new Success().getSuccess(ERROR_CODE.SUCCESS_EDIT), Field = Resource.Leave_Default };
            }
         }
         catch
         {
            return new ServiceResult() { Code = ERROR_CODE.ERROR_504_UPDATE_ERROR, Msg = new Error().getError(ERROR_CODE.ERROR_504_UPDATE_ERROR), Field = Resource.Leave_Default };
         }

      }

      //16-09-2015 Added by sun
      public ServiceResult InsertLeaveDefaultNormal(Leave_Default LeaveDefault)
      {
         try
         {
            using (var db = new SBS2DBContext())
            {

               db.Leave_Default.Add(LeaveDefault);
               db.SaveChanges();
               return new ServiceResult() { Code = ERROR_CODE.SUCCESS, Msg = new Success().getSuccess(ERROR_CODE.SUCCESS_CREATE), Field = Resource.Leave_Default };
            }
         }
         catch
         {
            return new ServiceResult() { Code = ERROR_CODE.ERROR_503_INSERT_ERROR, Msg = new Error().getError(ERROR_CODE.ERROR_503_INSERT_ERROR), Field = Resource.Leave_Default };
         }
      }

      //16-09-2015 Added by sun
      public ServiceResult InsertLeaveDefaultChild(Leave_Default LeaveDefault)
      {
         try
         {
            using (var db = new SBS2DBContext())
            {
               db.Leave_Default.Add(LeaveDefault);
               db.SaveChanges();
               return new ServiceResult() { Code = ERROR_CODE.SUCCESS, Msg = new Success().getSuccess(ERROR_CODE.SUCCESS_CREATE), Field = Resource.Leave_Default };
            }
         }
         catch
         {
            return new ServiceResult() { Code = ERROR_CODE.ERROR_503_INSERT_ERROR, Msg = new Error().getError(ERROR_CODE.ERROR_503_INSERT_ERROR), Field = Resource.Leave_Default };
         }
      }

      //16-09-2015 Added by sun
      public ServiceResult UpdateLeaveDefaultChild(Leave_Default LeaveDefault)
      {
         try
         {
            using (var db = new SBS2DBContext())
            {

               List<Leave_Default_Child_Detail> currLeaveDefaultDetail = new List<Leave_Default_Child_Detail>();
               var lDCDRemove = new List<Leave_Default_Child_Detail>();

               if (LeaveDefault.Default_ID == 0)
                  return new ServiceResult { Code = ERROR_CODE.ERROR_511_DATA_NOT_FOUND, Msg = Resource.Leave_Default + " " + Resource.Not_Found_Msg, Field = Resource.Leave_Default };

               var current = db.Leave_Default.Where(w => w.Default_ID == LeaveDefault.Default_ID).FirstOrDefault();
               if (current == null)
                  return new ServiceResult { Code = ERROR_CODE.ERROR_511_DATA_NOT_FOUND, Msg = Resource.Leave_Default + " " + Resource.Not_Found_Msg, Field = Resource.Leave_Default };

               currLeaveDefaultDetail = (from a in db.Leave_Default_Child_Detail where a.Default_ID == current.Default_ID select a).ToList();

               foreach (var row in currLeaveDefaultDetail)
               {
                  if (LeaveDefault.Leave_Default_Child_Detail == null || !LeaveDefault.Leave_Default_Child_Detail.Select(s => s.Leave_Default_Child_Detail_ID).Contains(row.Leave_Default_Child_Detail_ID))
                  {
                     lDCDRemove.Add(row);
                     var LDD = db.Leave_Default_Child_Detail.Where(w => w.Leave_Default_Child_Detail_ID == row.Leave_Default_Child_Detail_ID);
                     db.Leave_Default_Child_Detail.RemoveRange(LDD);
                  }
               }

               if (lDCDRemove.Count > 0)
                  db.Leave_Default_Child_Detail.RemoveRange(lDCDRemove);

               if (LeaveDefault.Leave_Default_Child_Detail.Count != 0)
               {
                  foreach (var row in LeaveDefault.Leave_Default_Child_Detail)
                  {
                     if (row.Leave_Default_Child_Detail_ID == 0 || !currLeaveDefaultDetail.Select(s => s.Leave_Default_Child_Detail_ID).Contains(row.Leave_Default_Child_Detail_ID))
                        db.Leave_Default_Child_Detail.Add(row);
                     else
                     {
                        var currLDCD = db.Leave_Default_Child_Detail.Where(w => w.Leave_Default_Child_Detail_ID == row.Leave_Default_Child_Detail_ID).FirstOrDefault();
                        if (currLDCD != null)
                        {
                           currLDCD.Default_Leave_Amount = row.Default_Leave_Amount;
                           currLDCD.Default_ID = row.Default_ID;
                           currLDCD.Residential_Status = row.Residential_Status;
                           currLDCD.Period = row.Period;
                           db.Entry(currLDCD).State = EntityState.Modified;
                        }
                     }
                  }
               }

               var lcons = db.Leave_Default_Condition.Where(w => w.Default_ID == LeaveDefault.Default_ID);
               db.Leave_Default_Condition.RemoveRange(lcons);

               if (LeaveDefault.Leave_Default_Condition != null)
               {
                  foreach (var crow in LeaveDefault.Leave_Default_Condition)
                  {
                     var lcon = new Leave_Default_Condition()
                     {
                        Lookup_Data_ID = crow.Lookup_Data_ID,
                        Default_ID = LeaveDefault.Default_ID
                     };
                     db.Leave_Default_Condition.Add(lcon);
                  }
               }
               db.Entry(current).CurrentValues.SetValues(LeaveDefault);
               db.SaveChanges();
               return new ServiceResult() { Code = ERROR_CODE.SUCCESS, Msg = new Success().getSuccess(ERROR_CODE.SUCCESS_EDIT), Field = Resource.Leave_Default };
            }
         }
         catch
         {
            return new ServiceResult() { Code = ERROR_CODE.ERROR_504_UPDATE_ERROR, Msg = new Error().getError(ERROR_CODE.ERROR_504_UPDATE_ERROR), Field = Resource.Leave_Default };
         }
      }

      //17-09-2015 Added by sun
      public ServiceResult DeleteLeaveDefault(Nullable<int> LeaveDefaultID)
      {
         try
         {
            using (var db = new SBS2DBContext())
            {
               var leaveDefault = db.Leave_Default.Where(w => w.Default_ID == LeaveDefaultID).FirstOrDefault();

               if (leaveDefault != null)
               {
                  if (leaveDefault.Type != LeaveConfigType.Child)
                  {
                     var leaveDefaulttypeDetail = db.Leave_Default_Detail.Where(w => w.Default_ID == leaveDefault.Default_ID);
                     db.Leave_Default_Detail.RemoveRange(leaveDefaulttypeDetail);
                  }
                  else
                  {
                     var leaveDefaultChildDetail = db.Leave_Default_Child_Detail.Where(w => w.Default_ID == leaveDefault.Default_ID);
                     db.Leave_Default_Child_Detail.RemoveRange(leaveDefaultChildDetail);
                  }

                  var lDeCon = db.Leave_Default_Condition.Where(w => w.Default_ID == leaveDefault.Default_ID);
                  db.Leave_Default_Condition.RemoveRange(lDeCon);

                  var l = db.Leave_Default.Where(w => w.Default_ID == leaveDefault.Default_ID).FirstOrDefault();
                  db.Leave_Default.Remove(l);

                  db.SaveChanges();
               }

               return new ServiceResult() { Code = ERROR_CODE.SUCCESS, Msg = new Success().getSuccess(ERROR_CODE.SUCCESS_DELETE) };
            }
         }
         catch
         {
            return new ServiceResult() { Code = ERROR_CODE.ERROR_505_DELETE_ERROR, Msg = new Error().getError(ERROR_CODE.ERROR_505_DELETE_ERROR) };
         }
      }

      //17-09-2015 Added by sun
      public ServiceResult MultipleDeleteLeaveDefault(int[] pDefaultIDs)
      {
         try
         {
            using (var db = new SBS2DBContext())
            {

               var current = db.Leave_Default.Where(w => pDefaultIDs.Contains(w.Default_ID));
               if (current == null)
                  return new ServiceResult { Code = ERROR_CODE.ERROR_511_DATA_NOT_FOUND, Msg = Resource.Leave_Default + " " + Resource.Not_Found_Msg, Field = Resource.Leave_Default };

               foreach (var expn in current)
                  DeleteLeaveDefault(expn.Default_ID);

               return new ServiceResult() { Code = ERROR_CODE.SUCCESS, Msg = new Success().getSuccess(ERROR_CODE.SUCCESS_DELETE) };
            }
         }
         catch
         {
            return new ServiceResult() { Code = ERROR_CODE.ERROR_505_DELETE_ERROR, Msg = new Error().getError(ERROR_CODE.ERROR_505_DELETE_ERROR) };
         }
      }
      #endregion

      #region Leave App
      public List<Leave_Report> LstLeaveApplicationDocumentReport(
         Nullable<int> pCompanyID,
         Nullable<int> pDepartmentID = null,
         Nullable<int> pLeaveTypeID = null,
         Nullable<int> pYear = null,
         List<int> pLeaveTypeSel = null)
      {
         var leaves = new List<Leave_Report>();
         try
         {
            using (var db = new SBS2DBContext())
            {
               if (pCompanyID.HasValue)
               {
                  var emps = db.Leave_Application_Document
                             .Where(w => w.Leave_Config.Company_ID == pCompanyID && w.Overall_Status == SBSWorkFlowAPI.Constants.WorkflowStatus.Closed)
                             .Select(s => s.Employee_Profile).Distinct();

                  //check test
                  var test = emps.ToList();

                  foreach (var emp in emps)
                  {
                     var name = emp.User_Profile.First_Name;
                     var doAddRow = true;

                     if (pDepartmentID.HasValue)
                     {
                        var emphists = (from a in db.Employment_History
                                        where a.Employee_Profile_ID == emp.Employee_Profile_ID &
                                        a.Effective_Date.Value.Year <= pYear
                                        orderby a.Effective_Date
                                        select a).FirstOrDefault();

                        if (emphists == null)
                           doAddRow = false;
                        else
                        {
                           if (emphists.Department_ID != pDepartmentID)
                              doAddRow = false;
                        }
                     }

                     if (doAddRow)
                     {
                        var leavedocs = db.Leave_Application_Document
                                    .Where(w => w.Leave_Config.Company_ID == pCompanyID &
                                    w.Employee_Profile_ID == emp.Employee_Profile_ID &
                                    w.Overall_Status == SBSWorkFlowAPI.Constants.WorkflowStatus.Closed && w.Cancel_Status != SBSWorkFlowAPI.Constants.WorkflowStatus.Cancelled);

                        if (pLeaveTypeID.HasValue && pLeaveTypeID.Value > 0)
                           leavedocs = leavedocs.Where(w => w.Leave_Config_ID == pLeaveTypeID.Value);

                        if (pYear.HasValue)
                           leavedocs = leavedocs.Where(w => (w.Start_Date.HasValue ? w.Start_Date.Value.Year : 0) == pYear || (w.End_Date.HasValue ? w.End_Date.Value.Year : 0) == pYear);

                        var leavetypelist = LstAndCalulateLeaveType(new LeaveTypeCriteria()
                        {
                           Company_ID = pCompanyID,
                           Profile_ID = emp.Profile_ID,
                           Emp = emp,
                           Leave_Config_IDs = pLeaveTypeSel,
                        });

                        var leavetypedocs = new List<Leave_Type_Report>();
                        foreach (var row in leavetypelist)
                        {
                           //Added by sun 07-03-2016
                           if (pLeaveTypeSel != null)
                           {
                              if (!pLeaveTypeSel.Contains(row.Leave_Config.Leave_Config_ID))
                              {
                                 continue;
                              }
                           }

                           var lConfig = row.Leave_Config;
                           var leaveleft = row.Leave_Left;
                           var ltype = new Leave_Type_Report();
                           if (leaveleft != null)
                           {
                              //decimal bfdays = 0M;
                              //decimal bfPercen = 0M;
                              //if (lConfig.Bring_Forward.HasValue && lConfig.Bring_Forward.Value)
                              //{
                              //   if (lConfig.Is_Bring_Forward_Days.HasValue && lConfig.Is_Bring_Forward_Days.Value)
                              //      bfdays = lConfig.Bring_Forward_Days.HasValue ? lConfig.Bring_Forward_Days.Value : 0;
                              //   else
                              //   {
                              //      bfPercen = lConfig.Bring_Forward_Percent.HasValue ? lConfig.Bring_Forward_Percent.Value : 0;
                              //      bfdays = leaveleft.Entitle * (bfPercen / 100);
                              //   }
                              //}

                              decimal daystakenSum = 0;

                              var daystaken = leavedocs
                                  .Where(w => w.Leave_Config_ID == lConfig.Leave_Config_ID)
                                  .Select(s => s.Days_Taken.HasValue ? s.Days_Taken.Value : 0).ToList();

                              if (daystaken.Count() > 0)
                                 daystakenSum = daystaken.Sum();

                              ltype.Days_Taken = daystakenSum;
                              ltype.Bring_Forward = leaveleft.BringForward;
                              ltype.Entitle = leaveleft.Entitle;
                              ltype.Leave_Config_ID = lConfig.Leave_Config_ID;
                              ltype.Leave_Name = lConfig.Leave_Name;
                           }

                           leavetypedocs.Add(ltype);
                        }

                        leaves.Add(new Leave_Report()
                        {
                           Employee_Name = emp.User_Profile.First_Name + " " + emp.User_Profile.Last_Name,
                           Employee_No = emp.Employee_No,
                           leaveTypelist = leavetypedocs,
                           Profile_ID = emp.Profile_ID,
                        });
                     }
                  }
               }
            }
         }
         catch (Exception e)
         {

         }
         return leaves.ToList(); ;
      }

      public bool LeaveCalIsExist(LeaveCalIsExistCriteria cri)
      {
         using (var db = new SBS2DBContext())
         {
            if (cri.Employee_Profile_ID.HasValue)
            {
               var cal = db.Leave_Calculation.Where(w => w.Employee_Profile_ID == cri.Employee_Profile_ID).FirstOrDefault();
               if (cal != null)
                  return true;
            }
            else if (cri.Profile_ID.HasValue)
            {
               var cal = db.Leave_Calculation.Include(i => i.Employee_Profile).Where(w => w.Employee_Profile.Profile_ID == cri.Profile_ID).FirstOrDefault();
               if (cal != null)
                  return true;
            }
            else if (cri.Relationship_ID.HasValue)
            {
               var cal = db.Leave_Calculation.Where(w => w.Relationship_ID == cri.Relationship_ID).FirstOrDefault();
               if (cal != null)
                  return true;
            }
         }
         return false;
      }

      public List<Leave_Application_Document> LstLeaveApplicationDocument(
          Nullable<int> pCompanyID,
          string pStatus = "",
          Nullable<int> pProfileID = null,
          Nullable<int> pEmployeeID = null,
          Nullable<int> pLeaveConfigID = null,
          string pDateApplied = "",
          string pDateAppliedFrom = "",
          string pDateAppliedTo = "",
          Nullable<int> pBranchID = null,
          Nullable<int> pDepartmentID = null)
      {
         var currentdate = StoredProcedure.GetCurrentDate();
         if (pCompanyID.HasValue)
         {
            var db = new SBS2DBContext();
            var leaveDocList = (from a in db.Leave_Application_Document
                                where a.Leave_Config.Company_ID == pCompanyID.Value
                                select a);

            if (pLeaveConfigID.HasValue)
               leaveDocList = (from a in leaveDocList where a.Leave_Config_ID == pLeaveConfigID select a);

            if (pProfileID.HasValue)
               leaveDocList = (from a in leaveDocList where a.Employee_Profile.Profile_ID == pProfileID select a);

            if (pEmployeeID.HasValue)
               leaveDocList = (from a in leaveDocList where a.Employee_Profile_ID == pEmployeeID select a);

            if (!string.IsNullOrEmpty(pDateApplied))
            {
               var d = DateUtil.ToDate(pDateApplied);
               if (d != null && d.HasValue)
                  leaveDocList = (from a in leaveDocList where a.Date_Applied.Value.Day == d.Value.Day & a.Date_Applied.Value.Month == d.Value.Month & a.Date_Applied.Value.Year == d.Value.Year select a);
            }
            if (!string.IsNullOrEmpty(pDateAppliedFrom))
            {
               var d = DateUtil.ToDate(pDateAppliedFrom);
               if (d != null && d.HasValue)
               {
                  leaveDocList = (from a in leaveDocList
                                  where (a.Date_Applied.Value.Day == d.Value.Day & a.Date_Applied.Value.Month == d.Value.Month & a.Date_Applied.Value.Year == d.Value.Year) |
                                  a.Date_Applied >= d
                                  select a);
               }

            }
            if (!string.IsNullOrEmpty(pDateAppliedTo))
            {
               var d = DateUtil.ToDate(pDateAppliedTo);
               if (d != null && d.HasValue)
               {
                  leaveDocList = (from a in leaveDocList
                                  where (a.Date_Applied.Value.Day == d.Value.Day & a.Date_Applied.Value.Month == d.Value.Month & a.Date_Applied.Value.Year == d.Value.Year) |
                                  a.Date_Applied <= d
                                  select a);
               }

            }
            if (!string.IsNullOrEmpty(pStatus))
            {
               if (pStatus == "Pending")
                  leaveDocList = (from a in leaveDocList where a.Overall_Status == SBSWorkFlowAPI.Constants.WorkflowStatus.Pending | a.Overall_Status == SBSWorkFlowAPI.Constants.WorkflowStatus.Approved select a);
               else
                  leaveDocList = (from a in leaveDocList where a.Overall_Status == pStatus select a);

            }

            if (pBranchID.HasValue)
               leaveDocList = leaveDocList.Where(w => w.Employee_Profile.Employment_History.Where(w1 => w1.Effective_Date <= currentdate).OrderByDescending(o => o.Effective_Date).FirstOrDefault() != null && w.Employee_Profile.Employment_History.Where(w1 => w1.Effective_Date <= currentdate).OrderByDescending(o => o.Effective_Date).FirstOrDefault().Branch_ID == pBranchID);

            if (pDepartmentID.HasValue)
               leaveDocList = leaveDocList.Where(w => w.Employee_Profile.Employment_History.Where(w1 => w1.Effective_Date <= currentdate).OrderByDescending(o => o.Effective_Date).FirstOrDefault() != null && w.Employee_Profile.Employment_History.Where(w1 => w1.Effective_Date <= currentdate).OrderByDescending(o => o.Effective_Date).FirstOrDefault().Department_ID == pDepartmentID);

            return leaveDocList.OrderByDescending(o => o.Start_Date).ToList();
         }
         return null;
      }

      public Leave_Application_Document GetLeaveApplicationDocument(Nullable<int> pDocID)
      {
         using (var db = new SBS2DBContext())
         {
            return db.Leave_Application_Document
                .Include(i => i.Employee_Profile)
                .Include(i => i.Employee_Profile.User_Profile)
                .Include(i => i.Employee_Profile.User_Profile.User_Authentication)
                .Include(i => i.Leave_Config)
                 .Include(i => i.Leave_Config_Detail)
                 .Include(i => i.Upload_Document)
                 .Include(i => i.Relationship)
                .Where(w => w.Leave_Application_Document_ID == pDocID).FirstOrDefault();
         }

      }

      public List<Leave_Application_Document> GetLeaveByRelationship(Nullable<int> pRelationID)
      {
         using (var db = new SBS2DBContext())
         {
            return db.Leave_Application_Document.Where(w => w.Relationship_ID == pRelationID).ToList();
         }
      }

      public ServiceResult ValidateLeaveApplicationDocument(Leave_Application_Document pLeaveApplicationDoc, Nullable<int> pProfileID)
      {
         try
         {
            using (var db = new SBS2DBContext())
            {
               if (pLeaveApplicationDoc != null && pProfileID.HasValue)
               {
                  if (pLeaveApplicationDoc.Leave_Application_Document_ID > 0)
                  {
                     //Update
                  }
                  else
                  {
                     //Insert
                     var emplyee = (from a in db.Employee_Profile where a.Profile_ID == pProfileID.Value select a).SingleOrDefault();
                     if (emplyee != null)
                     {
                        if (pLeaveApplicationDoc.Start_Date.HasValue && pLeaveApplicationDoc.End_Date.HasValue)
                        {
                           var checkDateDup = (from a in db.Leave_Application_Document
                                               where ((a.Start_Date <= pLeaveApplicationDoc.Start_Date && pLeaveApplicationDoc.Start_Date <= a.End_Date)
                                               | (a.Start_Date <= pLeaveApplicationDoc.End_Date && pLeaveApplicationDoc.End_Date <= a.End_Date)
                                               | a.Start_Date == pLeaveApplicationDoc.Start_Date
                                               | (pLeaveApplicationDoc.Start_Date <= a.Start_Date && a.Start_Date <= pLeaveApplicationDoc.End_Date))
                                               & a.Employee_Profile_ID == emplyee.Employee_Profile_ID
                                               & a.Overall_Status != SBSWorkFlowAPI.Constants.WorkflowStatus.Rejected
                                               & a.Cancel_Status != SBSWorkFlowAPI.Constants.WorkflowStatus.Cancelled
                                               select a);

                           foreach (var a in checkDateDup)
                           {
                              // Check Start Date
                              if (pLeaveApplicationDoc.Start_Date == a.Start_Date)
                              {
                                 if (string.IsNullOrEmpty(pLeaveApplicationDoc.Start_Date_Period) | pLeaveApplicationDoc.Start_Date_Period == Period.AM)
                                    return new ServiceResult() { Code = ERROR_CODE.ERROR_510_DATA_DUPLICATE, Msg = Resource.Date + " " + Resource.Is_Duplicated_Lower, Field = Resource.Start_Date };
                                 else if (a.Start_Date_Period == pLeaveApplicationDoc.Start_Date_Period)
                                    return new ServiceResult() { Code = ERROR_CODE.ERROR_510_DATA_DUPLICATE, Msg = Resource.Date + " " + Resource.Is_Duplicated_Lower, Field = Resource.Start_Date };
                              }

                              if (a.Start_Date < pLeaveApplicationDoc.Start_Date && pLeaveApplicationDoc.Start_Date < a.End_Date)
                                 return new ServiceResult() { Code = ERROR_CODE.ERROR_510_DATA_DUPLICATE, Msg = Resource.Date + " " + Resource.Is_Duplicated_Lower, Field = Resource.Start_Date };

                              if (pLeaveApplicationDoc.Start_Date == a.End_Date)
                              {
                                 if (string.IsNullOrEmpty(pLeaveApplicationDoc.Start_Date_Period))
                                    return new ServiceResult() { Code = ERROR_CODE.ERROR_510_DATA_DUPLICATE, Msg = Resource.Date + " " + Resource.Is_Duplicated_Lower, Field = Resource.Start_Date };
                                 if (string.IsNullOrEmpty(a.End_Date_Period) | a.End_Date_Period == Period.PM)   //All End Date Day
                                    return new ServiceResult() { Code = ERROR_CODE.ERROR_510_DATA_DUPLICATE, Msg = Resource.Date + " " + Resource.Is_Duplicated_Lower, Field = Resource.Start_Date };
                                 else
                                 {
                                    // Only End Date AM
                                    if (pLeaveApplicationDoc.Start_Date_Period == a.End_Date_Period)
                                       return new ServiceResult() { Code = ERROR_CODE.ERROR_510_DATA_DUPLICATE, Msg = Resource.Date + " " + Resource.Is_Duplicated_Lower, Field = Resource.Start_Date };
                                 }
                              }

                              // Check End Date
                              if (pLeaveApplicationDoc.End_Date == a.End_Date)
                              {
                                 if (string.IsNullOrEmpty(pLeaveApplicationDoc.End_Date_Period) | pLeaveApplicationDoc.End_Date_Period == Period.PM)
                                    return new ServiceResult() { Code = ERROR_CODE.ERROR_510_DATA_DUPLICATE, Msg = Resource.Date + " " + Resource.Is_Duplicated_Lower, Field = Resource.Start_Date };
                                 else if (a.End_Date_Period == pLeaveApplicationDoc.End_Date_Period)
                                    return new ServiceResult() { Code = ERROR_CODE.ERROR_510_DATA_DUPLICATE, Msg = Resource.Date + " " + Resource.Is_Duplicated_Lower, Field = Resource.Start_Date };
                              }

                              if (a.Start_Date < pLeaveApplicationDoc.End_Date && pLeaveApplicationDoc.End_Date < a.End_Date)
                                 return new ServiceResult() { Code = ERROR_CODE.ERROR_510_DATA_DUPLICATE, Msg = Resource.Date + " " + Resource.Is_Duplicated_Lower, Field = Resource.Start_Date };

                              if (pLeaveApplicationDoc.End_Date == a.Start_Date)
                              {
                                 if (string.IsNullOrEmpty(pLeaveApplicationDoc.End_Date_Period) | pLeaveApplicationDoc.End_Date_Period == Period.PM)  //All End Date Day
                                    return new ServiceResult() { Code = ERROR_CODE.ERROR_510_DATA_DUPLICATE, Msg = Resource.Date + " " + Resource.Is_Duplicated_Lower, Field = Resource.Start_Date };
                                 if (string.IsNullOrEmpty(a.Start_Date_Period) | a.Start_Date_Period == Period.AM)     //All Start Date Day
                                    return new ServiceResult() { Code = ERROR_CODE.ERROR_510_DATA_DUPLICATE, Msg = Resource.Date + " " + Resource.Is_Duplicated_Lower, Field = Resource.Start_Date };
                                 else
                                 {
                                    // Only Start Date AM
                                    if (pLeaveApplicationDoc.End_Date_Period == a.Start_Date_Period)
                                       return new ServiceResult() { Code = ERROR_CODE.ERROR_510_DATA_DUPLICATE, Msg = Resource.Date + " " + Resource.Is_Duplicated_Lower, Field = Resource.Start_Date };
                                 }
                              }

                              // Check in range
                              if (pLeaveApplicationDoc.Start_Date < a.Start_Date && a.Start_Date < pLeaveApplicationDoc.End_Date)
                                 return new ServiceResult() { Code = ERROR_CODE.ERROR_510_DATA_DUPLICATE, Msg = Resource.Date + " " + Resource.Is_Duplicated_Lower, Field = Resource.Start_Date };

                           }
                        }
                        else if (pLeaveApplicationDoc.Start_Date.HasValue)
                        {
                           var checkDateDup = (from a in db.Leave_Application_Document
                                               where ((a.Start_Date <= pLeaveApplicationDoc.Start_Date && pLeaveApplicationDoc.Start_Date <= a.End_Date)
                                               | a.Start_Date == pLeaveApplicationDoc.Start_Date)
                                               & a.Employee_Profile_ID == emplyee.Employee_Profile_ID
                                               & a.Overall_Status != SBSWorkFlowAPI.Constants.WorkflowStatus.Rejected
                                               & a.Cancel_Status != SBSWorkFlowAPI.Constants.WorkflowStatus.Cancelled
                                               select a);

                           foreach (var a in checkDateDup)
                           {
                              if (pLeaveApplicationDoc.Start_Date == a.Start_Date)
                              {
                                 if (string.IsNullOrEmpty(pLeaveApplicationDoc.Start_Date_Period))
                                    return new ServiceResult() { Code = ERROR_CODE.ERROR_510_DATA_DUPLICATE, Msg = Resource.Date + " " + Resource.Is_Duplicated_Lower, Field = Resource.Start_Date };
                                 else if (a.Start_Date_Period == pLeaveApplicationDoc.Start_Date_Period)
                                    return new ServiceResult() { Code = ERROR_CODE.ERROR_510_DATA_DUPLICATE, Msg = Resource.Date + " " + Resource.Is_Duplicated_Lower, Field = Resource.Start_Date };
                              }
                              if (a.Start_Date < pLeaveApplicationDoc.Start_Date && pLeaveApplicationDoc.Start_Date < a.End_Date)
                                 return new ServiceResult() { Code = ERROR_CODE.ERROR_510_DATA_DUPLICATE, Msg = Resource.Date + " " + Resource.Is_Duplicated_Lower, Field = Resource.Start_Date };

                              if (pLeaveApplicationDoc.Start_Date == a.End_Date)
                              {
                                 if (string.IsNullOrEmpty(pLeaveApplicationDoc.Start_Date_Period))
                                    return new ServiceResult() { Code = ERROR_CODE.ERROR_510_DATA_DUPLICATE, Msg = Resource.Date + " " + Resource.Is_Duplicated_Lower, Field = Resource.Start_Date };
                                 if (string.IsNullOrEmpty(a.End_Date_Period) | a.End_Date_Period == Period.PM) //All Day
                                    return new ServiceResult() { Code = ERROR_CODE.ERROR_510_DATA_DUPLICATE, Msg = Resource.Date + " " + Resource.Is_Duplicated_Lower, Field = Resource.Start_Date };
                                 else
                                 {
                                    // Only AM
                                    if (pLeaveApplicationDoc.Start_Date_Period == a.End_Date_Period)
                                       return new ServiceResult() { Code = ERROR_CODE.ERROR_510_DATA_DUPLICATE, Msg = Resource.Date + " " + Resource.Is_Duplicated_Lower, Field = Resource.Start_Date };
                                 }
                              }
                           }
                        }
                        return new ServiceResult() { Code = ERROR_CODE.SUCCESS };
                     }
                  }

               }
            }
         }
         catch
         {
            return new ServiceResult() { Code = ERROR_CODE.ERROR_506_SAVE_ERROR, Msg = new Error().getError(ERROR_CODE.ERROR_506_SAVE_ERROR) };
         }
         return new ServiceResult() { Code = ERROR_CODE.ERROR_506_SAVE_ERROR };
      }

      public ServiceResult SaveLeaveApplicationDocument(Leave_Application_Document pLeaveApplicationDoc, byte[] data, string filename, Nullable<int> pProfileID)
      {
         try
         {
            //var domain = UrlUtil.GetDomain(Request, ModuleDomain.HR);
            var currentdate = StoredProcedure.GetCurrentDate();
            using (var db = new SBS2DBContext())
            {
               if (pLeaveApplicationDoc != null && pProfileID.HasValue)
               {

                  if (pLeaveApplicationDoc.Leave_Application_Document_ID > 0)
                  {
                     //Update                          
                  }
                  else
                  {
                     //Insert
                     var leaveConfig = (from a in db.Leave_Config where a.Leave_Config_ID == pLeaveApplicationDoc.Leave_Config_ID select a).SingleOrDefault();
                     var emplyee = (from a in db.Employee_Profile where a.Profile_ID == pProfileID.Value select a).SingleOrDefault();
                     if (emplyee != null)
                     {
                        if (data != null)
                        {
                           if (leaveConfig.Upload_Document == true)
                           {
                              var doc = new Upload_Document()
                              {
                                 Document = data,
                                 File_Name = filename,
                                 Leave_Application_Document = pLeaveApplicationDoc,
                                 Create_By = pLeaveApplicationDoc.Create_By,
                                 Create_On = pLeaveApplicationDoc.Create_On,
                                 Update_By = pLeaveApplicationDoc.Update_By,
                                 Update_On = pLeaveApplicationDoc.Update_On
                              };
                              var guid = Guid.NewGuid();
                              while (db.Upload_Document.Where(w => w.Upload_Document_ID == guid).FirstOrDefault() != null)
                              {
                                 guid = Guid.NewGuid();
                              }
                              doc.Upload_Document_ID = guid;
                              db.Upload_Document.Add(doc);
                           }
                        }
                        pLeaveApplicationDoc.Overall_Status = SBSWorkFlowAPI.Constants.WorkflowStatus.Pending;
                        pLeaveApplicationDoc.Employee_Profile_ID = emplyee.Employee_Profile_ID;
                        pLeaveApplicationDoc.Date_Applied = currentdate;
                        if (pLeaveApplicationDoc.Start_Date == pLeaveApplicationDoc.End_Date)
                        {
                           pLeaveApplicationDoc.End_Date_Period = "";
                           pLeaveApplicationDoc.End_Date = null;
                        }

                        db.Leave_Application_Document.Add(pLeaveApplicationDoc);
                        db.SaveChanges();
                        db.Entry(pLeaveApplicationDoc).GetDatabaseValues();

                        return new ServiceResult() { Code = ERROR_CODE.SUCCESS, Msg = new Success().getSuccess(ERROR_CODE.SUCCESS) };
                     }
                  }

               }
            }
         }
         catch
         {
            return new ServiceResult() { Code = ERROR_CODE.ERROR_506_SAVE_ERROR, Msg = new Error().getError(ERROR_CODE.ERROR_506_SAVE_ERROR) };
         }
         return new ServiceResult() { Code = ERROR_CODE.ERROR_511_DATA_NOT_FOUND, Msg = new Error().getError(ERROR_CODE.ERROR_511_DATA_NOT_FOUND) };
      }

      public ServiceResult UpdateLeaveApplicationDocument(Leave_Application_Document pLeaveApplicationDoc)
      {
         try
         {
            using (var db = new SBS2DBContext())
            {
               db.Entry(pLeaveApplicationDoc).State = EntityState.Modified;
               db.SaveChanges();
               return new ServiceResult() { Code = ERROR_CODE.SUCCESS, Msg = new Success().getSuccess(ERROR_CODE.SUCCESS_EDIT), Field = Resource.Leave };
            }
         }
         catch
         {
            return new ServiceResult() { Code = ERROR_CODE.ERROR_506_SAVE_ERROR, Msg = new Error().getError(ERROR_CODE.ERROR_506_SAVE_ERROR), Field = Resource.Leave };
         }
      }

      public ServiceResult UpdateLeaveStatus(Nullable<int> pDocID, string Status = null, string Cancel_Status = null)
      {
         try
         {
            var currentdate = StoredProcedure.GetCurrentDate();
            using (var db = new SBS2DBContext())
            {
               var current = (from a in db.Leave_Application_Document where a.Leave_Application_Document_ID == pDocID.Value select a).FirstOrDefault();
               if (current != null)
               {
                  if (!string.IsNullOrEmpty(Status))
                     current.Overall_Status = Status;

                  if (!string.IsNullOrEmpty(Cancel_Status))
                     current.Cancel_Status = Cancel_Status;
                  db.SaveChanges();
                  return new ServiceResult() { Code = ERROR_CODE.SUCCESS, Msg = new Success().getSuccess(ERROR_CODE.SUCCESS_EDIT), Field = Resource.Leave };

               }
            }
         }
         catch
         {
            return new ServiceResult() { Code = ERROR_CODE.ERROR_504_UPDATE_ERROR, Msg = new Error().getError(ERROR_CODE.ERROR_504_UPDATE_ERROR), Field = Resource.Leave };
         }
         return new ServiceResult() { Code = ERROR_CODE.ERROR_511_DATA_NOT_FOUND, Msg = new Error().getError(ERROR_CODE.ERROR_511_DATA_NOT_FOUND), Field = Resource.Leave };

      }


      public ServiceResult UpdateLeaveUse(Nullable<int> pDocID, Nullable<int> pEmployeeID, string Status = null, string Cancel_Status = null, bool increaseuse = true)
      {
         try
         {
            //var domain = UrlUtil.GetDomain(Request, ModuleDomain.HR);
            var currentdate = StoredProcedure.GetCurrentDate();
            using (var db = new SBS2DBContext())
            {
               if (pDocID.HasValue && pEmployeeID.HasValue)
               {
                  var current = (from a in db.Leave_Application_Document where a.Leave_Application_Document_ID == pDocID.Value select a).FirstOrDefault();
                  if (current != null)
                  {
                     var currentcal = (from a in db.Leave_Calculation where a.Employee_Profile_ID == current.Employee_Profile_ID & a.Leave_Config_ID == current.Leave_Config_ID orderby a.Start_Date descending select a).FirstOrDefault();
                     if (currentcal != null)
                     {
                        if (increaseuse)
                        {
                           if (current.Weeks_Taken.HasValue && current.Weeks_Taken.Value > 0)
                              currentcal.Leave_Used += current.Weeks_Taken;
                           else
                              currentcal.Leave_Used += current.Days_Taken;
                        }
                        else
                        {
                           if (current.Weeks_Taken.HasValue && current.Weeks_Taken.Value > 0)
                              currentcal.Leave_Used -= current.Weeks_Taken;
                           else
                              currentcal.Leave_Used -= current.Days_Taken;
                        }
                        currentcal.Update_On = currentdate;
                     }
                     if (!string.IsNullOrEmpty(Status))
                        current.Overall_Status = Status;

                     if (!string.IsNullOrEmpty(Cancel_Status))
                        current.Cancel_Status = Cancel_Status;

                     db.SaveChanges();
                     return new ServiceResult() { Code = ERROR_CODE.SUCCESS, Msg = new Success().getSuccess(ERROR_CODE.SUCCESS_EDIT), Field = Resource.Leave };
                  }
               }
            }
         }
         catch
         {
            return new ServiceResult() { Code = ERROR_CODE.ERROR_504_UPDATE_ERROR, Msg = new Error().getError(ERROR_CODE.ERROR_504_UPDATE_ERROR), Field = Resource.Leave };
         }
         return new ServiceResult() { Code = ERROR_CODE.ERROR_511_DATA_NOT_FOUND, Msg = new Error().getError(ERROR_CODE.ERROR_511_DATA_NOT_FOUND), Field = Resource.Leave };
      }

      //public ServiceResult CalculateLeave(int pLeaveTypeID, Nullable<int> pProfileID = null, Nullable<int> pCompanyID = null)
      //{
      //    try
      //    {
      //        using (var db = new SBS2DBContext())
      //        {

      //            if (pCompanyID.HasValue)
      //            {
      //                var emps = (from a in db.Employee_Profile
      //                            where a.User_Profile.Company_ID == pCompanyID
      //                            select a);

      //                foreach (var emp in emps)
      //                {
      //                    var result = CalculateLeave(db, pLeaveTypeID, emp.User_Profile.Profile_ID);
      //                    if (result.Code != ERROR_CODE.SUCCESS)
      //                        return result;
      //                }
      //            }
      //            else
      //            {
      //                return CalculateLeave(db, pLeaveTypeID, pProfileID.Value);
      //            }
      //        }
      //    }
      //    catch
      //    {
      //        return new ServiceResult() { Code = ERROR_CODE.ERROR_513_CAL_LEAVE_ERROR, Msg = new Error().getError(ERROR_CODE.ERROR_513_CAL_LEAVE_ERROR) };
      //    }
      //    return new ServiceResult() { Code = ERROR_CODE.SUCCESS, Msg = new Success().getSuccess(ERROR_CODE.SUCCESS) };
      //}

      public ServiceResult CalculateLeaveAllUser(LeaveTypeCriteria cri)
      {
         try
         {
            using (var db = new SBS2DBContext())
            {
               var lconfig = db.Leave_Config
                          .Include(i => i.Leave_Config_Extra)
                          .Include(i => i.Leave_Config_Detail)
                          .Include(i => i.Leave_Config_Child_Detail)
                          .Where(w => w.Company_ID == cri.Company_ID && w.Record_Status != RecordStatus.Delete).FirstOrDefault();

               var emps = (from a in db.Employee_Profile
                           where a.User_Profile.Company_ID == cri.Company_ID
                           select a);

               foreach (var emp in emps)
               {
                  cri.Profile_ID = emp.Profile_ID;
                  cri.Emp = emp;
                  cri.Leave_Config = lconfig;
                  var result = CalculateLeave(cri);
                  if (result.Code != ERROR_CODE.SUCCESS)
                     return result;
               }
            }
            return new ServiceResult() { Code = ERROR_CODE.SUCCESS, Msg = new Success().getSuccess(ERROR_CODE.SUCCESS) };
         }
         catch
         {
            return new ServiceResult() { Code = ERROR_CODE.ERROR_513_CAL_LEAVE_ERROR, Msg = new Error().getError(ERROR_CODE.ERROR_513_CAL_LEAVE_ERROR) };
         }
      }

      public ServiceResult CalculateLeave(LeaveTypeCriteria cri)
      {

         try
         {
            using (var db = new SBS2DBContext())
            {
               var currentdate = StoredProcedure.GetCurrentDate().Date;

               if (cri.Emp == null)
                  cri.Emp = (from a in db.Employee_Profile where a.User_Profile.Profile_ID == cri.Profile_ID select a).FirstOrDefault();
               if (cri.Emp == null)
                  return new ServiceResult() { Code = ERROR_CODE.ERROR_511_DATA_NOT_FOUND, Msg = new Error().getError(ERROR_CODE.ERROR_511_DATA_NOT_FOUND) };

               var com = (from a in db.Company_Details
                          where a.Effective_Date <= currentdate
                          & a.Company_ID == cri.Emp.User_Profile.Company_ID
                          select a).FirstOrDefault();

               if (com == null)
                  return new ServiceResult() { Code = ERROR_CODE.ERROR_511_DATA_NOT_FOUND, Msg = new Error().getError(ERROR_CODE.ERROR_511_DATA_NOT_FOUND), Field = Resource.Company };


               if (cri.firsthist == null)
                  cri.firsthist = db.Employment_History.Where(w => w.Employee_Profile_ID == cri.Emp.Employee_Profile_ID).OrderBy(o => o.Effective_Date).FirstOrDefault();

               var beginyear = 0;
               var endyear = currentdate.Year;
               var yearservice = 0;


               Nullable<DateTime> hiredDate = currentdate;
               if (cri.firsthist != null && cri.firsthist.Effective_Date.HasValue)
               {
                  hiredDate = cri.firsthist.Effective_Date.Value;
                  beginyear = cri.firsthist.Effective_Date.Value.Year;
               }

               //Edit by sun 08-02-2016
               // var comregyear = com.Registration_Date.HasValue ? com.Registration_Date.Value.Year : beginyear;
               var comregyear = com.Leave_Start_Date.HasValue ? com.Leave_Start_Date.Value.Year : beginyear;

               if (cri.Leave_Config == null)
                  cri.Leave_Config = db.Leave_Config.Where(w => w.Leave_Config_ID == cri.Leave_Config_ID).FirstOrDefault();
               if (cri.Leave_Config == null)
                  return new ServiceResult() { Code = ERROR_CODE.ERROR_511_DATA_NOT_FOUND, Msg = new Error().getError(ERROR_CODE.ERROR_511_DATA_NOT_FOUND), Field = Resource.Leave };

               var gender = cri.Leave_Config.Leave_Config_Condition.Where(w => w.Lookup_Data_ID == cri.Emp.Gender).FirstOrDefault();
               var mstatus = cri.Leave_Config.Leave_Config_Condition.Where(w => w.Lookup_Data_ID == cri.Emp.Marital_Status).FirstOrDefault();

               if (gender == null)
                  return new ServiceResult() { Code = ERROR_CODE.SUCCESS, Msg = new Success().getSuccess(ERROR_CODE.SUCCESS) };

               if (mstatus == null)
                  return new ServiceResult() { Code = ERROR_CODE.SUCCESS, Msg = new Success().getSuccess(ERROR_CODE.SUCCESS) };

               if (cri.Leave_Config.Type == LeaveConfigType.Normal)
               {
                  var cals = (from a in db.Leave_Calculation
                              where a.Employee_Profile_ID == cri.Emp.Employee_Profile_ID &
                              a.Leave_Config.Leave_Config_ID == cri.Leave_Config_ID
                              orderby a.Start_Date descending
                              select a).ToList();

                  // renovate cal
                  foreach (var c in cals)
                  {
                     c.Bring_Forward = 0;
                     c.CutOff = 0;

                     var emphist = db.Employment_History
                         .Where(w => w.Employee_Profile_ID == cri.Emp.Employee_Profile_ID & w.Effective_Date <= c.Start_Date)
                         .OrderByDescending(o => o.Effective_Date)
                         .FirstOrDefault();

                     if (emphist != null)
                     {
                        var cworkspan = (c.Start_Date.Value.Date - hiredDate.Value.Date);
                        var cyearservice = NumUtil.ParseInteger(Math.Floor((cworkspan.TotalDays + 1) / 365));

                        Leave_Config_Detail lconfigDetail = cri.Leave_Config
                            .Leave_Config_Detail
                            .Where(w => w.Leave_Config_ID == c.Leave_Config_ID & (w.Designation_ID == c.Designation_ID | w.Designation_ID == null) & w.Year_Service <= cyearservice)
                            .OrderByDescending(o => o.Year_Service)
                            .FirstOrDefault();

                        if (lconfigDetail != null)
                        {
                           if (lconfigDetail.Designation_ID != null && lconfigDetail.Designation_ID != emphist.Designation_ID)
                           {
                              lconfigDetail = cri.Leave_Config
                                  .Leave_Config_Detail
                                  .Where(w => (w.Designation_ID == emphist.Designation_ID || w.Designation_ID == null) & w.Year_Service <= yearservice)
                                  .OrderByDescending(o => o.Year_Service)
                                  .FirstOrDefault();

                              if (lconfigDetail != null)
                                 c.Designation_ID = lconfigDetail.Designation_ID;
                           }

                           if (com.Default_Fiscal_Year.HasValue && com.Default_Fiscal_Year.Value == false)
                           {
                              var fday = com.Custom_Fiscal_Year.Value.Day;
                              var fmonth = com.Custom_Fiscal_Year.Value.Month;
                              if (fday != 31 || fmonth != 12)
                              {
                                 if (c.Start_Date.Value.Month == fmonth)
                                 {
                                    if (c.Start_Date.Value.Day > fday)
                                       c.Year_Assigned = (c.Start_Date.Value.Year + 1).ToString();
                                 }
                                 else if (c.Start_Date.Value.Month > fmonth)
                                    c.Year_Assigned = (c.Start_Date.Value.Year + 1).ToString();
                              }
                           }
                           if (lconfigDetail != null)
                              c.Entitle = lconfigDetail.Default_Leave_Amount;
                        }
                     }
                  }

                  db.SaveChanges();

                  for (int i = beginyear; i <= endyear; i++)
                  {
                     //if (i < comregyear)
                     //   continue;

                     var currentyear = i.ToString();
                     var fiscaldate = DateUtil.ToDate("31/12/" + currentyear);
                     if (com.Default_Fiscal_Year == false)
                     {
                        fiscaldate = DateUtil.ToDate(com.Custom_Fiscal_Year.Value.Day.ToString("00") + "/" + com.Custom_Fiscal_Year.Value.Month.ToString("00") + "/" + NumUtil.ParseInteger(currentyear));
                     }

                     var currentdateofyear = DateUtil.ToDate(currentdate.Day, currentdate.Month, i);
                     if (currentdateofyear == null)
                     {
                        if (currentdate.Day == 29 && currentdate.Month == 2)
                           currentdate = currentdate.Date.AddDays(-1);

                        currentdateofyear = DateUtil.ToDate(currentdate.Day, currentdate.Month, i);
                     }
                     var workspan = (currentdateofyear.Value.Date - hiredDate.Value.Date);
                     yearservice = NumUtil.ParseInteger(Math.Floor((workspan.TotalDays + 1) / 365));
                     if (yearservice < 0)
                        yearservice = 0;

                     var hiredateofyear = DateUtil.ToDate(hiredDate.Value.Day.ToString("00") + "/" + hiredDate.Value.Month.ToString("00") + "/" + currentyear);
                     if (hiredateofyear <= currentdate)
                     {
                        var hist = (from a in db.Employment_History
                                    where a.Employee_Profile_ID == cri.Emp.Employee_Profile_ID &
                                    a.Effective_Date <= hiredateofyear &
                                    a.Effective_Date <= currentdate
                                    orderby a.Effective_Date descending
                                    select a).FirstOrDefault();
                        if (hist != null)
                        {
                           var workspan2 = (hiredateofyear.Value.Date - hiredDate.Value.Date);
                           var yearservice2 = NumUtil.ParseInteger(Math.Floor((workspan2.TotalDays + 1) / 365));
                           if (yearservice2 < 0) yearservice2 = 0;

                           Leave_Config_Detail lconfigDetail = cri.Leave_Config
                               .Leave_Config_Detail
                               .Where(w => (w.Designation_ID == hist.Designation_ID || w.Designation_ID == null) & w.Year_Service <= yearservice2)
                               .OrderByDescending(o => o.Year_Service)
                               .FirstOrDefault();

                           if (lconfigDetail != null)
                           {
                              var cal = (from a in cals where a.Start_Date == hiredateofyear select a).FirstOrDefault();
                              if (cal == null)
                              {
                                 var newcal = new Leave_Calculation()
                                 {
                                    Employee_Profile_ID = cri.Emp.Employee_Profile_ID,
                                    Entitle = lconfigDetail.Default_Leave_Amount,
                                    Leave_Config_ID = cri.Leave_Config.Leave_Config_ID,
                                    Designation_ID = lconfigDetail.Designation_ID,
                                    Year_Assigned = currentyear,
                                    Start_Date = hiredateofyear,
                                    Adjustment = 0,
                                    Bring_Forward = 0,
                                    CutOff = 0,
                                    Leave_Used = 0,
                                    Create_On = currentdate,
                                    Create_By = cri.Emp.User_Profile.User_Authentication.Email_Address,
                                    Update_On = currentdate,
                                    Update_By = cri.Emp.User_Profile.User_Authentication.Email_Address
                                 };
                                 if (currentdateofyear <= fiscaldate)
                                 {
                                    if (com.Default_Fiscal_Year == false)
                                    {
                                       if (com.Custom_Fiscal_Year.Value.Day == 31 && com.Custom_Fiscal_Year.Value.Month == 12)
                                       {
                                          newcal.Year_Assigned = (NumUtil.ParseInteger(currentyear) - 1).ToString();
                                       }
                                    }
                                 }

                                 cals.Add(newcal);
                                 db.Leave_Calculation.Add(newcal);
                              }
                              else
                              {
                                 if (cri.Leave_Config_ID == 1021)
                                    Debug.Write(cri.Leave_Config_ID);

                                 cal.Leave_Config_ID = cri.Leave_Config.Leave_Config_ID;
                                 cal.Designation_ID = lconfigDetail.Designation_ID;
                                 cal.Entitle = lconfigDetail.Default_Leave_Amount;
                              }
                           }
                        }
                     }



                     var emphists = (from a in db.Employment_History
                                     where a.Employee_Profile_ID == cri.Emp.Employee_Profile_ID &
                                     a.Effective_Date.Value.Year <= i &
                                     a.Effective_Date <= currentdate
                                     orderby a.Effective_Date
                                     select a);


                     foreach (var hist in emphists)
                     {
                        var yesrhist = DateUtil.ToDate(hist.Effective_Date.Value.Day + "/" + hist.Effective_Date.Value.Month + "/" + currentyear);

                        var workspan2 = (yesrhist.Value.Date - hiredDate.Value.Date);
                        var yearservice2 = NumUtil.ParseInteger(Math.Floor((workspan2.TotalDays + 1) / 365));
                        if (yearservice2 < 0) yearservice2 = 0;

                        Leave_Config_Detail lconfigDetail = cri.Leave_Config
                            .Leave_Config_Detail
                            .Where(w => (w.Designation_ID == hist.Designation_ID || w.Designation_ID == null) & w.Year_Service <= yearservice2)
                            .OrderByDescending(o => o.Year_Service)
                            .FirstOrDefault();

                        if (lconfigDetail != null)
                        {
                           if (yesrhist <= currentdate)
                           {
                              var cal = (from a in cals where a.Start_Date == yesrhist select a).FirstOrDefault();
                              if (cal == null)
                              {
                                 // new cal
                                 var newcal = new Leave_Calculation()
                                 {
                                    Employee_Profile_ID = cri.Emp.Employee_Profile_ID,
                                    Entitle = lconfigDetail.Default_Leave_Amount,
                                    Leave_Config_ID = cri.Leave_Config.Leave_Config_ID,
                                    Designation_ID = lconfigDetail.Designation_ID,
                                    Year_Assigned = currentyear,
                                    Start_Date = yesrhist,
                                    Adjustment = 0,
                                    Bring_Forward = 0,
                                    CutOff = 0,
                                    Leave_Used = 0,
                                    Create_On = currentdate,
                                    Create_By = cri.Emp.User_Profile.User_Authentication.Email_Address,
                                    Update_On = currentdate,
                                    Update_By = cri.Emp.User_Profile.User_Authentication.Email_Address

                                 };
                                 if (com.Default_Fiscal_Year == false)
                                 {
                                    fiscaldate = DateUtil.ToDate(com.Custom_Fiscal_Year.Value.Day.ToString("00") + "/" + com.Custom_Fiscal_Year.Value.Month.ToString("00") + "/" + NumUtil.ParseInteger(currentyear));
                                    if (currentdateofyear <= fiscaldate)
                                    {
                                       if (com.Custom_Fiscal_Year.Value.Day == 31 && com.Custom_Fiscal_Year.Value.Month == 12)
                                          newcal.Year_Assigned = (NumUtil.ParseInteger(currentyear) - 1).ToString();
                                    }
                                 }
                                 cals.Add(newcal);
                                 db.Leave_Calculation.Add(newcal);
                              }
                              else
                              {
                                 cal.Leave_Config_ID = cri.Leave_Config.Leave_Config_ID;
                                 cal.Designation_ID = lconfigDetail.Designation_ID;
                                 cal.Entitle = lconfigDetail.Default_Leave_Amount;
                              }
                           }
                        }



                     }

                     var currentcal = (from a in cals where a.Year_Assigned != null && int.Parse(a.Year_Assigned) <= int.Parse(currentyear) orderby a.Start_Date descending select a).FirstOrDefault();
                     var yearAssign = currentyear;
                     if (currentcal != null)
                     {
                        cri.Leave_Config = db.Leave_Config.Where(w => w.Leave_Config_ID == currentcal.Leave_Config_ID).FirstOrDefault();
                        if (cri.Leave_Config != null)
                        {

                           if (com.Default_Fiscal_Year == false)
                           {
                              fiscaldate = DateUtil.ToDate(com.Custom_Fiscal_Year.Value.Day.ToString("00") + "/" + com.Custom_Fiscal_Year.Value.Month.ToString("00") + "/" + NumUtil.ParseInteger(currentyear));
                              if (com.Custom_Fiscal_Year.Value.Day == 31 && com.Custom_Fiscal_Year.Value.Month == 12)
                                 yearAssign = (NumUtil.ParseInteger(currentyear) - 1).ToString();
                           }
                           else
                              fiscaldate = DateUtil.ToDate("31/12/" + currentyear);

                           if (fiscaldate <= currentdate)
                           {
                              int yearAssignInt = NumUtil.ParseInteger(yearAssign);
                              var startdate = fiscaldate.Value.AddDays(1);

                              var workspan2 = (startdate.Date - hiredDate.Value.Date);
                              var yearservice2 = NumUtil.ParseInteger(Math.Floor((workspan2.TotalDays + 1) / 365));
                              if (yearservice2 < 0) yearservice2 = 0;

                              var lconfigDetail = cri.Leave_Config.Leave_Config_Detail
                          .Where(w => w.Leave_Config_ID == currentcal.Leave_Config_ID && w.Designation_ID == currentcal.Designation_ID & w.Year_Service <= yearservice2)
                              .OrderByDescending(o => o.Year_Service)
                          .FirstOrDefault();
                              if (lconfigDetail != null)
                              {
                                 var cal = (from a in cals where a.Employee_Profile_ID == cri.Emp.Employee_Profile_ID & a.Start_Date == startdate select a).FirstOrDefault();

                                 var leavecals = (from a in cals where a.Year_Assigned == yearAssign select (a.Entitle.HasValue ? a.Entitle : 0));
                                 var entitle = leavecals.Sum() / (leavecals.Count() > 0 ? leavecals.Count() : 1);
                                 var totalentitle = entitle;

                                 var adjustment = db.Leave_Adjustment
                                     .Where(w => w.Year_2 == yearAssignInt && w.Company_ID == cri.Emp.User_Profile.Company_ID
                                         && (w.Employee_Profile_ID.HasValue ? w.Employee_Profile_ID == cri.Emp.Employee_Profile_ID : true)
                                         && w.Leave_Config_ID == cri.Leave_Config_ID)
                                         .Select(s => (s.Adjustment_Amount.HasValue ? s.Adjustment_Amount.Value : 0))
                                         .ToList()
                                         .Sum();

                                 var leaveuse = (from a in cals where a.Year_Assigned == yearAssign select (a.Leave_Used.HasValue ? a.Leave_Used : 0)).Sum();
                                 var bpercen = (cri.Leave_Config.Bring_Forward_Percent.HasValue ? cri.Leave_Config.Bring_Forward_Percent : 0);

                                 if (cal == null)
                                 {
                                    var newcal = new Leave_Calculation()
                                    {
                                       Employee_Profile_ID = cri.Emp.Employee_Profile_ID,
                                       Entitle = lconfigDetail.Default_Leave_Amount,
                                       Leave_Config_ID = cri.Leave_Config.Leave_Config_ID,
                                       Designation_ID = lconfigDetail.Designation_ID,
                                       Year_Assigned = (NumUtil.ParseInteger(currentyear) + 1).ToString(),
                                       Start_Date = startdate,
                                       Adjustment = 0,
                                       Bring_Forward = 0,
                                       CutOff = 0,
                                       Leave_Used = 0,
                                       Create_On = currentdate,
                                       Create_By = cri.Emp.User_Profile.User_Authentication.Email_Address,
                                       Update_On = currentdate,
                                       Update_By = cri.Emp.User_Profile.User_Authentication.Email_Address
                                    };

                                    if (com.Default_Fiscal_Year == false)
                                    {
                                       if (com.Custom_Fiscal_Year.Value.Day == 31 && com.Custom_Fiscal_Year.Value.Month == 12)
                                          newcal.Year_Assigned = currentyear;
                                    }

                                    var histcontract = emphists.Where(w => startdate >= w.Contract_Start_Date & startdate <= w.Contract_End_Date & w.Contract_Staff == true).FirstOrDefault();

                                    if (cri.Leave_Config.Bring_Forward == true && histcontract == null & fiscaldate >= com.Registration_Date)
                                    {
                                       if (adjustment >= leaveuse)
                                          totalentitle = totalentitle + (adjustment - leaveuse);
                                       else
                                          totalentitle = totalentitle - (leaveuse - adjustment);

                                       var is29 = false;
                                       if (DateTime.DaysInMonth(NumUtil.ParseInteger(currentyear), 2) == 29)
                                          is29 = true;

                                       // Get Leave left
                                       decimal leaveleft = 0;
                                       var lcnt = 0;
                                       var lcals = cals.Where(w => w.Year_Assigned == currentyear).OrderBy(o => o.Start_Date).ToList();
                                       var workdayEntitle = 0M;
                                       foreach (var a in lcals)
                                       {
                                          var lentitle = (a.Entitle.HasValue ? a.Entitle : 0);
                                          var lleaveuse = (a.Leave_Used.HasValue ? a.Leave_Used : 0);
                                          Nullable<DateTime> expiredate = null;
                                          var workday = 1;
                                          if (lcals.Count > lcnt + 1)
                                             expiredate = lcals[lcnt + 1].Start_Date.Value.AddDays(-1);

                                          if (a.Start_Date.HasValue && expiredate.HasValue)
                                             workday = (int)(expiredate.Value.Date - a.Start_Date.Value.Date).TotalDays + 1;
                                          else
                                             workday = (int)(fiscaldate.Value.Date - a.Start_Date.Value.Date).TotalDays + 1;

                                          workdayEntitle += (decimal)((lentitle * workday) / (is29 == true ? 366 : 365));
                                          var left = ((lentitle * workday) / (is29 == true ? 366 : 365)) - lleaveuse;
                                          leaveleft = leaveleft + left.Value;
                                          lcnt++;
                                       }


                                       decimal canbring = (decimal)((workdayEntitle * bpercen) / 100);
                                       decimal bringforwardpercen = leaveleft;
                                       if (bringforwardpercen > canbring)
                                          bringforwardpercen = canbring;

                                       if (totalentitle > bringforwardpercen)
                                       {
                                          if (bringforwardpercen < 0)
                                             bringforwardpercen = 0;

                                          newcal.Bring_Forward = bringforwardpercen;
                                       }
                                       else
                                       {
                                          if (totalentitle < 0)
                                             totalentitle = 0;

                                          newcal.Bring_Forward = totalentitle;
                                       }
                                    }
                                    cals.Add(newcal);
                                    db.Leave_Calculation.Add(newcal);
                                 }
                                 else
                                 {
                                    var histcontract = emphists.Where(w => startdate >= w.Contract_Start_Date & startdate <= w.Contract_End_Date & w.Contract_Staff == true).FirstOrDefault();

                                    cal.Bring_Forward = 0;
                                    cal.Entitle = lconfigDetail.Default_Leave_Amount;
                                    if (cri.Leave_Config.Bring_Forward == true && histcontract == null & fiscaldate >= com.Registration_Date)
                                    {
                                       if (adjustment >= leaveuse)
                                          totalentitle = totalentitle + (adjustment - leaveuse);
                                       else
                                          totalentitle = totalentitle - (leaveuse - adjustment);

                                       var is29 = false;
                                       if (DateTime.DaysInMonth(NumUtil.ParseInteger(currentyear), 2) == 29)
                                          is29 = true;

                                       // Get Leave left
                                       decimal leaveleft = 0;
                                       var lcnt = 0;
                                       var lcals = cals.Where(w => w.Year_Assigned == currentyear).OrderBy(o => o.Start_Date).ToList();

                                       var workdayEntitle = 0M;
                                       foreach (var a in lcals)
                                       {
                                          var lentitle = (a.Entitle.HasValue ? a.Entitle : 0);
                                          var lleaveuse = (a.Leave_Used.HasValue ? a.Leave_Used : 0);
                                          Nullable<DateTime> expiredate = null;
                                          var workday = 1;
                                          if (lcals.Count > lcnt + 1)
                                             expiredate = lcals[lcnt + 1].Start_Date.Value.AddDays(-1);

                                          if (a.Start_Date.HasValue && expiredate.HasValue)
                                             workday = (int)(expiredate.Value.Date - a.Start_Date.Value.Date).TotalDays + 1;
                                          else
                                             workday = (int)(fiscaldate.Value.Date - a.Start_Date.Value.Date).TotalDays + 1;

                                          workdayEntitle += (decimal)((lentitle * workday) / (is29 == true ? 366 : 365));
                                          var left = ((lentitle * workday) / (is29 == true ? 366 : 365)) - lleaveuse;
                                          leaveleft = leaveleft + left.Value;
                                          lcnt++;
                                       }


                                       decimal canbring = (decimal)((workdayEntitle * bpercen) / 100);
                                       decimal bringforwardpercen = leaveleft;
                                       if (bringforwardpercen > canbring)
                                          bringforwardpercen = canbring;

                                       if (totalentitle > bringforwardpercen)
                                       {
                                          if (bringforwardpercen < 0)
                                             bringforwardpercen = 0;

                                          cal.Bring_Forward = bringforwardpercen;
                                       }
                                       else
                                       {
                                          if (totalentitle < 0)
                                             totalentitle = 0;

                                          cal.Bring_Forward = totalentitle;
                                       }
                                    }

                                 }
                              }

                           }




                        }


                     }

                  }


                  //manage cut off
                  foreach (var cal in cals)
                  {
                     if (cal.Leave_Config == null)
                        cal.Leave_Config = (from a in db.Leave_Config where a.Leave_Config_ID == cal.Leave_Config_ID select a).FirstOrDefault();

                     if (cal.Leave_Config.Months_To_Expiry.HasValue && cal.Leave_Config.Months_To_Expiry.Value > 0)
                     {
                        if (cal.Bring_Forward.HasValue && cal.Bring_Forward.Value > 0)
                        {
                           var lastdayinmonth = DateTime.DaysInMonth(Convert.ToInt32(cal.Year_Assigned), Convert.ToInt32(cal.Leave_Config.Months_To_Expiry.Value));
                           var expirydate = DateUtil.ToDate(lastdayinmonth, Convert.ToInt32(cal.Leave_Config.Months_To_Expiry.Value), Convert.ToInt32(cal.Year_Assigned));
                           if (currentdate >= expirydate)
                              cal.CutOff = cal.Bring_Forward;
                        }
                     }

                  }

                  db.SaveChanges();

                  var leaveconfigs = (from a in db.Leave_Calculation where a.Employee_Profile_ID == cri.Emp.Employee_Profile_ID select a.Leave_Config_ID).Distinct();

                  foreach (var lconfig in leaveconfigs)
                  {
                     var cals2 = (from a in db.Leave_Calculation
                                  where a.Employee_Profile_ID == cri.Emp.Employee_Profile_ID &
                                  a.Leave_Config_ID == lconfig
                                  orderby a.Start_Date descending
                                  select a);

                     Nullable<DateTime> enddate = null;
                     var cnt = 0;
                     foreach (var cal in cals2)
                     {
                        if (cal.Start_Date.HasValue)
                        {
                           if (cnt == 0)
                              enddate = cal.Start_Date.Value.AddDays(-1);
                           else
                           {
                              cal.Expiry_Date = enddate;
                              enddate = cal.Start_Date.Value.AddDays(-1);
                           }

                           //var used = db.Leave_Application_Document
                           //           .Where(w => w.Overall_Status == SBSWorkFlowAPI.Constants.WorkflowStatus.Closed
                           //              && string.IsNullOrEmpty(w.Cancel_Status)
                           //              && w.Employee_Profile_ID == cal.Employee_Profile_ID
                           //              && w.Leave_Config_ID == cal.Leave_Config_ID);
                           //              //&&  DbFunctions.TruncateTime(w.Start_Date)  >= DbFunctions.TruncateTime(cal.Start_Date));
                           //var leaveused = 0M;
                           //foreach (var item in used)
                           //{
                           //   if (item.Start_Date >= cal.Start_Date)
                           //   {
                           //      if (cal.Expiry_Date.HasValue)
                           //      {
                           //         if (item.Start_Date <= cal.Expiry_Date)
                           //         leaveused += item.Days_Taken.HasValue ? item.Days_Taken.Value : 0;
                           //      }
                           //      else
                           //         leaveused += item.Days_Taken.HasValue ? item.Days_Taken.Value : 0;

                           //   }
                           //}
                           //cal.Leave_Used = leaveused;
                        }
                        cnt++;
                     }
                  }
                  db.SaveChanges();

               }
               else
               {
                  Relationship child = null;
                  if (cri.Leave_Config.Leave_Name == LeaveType.AdoptionLeave)
                     child = cri.Emp.Relationships.Where(w => w.Child_Type == ChildType.AdoptedChild).OrderByDescending(o => o.DOB).FirstOrDefault();
                  else if (cri.Leave_Config.Leave_Name == LeaveType.MaternityLeave)
                     child = cri.Emp.Relationships.Where(w => w.Child_Type == ChildType.OwnChild).OrderByDescending(o => o.DOB).FirstOrDefault();
                  else if (cri.Leave_Config.Leave_Name == LeaveType.PaternityLeave)
                     child = cri.Emp.Relationships.Where(w => w.Child_Type == ChildType.OwnChild).OrderByDescending(o => o.DOB).FirstOrDefault();
                  //else if (cri.Leave_Config.Leave_Name == LeaveType.ChildCareLeave)
                  //   child = cri.Emp.Relationships.OrderByDescending(o => o.DOB).FirstOrDefault();

                  if (child == null)
                     return new ServiceResult() { Code = ERROR_CODE.SUCCESS, Msg = new Error().getError(ERROR_CODE.ERROR_511_DATA_NOT_FOUND), Field = "Child" };

                  var lconfigDetail = cri.Leave_Config.Leave_Config_Child_Detail.Where(w => w.Residential_Status == cri.Emp.Residential_Status).FirstOrDefault();
                  if (lconfigDetail == null)
                     return new ServiceResult() { Code = ERROR_CODE.ERROR_511_DATA_NOT_FOUND, Msg = new Error().getError(ERROR_CODE.ERROR_511_DATA_NOT_FOUND), Field = "Default Leave Amount" };

                  var cal = (from a in db.Leave_Calculation
                             where a.Employee_Profile_ID == cri.Emp.Employee_Profile_ID &
                             a.Leave_Config.Leave_Config_ID == cri.Leave_Config_ID &
                             a.Relationship_ID == child.Relationship_ID
                             select a).FirstOrDefault();
                  if (cal == null)
                  {
                     // add new call
                     var newcal = new Leave_Calculation();
                     newcal.Employee_Profile_ID = cri.Emp.Employee_Profile_ID;
                     newcal.Relationship_ID = child.Relationship_ID;
                     newcal.Leave_Config_ID = cri.Leave_Config.Leave_Config_ID;
                     newcal.Adjustment = 0;
                     newcal.Bring_Forward = 0;
                     newcal.CutOff = 0;
                     newcal.Leave_Used = 0;
                     newcal.Entitle = lconfigDetail.Default_Leave_Amount;
                     newcal.Create_On = currentdate;
                     newcal.Create_By = cri.Emp.User_Profile.User_Authentication.Email_Address;
                     db.Leave_Calculation.Add(newcal);
                  }
                  else
                  {
                     // edit call
                     cal.Update_On = currentdate;
                     cal.Update_By = cri.Emp.User_Profile.User_Authentication.Email_Address;
                  }
                  db.SaveChanges();
               }
            }
         }
         catch
         {
            return new ServiceResult() { Code = ERROR_CODE.ERROR_513_CAL_LEAVE_ERROR, Msg = new Error().getError(ERROR_CODE.ERROR_513_CAL_LEAVE_ERROR) };
         }
         return new ServiceResult() { Code = ERROR_CODE.SUCCESS, Msg = new Success().getSuccess(ERROR_CODE.SUCCESS) };
      }

      public Leave_Config GetLeaveConfig(Nullable<int> pLeaveConfigID = null)
      {
         var currentdate = StoredProcedure.GetCurrentDate();
         using (var db = new SBS2DBContext())
         {
            return db.Leave_Config.Where(w => w.Leave_Config_ID == pLeaveConfigID).FirstOrDefault();
         }
      }

      //Added bby sun 16-02-2016
      public ServiceResult UpdateLeaveStatusRequest(Nullable<int> pDocID, string Status, Nullable<int> pReqCancelID)
      {
         try
         {
            var currentdate = StoredProcedure.GetCurrentDate();
            using (var db = new SBS2DBContext())
            {
               var current = (from a in db.Leave_Application_Document where a.Leave_Application_Document_ID == pDocID.Value select a).FirstOrDefault();
               if (current != null)
               {
                  current.Request_Cancel_ID = pReqCancelID;
                  current.Cancel_Status = Status;
                  db.SaveChanges();
                  return new ServiceResult() { Code = ERROR_CODE.SUCCESS, Msg = new Success().getSuccess(ERROR_CODE.SUCCESS_EDIT), Field = Resource.Leave };

               }
            }
         }
         catch
         {
            return new ServiceResult() { Code = ERROR_CODE.ERROR_504_UPDATE_ERROR, Msg = new Error().getError(ERROR_CODE.ERROR_504_UPDATE_ERROR), Field = Resource.Leave };
         }
         return new ServiceResult() { Code = ERROR_CODE.ERROR_511_DATA_NOT_FOUND, Msg = new Error().getError(ERROR_CODE.ERROR_511_DATA_NOT_FOUND), Field = Resource.Leave };

      }

      //Edit by Jane 03-02-2016
      public Leave_Left CalculateLeaveLeft(LeaveTypeCriteria cri)
      {

         var currentdate = StoredProcedure.GetCurrentDate();
         using (var db = new SBS2DBContext())
         {
            if (!cri.Profile_ID.HasValue)
               return new Leave_Left();
            if (!cri.Leave_Config_ID.HasValue)
               return new Leave_Left();

            if (cri.Leave_Config == null)
               cri.Leave_Config = db.Leave_Config.Where(w => w.Leave_Config_ID == cri.Leave_Config_ID).FirstOrDefault();

            if (cri.Leave_Config == null)
               return new Leave_Left();

            if (cri.Emp == null)
               cri.Emp = (from a in db.Employee_Profile where a.Profile_ID == cri.Profile_ID select a).FirstOrDefault();

            if (cri.Emp == null)
               return new Leave_Left();

            if (cri.firsthist == null)
               cri.firsthist = db.Employment_History.Where(w => w.Employee_Profile.Profile_ID == cri.Profile_ID).OrderBy(o => o.Effective_Date).FirstOrDefault();

            if (cri.firsthist == null)
               return new Leave_Left();

            var hiredDate = currentdate;
            if (cri.firsthist != null && cri.firsthist.Effective_Date.HasValue)
               hiredDate = cri.firsthist.Effective_Date.Value;


            Debug.WriteLine("'***********APP DEBUG***********' " + DateTime.Now + "-Start sevice Get Leave left-" + cri.Leave_Config.Leave_Name);
            if (cri.Leave_Config.Type == LeaveConfigType.Normal)
            {
               var currentcal = new Leave_Calculation();
               if (cri.Year.HasValue && cri.Year.Value > 0)
               {
                  //Added by sun 29-12-2015
                  currentcal = (from a in db.Leave_Calculation
                                where a.Leave_Config.Leave_Config_ID == cri.Leave_Config_ID
                                & a.Start_Date <= currentdate
                                & a.Employee_Profile.Profile_ID == cri.Profile_ID
                                & a.Year_Assigned == cri.Year.Value.ToString()
                                orderby a.Start_Date descending
                                select a).FirstOrDefault();
               }
               else
               {
                  currentcal = (from a in db.Leave_Calculation
                                where a.Leave_Config.Leave_Config_ID == cri.Leave_Config_ID
                                & a.Start_Date <= currentdate
                                & a.Employee_Profile.Profile_ID == cri.Profile_ID
                                orderby a.Start_Date descending
                                select a).FirstOrDefault();
               }

               if (currentcal == null)
                  return new Leave_Left();

               decimal leaveleft = 0;
               var adjustAmount = 0M;
               var totalEntitle = 0M;
               var leaveleftstr = "";
               var totalleaveused = 0M;
               var totalbringforward = 0M;

               //Added by sun 08-02-2016
               var entitleAddExtra = 0M;
               var finalEntitleAll = 0M;
               //Added by sun 14-02-2016
               var extarAmount = 0M;

               var hist = db.Employment_History
                  .Where(w => w.Employee_Profile_ID == cri.Emp.Employee_Profile_ID & w.Effective_Date <= currentdate)
                  .OrderByDescending(o => o.Effective_Date)
                  .FirstOrDefault();

               if (hist == null)
                  return new Leave_Left();

               var lconfigDetail = cri.Leave_Config
                 .Leave_Config_Detail
                 .Where(w => w.Year_Service <= cri.Year_Service & (w.Designation_ID == hist.Designation_ID || w.Designation_ID == null))
                 .OrderByDescending(o => o.Year_Service)
                 .FirstOrDefault();

               if (lconfigDetail == null)
                  return new Leave_Left();

               IQueryable<Leave_Calculation> yearcals;
               if (hist.Contract_Staff.HasValue && hist.Contract_Staff.Value)
               {
                  yearcals = db.Leave_Calculation
                      .Where(w => w.Leave_Config.Leave_Config_ID == cri.Leave_Config_ID
                          & w.Employee_Profile.Profile_ID == cri.Profile_ID
                          & w.Start_Date >= hist.Contract_Start_Date
                          & w.Start_Date <= hist.Contract_End_Date)
                          .OrderBy(o => o.Start_Date);

                  var workday = (int)(currentdate.Date - hist.Contract_Start_Date.Value.Date).TotalDays + 1;
                  var contractday = (int)(hist.Contract_End_Date.Value.Date - hist.Contract_Start_Date.Value.Date).TotalDays + 1;

                  var leaveuse = 0M;
                  foreach (var a in yearcals)
                  {
                     totalEntitle += (a.Entitle.HasValue ? a.Entitle.Value : 0);
                     leaveuse += (a.Leave_Used.HasValue ? a.Leave_Used.Value : 0);
                  }

                  if (yearcals.Count() > 0)
                     totalEntitle = totalEntitle / yearcals.Count();

                  var left = ((totalEntitle * workday) / contractday) - (leaveuse);
                  leaveleft = leaveleft + left;
                  totalleaveused = leaveuse;
               }
               else
               {
                  yearcals = db.Leave_Calculation
                      .Where(w => w.Year_Assigned == currentcal.Year_Assigned
                          & w.Leave_Config.Leave_Config_ID == cri.Leave_Config_ID
                          & w.Employee_Profile.Profile_ID == cri.Profile_ID)
                          .OrderBy(o => o.Start_Date);


                  int yearAssignInt = NumUtil.ParseInteger(currentcal.Year_Assigned);

                  var adjusts = db.Leave_Adjustment.Where(w => w.Year_2 == yearAssignInt && w.Company_ID == cri.Emp.User_Profile.Company_ID && w.Leave_Config.Leave_Config_ID == cri.Leave_Config_ID & w.Record_Status != RecordStatus.Delete);
                  foreach (var row in adjusts)
                  {
                     if (hist != null && row.Department_ID.HasValue)
                     {
                        if (row.Department_ID == hist.Department_ID)
                        {
                           if (row.Employee_Profile_ID.HasValue)
                           {
                              if (row.Employee_Profile_ID == cri.Emp.Employee_Profile_ID)
                                 adjustAmount += (row.Adjustment_Amount.HasValue ? row.Adjustment_Amount.Value : 0);
                           }
                           else
                              adjustAmount += (row.Adjustment_Amount.HasValue ? row.Adjustment_Amount.Value : 0);
                        }
                     }
                     else
                     {
                        if (row.Employee_Profile_ID.HasValue)
                        {
                           if (row.Employee_Profile_ID == cri.Emp.Employee_Profile_ID)
                              adjustAmount += (row.Adjustment_Amount.HasValue ? row.Adjustment_Amount.Value : 0);
                        }
                        else
                           adjustAmount += (row.Adjustment_Amount.HasValue ? row.Adjustment_Amount.Value : 0);
                     }
                  }

                  var empadjust = db.Leave_Config_Extra.Where(w => w.Employee_Profile_ID == cri.Emp.Employee_Profile_ID & w.Leave_Config_ID == cri.Leave_Config_ID);
                  foreach (var row in empadjust)
                  {
                     //Add by sun 08-02-2016
                     entitleAddExtra += row.No_Of_Days.HasValue ? row.No_Of_Days.Value : 0;

                     //Edit by sun 14-02-2016
                     extarAmount += row.No_Of_Days.HasValue ? row.No_Of_Days.Value : 0;
                  }

                  if (!cri.Leave_Config.Leave_Config_Parent_ID.HasValue)
                  {
                     var isAcc = true;
                     if (cri.Leave_Config.Is_Accumulative == null || cri.Leave_Config.Is_Accumulative == false)
                        isAcc = false;

                     var lastdayofyear = DateUtil.ToDate("31/12/" + currentdate.Year);
                     var com = db.Company_Details.Where(w => w.Company_ID == cri.Company_ID).FirstOrDefault();
                     if (com != null && com.Default_Fiscal_Year == false)
                        lastdayofyear = DateUtil.ToDate(com.Custom_Fiscal_Year.Value.Day.ToString("00") + "/" + com.Custom_Fiscal_Year.Value.Month.ToString("00") + "/" + currentdate.Year);

                     if (lastdayofyear < currentdate)
                     {
                        lastdayofyear = lastdayofyear.Value.AddYears(1);
                     }
                     foreach (var a in yearcals)
                     {
                        var entitle = (a.Entitle.HasValue ? a.Entitle.Value : 0);
                        var leaveuse = (a.Leave_Used.HasValue ? a.Leave_Used.Value : 0);
                        var bringforward = (a.Bring_Forward.HasValue ? a.Bring_Forward : 0);
                        var cutoff = (a.CutOff.HasValue ? a.CutOff : 0);

                        var workday = 1;
                        if (a.Start_Date.HasValue && a.Expiry_Date.HasValue)
                           workday = (int)(a.Expiry_Date.Value.Date - a.Start_Date.Value.Date).TotalDays + 1;
                        else
                        {
                           if (isAcc)
                              workday = (int)(currentdate.Date - a.Start_Date.Value.Date).TotalDays + 1;
                           else
                              workday = (int)(lastdayofyear.Value.Date - a.Start_Date.Value.Date).TotalDays + 1;
                        }

                        //Edit By sun 14-02-2017
                        entitle += extarAmount;
                        totalbringforward += bringforward.HasValue ? bringforward.Value : 0;
                        totalEntitle += ((entitle * workday) / 365);
                        var left = ((entitle * workday) / 365) + (bringforward - cutoff) - (leaveuse - cutoff);
                        leaveleft = leaveleft + left.Value;
                        totalleaveused += leaveuse;
                     }
                  }
                  else
                  {
                     totalbringforward += currentcal.Bring_Forward.HasValue ? currentcal.Bring_Forward.Value : 0;
                     totalEntitle = (currentcal.Entitle.HasValue ? currentcal.Entitle.Value : 0);
                     var leaveuse = (currentcal.Leave_Used.HasValue ? currentcal.Leave_Used.Value : 0);
                     leaveleft = totalEntitle - leaveuse;
                     totalleaveused = leaveuse;
                  }

                  leaveleft += adjustAmount;
                  //Add by sun 08-02-2016
                  finalEntitleAll = entitleAddExtra;
               }

               if (cri.Leave_Config.Leave_Name == LeaveType.ChildCareLeave)
               {
                  var child = cri.Emp.Relationships.Where(w => w.Employee_Profile_ID == cri.Emp.Employee_Profile_ID).OrderByDescending(o => o.DOB).FirstOrDefault();
                  if (child != null)
                  {
                     if (child.Relationship1 != null)
                     {
                        var relationshipName = db.Global_Lookup_Data.Where(w => w.Lookup_Data_ID == child.Relationship1.Value).FirstOrDefault();
                        if (child.Child_Type == ChildType.OwnChild && (relationshipName.Description.ToUpper() == "SON" || relationshipName.Description.ToUpper() == "DAUGHTER"))
                        {
                           var tWorkExp = currentdate - cri.firsthist.Confirm_Date;
                           var workExp = Convert.ToDouble(tWorkExp.Value.TotalDays);
                           var t = currentdate - child.DOB;
                           var age = Convert.ToDouble(t.Value.TotalDays) / 365;
                           if ((age <= 12) && (child.Nationality.Name == "SG") && (workExp > 90))
                           {
                              if (age >= 7 && age <= 12)
                              {
                                 leaveleft += 2;
                                 finalEntitleAll += 2;
                              }
                           }
                        }
                     }
                  }
               }

               //Add by sun 08-02-2016
               finalEntitleAll += lconfigDetail.Default_Leave_Amount.HasValue ? lconfigDetail.Default_Leave_Amount.Value : 0;

               leaveleftstr = NumUtil.ParseDecimal(leaveleft.ToString()).ToString("#,##0.00");
               return new Leave_Left()
               {
                  Left = NumUtil.ParseDecimal(leaveleftstr),
                  Entitle = totalEntitle,
                  EntitleAll = finalEntitleAll,
                  Leave_Used = totalleaveused,
                  BringForward = totalbringforward,
               };

            }
            else
            {
               // child leave (Adoption Leave, Maternity Leave,Paternity Leave,Child Care Leave)

               var currentcal = (from a in db.Leave_Calculation
                                 where a.Leave_Config.Leave_Config_ID == cri.Leave_Config_ID
                                 & a.Employee_Profile.Profile_ID == cri.Profile_ID
                                 & a.Relationship_ID == cri.Relationship_ID
                                 orderby a.Start_Date descending
                                 select a).FirstOrDefault();

               if (currentcal != null)
               {
                  var child = currentcal.Relationship;
                  //var startdate = DateUtil.ToDate(pStartDate);
                  var workingdays = 7M;
                  var workdays = db.Working_Days.Where(w => w.Company_ID == cri.Emp.User_Profile.Company_ID).FirstOrDefault();
                  if (workdays != null)
                     workingdays = workdays.Days.HasValue ? workdays.Days.Value : 7;

                  var lconfigDetail = db.Leave_Config_Child_Detail.Where(w => w.Leave_Config_ID == cri.Leave_Config.Leave_Config_ID && cri.Emp.Residential_Status == w.Residential_Status).FirstOrDefault();
                  if (lconfigDetail != null)
                  {
                     if (cri.Leave_Config.Leave_Name == LeaveType.MaternityLeave)
                     {
                        var defaultWeeksAmount = lconfigDetail.Default_Leave_Amount.HasValue ? lconfigDetail.Default_Leave_Amount.Value : 0;
                        if (child.Is_Maternity_Share_Father.HasValue && child.Is_Maternity_Share_Father.Value)
                        {
                           var shareWeekAmount = 1;
                           defaultWeeksAmount -= shareWeekAmount;
                        }


                        if (cri.Leave_Config.Flexibly.HasValue && cri.Leave_Config.Flexibly.Value)
                        {
                           // Flexibly
                           if (currentcal.Leave_Used == 0)
                           {
                              // Before taking your leave (Flexible)
                              if (lconfigDetail.Period == TimePeriod.Weeks)
                                 return new Leave_Left() { Weeks_Left = defaultWeeksAmount, Is_First_Period = true, Entitle = defaultWeeksAmount, EntitleAll = defaultWeeksAmount, Leave_Used = 0 };
                           }
                           else
                           {
                              // after taking your leave (Flexible)
                              if (lconfigDetail.Period == TimePeriod.Weeks)
                              {
                                 decimal weekremainweeks = defaultWeeksAmount - (currentcal.Leave_Used.HasValue ? currentcal.Leave_Used.Value : 0);
                                 return new Leave_Left() { Weeks_Left = weekremainweeks, Is_First_Period = false, Entitle = defaultWeeksAmount, EntitleAll = defaultWeeksAmount, Leave_Used = (currentcal.Leave_Used.HasValue ? currentcal.Leave_Used.Value : 0) };
                              }
                           }
                        }
                        else if (cri.Leave_Config.Continuously.HasValue && cri.Leave_Config.Continuously.Value)
                        {
                           // Continuously
                           if (lconfigDetail.Period == TimePeriod.Weeks)
                              return new Leave_Left() { Weeks_Left = defaultWeeksAmount, Is_First_Period = true, Entitle = defaultWeeksAmount, EntitleAll = defaultWeeksAmount, Leave_Used = (currentcal.Leave_Used.HasValue ? currentcal.Leave_Used.Value : 0) };
                        }
                     }
                     else if (cri.Leave_Config.Leave_Name == LeaveType.PaternityLeave)
                     {
                        var defaultWeeksAmount = lconfigDetail.Default_Leave_Amount.HasValue ? lconfigDetail.Default_Leave_Amount.Value : 0;
                        if (lconfigDetail.Period == TimePeriod.Weeks)
                        {
                           if (currentcal.Leave_Used == 0)
                              return new Leave_Left() { Weeks_Left = defaultWeeksAmount, Entitle = defaultWeeksAmount, EntitleAll = defaultWeeksAmount, Leave_Used = 0 };
                           else
                              return new Leave_Left() { Weeks_Left = defaultWeeksAmount, Entitle = defaultWeeksAmount, EntitleAll = defaultWeeksAmount, Leave_Used = (currentcal.Leave_Used.HasValue ? currentcal.Leave_Used.Value : 0) };
                        }
                     }
                  }

               }

            }
            Debug.WriteLine("'***********APP DEBUG***********' " + DateTime.Now + "-End sevice Get Leave left-" + cri.Leave_Config.Leave_Name);
         }

         return new Leave_Left();
      }

      //Added by sun 04-02-2016
      public ServiceResult InsertLeaveApplicationDocument(Leave_Application_Document[] LeaveAppDocs)
      {
         try
         {
            using (var db = new SBS2DBContext())
            {
               db.Leave_Application_Document.AddRange(LeaveAppDocs);
               db.SaveChanges();
               return new ServiceResult() { Code = ERROR_CODE.SUCCESS, Msg = new Success().getSuccess(ERROR_CODE.SUCCESS_CREATE), Field = Resource.Leave };
            }
         }
         catch
         {
            return new ServiceResult() { Code = ERROR_CODE.ERROR_503_INSERT_ERROR, Msg = new Error().getError(ERROR_CODE.ERROR_503_INSERT_ERROR), Field = Resource.Leave };
         }
      }
      #endregion

      #region Working Day
      public Working_Days GetWorkingDay(Nullable<int> pCompanyID)
      {
         using (var db = new SBS2DBContext())
         {
            return (from a in db.Working_Days where a.Company_ID == pCompanyID select a).FirstOrDefault();
         }
      }



      public List<int> GetWorkingDayOfWeek(Nullable<int> pCompanyID, Nullable<int> pProfileID = null, Nullable<int> pEmpID = null)
      {
         var dws = new List<int>();
         //if (pProfileID.HasValue | pEmpID.HasValue)
         //{
         //   var hServ = new EmploymentHistoryService();
         //   Employment_History hist = null;
         //   if (pProfileID.HasValue)
         //      hist = hServ.GetCurrentEmploymentHistoryByProfile(pProfileID);
         //   else if (pEmpID.HasValue)
         //      hist = hServ.GetCurrentEmploymentHistory(pEmpID);

         //   if (hist != null)
         //   {
         //      if (!hist.CL_Sun.HasValue || !hist.CL_Sun.Value)
         //         dws.Add((int)DayOfWeek.Sunday);
         //      if (!hist.CL_Mon.HasValue || !hist.CL_Mon.Value)
         //         dws.Add((int)DayOfWeek.Monday);
         //      if (!hist.CL_Tue.HasValue || !hist.CL_Tue.Value)
         //         dws.Add((int)DayOfWeek.Tuesday);
         //      if (!hist.CL_Wed.HasValue || !hist.CL_Wed.Value)
         //         dws.Add((int)DayOfWeek.Wednesday);
         //      if (!hist.CL_Thu.HasValue || !hist.CL_Thu.Value)
         //         dws.Add((int)DayOfWeek.Thursday);
         //      if (!hist.CL_Fri.HasValue || !hist.CL_Fri.Value)
         //         dws.Add((int)DayOfWeek.Friday);
         //      if (!hist.CL_Sat.HasValue || !hist.CL_Sat.Value)
         //         dws.Add((int)DayOfWeek.Saturday);
         //   }
         //}

         //if (dws.Count == 0)
         //{
         var wd = GetWorkingDay(pCompanyID);
         if (wd != null)
         {
            if (!wd.CL_Sun.HasValue || !wd.CL_Sun.Value)
               dws.Add((int)DayOfWeek.Sunday);
            if (!wd.CL_Mon.HasValue || !wd.CL_Mon.Value)
               dws.Add((int)DayOfWeek.Monday);
            if (!wd.CL_Tue.HasValue || !wd.CL_Tue.Value)
               dws.Add((int)DayOfWeek.Tuesday);
            if (!wd.CL_Wed.HasValue || !wd.CL_Wed.Value)
               dws.Add((int)DayOfWeek.Wednesday);
            if (!wd.CL_Thu.HasValue || !wd.CL_Thu.Value)
               dws.Add((int)DayOfWeek.Thursday);
            if (!wd.CL_Fri.HasValue || !wd.CL_Fri.Value)
               dws.Add((int)DayOfWeek.Friday);
            if (!wd.CL_Sat.HasValue || !wd.CL_Sat.Value)
               dws.Add((int)DayOfWeek.Saturday);
         }
         //}
         return dws;
      }

      public List<int> GetWorkingAllDayOfWeek()
      {
         var dws = new List<int>();
         dws.Add((int)DayOfWeek.Sunday);
         dws.Add((int)DayOfWeek.Monday);
         dws.Add((int)DayOfWeek.Tuesday);
         dws.Add((int)DayOfWeek.Wednesday);
         dws.Add((int)DayOfWeek.Thursday);
         dws.Add((int)DayOfWeek.Friday);
         dws.Add((int)DayOfWeek.Saturday);
         return dws;
      }
      public ServiceResult SaveWorkingDay(Working_Days workingDays)
      {
         try
         {
            using (var db = new SBS2DBContext())
            {
               if (workingDays.Working_Days_ID > 0)
               {
                  //Update
                  var current = (from a in db.Working_Days where a.Working_Days_ID == workingDays.Working_Days_ID select a).FirstOrDefault();
                  if (current == null)
                     return new ServiceResult() { Code = ERROR_CODE.ERROR_511_DATA_NOT_FOUND, Msg = new Error().getError(ERROR_CODE.ERROR_511_DATA_NOT_FOUND), Field = Resource.Working_Days };

                  current.Days = workingDays.Days;
                  db.SaveChanges();
                  return new ServiceResult() { Code = ERROR_CODE.SUCCESS_EDIT, Msg = new Success().getSuccess(ERROR_CODE.SUCCESS_EDIT), Field = Resource.Working_Days };
               }
               else
               {
                  //Insert
                  db.Working_Days.Add(workingDays);
                  db.SaveChanges();
                  return new ServiceResult() { Code = ERROR_CODE.SUCCESS_CREATE, Msg = new Success().getSuccess(ERROR_CODE.SUCCESS_CREATE), Field = Resource.Working_Days };
               }
            }
         }
         catch
         {
            return new ServiceResult() { Code = ERROR_CODE.ERROR_506_SAVE_ERROR, Msg = new Error().getError(ERROR_CODE.ERROR_506_SAVE_ERROR), Field = Resource.Working_Days };
         }
      }
      #endregion

      public ServiceResult UpdateDeleteHolidayStatus(Nullable<int> pHolidayID, string pStatus, string pUpdateBy)
      {
         try
         {
            var currentdate = StoredProcedure.GetCurrentDate();
            using (var db = new SBS2DBContext())
            {
               var current = db.Holiday_Config.Where(w => w.Holiday_ID == pHolidayID).FirstOrDefault();
               if (current != null)
               {
                  current.Record_Status = pStatus;
                  current.Update_By = pUpdateBy;
                  current.Update_On = currentdate;
               }
               db.SaveChanges();
               return new ServiceResult() { Code = ERROR_CODE.SUCCESS, Msg = new Success().getSuccess(ERROR_CODE.SUCCESS_DELETE), Field = Resource.Holiday_And_Event };
            }
         }
         catch
         {
            return new ServiceResult() { Code = ERROR_CODE.ERROR_505_DELETE_ERROR, Msg = new Error().getError(ERROR_CODE.ERROR_505_DELETE_ERROR), Field = Resource.Holiday_And_Event };
         }
      }

      public ServiceResult UpdateMultipleDeleteHolidayStatus(int[] pHolidaysID, string pStatus, string pUpdateBy)
      {
         try
         {
            var currentdate = StoredProcedure.GetCurrentDate();
            using (var db = new SBS2DBContext())
            {
               var holidays = db.Holiday_Config.Where(w => pHolidaysID.Contains(w.Holiday_ID));
               if (holidays != null)
               {
                  foreach (var h in holidays)
                  {
                     h.Update_On = currentdate;
                     h.Update_By = pUpdateBy;
                     h.Record_Status = pStatus;
                  }
                  db.SaveChanges();
               }
               return new ServiceResult() { Code = ERROR_CODE.SUCCESS, Msg = new Success().getSuccess(ERROR_CODE.SUCCESS_DELETE), Field = Resource.Holiday_And_Event };
            }
         }
         catch
         {
            //Log
            return new ServiceResult() { Code = ERROR_CODE.ERROR_505_DELETE_ERROR, Msg = new Error().getError(ERROR_CODE.ERROR_505_DELETE_ERROR), Field = Resource.Holiday_And_Event };
         }
      }

      public ServiceResult UpdateMultipleDeleteLeaveDefaultStatus(int[] pDefaultIDs, string pStatus, string pUpdateBy)
      {
         try
         {
            var currentdate = StoredProcedure.GetCurrentDate();
            using (var db = new SBS2DBContext())
            {
               var current = db.Leave_Default.Where(w => pDefaultIDs.Contains(w.Default_ID));
               if (current == null)
                  return new ServiceResult { Code = ERROR_CODE.ERROR_511_DATA_NOT_FOUND, Msg = Resource.Leave_Default + " " + Resource.Not_Found_Msg, Field = Resource.Leave_Default };

               foreach (var ld in current)
               {
                  ld.Update_On = currentdate;
                  ld.Update_By = pUpdateBy;
                  ld.Record_Status = pStatus;
               }
               db.SaveChanges();
               return new ServiceResult() { Code = ERROR_CODE.SUCCESS, Msg = new Success().getSuccess(ERROR_CODE.SUCCESS_DELETE) };
            }
         }
         catch
         {
            return new ServiceResult() { Code = ERROR_CODE.ERROR_505_DELETE_ERROR, Msg = new Error().getError(ERROR_CODE.ERROR_505_DELETE_ERROR) };
         }
      }

      public ServiceResult UpdateDeleteLeaveDefaultStatus(Nullable<int> pLeaveDefaultID, string pStatus, string pUpdateBy)
      {
         try
         {
            var currentdate = StoredProcedure.GetCurrentDate();
            using (var db = new SBS2DBContext())
            {
               var leaveDefault = db.Leave_Default.Where(w => w.Default_ID == pLeaveDefaultID).FirstOrDefault();

               if (leaveDefault != null)
               {
                  leaveDefault.Record_Status = pStatus;
                  leaveDefault.Update_By = pUpdateBy;
                  leaveDefault.Update_On = currentdate;
                  db.SaveChanges();
               }
               return new ServiceResult() { Code = ERROR_CODE.SUCCESS, Msg = new Success().getSuccess(ERROR_CODE.SUCCESS_DELETE), Field = Resource.Leave_Default };
            }
         }
         catch
         {
            return new ServiceResult() { Code = ERROR_CODE.ERROR_505_DELETE_ERROR, Msg = new Error().getError(ERROR_CODE.ERROR_505_DELETE_ERROR), Field = Resource.Leave_Default };
         }
      }

      public ServiceResult UpdateDeleteLeaveTypeStatus(Nullable<int> pLeaveConfigID, string pStatus, string pUpdateBy)
      {
         try
         {
            var currentdate = StoredProcedure.GetCurrentDate();
            using (var db = new SBS2DBContext())
            {
               var leave = db.Leave_Config.Where(w => w.Leave_Config_ID == pLeaveConfigID).FirstOrDefault();

               if (leave != null)
               {
                  leave.Record_Status = pStatus;
                  leave.Update_By = pUpdateBy;
                  leave.Update_On = currentdate;

                  db.SaveChanges();
               }
               return new ServiceResult() { Code = ERROR_CODE.SUCCESS, Msg = new Success().getSuccess(ERROR_CODE.SUCCESS_DELETE), Field = Resource.Leave_Type };
            }
         }
         catch
         {
            return new ServiceResult() { Code = ERROR_CODE.ERROR_505_DELETE_ERROR, Msg = new Error().getError(ERROR_CODE.ERROR_505_DELETE_ERROR), Field = Resource.Leave_Type };
         }
      }

      public ServiceResult UpdateMultipleDeleteLeaveTypeStatus(int[] pLeaveTypeID, string pStatus, string pUpdateBy)
      {
         try
         {
            var currentdate = StoredProcedure.GetCurrentDate();
            using (var db = new SBS2DBContext())
            {
               var current = db.Leave_Config.Where(w => pLeaveTypeID.Contains(w.Leave_Config_ID));
               if (current == null)
                  return new ServiceResult { Code = ERROR_CODE.ERROR_511_DATA_NOT_FOUND, Msg = Resource.Leave_Type + " " + Resource.Not_Found_Msg, Field = Resource.Leave_Type };

               foreach (var lc in current)
               {
                  lc.Update_On = currentdate;
                  lc.Update_By = pUpdateBy;
                  lc.Record_Status = pStatus;
               }
               db.SaveChanges();
               return new ServiceResult() { Code = ERROR_CODE.SUCCESS, Msg = new Success().getSuccess(ERROR_CODE.SUCCESS_DELETE), Field = Resource.Leave_Type };
            }
         }
         catch
         {
            return new ServiceResult() { Code = ERROR_CODE.ERROR_505_DELETE_ERROR, Msg = new Error().getError(ERROR_CODE.ERROR_505_DELETE_ERROR), Field = Resource.Leave_Type };
         }
      }

      public ServiceResult UpdateDeleteLeaveAdjStatus(Nullable<int> pAdjustmentID, string pStatus, string pUpdateBy)
      {
         try
         {
            var currentdate = StoredProcedure.GetCurrentDate();
            using (var db = new SBS2DBContext())
            {
               var leaveAj = db.Leave_Adjustment.Where(w => w.Adjustment_ID == pAdjustmentID).FirstOrDefault();
               if (leaveAj != null)
               {
                  leaveAj.Record_Status = pStatus;
                  leaveAj.Update_By = pUpdateBy;
                  leaveAj.Update_On = currentdate;
                  db.SaveChanges();
               }

               return new ServiceResult() { Code = ERROR_CODE.SUCCESS, Msg = new Success().getSuccess(ERROR_CODE.SUCCESS_DELETE), Field = Resource.Leave_Adjustment };
            }
         }
         catch
         {
            return new ServiceResult() { Code = ERROR_CODE.ERROR_505_DELETE_ERROR, Msg = new Error().getError(ERROR_CODE.ERROR_505_DELETE_ERROR), Field = Resource.Leave_Adjustment };
         }
      }

      public ServiceResult UpdateMultipleDeleteLeaveAdjStatus(int[] pAdjustmentID, string pStatus, string pUpdateBy)
      {
         try
         {
            var currentdate = StoredProcedure.GetCurrentDate();
            using (var db = new SBS2DBContext())
            {
               var current = db.Leave_Adjustment.Where(w => pAdjustmentID.Contains(w.Adjustment_ID));
               if (current == null)
                  return new ServiceResult { Code = ERROR_CODE.ERROR_511_DATA_NOT_FOUND, Msg = Resource.Leave_Adjustment + " " + Resource.Not_Found_Msg, Field = Resource.Leave_Adjustment };

               foreach (var la in current)
               {
                  la.Update_On = currentdate;
                  la.Update_By = pUpdateBy;
                  la.Record_Status = pStatus;
               }
               db.SaveChanges();
               return new ServiceResult() { Code = ERROR_CODE.SUCCESS, Msg = new Success().getSuccess(ERROR_CODE.SUCCESS_DELETE), Field = Resource.Leave_Adjustment };
            }
         }
         catch
         {
            return new ServiceResult() { Code = ERROR_CODE.ERROR_505_DELETE_ERROR, Msg = new Error().getError(ERROR_CODE.ERROR_505_DELETE_ERROR), Field = Resource.Leave_Adjustment };
         }
      }





   }






   public class Holidays
   {
      public DateTime date { get; set; }
      public string name { get; set; }
      public Nullable<int> HolidayID { get; set; }

   }

   public class Leave_Report
   {
      public Nullable<int> Profile_ID { get; set; }
      public string Employee_No { get; set; }
      public string Employee_Name { get; set; }
      public List<Leave_Type_Report> leaveTypelist { get; set; }
      public List<int> Leave_Type_Sel { get; set; }
   }

   public class Leave_Type_Report
   {

      public string Leave_Name { get; set; }
      public int Leave_Config_ID { get; set; }
      public decimal Entitle { get; set; }
      public decimal Bring_Forward { get; set; }
      public Nullable<decimal> Days_Taken { get; set; }
   }


   public class LeaveCalIsExistCriteria
   {
      public Nullable<int> Employee_Profile_ID { get; set; }
      public Nullable<int> Profile_ID { get; set; }
      public Nullable<int> Relationship_ID { get; set; }
   }

}