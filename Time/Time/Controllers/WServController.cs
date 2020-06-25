using SBSModel.Common;
using SBSModel.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SBSResourceAPI;
using SBSTimeModel.Models;

namespace Time.Controllers
{
    public class WServController : Controller
    {
        public ActionResult GetWorkingHours(Nullable<int> pComID, Nullable<int> pEmpID, string pSDate, string pEDate)
        {
            var workingHours = 0;
            var workingMins = 0;

            if (!pComID.HasValue)
                return Json(workingHours, JsonRequestBehavior.AllowGet);
            if (!pEmpID.HasValue)
                return Json(workingHours, JsonRequestBehavior.AllowGet);
            if (string.IsNullOrEmpty(pSDate))
                return Json(workingHours, JsonRequestBehavior.AllowGet);
            if (string.IsNullOrEmpty(pEDate))
                return Json(workingHours, JsonRequestBehavior.AllowGet);

            var wkServ = new WorkingDaysService();
            var tmServ = new TimeService();
            var cri = new TimeTransactionCriteria();
            cri.Company_ID = pComID;
            cri.Employee_Profile_ID = pEmpID;
            cri.From = DateUtil.ToDate(pSDate, "-");
            cri.To = DateUtil.ToDate(pEDate, "-");

            if (!cri.From.HasValue)
                return Json(workingHours, JsonRequestBehavior.AllowGet);
            if (!cri.To.HasValue)
                return Json(workingHours, JsonRequestBehavior.AllowGet);

            var trans = tmServ.LstTimeTransaction(cri);

            var wk = wkServ.GetWorkingDay(pComID);
            int? curComID = null;
            int? curPin = null;
            string curDate = "";
            string curEmp = "";
            int? curEmpID = null;
            var i = 0;
            DateTime? clockin = null;
            DateTime? clockout = null;
            foreach (var row in trans)
            {
                if (curPin != row.Device_Employee_Pin | curDate != DateUtil.ToDisplayDate(row.Device_Transaction_Date))
                {
                    if (i > 0)
                    {
                        var duration = CalWorkingHours(clockin, clockout, curComID, curEmpID, wk);
                        if (duration != null)
                        {
                            workingHours += duration.Value.Hours;
                            workingMins += duration.Value.Minutes;
                        }
                    }
                    clockin = row.Device_Transaction_Date;
                }
                curEmpID = row.Employee_Profile_ID;
                curEmp = row.Employee_Name;
                curPin = row.Device_Employee_Pin;
                curDate = DateUtil.ToDisplayDate(row.Device_Transaction_Date);
                clockout = row.Device_Transaction_Date;
                curComID = row.Company_ID;

                if (i == trans.Count() - 1)
                {
                    var duration = CalWorkingHours(clockin, clockout, curComID, curEmpID, wk);
                    if (duration != null)
                    {
                        workingHours += duration.Value.Hours;
                        workingMins += duration.Value.Minutes;
                    }
                }
                i++;
            }

            if (workingMins > 0)
            {
                var hours = workingMins / 60;
                if ((workingMins % 60) >= 0)
                {                  
                    workingMins = (workingMins % 60);
                }
                workingHours += hours;
            }
        
            return Json(new{ workingHours = workingHours, workingMins = workingMins} ,JsonRequestBehavior.AllowGet);
        }

