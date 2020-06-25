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

namespace SBSModel.Models
{
    #region Time
    public class TimeService : ServiceBase
    {
        public TimeService(){}

        public TimeService(User_Profile userlogin) : base(userlogin){}

        #region Time Device
        public Time_Device GetTimeDevice(int? pDID = null, string pIPAddr = "")
        {
            using (var db = new SBS2TimeDBContext())
            {
                if (pDID.HasValue)
                    return db.Time_Device.Where(w => w.Device_ID == pDID).FirstOrDefault();
                else if (!string.IsNullOrEmpty(pIPAddr))
                    return db.Time_Device.Where(w => w.IP_Address == pIPAddr).FirstOrDefault();
                return null;
            }

        }

        public List<ZK_Users> GetZKUsers(int pDID)
        {
            using (var db = new SBS2TimeDBContext())
            {
                if (pDID > 0 )
                    return db.ZK_Users.Where(w => w.Device_ID == pDID).ToList();
               
                return null;
            }  

        }

        public List<Time_Device> LstTimeDevice(int? pCompanyID, int? pDID = null)
        {
            using (var db = new SBS2TimeDBContext())
            {
                var wds = db.Time_Device
                    .Where(w => w.Company_ID == pCompanyID & w.Record_Status != RecordStatus.Delete);

                if (pDID.HasValue)
                    wds = wds.Where(w => w.Device_ID == pDID);

                return wds.OrderBy(o => o.Device_No).ToList();
            }
        }

        public List<Time_Device> LstTimeDevice(int pCompanyID, string pDeviceNo)
        {
            using (var db = new SBS2TimeDBContext())
            {
                var wds = db.Time_Device
                    .Where(w => w.Company_ID == pCompanyID & w.Device_No == pDeviceNo & w.Record_Status != RecordStatus.Delete);                
                return wds.OrderBy(o => o.Device_No).ToList();
            }
        }

        public ServiceResult InsertTimeDevice(Time_Device pTD)
        {
            try
            {
                using (var db = new SBS2TimeDBContext())
                {
                    db.Time_Device.Add(pTD);
                    db.SaveChanges();
                    return new ServiceResult
                    {
                        Code = ERROR_CODE.SUCCESS,
                        Msg_Code = ERROR_CODE.SUCCESS_CREATE,
                        Field = Resource.Device
                    };
                }
            }
            catch
            {
                return new ServiceResult
                {
                    Code = ERROR_CODE.ERROR_503_INSERT_ERROR,
                    Field = Resource.Device
                };
            }
        }

        public ServiceResult UpdateTimeDevice(Time_Device pTD)
        {
            try
            {
                using (var db = new SBS2TimeDBContext())
                {
                    var curTD = db.Time_Device.Where(w => w.Device_ID == pTD.Device_ID).FirstOrDefault();
                    if (curTD != null)
                    {
                        if (pTD.IP_Address != curTD.IP_Address)
                        {
                            ResetTimeDevice(pTD.Device_ID);
                        }
                        db.Entry(curTD).CurrentValues.SetValues(pTD);
                        db.SaveChanges();
                    }
                    return new ServiceResult
                    {
                        Code = ERROR_CODE.SUCCESS,
                        Msg_Code = ERROR_CODE.SUCCESS_EDIT,
                        Field = Resource.Device
                    };
                }
            }
            catch
            {
                return new ServiceResult
                {
                    Code = ERROR_CODE.ERROR_504_UPDATE_ERROR,
                    Field = Resource.Device
                };
            }
        }

