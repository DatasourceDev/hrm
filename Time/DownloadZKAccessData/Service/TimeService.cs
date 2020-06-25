using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Core.Objects;
using System.Data.Entity.SqlServer;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DownloadZKAccessData.Models;
using SBSModel.Common;
using SBSResourceAPI;


namespace DownloadZKAccessData.Service
{
    public class TimeService
    {
        public List<Time_Device> LstZKTimeDeviceFromDB()
        {
            using (var db = new SBS2TimeDBContext())
            {
                var wds = db.Time_Device
                    .Where(w => w.Brand_Name == "ZKAccess" & w.Record_Status != "DELETE");

                return wds.OrderBy(o => o.Device_ID).ToList();
            }
        }
        public ServiceResult UpdateTimeTransactionToDB(Nullable<int> pComID, Nullable<int> pDID, List<ZKAccessManager.Transaction> pTrans)
        {
            var currentdate = DateTime.Today;
            try
            {
                using (var db = new SBS2TimeDBContext())
                {
                    Nullable<int> maxID = null;
                    Nullable<int> minID = null;
                    var i = 0;
                    DateTime dt = getMaxTranDateFromDB(pDID.Value);
                    List<ZK_Users> userList = getAllZKUsersByDevice(pDID.Value);                      
                    foreach (var row in pTrans)
                    {
                        if ((dt < DateTime.MaxValue && row.DateAndTime > dt) || (dt == DateTime.MaxValue))
                        {
                            //if (row.objUser == null)
                            //{
                            //    i++;
                            //    continue;
                            //}
                            var UserName = getUserNameByPin(ref userList, row.Pin);
                            if (UserName == "")
                                UserName = "User-" + row.Pin;
                                
                            Nullable<int> empID = null;
                            var map = db.Time_Device_Map.Where(w => w.Device_Employee_Pin == row.Pin & w.Device_ID == pDID & w.Time_Device.Company_ID == pComID).FirstOrDefault();
                            if (map != null)
                            {
                                empID = map.Employee_Profile_ID;
                                map.Employee_Name = UserName;
                            }

                            db.Time_Transaction.Add(new Time_Transaction()
                            {
                                Device_Transaction_ID = row.ID,
                                Company_ID = pComID,
                                Device_Employee_Pin = row.Pin,
                                Employee_Profile_ID = empID,
                                Employee_Name = UserName,
                                Job_Code = "241", //row.JobCode.ToString(),
                                Device_ID = pDID,
                                Device_Transaction_Date = row.DateAndTime,
                                Transaction_Type = row.TypeName,
                                Update_On = currentdate,
                                Create_On = currentdate,
                                Create_By = "Batch Job",
                                Update_By = "Batch Job"
                            });

                            maxID = row.ID;
                            if (i == 0)
                                minID = row.ID;
                            i++;
                        }
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

        public ServiceResult UpdateZKUsersToDB(Nullable<int> pDID, List<ZKAccessManager.User> pUsers)
        {
            var currentdate = DateTime.Today;
            try
            {
                using (var db = new SBS2TimeDBContext())
                {
                    var i = 0;
                    var j = 0;
                    var oldPin = 0;
                    var newPin = 0;
                    var bInsert = false;
                    foreach (var row in pUsers)
                    {
                        i++;
                        if (i == 1)
                        {
                            oldPin = row.Pin;
                            newPin = row.Pin;
                            bInsert = true;
                        }
                        else
                        {
                            if (oldPin == row.Pin)
                            {
                                bInsert = false;
                            }
                            else
                            {
                                oldPin = row.Pin;
                                bInsert = true;
                            }
                        }
                        if (!isExist_ZKUser(pDID.Value, row.Pin) && bInsert == true)
                        {
                            j++;
                            db.ZK_Users.Add(new ZK_Users()
                            {
                                Enroll_ID = row.Pin,
                                User_Name = row.Name,
                                User_Level = row.Level.ToString(),
                                User_Status = row.Enabled ? "A" : "I",
                                User_Pin = row.Pin,
                                Device_ID = pDID.Value,
                                Create_On = currentdate,
                                Create_By = "Batch Job",
                                Update_On = null,
                                Update_By = null
                            });
                        }                                           
                    }                    
                    db.SaveChanges();                    
                    return new ServiceResult
                    {
                        Code = ERROR_CODE.SUCCESS,
                        Msg_Code = ERROR_CODE.SUCCESS_CREATE,
                        Msg = Resource.Transaction + " " + new Success().getSuccess(ERROR_CODE.SUCCESS_CREATE) + "(" + j + ")",
                        Field = Resource.Transaction
                    };
                }
            }
            catch (Exception ex)
            {
                return new ServiceResult
                {
                    Code = ERROR_CODE.ERROR_503_INSERT_ERROR,
                    Msg = Resource.Transaction + "(Users) has error. " + ex.ToString(),
                    Field = Resource.Transaction
                };
            }
        }

        public DateTime getMaxTranDateFromDB(int DeviceID)
        {
            using (var db = new SBS2TimeDBContext())
            {
                var wds = db.Time_Transaction
                    .Where(w => w.Device_ID == DeviceID);
                
                if (wds.Count().Equals(0))
                    return DateTime.MaxValue;
                return wds.Max(p => p.Device_Transaction_Date).Value;                
            }
        }

        public bool isExist_ZKUser(int DeviceID, int EnrollID)
        {
            using (var db = new SBS2TimeDBContext())
            {                
                var wds = db.ZK_Users
                    .Where(w => w.Device_ID == DeviceID & w.Enroll_ID == EnrollID).FirstOrDefault();

                if (wds == null)
                    return false;
                else
                    return true;                
            }
        }
        public ZK_Users getZKUserByPin(int DeviceID, int EnrollID)
        {
            using (var db = new SBS2TimeDBContext())
            {
                var wds = db.ZK_Users
                    .Where(w => w.Device_ID == DeviceID & w.Enroll_ID == EnrollID).FirstOrDefault();

                return wds;
            }
        }
        public List<ZK_Users> getAllZKUsersByDevice(int DeviceID)
        {
            using (var db = new SBS2TimeDBContext())
            {
                var wds = db.ZK_Users
                    .Where(w => w.Device_ID == DeviceID );

                return wds.OrderBy(o => o.User_Pin).ToList();;
            }
        }
        public String getUserNameByPin(ref List<ZK_Users> ulist, int userPin)
        {
            var UserName = "";
            if (ulist.Count > 0)
            {
                foreach (var u in ulist)
                {
                    if (u.User_Pin == userPin)
                    {
                        return u.User_Name;
                    }
                }
            }
            return UserName;
        }
    }
}