        private TimeSpan? CalWorkingHours(DateTime? clockin, DateTime? clockout, int? curComID, int? curEmpID,Working_Days wk )
        {
            var tmServ = new TimeService();
            var clockintime = DateUtil.ToTime(DateUtil.ToDisplayTime(clockin));
            var clockouttime = DateUtil.ToTime(DateUtil.ToDisplayTime(clockout));
            TimeSpan? sdatetime = null;
            TimeSpan? edatetime = null;

            //sdatetime = clockintime;// GetStartTime(minfrom, clockintime);
            //edatetime = clockouttime; //  GetEndTime(maxto, clockouttime);

            var args = tmServ.LstTimeArrangement(curComID, clockin, curEmpID);
            if (args != null && args.Count > 0)
            {
                TimeSpan? minfrom = null;
                TimeSpan? maxto = null;
                foreach (var arg in args)
                {
                    if (arg.Time_From.HasValue & arg.Time_To.HasValue)
                    {
                        var tfrom = arg.Time_From;
                        var tTo = arg.Time_To.Value;

                        if (tfrom < minfrom | minfrom == null)
                            minfrom = arg.Time_From;

                        if (tTo > maxto | maxto == null)
                            maxto = arg.Time_To;
                    }
                }
                if (minfrom != null && maxto != null)
                {
                    sdatetime = clockintime;// GetStartTime(minfrom, clockintime);
                    edatetime = clockouttime; //  GetEndTime(maxto, clockouttime);
                }
            }
            else
            {
                if (wk != null)
                {
                    var dw = (int)clockin.Value.DayOfWeek;
                    if (dw == 0)
                    {
                        if (!wk.CL_Sun.HasValue || !wk.CL_Sun.Value)
                        {
                            sdatetime = clockintime;//GetStartTime(wk.ST_Sun_Time, clockintime);
                            edatetime = clockouttime; //  GetEndTime(wk.ET_Sun_Time, clockouttime);
                        }
                    }
                    else if (dw == 1)
                    {
                        if (!wk.CL_Mon.HasValue || !wk.CL_Mon.Value)
                        {
                            sdatetime = clockintime;// GetStartTime(wk.ST_Mon_Time, clockintime);
                            edatetime = clockouttime; // GetEndTime(wk.ET_Mon_Time, clockouttime);
                        }
                    }
                    else if (dw == 2)
                    {
                        if (!wk.CL_Tue.HasValue || !wk.CL_Tue.Value)
                        {
                            sdatetime = clockintime;// GetStartTime(wk.ST_Tue_Time, clockintime);
                            edatetime = clockouttime; // GetEndTime(wk.ET_Tue_Time, clockouttime);
                        }
                    }
                    else if (dw == 3)
                    {
                        if (!wk.CL_Wed.HasValue || !wk.CL_Wed.Value)
                        {
                            sdatetime = clockintime;// GetStartTime(wk.ST_Wed_Time, clockintime);
                            edatetime = clockouttime; // GetEndTime(wk.ET_Wed_Time, clockouttime);
                        }
                    }
                    else if (dw == 4)
                    {
                        if (!wk.CL_Thu.HasValue || !wk.CL_Thu.Value)
                        {
                            sdatetime = clockintime;// GetStartTime(wk.ST_Thu_Time, clockintime);
                            edatetime = clockouttime; // GetEndTime(wk.ET_Thu_Time, clockouttime);
                        }
                    }
                    else if (dw == 5)
                    {
                        if (!wk.CL_Fri.HasValue || !wk.CL_Fri.Value)
                        {
                            sdatetime = clockintime;//GetStartTime(wk.ST_Fri_Time, clockintime);
                            edatetime = clockouttime; // GetEndTime(wk.ET_Fri_Time, clockouttime);
                        }
                    }
                    else if (dw == 6)
                    {
                        if (!wk.CL_Sat.HasValue || !wk.CL_Sat.Value)
                        {
                            sdatetime = clockintime;//GetStartTime(wk.ST_Sat_Time, clockintime);
                            edatetime = clockouttime; // GetEndTime(wk.ET_Sat_Time, clockouttime);
                        }
                    }
                }
            }

            if (sdatetime.HasValue && edatetime.HasValue)
            {
                var duration = DateTime.Parse(DateUtil.ToDisplayTime(edatetime)).Subtract(DateTime.Parse(DateUtil.ToDisplayTime(sdatetime)));
                return duration;
            }
            return null;
        }

        private string GetStatusFromWorkdays(bool? disabled, TimeSpan? start, TimeSpan? end, DateTime? clockin)
        {
            if (!disabled.HasValue || !disabled.Value)
            {
                var clockintime = DateUtil.ToTime(DateUtil.ToDisplayTime(clockin));
                if (clockintime > start)
                    return "<span class='color-red'><strong>" + Resource.Late + "</strong></span>";
                else
                    return "<span class='color-green'><strong>" + Resource.On_Time + "</strong></span>";
            }
            return "";
        }

        private TimeSpan? GetStartTime(TimeSpan? t1, TimeSpan? t2)
        {
            if (t1.HasValue && t2.HasValue)
            {
                if (t2 > t1)
                    return t2; /*Late*/
                else
                    return t1;  /*On-Time*/
            }
            return null;
        }

        private TimeSpan? GetEndTime(TimeSpan? t1, TimeSpan? t2)
        {
            if (t1.HasValue && t2.HasValue)
            {
                if (t2 < t1)
                    return t2; /*early*/
                else
                    return t1;  /*On-Time*/
            }
            return null;
        }