        public ServiceResult DeleteTimeDevice(int? pDID)
        {
            try
            {
                using (var db = new SBS2TimeDBContext())
                {
                    var td = db.Time_Device.Where(w => w.Device_ID == pDID).FirstOrDefault();
                    if (td != null)
                    {
                        if (td.Time_Transaction != null)
                            db.Time_Transaction.RemoveRange(td.Time_Transaction);

                        if (td.Time_Device_Map != null)
                            db.Time_Device_Map.RemoveRange(td.Time_Device_Map);

                        db.Time_Device.Remove(td);
                        db.SaveChanges();
                    }
                    return new ServiceResult
                    {
                        Code = ERROR_CODE.SUCCESS,
                        Msg_Code = ERROR_CODE.SUCCESS_DELETE,
                        Field = Resource.Device
                    };
                }
            }
            catch
            {
                return new ServiceResult
                {
                    Code = ERROR_CODE.ERROR_505_DELETE_ERROR,
                    Field = Resource.Device
                };
            }
        }
        public ServiceResult ResetTimeDevice(int? pDID)
        {
            try
            {
                using (var db = new SBS2TimeDBContext())
                {
                    var td = db.Time_Device.Where(w => w.Device_ID == pDID).FirstOrDefault();
                    if (td != null)
                    {
                        if (td.Time_Transaction != null)
                            db.Time_Transaction.RemoveRange(td.Time_Transaction);

                        if (td.Time_Device_Map != null)
                            db.Time_Device_Map.RemoveRange(td.Time_Device_Map);

                        db.SaveChanges();
                    }
                    return new ServiceResult
                    {
                        Code = ERROR_CODE.SUCCESS,
                        Msg_Code = ERROR_CODE.SUCCESS_RESET,
                        Field = Resource.Device
                    };
                }
            }
            catch
            {
                return new ServiceResult
                {
                    Code = ERROR_CODE.ERROR_RESET,
                    Field = Resource.Device
                };
            }
        }
        #endregion

        #region Time Device Mapping

        //Added by Moet on 9-Oct-2016 for RFID
        public bool IsTagExisted(string TagID)
        {
            using (var db = new SBS2TimeDBContext())
            {
                var wds = db.Time_Device_Map
                    .Where(w => w.Device_Employee_Name == TagID).FirstOrDefault();

                if (wds == null)
                    return false;
                else
                    return true;
            }
        }

        public Time_Device_Map GetTimeDeviceMap(int? pMID)
        {
            using (var db = new SBS2TimeDBContext())
            {
                return db.Time_Device_Map
                    .Include(i => i.Time_Device)
                    .Where(w => w.Device_ID == pMID)
                    .FirstOrDefault();
            }

        }

        public Time_Device_Map GetTimeDeviceMap(int pDeviceID , string pTagID)
        {
            using (var db = new SBS2TimeDBContext())
            {
                return db.Time_Device_Map
                    .Include(i => i.Time_Device)
                    .Where(w => w.Device_ID == pDeviceID & w.Device_Employee_Name == pTagID)
                    .FirstOrDefault();
            }

        }

        public List<Time_Device_Map> LstTimeDeviceMap(int? pDID, int? pMID = null)
        {
            using (var db = new SBS2TimeDBContext())
            {
                var wds = db.Time_Device_Map
                    .Include(i => i.Time_Device)
                    .Where(w => w.Device_ID == pDID & w.Record_Status != RecordStatus.Delete);

                if (pMID.HasValue)
                    wds = wds.Where(w => w.Device_ID == pMID);

                return wds.OrderBy(o => o.Device_Employee_Name).ToList();
            }
        }

        public ServiceResult InsertTimeDeviceMap(Time_Device_Map pTD)
        {
            try
            {
                using (var db = new SBS2TimeDBContext())
                {
                    db.Entry(pTD).State = EntityState.Added;
                    db.SaveChanges();
                    return new ServiceResult
                    {
                        Code = ERROR_CODE.SUCCESS,
                        Msg_Code = ERROR_CODE.SUCCESS_CREATE,
                        Field = Resource.Device_Employee_Mapping
                    };
                }
            }
            catch
            {
                return new ServiceResult
                {
                    Code = ERROR_CODE.ERROR_503_INSERT_ERROR,
                    Field = Resource.Device_Employee_Mapping
                };
            }
        }

