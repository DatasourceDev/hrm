using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using HR.Models;
using HR.Common;
using SBSModel.Models;
using SBSModel.Common;
using System.Threading.Tasks;
using SBSTimeModel.Models;

namespace HR.Controllers
{
    public class WServController : Controller
    {
        public class wResult
        {
            public string status { get; set; }
            public string message { get; set; }
            public int CompanyID { get; set; }
            public List<wUser> UserInfo { get; set; }
        }
        public class wUser
        {
            public int Profile_ID { get; set; }
            public int Employee_Profile_ID { get; set; }
            public string Name { get; set; }
            public string Email { get; set; }
            public string User_Name { get; set; }
        }

        [AllowAnonymous]
        public async Task<ActionResult> IsAuthenticatedUser(String pUserName, String pPassword)
        {
            var _result = new wResult();
            var _userList = new List<wUser>();
            var uService = new UserService();
            var user = new ApplicationUser();
            var ctr = new AccountController();

            var _user = new wUser();
            user = await ctr.UserManager.FindAsync(pUserName, pPassword);
            if (user != null)
            {
                var prof = uService.getAuthenticatedUser(pUserName);
                if (prof != null)
                {                    
                    _user.Profile_ID = prof.Profile_ID;
                    _user.Employee_Profile_ID = prof.Employee_Profile.Select(s => s.Employee_Profile_ID).FirstOrDefault();
                    _user.Name = prof.First_Name + " " + prof.Middle_Name + " " + prof.Last_Name;
                    _user.Email = prof.Email;
                    _user.User_Name = prof.User_Name;                    
                    _userList.Add(_user);

                    _result.CompanyID = prof.Company_ID.Value;
                    _result.status = "200";
                    _result.message = "Successfully log in";
                    _result.UserInfo = _userList;
                }
                else
                {
                    _result.status = "1";
                    _result.message = "Fail";
                    _result.CompanyID = 0;
                }
            }
            else
            {
                _result.status = "1";
                _result.message = "Fail";
                _result.CompanyID = 0;
            }
            return Json(new { Data = _result }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ListUsers(int CompanyID)
        {
            var _result = new wResult();
            var _userList = new List<wUser>();
            var eService = new EmployeeService();            
            var empList = eService.LstEmployeeProfile(CompanyID);

            if (empList != null)
            {                
                foreach (var row in empList)
                {
                    var _user = new wUser();
                    _user.Profile_ID = row.User_Profile.Profile_ID;
                    _user.Employee_Profile_ID = row.Employee_Profile_ID;
                    _user.Name = row.User_Profile.First_Name + " " + row.User_Profile.Middle_Name + " " + row.User_Profile.Last_Name;
                    _user.Email = row.User_Profile.Email;
                    _user.User_Name = row.User_Profile.User_Name;
                    _userList.Add(_user);
                }
                _result.UserInfo = _userList;

                _result.status = "200";
                _result.message = "Success";
            }
            else
            {
                _result.status = "2";
                _result.message = "Fail";
            }

            return Json(new { Data = _result }, JsonRequestBehavior.AllowGet);
        }

        #region "For RFID reader and Android App"
        public class wservResult
        {
           public string status { get; set; }
           public string message { get; set; }
           public int ID { get; set; }
        }

        public class wtResult
        {
           public string status { get; set; }
           public string message { get; set; }
           public int DeviceID { get; set; }
           public int TransCount { get; set; }
           public List<wtTran> Trans { get; set; }
        }

        public class wtTran
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
           var _result = new wtResult();
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
                 device.Max_Transaction_Id = TimeTrans.Count();
              else
                 device.Max_Transaction_Id = 0;

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
           var _result = new wtResult();
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
           var _result = new wtResult();
           var tmServ = new TimeService();
           var tran = new Time_Transaction();
           var pMap = tmServ.GetTimeDeviceMap(pDeviceID, pTagID);
           if (pMap != null)
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

              var tList = new List<wtTran>();
              var t = new wtTran();
              // to continue here
              var eService = new EmployeeService();
              var p = eService.GetEmployeeProfile(pEmployeeProfileID);
              if (p != null)
                 t.Employee_Name = p.User_Profile.First_Name + " " + p.User_Profile.Last_Name;
              else
                 t.Employee_Name = "";
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

        public ActionResult LstTransaction(int pCompanyID, int pDeviceId, DateTime pDate)
        {
           var wTrans = new List<wtTran>();
           var _result = new wtResult();
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
                 var _wTran = new wtTran();
                 _wTran.Transaction_Date = row.Device_Transaction_Date.Value;
                 _wTran.Transaction_ID = row.Time_Transaction_ID;
                 _wTran.Employee_Profile_ID = row.Employee_Profile_ID.Value;
                 var p = eService.GetEmployeeProfile(row.Employee_Profile_ID.Value);
                 if (p != null)
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
       
       #region "For RFID reader and Android App phase 2"

       /*Register UUID*/
       public ActionResult MbDeviceMapping(string pUUID, int pEID)
        {
           var currentdate = StoredProcedure.GetCurrentDate();
           var _result = new wservResult();
           var tmServ = new TimeMoblieService();
           var empServ = new EmployeeService();
           var emp = empServ.GetEmployeeProfile(pEID);
           if (emp == null)
           {
              _result.status = "-1";
              _result.message = "Employee has not found!";
              return Json(new { Data = _result }, JsonRequestBehavior.AllowGet);
           }
           var map = tmServ.GetMapping(pUUID);
           if (map == null)
           {
              map = new Time_Mobile_Map();
              map.Create_On = currentdate;
              map.Create_By = pUUID;
           }
           map.UUID = pUUID;
           map.Employee_Profile_ID = pEID;
           map.Update_On = currentdate;
           map.Update_By = pUUID;
           var result = tmServ.SaveMapping(map);
           if (result.Code == ERROR_CODE.SUCCESS)
           {
              _result.status = "1";
              _result.message = "The mobile device has been Maped Successfully.";
              _result.ID = map.Map_ID;
           }
           else
           {
              _result.status = "-1";
              _result.message = "Fail";
              _result.ID = -1;
           }

           return Json(new { Data = _result }, JsonRequestBehavior.AllowGet);
        }

       /*Clock In with UUID*/
       public ActionResult MbClockIn(string pUUID)
       {
          var _result = new wservResult();
          var tmServ = new TimeMoblieService();
          var map = tmServ.GetMapping(pUUID);
          if (map == null)
          {
             _result.status = "-1";
             _result.message = "Mobile device mapping has not found!";
             return Json(new { Data = _result }, JsonRequestBehavior.AllowGet);
          }

          var currentdate = StoredProcedure.GetCurrentDate();
          var tran = new Time_Mobile_Trans();
          tran.Map_ID = map.Map_ID;
          tran.Trans_Date = currentdate;
          tran.Clock_In = currentdate.TimeOfDay;
          tran.UUID = pUUID;
          tran.Create_On = currentdate;
          tran.Create_By = pUUID;
          tran.Update_On = currentdate;
          tran.Update_By = pUUID;
          var result = tmServ.ClockIn(tran);
          if (result.Code == ERROR_CODE.SUCCESS)
          {
             _result.status = "1";
             _result.message = "The Transaction has been Clocked in Successfully.";
             _result.ID = tran.Trans_ID;
          }
          else
          {
             _result.status = "-1";
             _result.message = "Fail";
             _result.ID = -1;
          }

          return Json(new { Data = _result }, JsonRequestBehavior.AllowGet);
       }
       #endregion

    }




}