        public ActionResult LstTimesheet(Nullable<int> pComID, Nullable<int> pJobID)
        {
           var wtimesheets = new List<_Time_Sheet>();
           if (!pJobID.HasValue)
              return Json(wtimesheets, JsonRequestBehavior.AllowGet);

           var cri = new TimeSheetCriteria();
           cri.Job_Cost_ID = pJobID;
           cri.Company_ID = pComID;
           cri.Closed_Status = true ;
           var tmService = new TimeSheetService();
           var timesheets = tmService.LstTimeSheet(cri);
           wtimesheets = timesheets.Select(s => new _Time_Sheet()
           {
              Time_Sheet_ID = s.Time_Sheet_ID,
              Employee_Profile_ID = s.Employee_Profile_ID,
              Job_Cost_ID = s.Job_Cost_ID,
              Clock_In = DateUtil.ToDisplayTime(s.Clock_In) ,
              Clock_Out = DateUtil.ToDisplayTime(s.Clock_Out) ,
              Launch_Duration = DateUtil.ToDisplayTime(s.Launch_Duration) ,
              Duration = DateUtil.ToDisplayTime(s.Duration) ,
              Date_Of_Date = DateUtil.ToDisplayDate(s.Date_Of_Date),
              Note = s.Note,
              Hour_Rate = s.Hour_Rate,
              Total_Amount = s.Total_Amount,
           }).ToList();
           return Json(new { Timesheets = wtimesheets }, JsonRequestBehavior.AllowGet);
        }
        #region "For RFID reader and Android App"
            public class wResult
            {
                public string status { get; set; }
                public string message { get; set; }
                public int DeviceID { get; set; }
                public int TransCount { get; set; }
                public List<wTran> Trans { get; set; }
            }

            public class wTran
            {
                public DateTime Transaction_Date { get; set; }
                public int Transaction_ID { get; set; }           
                public int Employee_Profile_ID { get; set; }
                public int Device_Transaction_ID { get; set; }            
                public string Employee_Name { get; set; }
                public string Job_Code { get; set; }
                public string Transaction_Type { get; set; }            
            }

            public ActionResult RegisterDevice(int pCompanyID, String pBranchID, string pDeviceNo, string pUserName)
            {
                var _result = new wResult();
                var device = new Time_Device(); 

                var tmServ = new TimeService();
                var existInfo = tmServ.LstTimeDevice(pCompanyID, pDeviceNo.Trim());            
                if (existInfo.Count > 0)
                {
                    var existD = existInfo[0];
                    device.Device_ID = existD.Device_ID;

                    var cri = new TimeTransactionCriteria();
                    cri.Company_ID = pCompanyID;
                    cri.Device_ID = existD.Device_ID;

                    var TimeTrans = tmServ.LstTimeTransaction(cri);
                    if (TimeTrans != null)
                    {
                        device.Max_Transaction_Id = TimeTrans.Count();
                    }
                    else
                    {
                        device.Max_Transaction_Id = 0;
                    }

                    _result.status = "200";
                    _result.message = "Device is already existed";
                    _result.DeviceID = device.Device_ID;
                    _result.TransCount = device.Max_Transaction_Id.Value;
                }
                else
                {
                    device.Brand_Name = "RFID";
                    device.Company_ID = pCompanyID;
                    device.Device_No = pDeviceNo;
                    device.IP_Address = pDeviceNo;
                    device.Password = "";
                    device.Port = 2690;
                    device.User_Name = "";
                    device.Record_Status = RecordStatus.Active;
                    device.Min_Transaction_Id = 0;
                    device.Max_Transaction_Id = 0;
                    device.Create_By = pUserName;
                    device.Create_On = DateTime.Now;

                    var result = tmServ.InsertTimeDevice(device);
                
                    if (result.Code == ERROR_CODE.SUCCESS)
                    {
                        _result.status = "200";
                        _result.message = "Successfully registered";
                        _result.DeviceID = device.Device_ID;
                    }
                    else
                    {
                        _result.status = "1";
                        _result.message = "Fail";
                        _result.DeviceID = 0;
                    }
                }                                                               
                    return Json(new { Data = _result }, JsonRequestBehavior.AllowGet);
            }

            public ActionResult MappingTag(int pCompanyID, int pDeviceID, String pTagID, int pEmployeeProfileID, String pUserName)
            {
                var _result = new wResult();
                var tmServ = new TimeService();
                if (tmServ.IsTagExisted(pTagID) == true)
                {
                    _result.status = "1";
                    _result.message = "Tag ID is already mapped with other employee";
                    _result.DeviceID = pDeviceID;
                    return Json(new { Data = _result }, JsonRequestBehavior.AllowGet);
                }

                var map = new Time_Device_Map();
                var maps = new List<Time_Device_Map>();
                map.Device_Employee_Name = pTagID;
                map.Device_Employee_Pin = pEmployeeProfileID;
                map.Employee_Profile_ID = pEmployeeProfileID;
                map.Device_ID = pDeviceID;
                map.Record_Status = RecordStatus.Active;
                map.Create_By = pUserName;
                map.Create_On = DateTime.Now;

                maps.Add(map);
                var result = tmServ.SaveTimeDeviceMap(pCompanyID, pDeviceID, maps);          
                if (result.Code == ERROR_CODE.SUCCESS)
                {
                    _result.status = "200";
                    _result.message = "Successfully mapped";
                    _result.DeviceID = pDeviceID;
                }
                else
                {
                    _result.status = result.Msg_Code.ToString();
                    _result.message = result.Msg;
                    _result.DeviceID = pDeviceID;
                }
                return Json(new { Data = _result }, JsonRequestBehavior.AllowGet);
            }