        public ServiceResult UpdateTimeDeviceMap(Time_Device_Map pTD)
        {
            try
            {
                using (var db = new SBS2TimeDBContext())
                {
                    var curTD = db.Time_Device_Map.Where(w => w.Device_ID == pTD.Device_ID).FirstOrDefault();
                    db.Entry(curTD).CurrentValues.SetValues(pTD);
                    db.SaveChanges();
                    return new ServiceResult
                    {
                        Code = ERROR_CODE.SUCCESS,
                        Msg_Code = ERROR_CODE.SUCCESS_EDIT,
                        Field = Resource.Device_Employee_Mapping
                    };
                }
            }
            catch
            {
                return new ServiceResult
                {
                    Code = ERROR_CODE.ERROR_504_UPDATE_ERROR,
                    Field = Resource.Device_Employee_Mapping
                };
            }
        }

        public ServiceResult DeleteTimeDeviceMap(int? pMID)
        {
            try
            {
                using (var db = new SBS2TimeDBContext())
                {
                    var td = db.Time_Device_Map.Where(w => w.Device_ID == pMID).FirstOrDefault();
                    if (td != null)
                    {
                        td.Record_Status = RecordStatus.Delete;
                        db.SaveChanges();
                    }
                    return new ServiceResult
                    {
                        Code = ERROR_CODE.SUCCESS,
                        Msg_Code = ERROR_CODE.SUCCESS_DELETE,
                        Field = Resource.Device_Employee_Mapping
                    };
                }
            }
            catch
            {
                return new ServiceResult
                {
                    Code = ERROR_CODE.ERROR_505_DELETE_ERROR,
                    Field = Resource.Device_Employee_Mapping
                };
            }
        }

        public ServiceResult SaveTimeDeviceMap(int? pComID, int? pDID, List<Time_Device_Map> pMaps)
        {
            try
            {
                using (var db = new SBS2TimeDBContext())
                {
                    var curMaps = db.Time_Device_Map.Where(w => w.Device_ID == pDID & w.Record_Status != RecordStatus.Delete);
                    var rmMaps = new List<Time_Device_Map>();
                    foreach (var curMap in curMaps)
                    {
                        if (pMaps == null || !pMaps.Select(s => s.Map_ID).Contains(curMap.Map_ID))
                            curMap.Record_Status = RecordStatus.Delete;
                    }

                    if (pMaps != null)
                    {
                        foreach (var map in pMaps)
                        {
                            if (map.Map_ID == 0)
                            {
                                var trans = db.Time_Transaction.Where(w => w.Device_Employee_Pin == map.Device_Employee_Pin & w.Company_ID == pComID & w.Employee_Profile_ID == null);
                                foreach (var tran in trans)
                                {
                                    tran.Employee_Profile_ID = map.Employee_Profile_ID;
                                }
                                db.Time_Device_Map.Add(map);
                            }
                            else
                            {
                                var curInvt = db.Time_Device_Map.Where(w => w.Map_ID == map.Map_ID).FirstOrDefault();
                                if (curInvt != null)
                                {
                                    var trans = db.Time_Transaction.Where(w => w.Device_Employee_Pin == map.Device_Employee_Pin & w.Company_ID == pComID & w.Employee_Profile_ID == null);
                                    foreach (var tran in trans)
                                    {
                                        tran.Employee_Profile_ID = map.Employee_Profile_ID;
                                    }
                                    db.Entry(curInvt).CurrentValues.SetValues(map);
                                }

                            }
                        }
                    }

                    db.SaveChanges();
                    return new ServiceResult
                    {
                        Code = ERROR_CODE.SUCCESS,
                        Msg_Code = ERROR_CODE.SUCCESS_EDIT,
                        Field = Resource.Device_Employee_Mapping
                    };
                }
            }
            catch
            {
                return new ServiceResult
                {
                    Code = ERROR_CODE.ERROR_506_SAVE_ERROR,
                    Field = Resource.Device_Employee_Mapping
                };
            }
        }
        #endregion

        #region Time Transaction

        public Time_Transaction GetTimeTransaction(int? pTID)
        {
            using (var db = new SBS2TimeDBContext())
            {
                return db.Time_Transaction
                    .Where(w => w.Time_Transaction_ID == pTID)
                    .FirstOrDefault();
            }

        }


        public List<Time_Transaction> LstTimeTransaction(TimeTransactionCriteria cri)
        {
            using (var db = new SBS2TimeDBContext())
            {
                var trans = db.Time_Transaction
                    .Where(w => w.Company_ID == cri.Company_ID);

                if (cri.Branch_ID.HasValue)
                    trans = trans.Where(w => w.Time_Device.Branch_ID == cri.Branch_ID);

                if (cri.From.HasValue)
                    trans = trans.Where(w => EntityFunctions.CreateDateTime(w.Device_Transaction_Date.Value.Year, w.Device_Transaction_Date.Value.Month, w.Device_Transaction_Date.Value.Day, 0, 0, 0) >= cri.From);

                if (cri.To.HasValue)
                    trans = trans.Where(w => EntityFunctions.CreateDateTime(w.Device_Transaction_Date.Value.Year, w.Device_Transaction_Date.Value.Month, w.Device_Transaction_Date.Value.Day, 0, 0, 0) <= cri.To);

                if (cri.Employee_Profile_ID.HasValue)
                    trans = trans.Where(w => w.Employee_Profile_ID == cri.Employee_Profile_ID);

                if (cri.Device_ID.HasValue)
                    trans = trans.Where(w => w.Device_ID == cri.Device_ID);

                return trans.OrderBy(o => o.Device_Employee_Pin).ThenBy(o => o.Device_Transaction_Date).ToList();
            }
        }

        public List<Time_Transaction> Sum_LstTimeTransaction(TimeTransactionCriteria cri)
        {
            using (var db = new SBS2TimeDBContext())
            {
                var trans = db.Time_Transaction
                    .Where(w => w.Company_ID == cri.Company_ID);

                if (cri.Branch_ID.HasValue)
                    trans = trans.Where(w => w.Time_Device.Branch_ID == cri.Branch_ID);

                if (cri.From.HasValue)
                    trans = trans.Where(w => EntityFunctions.CreateDateTime(w.Device_Transaction_Date.Value.Year, w.Device_Transaction_Date.Value.Month, w.Device_Transaction_Date.Value.Day, 0, 0, 0) >= cri.From);

                if (cri.To.HasValue)
                    trans = trans.Where(w => EntityFunctions.CreateDateTime(w.Device_Transaction_Date.Value.Year, w.Device_Transaction_Date.Value.Month, w.Device_Transaction_Date.Value.Day, 0, 0, 0) <= cri.To);

                if (cri.Employee_Profile_ID.HasValue)
                    trans = trans.Where(w => w.Employee_Profile_ID == cri.Employee_Profile_ID);

                if (cri.Device_ID.HasValue)
                    trans = trans.Where(w => w.Device_ID == cri.Device_ID);

                return trans.OrderBy(o => o.Device_Employee_Pin).ThenBy(o => o.Device_Transaction_Date).ToList();
            }
        }