            public ActionResult SaveAttendance(int pCompanyID, int pDeviceID, String pTagID, int pTranID, String pUserName)
            {
                var _result = new wResult();
                var tmServ = new TimeService();
                var tran = new Time_Transaction();
                var pMap = tmServ.GetTimeDeviceMap(pDeviceID, pTagID);
                if(pMap != null)
                {
                    var pEmployeeProfileID = pMap.Employee_Profile_ID;
                    tran.Company_ID = pCompanyID;
                    tran.Device_ID = pDeviceID;
                    tran.Employee_Profile_ID = pEmployeeProfileID;
                    tran.Device_Employee_Pin = pEmployeeProfileID;
                    tran.Device_Transaction_ID = pTranID;
                    tran.Device_Transaction_Date = DateTime.Now;
                    tran.Job_Code = "241";
                    tran.Transaction_Type = "FPM: Verified";
                    tran.Employee_Name = pTagID;
                    tran.Create_By = pUserName;
                    tran.Create_On = DateTime.Now;

                    var tList = new List<wTran>();
                    var t = new wTran();
                    // to continue here
                    var eService = new EmployeeService();
                    var p = eService.GetEmployeeProfile(pEmployeeProfileID);
                    if (p != null)
                    {
                        t.Employee_Name = p.User_Profile.First_Name + " " + p.User_Profile.Last_Name;
                    }
                    else
                    {
                        t.Employee_Name = "";
                    }
                    t.Transaction_Date = DateTime.Now;
                    t.Job_Code = "241";
                    t.Transaction_Type = "FPM: Verified";                
                    tList.Add(t);

                    tran.Card_ID = t.Employee_Name; //Used Card ID to keep employee name from SBS only for RFID Reader
                    var result = tmServ.InsertTransaction(tran);
                    t.Device_Transaction_ID = tran.Device_Transaction_ID.Value;
                    t.Employee_Profile_ID = pEmployeeProfileID.Value;

                    if (result.Code == ERROR_CODE.SUCCESS)
                    {
                        _result.status = "200";
                        _result.message = "Successfully recorded";
                        _result.DeviceID = pDeviceID;
                        _result.Trans = tList;
                    }
                    else
                    {
                        _result.status = result.Msg_Code.ToString();
                        _result.message = result.Msg;
                        _result.DeviceID = pDeviceID;
                    }
                }   
                else
                {
                    _result.status = "201";
                    _result.message = "Not found mapping for Tag ID : " + pTagID;
                    _result.DeviceID = pDeviceID;
                }
                return Json(new { Data = _result }, JsonRequestBehavior.AllowGet);
            }

            public ActionResult LstTransaction(int pCompanyID, int pDeviceId, DateTime pDate )
            {
                var wTrans = new List<wTran>();
                var _result = new wResult();
                var tmServ = new TimeService();

                var cri = new TimeTransactionCriteria();
                cri.Company_ID = pCompanyID;
                cri.Device_ID = pDeviceId;
                cri.From = pDate;

                var TimeTran = tmServ.LstTimeTransaction(cri);
                var eService = new EmployeeService();
                if (TimeTran != null)
                {
                    foreach (var row in TimeTran)
                    {
                        var _wTran = new wTran();
                        _wTran.Transaction_Date = row.Device_Transaction_Date.Value;
                        _wTran.Transaction_ID = row.Time_Transaction_ID;
                        _wTran.Employee_Profile_ID = row.Employee_Profile_ID.Value;
                        var p = eService.GetEmployeeProfile(row.Employee_Profile_ID.Value);
                        if( p!= null)
                            _wTran.Employee_Name = p.User_Profile.First_Name + " " + p.User_Profile.Last_Name;
                        _wTran.Job_Code = row.Job_Code;
                        _wTran.Transaction_Type = row.Transaction_Type;
                        wTrans.Add(_wTran);
                    }
                    _result.status = "200";
                    _result.message = "Successfully retrieved";
                    _result.DeviceID = pDeviceId;
                    _result.Trans = wTrans;
                }
                else
                {
                    _result.status = "1";
                    _result.message = "Fail";
                    _result.DeviceID = pDeviceId;
                }
                return Json(new { Data = _result }, JsonRequestBehavior.AllowGet);
            }
        #endregion  
    }
}