        public ServiceResult UpdateTimeTransaction(Nullable<int> pComID, Nullable<int> pDID, List<CivinTecAccessManager.Transaction> pTrans)
        {
            var currentdate = StoredProcedure.GetCurrentDate();
            try
            {
                using (var db = new SBS2TimeDBContext())
                {
                    Nullable<int> maxID = null;
                    Nullable<int> minID = null;
                    var i = 0;
                    foreach (var row in pTrans)
                    {
                        if (row.objUser == null)
                        {
                            i++;
                            continue;
                        }
                        Nullable<int> empID = null;
                        var map = db.Time_Device_Map.Where(w => w.Device_Employee_Pin == row.objUser.Pin & w.Device_ID == pDID & w.Time_Device.Company_ID == pComID).FirstOrDefault();
                        if (map != null)
                        {
                            empID = map.Employee_Profile_ID;
                            map.Employee_Name = row.objUser.FirstName + " " + row.objUser.MiddleName + " " + row.objUser.LastName;
                        }

                        db.Time_Transaction.Add(new Time_Transaction()
                        {
                            Device_Transaction_ID = row.ID,
                            Company_ID = pComID,
                            Device_Employee_Pin = row.objUser.Pin,
                            Employee_Profile_ID = empID,
                            Employee_Name = row.objUser.FirstName + " " + row.objUser.MiddleName + " " + row.objUser.LastName,
                            Job_Code = row.JobCode.ToString(),
                            Device_ID = pDID,
                            Device_Transaction_Date = row.DateAndTime,
                            Transaction_Type = row.TypeName,
                            Update_On = currentdate,
                            Create_On = currentdate,
                            Create_By = userloginName,
                            Update_By = userloginName
                        });

                        maxID = row.ID;
                        if (i == 0)
                            minID = row.ID;
                        i++;
                    }
                    if (maxID.HasValue && minID.HasValue)
                    {
                        var curDiv = db.Time_Device.Where(w => w.Device_ID == pDID).FirstOrDefault();
                        if (curDiv != null)
                        {
                            if (curDiv.Min_Transaction_Id == 0)
                                curDiv.Min_Transaction_Id = minID;
                            curDiv.Max_Transaction_Id = maxID;
                        }
                    }

                    db.SaveChanges();
                    return new ServiceResult
                    {
                        Code = ERROR_CODE.SUCCESS,
                        Msg_Code = ERROR_CODE.SUCCESS_CREATE,
                        Msg = Resource.Transaction + " " + new Success().getSuccess(ERROR_CODE.SUCCESS_CREATE) + "(" + i + ")",
                        Field = Resource.Transaction
                    };
                }
            }
            catch (Exception ex)
            {
                return new ServiceResult
                {
                    Code = ERROR_CODE.ERROR_503_INSERT_ERROR,
                    Msg = Resource.Transaction + " has error. " + ex.ToString(),
                    Field = Resource.Transaction
                };
            }
        }
        //Added by Moet on 9-Oct-2016 for RFID       
        public ServiceResult InsertTransaction(Time_Transaction pTran)
        {
            try
            {
                using (var db = new SBS2TimeDBContext())
                {                    
                    db.Time_Transaction.Add(pTran);                        
                    db.SaveChanges();
                }
                return new ServiceResult
                {
                    Code = ERROR_CODE.SUCCESS,
                    Msg_Code = ERROR_CODE.SUCCESS_EDIT,
                    Field = Resource.Device
                };                
            }
            catch
            {
                return new ServiceResult
                {
                    Code = ERROR_CODE.ERROR_504_UPDATE_ERROR,
                    Field = Resource.Device
                };
            }
        }
        #endregion

        #region Time Employee Arrangement
        public Time_Arrangement GetTimeArrangement(int? pArgID)
        {
            using (var db = new SBS2TimeDBContext())
            {
                return db.Time_Arrangement
                    .Where(w => w.Arrangement_ID == pArgID)
                    .FirstOrDefault();
            }

        }
        public Time_Arrangement GetTimeArrangement(int? pComID, int? pEmpID, Nullable<DateTime> pEffdate)
        {
            using (var db = new SBS2TimeDBContext())
            {
                var args = db.Time_Arrangement.Where(w => w.Company_ID == pComID & w.Employee_Profile_ID == pEmpID);
                pEffdate = DateUtil.ToDate(DateUtil.ToDisplayDate(pEffdate));
                args = args.Where(w => (w.Repeat == true ?
                      (EntityFunctions.CreateDateTime(w.Effective_Date.Value.Year, w.Effective_Date.Value.Month, w.Effective_Date.Value.Day, 0, 0, 0) <= pEffdate) :
                      EntityFunctions.CreateDateTime(w.Effective_Date.Value.Year, w.Effective_Date.Value.Month, w.Effective_Date.Value.Day, 0, 0, 0) == pEffdate));

                return args.OrderByDescending(o => o.Effective_Date).FirstOrDefault();
            }

        }
        public List<Time_Arrangement> LstTimeArrangement(int? pComID, Nullable<DateTime> pEffdate = null, int? pEmpID = null)
        {
            using (var db = new SBS2TimeDBContext())
            {
                var args = db.Time_Arrangement
                    .Where(w => w.Company_ID == pComID);

                if (pEmpID.HasValue)
                    args = args.Where(w => w.Employee_Profile_ID == pEmpID);

                if (pEffdate.HasValue)
                {
                    pEffdate = DateUtil.ToDate(DateUtil.ToDisplayDate(pEffdate));
                    args = args.Where(w => (w.Repeat == true ?
                        (EntityFunctions.CreateDateTime(w.Effective_Date.Value.Year, w.Effective_Date.Value.Month, w.Effective_Date.Value.Day, 0, 0, 0) <= pEffdate) :
                        EntityFunctions.CreateDateTime(w.Effective_Date.Value.Year, w.Effective_Date.Value.Month, w.Effective_Date.Value.Day, 0, 0, 0) == pEffdate));
                }
                return args.OrderByDescending(o => o.Effective_Date).ToList();
            }
        }
        public List<Time_Arrangement> CalculateDuration(int? pComID, Nullable<DateTime> pEffdate = null, int? pEmpID = null)
        {
            using (var db = new SBS2TimeDBContext())
            {
                var args = db.Time_Arrangement
                    .Where(w => w.Company_ID == pComID);

                if (pEmpID.HasValue)
                    args = args.Where(w => w.Employee_Profile_ID == pEmpID);

                if (pEffdate.HasValue)
                {
                    args = args.Where(w => (w.Repeat == true ?
                        (EntityFunctions.CreateDateTime(w.Effective_Date.Value.Year, w.Effective_Date.Value.Month, w.Effective_Date.Value.Day, 0, 0, 0) <= pEffdate) :
                        EntityFunctions.CreateDateTime(w.Effective_Date.Value.Year, w.Effective_Date.Value.Month, w.Effective_Date.Value.Day, 0, 0, 0) == pEffdate));
                }
                return args.OrderByDescending(o => o.Effective_Date).ToList();
            }
        }


        public ServiceResult DupTimeArrangement(Time_Arrangement pArg)
        {
            var msg = new StringBuilder();
            using (var db = new SBS2TimeDBContext())
            {
                var args = db.Time_Arrangement.Where(w => (w.Repeat == true ?
                     (EntityFunctions.CreateDateTime(w.Effective_Date.Value.Year, w.Effective_Date.Value.Month, w.Effective_Date.Value.Day, 0, 0, 0) <= pArg.Effective_Date) :
                     EntityFunctions.CreateDateTime(w.Effective_Date.Value.Year, w.Effective_Date.Value.Month, w.Effective_Date.Value.Day, 0, 0, 0) == pArg.Effective_Date));

                args = db.Time_Arrangement.Where(w => w.Employee_Profile_ID == pArg.Employee_Profile_ID & w.Branch_ID == pArg.Branch_ID);
                if (pArg.Arrangement_ID > 0)
                    args = args.Where(w => w.Arrangement_ID != pArg.Arrangement_ID);

                var tfrom = pArg.Time_From.Value;
                var tTo = pArg.Time_To.Value;

                var dup = false;
                foreach (var arg in args)
                {
                    var doCheckTime = false;
                    var doCheckDayOfweek = false;
                    var aEff = DateUtil.ToDate(arg.Effective_Date.Value.Day, arg.Effective_Date.Value.Month, arg.Effective_Date.Value.Year);
                    var eff = pArg.Effective_Date.Value;

                    var aRepeat = arg.Repeat.HasValue ? arg.Repeat.Value : false;
                    var repeat = pArg.Repeat.HasValue ? pArg.Repeat.Value : false;
                    if (aRepeat & repeat)
                    {
                        if (aEff == eff)
                            doCheckDayOfweek = true;
                    }
                    else if (aRepeat & !repeat)
                    {
                        //if (aEff <= eff)
                        //    doCheckDayOfweek = true;
                        if (aEff == eff)
                            doCheckTime = true;
                    }
                    else if (!aRepeat & repeat)
                    {
                        //if (aEff >= eff)
                        //    doCheckDayOfweek = true;
                        if (aEff == eff)
                            doCheckTime = true;
                    }
                    else if (!aRepeat & !repeat)
                    {
                        if (aEff == eff)
                            doCheckTime = true;
                    }

                    if (doCheckDayOfweek)
                    {
                        if (aRepeat & repeat)
                        {
                            if (!string.IsNullOrEmpty(arg.Day_Of_Week) && !string.IsNullOrEmpty(pArg.Day_Of_Week))
                            {
                                var aDayOfweeks = arg.Day_Of_Week.Split('|');
                                var dayOfweeks = pArg.Day_Of_Week.Split('|');
                                for (var i = 0; i < aDayOfweeks.Length; i++)
                                {
                                    var dw = aDayOfweeks[i];
                                    if (!string.IsNullOrEmpty(dw))
                                    {
                                        if (dayOfweeks.Contains(dw))
                                        {
                                            doCheckTime = true;
                                            break;
                                        }
                                        else if (dayOfweeks.Contains("-1"))
                                        {
                                            doCheckTime = true;
                                            break;
                                        }
                                    }
                                    else if (dw == "-1")
                                    {
                                        doCheckTime = true;
                                        break;
                                    }
                                }
                            }
                        }
                        else if (aRepeat & !repeat)
                        {
                            if (!string.IsNullOrEmpty(arg.Day_Of_Week))
                            {
                                var dw = ((int)pArg.Effective_Date.Value.DayOfWeek).ToString();
                                var aDayOfweeks = arg.Day_Of_Week.Split('|');
                                if (aDayOfweeks.Contains(dw))
                                    doCheckTime = true;
                                else if (aDayOfweeks.Contains("-1"))
                                    doCheckTime = true;
                            }
                        }
                        else if (!aRepeat & repeat)
                        {
                            if (!string.IsNullOrEmpty(pArg.Day_Of_Week))
                            {
                                var adw = ((int)arg.Effective_Date.Value.DayOfWeek).ToString();
                                var dayOfweeks = pArg.Day_Of_Week.Split('|');
                                if (dayOfweeks.Contains(adw))
                                    doCheckTime = true;
                                else if (dayOfweeks.Contains("-1"))
                                    doCheckTime = true;
                            }
                        }
                    }

                    if (doCheckTime)
                    {
                        var atfrom = arg.Time_From.Value;
                        var atTo = arg.Time_To.Value;

                        if (tfrom >= atfrom & tfrom <= atTo)
                            dup = true;
                        else if (tTo >= atfrom & tTo <= atTo)
                            dup = true;
                        else if (atfrom >= tfrom & atfrom <= tTo)
                            dup = true;
                        else if (atTo >= tfrom & atTo <= tTo)
                            dup = true;

                        if (dup)
                        {
                            if (string.IsNullOrEmpty(msg.ToString()))
                                msg.AppendLine("The date/time field contains duplicate values.");

                            if (arg.Repeat.HasValue && arg.Repeat.Value)
                            {
                                var display = new StringBuilder();
                                var aDayOfweeks = arg.Day_Of_Week.Split('|');
                                if (aDayOfweeks.Length > 0)
                                {
                                    display.Append(Resource.Repeat);
                                    display.Append(" ");
                                    var i = 0;
                                    foreach (var dw in aDayOfweeks)
                                    {
                                        if (!string.IsNullOrEmpty(dw))
                                        {
                                            display.Append(DateUtil.GetFullDayOfweek(NumUtil.ParseInteger(dw)));
                                            if (i != aDayOfweeks.Length - 2)
                                                display.Append(", ");
                                        }
                                        i++;
                                    }
                                }
                                msg.AppendLine(DateUtil.ToDisplayDate(arg.Effective_Date) + " " + display.ToString() + " (" + DateUtil.ToDisplayTime(atfrom) + " - " + DateUtil.ToDisplayTime(atTo) + ")");
                            }
                            else
                                msg.AppendLine(DateUtil.ToDisplayDate(arg.Effective_Date) + "(" + DateUtil.ToDisplayTime(atfrom) + " - " + DateUtil.ToDisplayTime(atTo) + ")");
                        }

                    }
                }

                if (dup)
                    return new ServiceResult { Code = ERROR_CODE.ERROR_510_DATA_DUPLICATE, Msg = msg.ToString(), Field = Resource.Employee_Arrangement };

                return new ServiceResult { Code = ERROR_CODE.SUCCESS, Msg_Code = ERROR_CODE.SUCCESS, Field = Resource.Employee_Arrangement };
            }
        }

        public ServiceResult InsertTimeArrangement(Time_Arrangement pArg)
        {
            try
            {
                using (var db = new SBS2TimeDBContext())
                {
                    db.Entry(pArg).State = EntityState.Added;
                    db.SaveChanges();
                    return new ServiceResult
                    {
                        Code = ERROR_CODE.SUCCESS,
                        Msg_Code = ERROR_CODE.SUCCESS_CREATE,
                        Field = Resource.Employee_Arrangement
                    };
                }
            }
            catch
            {
                return new ServiceResult
                {
                    Code = ERROR_CODE.ERROR_503_INSERT_ERROR,
                    Field = Resource.Employee_Arrangement
                };
            }
        }

        public ServiceResult UpdateTimeArrangement(Time_Arrangement pArg)
        {
            try
            {
                using (var db = new SBS2TimeDBContext())
                {
                    var curArg = db.Time_Arrangement.Where(w => w.Arrangement_ID == pArg.Arrangement_ID).FirstOrDefault();
                    db.Entry(curArg).CurrentValues.SetValues(pArg);
                    db.SaveChanges();
                    return new ServiceResult
                    {
                        Code = ERROR_CODE.SUCCESS,
                        Msg_Code = ERROR_CODE.SUCCESS_EDIT,
                        Field = Resource.Employee_Arrangement
                    };
                }
            }
            catch
            {
                return new ServiceResult
                {
                    Code = ERROR_CODE.ERROR_504_UPDATE_ERROR,
                    Field = Resource.Employee_Arrangement
                };
            }
        }

        public ServiceResult DeleteTimeArrangement(Time_Arrangement pArg)
        {
            try
            {
                using (var db = new SBS2TimeDBContext())
                {

                    db.Entry(pArg).State = EntityState.Deleted;
                    db.SaveChanges();
                    return new ServiceResult
                    {
                        Code = ERROR_CODE.SUCCESS,
                        Msg_Code = ERROR_CODE.SUCCESS_DELETE,
                        Field = Resource.Employee_Arrangement
                    };
                }
            }
            catch
            {
                return new ServiceResult
                {
                    Code = ERROR_CODE.ERROR_505_DELETE_ERROR,
                    Field = Resource.Employee_Arrangement
                };
            }
        }


        #endregion]

       

    }
    #endregion

    public class TimeTransactionCriteria : CriteriaBase
    {
        public Nullable<int> Employee_Profile_ID { get; set; }
        public Nullable<DateTime> From { get; set; }
        public Nullable<DateTime> To { get; set; }
        public Nullable<int> Branch_ID { get; set; }
        public Nullable<int> Department_ID { get; set; }
        public Nullable<int> Device_ID { get; set; }
    }
}
